using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace WorkoutTracker.Services.Email;

/// <summary>
/// Sends emails through SMTP (SendGrid, Gmail, Office 365, etc.).
/// </summary>
public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
    /// </summary>
    /// <param name="options">SMTP configuration.</param>
    /// <param name="logger">Logger instance.</param>
    public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (!_options.IsConfigured)
        {
            _logger.LogError(
                "Email is not configured. Set Email:SmtpHost, Email:FromAddress, Email:SmtpUsername, and Email:SmtpPassword.");
            throw new InvalidOperationException(
                "Email sending is not configured. Contact the administrator or set SMTP settings.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlMessage };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _options.SmtpHost,
            _options.SmtpPort,
            _options.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

        await client.AuthenticateAsync(_options.SmtpUsername, _options.SmtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Sent email to {Recipient} with subject {Subject}", email, subject);
    }
}

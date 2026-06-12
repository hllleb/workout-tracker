namespace WorkoutTracker.Services.Email;

/// <summary>
/// SMTP settings used to send Identity emails (confirmation, password reset).
/// </summary>
public sealed class EmailOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Email";

    /// <summary>
    /// Gets or sets the SMTP host (e.g. smtp.sendgrid.net).
    /// </summary>
    public string? SmtpHost { get; set; }

    /// <summary>
    /// Gets or sets the SMTP port (587 for STARTTLS).
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Gets or sets the SMTP username.
    /// </summary>
    public string? SmtpUsername { get; set; }

    /// <summary>
    /// Gets or sets the SMTP password or API key.
    /// </summary>
    public string? SmtpPassword { get; set; }

    /// <summary>
    /// Gets or sets the sender email address.
    /// </summary>
    public string? FromAddress { get; set; }

    /// <summary>
    /// Gets or sets the sender display name.
    /// </summary>
    public string FromName { get; set; } = "WorkoutTracker";

    /// <summary>
    /// Gets or sets whether STARTTLS is used.
    /// </summary>
    public bool UseStartTls { get; set; } = true;

    /// <summary>
    /// Returns true when all required SMTP settings are present.
    /// </summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(SmtpHost)
        && !string.IsNullOrWhiteSpace(FromAddress)
        && !string.IsNullOrWhiteSpace(SmtpUsername)
        && !string.IsNullOrWhiteSpace(SmtpPassword);
}

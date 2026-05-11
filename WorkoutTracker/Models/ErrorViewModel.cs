namespace WorkoutTracker.Models;

/// <summary>
/// View model for displaying request error details.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the request identifier for diagnostics.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the request identifier should be displayed.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

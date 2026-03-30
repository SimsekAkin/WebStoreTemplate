namespace WebStore.Models;

/// <summary>
/// View model used by the error page.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Current request id used for diagnostics.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// True when <see cref="RequestId"/> has a value and can be shown on UI.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

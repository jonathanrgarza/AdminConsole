namespace admin;

/// <summary>
///     The console host types.
/// </summary>
public enum ConsoleHost
{
    /// <summary>
    ///     No console host and/or parent.
    /// </summary>
    None,

    /// <summary>
    ///     Unknown parent process.
    /// </summary>
    Unknown,

    /// <summary>
    ///     Command Prompt.
    /// </summary>
    Cmd,

    /// <summary>
    ///     Windows PowerShell (5.1).
    /// </summary>
    WindowsPowerShell,

    /// <summary>
    ///     PowerShell Core (6+).
    /// </summary>
    PowerShell
}
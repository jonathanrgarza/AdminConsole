namespace admin.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="ConsoleHost" /> enumeration.
/// </summary>
public static class ConsoleHostExtensions
{
    /// <summary>
    ///     Converts the specified <see cref="ConsoleHost" /> to its corresponding Windows Terminal console host name.
    /// </summary>
    /// <param name="host">The console host to convert.</param>
    /// <returns>The name of the Windows Terminal console host.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified console host is not recognized.</exception>
    public static string ToWtConsoleHost(this ConsoleHost host)
    {
        return host switch
        {
            ConsoleHost.Cmd => "Command Prompt",
            ConsoleHost.WindowsPowerShell => "Windows PowerShell",
            ConsoleHost.PowerShell => "PowerShell",
            _ => throw new ArgumentOutOfRangeException(nameof(host), host, null)
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="ConsoleHost" /> to its corresponding executable name.
    /// </summary>
    /// <param name="host">The console host to convert.</param>
    /// <returns>The executable name of the console host.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified console host is not recognized.</exception>
    public static string ToConsoleHostExecutables(this ConsoleHost host)
    {
        return host switch
        {
            ConsoleHost.Cmd => "cmd.exe",
            ConsoleHost.WindowsPowerShell => "powershell.exe",
            ConsoleHost.PowerShell => "pwsh.exe",
            _ => throw new ArgumentOutOfRangeException(nameof(host), host, null)
        };
    }
}
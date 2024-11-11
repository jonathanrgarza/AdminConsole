using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using admin.Extensions;

namespace admin;

/// <summary>
///     The class representing the program.
/// </summary>
public static class Program
{
    /// <summary>
    ///     A Win32 Error for operation was canceled by the user.
    /// </summary>
    private const int Win32ErrorCancelled = 1223;

    /// <summary>
    ///     The exit code for successful operation.
    /// </summary>
    private const int Success = 0;

    /// <summary>
    ///     The exit code for error during operation.
    /// </summary>
    private const int Error = 1;

    /// <summary>
    ///     Determines if the current process is running as an administrator.
    /// </summary>
    /// <returns>True if running as administrator, otherwise false.</returns>
    public static bool IsRunningAsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();

        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    ///     Gets the current console host and determines if it is within Windows Terminal.
    /// </summary>
    /// <param name="withinWindowsTerminal">Outputs whether the console host is within Windows Terminal.</param>
    /// <returns>The current console host.</returns>
    public static ConsoleHost GetConsoleHost(out bool withinWindowsTerminal)
    {
        using var process = Process.GetCurrentProcess();
        var parentProcess = process.GetParent();
        withinWindowsTerminal = false;

        if (parentProcess == null)
            return ConsoleHost.None;

        var host = ConsoleHost.Unknown;
        try
        {
            while (parentProcess != null)
            {
                switch (parentProcess.ProcessName.ToLower())
                {
                    case "cmd":
                        host = ConsoleHost.Cmd;
                        break;
                    case "powershell":
                        host = ConsoleHost.WindowsPowerShell;
                        break;
                    case "pwsh":
                        host = ConsoleHost.PowerShell;
                        break;
                    case "wt":
                    case "windowsterminal":
                        withinWindowsTerminal = true;
                        break;
                }

                if (host != ConsoleHost.Unknown && withinWindowsTerminal)
                    break;

                var currentParent = parentProcess;
                parentProcess = parentProcess.GetParent();
                currentParent.Dispose();
            }
        }
        finally
        {
            parentProcess?.Dispose();
        }

        return host;
    }

    /// <summary>
    ///     Launches a new Windows Terminal as an administrator with the specified console host.
    /// </summary>
    /// <param name="host">The console host to launch.</param>
    /// <returns>True if the launch was successful, otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when the console host is invalid.</exception>
    public static bool LaunchNewAdminWindowsTerminal(ConsoleHost host)
    {
        if (host is ConsoleHost.None or ConsoleHost.Unknown)
            throw new ArgumentException("Invalid console host.", nameof(host));

        string currentWorkingDirectory = Environment.CurrentDirectory;

        string wtHost = host.ToWtConsoleHost();
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "wt.exe",
            Arguments = $"-w 0 -d \"{currentWorkingDirectory}\" -p \"{wtHost}\"",
            Verb = "runas",
            UseShellExecute = true
        };


        try
        {
            Process.Start(processStartInfo);
        }
        catch (Win32Exception ex)
        {
            Console.WriteLine(ex.NativeErrorCode == Win32ErrorCancelled
                ? "User refused to allow elevation."
                : "Failed to create new console host");
            return false;
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to create new console host");
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Launches a new console host as an administrator.
    /// </summary>
    /// <param name="host">The console host to launch.</param>
    /// <returns>True if the launch was successful, otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when the console host is invalid.</exception>
    public static bool LaunchNewAdminConsoleHost(ConsoleHost host)
    {
        if (host is ConsoleHost.None or ConsoleHost.Unknown)
            throw new ArgumentException("Invalid console host.", nameof(host));

        string currentWorkingDirectory = Environment.CurrentDirectory;

        string hostExecutable = host.ToConsoleHostExecutables();
        var processStartInfo = new ProcessStartInfo
        {
            FileName = hostExecutable,
            WorkingDirectory = currentWorkingDirectory,
            Verb = "runas",
            UseShellExecute = true
        };

        try
        {
            Process.Start(processStartInfo);
        }
        catch (Win32Exception ex)
        {
            Console.WriteLine(ex.NativeErrorCode == Win32ErrorCancelled
                ? "User refused to allow elevation."
                : "Failed to create new console host");
            return false;
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to create new console host");
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Checks if PowerShell Core (pwsh) is installed and available.
    /// </summary>
    /// <returns>True if pwsh is available, otherwise false.</returns>
    public static bool IsPwshAvailable()
    {
        string[] paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? Array.Empty<string>();
        foreach (string path in paths)
        {
            string pwshPath = Path.Combine(path, "pwsh.exe");
            if (File.Exists(pwshPath))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The exit code of the application.</returns>
    public static int Main(string[] args)
    {
        if (IsRunningAsAdministrator() && args.Length <= 0)
        {
            Console.WriteLine("Running as administrator already.");
            return Success;
        }

        if (args.Length > 0) return ProcessCommandLineOptions(args);

        var consoleHost = GetConsoleHost(out bool withinWindowsTerminal);

        if (consoleHost is ConsoleHost.None or ConsoleHost.Unknown)
        {
            Console.WriteLine("No console host detected.");
            return 1;
        }

        if (withinWindowsTerminal) return LaunchNewAdminWindowsTerminal(consoleHost) ? Success : Error;

        return LaunchNewAdminConsoleHost(consoleHost) ? Success : Error;
    }

    /// <summary>
    ///     Processes the command-line options.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The exit code of the application.</returns>
    private static int ProcessCommandLineOptions(string[] args)
    {
        var options = new HashSet<string>(args, StringComparer.OrdinalIgnoreCase);

        if (options.Contains("--help"))
        {
            Console.WriteLine(
                "Usage: admin [--help] [--version] [--wt] [--cmd] [--powershell] [--winpowershell]");
            Console.WriteLine("Launches a new console host as an administrator.");
            return Success;
        }

        if (options.Contains("--version"))
        {
            Console.WriteLine("admin v1.0.0");
            return Success;
        }

        if (options.Contains("--wt"))
        {
            var host = ConsoleHost.WindowsPowerShell; // Default to Windows PowerShell if no other host is specified
            if (IsPwshAvailable())
            {
                host = ConsoleHost.PowerShell;
            }

            if (options.Contains("--cmd"))
                host = ConsoleHost.Cmd;
            else if (options.Contains("--powershell"))
                host = ConsoleHost.PowerShell;
            else if (options.Contains("--winpowershell"))
                host = ConsoleHost.WindowsPowerShell;
            return LaunchNewAdminWindowsTerminal(host) ? Success : Error;
        }

        if (options.Contains("--cmd"))
            return LaunchNewAdminConsoleHost(ConsoleHost.Cmd) ? Success : Error;

        if (options.Contains("--powershell"))
            return LaunchNewAdminConsoleHost(ConsoleHost.PowerShell) ? Success : Error;

        if (options.Contains("--win_powershell"))
            return LaunchNewAdminConsoleHost(ConsoleHost.WindowsPowerShell) ? Success : Error;

        Console.WriteLine($"Invalid argument '{options.First()}'. Use --help for usage information.");
        return Error;
    }
}
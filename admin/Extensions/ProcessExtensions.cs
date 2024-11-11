using System.Diagnostics;
using System.Runtime.InteropServices;

namespace admin.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="Process" /> class.
/// </summary>
public static class ProcessExtensions
{
    /// <summary>
    ///     Gets the parent process of the specified process.
    /// </summary>
    /// <param name="process">The process for which to get the parent process.</param>
    /// <returns>The parent process, or <c>null</c> if the parent process cannot be determined.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the process has exited or is not valid.</exception>
    /// <exception cref="System.ComponentModel.Win32Exception">Thrown when there is an error with the Win32 API call.</exception>
    public static Process? GetParent(this Process process)
    {
        int parentId;
        IntPtr handle = IntPtr.Zero;

        try
        {
            handle = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.QueryInformation, false, process.Id);
            if (handle == IntPtr.Zero)
                return null;

            var pbi = new NativeMethods.PROCESS_BASIC_INFORMATION();
            int returnLength = 0;
            int status =
                NativeMethods.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), ref returnLength);
            if (status != 0)
                return null;

            parentId = pbi.InheritedFromUniqueProcessId.ToInt32();
        }
        finally
        {
            if (handle != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(handle);
            }
        }

        try
        {
            return Process.GetProcessById(parentId);
        }
        catch (ArgumentException)
        {
            //Parent process is not running
            return null;
        }
    }
}
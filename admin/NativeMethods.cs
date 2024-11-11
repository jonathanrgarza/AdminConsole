using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace admin;

/// <summary>
///     Static class containing native methods.
/// </summary>
public static class NativeMethods
{
    /// <summary>
    ///     Specifies the access rights to a process.
    ///     <see href="https://learn.microsoft.com/en-us/windows/win32/procthread/process-security-and-access-rights">Link</see>
    /// </summary>
    [Flags]
    public enum ProcessAccessFlags : uint
    {
        /// <summary>
        ///     Required to retrieve certain information about a process, such as its token, exit code, and priority class.
        /// </summary>
        QueryInformation = 0x400
    }

    /// <summary>
    ///     Retrieves information about the specified process.
    /// </summary>
    /// <param name="processHandle">A handle to the process for which information is to be retrieved.</param>
    /// <param name="processInformationClass">
    ///     The type of process information to be retrieved.
    ///     <see
    ///         href="https://learn.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntqueryinformationprocess">
    ///         Link
    ///     </see>
    ///     .
    /// </param>
    /// <param name="processInformation">
    ///     A pointer to a buffer supplied by the calling application into which the function writes the requested information.
    ///     The size of the information written varies depending on the data type of the ProcessInformationClass parameter.
    /// </param>
    /// <param name="processInformationLength">
    ///     When the ProcessInformationClass parameter is ProcessBasicInformation,
    ///     the buffer pointed to by the ProcessInformation parameter should be large enough to
    ///     hold a single <see cref="PROCESS_BASIC_INFORMATION" /> structure.
    /// </param>
    /// <param name="returnLength">
    ///     A pointer to a variable in which the function returns the size of the requested information.
    ///     If the function was successful, this is the size of the information written to the buffer pointed to by the
    ///     ProcessInformation parameter (if the buffer was too small, this is the minimum size of
    ///     buffer needed to receive the information successfully).
    /// </param>
    /// <returns>
    ///     The function returns an NTSTATUS success (0) or an error code.
    /// </returns>
    [DllImport("ntdll.dll")]
    public static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
        ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength, ref int returnLength);

    /// <summary>
    ///     Opens an existing local process object.
    /// </summary>
    /// <param name="processAccess">
    ///     The access to the process object. This access right is checked against the security descriptor for the process.
    /// </param>
    /// <param name="inheritHandle">
    ///     If this value is TRUE, processes created by this process will inherit the handle.
    ///     Otherwise, the processes do not inherit this handle.
    /// </param>
    /// <param name="processId">
    ///     The identifier of the local process to be opened.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is an open handle to the specified process.
    ///     If the function fails, the return value is <see cref="IntPtr.Zero" />.
    ///     On failure, you can check <see cref="Marshal.GetLastWin32Error" />.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool inheritHandle, int processId);

    /// <summary>
    ///     Closes an open object handle.
    /// </summary>
    /// <param name="objectHandle">A valid handle to an open object.</param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
    ///     On failure, you can check <see cref="Marshal.GetLastWin32Error" />.
    /// </returns>
    /// <remarks>
    ///     The CloseHandle function closes handles to the following objects:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Access token</description>
    ///         </item>
    ///         <item>
    ///             <description>Communications device</description>
    ///         </item>
    ///         <item>
    ///             <description>Console input</description>
    ///         </item>
    ///         <item>
    ///             <description>Console screen buffer</description>
    ///         </item>
    ///         <item>
    ///             <description>Event</description>
    ///         </item>
    ///         <item>
    ///             <description>File</description>
    ///         </item>
    ///         <item>
    ///             <description>File mapping</description>
    ///         </item>
    ///         <item>
    ///             <description>I/O completion port</description>
    ///         </item>
    ///         <item>
    ///             <description>Job</description>
    ///         </item>
    ///         <item>
    ///             <description>Mailslot</description>
    ///         </item>
    ///         <item>
    ///             <description>Memory resource notification</description>
    ///         </item>
    ///         <item>
    ///             <description>Mutex</description>
    ///         </item>
    ///         <item>
    ///             <description>Named pipe</description>
    ///         </item>
    ///         <item>
    ///             <description>Pipe</description>
    ///         </item>
    ///         <item>
    ///             <description>Process</description>
    ///         </item>
    ///         <item>
    ///             <description>Semaphore</description>
    ///         </item>
    ///         <item>
    ///             <description>Thread</description>
    ///         </item>
    ///         <item>
    ///             <description>Transaction</description>
    ///         </item>
    ///         <item>
    ///             <description>Waitable timer</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr objectHandle);

    /// <summary>
    ///     A struct containing information about a process.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_BASIC_INFORMATION
    {
        /// <summary>
        ///     Contains the same value that GetExitCodeProcess returns.
        ///     However, the use of GetExitCodeProcess is preferable for clarity and safety.
        /// </summary>
        public IntPtr ExitStatus;

        /// <summary>
        ///     Points to a <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winternl/ns-winternl-peb">PEB</see>
        ///     structure.
        /// </summary>
        public IntPtr PebBaseAddress;

        /// <summary>
        ///     The Affinity Mask for the process; the same value that GetProcessAffinityMask returns for the lpProcessAffinityMask
        ///     parameter.
        /// </summary>
        public IntPtr AffinityMask;

        /// <summary>
        ///     Contains the process priority as described in
        ///     <see href="https://learn.microsoft.com/en-us/windows/win32/procthread/scheduling-priorities#base-priority">
        ///         Scheduling
        ///         Priorities
        ///     </see>
        ///     .
        /// </summary>
        public IntPtr BasePriority;

        /// <summary>
        ///     A unique identifier for this process.
        /// </summary>
        public IntPtr UniqueProcessId;

        /// <summary>
        ///     A unique identifier for the parent process.
        /// </summary>
        public IntPtr InheritedFromUniqueProcessId;
    }
}
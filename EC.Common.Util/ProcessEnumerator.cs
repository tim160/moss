using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EC.Common.Util
{
    public class ProcEntry
    {
        public string ExeName;
        public uint ID;
    }

    public class ProcessEnumerator
    {
        #region Constants
        private const uint TH32CS_SNAPPROCESS = 0x00000002;
        private const uint TH32CS_SNAPNOHEAPS = 0x40000000;

        private const int MAX_PATH = 260;
        #endregion

        #region Structs
        public struct PROCESSENTRY
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public uint th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szExeFile;
            uint th32MemoryBase;
            uint th32AccessKey;
        }

        public struct MEMORYSTATUS
        {
            public UInt32 dwLength;
            public UInt32 dwMemoryLoad;
            public UInt32 dwTotalPhys;
            public UInt32 dwAvailPhys;
            public UInt32 dwTotalPageFile;
            public UInt32 dwAvailPageFile;
            public UInt32 dwTotalVirtual;
            public UInt32 dwAvailVirtual;
        }

        #endregion

        #region P/Invoke
        [DllImport("toolhelp.dll")]
        private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processID);

        [DllImport("toolhelp.dll")]
        private static extern int CloseToolhelp32Snapshot(IntPtr snapshot);

        [DllImport("toolhelp.dll")]
        private static extern int Process32First(IntPtr snapshot, ref PROCESSENTRY processEntry);

        [DllImport("toolhelp.dll")]
        private static extern int Process32Next(IntPtr snapshot, ref PROCESSENTRY processEntry);


        static Dictionary<string, IntPtr> visibleWindows = new Dictionary<string, IntPtr>();
        static StringBuilder lpString;
        static bool visible;
        static bool hasOwner;
        static bool isToolWindow;
        delegate int WNDENUMPROC(IntPtr hwnd, uint lParam);
        const int GWL_EXSTYLE = -20;
        const uint WS_EX_TOOLWINDOW = 0x0080;
        [DllImport("coredll.dll")]
        static extern int EnumWindows(WNDENUMPROC lpEnumWindow, uint lParam);
        [DllImport("coredll.dll")]
        static extern bool IsWindowVisible(IntPtr hwnd);
        [DllImport("coredll.dll")]
        static extern IntPtr GetParent(IntPtr hwnd);
        [DllImport("coredll.dll")]
        static extern bool GetWindowText(IntPtr hwnd, StringBuilder lpString, int nMaxCount);
        [DllImport("coredll.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("coredll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out int lpdwProcessId);
        [DllImport("coredll.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("CoreDll.dll")]
        public static extern void GlobalMemoryStatus
        (
            ref MEMORYSTATUS lpBuffer
        );

        [System.Runtime.InteropServices.DllImport("CoreDll.dll")]
        public static extern int GetSystemMemoryDivision
        (
            ref UInt32 lpdwStorePages,
            ref UInt32 lpdwRamPages,
            ref UInt32 lpdwPageSize
        );

        #endregion

        #region public Methods

        public static bool KillProcess(string ExeFileName)
        {
            try
            {
                List<ProcEntry> list_ = new List<ProcEntry>();
                if (ProcessEnumerator.Enumerate(ref list_))
                {
                    for (int i = 0; i < list_.Count; i++)
                    {
                        if (list_[i].ExeName == ExeFileName)
                        {
                            ProcessEnumerator.KillProcess(list_[i].ID);
                            break;
                        }
                    }
                }
                list_.Clear();
                list_ = null;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ActivateProcess(string ExeFileName)
        {
            try
            {
                List<ProcEntry> list_ = new List<ProcEntry>();
                if (ProcessEnumerator.Enumerate(ref list_))
                {
                    for (int i = 0; i < list_.Count; i++)
                    {
                        if (list_[i].ExeName == ExeFileName)
                        {
                            ProcessEnumerator.ActivateProcess(list_[i].ID);
                            break;
                        }
                    }
                }
                list_.Clear();
                list_ = null;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsProcessRunning(string ExeFileName)
        {
            try
            {
                List<ProcEntry> list_ = new List<ProcEntry>();
                if (ProcessEnumerator.Enumerate(ref list_))
                {
                    for (int i = 0; i < list_.Count; i++)
                    {
                        if (list_[i].ExeName == ExeFileName)
                        {
                            return true;
                        }
                    }
                }
                list_.Clear();
                list_ = null;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetMemoryInfo()
        {
            UInt32 storePages = 0;
            UInt32 ramPages = 0;
            UInt32 pageSize = 0;
            int res = GetSystemMemoryDivision(ref storePages,
                ref ramPages, ref pageSize);

            // Call the native GlobalMemoryStatus method
            // with the defined structure.
            MEMORYSTATUS memStatus = new MEMORYSTATUS();
            GlobalMemoryStatus(ref memStatus);

            // Use a StringBuilder for the message box string.
            StringBuilder MemoryInfo = new StringBuilder();
            MemoryInfo.Append("Memory Load: "
                + memStatus.dwMemoryLoad.ToString() + "\n");
            MemoryInfo.Append("Total Physical: "
                + memStatus.dwTotalPhys.ToString() + "\n");
            MemoryInfo.Append("Avail Physical: "
                + memStatus.dwAvailPhys.ToString() + "\n");
            MemoryInfo.Append("Total Page File: "
                + memStatus.dwTotalPageFile.ToString() + "\n");
            MemoryInfo.Append("Avail Page File: "
                + memStatus.dwAvailPageFile.ToString() + "\n");
            MemoryInfo.Append("Total Virtual: "
                + memStatus.dwTotalVirtual.ToString() + "\n");
            MemoryInfo.Append("Avail Virtual: "
                + memStatus.dwAvailVirtual.ToString() + "\n");

            return MemoryInfo.ToString();
        }

        #endregion

        #region private Methods

        private static bool Enumerate(ref List<ProcEntry> list)
        {
            try
            {
                if (list == null)
                {
                    return false;
                }
                list.Clear();

                //IntPtr snapshot_ = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
                IntPtr snapshot_ = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS | TH32CS_SNAPNOHEAPS, 0);

                if (snapshot_ == IntPtr.Zero)
                {
                    return false;
                }

                PROCESSENTRY entry_ = new PROCESSENTRY();
                entry_.dwSize = (uint)Marshal.SizeOf(entry_);
                if (Process32First(snapshot_, ref entry_) == 0)
                {
                    CloseToolhelp32Snapshot(snapshot_);
                    return false;
                }

                do
                {
                    ProcEntry procEntry = new ProcEntry();
                    procEntry.ExeName = entry_.szExeFile;
                    procEntry.ID = entry_.th32ProcessID;
                    list.Add(procEntry);
                    entry_.dwSize = (uint)Marshal.SizeOf(entry_);
                }
                while (Process32Next(snapshot_, ref entry_) != 0);

                CloseToolhelp32Snapshot(snapshot_);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool KillProcess(uint procID)
        {
            try
            {
                System.Diagnostics.Process proc_ = System.Diagnostics.Process.GetProcessById((int)procID);
                proc_.Kill();
                proc_.Dispose();
                return true;
            }
            catch (ArgumentException)
            {
                return false; //process does not exist
            }
            catch (Exception)
            {
                return false; //cannot kill process (perhaps its system process)
            }
        }

        private static int Callback(IntPtr hwnd, uint lParam)
        {
            try
            {
                hasOwner = GetParent(hwnd) != IntPtr.Zero;
                visible = IsWindowVisible(hwnd);
                isToolWindow = (GetWindowLong(hwnd, GWL_EXSTYLE) & WS_EX_TOOLWINDOW) != 0;
                lpString.Remove(0, lpString.Length);
                GetWindowText(hwnd, lpString, 1024);
                string key = lpString.ToString();

                if (!hasOwner && visible && !isToolWindow && !string.IsNullOrEmpty(key) && !visibleWindows.ContainsKey(key))
                {
                    visibleWindows.Add(key, hwnd);
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static bool ActivateProcess(uint procID)
        {
            try
            {
                // need to enumerate the windows
                lpString = new StringBuilder(1024);
                EnumWindows(Callback, 0);
                int pid = 0;
                foreach (string key in visibleWindows.Keys)
                {
                    IntPtr hwnd = visibleWindows[key];
                    visible = IsWindowVisible(hwnd);
                    lpString.Remove(0, lpString.Length);
                    GetWindowText(hwnd, lpString, 1024);
                    //Debug.WriteLine("Handle: " + hwnd + "Is Visible: " + visible + "; Text: " + lpString);

                    //get windows process
                    GetWindowThreadProcessId(hwnd, out pid);

                    // if procId matches, then activate the window
                    if ((uint)pid == procID)
                    {
                        SetForegroundWindow(hwnd);
                    }
                }
                return true;
            }
            catch (ArgumentException)
            {
                return false; //process does not exist
            }
            catch (Exception)
            {
                return false;   //cannot activate process (perhaps its hidden)
            }
        }
        #endregion

    }
}

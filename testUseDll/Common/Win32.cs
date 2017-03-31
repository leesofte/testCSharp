using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using HANDLE = System.IntPtr;

namespace testUseDllByCSharp
{
    public enum EventFlags
    {
        PULSE = 1,
        RESET = 2,
        SET = 3
    }
    public enum ModePass
    {
        ByValue = 0x0001,
        ByRef = 0x0002
    }
    public class Win32
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
        public extern static long CopyMemory(IntPtr dest, IntPtr source, int size);  

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenEvent(bool dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("kernel32", EntryPoint = "WaitForSingleObject", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WaitForSingleObject(HANDLE hHandle, int dwMilliseconds);
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool SetEvent(HANDLE hEvent, int dEvent);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        //创建文件映射  
        [DllImport("kernel32.dll", EntryPoint = "CreateFileMapping", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFileMapping(int hFile, IntPtr lpAttribute, uint flProtected, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
        //打开文件映射对象  
        [DllImport("kernel32.dll", EntryPoint = "OpenFileMapping", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, String lpName);
        //把文件数据映射到进程的地址空间  
        [DllImport("Kernel32.dll")]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        //从调用线程的地址空间释放文件数据映像  
        [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
        //关闭一个已经打开的文件句柄  
        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CloseHandle(IntPtr hHandle);
        //获取最后一个错误信息  
        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetLastError();

        //发送消息需要函数  
        [DllImport("User32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "PostMessage", CharSet = CharSet.Unicode)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
        //  
        const int ERROR_ALREADY_EXISTS = 183;
        //OpenFileMapping和MapViewOf函数中，使用的文件访问权限  
        const int FILE_MAP_COPY = 0x0001;
        const int FILE_MAP_WRITE = 0x0002;
        const int FILE_MAP_READ = 0x0004;
        const int FILE_MAP_ALL_ACCESS = 0x0002 | 0x0004;
        //CreateFileMapping函数中，文件映射时，保护属性  
        const int PAGE_READONLY = 0x02;
        const int PAGE_READWRITE = 0x04;
        const int PAGE_WRITECOPY = 0x08;
        const int PAGE_EXECUTE = 0x10;
        const int PAGE_EXECUTE_READ = 0x20;
        const int PAGE_EXECUTE_READWRITE = 0x40;
        //  
        const int INVALID_HANDLE_VALUE = -1;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.IO;
using testUseDll.Complex;
using System.Data;
using System.ComponentModel;
using HANDLE = System.IntPtr;  
namespace testUseDll
{
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public int lpSecurityDescriptor;
        public int bInheritHandle;
    }
    public class ShareMemory
    {
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenEvent(bool dwDesiredAccess, bool bInheritHandle, string lpName);
        //OpenEventW 
        
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool SetEvent(HANDLE hEvent, int dEvent);
        [DllImport("kernel32", EntryPoint = "WaitForSingleObject", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WaitForSingleObject(HANDLE hHandle, int dwMilliseconds);
        public enum EventFlags
        {
            PULSE = 1,
            RESET = 2,
            SET = 3
        }
        private static bool SetEventAPI(HANDLE hEvent)
        {
            return SetEvent(hEvent, (int)EventFlags.SET);
        }
        private static bool PulseEvent(HANDLE hEvent)
        {
            return SetEvent(hEvent, (int)EventFlags.PULSE);
        }
        private static bool ResetEvent(HANDLE hEvent)
        {
            return SetEvent(hEvent, (int)EventFlags.RESET);
        }
        /*
        public int initRead(uint structSize, string fileName)
        {
            IntPtr hShareMemoryHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            //判断参数的合法性  
            if (structSize > 0 && fileName.Length > 0)
            {
                hShareMemoryHandle = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READONLY, 0, (uint)structSize, GLOBAL_MEMORY_OUT_NAME);
                if (hShareMemoryHandle == IntPtr.Zero)
                {
                    return -2;
                }
                else
                {
                    if (ERROR_ALREADY_EXISTS == GetLastError())
                    {
                        return -3;
                    }
                }
                hVoid = MapViewOfFile(hShareMemoryHandle, FILE_MAP_READ, 0, 0, structSize);
                if (hVoid == IntPtr.Zero)
                {
                    CloseHandle(hShareMemoryHandle);
                    return -4;
                }
            }
            m_hReadEvent = CreateEvent(IntPtr.Zero, false, false, GLOBAL_EVENT_OUT_NAME);
            return 0;
        }
        */
        public int initRead()
        {
            IntPtr hMappingHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;
            m_hReadEvent = OpenEvent(true, false, GLOBAL_EVENT_OUT_NAME);
            uint i = GetLastError();
            hMappingHandle = OpenFileMapping((uint)FILE_MAP_READ, false, GLOBAL_MEMORY_OUT_NAME);
            if (hMappingHandle == IntPtr.Zero)
            {
                return 1;
            }
            return 0;
        }
        public int initWrite()
        {
            IntPtr hMappingHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            hMappingHandle = OpenFileMapping((uint)FILE_MAP_WRITE, false, GLOBAL_MEMORY_IN_NAME);
            if (hMappingHandle == IntPtr.Zero)
            {
                return 1;
            }
            m_hWriteEvent = OpenEvent(true, false, GLOBAL_EVENT_IN_NAME);
            return 0;
        }

        /// 读共享内存  
        /// </summary>  
        /// <param name="structSize">需要映射的文件的字节数量</param>  
        /// <param name="type">类型</param>  
        /// <param name="fileName">文件映射对象的名称</param>  
        /// <returns>返回读到的映射字节数据</returns>  
        public byte[] ReadFromMemory(uint structSize, Type type)
        {

            IntPtr hMappingHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            hMappingHandle = OpenFileMapping((uint)FILE_MAP_READ, false, GLOBAL_MEMORY_OUT_NAME);
            if (hMappingHandle == IntPtr.Zero)
            {
                return null;
            }
            hVoid = MapViewOfFile(hMappingHandle, FILE_MAP_READ, 0, 0, structSize);
            if (hVoid == IntPtr.Zero)
            {
                return null;
            }

            //Object obj = Marshal.PtrToStructure(hVoid, type);  
            byte[] bytes = new byte[structSize];
            Marshal.Copy(hVoid, bytes, 0, bytes.Length);

            if (hVoid != IntPtr.Zero)
            {
                UnmapViewOfFile(hVoid);
                hVoid = IntPtr.Zero;
            }
            if (hMappingHandle != IntPtr.Zero)
            {
                CloseHandle(hMappingHandle);
                hMappingHandle = IntPtr.Zero;
            }
            return bytes;
        }

        /// 读共享内存  
        /// </summary>  
        /// <param name="structSize">需要映射的文件的字节数量</param>  
        /// <param name="type">类型</param>  
        /// <param name="fileName">文件映射对象的名称</param>  
        /// <returns>返回读到的映射对象</returns>  
        public Object ReadFromMemoryToObj(uint structSize, Type type)
        {
            WaitForSingleObject(m_hReadEvent, System.Threading.Timeout.Infinite); 
            IntPtr hMappingHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            hMappingHandle = OpenFileMapping(FILE_MAP_ALL_ACCESS, false, GLOBAL_MEMORY_OUT_NAME);
            if (hMappingHandle == IntPtr.Zero)
            {
                return null;
            }
            hVoid = MapViewOfFile(hMappingHandle, FILE_MAP_READ, 0, 0, structSize);
            if (hVoid == IntPtr.Zero)
            {
                return null;
            }

            Object obj = Marshal.PtrToStructure(hVoid, type);
            /*
            if (hVoid != IntPtr.Zero)
            {
                UnmapViewOfFile(hVoid);
                hVoid = IntPtr.Zero;
            }
            if (hMappingHandle != IntPtr.Zero)
            {
                CloseHandle(hMappingHandle);
                hMappingHandle = IntPtr.Zero;
            }
             * */
            ResetEvent(m_hReadEvent);
            return obj;
        }

        /// 写共享内存  
        /// </summary>  
        /// <param name="structSize">需要映射的文件的字节数量</param>  
        /// <param name="obj">映射对象（简单类型、结构体等）</param>  
        /// <param name="fileName">文件映射对象的名称</param>  
        /// <param name="windowName">发送消息的窗口句柄</param>  
        /// <param name="Msg">发送消息</param>  
        /// <returns></returns>  
        public int WriteToMemory(uint structSize, Object obj, string windowName, uint Msg)
        {
            IntPtr hShareMemoryHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            //判断参数的合法性  
            if (structSize > 0)
            {
                hShareMemoryHandle = OpenFileMapping(FILE_MAP_ALL_ACCESS, false, GLOBAL_MEMORY_IN_NAME);
                if (hShareMemoryHandle == IntPtr.Zero)
                {
                    return -2;
                }
                else
                {
                    if (ERROR_ALREADY_EXISTS == GetLastError())
                    {
                        return -3;
                    }
                }
                hVoid = MapViewOfFile(hShareMemoryHandle, FILE_MAP_WRITE, 0, 0, structSize);
                if (hVoid == IntPtr.Zero)
                {
                    CloseHandle(hShareMemoryHandle);
                    return -4;
                }
                Marshal.StructureToPtr(obj, hVoid, false);
            }
            else
            {
                return -1;
            }
            SetEventAPI(m_hWriteEvent);
            return 0;

            /*
            IntPtr hShareMemoryHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            //判断参数的合法性  
            if (structSize > 0 && fileName.Length > 0)
            {
                hShareMemoryHandle = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)structSize, fileName);
                if (hShareMemoryHandle == IntPtr.Zero)
                {
                    return -2;
                }
                else
                {
                    if (ERROR_ALREADY_EXISTS == GetLastError())
                    {
                        return -3;
                    }
                }
                hVoid = MapViewOfFile(hShareMemoryHandle, FILE_MAP_WRITE, 0, 0, structSize);
                if (hVoid == IntPtr.Zero)
                {
                    CloseHandle(hShareMemoryHandle);
                    return -4;
                }
                Marshal.StructureToPtr(obj, hVoid, false);
                //发送消息，通知接收  
                /*
                IntPtr handle = FindWindow(null, windowName.Trim());
                if (handle == IntPtr.Zero)
                {
                    return -5;
                }
                else
                {
                    if (PostMessage(handle, (uint)Msg, 0, 0))
                    {

                    }
                }
                
            }
            else
            {
                return -1;
            }
            SetEvent(m_hWriteEvent);
            return 0;
            */
        }

        /// 写共享内存  
        /// </summary>  
        /// <param name="structSize">需要映射的文件的字节数量</param>  
        /// <param name="obj">映射对象（简单类型、结构体等）</param>  
        /// <param name="fileName">文件映射对象的名称</param>  
        /// <param name="windowName">发送消息的窗口句柄</param>  
        /// <param name="Msg">发送消息</param>  
        /// <returns></returns>  
        public int WriteToMemory(uint structSize, Object obj)
        {
            IntPtr hShareMemoryHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            //判断参数的合法性  
            if (structSize > 0)
            {
                hShareMemoryHandle = OpenFileMapping(FILE_MAP_ALL_ACCESS, false, GLOBAL_MEMORY_IN_NAME);
                if (hShareMemoryHandle == IntPtr.Zero)
                {
                    return -2;
                }
                else
                {
                    if (ERROR_ALREADY_EXISTS == GetLastError())
                    {
                        return -3;
                    }
                }
                hVoid = MapViewOfFile(hShareMemoryHandle, FILE_MAP_WRITE, 0, 0, structSize);
                
                /*if (hVoid == IntPtr.Zero)
                {
                    CloseHandle(hShareMemoryHandle);
                    return -4;
                }
                 * */
                Marshal.StructureToPtr(obj, hVoid, false);
            }
            else
            {
                return -1;
            }
            SetEventAPI(m_hWriteEvent);
            return 0;
            /*
            IntPtr hShareMemoryHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            //判断参数的合法性  
            if (structSize > 0 && fileName.Length > 0)
            {
                hShareMemoryHandle = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)structSize, fileName);
                if (hShareMemoryHandle == IntPtr.Zero)
                {
                    return -2;
                }
                else
                {
                    if (ERROR_ALREADY_EXISTS == GetLastError())
                    {
                        return -3;
                    }
                }
                hVoid = MapViewOfFile(hShareMemoryHandle, FILE_MAP_WRITE, 0, 0, structSize);
                if (hVoid == IntPtr.Zero)
                {
                    CloseHandle(hShareMemoryHandle);
                    return -4;
                }
                Marshal.StructureToPtr(obj, hVoid, false);
            }
            else
            {
                return -1;
            }
            return 0;
             * */
        }

        //创建文件映射  
        [DllImport("kernel32.dll", EntryPoint = "CreateFileMapping", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFileMapping(int hFile, IntPtr lpAttribute, uint flProtected, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
        //打开文件映射对象  
        [DllImport("kernel32.dll", EntryPoint = "OpenFileMapping", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, String lpName);
        //把文件数据映射到进程的地址空间  
        [DllImport("Kernel32.dll")]
        private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        //从调用线程的地址空间释放文件数据映像  
        [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
        //关闭一个已经打开的文件句柄  
        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CloseHandle(IntPtr hHandle);
        //获取最后一个错误信息  
        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint GetLastError();

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

        const string GLOBAL_MEMORY_IN_NAME = "ShareMemoryIN";
        const string GLOBAL_MEMORY_OUT_NAME = "ShareMemoryOUT";
        const string GLOBAL_EVENT_IN_NAME = "ShareMemoryEventIN";
        const string GLOBAL_EVENT_OUT_NAME = "ShareMemoryEventOUT"; 

        private HANDLE m_hReadEvent = IntPtr.Zero;
        private HANDLE m_hWriteEvent = IntPtr.Zero;
    }
}

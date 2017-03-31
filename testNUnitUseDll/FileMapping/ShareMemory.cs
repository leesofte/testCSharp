using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Data;
using System.ComponentModel;
using testUseDllByCSharp;
using System.Threading;
using HANDLE = System.IntPtr;
using ThreadMessaging;

namespace testNUnitUseDll
{
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public int lpSecurityDescriptor;
        public int bInheritHandle;
    }
    public class ShareMemory
    {
        private static bool SetEventAPI(HANDLE hEvent)
        {
            return Win32.SetEvent(hEvent, (int)EventFlags.SET);
        }
        private static bool PulseEvent(HANDLE hEvent)
        {
            return Win32.SetEvent(hEvent, (int)EventFlags.PULSE);
        }
        private static bool ResetEvent(HANDLE hEvent)
        {
            return Win32.SetEvent(hEvent, (int)EventFlags.RESET);
        }
        public int initRead()
        {
            m_hReadEvent = new Semaphore(0, 1, GLOBAL_EVENT_OUT_NAME);//new ProcessSemaphore(GLOBAL_EVENT_OUT_NAME, 0, 1);
            IntPtr hMappingHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;
            uint i = Win32.GetLastError();
            NameEntity nameEntity = new NameEntity();

            hMappingHandle = Win32.CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)Marshal.SizeOf(nameEntity), GLOBAL_MEMORY_OUT_NAME);
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
            m_hWriteEvent[0] = new Semaphore(0, 1, GLOBAL_EVENT_IN1_NAME);
            m_hWriteEvent[1] = new Semaphore(0, 1, GLOBAL_EVENT_IN2_NAME);
            NameEntity nameEntity = new NameEntity();
            hMappingHandle = Win32.CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)Marshal.SizeOf(nameEntity), GLOBAL_MEMORY_IN_NAME);
            if (hMappingHandle == IntPtr.Zero)
            {
                return 1;
            }
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

            hMappingHandle = Win32.OpenFileMapping((uint)FILE_MAP_READ, false, GLOBAL_MEMORY_OUT_NAME);
            if (hMappingHandle == IntPtr.Zero)
            {
                return null;
            }
            hVoid = Win32.MapViewOfFile(hMappingHandle, FILE_MAP_READ, 0, 0, structSize);
            if (hVoid == IntPtr.Zero)
            {
                return null;
            }

            //Object obj = Marshal.PtrToStructure(hVoid, type);  
            byte[] bytes = new byte[structSize];
            Marshal.Copy(hVoid, bytes, 0, bytes.Length);

            if (hVoid != IntPtr.Zero)
            {
                Win32.UnmapViewOfFile(hVoid);
                hVoid = IntPtr.Zero;
            }
            if (hMappingHandle != IntPtr.Zero)
            {
                Win32.CloseHandle(hMappingHandle);
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
            m_hReadEvent.WaitOne();
            IntPtr hMappingHandle = IntPtr.Zero;
            IntPtr hVoid = IntPtr.Zero;

            hMappingHandle = Win32.OpenFileMapping(FILE_MAP_ALL_ACCESS, false, GLOBAL_MEMORY_OUT_NAME);
            if (hMappingHandle == IntPtr.Zero)
            {
                return null;
            }
            hVoid = Win32.MapViewOfFile(hMappingHandle, FILE_MAP_READ, 0, 0, structSize);
            if (hVoid == IntPtr.Zero)
            {
                return null;
            }
            object obj = null;
            unsafe
            {
                //obj = Marshal.PtrToStructure(hVoid, type);  
            }            
            
            if (hVoid != IntPtr.Zero)
            {
                Win32.UnmapViewOfFile(hVoid);
                hVoid = IntPtr.Zero;
            }
            if (hMappingHandle != IntPtr.Zero)
            {
                Win32.CloseHandle(hMappingHandle);
                hMappingHandle = IntPtr.Zero;
            }

            m_hReadEvent.Release();
            m_hWriteEvent[1].Release();
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
                hShareMemoryHandle = Win32.OpenFileMapping(FILE_MAP_ALL_ACCESS, false, GLOBAL_MEMORY_IN_NAME);
                if (hShareMemoryHandle == IntPtr.Zero)
                {
                    return -2;
                }
                else
                {
                    if (ERROR_ALREADY_EXISTS == Win32.GetLastError())
                    {
                        return -3;
                    }
                }
                hVoid = Win32.MapViewOfFile(hShareMemoryHandle, FILE_MAP_WRITE, 0, 0, structSize);
                if (hVoid == IntPtr.Zero)
                {
                    Win32.CloseHandle(hShareMemoryHandle);
                    return -4;
                }
                Marshal.StructureToPtr(obj, hVoid, false);
            }
            else
            {
                return -1;
            }
            m_hWriteEvent[0].Release();
            return 0;
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
                hShareMemoryHandle = Win32.OpenFileMapping(FILE_MAP_ALL_ACCESS, false, GLOBAL_MEMORY_IN_NAME);
                if (hShareMemoryHandle == IntPtr.Zero)
                {
                    return -2;
                }
                else
                {
                    if (ERROR_ALREADY_EXISTS == Win32.GetLastError())
                    {
                        return -3;
                    }
                }
                hVoid = Win32.MapViewOfFile(hShareMemoryHandle, FILE_MAP_WRITE, 0, 0, structSize);
                if (hVoid == IntPtr.Zero)
                {
                    Win32.CloseHandle(hShareMemoryHandle);
                    return -4;
                }
                Marshal.StructureToPtr(obj, hVoid, false);
            }
            else
            {
                return -1;
            }
            m_hWriteEvent[0].Release();
            return 0;       
        }

 
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
        const string GLOBAL_EVENT_IN1_NAME = "ShareMemoryEventIN1";
        const string GLOBAL_EVENT_IN2_NAME = "ShareMemoryEventIN2";
        const string GLOBAL_EVENT_OUT_NAME = "ShareMemoryEventOUT";

        private Semaphore m_hReadEvent = null;
        private Semaphore[] m_hWriteEvent = {null,null};
    }
}

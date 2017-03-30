using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace testUseDll.Complex
{
    public class UseComplexDllByLoadLibrary:UseDllByLoadLibrary
    {
        delegate bool CreateNameEntitysByType(string text,
                                     out IntPtr nameArray,
                                     ref uint arraySize,
                                     int type);
        delegate bool CreateNameEntityByType(out IntPtr namePtr, int type);

        public bool TestComplexDllLoadLibrary()
        {
            Console.WriteLine("Complex:Use DllLoadLibrary, ");
            IntPtr hModule = UseComplexDllByLoadLibrary.LoadLibrary("testCppDll.dll");
            int error = Marshal.GetLastWin32Error();
            //NameEntityType type = NameEntityType.OrganizationName;
            int type = 0;

            if (hModule == IntPtr.Zero) return false;
            {
                //test findNames func                
                CreateNameEntitysByType createNameEntitys = (CreateNameEntitysByType)UseComplexDllByLoadLibrary.GetFunctionAddress(hModule, "CreateNameEntitysByType", typeof(CreateNameEntitysByType));
                string text = "";
                IntPtr arrayPtr = IntPtr.Zero;
                uint arraySize = 0;
                List<NameEntity> nameList = new List<NameEntity>();
                if (createNameEntitys(text, out arrayPtr, ref arraySize, type))
                {
                    NameEntity[] names = new NameEntity[arraySize];
                    IntPtr cur = arrayPtr;
                    for (int i = 0; i < arraySize; i++)
                    {
                        names[i] = new NameEntity();
                        //cur->names, free cur
                        Marshal.PtrToStructure(cur, names[i]);
                        Marshal.DestroyStructure(cur, typeof(NameEntity));
                        cur = (IntPtr)((int)cur + Marshal.SizeOf(names[i]));
                    }
                    Marshal.FreeCoTaskMem(arrayPtr);
                    nameList.AddRange(names);
                }
                //createName
                CreateNameEntityByType createNameEntity = (CreateNameEntityByType)UseComplexDllByLoadLibrary.GetFunctionAddress(hModule, "CreateNameEntityByType", typeof(CreateNameEntityByType));
                IntPtr namePtr = IntPtr.Zero;
                if (createNameEntity(out namePtr, type))
                {
                    NameEntity retStruct = (NameEntity)Marshal.PtrToStructure(namePtr, typeof(NameEntity));
                    // 在非托管代码中使用CoTaskMemAlloc分配的内存，可以使用Marshal.FreeCoTaskMem方法释放
                    Marshal.FreeCoTaskMem(namePtr);
                    Console.WriteLine("非托管函数返回的结构体数据：double = {0:f6}, int = {1}",
                        retStruct._score, retStruct._highlightLength);

                    AppDomain appDomain = AppDomain.CurrentDomain;
                    appDomain.SetData("createNameEntity", retStruct);
                }
            }
            UseComplexDllByLoadLibrary.FreeLibrary(hModule);
            Console.WriteLine("Complex: Test END\n");
            return true;
        }

        static void PrintCurrentProcessMemUsage()
        {
            Process ps = Process.GetCurrentProcess();
            {
                PerformanceCounter pf1 = new PerformanceCounter("Process", "Working Set - Private", ps.ProcessName);
                PerformanceCounter pf2 = new PerformanceCounter("Process", "Working Set", ps.ProcessName);
                Console.WriteLine("{0}:{1}  {2:N}MB", ps.ProcessName, "工作集(进程类)", ps.WorkingSet64 / 1024 / 1024);
                Console.WriteLine("{0}:{1}  {2:N}MB", ps.ProcessName, "工作集        ", pf2.NextValue() / 1024 / 1024);
                //私有工作集
                Console.WriteLine("{0}:{1}  {2:N}MB", ps.ProcessName, "私有工作集    ", pf1.NextValue() / 1024 / 1024);
            }
        }

        public bool TestComplexDllLoadLibraryByParam(IntPtr nameEntityPtr)
        {
            Console.WriteLine("Complex:Use DllLoadLibrary, ");
            IntPtr hModule = UseComplexDllByLoadLibrary.LoadLibrary("testCppDll.dll");
            int error = Marshal.GetLastWin32Error();
            //NameEntityType type = NameEntityType.OrganizationName;
            int type = 0;

            if (hModule == IntPtr.Zero) return false;
            {
                //test findNames func                
                CreateNameEntitysByType createNameEntitys = (CreateNameEntitysByType)UseComplexDllByLoadLibrary.GetFunctionAddress(hModule, "CreateNameEntitysByType", typeof(CreateNameEntitysByType));
                string text = "";
                IntPtr arrayPtr = IntPtr.Zero;
                uint arraySize = 0;
                List<NameEntity> nameList = new List<NameEntity>();
                if (createNameEntitys(text, out arrayPtr, ref arraySize, type))
                {
                    NameEntity[] names = new NameEntity[arraySize];
                    IntPtr cur = arrayPtr;
                    for (int i = 0; i < arraySize; i++)
                    {
                        names[i] = new NameEntity();
                        //cur->names, free cur
                        Marshal.PtrToStructure(cur, names[i]);
                        Marshal.DestroyStructure(cur, typeof(NameEntity));
                        cur = (IntPtr)((int)cur + Marshal.SizeOf(names[i]));
                    }
                    Marshal.FreeCoTaskMem(arrayPtr);
                    nameList.AddRange(names);
                }
                //createName
                CreateNameEntityByType createNameEntity = (CreateNameEntityByType)UseComplexDllByLoadLibrary.GetFunctionAddress(hModule, "CreateNameEntityByType", typeof(CreateNameEntityByType));
                IntPtr namePtr = IntPtr.Zero;
                if (createNameEntity(out namePtr, type))
                {
                    NameEntity retStruct = (NameEntity)Marshal.PtrToStructure(namePtr, typeof(NameEntity));
                    // 在非托管代码中使用CoTaskMemAlloc分配的内存，可以使用Marshal.FreeCoTaskMem方法释放
                    Marshal.FreeCoTaskMem(namePtr);
                    Console.WriteLine("非托管函数返回的结构体数据：double = {0:f6}, int = {1}",
                        retStruct._score, retStruct._highlightLength);

                    AppDomain appDomain = AppDomain.CurrentDomain;
                    appDomain.SetData("createNameEntity", retStruct);
                }


                NameEntity paramStruct = (NameEntity)Marshal.PtrToStructure(nameEntityPtr, typeof(NameEntity));
                paramStruct._type = NameEntityType.OrganizationName;
                Marshal.StructureToPtr(paramStruct, nameEntityPtr,false);
                //在非托管代码中使用CoTaskMemAlloc分配的内存，可以使用Marshal.FreeCoTaskMem方法释放
            }
            UseComplexDllByLoadLibrary.FreeLibrary(hModule);
            Console.WriteLine("Complex: Test END\n");
            return true;
        }
    }
}

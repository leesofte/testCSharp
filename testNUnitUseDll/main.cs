using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows.Forms;
using WcfSvcTest;
using HANDLE = System.IntPtr;

/************************************************************************
 * Initial version for C# invoke C++ lib                                
 * 1, by dynamic load dll using LoadLibrary 
 * 2, by static load dll
 * 3, by dynamic load dll using LoadLibrary and by reflection(later finish)
 * 
 * to-do list;
 * 1, dll(library), not .def 
 * 2, input and output param be struct or other Complex dataType
 * 3, loadlibrary and reflection 
 * 4, Test Framework 
 * by lpw 20170317
 * 
 ***********************************************************************
 */
using testUseDll.Complex;
using testAppDomain;

namespace testUseDll
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [Serializable]
    public class Message
    {
        public Message()
        {

        }
        public string flags
        {
            get;
            set;
        }      
    }
    public enum EventFlags
    {
        PULSE = 1,
        RESET = 2,
        SET = 3
    }
    public class TestUseDll
    {
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenEvent(bool dwDesiredAccess, bool bInheritHandle, string lpName);
        //OpenEventW 

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool SetEvent(HANDLE hEvent, int dEvent);

        delegate double Add(double a, double b);
        delegate bool InitFunc(string resourcePath);
        delegate bool CreateNameEntitysByType(string text,
                                             out IntPtr nameArray,
                                             ref uint arraySize,
                                             int type);
        delegate bool CreateNameEntityByType(out IntPtr namePtr, int type);
        const string WCF_EVENT_NAME = "Global\\WCF";
        const string WCF_SEM_NAME = "Global\\WCF_SEM";

        static void TestComplexDllStatic()
        {
            Console.WriteLine("Complex: Use StaticDllImport,");
            PrintCurrentProcessMemUsage();
            UseComplexDllByStatic instanceUseComplexDllByStatic = new UseComplexDllByStatic();

            bool isInit = instanceUseComplexDllByStatic.Initialize(@".\data");
            if (isInit)
            {
                List<NameEntity> nameResults = new List<NameEntity>();
                string text = "";
                bool isSuccessFindNames = instanceUseComplexDllByStatic.CreateNameEntitys(text, ref nameResults);

                NameEntity nameEntity = new NameEntity();
                bool isSuccessCreateName = instanceUseComplexDllByStatic.CreateNameEntity(ref nameEntity);
            }
            ClearMemory();
            PrintCurrentProcessMemUsage();
            Console.WriteLine("Complex: Test END\n");
        }

        static void TestComplexDllLoadLibrary()
        {
            Console.WriteLine("Complex:Use DllLoadLibrary, ");
            PrintCurrentProcessMemUsage();
            IntPtr hModule = UseComplexDllByLoadLibrary.LoadLibrary("testCppDll.dll");
            int error = Marshal.GetLastWin32Error();
            //NameEntityType type = NameEntityType.OrganizationName;
            int type = 0;
            if (hModule == IntPtr.Zero) return;
            {                
                //test findNames func
                CreateNameEntitysByType createNameEntitys = (CreateNameEntitysByType)UseComplexDllByLoadLibrary.GetFunctionAddress(hModule, "CreateNameEntitysByType", typeof(CreateNameEntitysByType));
                string text = "";
                IntPtr arrayPtr = IntPtr.Zero;
                uint arraySize = 0;
                List<NameEntity>  nameList = new List<NameEntity>();
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
                }

            }
            UseComplexDllByLoadLibrary.FreeLibrary(hModule);
            //ClearMemory();
            PrintCurrentProcessMemUsage();
            Console.WriteLine("Complex: Test END\n");
        }

        static void TestSimpleDllStatic() 
        {
            Console.WriteLine("Simple:Use StaticImportDll,");
            unsafe 
            {
                double ret = UseSimpleDllByStatic.Add(1.0, 2.0);
                Console.WriteLine(ret);
            }
            Console.WriteLine("Simple:Test END\n");
        }

        static void TestSimpleDllLoadLibrary()
        {
            Console.WriteLine("Simple:Use DllLoadLibrary,");
            IntPtr hModule = UseDllByLoadLibrary.LoadLibrary("testCppDll.dll");
            int error = Marshal.GetLastWin32Error();
            if (hModule == IntPtr.Zero) return;
            unsafe
            {
                Add addFunction = (Add)UseDllByLoadLibrary.GetFunctionAddress(hModule, "Add", typeof(Add));
                error = Marshal.GetLastWin32Error();
                Console.WriteLine(addFunction(1.0, 2.0));
            }
            UseDllByLoadLibrary.FreeLibrary(hModule);
            Console.WriteLine("Simple:Test END\n");
        }

        //not success
        static void TestComplexDllReflection()
        {
            Console.WriteLine("Complex:Use DllLoadLibrary&Reflection,");
            PrintCurrentProcessMemUsage();
            UseDllByReflection reflectFun = new UseDllByReflection();
            reflectFun.LoadDll("testCppDll.dll");
            reflectFun.LoadFun("CreateNameEntityByTypeByReturn");

            //NameEntity a = new NameEntity();
            int a = 0;
            unsafe
            {
                object[] Parameters = new object[] { a};
                Type[] ParameterTypes = new Type[] { typeof(int)};
                ModePass[] themode = new ModePass[] { ModePass.ByRef };
                Type Type_Return = typeof(IntPtr);
                string methodName1 = "CreateNameEntityByTypeByReturn";
                object ret = reflectFun.Invoke(methodName1, Parameters, ParameterTypes, themode, Type_Return);
                IntPtr retPtr = (IntPtr)ret;
                NameEntity retStruct = (NameEntity)Marshal.PtrToStructure(retPtr, typeof(NameEntity));
                //在非托管代码中使用CoTaskMemAlloc分配的内存，可以使用Marshal.FreeCoTaskMem方法释放
                Marshal.FreeCoTaskMem(retPtr);
                Console.WriteLine("非托管函数返回的结构体数据：double = {0:f6}, int = {1}",
                    retStruct._score, retStruct._highlightLength);             
            }
            reflectFun.UnLoadDll();

            reflectFun.LoadDll("testCppDll.dll");
            reflectFun.LoadFun("CreateNameEntityByType");

            NameEntity nameEntity = new NameEntity();
            IntPtr paraPtr = Marshal.AllocHGlobal(Marshal.SizeOf(nameEntity));
            Marshal.StructureToPtr(nameEntity, paraPtr, false);
            uint b = 0;
            unsafe
            {
                object[] Parameters = new object[] { paraPtr, b };
                Type[] ParameterTypes = new Type[] { typeof(IntPtr),typeof(uint) };
                ModePass[] themode = new ModePass[] { ModePass.ByRef,ModePass.ByValue };
                Type Type_Return = typeof(bool);
                string methodName2 = "CreateNameEntityByType";
                object ret = reflectFun.Invoke(methodName2, Parameters, ParameterTypes, themode, Type_Return);
                NameEntity retStruct = (NameEntity)Marshal.PtrToStructure(paraPtr, typeof(NameEntity));
                //在非托管代码中使用CoTaskMemAlloc分配的内存，可以使用Marshal.FreeCoTaskMem方法释放
                Marshal.FreeCoTaskMem(paraPtr);
                Console.WriteLine("非托管函数返回的结构体数据：double = {0:f6}, int = {1}",
                    retStruct._score, retStruct._highlightLength);   
            }
            reflectFun.UnLoadDll();
            PrintCurrentProcessMemUsage();
            Console.WriteLine("Complex: End");
        }

        static void TestComplexDllMemUsage()
        {
            Console.WriteLine("DllMemUsage:Use Arg and ReturnType,");
            UseComplexDllByStatic instance = new UseComplexDllByStatic();
            instance.TestReturnStructByNew();
            instance.TestReturnStructByArg();
            Console.WriteLine("DllMemUsage:Test END\n");
        }

        static void PrintCurrentProcessMemUsage()
        {
            Process ps = Process.GetCurrentProcess();
            {
                PerformanceCounter pf1 = new PerformanceCounter("Process", "Working Set - Private", ps.ProcessName);
                PerformanceCounter pf2 = new PerformanceCounter("Process", "Working Set", ps.ProcessName);
                Console.WriteLine("{0}:{1}  {2:N}MB", ps.ProcessName, "工作集(进程类)", ps.WorkingSet64 / 1024/1024);
                Console.WriteLine("{0}:{1}  {2:N}MB", ps.ProcessName, "工作集        ", pf2.NextValue() / 1024 / 1024);
                //私有工作集
                Console.WriteLine("{0}:{1}  {2:N}MB", ps.ProcessName, "私有工作集    ", pf1.NextValue() / 1024 / 1024);
            }
        }

        static void TestUnloadDllBySecondAppDomain()
        {
            Console.WriteLine("Complex:Use SecondAppDomain");
            PrintCurrentProcessMemUsage();
            using (AppDomainDynamicLoader domainLoader = new AppDomainDynamicLoader(@"testUseDllByCSharp.dll"))
            {
                //construct param
                NameEntity nameEntityTemp = new NameEntity();
                IntPtr structPtr = Marshal.AllocHGlobal(Marshal.SizeOf(nameEntityTemp));
                Marshal.StructureToPtr(nameEntityTemp, structPtr, false);

                object[] Parameters = new object[] { structPtr };
                //appdomain use SetData And GetData
                domainLoader.InvokeMethod("testUseDll.Complex.UseComplexDllByLoadLibrary", "TestComplexDllLoadLibraryByParam", Parameters);
                NameEntity nameEntity = domainLoader.appDomain.GetData("createNameEntity") as NameEntity;
                
                //review param 
                NameEntity nameEntityTempReview = (NameEntity)Marshal.PtrToStructure(structPtr,typeof(NameEntity));
                Marshal.FreeHGlobal(structPtr);
            }
            PrintCurrentProcessMemUsage();
            Console.WriteLine("Complex:End");
        }

        static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void TestComplexDllByProcess()
        {
            /*
            //Create a new process info structure.
            ProcessStartInfo pInfo = new ProcessStartInfo();
            //Set the file name member of the process info structure.
            pInfo.FileName = "testLeakConsoleExe.exe";
            //Start the process.
            Process p = Process.Start(pInfo);
            */
            NameEntity nameEntity = new NameEntity();
            ShareMemory sm = new ShareMemory();
            sm.initRead();
            sm.initWrite();
            nameEntity._type = NameEntityType.OrganizationName;
            // ShareMemory.WriteToMemory((uint)Marshal.SizeOf(nameEntity), nameEntity, "Global\\ShareMemory");
            Message msg = new Message();
            sm.WriteToMemory((uint)Marshal.SizeOf(msg), msg);
            object obj = (NameEntity)sm.ReadFromMemoryToObj((uint)Marshal.SizeOf(nameEntity), typeof(NameEntity));
            if (obj == null)
            {
                return;
            }
        }

        static void TestComplexDllByWCF()
        {
            /*
            //Create a new process info structure.
            ProcessStartInfo pInfo = new ProcessStartInfo();
            //Set the file name member of the process info structure.
            pInfo.FileName = "WcfServiceHost.exe";
            //Start the process.
            Process p = Process.Start(pInfo);*/
            //HANDLE m_hEvent = OpenEvent(true, false, WCF_EVENT_NAME);
            Console.WriteLine("begin StandardSynchronousTests");
            PrintCurrentProcessMemUsage();
            Semaphore m_Semaphore = Semaphore.OpenExisting(WCF_SEM_NAME);
            var sst = new StandardSynchronousTests();
            sst.DoTest();
            m_Semaphore.Release();
            PrintCurrentProcessMemUsage();            
            Console.WriteLine("end StandardSynchronousTests");
            //SetEvent(m_hEvent, (int)EventFlags.SET);
        }

        [STAThread]
        static void Main(string[] args)
        {
            TestComplexDllByWCF();

            //test Unload AppDomain
            TestUnloadDllBySecondAppDomain();

            //test memUsage
            TestComplexDllMemUsage();

            //test Complex dll static
            TestComplexDllStatic();

            //test complex dll by loadlibrary
            TestComplexDllLoadLibrary();

            //test complex dll by process,by mem mapping
            //TestComplexDllByProcess();

            //test SimpleDll by Static and Loadlibrary
            //TestSimpleDllStatic();
            //TestSimpleDllLoadLibrary();

            //test complex dll by reflection
            TestComplexDllReflection();
            Console.Read();

            /*
            ProcessStartInfo pInfo = new ProcessStartInfo();
            //Set the file name member of the process info structure.
            pInfo.FileName = "testLeakConsoleExe.exe";
            //Start the process.
            Process p = Process.Start(pInfo);

            //Wait for the process to end.
            p.WaitForExit(3000);
            */

            //use to test wndproc for process communication
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MessageWindow());
        }
    }
}

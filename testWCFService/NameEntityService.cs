using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.InteropServices;
using testUseDll.Complex;
using testWCFService.Interface;

namespace testWCFService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,UseSynchronizationContext = false)]
    public class NameEntityService : INameEntityService, IDisposable
    {
        delegate bool CreateNameEntityByTypeDelegate(out IntPtr namePtr, int type);
        //private IntPtr hModule = IntPtr.Zero;

        public NameEntityService()
        {
            
        }
        public void Dispose()
        {
            Console.WriteLine("xxxxxxxxxx");
        }

        ~NameEntityService()
        {

        }

        public bool CreateNameEntityByType(int value, IntPtr namePtr)
        {
            int type = 0;
            IntPtr hModule = UseComplexDllByLoadLibrary.LoadLibrary("testCppDll.dll");
            CreateNameEntityByTypeDelegate createNameEntity = (CreateNameEntityByTypeDelegate)UseComplexDllByLoadLibrary.GetFunctionAddress(hModule, "CreateNameEntityByType", typeof(CreateNameEntityByTypeDelegate));
            //IntPtr namePtr = IntPtr.Zero;
            if (createNameEntity(out namePtr, type))
            {
                NameEntity retStruct = (NameEntity)Marshal.PtrToStructure(namePtr, typeof(NameEntity));
                // 在非托管代码中使用CoTaskMemAlloc分配的内存，可以使用Marshal.FreeCoTaskMem方法释放
                //Marshal.FreeCoTaskMem(namePtr);
                Console.WriteLine("非托管函数返回的结构体数据：double = {0:f6}, int = {1}",
                    retStruct._score, retStruct._highlightLength);
            }
            UseComplexDllByLoadLibrary.FreeLibrary(hModule);
            return true;
        }

        public bool CreateNameEntityByTypeByParam(int value, ref NameEntity nameEntity)
        {
            int type = 0;
            IntPtr hModule = UseComplexDllByLoadLibrary.LoadLibrary("testCppDll.dll");
            CreateNameEntityByTypeDelegate createNameEntity = (CreateNameEntityByTypeDelegate)UseComplexDllByLoadLibrary.GetFunctionAddress(hModule, "CreateNameEntityByType", typeof(CreateNameEntityByTypeDelegate));
            IntPtr namePtr = IntPtr.Zero;
            if (createNameEntity(out namePtr, type))
            {
                nameEntity = (NameEntity)Marshal.PtrToStructure(namePtr, typeof(NameEntity));
                // 在非托管代码中使用CoTaskMemAlloc分配的内存，可以使用Marshal.FreeCoTaskMem方法释放
                Marshal.FreeCoTaskMem(namePtr);
                Console.WriteLine("非托管函数返回的结构体数据：double = {0:f6}, int = {1}",
                    nameEntity._score, nameEntity._highlightLength);
            }
            UseComplexDllByLoadLibrary.FreeLibrary(hModule);
            return true;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}

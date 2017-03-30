using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace testUseDll.Complex
{
    public class UseComplexDllByStatic:IDisposable
    {
        private const string _dllName = "testCppDll.dll";
        // 创建对象，并返回一个句柄
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        private extern static NameFinderSafeHandle CreateNameFinderInstance(NameEntityType type);

        // 初始化对象，比如一些模型和运行时依赖的数据
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private extern static bool Initialize(NameFinderSafeHandle hHandle, string resourcePath);

        // 根据给定的一段文字，返回其中包含的各种名字
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private extern static bool CreateNameEntitys(
            NameFinderSafeHandle hHandle, 
            string text,
            out IntPtr nameArray,
            ref uint arraySize);

        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private extern static bool CreateNameEntity(
            NameFinderSafeHandle hHandle,  out IntPtr namePtr);

        // 释放对象资源
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        private extern static void UnInitialize(IntPtr hHandle);

        // 销毁对象
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        private extern static void DestroyInstance(IntPtr hHandle);

        private class NameFinderSafeHandle : SafeNativeHandle
        {
            protected override bool ReleaseHandle()
            {
                UseComplexDllByStatic.UnInitialize(handle);
                UseComplexDllByStatic.DestroyInstance(handle);
                SetHandle(IntPtr.Zero);
                return true;
            }
        }

        private NameFinderSafeHandle _handle;

        public UseComplexDllByStatic(NameEntityType type)
        {
            _handle = CreateNameFinderInstance(type);
        }

        public UseComplexDllByStatic()
        {
            NameEntityType allType = NameEntityType.OrganizationName | NameEntityType.PersonName | NameEntityType.PlaceName;
            _handle = CreateNameFinderInstance(allType);
        }

        public bool Initialize(string resourcePath)
        {
            return Initialize(_handle, resourcePath);
        }

        public bool CreateNameEntity(ref NameEntity nameEntity)
        {
            IntPtr namePtr = IntPtr.Zero;
            if (_handle.IsInvalid)
            {
                return false;
            }
            if (CreateNameEntity(_handle,out namePtr))
            {
                Marshal.PtrToStructure(namePtr, nameEntity);
                Marshal.DestroyStructure(namePtr, typeof(NameEntity));
                Marshal.FreeCoTaskMem(namePtr);
            }
            return true;
        }

        public bool CreateNameEntitys(string text, ref List<NameEntity> nameList)
        {
            nameList = new List<NameEntity>();

            IntPtr arrayPtr = IntPtr.Zero;
            uint arraySize = 0;
            if (_handle.IsInvalid)
            {
                return false;
            }
            //CoTaskMem, FreeCoTaskMem
            if (CreateNameEntitys(_handle, text, out arrayPtr, ref arraySize))
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

                return true;
            }
            else
            {
                return false;
            }
        }

        // IDisposable接口的实现
        #region IDisposable Members

        public void Dispose()
        {
            _handle.Dispose();
        }
        #endregion

        #region Test Mem Usage
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr TestReturnNewStruct();
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        private extern static void FreeStruct(IntPtr pStruct);

        public void TestReturnStructByNew()
        {
            IntPtr pStruct = TestReturnNewStruct();
            NameEntity retStruct =
                (NameEntity)Marshal.PtrToStructure(pStruct, typeof(NameEntity));
            // 在非托管代码中使用new/malloc分配的内存，
            // 需要调用对应的释放内存的释放方法将其释放掉
            FreeStruct(pStruct);
            Console.WriteLine("非托管函数返回的结构体数据：double = {0:f6}, int = {1}",
                retStruct._score, retStruct._highlightLength);
        }

        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        private extern static void TestReturnStructFromArg(ref IntPtr pStruct);
        public void TestReturnStructByArg()
        {
            IntPtr ppStruct = IntPtr.Zero;
            TestReturnStructFromArg(ref ppStruct);
            NameEntity retStruct =
                (NameEntity)Marshal.PtrToStructure(ppStruct, typeof(NameEntity));
            // 在非托管代码中使用CoTaskMemAlloc分配的内存，可以使用Marshal.FreeCoTaskMem方法释放
            Marshal.FreeCoTaskMem(ppStruct);
            Console.WriteLine("非托管函数返回的结构体数据：double = {0:f6}, int = {1}",
                retStruct._score, retStruct._highlightLength);
        }
        #endregion
    }
}

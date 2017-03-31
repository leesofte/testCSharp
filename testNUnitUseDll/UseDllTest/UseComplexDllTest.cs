using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using NUnit.Framework;
using testUseDllByCSharp;

namespace testNUnitUseDll
{
    [TestFixture]
    public class UseComplexDllTest
    {
        private delegate bool InitFunc(string resourcePath);
        private delegate bool FindNames(string text, 
                                out IntPtr nameArray,
                                out uint arraySize);

        [Test]
        public void TestComplexLoadLibrary()
        {
            IntPtr hModule = Win32.LoadLibrary("testCppDll.dll");
            int error = Marshal.GetLastWin32Error();
            //Console.WriteLine("The Last Win32 Error was: " + error);
            if (hModule == IntPtr.Zero) return;
            unsafe
            {
                InitFunc initFunc = (InitFunc)UseDllByLoadLibrary.GetFunctionAddress(hModule, "Initialize", typeof(InitFunc));
                bool isInit = initFunc("");
                if(isInit)
                {
                    string text = "";
                    FindNames findNamesFunc = (FindNames)UseDllByLoadLibrary.GetFunctionAddress(hModule, "FindNames", typeof(FindNames));
                    IntPtr arrayPtr = IntPtr.Zero;
                    uint arraySize;
                    if (findNamesFunc(text, out arrayPtr, out arraySize))
                    {
                        NameEntity[] names = new NameEntity[arraySize];
                        IntPtr cur = arrayPtr;
                        for (int i = 0; i < arraySize; i++)
                        {
                            names[i] = new NameEntity();
                            Marshal.PtrToStructure(cur, names[i]);
                            Marshal.DestroyStructure(cur, typeof(NameEntity));
                            cur = (IntPtr)((int)cur + Marshal.SizeOf(names[i]));
                        }
                        Assert.AreEqual(names.Count(), arraySize);
                    }
                    Assert.AreNotEqual(arrayPtr, IntPtr.Zero);
                    Marshal.FreeCoTaskMem(arrayPtr);
                }
            }
            Win32.FreeLibrary(hModule);
        }

        [Test]
        public void TestComplexDllImport()
        {
            NameEntityType allType = NameEntityType.OrganizationName | NameEntityType.PersonName | NameEntityType.PlaceName;
            UseComplexDllByStatic nameFinder = new UseComplexDllByStatic(allType);
            
            //param is not didn't use
            bool isInit = nameFinder.Initialize(@".\data");
            if (isInit)
            {
                List<NameEntity> nameResults = new List<NameEntity>();
                //param text is not didn't use
                string text = "";
                bool isSuccess = nameFinder.CreateNameEntitys(text, ref nameResults);
                Assert.AreEqual(isSuccess, true);
            }
        }
    }
}

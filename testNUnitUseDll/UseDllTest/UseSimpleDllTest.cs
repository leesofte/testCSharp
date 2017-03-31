using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using NUnit.Framework;

/************************************************************************
 * Initial version for C# invoke C++ lib                                
 * 1, By LoadLibrary 
 * 2, By dllimport 
 * 3, By LoadLibrary and Reflection
 * 4, By New AppDomain and LoadLibrary 
 * 5, By New Process(Common) to invoke C++
 * 6, By New Process(WCF) to invoke C++
 * 
 * by lpw 20170317
 * 
 ***********************************************************************
 */
using testUseDllByCSharp;

namespace testNUnitUseDll
{
    [TestFixture]
    public class UseSimpleDllTest
    {
        public delegate double Add(double a, double b);
        private Add addFunction = null;
        private IntPtr hModule = IntPtr.Zero;

        [SetUp]
        public void Setup()
        {
            IntPtr hModule = Win32.LoadLibrary("testCppDll.dll");
            int error = Marshal.GetLastWin32Error();
            //Console.WriteLine("The Last Win32 Error was: " + error);
            if (hModule == IntPtr.Zero) return;
            addFunction = (Add)UseComplexDllByLoadLibrary.GetFunctionAddress(hModule, "Add", typeof(Add));
        }

        [TearDown]
        public void TearDown()
        {
            Win32.FreeLibrary(hModule);
        }

        [Test]
        public void TestSimpleLoadLibrary()
        {
            unsafe
            {
                double ret = addFunction(1.0, 2.0);
                Assert.AreEqual(3.0, ret);
            }
            //Console.Read();
        }

        [Test]
        public void TestSimpleDllImport() 
        {
            unsafe 
            {
                double ret = UseSimpleDllByStatic.Add(1.0, 2.0);
                Console.WriteLine(ret);
                Assert.AreEqual(3.0, ret);
            }
            //Console.Read();   
        }

        [Test]
        public void TestReflection()
        {

        }
    }
}

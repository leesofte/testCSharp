using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using NUnit.Framework;

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
using testUseDll;

namespace testUseDll.Simple
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
            IntPtr hModule = UseDllByLoadLibrary.LoadLibrary("testCppDll.dll");
            int error = Marshal.GetLastWin32Error();
            //Console.WriteLine("The Last Win32 Error was: " + error);
            if (hModule == IntPtr.Zero) return;
            addFunction = (Add)UseDllByLoadLibrary.GetFunctionAddress(hModule, "Add", typeof(Add));
        }

        [TearDown]
        public void TearDown()
        {
            UseDllByLoadLibrary.FreeLibrary(hModule);
        }

        [Test]
        public void TestLoadLibrary()
        {
            unsafe
            {
                double ret = addFunction(1.0, 2.0);
                Assert.AreEqual(3.0, ret);
            }
            //Console.Read();
        }

        [Test]
        public void TestStatic() 
        {
            unsafe 
            {
                double ret = UseSimpleDllByStatic.Add(1.0, 2.0);
                Console.WriteLine(ret);
            }
            //Console.Read();   
        }

        [Test]
        public void TestReflection()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace testUseDll
{
    public class UseSimpleDllByStatic
    {
        [DllImport("testCppDll.dll")]
        public static extern double Add(double a, double b);

        [DllImport("testCppDll.dll")]
        public static extern double Multiply(double a, double b);

        [DllImport("testCppDll.dll")]
        public static extern double AddMultiply(double a, double b);

    }
}

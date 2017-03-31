using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace testUseDllByCSharp
{
    public class UseDllByLoadLibrary
    {
        public static Delegate GetFunctionAddress(IntPtr dllModule, string functionName, Type t)
        {
            IntPtr address = Win32.GetProcAddress(dllModule, functionName);
            if (address == IntPtr.Zero)
                return null;
            else
                return Marshal.GetDelegateForFunctionPointer(address, t);
        }
    }
}

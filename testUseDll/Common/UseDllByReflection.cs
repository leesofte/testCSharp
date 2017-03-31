using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;

namespace testUseDllByCSharp
{
    public class UseDllByReflection
    {
        private IntPtr hModule = IntPtr.Zero;
        private IntPtr farProc = IntPtr.Zero;

        public void LoadDll(string lpFileName)
        {
            hModule = Win32.LoadLibrary(lpFileName);
            if (hModule == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
        }
        public void LoadDll(IntPtr HMODULE)
        {
            if (HMODULE == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
            hModule = HMODULE;
        }
        public void LoadFun(string lpProcName)
        {
            if (hModule == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
            farProc = Win32.GetProcAddress(hModule, lpProcName);
            if (farProc == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
        }
        public void LoadFun(string lpFileName, string lpProcName)
        {
            hModule = Win32.LoadLibrary(lpFileName);
            if (hModule == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
            farProc = Win32.GetProcAddress(hModule, lpFileName);
            if (farProc == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
        }
        public void UnLoadDll()
        {
            Win32.FreeLibrary(hModule);
            hModule = IntPtr.Zero;
            farProc = IntPtr.Zero;
        }

        public object Invoke(string methodName, object[] ObjArray_parameter, Type[] TypeArray_parameterType, ModePass[] ModePassArray_parameter, Type Type_Return)
        {
            if (hModule == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
            if (farProc == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
            if (ObjArray_parameter.Length != ModePassArray_parameter.Length)
            {
                throw (new Exception(""));
            }
            AssemblyName assName = new AssemblyName();
            assName.Name = "testUseDllByCSharp";
            AssemblyBuilder assBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assBuilder.DefineDynamicModule("testCppDll");
            MethodBuilder methodBuilder = moduleBuilder.DefineGlobalMethod(methodName, MethodAttributes.Public | MethodAttributes.Static, Type_Return, TypeArray_parameterType);
            ILGenerator IL = methodBuilder.GetILGenerator();
            for (int i = 0; i < ObjArray_parameter.Length; i++)
            {
                switch (ModePassArray_parameter[i])
                {
                    case ModePass.ByValue:
                        IL.Emit(OpCodes.Ldarg, i);
                        break;
                    case ModePass.ByRef:
                        IL.Emit(OpCodes.Ldarga, i);
                        break;
                    default:
                        break;
                }
            }
            if (IntPtr.Size == 4)
            {
                IL.Emit(OpCodes.Ldc_I4, farProc.ToInt32());
            }
            else if (IntPtr.Size == 8)
            {
                IL.Emit(OpCodes.Ldc_I8, farProc.ToInt64());
            }
            IL.EmitCalli(OpCodes.Calli, CallingConvention.Cdecl, Type_Return, TypeArray_parameterType);
            IL.Emit(OpCodes.Ret);
            moduleBuilder.CreateGlobalFunctions();
            MethodInfo methodInfo = moduleBuilder.GetMethod(methodName);
            return methodInfo.Invoke(null, ObjArray_parameter);
        }
        public object Invoke(string methodName,IntPtr IntPtr_Func, object[] ObjArray_parameter, Type[] TypeArray_parameterType, ModePass[] ModePassArray_Parameter, Type Type_Return)
        {
            if (hModule == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
            if (IntPtr_Func == IntPtr.Zero)
            {
                throw (new Exception(""));
            }
            farProc = IntPtr_Func;
            return Invoke(methodName,ObjArray_parameter, TypeArray_parameterType, ModePassArray_Parameter, Type_Return);
        }


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

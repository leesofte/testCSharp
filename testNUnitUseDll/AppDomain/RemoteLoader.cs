using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace testAppDomain
{
    /// <summary>
    /// The Remote loader.
    /// </summary>
    public class RemoteLoader : MarshalByRefObject, IDisposable
    {

        Assembly assembly = null;

        public void LoadAssembly(string dllpath)
        {
            if (!File.Exists(dllpath))
            {
                throw new ArgumentException(dllpath + " error");
            }
            byte[] fsContent;
            using (FileStream fs = File.OpenRead(dllpath))
            {
                fsContent = new byte[fs.Length];
                fs.Read(fsContent, 0, fsContent.Length);

            }
            assembly = Assembly.Load(fsContent);
        }

        public object Invoke(string fullClassName, string methodName, params Object[] args)
        {
            object result = null;
            try
            {
                Type pgmType = null;
                if (this.assembly != null)
                {
                    pgmType = this.assembly.GetType(fullClassName, true, true);
                }
                else
                {
                    pgmType = Type.GetType(fullClassName, true, true);
                }
                BindingFlags defaultBinding = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Static;
                CultureInfo cultureInfo = new CultureInfo("es-ES", false);
                try
                {
                    MethodInfo methisInfo = assembly.GetType(fullClassName, true, true).GetMethod(methodName);

                    if (methisInfo == null)
                    {
                        new Exception("EMethod　does　not　exist!");
                    }

                    if (methisInfo.IsStatic)
                    {
                        if (methisInfo.GetParameters().Length == 0)
                        {
                            if (methisInfo.ReturnType == typeof(void))
                            {
                                pgmType.InvokeMember(methodName, defaultBinding, null, null, null, cultureInfo);
                            }
                            else
                            {
                                result = pgmType.InvokeMember(methodName, defaultBinding, null, null, null, cultureInfo);
                            }
                        }
                        else
                        {
                            if (methisInfo.ReturnType == typeof(void))
                            {
                                pgmType.InvokeMember(methodName, defaultBinding, null, null, args, cultureInfo);
                            }

                            else
                            {
                                result = pgmType.InvokeMember(methodName, defaultBinding, null, null, args, cultureInfo);
                            }
                        }
                    }
                    else
                    {

                        if (methisInfo.GetParameters().Length == 0)
                        {
                            object pgmClass = Activator.CreateInstance(pgmType);
                            if (methisInfo.ReturnType == typeof(void))
                            {
                                pgmType.InvokeMember(methodName, defaultBinding, null, pgmClass, null, cultureInfo);
                            }
                            else
                            {
                                result = pgmType.InvokeMember(methodName, defaultBinding, null, pgmClass, null, cultureInfo);
                            }
                        }
                        else
                        {
                            object pgmClass = Activator.CreateInstance(pgmType);
                            if (methisInfo.ReturnType == typeof(void))
                            {
                                pgmType.InvokeMember(methodName, defaultBinding, null, pgmClass, args, cultureInfo);
                            }
                            else
                            {
                                result = pgmType.InvokeMember(methodName, defaultBinding, null, pgmClass, args, cultureInfo);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    result = pgmType.InvokeMember(methodName, defaultBinding, null, null, null, cultureInfo);
                }
                return result;
            }
            catch (Exception ee)
            {
                return result;
            }
        }

        #region IDisposable Members
        public void Dispose()
        {

        }
        #endregion
    }
}
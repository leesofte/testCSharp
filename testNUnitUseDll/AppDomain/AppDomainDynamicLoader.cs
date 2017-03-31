using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Policy;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace testNUnitUseDll
{
    public class AppDomainDynamicLoader : IDisposable
    {

        /// <summary>
        /// The new appdomain.
        /// </summary>
        public AppDomain appDomain = null;

        /// <summary>
        /// The remote loader.
        /// </summary>
        private RemoteLoader remoteLoader = null;

        public AppDomainDynamicLoader(string dllpath)
        {
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            ads.DisallowCodeDownload = true;
            ads.ApplicationName = "ApplicationLoader";
            // Set up the Evidence
            Evidence baseEvidence = AppDomain.CurrentDomain.Evidence;
            Evidence evidence = new Evidence(baseEvidence);
            // Create the AppDomain     
            appDomain = AppDomain.CreateDomain("ApplicationLoaderDomain", null, ads);
            String name = Assembly.GetExecutingAssembly().GetName().FullName;
            remoteLoader = (RemoteLoader)appDomain.CreateInstanceAndUnwrap(name, typeof(RemoteLoader).FullName);
            LoadDll(dllpath);
        }

        private void LoadDll(string dllpath)
        {
            remoteLoader.LoadAssembly(dllpath);
        }

        private void UnLoadDll()
        {
            try
            {
                AppDomain.Unload(this.appDomain);
                this.appDomain = null;
            }
            catch (CannotUnloadAppDomainException ex)
            {
                //log.Error("To unload assembly error!", ex);
            }
        }

        public object InvokeMethod(string fullClassName, string methodName, params Object[] args)
        {
            return remoteLoader.Invoke(fullClassName, methodName, args);
        }

        // IDisposable接口的实现
        #region IDisposable Members

        public void Dispose()
        {
            UnLoadDll();
        }
        #endregion
    }
}

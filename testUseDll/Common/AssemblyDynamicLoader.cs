using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System;
//using Ark.Log;

/// <summary>
/// The local loader.
/// </summary>
public class AssemblyDynamicLoader
{
    /// <summary>
    /// The log util.
    /// </summary>
    //private static ILog log = LogManager.GetLogger(typeof(AssemblyDynamicLoader));

    /// <summary>
    /// The new appdomain.
    /// </summary>
    private AppDomain appDomain;

    /// <summary>
    /// The remote loader.
    /// </summary>
    private RemoteLoader remoteLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalLoader"/> class.
    /// </summary>
    public AssemblyDynamicLoader()
    {
        AppDomainSetup setup = new AppDomainSetup();
        setup.ApplicationName = "ApplicationLoader";
        setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
        setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "private");
        setup.CachePath = setup.ApplicationBase;
        setup.ShadowCopyFiles = "true";
        setup.ShadowCopyDirectories = setup.ApplicationBase;

        this.appDomain = AppDomain.CreateDomain("ApplicationLoaderDomain", null, setup);
        string name = Assembly.GetExecutingAssembly().GetName().FullName;

        this.remoteLoader = (RemoteLoader)this.appDomain.CreateInstanceAndUnwrap(name, typeof(RemoteLoader).FullName);
    }

    /// <summary>
    /// Invokes the method.
    /// </summary>
    /// <param name="fullName">The full name.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="argsInput">The args input.</param>
    /// <param name="programName">Name of the program.</param>
    /// <returns>The output of excuting.</returns>
    public string InvokeMethod(string fullName, string className, string argsInput, string programName)
    {
        this.remoteLoader.InvokeMethod(fullName, className, argsInput, programName);
        return this.remoteLoader.Output;
    }
    //AssemblyDynamicLoader loader = new AssemblyDynamicLoader();
    //String output = loader.InvokeMethod("fileName", "ymtcla", "yjoinp", "ymtpgm");

    /// <summary>
    /// Unloads this instance.
    /// </summary>
    public void Unload()
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
}
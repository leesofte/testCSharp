using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using WcfServiceCommon;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Threading;
using HANDLE = System.IntPtr;  

namespace WcfServiceHost
{
	class Program
	{
        const string WCF_EVENT_NAME = "Global\\WCF";
        const string WCF_SEM_NAME = "Global\\WCF_SEM";
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenEvent(bool dwDesiredAccess, bool bInheritHandle, string lpName);
        //OpenEventW 
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool SetEvent(HANDLE hEvent, int dEvent);
        [DllImport("kernel32", EntryPoint = "WaitForSingleObject", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WaitForSingleObject(HANDLE hHandle, int dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hobject);
        public enum EventFlags
        {
            PULSE = 1,
            RESET = 2,
            SET = 3
        }
		static void Main(string[] args)
		{
			if (Config.IsConsoleMode)
			{
				foreach (var sc in Config.GetServiceConfigList())
				{
					Console.WriteLine(sc.Item.Key);
					Console.WriteLine("{0}: {1}", "HostTypeDeclaration", sc.Item.HostTypeDeclaration);
					Console.WriteLine("{0}: {1}", "HostTypeFullname", sc.Item.HostTypeFullname);
					Console.WriteLine("{0}: {1}", "HostTypeAssembly", sc.Item.HostTypeAssembly);
					Console.WriteLine("{0}: {1}", "ContractTypeDeclaration", sc.Item.ContractTypeDeclaration);
					Console.WriteLine("{0}: {1}", "ContractTypeFullname", sc.Item.ContractTypeFullname);
					Console.WriteLine("{0}: {1}", "ContractTypeAssembly", sc.Item.ContractTypeAssembly);
					Console.WriteLine("{0}: {1}", "ServiceAddressPort", sc.Item.ServiceAddressPort);
					Console.WriteLine("{0}: {1}", "EndpointName", sc.Item.EndpointName);
					Console.WriteLine("{0}: {1}", "AuthorizedGroups", sc.Item.AuthorizedGroups);
					Console.WriteLine("");
				}
				ServiceRunner sr = new ServiceRunner();
				sr.Start(args);
				Console.WriteLine("WcfServiceHost started... Hit enter to stop...");
				//Console.ReadLine();
                //HANDLE m_hEvent = IntPtr.Zero;                
                //m_hEvent = CreateEvent(IntPtr.Zero, false, false, WCF_EVENT_NAME);
                //SetEvent(m_hEvent, (int)EventFlags.RESET);
                //WaitForSingleObject(m_hEvent, System.Threading.Timeout.Infinite); 
                Semaphore m_Read = new Semaphore(0, 1, WCF_SEM_NAME);//没有数据可读;
                m_Read.WaitOne();                
				sr.Stop();
                //CloseHandle(m_hEvent);
                //Console.ReadLine();
			}
			else
			{
				//Run it as a service
				ServiceBase[] servicesToRun;
				try
				{
					servicesToRun = new ServiceBase[] { new WcfService() };
					ServiceBase.Run(servicesToRun);
				}
				catch (Exception e)
				{
					//do your exception handling thing
					e.ProcessUnhandledException("WcfServiceHost");
				}
			}
		}
	}
}

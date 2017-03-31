using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using WcfServiceCommon;
using System.Threading;
using System.Reflection;
using testUseDllByCSharp;
using testWCFService.Interface;

namespace WcfSvcTest
{
	internal class ClientBaseInvokeTests
	{
		//cannot wrap in a using, otherwise, you get a connect access disposed object error
		WcfServiceClient<INameEntityService> client = null;

		public void DoTest()
		{
			Console.WriteLine("DoTest thread: {0}", Thread.CurrentThread.ManagedThreadId);
            client = WcfServiceClient<INameEntityService>.Create("test1");
			client.AsyncCompleted += new EventHandler<GenericAsyncCompletedEventArgs>(client1_AsynchCompleted);

			//client.AsyncBegin(client.Instance.GetAddress, null, 8);
			
			client.AsyncBegin("GetAddress", null, 8);
			client.AsyncBegin("DoSomethingElse", null, "comm");  //test a void return type
			client.AsyncBegin("DoNothing", null, null); //test void return type with no params
		}

		void client1_AsynchCompleted(object sender, GenericAsyncCompletedEventArgs e)
		{
			Console.WriteLine("AsynchCompleted thread: {0}", Thread.CurrentThread.ManagedThreadId);
			Console.WriteLine("method: {0}", e.MethodName);
			var retval = e.Result as NameEntity;
			if (retval != null)
				Console.WriteLine("AsyncCompleted {0}", retval._name);
			if (client != null) client.Dispose();
		}
	}
}

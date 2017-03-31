using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using testWCFService.Interface;
using WcfServiceCommon;
using System.Runtime.Remoting.Messaging;
using testUseDllByCSharp;

namespace WcfSvcTest
{
	internal class ExternalAsynchTests
	{
		ManualResetEvent waiter;
        /*
		void OneArgFuncCallback(IAsyncResult result)
		{
			Console.WriteLine("OneArgFuncCallback thread: {0}", Thread.CurrentThread.ManagedThreadId);
			Thread.Sleep(500);
			Func<int, object> nnn = (Func<int, object>)((AsyncResult)result).AsyncDelegate;

            NameEntity ret = nnn.EndInvoke(result) as NameEntity;
            if (null != ret)
                Console.WriteLine("OneArgFuncCallback return value (name): {0}", ret._name);

			waiter.Set();
		}

        public NameEntity GetNameEntity(int id)
		{
            client = WcfServiceClient<INameEntityService>.Create("test1");
            return null;//client.Instance.CreateNameEntityByTypeByParam(id);
		}

		public void DoWrapperTest()
		{
			Console.WriteLine("DoWrapperTest thread: {0}", Thread.CurrentThread.ManagedThreadId);
			waiter = new ManualResetEvent(false);

            Func<int, object> nnn = new Func<int, object>(this.GetNameEntity);

			IAsyncResult result = nnn.BeginInvoke(8, new AsyncCallback(this.OneArgFuncCallback), null);

			Console.WriteLine("GetAddress invoked");

			waiter.WaitOne();

		}

        
        WcfServiceClient<INameEntityService> client;

		public void DoDirectClientTest()
		{
            client = WcfServiceClient<INameEntityService>.Create("test1");

            Console.WriteLine("DoDirectClientTest thread: {0}", Thread.CurrentThread.ManagedThreadId);
			waiter = new ManualResetEvent(false);

            Func<int, object> nnn = new Func<int, object>(client.Instance.GetNameEntity);

			IAsyncResult result = nnn.BeginInvoke(8, new AsyncCallback(this.OneArgFuncCallback), null);

			Console.WriteLine("GetAddress invoked");

			waiter.WaitOne();
		}*/
	}
}

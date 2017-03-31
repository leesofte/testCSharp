using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WcfServiceCommon;
using System.ServiceModel;
using testWCFService.Interface;
using testUseDllByCSharp;

namespace WcfSvcTest
{
	internal class StandardSynchronousTests
	{
		public void DoTest()
		{
			using (var client1 = WcfServiceClient<INameEntityService>.Create("test1"))
			{
                try
                {
                    int type = 0;
                    NameEntity nameEntity = new NameEntity();
                    bool ret = client1.Instance.CreateNameEntityByTypeByParam(type, ref nameEntity);
                    Console.WriteLine("{0}", ret);
                }
                catch (FaultException<WcfServiceFault> fault)
                {
                    Console.WriteLine(fault.ToString());
                }
                catch (Exception e) //handles exceptions not in wcf communication
                {
                    Console.WriteLine(e.ToString());
                }
			}
		}
	}
}

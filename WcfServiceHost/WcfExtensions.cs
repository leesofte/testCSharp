using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WcfServiceCommon;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WcfServiceHost
{
	public static class WcfExtensions
	{
		public static ServiceHost CreateServiceHost(this WcfServiceConfigElement item)
		{
			//check configuration, no launch if no config
			if (string.IsNullOrEmpty(item.ServiceAddressPort) || string.IsNullOrEmpty(item.EndpointName)) return null;

			Type hostType = Type.GetType(item.HostTypeDeclaration);
			Type contractType = Type.GetType(item.ContractTypeDeclaration);

			Uri tcpBaseAddress = new Uri(string.Format("net.tcp://{0}/", item.ServiceAddressPort));
			ServiceHost host = new ServiceHost(hostType, tcpBaseAddress);

			NetTcpBinding tcpBinding = TcpBindingUtility.CreateNetTcpBinding();

			host.AddServiceEndpoint(contractType, tcpBinding,
				string.Format("net.tcp://{0}/{1}", item.ServiceAddressPort, item.EndpointName));

			//this is the default but good to know if we want to change it later
			host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.UseWindowsGroups;

			return host;
		}
	}
}

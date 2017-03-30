using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WcfServiceHost
{
	partial class WcfService : ServiceBase
	{
		ServiceRunner serviceRunner = null;

		public WcfService()
		{
			InitializeComponent();
			serviceRunner = new ServiceRunner();
		}

		protected override void OnStart(string[] args)
		{
			serviceRunner.Start(args);
		}

		protected override void OnStop()
		{
			serviceRunner.Stop();
		}
	}
}

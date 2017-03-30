using System;

using ThreadMessaging;

namespace ProcessTest
{
	[Serializable]
	struct Message
	{
		public string Text;
	}

	class Test
	{
		IMailBox mail;

		public Test()
		{
			mail = new ProcessMailBox("TMProcessTest",1024);
		}

		public void RunWriter()
		{
			Console.WriteLine("Writer started");
			Message msg;
			while(true)
			{
				msg.Text = Console.ReadLine();
				if(msg.Text.Equals("exit"))
					break;
				mail.Content = msg;
			}
		}

		public void RunReader()
		{
			Console.WriteLine("Reader started");
			while(true)
			{
				Message msg = (Message)mail.Content;
				Console.WriteLine(msg.Text);
			}
		}


		[STAThread]
		static void Main(string[] args)
		{
			Test test = new Test();
			if(args.Length > 0)
				test.RunWriter();
			else
				test.RunReader();
		}
	}
}

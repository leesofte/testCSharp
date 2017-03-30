using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using testUseDll.Complex;

namespace testUseDll
{
    public class MessageWindow:Form
    {
        private Button button1;
        public const int WM_COPYDATA = 0x4A;

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }
        public MessageWindow()
        {
            InitializeComponent();
        }
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
               int hWnd, 
               int Msg,                              
               int wParam,                            
               ref COPYDATASTRUCT lParam 
               );

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);


        // 接收到数据委托与事件定义
        public delegate void ReceiveBytesEvent(object sender, uint flag, byte[] bt);
        public event ReceiveBytesEvent OnReceiveBytes;
        // 发送数据委托与事件定义
        public delegate void SendBytesEvent(object sender, uint flag, byte[] bt);
        public event SendBytesEvent OnSendBytes;

        public bool SendBytes(string destWindow, uint flag, byte[] data)
        {
            int WINDOW_HANDLER = 5572654;//FindWindow(null, @destWindow);
            if (WINDOW_HANDLER == 0) return false;
            int hwnd = FindWindow(null, @"testNUnitUseDll");
            try
            {
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)flag;
                cds.cbData = data.Length;
                cds.lpData = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, cds.lpData, data.Length);
                SendMessage(WINDOW_HANDLER, WM_COPYDATA, 0, ref cds);
                if (OnSendBytes != null)
                {
                    OnSendBytes(this, flag, data);
                }
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                // 接收 CopyData 消息，读取发送过来的数据
                case WM_COPYDATA:
                    Console.WriteLine("xxxxxxxxxxxx");
                    COPYDATASTRUCT cds = new COPYDATASTRUCT();
                    Type mytype = cds.GetType();
                    cds = (COPYDATASTRUCT)m.GetLParam(mytype);
                    uint flag = (uint)(cds.dwData);
                    byte[] bt = new byte[cds.cbData];
                    Marshal.Copy(cds.lpData, bt, 0, bt.Length);
                    object obj = SerializeUtil.Deserialize(bt);
                    NameEntity nameEntity = (NameEntity)obj;
                    if (OnReceiveBytes != null)
                    {
                        OnReceiveBytes(this, flag, bt);
                    }
                    break;

                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(48, 94);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MessageWindow
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.button1);
            this.Name = "MessageWindow";
            this.ResumeLayout(false);

            this.button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NameEntity nameEntity = new NameEntity();
            SendBytes("testLeakConsoleExe", 0, SerializeUtil.Serialize(nameEntity));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace PumpProtocol
{
    public partial class ProtocolTester : Form
    {
        SerialPort comPort;
        Thread th_recvFromPumpSerialPort;

        public struct commu
        {
            public SerialPort pumpPort;

            public string STX;
            public string ID;
            public string AI;
            public string PFC;
            public string VALUE;
            public string CRC;
            public string ETX;

            public bool revLock;
        }
        public commu t_SerialPortCommu;

        public ProtocolTester()
        {
            InitializeComponent();
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            if (bConnect.Text == "Connect")
            {
                comPort = new SerialPort();
                comPort.PortName = lCOM.Text;
                comPort.ReadTimeout = 100;
                //port.ReceivedBytesThreshold = 1;
                comPort.RtsEnable = false;
                comPort.DtrEnable = false;
                comPort.BaudRate = System.Convert.ToInt32(lBAUD.Text);
                comPort.DataBits = 8;
                comPort.StopBits = StopBits.One;
                comPort.Parity = Parity.None;
                comPort.DiscardNull = false;
                comPort.NewLine = "!";
                comPort.Open();

                th_recvFromPumpSerialPort = new Thread(RecvProtocol);
                th_recvFromPumpSerialPort.Start();

                bConnect.Text = "DisConnect";
                bSend.Enabled = true;
            } else if (bConnect.Text == "DisConnect")
            {
                if (th_recvFromPumpSerialPort != null) th_recvFromPumpSerialPort.Abort();
                comPort.Dispose();
                bConnect.Text = "Connect";
                bSend.Enabled = false;
            }
            //AppConfig.frmBottom.EquipmentLOG_Text.Text = AppConfig.frmBottom.EquipmentLOG_Text.Text.Insert(0, Environment.NewLine);

        }

        private void ProtocolTester_Load(object sender, EventArgs e)
        {
            lCOM.Text = "COM9";
            lBAUD.Text = "9600";
            bSend.Enabled = false;
        }

        private void ProtocolTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (comPort != null)
                comPort.Dispose();
            if (th_recvFromPumpSerialPort != null)
                th_recvFromPumpSerialPort.Abort();
        }

        private void RecvProtocol()
        {
            string recvStr = string.Empty;
            string recvProtocol = string.Empty;

            byte[] recvByte = new byte[100];
            string sendStr = string.Empty;

            while (true)
            {
                Thread.Sleep(1);
                if (comPort != null)
                {
                    try
                    {
                        //if (t_SerialPortCommu.ID == "Tauto")
                        //{
                        //    t_SerialPortCommu.pumpPort.ReadTo("Q");
                        //    pressureStr = "Q" + t_SerialPortCommu.pumpPort.ReadTo("Y");
                        //}
                        //else
                        //{
                        recvStr = string.Empty;
                         recvProtocol = string.Empty;

                        //while (!(recvStr = comPort.ReadExisting()).Contains("!")) ;
                        while (!(recvStr.Contains("!") && recvStr.Contains("\n"))) recvStr += comPort.ReadExisting();
                        //comPort.ReadTo("!");
                        //recvStr = recvStr + comPort.ReadTo("\n") + "\n";
                        recvProtocol = recvStr.Substring(recvStr.LastIndexOf('!'), recvStr.LastIndexOf('\n') + 1);

                        //}
                        this.Invoke(new EventHandler(delegate
                        {
                            tCommu.Text = tCommu.Text.Insert(0, Environment.NewLine);
                            tCommu.Text = tCommu.Text.Insert(0, recvProtocol + Environment.NewLine);
                            tCommu.Text = tCommu.Text.Insert(0, DateTime.Now.ToLongTimeString() + Environment.NewLine);


                            rSTX.Text = recvProtocol.Substring(0, 1);
                            rID.Text = recvProtocol.Substring(1, 2);
                            rAI.Text = recvProtocol.Substring(3, 1);
                            rPFC.Text = recvProtocol.Substring(4, 2);
                            rVALUE.Text = recvProtocol.Substring(6, 6);
                            rCRC.Text = recvProtocol.Substring(0x0C, 3);
                            rETX.Text = recvProtocol.Substring(0x0F, 1);
                        }));

                        if (recvProtocol.Substring(4, 2) == "01")
                        {
                            sendStr = "!10001    10";
                            comPort.Write(sendStr + GetCRC(sendStr) + "\n");
                        }
                        if (recvProtocol.Substring(4, 2) == "03")
                        {
                            sendStr = "!10003123456";
                            comPort.Write(sendStr + GetCRC(sendStr) + "\n");
                        }
                        if (recvProtocol.Substring(4, 2) == "18")
                        {
                            comPort.Write("#");
                        }
                        if (recvProtocol.Substring(4, 2) == "13")
                        {
                            comPort.Write("#");
                        }
                        if (recvProtocol.Substring(4, 2) == "14")
                        {
                            comPort.Write("#");
                        }
                        if (recvProtocol.Substring(4, 2) == "19")
                        {
                            comPort.Write("#");
                        }
                        if (recvProtocol.Substring(4, 2) == "10")
                        {
                            comPort.Write("#");
                        }
                        if (recvProtocol.Substring(4, 2) == "15")
                        {
                            comPort.Write("#");
                        }
                        if (recvProtocol.Substring(4, 2) == "16")
                        {
                            comPort.Write("#");
                        }
                    }
                    catch
                    {
                        recvStr = "";
                        recvProtocol = "";
                        continue;
                    }
                }
            }
        }




        private string GetCRC(string s)
        {
            int sum = 0;
            byte[] b = System.Text.Encoding.Default.GetBytes(s);
            for (int i = 0; i < s.Length; ++i)
            {
                //sum = sum + Convert.ToInt32(s.Substring(i, 1));
                sum = sum + Convert.ToInt32(b[i]);
            }

            return string.Format("{0:000}", sum % 256);
        }
    }
}

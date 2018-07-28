using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRC16
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] s = (textBox1.Text.Trim()).Split(' ');
            byte[] b = new byte[s.Length];

            for (int i = 0; i < s.Length; ++i)
            {
                b[i] = Convert.ToByte(s[i], 16);
            }
            byte[] crc = crc16(b);
            textBox2.Text = textBox1.Text.Trim() + " " + Convert.ToString(crc[0], 16).ToUpper() + " " + Convert.ToString(crc[1], 16).ToUpper();
        }

        private byte[] crc16(byte[] puchMsg)
        {
            byte[] ret = new byte[2];
            UInt16 wCRCin = 0xFFFF;
            byte wCRCinbit;
            UInt16 wCPoly = 0xA001;

            for (int i = 0; i < puchMsg.Length; i++)
            {
                wCRCin ^= puchMsg[i];
                for (int j = 0; j < 8; j++)
                {
                    wCRCinbit = (byte)(wCRCin & 0x01);
                    wCRCin >>= 1;
                    if (wCRCinbit == 1)
                        wCRCin ^= wCPoly;
                }
            }
            ret = new byte[2] {(byte)(wCRCin&0xFF), (byte)(wCRCin >> 8)};
            return (ret);
        }
    }
}

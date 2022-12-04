using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Seti2
{
    public partial class Form1 : Form
    {
        string inputMessage = "";
        string outputMessage = "";
        string outputMessage2 = "";
        string outputMessage22 = "";
        string outputMessage3 = "";
        string dopSymb = "";
        bool tactSignal = true;
        bool bufer = false;
        bool tumbler = false;
        public Form1()
        {
            InitializeComponent();
        }

        bool toBool(char c)
        {
            if (c == '1')
                return true;
            else
                return false;
        }

        char toChar(bool c)
        {
            if (c)
                return '1';
            else
                return '0';
        }

        string scrembl(string inp)
        {
            string outp = "";
            for (int i = 0; i < 5; i++)
            {
                outp += inp[i];
            }
            outp += outp[0] ^ inp[5];
            outp += outp[1] ^ inp[6];
            outp += outp[2] ^ inp[7];
            outp += outp[3] ^ inp[8];
            outp += toChar(toBool(outp[4]) ^ toBool(outp[0]) ^ toBool(inp[9]));
            outp += toChar(toBool(outp[5]) ^ toBool(outp[1]) ^ toBool(inp[10]));
            for (int i = 11; i < inp.Length; i++)
            {
                outp += toChar(toBool(outp[i - 5]) ^ toBool(outp[i-9]) ^ toBool(outp[i-11]) ^ toBool(inp[i]));
            }
            return outp;
        }

        string descrembl(string inp)
        {
            string outp = "";
            for (int i = 0; i < 5; i++)
            {
                outp += inp[i];
            }
            outp += inp[0] ^ inp[5];
            outp += inp[1] ^ inp[6];
            outp += inp[2] ^ inp[7];
            outp += inp[3] ^ inp[8];
            outp += toChar(toBool(inp[4]) ^ toBool(inp[0]) ^ toBool(inp[9]));
            outp += toChar(toBool(inp[5]) ^ toBool(inp[1]) ^ toBool(inp[10]));
            for (int i = 11; i < inp.Length; i++)
            {
                outp += toChar(toBool(inp[i - 5]) ^ toBool(inp[i-9]) ^ toBool(inp[i-11]) ^ toBool(inp[i]));
            }
            return outp;
        }

        void bass()
        {
            inputMessage = textBox1.Text;
            outputMessage = "";
            outputMessage2 = "";
            outputMessage22 = "";
            tumbler = false;
            dopSymb = "";
            foreach (var symb in inputMessage)
            {
                if (symb != ' ')
                {
                    dopSymb = BitConverter.ToString(Encoding.Default.GetBytes(symb.ToString()));
                    
                }
                else
                {
                    if (!tumbler)
                    {
                        dopSymb = "00";
                        tumbler = true;
                    }
                    else
                    {
                        dopSymb = "FF";
                    }
                }
                outputMessage += dopSymb;
                outputMessage2 += Convert.ToString(Convert.ToInt64(dopSymb, 16), 2).PadLeft(dopSymb.Length * 4, '0');
            }
            if (radioButton2.Checked)
            {
                string four;
                string[] mas1 = new string[] { "0000", "0001", "0010", "0011", "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011", "1100", "1101", "1110", "1111" };
                string[] mas2 = new string[] { "11110", "01001", "10100", "10101", "01010", "01011", "01110", "01111", "10010", "10011", "10110", "10111", "11010", "11011", "11100", "11101" };
                for(int i = 3;i < outputMessage2.Length;i+=4)
                {
                    four = outputMessage2[i - 3].ToString() + outputMessage2[i - 2].ToString() + outputMessage2[i - 1].ToString() + outputMessage2[i].ToString();
                    for(int j = 0; j < 16; j++)
                    {
                        if (four == mas1[j])
                        {
                            outputMessage22 += mas2[j];
                            break;
                        }
                    }
                }
                outputMessage2 = outputMessage22;
            }
            else if (radioButton3.Checked)
            {
                string msg = outputMessage2 == descrembl(scrembl(outputMessage2)) ? "OK" : "ERROR";
                MessageBox.Show(outputMessage2 + '\n' + descrembl(scrembl(outputMessage2))+ '\n' + msg);
                outputMessage2 = scrembl(outputMessage2);
            }
            label1.Text = outputMessage2;
            label2.Text = outputMessage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            outputMessage3 = "";
            bass();
            chart1.Series[0].Points.Clear();
            foreach (var symb in outputMessage2)
            {
                if (symb == '0') bufer = false;
                else bufer = true;
                if (bufer)
                {
                    chart1.Series[0].Points.Add(1);
                    chart1.Series[0].Points.Add(0);
                    outputMessage3 +='1';
                }
                else
                {
                    chart1.Series[0].Points.Add(0);
                    chart1.Series[0].Points.Add(1);
                    outputMessage3 += '0';
                }
                tactSignal = !tactSignal;
            }
            richTextBox1.Text = outputMessage + '\n' + outputMessage2 + '\n' + outputMessage3 + '\n';
        }

        private void button2_Click(object sender, EventArgs e)
        {
            outputMessage3 = "";
            bool previousTrue = false;
            bass();
            chart1.Series[0].Points.Clear();
            foreach (var symb in outputMessage2)
            {
                if (symb == '1')
                {
                    if(previousTrue)
                    {
                        chart1.Series[0].Points.Add(-1);
                        outputMessage3+='1';
                    }
                    else
                    {
                        chart1.Series[0].Points.Add(1);
                        outputMessage3 += '1';
                    }
                    previousTrue = !previousTrue;
                }
                else
                {
                    chart1.Series[0].Points.Add(0);
                    outputMessage3 += '0';
                }
            }
            richTextBox1.Text = outputMessage + '\n' + outputMessage2 + '\n' + outputMessage3 + '\n';
        }

        private void button3_Click(object sender, EventArgs e)
        {
            outputMessage3 = "";
            bool previousTact = false;
            bass();
            chart1.Series[0].Points.Clear();
            foreach (var symb in outputMessage2)
            {
                if (symb == '1')
                {
                    previousTact = !previousTact;
                }
                if (previousTact)
                {
                    chart1.Series[0].Points.Add(1);
                    outputMessage3 += '1';
                }
                else
                {
                    chart1.Series[0].Points.Add(0);
                    outputMessage3 += '0';
                }
            }
            richTextBox1.Text = outputMessage + '\n' + outputMessage2 + '\n' + outputMessage3 + '\n';
        }

        private void button4_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            chart1.Series[0].Color = colorDialog1.Color;
        }
    }
}

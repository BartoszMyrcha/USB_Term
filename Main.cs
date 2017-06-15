using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USB_Term
{
    public partial class Main : Form
    {
        List<Pomiar> pomiary = new List<Pomiar>();
        SerialPort port = new SerialPort(" ", 115200, Parity.None, 8, StopBits.One);
        bool zapisuj = false, pomiar=false;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        delegate void SetTextCallback(string text);
        delegate void SetTextCallback2(string text);


        public Main()
        {
            InitializeComponent();
            AllocConsole();
            String[] porty = SerialPort.GetPortNames();
            foreach (String x in porty)
            comboBox1.Items.Add(x);
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            timer1.Start();
           
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string Temp="ERR";
            string Hum="ERR";
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            if (pomiar)
            {
                if (indata.Length == 13)
                {
                    double T = double.Parse(indata.Substring(0, 3)) / 10;
                    double H = double.Parse(indata.Substring(4, 3)) / 10;
                    double suma = double.Parse(indata.Substring(8, 4)) / 10;
                    Console.WriteLine(indata);
                    if (Math.Abs(T + H - suma) > 0.001)
                    {
                        Console.Clear();
                        Console.WriteLine("Temperatura: ERROR (" + T + ")");
                        Console.WriteLine("Wilgotność: ERROR (" + H + ")");
                        Console.WriteLine("Suma kontrolna: (" + suma + ")");
                        Console.WriteLine(indata);
                    }
                    else
                    {
                        Console.Clear();
                        Temp = String.Format("{0:0.0}", T);
                        Hum = String.Format("{0:0.0}", H);
                        //pomiary.Add(new Pomiar(DateTime.Now, T, H)); -- nie używane (przydatne przy masowym zapisywaniu wyników lub wyświetlaniu wykresu)

                        Console.WriteLine("Temperatura: " + Temp + "*C");
                        Console.WriteLine("Wilgotność: " + Hum + "%");
                        //dodać zapis do pliku
                        if (zapisuj)
                        {
                            if (!File.Exists("dane.bmf"))
                            {
                                StreamWriter sw = File.CreateText("dane.bmf");
                                sw.WriteLine("Data i czas\t\t|\tTemperatura\t|\tWilgotność");
                                sw.WriteLine("----------------------------------------------------------------------");
                                sw.Close();
                            }
                            else
                                using (StreamWriter sw = new StreamWriter("dane.bmf", true))
                                {
                                    sw.WriteLine(DateTime.Now + "\t|\t" + Temp + "*C\t\t|\t" + Hum + "%");
                                    sw.Close();
                                }
                        }
                        pomiar = false;
                    }
                }    
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (port.IsOpen == false)
            {
                port.PortName = comboBox1.SelectedItem.ToString();
                port.Open();
                MessageBox.Show("Otwarto port: " + port.PortName);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            zapisuj = checkBox1.Checked;
            Console.WriteLine("Zapisuj = " + zapisuj);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = "Odstęp między pomiarami: " + trackBar1.Value + "s";
            timer1.Interval = trackBar1.Value * 1000;
            Console.WriteLine("Timer interval = " + timer1.Interval);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pomiar = true;
            Console.WriteLine("Pomiar = " + pomiar);
        }

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox1.Items.Clear();
            String[] porty = SerialPort.GetPortNames();
            foreach (String x in porty)
                comboBox1.Items.Add(x);
        }

        private void SetTempText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTempText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text = text;
            }
        }
    }
}

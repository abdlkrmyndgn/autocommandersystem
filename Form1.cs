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
using System.Speech.Synthesis;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Http;
using Newtonsoft.Json;
using System.Management;

namespace autocommandersystem
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        PromptBuilder asistan = new PromptBuilder();//Bilgisayarımızın Bize Sesli Dönüş yapması için kullanılır.
        SpeechSynthesizer synt = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();
            serialPort = new SerialPort("COM5", 9600);
            try
            {
                serialPort.Open();//Seri haberleşme birimlerini etkin hale getirdim
            }
            catch
            {
                Console.WriteLine("Port Açılamadı Port aktif deil.");
            }
        }

        public class WeatherData
        {
            public MainData Main { get; set; }
        }

        public class MainData
        {
            public float Temp { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)//Çalıştırma Butonu
        {
            asistanSpeak("Engine is Starting");
            asistanSpeak("Three");
            Thread.Sleep(1000);
            asistanSpeak("Two");
            Thread.Sleep(1000);
            asistanSpeak("One");
            if (serialPort.IsOpen)
            {
                serialPort.Write("1");
                radioButton2.Checked = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)//Durdurma Butonu
        {
            if(serialPort.IsOpen)
            {
                serialPort.Write("0");
            }
            asistanSpeak("Engine Stop");
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(serialPort.IsOpen)
            {
                serialPort.Write("0");
                serialPort.Close();
            }
        }

        public void asistanSpeak(string text)
        {
            synt.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            asistan.ClearContent();
            asistan.AppendText(text);
            synt.Speak(asistan);
        }


        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private const int WM_SYSCOMMAND = 274;

        private const int SC_MAXIMIZE = 61488;


        private void Form1_Load(object sender, EventArgs e)
        {
            asistanSpeak("WellCome To AutoCommander, Pleas Wait Programing is starting...");
            pictureBox1.BackColor = Color.Transparent;
            label1.BackColor = Color.Transparent;
            string exeyolu = "C:\\Program Files (x86)\\Mission Planner\\MissionPlanner.exe";
            Process calistir = Process.Start(exeyolu);

            while (calistir.MainWindowHandle == IntPtr.Zero || !IsWindowVisible(calistir.MainWindowHandle))
            {
                System.Threading.Thread.Sleep(10);
                calistir.Refresh();
            }
            System.Threading.Thread.Sleep(100);
            //calistir.WaitForInputIdle();
            SetParent(calistir.MainWindowHandle, this.panel2.Handle);
            SendMessage(calistir.MainWindowHandle, WM_SYSCOMMAND, SC_MAXIMIZE, 0);

            //Hava Durumu Verilerini Alma//
            async System.Threading.Tasks.Task Main(string[] args)
            {
                string apiKey = "27c907479028a96b0ce70e2fc17cfeb3"; //HAVA DURUMU API TOKENİM
                string city = "Bilecik";

                HttpClient client = new HttpClient();

                try
                {
                    string url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
                    string json = await client.GetStringAsync(url);

                    WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(json);

                    label1.Text = city + weatherData.Main.Temp;
                   
                }
                catch (Exception ex)
                {
                    for (; ; )
                    {
                        asistanSpeak("Weather data is not received please stop the vehicle.");//Hava Durumu verileri alınamadı
                    }
                }
                finally
                {
                    client.Dispose();
                }
            }
            sarjverilerinial();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
          
            if (radioButton2.Checked||radioButton3.Checked||radioButton1.Checked)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Write("2");
                }
                asistanSpeak("Lighting is Open");
            }
        }

        void sarjverilerinial()//Windows kaynak izleme kullanılarak sarj verileri alınmıştır.
        {
            try
            {
                ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\CIMV2");
                ObjectQuery query = new ObjectQuery("SELECT EstimatedChargeRemaining, BatteryStatus FROM Win32_Battery");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    int chargeRemaining = Convert.ToInt32(obj["EstimatedChargeRemaining"]);
                    progressBar1.Value = chargeRemaining;
                }
            }
            catch (Exception ex)
            {
                for (; ; )
                {
                    asistanSpeak("charging data is not received please stop the vehicle.");//Sarj verileri alınamadı.
                }
                
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton4.Checked)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Write("0");
                }
                asistanSpeak("Lighting is Close");
            }
            
        }
    }
}

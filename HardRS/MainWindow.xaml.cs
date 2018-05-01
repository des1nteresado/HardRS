using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using System.Management;
using OpenHardwareMonitor.Hardware;
using System.Globalization;

namespace HardRS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       


        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");


        public MainWindow()
        {
            InitializeComponent();
            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Tick += new EventHandler(Timer_Tick);
            //timer.Interval = new TimeSpan(0, 0, 1);
            //timer.Start();

            //DispatcherTimer timer2 = new DispatcherTimer();
            //timer2.Tick += new EventHandler(GetCpuTemp);
            //timer2.Interval = new TimeSpan(0, 0, 1);
            //timer2.Start();

        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    //температу процессора плохо работает
        //    try
        //    {

        //        foreach (ManagementObject queryObj in searcher.Get())
        //        {
        //            Double temp = Convert.ToDouble(queryObj["CurrentTemperature"].ToString());
        //            temp = temp / 10 - 273;
        //            textBox5.Text = temp.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        textBox5.Text = ex.Message;
        //    }
            



        //}


    }
}

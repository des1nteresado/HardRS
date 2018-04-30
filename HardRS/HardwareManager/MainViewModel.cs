using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CircularProgressBarApp.mvvmSupport;
using HardRS.HardwareManager;
using OpenHardwareMonitor.Hardware;

namespace HardRS.CircularProgressBar
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //PerformanceCounter cpuSCounter = new PerformanceCounter("Сведения о процессоре", "Частота процессора", "_Total");
        //PerformanceCounter cpuCounter = new PerformanceCounter("Сведения о процессоре", "% загруженности процессора", "_Total");
        //PerformanceCounter sysCounter = new PerformanceCounter("System", "System Up Time");
        //PerformanceCounter diskCounter = new PerformanceCounter("Физический диск", "% активности диска", "_Total");

        //PerformanceCounter cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        //PerformanceCounter memCounter = new PerformanceCounter("Memory", "% Commited Bytes In Use");
        //PerformanceCounter memCounter = new PerformanceCounter("Memory", "Available MBytes");
        //PerformanceCounter diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Read Time", "_Total");

        //    textBox1.Text = "" + (int)cpuSCounter.NextValue();
        //    //textBox1.Text = "CPU " + (int)cpuCounter.NextValue() + "%";
        //    textBox2.Text = "Available Memory:" + (int)memCounter.NextValue() + "MB";
        //    textBox3.Text = "System Up Time:" + (int)sysCounter.NextValue()/60/60 + "Hours";
        //    //textBox4.Text = "Физ. диск:" + (int)diskCounter.NextValue() + "%";
        //    textBox4.Text = "Частота процессора:" + (int)cpuSCounter.NextValue();

        private BackgroundWorker _bgWorker = new BackgroundWorker();


    private int _progressCPU;
        private int _progressMEM;
        private int _progressHDD;

        public int ProgressCPU
        {
            get
            {
                return _progressCPU;
            }
            set
            {
                if (value != 0)
                {
                    _progressCPU = value;
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressCPU"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressCPUText"));
                }
            }
        }

        public int ProgressMEM
        {
            get
            {
                return _progressMEM;
            }
            set
            {
                if (value != 0)
                {
                    _progressMEM = value;
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressMEM"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressMEMText"));
                }
            }
        }

        public int ProgressHDD
        {
            get
            {
                return _progressHDD;
            }
            set
            {
                _progressHDD = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressHDD"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressHDDText"));
                }
            }
        }

        public string ProgressCPUText
        {
            get { return string.Format("{0} %", _progressCPU); }
        }
        public string ProgressMEMText
        {
            get { return string.Format("{0} %", _progressMEM); }
        }
        public string ProgressHDDText
        {
            get { return string.Format("{0} %", _progressHDD); }
        }

        public MainViewModel()
        {
            _bgWorker.WorkerSupportsCancellation = true;
            _bgWorker.CancelAsync();

          
            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs y)
        {
            PerformanceCounter cpuCounter;
            cpuCounter = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };

            //PerformanceCounter avMemory = new PerformanceCounter("Memory", "Total Physical Memory");
            PerformanceCounter memCounter;
            memCounter = new PerformanceCounter
            {
                CategoryName = "Memory",
                CounterName = "% Committed Bytes In Use"
            };

            PerformanceCounter hddCounter;
            hddCounter = new PerformanceCounter
            {
                CategoryName = "PhysicalDisk",
                CounterName = "% Disk Read Time",
                InstanceName = "_Total"
            };

            try
            {
            _bgWorker.RunWorkerAsync();
            }
#pragma warning disable CS0168 // Переменная "e" объявлена, но ни разу не использована.
            catch(Exception e)
#pragma warning restore CS0168 // Переменная "e" объявлена, но ни разу не использована.
            {
                _bgWorker.WorkerSupportsCancellation = true;
                _bgWorker.CancelAsync();
            }



            _bgWorker.DoWork += (s, e) =>
            {
                ProgressCPU = (int)cpuCounter.NextValue();
                ProgressMEM = (int)memCounter.NextValue(); // = Used + Cashed
                ProgressHDD = (int)hddCounter.NextValue();
            };
        }

        public void GetCpuTemp(object sender, EventArgs e)
        {
            var computer = new Computer // ИСПОЛЬЗУЕМ КАСТОМНУЮ БИБЛИОТЕКУ OPENHARDWARE
            {
                MainboardEnabled = false,
                CPUEnabled = false,
                RAMEnabled = false,
                GPUEnabled = false,
                FanControllerEnabled = false,
                HDDEnabled = true
            };

            computer.Open();

            var temps = new List<string>();

            foreach (var item in computer.Hardware)
            {
                temps.AddRange(from sensor in item.Sensors where sensor.SensorType == SensorType.Load where sensor.Value != null select sensor.Value.Value.ToString(CultureInfo.CurrentCulture));
                //temps.AddRange(from sensor in item.Sensors where sensor.SensorType == SensorType.Temperature where sensor.Value != null select sensor.Value.Value.ToString(CultureInfo.CurrentCulture));
            }



            computer.Close();

            //textBoxHard1.Text = temps[0];
            //textBoxHard2.Text = temps[1];
            //textBoxHard3.Text = temps[2];
            //textBoxHard4.Text = temps[3];
            //textBoxHard5.Text = temps[4];
            //textBoxHard6.Text = temps[5];

        }

        #region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

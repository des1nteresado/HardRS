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
        private BackgroundWorker _bgWorker = new BackgroundWorker();

        private int _progressCPU;
        private int _progressMEM;
        private int _progressHDD;
        private string _tempCPU;

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

        public string TempCPU
        {
            get
            {
                return _tempCPU;
            }
            set
            {
                _tempCPU = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TempCPU"));
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

            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true,
                HDDEnabled = true
            };

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSStorageDriver_ATAPISmartData WHERE Active=True");

            try
            {
                _bgWorker.RunWorkerAsync();
            }
#pragma warning disable CS0168 // Переменная "e" объявлена, но ни разу не использована.
            catch (Exception e)
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
                TempCPU = "";
                //foreach (ManagementObject obj in searcher.Get())
                //{
                //    byte[] vendorSpec = obj["VendorSpecific"] as byte[];
                //    if (vendorSpec != null)
                //    {
                //        Console.WriteLine("Температура SSD = " + vendorSpec[115]);
                //    }

                //}

                computer.Open();
                computer.Accept(updateVisitor);

                for (int i = 0; i < computer.Hardware.Length; i++)
                {
                    if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                    {
                        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                        {
                            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                                TempCPU += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                        }
                    }
                    if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia || computer.Hardware[i].HardwareType == HardwareType.GpuAti)
                    {
                        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                        {
                            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                                TempCPU += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                        }
                    }
                    if (computer.Hardware[i].HardwareType == HardwareType.HDD)
                    {
                        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                        {
                            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                                TempCPU += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                        }
                    }
                }
                computer.Close();
            };
        }


        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
       

        //----------------------------------------------это моё
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

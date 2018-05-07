using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private BackgroundWorker _bgWorker = new BackgroundWorker(); //async-worker

        ManagementObjectSearcher videoSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
        ManagementObjectSearcher processorSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
        ManagementObjectSearcher memorySrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
        ManagementObjectSearcher diskSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");

        public ObservableCollection<Hardware> Hardwares { get; set; }
        public ObservableCollection<Processor> Processors { get; set; }

        public ObservableCollection<VideoController> VideoControllers { get; set; }
        public ObservableCollection<Memory> Memoryes { get; set; }
        public ObservableCollection<DiskDrive> DiskDrives { get; set; }

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
            Processor[] processor = new Processor[processorSrch.Get().Count];
            VideoController[] video = new VideoController[videoSrch.Get().Count];
            Memory[] memory = new Memory[memorySrch.Get().Count];
            DiskDrive[] diskDrive = new DiskDrive[diskSrch.Get().Count];
           
            Processors = new ObservableCollection<Processor>();
            VideoControllers = new ObservableCollection<VideoController>();
            Memoryes = new ObservableCollection<Memory>();
            DiskDrives = new ObservableCollection<DiskDrive>();

            int c = 0;

            foreach (ManagementObject queryObj in videoSrch.Get())
            {
                Console.WriteLine(videoSrch.Get().Count);
                video[c] = new VideoController();
                video[c].Title = "Video";
                video[c].VcName = "Name: " + queryObj["Caption"];
                video[c].VcMemory = "Memory: " + Convert.ToDouble(queryObj["AdapterRAM"]) / 1024 / 1024 + " MB";
                video[c].VcCpu = "Processor: " + queryObj["VideoProcessor"];
                VideoControllers.Add(video[c]);
                c++;
            }

            int c1 = 0;
            foreach (ManagementObject queryObj in processorSrch.Get())
            {
                processor[c1] = new Processor();
                processor[c1].Title = "Processor";
                processor[c1].CpuName = "Name: " + queryObj["Name"];
                processor[c1].CpuCores = "Number Of Cores: " + queryObj["NumberOfCores"];
                processor[c1].CpuId = "Processor Id: " + queryObj["ProcessorId"];
                Processors.Add(processor[c1]);
                c1++;
            }

            int c2 = 0;
            foreach (ManagementObject queryObj in memorySrch.Get())
            {
                memory[c2] = new Memory();
                memory[c2].Title = "Physical Memory";
                memory[c2].MBar = "Memory bar #" + (c2 + 1);
                memory[c2].MCapacity = "Capacity: " + Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2) + " GB ";
                memory[c2].MSpeed = "Speed: " + queryObj["Speed"];
                Memoryes.Add(memory[c2]);
                c2++;
            }

            int c3= 0;
            foreach (ManagementObject queryObj in diskSrch.Get())
            {
                diskDrive[c3] = new DiskDrive();
                diskDrive[c3].Title = "Disk Drive";
                diskDrive[c3].DiskModel = "Model:" + queryObj["Model"];
                diskDrive[c3].DiskSize = "Size:" + Math.Round(Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2) + " Gb";
                diskDrive[c3].DiskSerialNumber = "SerialNumber:" + queryObj["SerialNumber"];
                DiskDrives.Add(diskDrive[c3]);
                c3++;
            }
            //Hardwares = new ObservableCollection<Hardware>
            //{
            //    new Hardware
            //    {
            //    Processor = processor,
            //    VideoController = video,
            //    Memory = memory,
            //    DiskDrive = diskDrive
            //    }
            //};
      

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

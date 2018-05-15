using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Threading;
using HardRS.HardwareManager;
using HardRS.HardwareManager.Classes;
using OpenHardwareMonitor.Hardware;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlotDemo.Annotations;


namespace HardRS.HardwareManager
{
    public class HManagerViewModel : INotifyPropertyChanged
    {
        private BackgroundWorker _bgWorker = new BackgroundWorker(); //async-worker

        //public List<DataPoint> Points { get; set; }
        //public List<DataPoint> Points2 { get; set; }
        //public List<LineSeries> ListLine { get; set; }

        //double popl = 0;
        //double popl2;
        //double popl22;

        ManagementObjectSearcher videoSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
        ManagementObjectSearcher processorSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
        ManagementObjectSearcher memorySrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
        ManagementObjectSearcher diskSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");

        public List<Hardware> HardwareList;
        public Progress Progress { get; set; }
        //public LineSeries Lines { get; set; }

        //private PlotModel plotModel;

        //public PlotModel PlotModel
        //{
        //    get { return plotModel; }
        //    set { plotModel = value; OnPropertyChanged("PlotModel"); }
        //}

        private string temp;

        public string Temp
        {
            get
            {
                return temp;
            }
            set
            {
                temp = value;

                OnPropertyChanged("Temp");
            }
        }

        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        private DateTime lastUpdate = DateTime.Now;

        private readonly List<OxyColor> colors = new List<OxyColor>
                                            {
                                                OxyColors.Green,
                                                OxyColors.IndianRed,
                                                OxyColors.Coral,
                                                OxyColors.Chartreuse,
                                                OxyColors.Azure
                                            };

        private readonly List<MarkerType> markerTypes = new List<MarkerType>
                                                   {
                                                       MarkerType.Plus,
                                                       MarkerType.Star,
                                                       MarkerType.Diamond,
                                                       MarkerType.Triangle,
                                                       MarkerType.Cross
                                                   };


        private void SetUpModel()
        {
            PlotModel.LegendTitle = "Legend";
            PlotModel.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.LegendPlacement = LegendPlacement.Outside;
            PlotModel.LegendPosition = LegendPosition.TopRight;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.LegendBorder = OxyColors.Black;
            var endDate = DateTime.Now.AddMinutes(-3);
            double minDate = DateTimeAxis.ToDouble(endDate);
            //Вертикальная вид линии решетки      //Горизонтальная
            var dateAxis = new DateTimeAxis(AxisPosition.Bottom, "Date", "HH:mm") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 60 };
            PlotModel.Axes.Add(dateAxis);
            var valueAxis = new LinearAxis(AxisPosition.Left, 30) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Maximum=100, Title = "Value" };
            PlotModel.Axes.Add(valueAxis);

        }

        private void LoadData()
        {
            //List<Measurement> measurements = Data.GetData();
            List<Measurement> measurements = Data.GetUpdateData(lastUpdate);
            var dataPerDetector = measurements.GroupBy(m => m.DetectorId).OrderBy(m => m.Key).ToList();
            foreach (var data in dataPerDetector)
            {
                var lineSerie = new LineSeries
                {
                    StrokeThickness = 2,
                    MarkerSize = 3,
                    MarkerStroke = colors[data.Key],
                    MarkerType = markerTypes[data.Key],
                    CanTrackerInterpolatePoints = false,
                    Title = string.Format("Detector {0}", data.Key),
                    Smooth = false,
                };

                data.ToList().ForEach(d => lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.DateTime), d.Value)));
                PlotModel.Series.Add(lineSerie);
            }
            Console.WriteLine("///////////////1////////////");
            Console.WriteLine(PlotModel.Series.Count);
            lastUpdate = DateTime.Now;
        }

        public void UpdateModel()
        {
            List<Measurement> measurements = Data.GetUpdateData(lastUpdate);
            var dataPerDetector = measurements.GroupBy(m => m.DetectorId).OrderBy(m => m.Key).ToList();

            foreach (var data in dataPerDetector)
            {
                Console.WriteLine("/////////////2/////////////");
                Console.WriteLine(PlotModel.Series.Count);
                if(PlotModel.Series.Count == 0)
                {
                    PlotModel = new PlotModel();
                    SetUpModel();
                    LoadData();
                }
                else
                {
                    if(PlotModel.Series.Count == data.Key)
                    {
                        var lineSerie = PlotModel.Series[data.Key-1] as LineSeries;
                        if (lineSerie != null)
                        {
                            data.ToList()
                                .ForEach(d => lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.DateTime), d.Value)));
                        }
                    }
                    else
                    {
                        var lineSerie = PlotModel.Series[data.Key] as LineSeries;
                        if (lineSerie != null)
                        {
                            data.ToList()
                                .ForEach(d => lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.DateTime), d.Value)));
                        }
                    }
                    
                }
            }
            lastUpdate = DateTime.Now;
        }


        public HManagerViewModel()
        {
       


            Processor[] processor = new Processor[processorSrch.Get().Count];
            VideoController[] video = new VideoController[videoSrch.Get().Count];
            Memory[] memory = new Memory[memorySrch.Get().Count];
            DiskDrive[] diskDrive = new DiskDrive[diskSrch.Get().Count];
            HardwareList = new List<Hardware>();
            Progress = new Progress();
            //Points = new List<DataPoint>();
            //Points2 = new List<DataPoint>();
            //ListLine = new List<LineSeries>();


            PlotModel = new PlotModel { Title = "Температура датчиков" };


            int c = 0;

            foreach (ManagementObject queryObj in videoSrch.Get())
            {
                Console.WriteLine(videoSrch.Get().Count);
                video[c] = new VideoController();
                video[c].Type = "Video";
                video[c].Name = queryObj["Caption"].ToString();
                video[c].VcMemory = "Memory: " + Convert.ToDouble(queryObj["AdapterRAM"]) / 1024 / 1024 + " MB";
                video[c].VcCpu = "Processor: " + queryObj["VideoProcessor"];
                HardwareList.Add(video[c]);
                //hardware.VideoControllers.Add(video[c].VcName);
                c++;
            }

            int c1 = 0;
            foreach (ManagementObject queryObj in processorSrch.Get())
            {
                processor[c1] = new Processor();
                processor[c1].Type = "Processor";
                processor[c1].Name = queryObj["Name"].ToString();
                processor[c1].CpuCores = "Number Of Cores: " + queryObj["NumberOfCores"];
                processor[c1].CpuId = "Processor Id: " + queryObj["ProcessorId"];
                HardwareList.Add(processor[c1]);
                c1++;
            }

            int c2 = 0;
            foreach (ManagementObject queryObj in memorySrch.Get())
            {
                memory[c2] = new Memory();
                memory[c2].Type = "Physical Memory";
                memory[c2].Name = "Memory bar #" + (c2 + 1);
                memory[c2].MCapacity = "Capacity: " + Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2) + " GB ";
                memory[c2].MSpeed = "Speed: " + queryObj["Speed"];
                HardwareList.Add(memory[c2]);
                c2++;
            }

            int c3 = 0;
            foreach (ManagementObject queryObj in diskSrch.Get())
            {
                diskDrive[c3] = new DiskDrive();
                diskDrive[c3].Type = "Disk Drive";
                diskDrive[c3].Name = queryObj["Model"].ToString();
                diskDrive[c3].DiskSize = "Size:" + Math.Round(Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2) + " Gb";
                diskDrive[c3].DiskSerialNumber = "SerialNumber:" + queryObj["SerialNumber"];
                HardwareList.Add(diskDrive[c3]);
                c3++;
            }

            Console.WriteLine(HardwareList[0].Type);

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

            //ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSStorageDriver_ATAPISmartData WHERE Active=True");

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
                Progress.ProgressCPU = (int)cpuCounter.NextValue();
                Progress.ProgressMEM = (int)memCounter.NextValue(); // = Used + Cashed
                Progress.ProgressHDD = (int)hddCounter.NextValue();

                //popl2 = Progress.ProgressMEM;
                //popl22 = Progress.ProgressCPU;

                //GetSeries(PlotModel, Points, popl, popl2, popl22, OxyColors.Green, OxyColors.Red);

                //GetSeries(PlotModel, Points, popl, popl22, OxyColors.Red);


                //string heh2 = "";
                //foreach (ManagementObject obj in searcher.Get())
                //{
                //    byte[] vendorSpec = obj["VendorSpecific"] as byte[];
                //    if (vendorSpec != null)
                //    {
                //        Console.WriteLine("Температура SSD = " + vendorSpec[115]);
                //    }

                //}

                //computer.Open();
                //computer.Accept(updateVisitor);

                //for (int i = 0; i < computer.Hardware.Length; i++)
                //{
                //    if (computer.Hardware[i].HardwareType == HardwareType.CPU)//temp of cpu
                //    {
                //        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                //        {
                //            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                //                heh2 += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                //        }
                //    }
                //    if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia || computer.Hardware[i].HardwareType == HardwareType.GpuAti)
                //    { //temp of videocard
                //        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                //        {
                //            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                //                heh2 += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                //        }
                //    }
                //    if (computer.Hardware[i].HardwareType == HardwareType.HDD)//temp of hdd
                //    {
                //        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                //        {
                //            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                //            {
                //                heh2 += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                //                //popl2 = (double)computer.Hardware[i].Sensors[j].Value;
                //                //Lines.Points.Add(new DataPoint(popl, popl2));
                //                //PlotModel.Series.Add(Lines);
                //                //PlotModel.InvalidatePlot(true);
                //                //DataPoint1 = new DataPoint(popl, popl2);
                //                //Points.Add(new DataPoint(popl, popl2));
                //            }
                //        }
                //    }
                //}
                //Temp = heh2;
                //popl += 1;
                computer.Close();
            };
        }


        //public void GetSeries(PlotModel PlotModel, List<DataPoint> list, double x, double y, double y2, OxyColor color2, OxyColor color)
        //    {
        //        ListLine = new List<LineSeries>();
        //        var series = new LineSeries
        //        {
        //            Color = color
        //        };
        //        //list.Add(new DataPoint(x, y));
        //        //series.Title = "heh" + y.ToString();
        //        //series.YAxisKey = "heh" + y.ToString();
        //        //series.ItemsSource = list;
        //        series.Points.Add(new DataPoint(x, y));
        //        series.Points.Add(new DataPoint(x, y2));

        //        ListLine.Add(series);
        //        foreach (var s in ListLine)
        //        {
        //            PlotModel.Series.Add(s);
        //            PlotModel.InvalidatePlot(true);
        //        }
        //        }




        //----------------------------------------------это моё
        //public void GetCpuTemp(object sender, EventArgs e)
        //{
        //    var computer = new Computer // ИСПОЛЬЗУЕМ КАСТОМНУЮ БИБЛИОТЕКУ OPENHARDWARE
        //    {
        //        MainboardEnabled = false,
        //        CPUEnabled = false,
        //        RAMEnabled = false,
        //        GPUEnabled = false,
        //        FanControllerEnabled = false,
        //        HDDEnabled = true
        //    };

        //    computer.Open();

        //    var temps = new List<string>();

        //    foreach (var item in computer.Hardware)
        //    {
        //        temps.AddRange(from sensor in item.Sensors where sensor.SensorType == SensorType.Load where sensor.Value != null select sensor.Value.Value.ToString(CultureInfo.CurrentCulture));
        //        //temps.AddRange(from sensor in item.Sensors where sensor.SensorType == SensorType.Temperature where sensor.Value != null select sensor.Value.Value.ToString(CultureInfo.CurrentCulture));
        //    }



            //computer.Close();

            //textBoxHard1.Text = temps[0];
            //textBoxHard2.Text = temps[1];
            //textBoxHard3.Text = temps[2];
            //textBoxHard4.Text = temps[3];
            //textBoxHard5.Text = temps[4];
            //textBoxHard6.Text = temps[5];

        //}

        public ICollectionView Hardwares
        {
            get
            {
                var source = CollectionViewSource.GetDefaultView(this.HardwareList);
                source.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                return source;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

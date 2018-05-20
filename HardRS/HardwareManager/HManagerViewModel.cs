using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Windows.Data;
using System.Windows.Threading;
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

        #region объекты для получения инфы о системе
        ManagementObjectSearcher videoSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
        ManagementObjectSearcher processorSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
        ManagementObjectSearcher memorySrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
        ManagementObjectSearcher diskSrch = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
        #endregion
        public List<Hardware> HardwareList;
        public Progress Progress { get; set; }
        public int counter = 0;
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

        private string mTemp;
        public string MTemp
        {
            get
            {
                return mTemp;
            }
            set
            {
                mTemp = value;

                OnPropertyChanged("MTemp");
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
            var endDate = DateTime.Now;
            double minDate = DateTimeAxis.ToDouble(DateTime.Now);
            //Вертикальная вид линии решетки      //Горизонтальная
            var dateAxis = new DateTimeAxis(AxisPosition.Bottom, "Date", "HH:mm:ss") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 60 };
            PlotModel.Axes.Add(dateAxis);
            var valueAxis = new LinearAxis(AxisPosition.Left, 30) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Maximum = 100, Title = "Value" };
            PlotModel.Axes.Add(valueAxis);
        }

        private void LoadData()
        {
            List<Measurement> measurements = Data.GetUpdateData(lastUpdate);
            foreach (var m in measurements)
            {
                var lineSerie = new LineSeries
                {
                    StrokeThickness = 2,
                    MarkerSize = 3,
                    MarkerStroke = colors[m.DetectorId],
                    MarkerType = markerTypes[m.DetectorId],
                    CanTrackerInterpolatePoints = false,
                    Title = string.Format("{0}", m.Name),
                    Smooth = false,
                };

                lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(m.DateTime), m.Value));
                PlotModel.Series.Add(lineSerie);
            }
            lastUpdate = DateTime.Now;
        }

        public void UpdateModel()
        {
            
            List<Measurement> measurements = Data.GetUpdateData(lastUpdate);
            Temp = measurements[0].Temp;
            MTemp = MaxTemp.MTemp;
            foreach (var m in measurements)
            {
                if (PlotModel.Series.Count == 0)
                {
                    PlotModel = new PlotModel();
                    SetUpModel();
                    LoadData();
                }
                else
                {
                    if (PlotModel.Series.Count == m.DetectorId)
                    {
                        var lineSerie = PlotModel.Series[m.DetectorId - 1] as LineSeries;
                        if (lineSerie != null)
                        {
                            lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(m.DateTime), m.Value));
                            PlotModel.InvalidatePlot(true);
                        }
                    }
                    else
                    {
                        if (PlotModel.Series[m.DetectorId] is LineSeries lineSerie)
                        {
                            lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(m.DateTime), m.Value));
                        }
                    }

                }
            }
            lastUpdate = DateTime.Now;
            for(int i = 0; i < PlotModel.Series.Count; i++)
            {
                if ((PlotModel.Series[i] as LineSeries).Points.Count > 10) //show only 10 last points 
                    (PlotModel.Series[i] as LineSeries).Points.RemoveAt(0); //remove first point 
            }
        }

        public HManagerViewModel()
        {
            Processor[] processor = new Processor[processorSrch.Get().Count];
            VideoController[] video = new VideoController[videoSrch.Get().Count];
            Memory[] memory = new Memory[memorySrch.Get().Count];
            DiskDrive[] diskDrive = new DiskDrive[diskSrch.Get().Count];
            HardwareList = new List<Hardware>();
            Progress = new Progress();

            PlotModel = new PlotModel { Title = "Температура датчиков" };
            #region получаем значения о системе
            foreach (ManagementObject queryObj in videoSrch.Get())
            {
                HardwareList.Add(new VideoController()
                {
                    Type = "Video",
                    Name = queryObj["Caption"].ToString(),
                    VcMemory = "Memory: " + Convert.ToDouble(queryObj["AdapterRAM"]) / 1024 / 1024 + " MB",
                    VcCpu = "Processor: " + queryObj["VideoProcessor"],
                });
            }

            foreach (ManagementObject queryObj in processorSrch.Get())
            {
                HardwareList.Add(new Processor()
                {
                    Type = "Processor",
                    Name = queryObj["Name"].ToString(),
                    CpuCores = "Number Of Cores: " + queryObj["NumberOfCores"],
                    CpuId = "Processor Id: " + queryObj["ProcessorId"]
                });
            }

            int memCount = 0;
            foreach (ManagementObject queryObj in memorySrch.Get())
            {
                HardwareList.Add(new Memory()
                {
                    Type = "Physical Memory",
                    Name = "Memory bar #" + (memCount + 1),
                    MCapacity = "Capacity: " + Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2) + " GB ",
                    MSpeed = "Speed: " + queryObj["Speed"]
                });
                memCount++;
            }

            foreach (ManagementObject queryObj in diskSrch.Get())
            {
                HardwareList.Add(new DiskDrive()
                {
                    Type = "Disk Drive",
                    Name = queryObj["Model"].ToString(),
                    DiskSize = "Size:" + Math.Round(Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2) + " Gb",
                    DiskSerialNumber = "SerialNumber:" + queryObj["SerialNumber"]
                });
            }
            #endregion

            _bgWorker.WorkerSupportsCancellation = true;
            _bgWorker.CancelAsync();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs y)
        {
            #region обьявление классов для баров
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

            UpdateVisitor updateVisitor1 = new UpdateVisitor();
            Computer computer1 = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = false,
                HDDEnabled = true
            };
            #endregion
            #region Значения баров
            try
            {
                _bgWorker.RunWorkerAsync();
            }
            catch (Exception)
            {
                _bgWorker.WorkerSupportsCancellation = true;
                _bgWorker.CancelAsync();
            }
            _bgWorker.DoWork += (s, e) =>
            {
                Progress.ProgressCPU = (int)cpuCounter.NextValue();
                Progress.ProgressMEM = (int)memCounter.NextValue(); // = Used + Cashed
                Progress.ProgressHDD = (int)hddCounter.NextValue();
                if (_bgWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            };
            #endregion
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

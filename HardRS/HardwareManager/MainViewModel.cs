using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using CircularProgressBarApp.mvvmSupport;

namespace HardRS.CircularProgressBar
{
    public class MainViewModel : INotifyPropertyChanged
    {
        PerformanceCounter cpuSCounter = new PerformanceCounter("Сведения о процессоре", "Частота процессора", "_Total");

        private BackgroundWorker _bgWorker = new BackgroundWorker();

        private int _workerState;

        public int WorkerState
        {
            get
            {
                return _workerState;
            }
            set
            {
                _workerState = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("WorkerState"));
                    PropertyChanged(this, new PropertyChangedEventArgs("WorkerText"));
                }
            }
        }

        public string WorkerText
        {
            get { return string.Format("{0} GHz", _workerState); }
        }

        public MainViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs y)
        {
            _bgWorker.RunWorkerAsync();

            _bgWorker.DoWork += (s, e) =>
            {
                WorkerState = (int)cpuSCounter.NextValue();
            };
        }

        #region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

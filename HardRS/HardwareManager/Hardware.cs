using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardRS.HardwareManager
{
    class Hardware : INotifyPropertyChanged
    {
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
                if (value != 0)
                {
                    _progressHDD = value;
                }
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

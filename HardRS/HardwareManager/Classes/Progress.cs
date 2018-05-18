using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HardRS.HardwareManager.Classes
{
    public class Progress : INotifyPropertyChanged
    {
        private int progressCPU;
        private int progressMEM;
        private int progressHDD;

        public int ProgressCPU
        {
            get
            {
                return progressCPU;
            }
            set
            {
                if (value != 0)
                {
                    progressCPU = value;
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
                return progressMEM;
            }
            set
            {
                if (value != 0)
                {
                    progressMEM = value;
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
                return progressHDD;
            }
            set
            {
                progressHDD = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressHDD"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressHDDText"));
                }
            }
        }


        public string ProgressCPUText
        {
            get { return string.Format("{0} %", progressCPU); }
        }
        public string ProgressMEMText
        {
            get { return string.Format("{0} %", progressMEM); }
        }
        public string ProgressHDDText
        {
            get { return string.Format("{0} %", progressHDD); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HardRS.HardwareManager
{
    public class Processor : Hardware, INotifyPropertyChanged
    {
        private string cpuName;
        private string cpuCores;
        private string cpuId;

        public string CpuName
        {
            get
            {
                return cpuName;
            }
            set
            {
                cpuName = value;
                OnPropertyChanged("CpuName");
            }
        }
        public string CpuCores
        {
            get
            {
                return cpuCores;
            }
            set
            {
                cpuCores = value;
                OnPropertyChanged("CpuCores");
            }
        }
        public string CpuId
        {
            get
            {
                return cpuId;
            }
            set
            {
                cpuId = value;
                OnPropertyChanged("CpuId");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public new void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

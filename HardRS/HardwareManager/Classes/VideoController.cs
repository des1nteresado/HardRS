using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HardRS.HardwareManager
{
    public class VideoController : Hardware, INotifyPropertyChanged
    {
        private string vcName;
        private string vcMemory;
        private string vcCpu;

        public string VcName
        {
            get
            {
                return vcName;
            }
            set
            {
                vcName = value;
                OnPropertyChanged("VcName");
            }
        }
        public string VcMemory
        {
            get
            {
                return vcMemory;
            }
            set
            {
                vcMemory = value;
                OnPropertyChanged("VcMemory");
            }
        }
        public string VcCpu
        {
            get
            {
                return vcCpu;
            }
            set
            {
                vcCpu = value;
                OnPropertyChanged("VcCpu");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public new void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HardRS.HardwareManager
{
    public class DiskDrive : Hardware, INotifyPropertyChanged
    {
        private string diskModel;
        private string diskSize;
        private string diskSerialNumber;


        public string DiskModel
        {
            get
            {
                return diskModel;
            }
            set
            {
                diskModel = value;
                OnPropertyChanged("DiskModel");
            }
        }
        public string DiskSize
        {
            get
            {
                return diskSize;
            }
            set
            {
                diskSize = value;
                OnPropertyChanged("DiskSize");
            }
        }
        public string DiskSerialNumber
        {
            get
            {
                return diskSerialNumber;
            }
            set
            {
                diskSerialNumber = value;
                OnPropertyChanged("DiskSerialNumber");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public new void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

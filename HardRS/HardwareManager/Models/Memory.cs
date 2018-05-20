using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HardRS.HardwareManager
{
    public class Memory : Hardware, INotifyPropertyChanged
    {
        private string mBar;
        private string mCapacity;
        private string mSpeed;

        public string MBar
        {
            get
            {
                return mBar;
            }
            set
            {
                mBar = value;
                OnPropertyChanged("MBar");
            }
        }
        public string MCapacity
        {
            get
            {
                return mCapacity;
            }
            set
            {
                mCapacity = value;
                OnPropertyChanged("MCapacity");
            }
        }
        public string MSpeed
        {
            get
            {
                return mSpeed;
            }
            set
            {
                mSpeed = value;
                OnPropertyChanged("MSpeed");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public new void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

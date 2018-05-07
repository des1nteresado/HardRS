using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HardRS.HardwareManager
{
    public class Memory : INotifyPropertyChanged
    {
        private string title;
        private string mBar;
        private string mCapacity;
        private string mSpeed;


        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }
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
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HardRS.HardwareManager
{
    class Hardware : INotifyPropertyChanged
    {
        private string processor;
        private string videoController;
        private string physMemory;
        private string diskDrive;


        public string Processor
        {
            get
            {
                return processor;
            }
            set
            {
                processor = value;
                OnPropertyChanged("Processor");
            }
        }
        public string VideoController
        {
            get
            {
                return videoController;
            }
            set
            {
                videoController = value;
                OnPropertyChanged("VideoController");
            }
        }
        public string PhysMemory
        {
            get
            {
                return physMemory;
            }
            set
            {
                physMemory = value;
                OnPropertyChanged("PhysMemory");
            }
        }
        public string DiskDrive
        {
            get
            {
                return diskDrive;
            }
            set
            {
                diskDrive = value;
                OnPropertyChanged("DiskDrive");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

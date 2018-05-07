using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HardRS.HardwareManager
{
    public class Processor : INotifyPropertyChanged
    {
        private string title;
        private string cpuName;
        private string cpuCores;
        private string cpuId;


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
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

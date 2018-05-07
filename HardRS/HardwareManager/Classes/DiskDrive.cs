using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HardRS.HardwareManager
{
    
            //foreach (ManagementObject queryObj in searcher13.Get())
            //{
            //    ListOfHardware.Items.Add("--------- Win32_DiskDrive instance ---------------");
            //    ListOfHardware.Items.Add("Model:" + queryObj["Model"] + 
            //    "\nDeviceID:" + queryObj["DeviceID"] +
            //    "\nInterfaceType:" + queryObj["InterfaceType"] +
            //    "\nManufacturer:" + queryObj["Manufacturer"] +
            //    "\nSerialNumber:" + queryObj["SerialNumber"] +
            //    "\nSize:" + Math.Round(Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2) + " Gb");
            //}
public class DiskDrive : INotifyPropertyChanged
    {
        private string title;
        private string diskModel;
        private string diskSize;
        private string diskSerialNumber;


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
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

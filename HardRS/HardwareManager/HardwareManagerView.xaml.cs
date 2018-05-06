using HardRS.CircularProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HardRS.HardwareManager
{
    /// <summary>
    /// Логика взаимодействия для HardwareManagerView.xaml
    /// </summary>
    public partial class HardwareManagerView : UserControl
    {
        ManagementObjectSearcher searcher11 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
        ManagementObjectSearcher searcher8 =  new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
        ManagementObjectSearcher searcher12 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
        ManagementObjectSearcher searcher13 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");

        public HardwareManagerView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                ListOfHardware.Items.Add("----------- Win32_VideoController instance -----------");
                ListOfHardware.Items.Add("Name: " + queryObj["Caption"]);
                ListOfHardware.Items.Add("Memory: " + Convert.ToDouble(queryObj["AdapterRAM"])/1024/1024 + " MB");
                ListOfHardware.Items.Add("Processor: " + queryObj["VideoProcessor"]);
            }
            foreach (ManagementObject queryObj in searcher8.Get())
            {
                ListOfHardware.Items.Add("------------- Win32_Processor instance ---------------");
                ListOfHardware.Items.Add("Name: " + queryObj["Name"]);
                ListOfHardware.Items.Add("Number Of Cores: " + queryObj["NumberOfCores"]);
                ListOfHardware.Items.Add("Processor Id: " + queryObj["ProcessorId"]);
            }
            foreach (ManagementObject queryObj in searcher12.Get())
            {
                ListOfHardware.Items.Add("------------- Win32_PhysicalMemory instance --------");

                ListOfHardware.Items.Add("Memory bar \n" + "Capacity: " +
                                  Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2) + " GB " +
                                  "\nSpeed: " + queryObj["Speed"]);
            }
     


            foreach (ManagementObject queryObj in searcher13.Get())
            {
                ListOfHardware.Items.Add("--------- Win32_DiskDrive instance ---------------");
                ListOfHardware.Items.Add("Model:" + queryObj["Model"] + 
                "\nDeviceID:" + queryObj["DeviceID"] +
                "\nInterfaceType:" + queryObj["InterfaceType"] +
                "\nManufacturer:" + queryObj["Manufacturer"] +
                "\nSerialNumber:" + queryObj["SerialNumber"] +
                "\nSize:" + Math.Round(Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2) + " Gb");
            }
           
        }
    }
}

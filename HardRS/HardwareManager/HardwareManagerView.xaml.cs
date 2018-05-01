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
        ManagementObjectSearcher searchercputemp = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");

        public HardwareManagerView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                ListOfHardware.Items.Add("----------- Win32_VideoController instance -----------");
                ListOfHardware.Items.Add("AdapterRAM:" + queryObj["AdapterRAM"]);
                ListOfHardware.Items.Add("Caption:" + queryObj["Caption"]);
                ListOfHardware.Items.Add("Description:" + queryObj["Description"]);
                ListOfHardware.Items.Add("VideoProcessor:" + queryObj["VideoProcessor"]);
            }
            foreach (ManagementObject queryObj in searcher8.Get())
            {
                ListOfHardware.Items.Add("------------- Win32_Processor instance ---------------");
                ListOfHardware.Items.Add("Name:" + queryObj["Name"]);
                ListOfHardware.Items.Add("NumberOfCores:" + queryObj["NumberOfCores"]);
                ListOfHardware.Items.Add("ProcessorId:" + queryObj["ProcessorId"]);
            }
            foreach (ManagementObject queryObj in searcher12.Get())
            {
                ListOfHardware.Items.Add("------------- Win32_PhysicalMemory instance --------");

                ListOfHardware.Items.Add("BankLabel:" + queryObj["BankLabel"] + " Capacity:" +
                                  Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2) + " GB " +
                                  "Speed:" + queryObj["Speed"]);
            }
     


            foreach (ManagementObject queryObj in searcher13.Get())
            {
                ListOfHardware.Items.Add("--------- Win32_DiskDrive instance ---------------");
                ListOfHardware.Items.Add("DeviceID:" + queryObj["DeviceID"] +
                " InterfaceType:" + queryObj["InterfaceType"] +
                " Manufacturer:" + queryObj["Manufacturer"] +
                " Model:" + queryObj["Model"] +
                " SerialNumber:" + queryObj["SerialNumber"] +
                " Size:" + Math.Round(Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2) + " Gb");
            }
            foreach (ManagementObject obj in searchercputemp.Get())
            {
                Double temp = Convert.ToDouble(obj["CurrentTemperature"].ToString()) - 2732;
                temp = temp * 0.1;
                string name = obj["InstanceName"].ToString();
                ListOfHardware2.Items.Add(name + " " + temp);
                // ну, и в TList теперь их (temp и name), к примеру
            }
        }
    }
}

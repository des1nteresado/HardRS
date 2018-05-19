using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Windows;
using static HardRS.HardwareManager.HManagerViewModel;

namespace HardRS.HardwareManager
{
    public static class MaxTemp
    {
        public static string MTemp { get; set; }
        public static int[] MTemps { get; set; }

        static MaxTemp()
        {
            MTemp = "";
            MTemps = new int[10];
            for (int i = 0; i < 10; i++)
            {
                MTemps[i] = 0;
            }
        }
    }

    public class Data
    {
        public static List<Measurement> GetUpdateData(DateTime dateTime)
        {
            var measurements = new List<Measurement>();
            string t = "";
            int counter = 0;
            MaxTemp.MTemp = "";
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true,
                HDDEnabled = true
            };

            computer.Open();
            computer.Accept(updateVisitor);

            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)//temp of cpu
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            if (computer.Hardware[i].Sensors[j].Name.Contains("CPU Package"))
                            {
                                t += "Cpu Total: " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                            }
                            else
                                t += computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                            if (MaxTemp.MTemps[counter] < computer.Hardware[i].Sensors[j].Value)
                            {
                                MaxTemp.MTemps[counter] = (int)computer.Hardware[i].Sensors[j].Value;
                                if (computer.Hardware[i].Sensors[j].Name.Contains("CPU Package"))
                                {
                                    MaxTemp.MTemp += "Max " + "Cpu Total: " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                                }
                                else
                                    MaxTemp.MTemp += "Max " + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                            }
                            else
                            {
                                if (computer.Hardware[i].Sensors[j].Name.Contains("CPU Package"))
                                {
                                    MaxTemp.MTemp += "Max " + "Cpu Total: " + MaxTemp.MTemps[counter] + "\n";
                                }
                                else
                                MaxTemp.MTemp += "Max " + computer.Hardware[i].Sensors[j].Name + ": " + MaxTemp.MTemps[counter] + "\n";
                            }
                        }
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature && computer.Hardware[i].Sensors[j].Name.Contains("CPU Package"))
                        {
                            measurements.Add(new Measurement()
                            {
                                DetectorId = counter,
                                DateTime = dateTime.AddSeconds(1),
                                Value = (int)computer.Hardware[i].Sensors[j].Value,
                                Name = computer.Hardware[i].Sensors[j].Name
                            });
                            counter++;
                            //heh2 += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                        }
                     
                    }
                }
                if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia || computer.Hardware[i].HardwareType == HardwareType.GpuAti)
                { //temp of videocard
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            if((int)computer.Hardware[i].Sensors[j].Value != 0)
                            {
                                if (MaxTemp.MTemps[counter] < computer.Hardware[i].Sensors[j].Value)
                                {
                                    MaxTemp.MTemps[counter] = (int)computer.Hardware[i].Sensors[j].Value;
                                    MaxTemp.MTemp += "Max " + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                                }
                                else
                                {
                                    MaxTemp.MTemp += "Max " + computer.Hardware[i].Sensors[j].Name + ": " + MaxTemp.MTemps[counter] + "\n";
                                }
                                measurements.Add(new Measurement()
                                {
                                    DetectorId = counter,
                                    DateTime = dateTime.AddSeconds(1),
                                    Value = (int)computer.Hardware[i].Sensors[j].Value,
                                    Name = computer.Hardware[i].Sensors[j].Name
                                });
                                counter++;
                                t += computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                            }
                        }
                    }
                }
                if (computer.Hardware[i].HardwareType == HardwareType.HDD)//temp of disk
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            if (MaxTemp.MTemps[counter] < computer.Hardware[i].Sensors[j].Value)
                            {
                                MaxTemp.MTemps[counter] = (int)computer.Hardware[i].Sensors[j].Value;
                                MaxTemp.MTemp += "Max " + "Storage" +": " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                            }
                            else
                            {
                                MaxTemp.MTemp += "Max " + "Storage" + ": " + MaxTemp.MTemps[counter] + "\n";
                            }
                            try
                            {
                                measurements.Add(new Measurement()
                                {
                                    DetectorId = counter,
                                    DateTime = dateTime.AddSeconds(1),
                                    Value = (int)computer.Hardware[i].Sensors[j].Value,
                                    Name = computer.Hardware[i].Sensors[j].Name
                                });
                                counter++;
                                t += "Storage" + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                            }
                            catch(Exception e)
                            {
                                MessageBox.Show("Диск сильно нагружен! " + e.Message);
                            }
                        }
                    }
                }
            }
            measurements[0].Temp = t;
            computer.Close();
            return measurements;
        }
    }

    public class Measurement
    {
        public int DetectorId { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public DateTime DateTime { get; set; }
        public string Temp { get; set; }

    }
}

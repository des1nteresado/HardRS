using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HardRS.HardwareManager
{
    public class Data
    {
        public static List<Measurement> GetUpdateData(DateTime dateTime)
        {
            var measurements = new List<Measurement>();
            var r = new Random();
            int counter = 0;
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
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature && computer.Hardware[i].Sensors[j].Name.Contains("CPU Package"))
                        {
                            measurements.Add(new Measurement()
                            {
                                DetectorId = counter,
                                DateTime = dateTime.AddSeconds(1),
                                Value = (int)computer.Hardware[i].Sensors[j].Value
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
                                measurements.Add(new Measurement()
                                {
                                    DetectorId = counter,
                                    DateTime = dateTime.AddSeconds(1),
                                    Value = (int)computer.Hardware[i].Sensors[j].Value
                                });
                                counter++;
                            }
                            //heh2 += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                        }
                    }
                }
                if (computer.Hardware[i].HardwareType == HardwareType.HDD)//temp of hdd
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            measurements.Add(new Measurement()
                            {
                                DetectorId = counter,
                                DateTime = dateTime.AddSeconds(1),
                                Value = (int)computer.Hardware[i].Sensors[j].Value
                            });
                            counter++;
                            //heh2 += computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                            //popl2 = (double)computer.Hardware[i].Sensors[j].Value;
                            //Lines.Points.Add(new DataPoint(popl, popl2));
                            //PlotModel.Series.Add(Lines);
                            //PlotModel.InvalidatePlot(true);
                            //DataPoint1 = new DataPoint(popl, popl2);
                            //Points.Add(new DataPoint(popl, popl2));
                        }
                    }
                }
            }

            //for (int i = 0; i < 5; i++)
            //{

            //    measurements.Add(new Measurement() { DetectorId = i, DateTime = dateTime.AddSeconds(1), Value = r.Next(1, 30) });
            //}

            computer.Close();
            return measurements;
        }
    }

    public class Measurement
    {
        public int DetectorId { get; set; }
        public int Value { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
}

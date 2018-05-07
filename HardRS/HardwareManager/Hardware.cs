﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HardRS.HardwareManager
{
    public class Hardware : INotifyPropertyChanged
    {
        private Processor processor;
        private VideoController videoController;
        private Memory memory;
        private DiskDrive diskDrive;


        public Processor Processor
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
        public VideoController VideoController
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
        public Memory Memory
        {
            get
            {
                return memory;
            }
            set
            {
                memory = value;
                OnPropertyChanged("Memory");
            }
        }
        public DiskDrive DiskDrive
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

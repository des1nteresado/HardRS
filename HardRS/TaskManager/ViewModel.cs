﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using HardRS.Annotations;

namespace HardRS.TaskManager
{
    public class ViewModel : INotifyPropertyChanged
    {
        string _pattern;
        public string Pattern
        {
            get => _pattern;
            set
            {
                Set(ref _pattern, value);
                try
                {
                    SelectedProcessItem = Processes.FirstOrDefault(s => s.ProcessName.StartsWith(Pattern));
                    SelectedProcess = Process.GetProcessById(SelectedProcessItem.Id);
                }
                catch { }
            }
        }


        public ViewModel()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += UpdateProcesses;
            timer.Start();
        }

        private Process _selectedProcess;
        public Process SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                Set(ref _selectedProcess, value);
            }
        }

        private ProcessListItem _selectedProcessItem;
        public ProcessListItem SelectedProcessItem
        {
            get => _selectedProcessItem;
            set
            {
                Set(ref _selectedProcessItem, value);
            }
        }

        public ObservableCollection<ProcessListItem> Processes { get; } = new ObservableCollection<ProcessListItem>();

        public void UpdateProcesses(object sender, EventArgs e)
        {
            var currentIds = Processes.Select(p => p.Id).ToList();

            foreach (var p in Process.GetProcesses())
            {
                if (!currentIds.Remove(p.Id)) // новый процесс
                {
                    Processes.Add(new ProcessListItem(p));
                }
            }

            foreach (var id in currentIds) // если больше не существует
            {
                var process = Processes.First(p => p.Id == id);
                if (process.KeepAlive)
                {
                    Process.Start(process.ProcessName, process.Arguments);
                }
                Processes.Remove(process);
            }
        }

        public void ChangePriority(ProcessPriorityClass priority)
        {
            SelectedProcess.PriorityClass = priority;
        }

        public void KillSelectedProcess()
        {
            SelectedProcess.Kill();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

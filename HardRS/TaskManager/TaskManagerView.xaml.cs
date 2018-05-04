using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using HardRS.TaskManager;

namespace HardRS.TaskManager
{
    /// <summary>
    /// Логика взаимодействия для TaskManagerView.xaml
    /// </summary>
    public partial class TaskManagerView : UserControl
    {
        public TaskManagerView()
        {
            InitializeComponent();
            this.DataContext = new ViewModel(); 
            Priorities.ItemsSource = Enum.GetValues(typeof(ProcessPriorityClass)).Cast<ProcessPriorityClass>();
            Priorities.SelectedIndex = 0;
        }

        private void TasksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            listBox.ScrollIntoView(listBox.SelectedItem);
            if (listBox.SelectedItems.Count > 0)
            {
                var viewModel = (ViewModel)DataContext;
                viewModel.SelectedProcess = ((ProcessListItem)listBox.SelectedItems[0]).Process;

            }
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (ViewModel)DataContext;
            listBox.SelectedIndex = 0;
            listBox.ScrollIntoView(listBox.SelectedIndex);
            viewModel.UpdateProcesses(sender, e);
        }

        private void ChangePriority_OnClick(object sender, RoutedEventArgs e)
        {
            var priority = (ProcessPriorityClass)Priorities.SelectionBoxItem;
            var viewModel = (ViewModel)DataContext;
            viewModel.ChangePriority(priority);
        }

        private void KillProcess_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (ViewModel)DataContext;
            viewModel.KillSelectedProcess();
            viewModel.UpdateProcesses(sender, e);
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var processListItem = (ProcessListItem)checkBox.DataContext;
            processListItem.KeepAlive = true;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var processListItem = (ProcessListItem)checkBox.DataContext;
            processListItem.KeepAlive = false;
        }
    }
}

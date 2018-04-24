using HardRS.CircularProgressBar;
using System;
using System.Collections.Generic;
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

namespace HardRS.HardwareManager
{
    /// <summary>
    /// Логика взаимодействия для HardwareManagerView.xaml
    /// </summary>
    public partial class HardwareManagerView : UserControl
    {
        public HardwareManagerView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}

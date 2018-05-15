using HardRS.CircularProgressBar;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class HardwareManagerView : UserControl
    {
        private HManagerViewModel viewModel;
        public HardwareManagerView()
        {
            viewModel = new HManagerViewModel();
            DataContext = viewModel;
            //чтобы обновление модели НЕ начиналось до того, как предыдущее НЕ было полностью отображено
            CompositionTarget.Rendering += CompositionTargetRendering;
            stopwatch.Start();

            InitializeComponent();
        }

        private long frameCounter;
        private Stopwatch stopwatch = new Stopwatch();
        private long lastUpdateMilliSeconds;

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds > lastUpdateMilliSeconds + 5000)
            {
                viewModel.UpdateModel();
                Plot1.RefreshPlot(true);
                lastUpdateMilliSeconds = stopwatch.ElapsedMilliseconds;
            }
        }
    }
}

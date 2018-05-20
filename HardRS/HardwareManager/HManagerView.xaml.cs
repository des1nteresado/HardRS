using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

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

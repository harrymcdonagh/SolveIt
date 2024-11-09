using OxyPlot;
using OxyPlot.Series;
using System.Windows.Controls;

namespace SolveIt.View.UserControls
{
    public partial class GraphDisplay : UserControl
    {
        private PlotModel plotModel;

        public GraphDisplay()
        {
            InitializeComponent();
            InitializePlot();
        }

        private void InitializePlot()
        {
            plotModel = new PlotModel { Title = "Graph Display" };
            plotView.Model = plotModel;
        }

        public void UpdateAnswer(float result)
        {
            plotModel.Series.Clear();
            var series = new LineSeries
            {
                Title = "Result",
                Points = { new DataPoint(0, 0), new DataPoint(1, result) }
            };
            plotModel.Series.Add(series);
            plotModel.InvalidatePlot(true);
        }
    }
}
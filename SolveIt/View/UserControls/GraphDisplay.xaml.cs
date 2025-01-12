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
        public void PlotValues(List<double> xValues, List<double> yValues)
        {
            if (xValues == null || yValues == null || xValues.Count != yValues.Count)
            {
                throw new ArgumentException("X and Y values must be non-null");
            }

            plotModel.Series.Clear();

            var series = new LineSeries
            {
                Title = "Custom Data"
            };

            for (int i = 0; i < xValues.Count; i++)
            {
                series.Points.Add(new DataPoint(xValues[i], yValues[i]));
            }

            plotModel.Series.Add(series);
            plotModel.InvalidatePlot(true);
        }
    }
}
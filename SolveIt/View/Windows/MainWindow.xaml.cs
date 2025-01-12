using System.Windows;

namespace SolveIt.View.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            inputPanel.ResultCalculated += InputPanel_ResultCalculated;
            inputPanel.GraphCalculated += InputPanel_GraphCalculated;
        }

        private void InputPanel_ResultCalculated(object sender, float result)
        {
            graphDisplay.UpdateAnswer(result);
        }
        private void InputPanel_GraphCalculated(object sender, Tuple<List<double>, List<double>> graphData)
        {
            var xValues = graphData.Item1;
            var yValues = graphData.Item2;

            graphDisplay.PlotValues(yValues, xValues);
        }
    }
}
using System.Windows;


namespace SolveIt
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            inputPanel.ResultCalculated += InputPanel_ResultCalculated;
        }
        private void InputPanel_ResultCalculated(object sender, int result)
        {
            // Update the graph display with the new result
            graphDisplay.UpdateAnswer(result);
        }
    }
}
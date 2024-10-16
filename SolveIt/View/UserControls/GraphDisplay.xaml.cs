using System.Windows.Controls;

namespace SolveIt.View.Graph
{
    /// <summary>
    /// Interaction logic for GraphDisplay.xaml
    /// </summary>
    public partial class GraphDisplay : UserControl
    {
        public GraphDisplay()
        {
            InitializeComponent();
        }

        public void UpdateAnswer(int result)
        {
            AnswerText.Text = result.ToString();
        }
    }
}

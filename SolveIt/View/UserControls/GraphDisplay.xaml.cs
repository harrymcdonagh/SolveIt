using System.Windows.Controls;

namespace SolveIt.View.UserControls
{
    public partial class GraphDisplay : UserControl
    {
        public GraphDisplay()
        {
            InitializeComponent();
        }

        public void UpdateAnswer(float result)
        {
            AnswerText.Text = result.ToString();
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MathInterpreter;

namespace SolveIt.View.UserControls
{
    public partial class InputPanel : UserControl
    {
        public event EventHandler<float> ResultCalculated;
        public InputPanel()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text;
            if (string.IsNullOrEmpty(input))
            {
                StatusOutput.Foreground = Brushes.Red;
                StatusOutput.Text = "Please enter a valid input";
                return;
            }
            try
            {
                float result = (float)Interpreter.interpret(input);
                StatusOutput.Foreground = Brushes.Green;
                StatusOutput.Text = "Success";
                ResultCalculated?.Invoke(this, result);
            }
            catch (Exception ex)
            {
                StatusOutput.Foreground = Brushes.Red;
                StatusOutput.Text = $"ERROR: {ex.Message}";
            }
        }
    }
}

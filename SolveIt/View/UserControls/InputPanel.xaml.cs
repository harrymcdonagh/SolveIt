using System.ComponentModel;
using System.Text.RegularExpressions;
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
            System.Diagnostics.Debug.WriteLine("Input: " + input);
            string eq = @"y\s*=\s*(.+)";
            Match match = Regex.Match(input, eq);
            if (match.Success)
            {
                System.Diagnostics.Debug.WriteLine("Match: " + match.Groups[1].Value);
                EvaluateEquation(match.Groups[1].Value);
            }
            else
            {
                try
                {
                    float result = (float)Interpreter.interpret(input);
                    StatusOutput.Foreground = Brushes.Green;
                    StatusOutput.Text = "Success! Answer - " + result;
                    ResultCalculated?.Invoke(this, result);
                }
                catch (Exception ex)
                {
                    StatusOutput.Foreground = Brushes.Red;
                    StatusOutput.Text = $"ERROR: {ex.Message}";
                }
            }
        }
        public event EventHandler<Tuple<List<double>, List<double>>> GraphCalculated;
        public void EvaluateEquation(string equation)
        {
            System.Diagnostics.Debug.WriteLine("Equation: " + equation);
            double start = -10;
            double end = 10;
            double step = 0.5;
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            for (double x = start; x <= end; x += step)
            {
                string brackx = "(" + x + ")";
                yValues.Add(x);
                xValues.Add(Interpreter.interpret(equation.Replace("x", brackx)));
            }
            GraphCalculated?.Invoke(this, new Tuple<List<double>, List<double>>(xValues, yValues));
            System.Diagnostics.Debug.WriteLine("X Values: " + string.Join(", ", xValues));

        }
    }
}

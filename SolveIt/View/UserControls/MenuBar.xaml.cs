using System.Windows;
using System.Windows.Controls;
using SolveIt.View.Windows;


namespace SolveIt.View.UserControls
{
    public partial class MenuBar : UserControl
    {
        public MenuBar()
        {
            InitializeComponent();
        }

        private void miHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }
    }
}

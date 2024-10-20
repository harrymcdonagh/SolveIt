using System.Windows;
using System.Windows.Controls;
using SolveIt.View.UserControls;

namespace SolveIt.View.Windows
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void Help_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (HelpListBox.SelectedItem is ListBoxItem selectedItem)
            {
                UserControl selectedControl = null;
                switch (selectedItem.Content.ToString())
                {
                    case "Overview":
                        selectedControl = new HelpOverview();
                        break;
                    case "Valid Syntax":
                        selectedControl = new HelpSyntax();
                        break;
                    case "Graphing":
                        selectedControl = new HelpGraphing();
                        break;
                    default:
                        // Handle any other cases or leave empty
                        break;
                }

                // Set the selected UserControl to the ContentControl
                if (selectedControl != null)
                {
                    HelpContentControl.Content = selectedControl;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}

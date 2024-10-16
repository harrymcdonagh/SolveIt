﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

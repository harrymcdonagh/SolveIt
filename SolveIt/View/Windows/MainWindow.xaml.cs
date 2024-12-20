﻿using System.Windows;

namespace SolveIt.View.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            inputPanel.ResultCalculated += InputPanel_ResultCalculated;
        }

        private void InputPanel_ResultCalculated(object sender, float result)
        {
            graphDisplay.UpdateAnswer(result);
        }
    }
}
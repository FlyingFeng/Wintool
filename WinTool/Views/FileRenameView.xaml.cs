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

namespace WinTool.Views
{
    /// <summary>
    /// FileRenameView.xaml 的交互逻辑
    /// </summary>
    public partial class FileRenameView : UserControl
    {
        public FileRenameView()
        {
            InitializeComponent();
        }

        private void Run_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RenameReadmeWindow w = new RenameReadmeWindow();
            w.ShowDialog();
        }
    }
}
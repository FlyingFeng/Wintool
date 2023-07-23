using System;
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
using WinTool.Internal;

namespace WinTool
{
    /// <summary>
    /// ProgressBarMask.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBarMask : UserControl, IProgressBar
    {
        private double _current;
        private double _total;


        public ProgressBarMask()
        {
            InitializeComponent();
            Caller.ProgressBar = this;
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
            _current = 0;
            _total = 0;
            progressBar.Value = 0;
            num.Text = "0";
        }

        public void SetCurrent(double current)
        {
            _current = current;
            if (_total != 0 && _current != 0)
            {
                var val = _current / _total * 100.0;
                progressBar.Value = val;
                num.Text = $"{(int)val}%";

                if (val == 100)
                {
                    Hide();
                }
            }
        }

        public void SetTotal(double total)
        {
            _total = total;
        }

        public void Show()
        {
            Caller.WaitingMask.Hide();
            this.Visibility = Visibility.Visible;
        }

    }
}

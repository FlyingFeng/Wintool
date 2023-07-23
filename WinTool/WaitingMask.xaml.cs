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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinTool.Internal;

namespace WinTool
{
    /// <summary>
    /// WaitingMask.xaml 的交互逻辑
    /// </summary>
    public partial class WaitingMask : UserControl, IWaitingMask
    {
        private Storyboard? _storyboard;

        public WaitingMask()
        {
            InitializeComponent();
            Caller.WaitingMask = this;
        }

        public void Hide()
        {
            if (_storyboard == null)
            {
                _storyboard = (Storyboard)FindResource("RotateStoryBoard");
            }
            _storyboard.Pause();
            this.Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            if (_storyboard == null)
            {
                _storyboard = (Storyboard)FindResource("RotateStoryBoard");
            }
            _storyboard.Begin();
            Caller.ProgressBar.Hide();
            this.Visibility = Visibility.Visible;
        }
    }
}

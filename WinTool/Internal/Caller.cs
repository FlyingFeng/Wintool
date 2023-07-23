using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinTool.Models.MyMap;

namespace WinTool.Internal
{

    internal class DefaultWaitingMask : IWaitingMask
    {
        public void Hide()
        {
        }

        public void Show()
        {
        }
    }

    internal class DefaultProgressBar : IProgressBar
    {
        public void Hide()
        {

        }

        public void SetCurrent(double current)
        {
        }

        public void SetTotal(double total)
        {
        }

        public void Show()
        {
        }
    }


    internal static class Caller
    {
        internal static IProgressBar ProgressBar = new DefaultProgressBar();
        internal static IWaitingMask WaitingMask = new DefaultWaitingMask();

        internal static China? ChinaRegion;


    }
}

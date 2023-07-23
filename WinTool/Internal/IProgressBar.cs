using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Internal
{
    internal interface IProgressBar : IWaitingMask
    {
        void SetCurrent(double current);
        void SetTotal(double total);
    }
}

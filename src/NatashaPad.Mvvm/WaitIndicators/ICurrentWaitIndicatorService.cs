using System;
using System.Collections.Generic;
using System.Text;

namespace NatashaPad.Mvvm.WaitIndicators
{
    public interface ICurrentWaitIndicatorService : IDisposable
    {
        void Show();
        void Close();
    }
}

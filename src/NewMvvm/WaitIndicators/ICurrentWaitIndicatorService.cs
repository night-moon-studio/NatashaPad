using System;
using System.Collections.Generic;
using System.Text;

namespace NewMvvm.WaitIndicators
{
    public interface ICurrentWaitIndicatorService : IDisposable
    {
        void Show();
        void Close();
    }
}

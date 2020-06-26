using System;
using System.Diagnostics;

namespace NatashaPad
{
    public class DumpOutHelper
    {
        public static Action<string> OutputAction = (str) => { Debug.WriteLine(str); };
    }
}
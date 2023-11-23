// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;

namespace NatashaPad;

public static class DumpOutHelper
{
    public static Action<string> OutputAction = (str) => { Debug.WriteLine(str); };
}

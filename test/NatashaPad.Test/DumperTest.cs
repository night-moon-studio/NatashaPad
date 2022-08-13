// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad.Test;

public class DumperTest
{
    private readonly IDumper _dumper = DefaultDumper.Instance;

    [Theory]
    [InlineData("NatashaPad")]
    public void StringTest(string str)
    {
        Assert.Equal(str, _dumper.Dump(str));
    }
}

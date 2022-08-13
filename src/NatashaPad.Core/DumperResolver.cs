// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad;

public class DumperResolver
{
    private readonly ICollection<IDumper> _dumpers;

    public DumperResolver(IEnumerable<IDumper> dumpers)
    {
        _dumpers = dumpers.Reverse().ToArray();
    }

    public IDumper Resolve(Type type)
    {
        foreach (var dumper in _dumpers)
        {
            if (dumper.TypePredicate(type))
                return dumper;
        }

        return DefaultDumper.Instance;
    }
}

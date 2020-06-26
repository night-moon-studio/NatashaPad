using System;
using WeihanLi.Extensions;

namespace NatashaPad
{
    public interface IDumper
    {
        Func<Type, bool> TypePredicate { get; }

        string Dump(object obj);
    }

    public class DefaultDumper : IDumper
    {
        public Func<Type, bool> TypePredicate { get; } = t => true;

        public string Dump(object obj)
        {
            return obj.ToJsonOrString();
        }
    }
}
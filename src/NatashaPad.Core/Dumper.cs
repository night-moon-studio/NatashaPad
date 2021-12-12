using WeihanLi.Extensions;

namespace NatashaPad;

public interface IDumper
{
    Func<Type, bool> TypePredicate { get; }

    string Dump(object obj);
}

public class DefaultDumper : IDumper
{
    public static readonly IDumper Instance = new DefaultDumper();

    public Func<Type, bool> TypePredicate { get; } = t => true;

    public string Dump(object obj)
    {
        return obj.ToJsonOrString();
    }
}

public static class DumperExtensions
{
    public static void Dump(this object obj)
    {
        string dumpedResult;
        if (obj is null)
        {
            dumpedResult = "(null)";
        }
        else
        {
            dumpedResult = DefaultDumper.Instance.Dump(obj);
        }
        DumpOutHelper.OutputAction?.Invoke(dumpedResult);
    }
}
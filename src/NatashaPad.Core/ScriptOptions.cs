namespace NatashaPad;

public class NScriptOptions
{
    public HashSet<string> UsingList { get; } = new HashSet<string>()
    {
        "System",
        "System.Collections.Generic",
        "System.Diagnostics",
        "System.Linq",
        "System.Linq.Expressions",
        "System.IO",
        "System.Reflection",
        "System.Text",
        "System.Text.RegularExpressions",
        "System.Threading",
        "System.Threading.Tasks",
    };

    public IList<IReferenceResolver> ReferenceResolvers { get; } = new List<IReferenceResolver>();
}
using System.Collections.Generic;

namespace NatashaPad
{
    public class NScriptOptions
    {
        public HashSet<string> UsingList { get; } = new HashSet<string>()
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.Threading.Tasks",
            "NatashaPad",
        };

        public IReferenceResolver[] ReferenceResolvers { get; set; }
    }
}
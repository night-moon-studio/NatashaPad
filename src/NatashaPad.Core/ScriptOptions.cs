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
        };

        public ICollection<IReferenceResolver> ReferenceResolvers { get; } = new List<IReferenceResolver>();
    }
}
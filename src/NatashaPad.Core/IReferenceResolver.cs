using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatashaPad
{
    public interface IReferenceResolver
    {
        string ReferenceType { get; }

        Task<IList<PortableExecutableReference>> Resolve();
    }
}
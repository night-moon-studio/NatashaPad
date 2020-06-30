using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace NatashaPad
{
    public interface IReferenceResolver
    {
        string ReferenceType { get; }

        Task<PortableExecutableReference[]> Resolve();
    }
}
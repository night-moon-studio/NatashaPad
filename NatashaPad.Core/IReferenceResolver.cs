using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace NatashaPad
{
    public interface IReferenceResolver
    {
        Task<MetadataReference> Resolve();
    }
}
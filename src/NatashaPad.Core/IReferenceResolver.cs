namespace NatashaPad;

public interface IReferenceResolver
{
    string ReferenceType { get; }

    Task<IList<PortableExecutableReference>> Resolve(CancellationToken cancellationToken = default);
}
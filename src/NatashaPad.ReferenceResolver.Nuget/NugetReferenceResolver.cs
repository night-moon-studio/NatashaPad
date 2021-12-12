namespace NatashaPad.ReferenceResolver.Nuget;

public class NugetReferenceResolver : IReferenceResolver
{
    public string PackageName { get; }
    public string PackageVersion { get; }

    public NugetReferenceResolver(string packageName, string packageVersion)
    {
        PackageName = packageName ?? throw new ArgumentNullException(nameof(packageName));
        PackageVersion = packageVersion ?? throw new ArgumentNullException(nameof(packageVersion));
    }

    public string ReferenceType => "NugetReference";

    public Task<IList<PortableExecutableReference>> Resolve(CancellationToken cancellationToken = default)
    {
        return NugetHelper.ResolveAssemblies(PackageName, PackageVersion, cancellationToken);
    }

    public override int GetHashCode()
    {
        return $"{PackageName}::{PackageVersion}".GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is NugetReferenceResolver reference)
        {
            return $"{PackageName}::{PackageVersion}".Equals($"{reference.PackageName}::{reference.PackageVersion}", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
}
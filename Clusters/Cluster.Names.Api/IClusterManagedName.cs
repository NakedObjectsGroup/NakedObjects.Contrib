using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Names.Api
{
    /// <summary>
    /// A Result interface representing a name that is persisted in the Name cluster.
    /// </summary>
    public interface IClusterManagedName : IName, IDomainInterface
    {
    }
}

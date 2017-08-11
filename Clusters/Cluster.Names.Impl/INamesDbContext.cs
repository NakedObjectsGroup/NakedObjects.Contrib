using System.Data.Entity;

namespace Cluster.Names.Impl
{
    /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
    public interface INamesDbContext
    {
        DbSet<AbstractName> Names { get; set; }
    }
}

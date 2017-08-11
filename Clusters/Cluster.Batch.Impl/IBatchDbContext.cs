using System.Data.Entity;

namespace Cluster.Batch.Impl
{
    /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
    public interface IBatchDbContext
    {
        DbSet<BatchProcessDefinition> BatchProcessDefinitions { get; set; }
        DbSet<BatchLog> BatchLogs { get; set; }
    }
}

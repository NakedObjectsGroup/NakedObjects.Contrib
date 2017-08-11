using System.Data.Entity;

namespace Cluster.Countries.Impl
{
    /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
   public interface ICountriesDbContext
    {
        DbSet<Country> Countries { get; set; }
    }
}

using System.Data.Entity;

namespace Cluster.Telephones.Impl
{
    /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
    public interface ITelephonesDbContext
    {
        DbSet<TelephoneCountryCode> TelephoneCountryCodes { get; set; }
        DbSet<TelephoneDetails> TelephoneDetails { get; set; }
    }
}

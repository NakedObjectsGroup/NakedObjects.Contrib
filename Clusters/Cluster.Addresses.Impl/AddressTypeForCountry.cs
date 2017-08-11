using NakedObjects;
namespace Cluster.Addresses.Impl
{
    /// <summary>
    /// Represents an association between a country and an address type. Not intended for use viewing.
    /// </summary>
    [Immutable]
    public class AddressTypeForCountry
    {
        
        public virtual int Id { get; set; }

        
        public virtual string CountryISOCode { get; set; }

        /// <summary>
        /// As fully-qualified type name
        /// </summary>
        public virtual string AddressType { get; set; }
      
      
      
    }
}

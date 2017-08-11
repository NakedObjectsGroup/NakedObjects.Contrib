using System.Collections.Generic;
using System.Linq;
using Cluster.Countries.Api;

namespace Cluster.Addresses.Api
{
    public interface IAddressService
    {
        IClusterManagedPostalAddress FindAddressById(int addressId);

        /// <summary>
        /// Returns a transient object of the type appropriate to the default country code, as specified in App.Settings
        /// </summary>
        IClusterManagedPostalAddress CreateNewAddress();

        /// <summary>
        /// Returns a transient object.  If the specified type is not explicitly supported, it will return a generic implementation.
        /// This is the preferred way to create addresses
        /// </summary>
        IClusterManagedPostalAddress CreateNewAddressForCountry(string countryIsoCode);


        /// <summary>
        /// Allows creation of addresses without having to specify country first, but will not take advantage of any special
        /// implementations.
        /// </summary>
        /// <returns></returns>
        IClusterManagedPostalAddress CreateNewGenericAddress();
    }
}

using System.Collections.Generic;

namespace Cluster.Addresses.Api
{
    // Role interface implemented by any class that can return one or more PostalAddresses
    public interface IPostalAddressProvider
    {

        IPostalAddress DefaultPostalAddress();

    }
}

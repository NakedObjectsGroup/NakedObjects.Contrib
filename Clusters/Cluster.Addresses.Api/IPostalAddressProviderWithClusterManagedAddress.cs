using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster.Addresses.Api
{
    public interface IPostalAddressProviderWithClusterManagedAddress : IPostalAddressProvider
    {
        //Persistent property holding the FK for the PostalAddress object.
        int PostalAddressId { get; }
    }
}

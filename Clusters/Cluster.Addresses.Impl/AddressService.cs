using Cluster.Addresses.Api;
using NakedObjects;
using System.Linq;
using System.Collections.Generic;
using System;
using NakedObjects.Util;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Cluster.Countries.Api;

namespace Cluster.Addresses.Impl
{
    public class AddressService : IAddressService
    {
        #region Injected Services
        public ICountryService CountryService{ set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        public IClusterManagedPostalAddress FindAddressById(int addressId)
        {
            return Container.Instances<AbstractAddress>().Single(x => x.Id == addressId);
        }

        #region Creating New

        public IClusterManagedPostalAddress CreateNewAddress()
        {
            return CreateNewAddressForCountry(Cluster.Countries.Api.AppSettings.DefaultCountryISOCode());
        }

        public IClusterManagedPostalAddress CreateNewAddressForCountry(string countryIsoCode)
        {
            Type type = AddressTypeForCountry(countryIsoCode);
            var addr = (AbstractAddress) Container.NewTransientInstance(type);
            addr.ISOCode = countryIsoCode;
            return addr;
        }

        public IClusterManagedPostalAddress CreateNewGenericAddress()
        {
            return Container.NewTransientInstance<GenericAddress>();
        }

        
        #endregion

        [NakedObjectsIgnore]
        public Type AddressTypeForCountry(string countryIsoCode)
        {
            var assoc = Container.Instances<AddressTypeForCountry>().Where(x => x.CountryISOCode == countryIsoCode).FirstOrDefault();
            if (assoc != null && assoc.AddressType != null)
            {
                return TypeUtils.GetType(assoc.AddressType);
            }
            return typeof(GenericAddress);
        }
    }
}

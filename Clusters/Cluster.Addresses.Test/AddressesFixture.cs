using System;
using System.Data.Entity;
using System.IO;
using Cluster.Addresses.Api;
using Cluster.Addresses.Impl;
using Cluster.Countries.Api;
using NakedObjects;
using NakedObjects.Core.Context;


namespace Cluster.Addresses.Test
{
    public class AddressesFixture
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public void Install()
        {
            AllMockCountries();
            NewTransaction(); //As countries are looked up when creating addresses

            UKAddresses();
            GenericAddresses();
            Associations();
        }

        public void AllMockCountries()
        {
            NewCountry("United Kingdom", CountryCodes.UK);
            NewCountry("United States", CountryCodes.USA);
            NewCountry("Ireland", CountryCodes.IRELAND);
        }

        public void UKAddresses()
        {
            NewUKAddress("Woodlark", "RG9 4LZ");
            NewUKAddress("Tigoni", "RG9 4LZ");
            NewUKAddress("Meadowcroft", "RG9 4LZ");
            NewUKAddress("Lilac Cottage", "RG9 4LY");
            NewUKAddress("Yew Tree Cottage", "RG9 4LY");
        }


        public void GenericAddresses()
        {
            NewGenericAddress("101 Page Brook Road", "Carlisle", "MA 02139", CountryCodes.USA);

        }

        public void Associations()
        {
            NewAddressTypeForCountry(CountryCodes.UK, typeof(UKAddress));
        }

        public ICountry NewCountry(string name, string code)
        {
            var c = Container.NewTransientInstance<MockCountry>();
            c.Name = name;
            c.ISOCode = code;
            Container.Persist(ref c);
            return c;
        }

        public UKAddress NewUKAddress(string line1, string postcode)
        {
            var a = Container.NewTransientInstance<UKAddress>();
            a.Line1 = line1;
            a.Line5 = postcode;
            Container.Persist(ref a);
            return a;

        }

        public GenericAddress NewGenericAddress(
            string line1, string line2, string line3, string isoCode)
        {
            var a = Container.NewTransientInstance<GenericAddress>();
            a.Line1 = line1;
            a.Line2 = line2;
            a.Line3 = line3;
            a.ISOCode = isoCode;
            Container.Persist(ref a);
            return a;
        }

        public AddressTypeForCountry NewAddressTypeForCountry(
             string isoCode, Type addressType)
        {
            var atfc = Container.NewTransientInstance<AddressTypeForCountry>();
            atfc.CountryISOCode = isoCode;
            atfc.AddressType = addressType.FullName;
            Container.Persist(ref atfc);
            return atfc;
        }

        protected void NewTransaction()
        {
            NakedObjectsContext.ObjectPersistor.EndTransaction();
            NakedObjectsContext.ObjectPersistor.StartTransaction();
        }
    }
}


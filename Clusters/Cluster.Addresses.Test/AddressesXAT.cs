using System;
using System.Linq;
using Cluster.Addresses.Impl;
using Cluster.Countries.Api;
using Cluster.System.Mock;
using Helpers.nof9;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace Cluster.Addresses.Test
{
	[TestClass()]
    public class AddressesXAT : ClusterXAT<AddressesTestDbContext> // AcceptanceTestCase
    {
		#region Run settings

		protected override Type[] Types
		{
			get
			{
				return new Type[]
				{
					typeof(AddressTypeForCountry),
					typeof(UKAddress),
					typeof(GenericAddress),
					typeof(MockCountryService),
					typeof(MockCountry)
				};
			}
		}

		protected override Type[] Services
		{
			get
			{
				return new Type[]
				{
					typeof(AddressService),
					typeof(SimpleRepository<UKAddress>),
					typeof(SimpleRepository<GenericAddress>),
					typeof(MockCountryService),
					typeof(FixedClock) // TODO: new FixedClock(new DateTime(2000, 1, 1))
				};
			}
		}
		#endregion

		#region Old Run configuration

        //protected override IObjectPersistorInstaller Persistor
        //{
        //    get
        //    {
        //        var installer = new EntityPersistorInstaller();
        //        Database.SetInitializer(new DropDatabaseAndInstallFixtures<AddressesTestDbContext>());
        //        installer.UsingCodeFirstContext(() => new AddressesTestDbContext());
        //        installer.IsInitializedCheck = () => DropDatabaseAndInstallFixtures<AddressesTestDbContext>.IsInitialized;
        //        return installer;
        //    }
        //}

        //protected override IFixturesInstaller Fixtures
        //{
        //    get
        //    {
        //        return new FixturesInstaller(new AddressesFixture());
        //    }
        //}
        #endregion
		
		#region Test Methods

		[TestMethod(), TestCategory("Addresses")]
        public virtual void UKAddress_Properties()
        {
            var uka = GetTestService("UK Addresses").GetAction("New Instance").InvokeReturnObject();
            uka.AssertIsType(typeof(UKAddress)).AssertIsTransient();
            uka.GetPropertyByName("Line1").AssertIsMandatory().AssertIsEmpty().AssertIsModifiable();
            uka.GetPropertyByName("Line2").AssertIsOptional().AssertIsEmpty().AssertIsModifiable();
            uka.GetPropertyByName("Line3").AssertIsOptional().AssertIsEmpty().AssertIsModifiable();
            uka.GetPropertyByName("Town").AssertIsMandatory().AssertIsEmpty().AssertIsModifiable();
            uka.GetPropertyByName("Postcode").AssertIsMandatory().AssertIsEmpty().AssertIsModifiable();
            uka.GetPropertyByName("Country").AssertIsUnmodifiable().AssertTitleIsEqual("United Kingdom");
        }

		[TestMethod(), TestCategory("Addresses")]
		public virtual void UKAddress_Save()
        {
            var uka = GetTestService("UK Addresses").GetAction("New Instance").InvokeReturnObject();
            uka.GetPropertyByName("Line1").SetValue("Dun Roamin");
            uka.GetPropertyByName("Line2").SetValue("4 Sea Lane");
            uka.GetPropertyByName("Line3").SetValue("Boscombe");
            uka.GetPropertyByName("Town").SetValue("Portsmouth");
            uka.GetPropertyByName("Postcode").SetValue("PO1 5TU");
            uka.AssertCanBeSaved().Save();
            uka.AssertIsPersistent().AssertTitleEquals("Dun Roamin, PO1 5TU");
            uka.AssertIsImmutable();
        }

		[TestMethod(), TestCategory("Addresses")]
		public virtual void UKAddress_PostcodeValidation()
        {
            var uka = GetTestService("UK Addresses").GetAction("New Instance").InvokeReturnObject();
            var pc = uka.GetPropertyByName("Postcode");
            pc.AssertFieldEntryIsValid("RG9 1AB");
            pc.AssertFieldEntryInvalid("RG91AB");
            pc.AssertFieldEntryInvalid("rg91ab");
            pc.AssertFieldEntryInvalid("RG9 1A");
            pc.AssertFieldEntryInvalid("RG9 123");
        }

		[TestMethod(), TestCategory("Addresses")]
		public virtual void GenericAddress_Properties()
        {
            var gen = GetTestService("Generic Addresses").GetAction("New Instance").InvokeReturnObject();
            gen.AssertIsType(typeof(GenericAddress)).AssertIsTransient();
            gen.GetPropertyByName("Line1").AssertIsMandatory().AssertIsEmpty().AssertIsModifiable();
            gen.GetPropertyByName("Line2").AssertIsMandatory().AssertIsEmpty().AssertIsModifiable();
            gen.GetPropertyByName("Line3").AssertIsOptional().AssertIsEmpty().AssertIsModifiable();
            gen.GetPropertyByName("Line4").AssertIsOptional().AssertIsEmpty().AssertIsModifiable();
            gen.GetPropertyByName("Line5").AssertIsOptional().AssertIsEmpty().AssertIsModifiable();
            var countries = gen.GetPropertyByName("Country").AssertIsMandatory().AssertIsEmpty().AssertIsModifiable();
            var uk = ((ITestObject) countries.GetChoices().First()).AssertTitleEquals("United Kingdom");
        }

		[TestMethod(), TestCategory("Addresses")]
		public virtual void GenericAddress_Save()
        {
            var gen = GetTestService("Generic Addresses").GetAction("New Instance").InvokeReturnObject();
            gen.GetPropertyByName("Line1").SetValue("Bog House");
            gen.GetPropertyByName("Line2").SetValue("Coastal Road");
            gen.GetPropertyByName("Line3").SetValue("BallyCroy");
            gen.GetPropertyByName("Line4").SetValue("Co. Mayo");
             var countries = gen.GetPropertyByName("Country");
            var ire = ((ITestObject) countries.GetChoices().Last()).AssertTitleEquals("Ireland");
            countries.SetObject(ire);

            gen.AssertCanBeSaved().Save();
            gen.AssertIsPersistent().AssertTitleEquals("Bog House, IRE");
            gen.AssertIsImmutable();
        }

		[TestMethod(), TestCategory("Addresses")]
		public virtual void GenericAddress_CountryNotShownIfPresent()
        {
            var gen = GetTestService("Generic Addresses").GetAction("New Instance").InvokeReturnObject();
            gen.GetPropertyByName("Line1").SetValue("Bog House");
            gen.GetPropertyByName("Line2").SetValue("Coastal Road");
            gen.GetPropertyByName("Line3").SetValue("BallyCroy");
            gen.GetPropertyByName("Line4").SetValue("Co. Mayo");
            var countries = gen.GetPropertyByName("Country");
            var uk = ((ITestObject)countries.GetChoices().First()).AssertTitleEquals("United Kingdom");
            countries.SetObject(uk);

            gen.AssertCanBeSaved().Save();
            gen.AssertIsPersistent().AssertTitleEquals("Bog House, Coastal Road");
        }

		[TestMethod(), TestCategory("Addresses")]
		public virtual void FindAddressById()
        {
            var find =GetTestService("Address Service").GetAction("Find Address By Id");
            var a1 = find.InvokeReturnObject("1");
            a1.AssertIsType(typeof(UKAddress));

            var a6 = find.InvokeReturnObject(6);
            a6.AssertIsType(typeof(GenericAddress));

        }

		[TestMethod(), TestCategory("Addresses")]
		public virtual void CreateNewAddress()
        {
            var addr = GetTestService("Address Service").GetAction("Create New Address").InvokeReturnObject();
            addr.AssertIsType(typeof(UKAddress)).AssertIsTransient();
        }
		
		[TestMethod(), TestCategory("Addresses")]
		public virtual void CreateNewGenericAddress()
        {
            var addr = GetTestService("Address Service").GetAction("Create New Generic Address").InvokeReturnObject();
            addr.AssertIsType(typeof(GenericAddress)).AssertIsTransient();
            addr.GetPropertyByName("Country").AssertIsEmpty();
        }

		[TestMethod(), TestCategory("Addresses")]
		public virtual void CreateNewAddressForCountry()
        {
            var action = GetTestService("Address Service").GetAction("Create New Address For Country");
            var p1 = action.Parameters.ElementAt(0);
            p1.AssertIsNamed("Country Iso Code").AssertIsMandatory();
            var addr = action.InvokeReturnObject(CountryCodes.IRELAND);
            addr.AssertIsTransient().AssertIsType(typeof(GenericAddress));
            addr.GetPropertyByName("Country").AssertIsNotEmpty().AssertTitleIsEqual("Ireland");

            addr = action.InvokeReturnObject(CountryCodes.UK);
            addr.AssertIsTransient().AssertIsType(typeof(UKAddress));
        }

		#endregion
	}
}

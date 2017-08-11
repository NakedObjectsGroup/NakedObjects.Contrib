using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Telephones.Impl;
using System.Data.Entity;

namespace Cluster.Telephones.Test
{
    [TestClass()]
    public class Telephones : AcceptanceTestCase
    {

        #region Constructors
        public Telephones(string name) : base(name) { }

        public Telephones() : this(typeof(Telephones).Name) { }
        #endregion

        #region Run configuration

        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(new TelephoneService(), 
                    new SimpleRepository<TelephoneCountryCode>(), 
                    new MockCountryService());
            }
        }

        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
                var installer = new EntityPersistorInstaller();
                installer.UsingCodeFirstContext(() => new TelephonesTestDbContext());
                return installer;
            }
        }
        #endregion

        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        #endregion

        [TestMethod]
        public virtual void CountryCode()
        {
            var codes = GetTestService("Telephone Country Codes").GetAction("All Instances").InvokeReturnCollection();
            codes.AssertIsNotEmpty();
            var tcc = codes.ElementAt(0);
            tcc.AssertIsType(typeof(TelephoneCountryCode));
        }

        [TestMethod()]
        public virtual void FindCountryCodes()
        {
            var find = GetTestService("Telephone Service").GetAction("Find Telephone Country Codes");
            var results = find.InvokeReturnCollection("UK");
            results.AssertIsNotEmpty().AssertCountIs(1);
            var tcc = results.ElementAt(0);
            tcc.AssertIsType(typeof(TelephoneCountryCode));
            tcc.AssertTitleEquals("United Kingdom +44");

            results = find.InvokeReturnCollection("Un");
            results.AssertCountIs(2);
            results.ElementAt(0).AssertTitleEquals("United Kingdom +44");
            results.ElementAt(1).AssertTitleEquals("United States +1");

            results = find.InvokeReturnCollection("353");
            results.AssertCountIs(1);
            results.ElementAt(0).AssertTitleEquals("Ireland +353");
        }

        [TestMethod]
        public virtual void CreateNewTelephoneDetails()
        {
            //Just to check that we haven't forgotten to set App.Settings!
             Assert.AreEqual("UK", Cluster.Countries.Api.AppSettings.DefaultCountryISOCode());

            var act = GetTestService("Telephone Service").GetAction("Create New Telephone Details");
            var td = act.InvokeReturnObject();
            td.AssertIsType(typeof(TelephoneDetails));
        }



    }
}
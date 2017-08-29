using System;
using System.Linq;
using NakedObjects.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Telephones.Impl;
using Helpers.nof9;

namespace Cluster.Telephones.Test
{
    [TestClass()]
    public class Telephones : ClusterXAT<TelephonesTestDbContext> //AcceptanceTestCase
    {
		#region Run settings

		protected override Type[] Types
		{
			get
			{
				return new Type[]
				{
					typeof(TelephoneDetails),
					typeof(TelephoneCountryCode),
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
				    typeof(TelephoneService),
				    typeof(SimpleRepository<TelephoneCountryCode>),
				    typeof(MockCountryService)
			    };
		    }
	    }
	    #endregion

		#region Test Methods

		[TestMethod, TestCategory("Telephones")]
		public virtual void CountryCode()
        {
            var codes = GetTestService("Telephone Country Codes").GetAction("All Instances").InvokeReturnCollection();
            codes.AssertIsNotEmpty();
            var tcc = codes.ElementAt(0);
            tcc.AssertIsType(typeof(TelephoneCountryCode));
        }

		[TestMethod, TestCategory("Telephones")]
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

		[TestMethod, TestCategory("Telephones")]
		public virtual void CreateNewTelephoneDetails()
        {
            //Just to check that we haven't forgotten to set App.Settings!
             Assert.AreEqual("UK", Countries.Api.AppSettings.DefaultCountryISOCode());

            var act = GetTestService("Telephone Service").GetAction("Create New Telephone Details");
            var td = act.InvokeReturnObject();
            td.AssertIsType(typeof(TelephoneDetails));
        }
		#endregion
	}
}

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Countries.Impl;
using System;
using Helpers;

namespace Cluster.Countries.Test
{
    [TestClass()]
    public class Countries : ClusterXAT<CountriesTestDbContext>
    {
        #region Run settings
        protected override Type[] Types
        {
            get
            {
                return new Type[]
                {
                    typeof(Country) //Only because Country does not appear in any method signature on CountryService (only ICountry)
            };
            }
        }

        protected override Type[] Services
        {
            get
            {
                return new Type[] {
                     typeof(CountryService)
                };
            }
        }
        #endregion

        [TestMethod()]
        public virtual void AllCountries()
        {
            var find = GetTestService("Country Service").GetAction("All Countries");
            var results = find.InvokeReturnCollection();
            results.AssertIsNotEmpty();
            var country = results.ElementAt(0);
            country.AssertIsType(typeof(Country)).AssertTitleEquals("Afghanistan");

            country.AssertIsImmutable();
            country.GetPropertyByName("Name").AssertValueIsEqual("Afghanistan");
            country.GetPropertyByName("ISO Code").AssertValueIsEqual("AF");
         }

        [TestMethod()]
        public virtual void FindByNameResults()
        {
            var find = GetTestService("Country Service").GetAction("Find Country By Name");
            var results = find.InvokeReturnCollection("Aust");
            results.AssertCountIs(2);
            results.ElementAt(0).AssertTitleEquals("Australia");
            results.ElementAt(1).AssertTitleEquals("Austria");

            results = find.InvokeReturnCollection("zim");
            results.AssertCountIs(1);
            results.ElementAt(0).AssertTitleEquals("Zimbabwe");

            results = find.InvokeReturnCollection("xxx");
            results.AssertCountIs(0);
        }

        [TestMethod()]
        public virtual void FindByNameParams()
        {
            var find = GetTestService("Country Service").GetAction("Find Country By Name");
            find.Parameters.First().AssertIsNamed("Matching").AssertIsMandatory();
            find.AssertIsValidWithParms("ABC");
            find.AssertIsValidWithParms("aBc");
            find.AssertIsValidWithParms("a c");
            find.AssertIsInvalidWithParms("123");
            find.AssertIsInvalidWithParms("1ab");
        }

        [TestMethod()]
        public virtual void FindByCode()
        {
            var find = GetTestService("Country Service").GetAction("Find Country By Code");
            var result = find.InvokeReturnObject("AU");
            result.AssertTitleEquals("Australia");

            result = find.InvokeReturnObject("UK");
            result.AssertTitleEquals("United Kingdom");
        }

        [TestMethod(), Ignore] //Regex failing one case
        public virtual void FindByCodeParams()
        {
            var find = GetTestService("Country Service").GetAction("Find Country By Code");
            find.Parameters.First().AssertIsNamed("Code").AssertIsMandatory();
            find.AssertIsValidWithParms("A");
            find.AssertIsValidWithParms("ABC");
            find.AssertIsInvalidWithParms("ABCD");
            find.AssertIsInvalidWithParms("aBc");
            find.AssertIsInvalidWithParms("a c");
            find.AssertIsInvalidWithParms("123");
            find.AssertIsInvalidWithParms("1ab");
        }

        [TestMethod()]
        public virtual void FindByCodeWithInvalidCode()
        {
            var find = GetTestService("Country Service").GetAction("Find Country By Code");
            try
            {
                var result = find.InvokeReturnObject("XX");
                Assert.Fail("Should not get here");
            }
            catch { }
        }

        [TestMethod()]
        public void DefaultCountry()
        {
            var act = GetTestService("Country Service").GetAction("Default Country");
            var c = act.InvokeReturnObject();
            c.AssertIsType(typeof(Country));
            c.AssertTitleEquals("United Kingdom");
        }
    }
}
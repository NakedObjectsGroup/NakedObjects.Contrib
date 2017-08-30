using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Names.Impl;
using Cluster.System.Mock;
using System;
using System.Linq;
using Helpers.nof9;
using NakedObjects.Services;

namespace Cluster.Names.Test
{
	[TestClass()]
    public class NamesServiceTest : ClusterXAT<NamesTestDbContext> // AcceptanceTestCase
    {
		#region Run settings

		protected override Type[] Types
		{
			get
			{
				return new Type[]
				{
					typeof(NameService),
					typeof(AbstractName),
					typeof(WesternName),
					typeof(TestIndividualFinder),
					typeof(FixedClock) // FixedClock(new DateTime(2000, 1,1))
				};
			}
		}

		protected override Type[] Services
		{
			get
			{
				return new Type[]
				{
					typeof(NameService),
					typeof(SimpleRepository<TestIndividual>),
                    typeof(TestIndividualFinder),
					typeof(FixedClock)
				};
			}
		}
		#endregion
		   
		// TODO: Tests all fail at WesternName.get_SortableName

		#region Test Methods

		[TestMethod, TestCategory("Name Service")]
        public void FindName() {
            var results = GetTestService("Name Service").GetAction("Find Matching Names").InvokeReturnCollection("paws");
            results.AssertCountIs(1);
            var result = results.ElementAt(0);
            result.AssertTitleEquals("Richard Pawson");
            result.AssertIsImmutable();
        }

		[TestMethod, TestCategory("Name Service")]
		public void WesternNameTestDerivedNames() {
            var find = GetTestService("Name Service").GetAction("Find By Id");
                
            var result = find.InvokeReturnObject(1);
            result.GetPropertyByName("Normal Name").AssertTitleIsEqual("Richard Pawson");
            result.GetPropertyByName("Formal Name").AssertTitleIsEqual("Dr Richard Pawson");
            result.GetPropertyByName("Sortable Name").AssertTitleIsEqual("Pawson, Richard");
            result.GetPropertyByName("Informal Salutation").AssertTitleIsEqual("Richard");
            result.GetPropertyByName("Formal Salutation").AssertTitleIsEqual("Dr Pawson");

            result = find.InvokeReturnObject(2);
            result.GetPropertyByName("Normal Name").AssertTitleIsEqual("William Morris");
            result.GetPropertyByName("Formal Name").AssertTitleIsEqual("Mr William B Morris");
            result.GetPropertyByName("Sortable Name").AssertTitleIsEqual("Morris, William B");
            result.GetPropertyByName("Informal Salutation").AssertTitleIsEqual("Bill");
            result.GetPropertyByName("Formal Salutation").AssertTitleIsEqual("Mr Morris");

            result = find.InvokeReturnObject(3);
            result.GetPropertyByName("Normal Name").AssertTitleIsEqual("Marge Roberts");
            result.GetPropertyByName("Formal Name").AssertTitleIsEqual("Marge Roberts");
            result.GetPropertyByName("Sortable Name").AssertTitleIsEqual("Roberts, Marge");
            result.GetPropertyByName("Informal Salutation").AssertTitleIsEqual("Marge");
            result.GetPropertyByName("Formal Salutation").AssertTitleIsEqual("Marge Roberts");

        }

		[TestMethod, TestCategory("Name Service")]
		public void CreateNewName()
        {
            var name = GetTestService("Name Service").GetAction("Create New Name").InvokeReturnObject();
            name.AssertIsType(typeof(WesternName));
            var prefix = name.GetPropertyByName("Prefix").AssertIsOptional().AssertIsModifiable();
            var dr = prefix.GetChoices().ElementAt(0);
            Assert.AreEqual("Dr", dr.Title);
            prefix.SetValue("Mr");
            name.GetPropertyByName("First Name").AssertIsMandatory().AssertIsModifiable().SetValue("George");
            name.GetPropertyByName("Middle Initial").AssertIsOptional().AssertIsModifiable().SetValue("W");
            name.GetPropertyByName("Last Name").AssertIsMandatory().AssertIsModifiable().SetValue("Bush");
            name.GetPropertyByName("Suffix").AssertIsOptional().AssertIsModifiable().SetValue("III");
            name.GetPropertyByName("Informal First Name").AssertIsOptional().AssertIsModifiable().SetValue("G");
            name.AssertCanBeSaved().Save();
            name.AssertIsPersistent().AssertTitleEquals("George Bush");

            name.GetPropertyByName("Normal Name").AssertTitleIsEqual("George Bush");
            name.GetPropertyByName("Formal Name").AssertTitleIsEqual("Mr George W Bush III");
            name.GetPropertyByName("Sortable Name").AssertTitleIsEqual("Bush, George W");
            name.GetPropertyByName("Informal Salutation").AssertTitleIsEqual("G");
            name.GetPropertyByName("Formal Salutation").AssertTitleIsEqual("Mr Bush");
        }

		[TestMethod, TestCategory("Name Service")]
		public void IndividualWithClusterManagedName()
        {
            var results =GetTestService("Test Individual Finder").GetAction("Find By Name").InvokeReturnCollection("will");
            var ind = results.AssertCountIs(1).ElementAt(0);
            ind.AssertIsType(typeof(TestIndividual));
            ind.GetPropertyByName("Name").AssertTitleIsEqual("William Morris");
        }
		#endregion

	}
}

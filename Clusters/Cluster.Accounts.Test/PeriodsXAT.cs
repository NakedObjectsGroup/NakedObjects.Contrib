using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Accounts.Impl;
using System;
using Cluster.System.Mock;
using System.Data.Entity;
using Helpers;

namespace Cluster.Accounts.Test
{
    [TestClass()]
    public class PeriodsXAT : ClusterXAT<AccountsTestDbContext, AccountsFixture>
    {
        #region Run configuration
        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new AccountsService(),
                    new FixedClock(new DateTime(2000, 1, 1)),
                    new SimpleRepository<Period>(),
                    new SimpleRepository<MockAccountHolder>(),
                    new SimpleRepository<MockSalesItem>(),
                    new SimpleRepository<MockPayment>(),
                    new CustomerAccountsService(),
                    new MockCustomerAccountNumberCreator(),
                    new PolymorphicNavigator(),
                    new SimpleRepository<MockAccountHolder>(),
                    new SimpleRepository<MockSalesItem>(),
                    new SimpleRepository<MockPayment>()
                    );
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

        #region Periods
        [TestMethod()]
        public virtual void ListOpen()
        {
            var periods = GetTestService("Accounts").GetAction("List Open Periods", "Periods").InvokeReturnCollection().AssertIsNotEmpty();
            int closed = periods.Count(x => x.GetPropertyByName("Closed").Title == "True");
            Assert.AreEqual(0, closed);
        }

        #endregion

        #region Close Period
        [TestMethod()]
        public virtual void ClosePeriodDec99()
        {

            //todo, set the clock to 1999
            var accounts = GetTestService("Accounts");
            var period = accounts.GetAction("List Open Periods", "Periods").InvokeReturnCollection().ElementAt(0);
            period.AssertTitleEquals("Dec 99");

            period.GetPropertyByName("Closed").AssertValueIsEqual("False");


            var a5 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 5").ElementAt(0);
            a5.AssertTitleEquals("A5 - Account 5");

            var a6 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 6").ElementAt(0);
            a6.AssertTitleEquals("A6 - Account 6");

            var post = accounts.GetAction("Post Transaction");
            post.InvokeReturnObject(new DateTime(1999, 12, 15), "Trans 1", "USD", new decimal(12.34), a5, a6);



            var entries5 = a5.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(3);
            var startBal = entries5.ElementAt(0);
            startBal.GetPropertyByName("Description").AssertValueIsEqual("Opening Balance");
            startBal.GetPropertyByName("Debit").AssertIsEmpty();
            startBal.GetPropertyByName("Credit").AssertTitleIsEqual("0.00");

            var trans1 = entries5.ElementAt(1);
            trans1.GetPropertyByName("Description").AssertValueIsEqual("Trans 1");
            trans1.GetPropertyByName("Debit").AssertValueIsEqual("12.34");
            trans1.GetPropertyByName("Credit").AssertIsEmpty();

            var currBal = entries5.ElementAt(2);
            currBal.GetPropertyByName("Description").AssertValueIsEqual("Current Balance");
            currBal.GetPropertyByName("Debit").AssertValueIsEqual("12.34");
            currBal.GetPropertyByName("Credit").AssertIsEmpty();

            var entries6 = a6.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(3);
            startBal = entries6.ElementAt(0);
            startBal.GetPropertyByName("Description").AssertValueIsEqual("Opening Balance");
            startBal.GetPropertyByName("Debit").AssertIsEmpty();
            startBal.GetPropertyByName("Credit").AssertTitleIsEqual("0.00");

            currBal = entries6.ElementAt(2);
            currBal.GetPropertyByName("Description").AssertValueIsEqual("Current Balance");
            currBal.GetPropertyByName("Credit").AssertValueIsEqual("12.34");
            currBal.GetPropertyByName("Debit").AssertIsEmpty();

            //Now close the period
            var close = accounts.GetAction("Close Period", "Periods");
            close.Parameters[1].AssertIsNamed("Confirm That You Wish To Close This Period").AssertIsMandatory();
            close.AssertIsInvalidWithParms(period, false);
            close.AssertIsValidWithParms(period, true);


            close.InvokeReturnObject(period, true);
            period.GetPropertyByName("Closed").AssertValueIsEqual("True");

            //Test that transaction is no longer modifiable
            //trans1.GetPropertyByName("Date").AssertIsUnmodifiable();
            //trans1.GetPropertyByName("Description").AssertIsUnmodifiable();
            //trans1.GetPropertyByName("Amount").AssertIsUnmodifiable();
            //trans1.GetPropertyByName("Debit Account").AssertIsUnmodifiable();
            //trans1.GetPropertyByName("Credit Account").AssertIsUnmodifiable();

            entries5 = a5.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);
            startBal = entries5.ElementAt(0);
            startBal.GetPropertyByName("Description").AssertValueIsEqual("Balance at last Period close");
            startBal.GetPropertyByName("Debit").AssertValueIsEqual("12.34");
            startBal.GetPropertyByName("Credit").AssertIsEmpty();

            currBal = entries5.ElementAt(1);
            currBal.GetPropertyByName("Description").AssertValueIsEqual("Current Balance");
            currBal.GetPropertyByName("Debit").AssertValueIsEqual("12.34");
            currBal.GetPropertyByName("Credit").AssertIsEmpty();

            //And on account6
            entries6 = a6.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);
            startBal = entries6.ElementAt(0);
            startBal.GetPropertyByName("Description").AssertValueIsEqual("Balance at last Period close");
            startBal.GetPropertyByName("Credit").AssertValueIsEqual("12.34");

            currBal = entries6.ElementAt(1);
            currBal.GetPropertyByName("Debit").AssertIsEmpty();
            currBal = entries6.ElementAt(1);
            currBal.GetPropertyByName("Description").AssertValueIsEqual("Current Balance");
            currBal.GetPropertyByName("Credit").AssertValueIsEqual("12.34");
            currBal.GetPropertyByName("Debit").AssertIsEmpty();
        }

        [TestMethod]
        public virtual void CannotCloseAPeriodAlreadyClosed()
        {
            var p = GetTestService("Periods").GetAction("All Instances").InvokeReturnCollection().ElementAt(0).AssertTitleEquals("Nov 99");
            p.GetPropertyByName("Closed").AssertValueIsEqual("True");
            GetTestService("Accounts").GetAction("Close Period", "Periods").AssertIsInvalidWithParms(p, true);
        }

        [TestMethod]
        public virtual void CannotCloseAPeriodNotYetStarted()
        {
            var p = GetTestService("Periods").GetAction("All Instances").InvokeReturnCollection().ElementAt(3).AssertTitleEquals("Feb 00");
            p.GetPropertyByName("Closed").AssertValueIsEqual("False");
            GetTestService("Accounts").GetAction("Close Period", "Periods").AssertIsInvalidWithParms(p, true);
        }
        #endregion
    }
}
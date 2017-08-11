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
    public class AccountsXAT : ClusterXAT<AccountsTestDbContext, AccountsFixture>
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

         #region Creating Accounts
        [TestMethod()]
        public virtual void FindAccountByName()
        {
            var find = GetTestService("Accounts").GetAction("Find Account By Name");
            find.Parameters[0].AssertIsNamed("Name").AssertIsMandatory();
            var results = find.InvokeReturnCollection("foo");
            results.AssertCountIs(2).ElementAt(0).AssertIsType(typeof(Account)).AssertTitleEquals("F1 - Foo 1");
            results = find.InvokeReturnCollection("Foo 2");
            results.AssertCountIs(1).ElementAt(0).AssertIsType(typeof(Account)).AssertTitleEquals("F2 - Foo 2");
        }

        [TestMethod()]
        public virtual void CreateNewAccountParams()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            Assert.AreEqual(6, newAccount.Parameters.Count());
            var p0 = newAccount.Parameters[0].AssertIsNamed("Code").AssertIsMandatory();
            var p1 = newAccount.Parameters[1].AssertIsNamed("Name").AssertIsMandatory();
            var p2 = newAccount.Parameters[2].AssertIsNamed("Type").AssertIsMandatory();
            var p3 = newAccount.Parameters[3].AssertIsNamed("Currency").AssertIsMandatory();
            Assert.AreEqual("USD", p3.GetDefault().Title);
            Assert.AreEqual(3, p3.GetChoices().Count());
            var p4 = newAccount.Parameters[4].AssertIsNamed("Opening Balance").AssertIsMandatory();
            Assert.AreEqual("0", p4.GetDefault().Title);
            var p5 = newAccount.Parameters[5].AssertIsNamed("As Of").AssertIsMandatory();
            Assert.AreEqual("30/11/1999 00:00:00", p5.GetDefault().Title);


        }

        [TestMethod()]
        public virtual void AsOfDateMustBeBeforeToday()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            newAccount.AssertIsValidWithParms("INC12345", "Income 1", AccountTypes.Income, "USD", new decimal(10), new DateTime(1999, 12, 31));
            newAccount.AssertIsInvalidWithParms("INC12345", "Income 1", AccountTypes.Income, "USD", new decimal(10), new DateTime(2000, 1, 1));

        }

        [TestMethod()]
        public virtual void CreateNewIncomeAccount()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            var a1 = newAccount.InvokeReturnObject("INC1", "Income 1", AccountTypes.Income, "USD", new decimal(10), new DateTime(1999, 4, 1));
            a1.AssertIsType(typeof(Account)).AssertIsPersistent().AssertTitleEquals("INC1 - Income 1");

            Assert.AreEqual(6, a1.Properties.Count()); //To check for untested properties being added! (Does not include Id)

            a1.GetPropertyByName("Code").AssertIsMandatory().AssertIsUnmodifiable().AssertValueIsEqual("INC1");
            a1.GetPropertyByName("Name").AssertIsMandatory().AssertIsModifiable().AssertValueIsEqual("Income 1");
            a1.GetPropertyByName("Type").AssertIsUnmodifiable().AssertTitleIsEqual("Income");
            a1.GetPropertyByName("Currency").AssertIsUnmodifiable().AssertValueIsEqual("USD");
            a1.GetPropertyByName("Last Modified").AssertIsUnmodifiable().AssertValueIsEqual("01/01/2000 00:00:00");

            var entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);
            var opening = entries.ElementAt(0);
            opening.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/04/1999 00:00:00");
            opening.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Opening Balance");
            opening.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertValueIsEqual("10");
            opening.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertIsEmpty();

            var current = entries.ElementAt(1);
            current.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/01/2000 00:00:00");
            current.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Current Balance");
            current.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertValueIsEqual("10");
            current.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertIsEmpty();
        }

        [TestMethod]
        public virtual void CreateNewIncomeAccountZeroBalance()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            var a1 = newAccount.InvokeReturnObject("INC2", "Income 2",
                AccountTypes.Income, "USD", new decimal(0), new DateTime(1999, 4, 1));
            var entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);
            var opening = entries.ElementAt(0);
            opening.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertValueIsEqual("0");
            opening.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertIsEmpty();

            var current = entries.ElementAt(1);
            current.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertValueIsEqual("0");
            current.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertIsEmpty();
        }

        [TestMethod]
        public virtual void CreateNewLiabilityAccountZeroBalance()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            var a1 = newAccount.InvokeReturnObject("LIAB2", "Liability 2",
                AccountTypes.Liability, "USD", new decimal(0), new DateTime(1999, 4, 1));
            var entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);
            var opening = entries.ElementAt(0);
            opening.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertValueIsEqual("0");
            opening.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertIsEmpty();

            var current = entries.ElementAt(1);
            current.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertValueIsEqual("0");
            current.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertIsEmpty();
        }

        [TestMethod]
        public virtual void CreateNewLiabilityAssetZeroBalance()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            var a1 = newAccount.InvokeReturnObject("ASS2", "Asset 2",
                AccountTypes.Asset, "USD", new decimal(0), new DateTime(1999, 4, 1));
            var entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);
            var opening = entries.ElementAt(0);
            opening.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();
            opening.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("0");

            var current = entries.ElementAt(1);
            current.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();
            current.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("0");
        }

        [TestMethod]
        public virtual void CreateNewExpenseAssetZeroBalance()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            var a1 = newAccount.InvokeReturnObject("EXP2", "Expense 2",
                AccountTypes.Expense, "USD", new decimal(0), new DateTime(1999, 4, 1));
            var entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);
            var opening = entries.ElementAt(0);
            opening.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();
            opening.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("0");

            var current = entries.ElementAt(1);
            current.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();
            current.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("0");
        }



        [TestMethod()]
        public virtual void CreateNewAssetAccount()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");

            var a1 = newAccount.InvokeReturnObject("ASS1", "Asset 1", AccountTypes.Asset, "GBP", new decimal(10), new DateTime(1999, 4, 1));
            a1.AssertIsType(typeof(Account)).AssertIsPersistent().AssertTitleEquals("ASS1 - Asset 1");

            a1.GetPropertyByName("Code").AssertIsMandatory().AssertIsUnmodifiable().AssertValueIsEqual("ASS1");
            a1.GetPropertyByName("Name").AssertIsMandatory().AssertIsModifiable().AssertValueIsEqual("Asset 1");
            a1.GetPropertyByName("Type").AssertIsUnmodifiable().AssertTitleIsEqual("Asset");

            var entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);
            var opening = entries.ElementAt(0);
            opening.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/04/1999 00:00:00");
            opening.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Opening Balance");
            opening.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();
            opening.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("10");

            var current = entries.ElementAt(1);
            current.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/01/2000 00:00:00");
            current.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Current Balance");
            current.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();
            current.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("10");
        }

        [TestMethod()]
        public virtual void AttemptToCreateAccountWithExistingName()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            newAccount.AssertIsInvalidWithParms("A87654", "Account 3", AccountTypes.Income, "EUR", new decimal(0), new DateTime(1999, 4, 1));
            newAccount.AssertIsValidWithParms("A87654", "Account 3x", AccountTypes.Income, "EUR", new decimal(0), new DateTime(1999, 4, 1));
        }

        [TestMethod()]
        public virtual void AttemptToCreateAccountWithExistingCode()
        {
            var newAccount = GetTestService("Accounts").GetAction("Create New Account");
            newAccount.AssertIsInvalidWithParms("A1", "Brand New Account Name", AccountTypes.Income, "EUR", new decimal(0), new DateTime(1999, 4, 1));
            newAccount.AssertIsValidWithParms("A8", "Brand New Account Name", AccountTypes.Income, "EUR", new decimal(0), new DateTime(1999, 4, 1));
        }
        #endregion

        #region Posting Transactions

        [TestMethod]
        public virtual void PostTransactionTestParameters()
        {
            var post = GetTestService("Accounts").GetAction("Post Transaction");
            Assert.AreEqual(6, post.Parameters.Count());
            var date = post.Parameters[0].AssertIsNamed("Date").AssertIsMandatory();
            Assert.AreEqual("01/01/2000 00:00:00", date.GetDefault().Title);
            var desc = post.Parameters[1].AssertIsNamed("Description").AssertIsMandatory();
            var curr = post.Parameters[2].AssertIsNamed("Currency").AssertIsMandatory();
            Assert.AreEqual("USD", curr.GetDefault().Title);
            Assert.AreEqual(3, curr.GetChoices().Count());
            var amount = post.Parameters[3].AssertIsNamed("Amount").AssertIsMandatory();
            //Assert.IsNull(desc.GetDefault()); //TODO: Not worrking correctly?
            var debit = post.Parameters[4].AssertIsNamed("Debit Account").AssertIsMandatory();
            var credit = post.Parameters[5].AssertIsNamed("Credit Account").AssertIsMandatory();
        }

        [TestMethod()]
        public virtual void PostATransaction()
        {
            var accounts = GetTestService("Accounts");
            var a1 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 1").ElementAt(0);
            var opening = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2).ElementAt(0);

            var a2 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 2").ElementAt(0);
            a2.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(2);

            var post = accounts.GetAction("Post Transaction");
            post.InvokeReturnObject(new DateTime(2000, 1, 2), "Trans 1", "USD", new decimal(12.34), a1, a2);
            //trans1.AssertIsType(typeof(Transaction)).AssertIsPersistent().AssertTitleEquals("Transaction");
            //trans1.GetPropertyByName("Period").AssertIsUnmodifiable().AssertTitleIsEqual("Jan 00");

            //Now check that it appears correctly in each account
            var entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(3);

            var transView = entries.ElementAt(1);
            transView.AssertIsType(typeof(TransactionInAccountView));
            transView.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("02/01/2000 00:00:00");
            transView.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Trans 1");
            transView.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("12.34");
            transView.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();

            var balance = entries.ElementAt(2);
            balance.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/01/2000 00:00:00");
            balance.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Current Balance");
            balance.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("12.34");
            balance.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();

            //Post a reverse transaction
            post.InvokeReturnObject(new DateTime(2000, 02, 1), "Trans 2", "USD", new decimal(5.12), a2, a1);
            //

            entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(4);
            transView = entries.ElementAt(2);
            transView.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/02/2000 00:00:00");
            transView.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Trans 2");
            transView.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertIsEmpty();
            transView.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertValueIsEqual("5.12");

            var trans2 = transView.GetPropertyByName("Transaction").ContentAsObject;
            trans2.AssertTitleEquals("Detail");
            trans2.GetPropertyByName("Period").AssertIsUnmodifiable().AssertTitleIsEqual("Feb 00");

            balance = entries.ElementAt(3);
            balance.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/01/2000 00:00:00");
            balance.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Current Balance");
            balance.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("7.22");
            balance.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();

            entries = a2.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(4);
            transView = entries.ElementAt(2);
            transView.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/02/2000 00:00:00");
            transView.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertValueIsEqual("5.12");
            transView.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertIsEmpty();

            balance = entries.ElementAt(3);
            balance.GetPropertyByName("Date").AssertIsUnmodifiable().AssertValueIsEqual("01/01/2000 00:00:00");
            balance.GetPropertyByName("Description").AssertIsUnmodifiable().AssertValueIsEqual("Current Balance");
            balance.GetPropertyByName("Debit").AssertIsUnmodifiable().AssertIsEmpty();
            balance.GetPropertyByName("Credit").AssertIsUnmodifiable().AssertValueIsEqual("7.22");
        }

        [TestMethod()]
        public virtual void ValidateParamsOnPostTransaction()
        {
            var accounts = GetTestService("Accounts");
            var a1 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 1").ElementAt(0);
            var a2 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 2").ElementAt(0);
            var a7 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 7").ElementAt(0);
            var post = accounts.GetAction("Post Transaction");

            //Debit and Credit accounts cannot be the same
            post.AssertIsInvalidWithParms(new DateTime(2000, 1, 1), "Trans 1", "USD", new decimal(12.34), a1, a1);
            post.AssertIsValidWithParms(new DateTime(2000, 1, 1), "Trans 1", "USD", new decimal(12.34), a1, a2);

            //Date cannot be within a closed period
            post.AssertIsInvalidWithParms(new DateTime(1999, 11, 30), "Trans 1", "USD", new decimal(12.34), a1, a2);
            post.AssertIsValidWithParms(new DateTime(2000, 02, 01), "Trans 1", "USD", new decimal(12.34), a1, a2);

            //Amount cannot be zero or negative
            post.AssertIsInvalidWithParms(new DateTime(2000, 02, 01), "Trans 1", "USD", new decimal(0), a1, a2);
            post.AssertIsInvalidWithParms(new DateTime(2000, 02, 01), "Trans 1", "USD", new decimal(-12.34), a1, a2);
            post.AssertIsValidWithParms(new DateTime(2000, 02, 01), "Trans 1", "USD", new decimal(12.34), a1, a2);

            //Accounts must both have same currency as posting
            post.AssertIsInvalidWithParms(new DateTime(2000, 02, 01), "Trans 1", "EUR", new decimal(12.34), a1, a2);
            post.AssertIsInvalidWithParms(new DateTime(2000, 02, 01), "Trans 1", "EUR", new decimal(12.34), a1, a7);
            post.AssertIsInvalidWithParms(new DateTime(2000, 02, 01), "Trans 1", "USD", new decimal(12.34), a1, a7);
            post.AssertIsInvalidWithParms(new DateTime(2000, 02, 01), "Trans 1", "USD", new decimal(12.34), a7, a1);
        }

        [TestMethod()]
        public virtual void SomeFieldsOnATransactionMayBeEditedWhileItsPeriodIsStillOpen()
        {
            var accounts = GetTestService("Accounts");
            var a3 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 3").ElementAt(0);
            a3.AssertTitleEquals("A3 - Account 3");

            var a4 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Account 4").ElementAt(0);
            a4.AssertTitleEquals("A4 - Account 4");

            var post = accounts.GetAction("Post Transaction");
            post.InvokeReturnObject(new DateTime(2000, 4, 1), "Trans 1", "USD", new decimal(12.34), a3, a4);

            var entries = a3.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(3);
            var transView = entries.ElementAt(1);
            var trans1 = transView.GetPropertyByName("Transaction").ContentAsObject;

            trans1.AssertIsType(typeof(Transaction)).AssertIsPersistent().AssertTitleEquals("Detail");
            var period = trans1.GetPropertyByName("Period").AssertIsUnmodifiable().AssertTitleIsEqual("Apr 00").ContentAsObject;
            period.GetPropertyByName("Closed").AssertValueIsEqual("False");

            var date = trans1.GetPropertyByName("Date").AssertIsModifiable();
            trans1.GetPropertyByName("Description").AssertIsModifiable();
            var amount = trans1.GetPropertyByName("Amount").AssertIsModifiable();
            trans1.GetPropertyByName("Debit Account").AssertIsUnmodifiable();
            trans1.GetPropertyByName("Credit Account").AssertIsUnmodifiable();

            date.AssertFieldEntryIsValid("02/04/2000");  //Another date in same period
            date.AssertFieldEntryIsValid("01/03/2000");  //Another date in other open period
            date.AssertFieldEntryInvalid("30/11/1999");  //Not a date in a closed period

            amount.AssertFieldEntryIsValid("12.34");
            amount.AssertFieldEntryInvalid("-12.34");
            amount.AssertFieldEntryInvalid("0");
        }

        #endregion

        #region Customer Accounts
        [TestMethod()]
        public virtual void CreateNewAccountForHolder()
        {
            var ah1 = GetTestService("Mock Account Holders").GetAction("All Instances").InvokeReturnCollection().ElementAt(0);
            var create = GetTestService("Customer Accounts").GetAction("Create New Account");
            Assert.AreEqual(5, create.Parameters.Count());
            create.Parameters.ElementAt(0).AssertIsNamed("For Holder").AssertIsMandatory();
            var curr = create.Parameters.ElementAt(1).AssertIsNamed("Currency Code").AssertIsMandatory();
            Assert.AreEqual("USD", curr.GetDefault().Title);
            Assert.AreEqual(3, curr.GetChoices().Count());
            var bal = create.Parameters.ElementAt(2).AssertIsNamed("Opening Balance").AssertIsMandatory();
            Assert.AreEqual("0", bal.GetDefault().Title);
            var asOf = create.Parameters.ElementAt(3).AssertIsNamed("As Of").AssertIsMandatory();
            Assert.AreEqual("30/11/1999 00:00:00", asOf.GetDefault().Title);
            create.Parameters.ElementAt(4).AssertIsNamed("Customer's own description").AssertIsOptional();

            SetNextAcNumber("B001");
            var ac = create.InvokeReturnObject(ah1, "USD", new decimal(0), new DateTime(1998, 1, 1), "Foo Account");
            ac.AssertIsType(typeof(CustomerAccount)).AssertIsPersistent();

            ac.GetPropertyByName("Account Holder").AssertObjectIsEqual(ah1).AssertIsUnmodifiable();
            ac.GetPropertyByName("Name").AssertValueIsEqual("Customer A/c for Mock1").AssertIsUnmodifiable().AssertIsMandatory();
            ac.GetPropertyByName("Customer's own description").AssertValueIsEqual("Foo Account").AssertIsModifiable().AssertIsMandatory();
            ac.GetPropertyByName("Currency").AssertValueIsEqual("USD").AssertIsUnmodifiable();
            ac.GetPropertyByName("Clearance Mechanism").AssertValueIsEqual("Clear Oldest Debits First").AssertIsModifiable().AssertIsMandatory();
        }

        [TestMethod()]
        public virtual void FindAccountsForAHolder()
        {
            var ah = GetTestService("Mock Account Holders").GetAction("All Instances").InvokeReturnCollection().ElementAt(1);
            ah.AssertTitleEquals("Mock2");
            var listAccounts = ah.GetAction("All Accounts", "Customer Accounts");
            var holderParam = listAccounts.Parameters.ElementAt(0).AssertIsNamed("For Holder").AssertIsMandatory();
            Assert.AreEqual(ah, holderParam.GetDefault());

            //TODO:  This should be done in fixturess
            var newAc = ah.GetAction("Create New Account", "Customer Accounts");
            SetNextAcNumber("A001");
            newAc.InvokeReturnObject(ah, "USD", new decimal(0), new DateTime(1998, 1, 1), "My Account 1");
            SetNextAcNumber("A002");
            newAc.InvokeReturnObject(ah, "USD", new decimal(0), new DateTime(1998, 1, 1), "My Account 2");
            SetNextAcNumber("A003");
            newAc.InvokeReturnObject(ah, "USD", new decimal(0), new DateTime(1998, 1, 1), "My Account 3");
            SetNextAcNumber("A004");
            newAc.InvokeReturnObject(ah, "EUR", new decimal(0), new DateTime(1998, 1, 1), "Euro Account");

            var all = listAccounts.InvokeReturnCollection(ah).AssertCountIs(5);
            //all.ElementAt(0).AssertTitleEquals("Customer A/c 00000004 - My Account 1");

            var find = ah.GetAction("Find Accounts", "Customer Accounts");
            find.InvokeReturnCollection(ah, "my").AssertCountIs(3).ElementAt(0).AssertTitleEquals("Customer A/c for Mock2 - My Account 1");
            find.InvokeReturnCollection(ah, "euro").AssertCountIs(1).ElementAt(0).AssertTitleEquals("Customer A/c for Mock2 - Euro Account");
        }

        private void SetNextAcNumber(string next)
        {
            GetTestService("Number Creator").GetAction("Set Next Number").InvokeReturnObject(next);

        }

        [TestMethod()]
        public virtual void AttemptCreateTwoAccountsWithSameName()
        {
            var ah = GetTestService("Mock Account Holders").GetAction("All Instances").InvokeReturnCollection().ElementAt(2);
            ah.AssertTitleEquals("Mock3");
            var newAc = ah.GetAction("Create New Account", "Customer Accounts");
            //Create first account Foo
            newAc.InvokeReturnObject(ah, "USD", new decimal(0), new DateTime(1998, 1, 1), "Foo Account");

            //Can't create a second with same name
            newAc.AssertIsInvalidWithParms(ah, "USD", new decimal(0), new DateTime(1998, 1, 1), "Foo Account");
            //... even for a different currency
            newAc.AssertIsInvalidWithParms(ah, "EUR", new decimal(0), new DateTime(1998, 1, 1), "Foo Account");
            //Can create one with different name
            newAc.AssertIsValidWithParms(ah, "USD", new decimal(0), new DateTime(1998, 1, 1), "Bar Account");
        }
        #endregion

        #region Matching
        [TestMethod]
        public void MatchTransactions()
        {
            var accounts = GetTestService("Accounts");
            var a1 = accounts.GetAction("Find Account By Name").InvokeReturnCollection("Receivables").ElementAt(0);
            var entries = a1.GetPropertyByName("Entries").ContentAsCollection.AssertCountIs(7);
            var t1 = entries.ElementAt(1).GetPropertyByName("Transaction").ContentAsObject.AssertTitleEquals("Detail");


        }
        #endregion
    }
}
using System;
using System.Linq;
using System.Data.Entity;
using Cluster.Accounts.Impl;
using Cluster.Accounts.Api;
using NakedObjects;
using NakedObjects.Core.Context;

namespace Cluster.Accounts.Test
{
    public class AccountsFixture
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public AccountsService AccountsService { set; protected get; }

        public CustomerAccountsService CustomerAccountsService { set; protected get; }

        public Cluster.System.Api.IClock Clock { set; protected get; }
        #endregion

        public void Install()
        {
            TestAccounts();
            TestPeriods();

            AllMockAccountHoldersAndTheirAccounts();
            AllMockSalesItems();
            AllMockPayments();
            //DemoAccounts();

            TestTransactions();

        }

        private static Account Receivables;
        private static Account Bank;
        private static Account Sales;

        public void DemoAccounts()
        {
            Receivables = NewAccount("REC", "Receivables", AccountTypes.Asset);
            Bank = NewAccount("BANK", "Bank", AccountTypes.Asset);
            Sales = NewAccount("SALES", "Sales", AccountTypes.Income);
            NewAccount("SAL", "Salaries", AccountTypes.Expense);

        }

        private Account NewAccount(string code, string name, AccountTypes type, string currency = "USD", DateTime? date = null)
        {
            DateTime startDate = date != null ? date.Value : Clock.Today().AddYears(-1);
            return AccountsService.CreateNewAccount(code, name, type, currency, new decimal(0), startDate);
        }

        public void DemoPeriods()
        {
            AccountsService.NewPeriod("Jun 13", new DateTime(2013, 6, 1), new DateTime(2013, 6, 30));
            AccountsService.NewPeriod("Jul 13", new DateTime(2013, 7, 1), new DateTime(2013, 7, 31));
            AccountsService.NewPeriod("Aug 13", new DateTime(2013, 8, 1), new DateTime(2013, 8, 31));
            AccountsService.NewPeriod("Sep 13", new DateTime(2013, 9, 1), new DateTime(2013, 9, 30));
            AccountsService.NewPeriod("Oct 13", new DateTime(2013, 10, 1), new DateTime(2013, 10, 31));
        }

        public void DemoTransactions()
        {
            AccountsService.PostTransaction(new DateTime(2013, 6, 4), "Service  - Regsitration", "USD", 150, Receivables, Sales);
            AccountsService.PostTransaction(new DateTime(2013, 6, 5), "Service  - Regsitration", "USD", 250, Receivables, Sales);
            AccountsService.PostTransaction(new DateTime(2013, 6, 7), "Service  - Regsitration", "USD", 600, Receivables, Sales);

            AccountsService.PostTransaction(new DateTime(2013, 6, 10), "Cheque Payment", "USD", 200, Bank, Receivables);
            AccountsService.PostTransaction(new DateTime(2013, 6, 11), "Cash Payment", "USD", 100, Bank, Receivables);

            AccountsService.PostTransaction(new DateTime(2013, 6, 11), "Cash Payment", "USD", 100, Bank, Receivables);
        }
        public void TestAccounts()
        {
            NewAccount("F1", "Foo 1", AccountTypes.Income);
            NewAccount("F2", "Foo 2", AccountTypes.Income);
            NewAccount("A1", "Account 1", AccountTypes.Income);
            NewAccount("A2", "Account 2", AccountTypes.Income);
            NewAccount("A3", "Account 3", AccountTypes.Income);
            NewAccount("A4", "Account 4", AccountTypes.Income);
            NewAccount("A5", "Account 5", AccountTypes.Income);
            NewAccount("A6", "Account 6", AccountTypes.Income);
            NewAccount("A7", "Account 7", AccountTypes.Income, "EUR");

            DateTime yearOld = Clock.Today().AddYears(-1);
            Receivables = NewAccount("REC", "Receivables", AccountTypes.Asset, "USD", yearOld);
            Bank = NewAccount("BANK", "Bank", AccountTypes.Asset, "USD", yearOld);
            Sales = NewAccount("SALES", "Sales", AccountTypes.Income, "USD", yearOld);
        }

        public void TestTransactions()
        {
            //For Matching tests
            AccountsService.PostTransaction(new DateTime(1999, 12, 11), "Invoice1", "USD", 100, Receivables, Sales);
            AccountsService.PostTransaction(new DateTime(1999, 12, 2), "Invoice1", "USD", 90, Receivables, Sales);
            AccountsService.PostTransaction(new DateTime(1999, 12, 2), "Payment1", "USD", 80, Bank, Receivables);
            AccountsService.PostTransaction(new DateTime(1999, 12, 2), "Payment2", "USD", 30, Receivables, Sales);
            AccountsService.PostTransaction(new DateTime(1999, 12, 2), "Payment3", "USD", 60, Receivables, Sales);
        }

        public void TestPeriods()
        {
            AccountsService.NewPeriod("Nov 99", new DateTime(1999, 11, 1), new DateTime(1999, 11, 30), true);
            AccountsService.NewPeriod("Dec 99", new DateTime(1999, 12, 1), new DateTime(1999, 12, 31));
            AccountsService.NewPeriod("Jan 00", new DateTime(2000, 1, 1), new DateTime(2000, 1, 31));
            AccountsService.NewPeriod("Feb 00", new DateTime(2000, 2, 1), new DateTime(2000, 2, 28));
            AccountsService.NewPeriod("Mar 00", new DateTime(2000, 3, 1), new DateTime(2000, 3, 31));
            AccountsService.NewPeriod("Apr 00", new DateTime(2000, 4, 1), new DateTime(2000, 4, 30));
        }

        public  void AllMockAccountHoldersAndTheirAccounts()
        {
            var mock1 = NewMockAccountHolder( "Mock1");
            var mock2 = NewMockAccountHolder( "Mock2");
            NewMockAccountHolder( "Mock3");
            NewMockAccountHolder( "Mock4");

            NakedObjectsContext.ObjectPersistor.EndTransaction();
            NakedObjectsContext.ObjectPersistor.StartTransaction();

            CustomerAccountsService.CreateNewAccount(mock1, "USD", new decimal(0), Clock.Today(), null);
            CustomerAccountsService.CreateNewAccount(mock2, "USD", new decimal(0), Clock.Today(), null);
        }

        public  MockAccountHolder NewMockAccountHolder(string name)
        {
            var mah = Container.NewTransientInstance<MockAccountHolder>();
            mah.Name = name;
            Container.Persist(ref mah);
            return mah;
        }

          public void AllMockSalesItems()
        {
            NewMockSalesItem( new decimal(1.00), "Item1", "AB001");
            NewMockSalesItem( new decimal(2.00), "Item2");
            NewMockSalesItem( new decimal(3.00), "Item3");

            NewMockSalesItem( new decimal(4.00), "Item4");
            NewMockSalesItem( new decimal(5.00), "Item5");
        }

        public  MockSalesItem NewMockSalesItem(
            decimal amount,
            string description,
            string analysisCodes = null)
        {
            var mci = Container.NewTransientInstance<MockSalesItem>();
            
                // CurrencyCode = currencyCode,
                mci.Amount = amount;
                mci.Description = description;
                mci.Description = description;
                mci.AnalysisCodes = analysisCodes;
                Container.Persist(ref mci);
            return mci;
        }

        public  void AllMockPayments()
        {
            NewMockPayment(new decimal(10), "Payment1");
            NewMockPayment( new decimal(20), "Payment2");
        }


        public  MockPayment NewMockPayment(
            decimal amount,
            string description,
            string analysisCodes = null
            )
        {
            var pay = Container.NewTransientInstance<MockPayment>();
            
                // CurrencyCode = currencyCode,
                pay.Amount = amount;
                pay.Description = description;
                pay.AnalysisCodes = analysisCodes;
                Container.Persist(ref pay);
            return pay;
        }
    }
}
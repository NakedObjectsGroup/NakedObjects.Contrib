using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cluster.Accounts.Api;
using Cluster.Accounts.Impl;
using NakedObjects;

namespace Cluster.Accounts.Impl
{
    [DisplayName("Accounts")]
    public class AccountsService : IAccountsService
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public Cluster.System.Api.IClock Clock { set; protected get; }

        #endregion

        #region Create New Account
         [MemberOrder(40)]
        public Account CreateNewAccount(
             string code,
            string name,
            AccountTypes type,
            string currency,
            [Mask("N")] decimal openingBalance,
            DateTime asOf)
        {
            return CreateNewAccount<Account>(code, name, type, currency, openingBalance, asOf);
        }

         public string Validate0CreateNewAccount(string code)
         {
             var rb = new ReasonBuilder();
             int matches = Container.Instances<Account>().Where(x => x.Code == code).Count();
             rb.AppendOnCondition(matches > 0, "Account code already exists");
             return rb.Reason;
         }

        public string Validate1CreateNewAccount(string name)
        {
            var rb = new ReasonBuilder();
            int matches = Container.Instances<Account>().Where(x => x.Name.ToUpper() == name.ToUpper()).Count();
            rb.AppendOnCondition(matches > 0, "Account name already exists");
            return rb.Reason;
        }

        public string Default3CreateNewAccount()
        {
            return AppSettings.DefaultCurrencyCode();
        }

        public string[] Choices3CreateNewAccount()
        {
            return AppSettings.ValidCurrencyCodes();
        }

        public DateTime Default5CreateNewAccount()
        {
            return ListOpenPeriods().FirstOrDefault().FromDate.AddDays(-1);
        }

        public string Validate5CreateNewAccount(DateTime asOf)
        {
            bool beforeToday = asOf.Date < Clock.Today();
            var rb = new ReasonBuilder();
             rb.AppendOnCondition(!beforeToday, "As Of date must be before today");
            return rb.Reason;
        }
        internal T CreateNewAccount<T>(
                    string code,
                    string name,
                    AccountTypes type,
                    string currency,
                    [Mask("N")] decimal openingBalance,
                    DateTime asOf) where T : Account, new()
        {
            T ac = Container.NewTransientInstance<T>();
            ac.Code = code;
            ac.Name = name;
            ac.Type = type;
            ac.Currency = currency;
            Container.Persist(ref ac);
            ac.CreateOpeningBalance(openingBalance, asOf);
            return ac;
        }

        #endregion

        #region FindAccountByName
        [MemberOrder(10)]
        public IQueryable<Account> FindAccountByName(string name)
        {
            return from obj in Container.Instances<Account>()
                   where obj.Name.ToUpper().Contains(name.ToUpper())
                   select obj;
        }
        #endregion

        #region FindAccountByCode
        [MemberOrder(20)]
        public Account FindAccountByCode(string code)
        {
            var acc = Container.Instances<Account>().SingleOrDefault(obj => obj.Code.ToUpper() == code.ToUpper());
            if (acc == null) Container.WarnUser("No Account exists with code "+code);
            return acc;
        }
        #endregion

                
        public IQueryable<Account> ChartOfAccounts()
        {
            return Container.Instances<Account>();
        }
      
        #region Post Transaction
        [NotContributedAction(), MemberOrder(30)]
        public void PostTransaction(
            [Mask("d")] DateTime date, 
            string description, 
            string currency, [Mask("N")] 
            decimal amount, 
            Account debitAccount, 
            Account creditAccount)
        {
            PostTransaction(date, description, currency, amount, debitAccount.Code, creditAccount.Code);
        }

        public string ValidatePostTransaction(
            DateTime date, 
            string description, 
            string currency, 
            decimal amount, 
            Account debitAccount, 
            Account creditAccount)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(debitAccount == creditAccount, "Accounts must be different");
            DateMustBeInOpenPeriod(rb, date);
            rb.AppendOnCondition(amount <= 0, "Amount must be > 0");
            rb.AppendOnCondition(debitAccount.Currency != currency ||
            creditAccount.Currency != currency, "Currency must be the same as for both accounts");
            return rb.Reason;
        }

        public DateTime Default0PostTransaction()
        {
            return Clock.Now();
        }

        public string Default2PostTransaction()
        {
            return AppSettings.DefaultCurrencyCode();
        }

        public string[] Choices2PostTransaction()
        {
            return AppSettings.ValidCurrencyCodes();
        }

        public IQueryable<Account> AutoComplete4PostTransaction(string name)
        {
            return FindAccountByName(name);
        }

        public IQueryable<Account> AutoComplete5PostTransaction(string name)
        {
            return FindAccountByName(name);
        }

        [NakedObjectsIgnore]
        public void PostTransaction(
                 DateTime date,
                 string description,
                 string currency, 
                 decimal amount,
                 string debitAccountCode,
                 string creditAccountCode)
        {
            Account debit = FindAccountByCode(debitAccountCode);
            Account credit = FindAccountByCode(creditAccountCode);
            var trans = Container.NewTransientInstance<Transaction>();
            trans.Date = date;
            trans.Description = description;
            trans.Currency = currency;
            trans.Amount = amount;
            trans.DebitAccount = debit;
            trans.CreditAccount = credit;
            Container.Persist(ref trans);
        }
        #endregion

        #region Periods
        [NakedObjectsIgnore]
        public Period GetPeriodForDate(DateTime date)
        {
            var rawDate = date.Date;
            return AllPeriods().Where(x => x.FromDate <= rawDate && x.ToDate >= rawDate).SingleOrDefault();
        }

         [MemberOrder(100, Name="Periods")]
        public Period CurrentPeriod()
        {
            return GetPeriodForDate(Clock.Today());
        }

         [MemberOrder(110, Name = "Periods")]
        public IQueryable<Period> ListOpenPeriods()
        {
            return AllPeriods().Where(x => !x.Closed).OrderBy(x => x.FromDate);
        }
         [MemberOrder(120, Name = "Periods")]
        private IQueryable<Period> AllPeriods()
        {
            return Container.Instances<Period>();
        }

        [NakedObjectsIgnore]
        public void DateMustBeInOpenPeriod(ReasonBuilder rb, DateTime d)
        {
            Period p = GetPeriodForDate(d);
            rb.AppendOnCondition(p == null || p.Closed, "Date must be within an open Period");
        }

        [NakedObjectsIgnore]
        public Period NewPeriod(
                string name,
                DateTime fromDate,
                DateTime toDate,
                bool closed = false)
        {
            Period p = Container.NewTransientInstance<Period>();
            p.Name = name;
            p.FromDate = fromDate;
            p.ToDate = toDate;
            p.Closed = closed;
            Container.Persist(ref p);
            return p;
        }
        #endregion

        internal void PersistBalanceForAllAccounts(DateTime asOf)
        {
            foreach (Account acc in Container.Instances<Account>())
            {
                acc.PersistBalance(asOf);
            }
        }

        #region List Balances
        [TableView(false, "Account", "Debit", "Credit", "Date"), MemberOrder(20)]
        public IQueryable<PersistedBalance> ListBalancesForPeriodEnd(Period period)
        {
            DateTime asOf = period.ToDate;
            return Container.Instances<PersistedBalance>().Where(pb => pb.Date == asOf);
        }
        public IList<Period> Choices0ListBalancesForPeriodEnd()
        {
            return Container.Instances<Period>().Where(p => p.Closed).OrderByDescending(p => p.FromDate).ToList();
        }

        public Period Default0ListBalancesForPeriodEnd()
        {
            return Choices0ListBalancesForPeriodEnd().FirstOrDefault();
        }

        #endregion


        #region Actions
         [MemberOrder(130, Name = "Periods")]
        public void ClosePeriod(Period periodToClose, bool confirmThatYouWishToCloseThisPeriod)
        {
            periodToClose.Closed = true;
            PersistBalanceForAllAccounts(periodToClose.ToDate);
        }

        public Period Default0ClosePeriod()
        {
            return ListOpenPeriods().FirstOrDefault();
        }

        public string ValidateClosePeriod(Period periodToClose, bool confirmThatYouWishToCloseThisPeriod)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(!confirmThatYouWishToCloseThisPeriod, "Check Confirm too proceed");
            rb.AppendOnCondition(periodToClose != ListOpenPeriods().First(), "Must  Close Periods in calendar order");
            rb.AppendOnCondition(Clock.Today() < periodToClose.FromDate, "Cannot close a Period not yet started");
            return rb.Reason;
        }
        #endregion
    }
}

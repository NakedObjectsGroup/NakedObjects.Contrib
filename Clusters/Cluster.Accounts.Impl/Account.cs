using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Cluster.Accounts.Api;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Impl
{
    public class Account : IAccount, IUpdateableEntity
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public AccountsService AccountsService { set; protected get; }

        public IClock Clock { set; protected get; }

        #endregion
        #region Life Cycle Methods
        public virtual void Persisting()
        {
            LastModified = Clock.Now();
        }

        public void Updating()
        {
            LastModified = Clock.Now();
        }
        #endregion

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Code).Append(" -", Name);
            return t.ToString();
        }

        #region Properties

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [Disabled, MemberOrder(5)]
        public virtual string Code { get; set; }
      
        #region Name
        [MemberOrder(10)]
        public virtual string Name { get; set; }

        public virtual string DisableName()
        {
            return null;  //To be potentially overridden in sub-classes
        }
        #endregion

        [MemberOrder(20), Disabled]
        public virtual AccountTypes Type { get; set; }

        [MemberOrder(30), Disabled]
        public virtual string Currency { get; set; }

        [MemberOrder(40), NotPersisted, NotMapped, TableView(false, "Date", "Description", "Debit", "Credit", "Transaction"), Eagerly(EagerlyAttribute.Do.Rendering)]
        public ICollection<IAccountEntry> Entries
        {
            get
            {
                int id = Id;
                var views = new List<IAccountEntry>();

                var bal = GetMostRecentPersistedBalance();
                views.Add(bal);
                foreach (Transaction tr in AllTransactionsForAccountFrom(bal.Date))
                {
                    var view = Container.NewViewModel<TransactionInAccountView>();
                    view.AccountViewedFrom = this;
                    view.Transaction = tr;
                    views.Add(view);
                }
                views.Add(CreateCurrentBalance());
                return views;

            }
        }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }

        #endregion

        private IQueryable<Transaction> AllTransactionsForAccountFrom(DateTime after)
        {
            int id = Id;
            return Container.Instances<Transaction>().Where(x => (x.CreditAccount.Id == id ||
                x.DebitAccount.Id == id)
                && x.Date > after);
        }

        internal void PersistBalance(DateTime asOf)
        {
            var pb = NewTransientBalance(asOf, PersistedBalance.PeriodCloseBalance);
            pb.Date = asOf;
            SetDebitOrCreditOnBalance(pb);
            Container.Persist(ref pb);
        }

        private IAccountEntry CreateCurrentBalance()
        {
            var currentBalance = Container.NewViewModel<CurrentBalance>();

            SetDebitOrCreditOnBalance(currentBalance);

            return currentBalance;
        }

        private void SetDebitOrCreditOnBalance(IAccountBalance balance)
        {
            var openingbalance = GetMostRecentPersistedBalance();
            decimal openingDebitValue = openingbalance.Debit ?? new decimal(0);
            decimal openingCreditValue = openingbalance.Credit ?? new decimal(0);
            var trans = AllTransactionsForAccountFrom(openingbalance.Date);
            var newDebits = trans.Where(x => x.DebitAccount.Id == Id).Select(x => x.Amount).DefaultIfEmpty(0).Sum(); //Cant' use Sum(x => x.Amount) if there are no Transactions
            var newCredits = trans.Where(x => x.CreditAccount.Id == Id).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            var currentDebit = openingDebitValue + newDebits;
            var currentCredit = openingCreditValue + newCredits;
            if (currentCredit == currentDebit)
            {
                switch (Type)
                {
                    case AccountTypes.Income:
                        balance.Credit = new decimal(0);
                        balance.Debit = null;
                        break;
                    case AccountTypes.Expense:
                        balance.Credit = null;
                        balance.Debit = new decimal(0);
                        break;
                    case AccountTypes.Asset:
                        balance.Credit = null;
                        balance.Debit = new decimal(0);
                        break;
                    case AccountTypes.Liability:
                        balance.Credit = new decimal(0);
                        balance.Debit = null;
                        break;
                    default:
                        throw new DomainException("Unrecognised Account Type");
                }
               
            }
            else if (currentCredit > currentDebit)
            {
                balance.Credit = currentCredit - currentDebit;
            }
            else
            {
                balance.Debit = currentDebit - currentCredit;
            }
        }

        #region Balances

        internal PersistedBalance GetMostRecentPersistedBalance()
        {
            return Container.Instances<PersistedBalance>().Where(x => x.Account.Id == Id).OrderByDescending(x => x.Date).ThenBy(x => x.Id).FirstOrDefault();
        }

        internal PersistedBalance CreateOpeningBalance(decimal balance, DateTime asOf)
        {
            switch (Type)
            {
                case AccountTypes.Income:
                    return SetOpeningBalance(null, balance, asOf);
                case AccountTypes.Expense:
                    return SetOpeningBalance(balance, null, asOf);
                case AccountTypes.Asset:
                    return SetOpeningBalance(balance, null, asOf);
                case AccountTypes.Liability:
                    return SetOpeningBalance(null, balance, asOf);
            }
            throw new DomainException("Unrecognised Account Type");
        }

        private PersistedBalance SetOpeningBalance(decimal? debitAmount, decimal? creditAmount, DateTime asOf)
        {
            var bal = NewTransientBalance(asOf, PersistedBalance.OpeningBalance);
            bal.Debit = debitAmount;
            bal.Credit = creditAmount;
            Container.Persist(ref bal);
            return bal;
        }

        private PersistedBalance NewTransientBalance(DateTime asOf, string description)
        {
            var bal = Container.NewTransientInstance<PersistedBalance>();
            bal.Account = this;
            bal.Description = description;
            bal.Debit = null;
            bal.Credit = null;
            bal.Date = asOf;
            return bal;
        }
        #endregion

        #region Reconciliation
        public IQueryable<TransactionInAccountView> UnreconciledTransactions()
        {
            //Find all transactions on this account that are not reconciled.
            //Present them as TransactionInAccountView
            throw new NotImplementedException();
        }
        #endregion
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cluster.Accounts.Api;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Impl
{
    public class Transaction : IUpdateableEntity
    {
        #region Injected Services
        public AccountsService AccountsService { set; protected get; }

        public IClock Clock { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }
        #endregion
        #region LifeCycle Methods
        public void Persisting()
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
            t.Append("Detail");
            return t.ToString();
        }

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        #region Date
        [Mask("d"), MemberOrder(10)]
        public virtual DateTime Date { get; set; }

        public string ValidateDate(DateTime date)
        {
            var rb = new ReasonBuilder();
            AccountsService.DateMustBeInOpenPeriod(rb, date);
            return rb.Reason;
        }

        public string DisableDate()
        {
            return DisableIfPeriodClosed();
        }

        private string DisableIfPeriodClosed()
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(Period.Closed, "Cannot edit a transaction for a closed Period ");
            return rb.Reason;
        }

        #endregion

        #region Description
        [MemberOrder(20)]
        public virtual string Description { get; set; }

        public string DisableDescription()
        {
            return DisableIfPeriodClosed();
        }
        #endregion

        [MemberOrder(25), Disabled]
        public virtual string Currency { get; set; }

        #region Amount
        [Mask("N"), MemberOrder(30)]
        public virtual decimal Amount { get; set; }

        public string DisableAmount()
        {
            return DisableIfPeriodClosed();
        }

        public string ValidateAmount(decimal amount)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(amount <= 0, "Amount must be > 0");
            return rb.Reason;
        }

        #endregion

        [MemberOrder(100), Disabled]
        public virtual Account DebitAccount { get; set; }

        [MemberOrder(110), Disabled]
        public virtual Account CreditAccount { get; set; }

        [MemberOrder(120)]
        public virtual Period Period
        {
            get
            {
                return AccountsService.GetPeriodForDate(Date);
            }
        }

        [MemberOrder(130)]       
        public virtual bool Reconciled { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
    }
}


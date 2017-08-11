using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Impl
{
    /// <summary>
    /// Represents a fixed balanc on an account, which might be the opening balance (manually
    /// specified) or a calculated balance for a period-end.
    /// </summary>
    public class PersistedBalance : IUpdateableEntity, IAccountBalance
    {
        #region Injected Services
        public IClock Clock { set; protected get; }

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
            t.Append("Balance");
            return t.ToString();
        }

        #region Properties
        public string DisablePropertyDefault()
        {
            return "Not editable";
        }

        public const string PeriodCloseBalance = "Balance at last Period close";
        public const string OpeningBalance = "Opening Balance";

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [NakedObjectsIgnore]
        public virtual int AccountId { get; set; }

        public virtual Account Account { get; set; }


        public virtual string Description { get; set; }

        [Mask("d")]
        public virtual DateTime Date { get; set; }

        [Mask("N")]
        public virtual decimal? Debit { get; set; }

        [Mask("N")]
        public virtual decimal? Credit { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
#endregion

    }
}

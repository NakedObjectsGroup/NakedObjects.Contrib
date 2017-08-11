using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Impl
{

    public class CurrentBalance : IViewModel, IAccountBalance, IUpdateableEntity
    {
        #region Injected Services
        public IClock Clock { set; protected get; }

        #endregion

        #region Life Cycle Methods
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

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }
      
        [Mask("d")]
        public virtual DateTime Date
        {
            get
            {
                return Clock.Now();
            }

        }
        public virtual string Description
        {
            get
            {
                return "Current Balance";
            }
        }

        [Disabled]
        public decimal? Debit { get; set; }

        [Disabled]
        public decimal? Credit { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
#endregion

        #region Implementation of IViewModel
        public string[] DeriveKeys()
        {
            string debitKey = Debit == null ? null : Debit.Value.ToString();
            string creditKey = Credit == null ? null : Credit.Value.ToString();
            return new string[] { debitKey, creditKey };
        }

        public void PopulateUsingKeys(string[] keys)
        {
            if (keys[0] != null)
            {
                Debit = decimal.Parse(keys[0]);
            }
            if (keys[1] != null)
            {
                Credit = decimal.Parse(keys[1]);
            }
        }
        #endregion
    }
}

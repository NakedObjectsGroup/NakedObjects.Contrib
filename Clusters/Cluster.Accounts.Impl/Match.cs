using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Impl
{

    /// <summary>
    /// Matches a Debit (either partly or wholly) to a Credit (either partly or wholly) 
    /// within the same account.
    /// </summary>
    public class Match : IUpdateableEntity
    {
        #region Injected Services
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
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public virtual decimal AmountMatched { get; set; }
      
        public virtual Account Account { get; set; }
      
        public virtual Transaction DebitTransaction { get; set; }
       
        public virtual Transaction CreditTransaction { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
    }
}

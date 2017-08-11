using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Impl
{

    public class Period :  IUpdateableEntity
    {
        #region Injected Services
        public AccountsService AccountsService { set; protected get; }

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
            t.Append(Name);
            return t.ToString();
        }

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(10), Disabled(WhenTo.OncePersisted)]
        public virtual string Name { get; set; }

        [Mask("d"), MemberOrder(20), DisplayName("From"), Disabled(WhenTo.OncePersisted)]
        public virtual DateTime FromDate { get; set; }

        [Mask("d"), MemberOrder(30), DisplayName("To"), Disabled(WhenTo.OncePersisted)]
        public virtual DateTime ToDate { get; set; }

        [MemberOrder(40), Disabled]
        public virtual bool Closed { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }

    }
}

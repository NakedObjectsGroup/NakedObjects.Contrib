using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;
using System.ComponentModel.DataAnnotations;

namespace Cluster.Addresses.Impl
{
    public class DomainEntity : IUpdateableEntity
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
        #region Title
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append("DomainEntity");
            return t.ToString();
        }
        #endregion

        #region Properties
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
        #endregion

        #region Actions

        #endregion
    }
}



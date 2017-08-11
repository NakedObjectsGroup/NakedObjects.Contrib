using System;
using System.ComponentModel.DataAnnotations;
using Cluster.Names.Api;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Names.Impl
{
    /// <summary>
    /// Abstract implementation of clustert-managed name. Note that it is immutable. Any change of name requires a replacement
    /// object.
    /// </summary>
    [Immutable(WhenTo.OncePersisted)]
    public abstract class AbstractName : IClusterManagedName, IUpdateableEntity
    {
        #region Injected Services

        public IClock Clock { set; protected get; }

        #endregion
        #region LifeCycle Methods
        public void Persisting()
        {
            LastModified = Clock.Now();
            //TODO: Take out all non-alpha characters
            Searchable = NormalName.ToUpper();
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
            t.Append(NormalName);
            return t.ToString();
        }
        #endregion

        #region IName properties
        public abstract string NormalName { get; }

        public abstract string FormalName { get; }

        public abstract string SortableName { get; }

        public abstract string FormalSalutation { get; }

        public abstract string InformalSalutation { get; }
        #endregion

        #region Properties
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [NakedObjectsIgnore]
        public virtual NameTypes Type { get; set; }

        //De-normalised version all elements of the name that may be text searchable, formatted for sorting e.g.  Bush, George W
        [NakedObjectsIgnore]
        public virtual string Searchable { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
        #endregion
    }
}

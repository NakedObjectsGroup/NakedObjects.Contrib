using System.Linq;
using System.Collections.Generic;
using Cluster.Addresses.Api;
using NakedObjects;
using Cluster.Countries.Api;
using System.ComponentModel.DataAnnotations.Schema;
using Cluster.System.Api;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cluster.Addresses.Impl
{
    public abstract class AbstractAddress : IClusterManagedPostalAddress, IUpdateableEntity
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public ICountryService CountryService { set; protected get; }

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
        #region Title methods
        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Line1);
            t.Append(",", CountryIsDefaultCountry() ? Line2 : ISOCode);
            return t.ToString();
        }

        #endregion

        #region Properties
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(101)]
        public virtual string Line1 { get; set; }

        [MemberOrder(102)]
        public virtual string Line2 { get; set; }

        [MemberOrder(103)]
        public virtual string Line3 { get; set; }

        [MemberOrder(104)]
        public virtual string Line4 { get; set; }

        [MemberOrder(105)]
        public virtual string Line5 { get; set; }
        
        #region Country Property of type ICountry ('Result' interface)

        [Hidden()]
        public virtual string ISOCode { get; set; }

        protected bool CountryIsDefaultCountry()
        {
            return Cluster.Countries.Api.AppSettings.DefaultCountryISOCode() == ISOCode;
        }

        private ICountry _Country;

        [NotPersisted(), NotMapped(), MemberOrder(110)]
        public ICountry Country
        {
            get
            {
                if (_Country == null && ISOCode != null)
                {
                    _Country = CountryService.FindCountryByCode(ISOCode);
                }
                return _Country;
            }
            set
            {
                _Country = value;
                if (value == null)
                {
                    ISOCode = null;
                }
                else
                {
                    ISOCode = value.ISOCode;
                };
            }
        }
    
        public IList<ICountry> ChoicesCountry()
        {
            return CountryService.AllCountries().ToList();
        }
 
        public virtual ICountry DefaultCountry()
        {
            //Exists to allow overriding in sub-types
            return null;
        }

        public virtual string DisableCountry()
        {
            //Exists to allow overriding in sub-types
            return null;
        }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }

        #endregion

        #endregion

        #region Implementation of IPostalAddress
        public virtual string AsSingleLine()
        {
            var tb = new TitleBuilder();
            Append4Lines(tb);
            tb.Append(",", Country);
            return tb.ToString();
        }

        protected virtual void Append4Lines(TitleBuilder tb)
        {
            tb.Append(Line1).Append(",", Line2).Append(", ", Line3).Append(",", Line4).Append(",", Line5);
        }

        public virtual string[] LabelFormIncludingCountry()
        {
            var label = LabelWithoutCountry();
            label.Add(Country.Name);
            return label.ToArray();
        }

        protected virtual List<string> LabelWithoutCountry()
        {
            var label = new List<string>();
            label.Add(Line1);
            if (Line2 != null) label.Add(Line2);
            if (Line3 != null) label.Add(Line3);
            if (Line4 != null) label.Add(Line4);
            if (Line5 != null) label.Add(Line5);
            return label;
        }

        public virtual string[] LabelFormAsPostedFrom(ICountry country)
        {
            var label = LabelWithoutCountry();
            if (Country != country)
            {
                label.Add(Country.Name);
            }
            return label.ToArray();
        }

        public ICountry DestinationCountry()
        {
            return Country;
        }
        #endregion
    }
}

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using Cluster.System.Api;
using Cluster.Telephones.Api;
using Cluster.Users.Api;
using NakedObjects;

namespace Cluster.Telephones.Impl
{
    public class TelephoneDetails : ITelephoneDetails, IUpdateableEntity
    {
        #region Injected Services
        public TelephoneService TelephoneService { set; protected get; }

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
            t.Append(NumberToDial(true, Cluster.Countries.Api.AppSettings.DefaultCountryISOCode()));
            return t.ToString();
        }
        #endregion

        #region Properties
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        #region CountryCode
        [MemberOrder(10)]
        public virtual TelephoneCountryCode CountryCode { get; set; }

        public IQueryable<TelephoneCountryCode> AutoCompleteCountryCode([MinLength(2)] string matching)
        {
            return TelephoneService.FindTelephoneCountryCodes(matching);
        }
     
        public TelephoneCountryCode DefaultCountryCode()
        {
            return TelephoneService.TelephoneCountryCode(Cluster.Countries.Api.AppSettings.DefaultCountryISOCode());
        }     
        #endregion

        /// <summary>
        /// Including a leading zero, if applicable.
        /// </summary>
        [MemberOrder(20)]
        public virtual string AreaCode { get; set; }

        private string AreaCodeMinusLeadingDigit()
        {
            return AreaCode == null ? null : AreaCode.Substring(1);
        }

        [MemberOrder(30)]
        public virtual string Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatted">If true, number includes spaces and "+", maybe (); if false just continguous numbers</param>
        /// <param name="fromCountry">If null; the number is in full, starting '+'</param>
        /// <returns></returns>
        public string NumberToDial(bool formatted, string fromISOCountryCode)
        {
            //TODO:  Currently UK-oriented.  Should delegate to format strings on TCC
            string format = null;
            if (fromISOCountryCode == this.CountryCode.ISOCountryCode)
            {
                if (formatted)
                {
                    format = "0{2} {3}";
                }
                else
                {
                    format = "0{2}{3}";
                }
            }
            else
            {
                if (formatted)
                {
                    format = "+{1} {2} {3}";
                }
                else
                {
                    format = "{0}{1}{2}{3}";
                }
            }
            string prefix = DefaultCountryCode().InternationalCallPrefix;
            return string.Format(format, prefix, CountryCode.CountryCallingCode, AreaCodeMinusLeadingDigit(), Number);
        }   

        [MemberOrder(40)]
        public virtual TelephoneType Type { get; set; }

        /// <summary>
        /// e.g. 'Home', 'Office',  'Only after 8.00pm', 'Emergency'
        /// </summary>
        [Optionally, MemberOrder(50)]
        public virtual string Description { get; set; }

        [MemberOrder(60)]
        public virtual bool Preferred { get; set; }
          
        [MemberOrder(70), DefaultValue(true)]
        public virtual bool IsCurrent { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
#endregion
    }
}

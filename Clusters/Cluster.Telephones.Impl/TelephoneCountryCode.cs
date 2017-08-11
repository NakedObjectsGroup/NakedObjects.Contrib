using Cluster.Telephones.Api;
using NakedObjects;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Cluster.Countries.Api;

namespace Cluster.Telephones.Impl
{
    [Immutable(WhenTo.OncePersisted)]
    public class TelephoneCountryCode
    {
        #region Injected Services
        public TelephoneService TelephoneService { set; protected get; }

        public ICountryService CountryService { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
           t.Append(Country).Append("+").Concat(CountryCallingCode);
            return t.ToString();
        }
      
        [Hidden]
        public virtual int Id { get; set; }

        
        #region Country Property of type Country

        [Hidden()]
        public virtual string ISOCountryCode { get; set; }


        private ICountry _Country;

        [NotPersisted(), NotMapped, MemberOrder(10)]
        public ICountry Country
        {
            get
            {
                if (_Country == null && ISOCountryCode != null)
                {
                    _Country = CountryService.FindCountryByCode(ISOCountryCode);
                }
                return _Country;
            }
        }
        #endregion


        /// <summary>
        /// e.g. '44' for the UK
        /// </summary>
        [MemberOrder(20)]
        public virtual string CountryCallingCode { get; set; }

        /// <summary>
        /// e.g. 00 from the UK
        /// </summary>
        [MemberOrder(30), Optionally]
        public virtual string InternationalCallPrefix { get; set; }

        /// <summary>
        /// E.g. to enforce that it begins with 0, or, for USA, fits known number ranges.
        /// </summary>
        [MemberOrder(40), Optionally]
        public virtual string AreaCodeRegexValidation { get; set; }

         [MemberOrder(50), Optionally]
        public virtual string NumberRegexValidation { get; set; }


        /// <summary>
        /// TODO:  document formats
        /// </summary>
         [MemberOrder(60), Optionally]
        public virtual string FormatDisplayNumberFromSameCountry { get; set; }

         [MemberOrder(70), Optionally]
        public virtual string FormatNumberToDialFromSameCountry { get; set; }

         [MemberOrder(80), Optionally]
        public virtual string FormatDisplayNumberFromOtherCountry { get; set; }

         [MemberOrder(90), Optionally]
        public virtual string FormatNumberToDialFromOtherCountry { get; set; }
      
      
    }
}

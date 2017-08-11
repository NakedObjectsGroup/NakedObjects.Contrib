using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Cluster.Addresses.Api;
using Cluster.Countries.Api;
using NakedObjects;

namespace Cluster.Addresses.Impl
{
     [Immutable(WhenTo.OncePersisted)]
    public class UKAddress : AbstractAddress
    {
        #region Injected Services
        #endregion

        #region LifeCycle Methods
        public void Created()
        {
            this.ISOCode = CountryCodes.UK;
        }
        #endregion

        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Line1);
            t.Append(",", CountryIsDefaultCountry() ? Line5 : ISOCode);
            return t.ToString();
        }    

        public const string UKPostCodeRegex = @"^([A-PR-UWYZ0-9][A-HK-Y0-9][AEHMNPRTVXY0-9]?[ABEHMNPRVWXY0-9]? {1,2}[0-9][ABD-HJLN-UW-Z]{2}|GIR 0AA)$";

        #region Properties

        //TODO:  Ability to look up addresses from Postcode

        [MemberOrder(102), Optionally]
        public override string Line2 { get; set; }

        [MemberOrder(103), Optionally]
        public override string Line3 { get; set; }

        [MemberOrder(104), DisplayName("Town")]
        public override string Line4 { get; set; }

        [MemberOrder(105), RegularExpression(UKPostCodeRegex), DisplayName("Postcode")]
        public override string Line5 { get; set; }

        public override string DisableCountry()
        {
            return "Uneditable";
        }
        #endregion
    }
}

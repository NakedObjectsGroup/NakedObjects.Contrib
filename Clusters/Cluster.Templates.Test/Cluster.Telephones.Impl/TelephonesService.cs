using Cluster.Telephones.Api;
using NakedObjects;
using System.Linq;
using System.Collections.Generic;
using System;
using NakedObjects.Util;
using System.ComponentModel.DataAnnotations;
using Cluster.Users.Api;
using System.Configuration;
using Cluster.Countries.Api;

namespace Cluster.Telephones.Impl
{
    public class TelephoneService : ITelephoneService
    {
        #region Injected Services
        public ICountryService CountryService { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Telephones
        public ITelephoneDetails CreateNewTelephoneDetails()
        {
            return Container.NewTransientInstance<TelephoneDetails>();
        }

        public ITelephoneDetails FindTelephoneDetailsById(int telephoneId)
        {
            return Container.Instances<TelephoneDetails>().Single(x => x.Id == telephoneId);
        }

        public IQueryable<TelephoneCountryCode> FindTelephoneCountryCodes(string matching)
        {
            string m = matching.Trim().ToUpper();

            //This pattern is necessary as you can't join to a queryable of type Interface
            var isoCodesForMatchingCountries = CountryService.FindCountryByName(matching).Select(x => x.ISOCode).ToList();

            IList<String> allCodes = CountryService.AllCountries().Select(x => x.ISOCode).ToList(); ;
            var codes = Container.Instances<TelephoneCountryCode>();

            var q = from code in codes
                     where code.CountryCallingCode.Contains(m) ||
                        code.ISOCountryCode.ToUpper().Contains(m) ||
                        (isoCodesForMatchingCountries.Contains(code.ISOCountryCode))
                    select code;

            return q.Distinct().OrderBy(X => X.ISOCountryCode);
        }



        public TelephoneCountryCode TelephoneCountryCode(string forISOCountryCode)
        {
            return Container.Instances<TelephoneCountryCode>().Single(x => x.ISOCountryCode == forISOCountryCode);
        }

        #endregion

   }
}

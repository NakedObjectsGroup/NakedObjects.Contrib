using Cluster.Countries.Api;
using NakedObjects;
using System.Linq;
using System.Collections.Generic;
using System;
using NakedObjects.Util;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Cluster.Countries.Impl
{
    public class CountryService : ICountryService
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        #region Countries
        public IQueryable<ICountry> AllCountries()
        {
            return Container.Instances<Country>();
        }

        public IQueryable<ICountry> FindCountryByName([RegularExpression(Country.CountryNameRegEx)] string matching)
        {
            return Container.Instances<Country>().Where(x => x.Name.ToUpper().Contains(matching.Trim().ToUpper()));
        }

        public ICountry FindCountryByCode([RegularExpression(Country.ISOCountryCodeRegEx)] string code)
        {
            return Container.Instances<Country>().Single(x => x.ISOCode == code);
        }

        /// <summary>
        /// Based on DefaultCountryISOCode in App.Settings
        /// </summary>
        /// <returns></returns>
        public ICountry DefaultCountry()
        {
            return FindCountryByCode(AppSettings.DefaultCountryISOCode());
        }

        #endregion
    }
}

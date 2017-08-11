using System.Collections.Generic;
using System.Linq;

namespace Cluster.Countries.Api
{
    public interface ICountryService
    {
        #region Countries
        IQueryable<ICountry> AllCountries();

        IQueryable<ICountry> FindCountryByName(string match);

        ICountry FindCountryByCode(string exactMatch);

        ICountry DefaultCountry();
        #endregion
    }
}

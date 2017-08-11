using System;
using System.Data.Entity;
using System.IO;
using Cluster.Countries.Api;
using Cluster.Countries.Impl;


namespace Cluster.Countries.Test
{
    public class CountriesTestInitializer : DropCreateDatabaseAlways<CountriesTestDbContext>
    {
        protected override void Seed(CountriesTestDbContext context)
        {
            AllCountries(context);
        }


        public static void AllCountries(ICountriesDbContext context)
        {
            DbSet<Country> dbSet = context.Countries;
            NewCountry(dbSet, "Afghanistan", CountryCodes.AFGHANISTAN);
            NewCountry(dbSet, "Australia", CountryCodes.AUSTRALIA);
            NewCountry(dbSet, "Austria", CountryCodes.AUSTRIA);
            var uk = NewCountry(dbSet, "United Kingdom", CountryCodes.UK);
            var usa = NewCountry(dbSet, "United States", CountryCodes.USA);
            NewCountry(dbSet, "France", CountryCodes.FRANCE);
            NewCountry(dbSet, "Ireland", CountryCodes.IRELAND);
            NewCountry(dbSet, "Zimbabwe", CountryCodes.ZIMBABWE);
        }

        public static Country NewCountry(DbSet<Country> dbSet, string name, string code)
        {
            var c = new Country()
            {
                Name = name,
                ISOCode = code,
            };
            dbSet.Add(c);
            return c;
        }

   }
}


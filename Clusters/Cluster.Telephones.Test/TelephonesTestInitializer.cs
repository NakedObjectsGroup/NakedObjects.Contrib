using System;
using System.Data.Entity;
using System.IO;
using Cluster.Countries.Api;
using Cluster.Telephones.Api;
using Cluster.Telephones.Impl;

namespace Cluster.Telephones.Test
{
    public class TelephonesTestInitializer : DropCreateDatabaseAlways<TelephonesTestDbContext>
    {
        protected override void Seed(TelephonesTestDbContext context)
        {
            AllTelephoneCodes(context);
            AllMockCountries(context.MockCountries);
        }


        public static void AllTelephoneCodes(ITelephonesDbContext context)
        {
            DbSet<TelephoneCountryCode> dbSet = context.TelephoneCountryCodes;
            NewTelephoneCountryCode(dbSet, CountryCodes.AFGHANISTAN, "93", "?");
            NewTelephoneCountryCode(dbSet, CountryCodes.AUSTRALIA, "61", "0011");
            NewTelephoneCountryCode(dbSet, CountryCodes.AUSTRIA, "43", "00");
            NewTelephoneCountryCode(dbSet, CountryCodes.UK, "00", "44");
            NewTelephoneCountryCode(dbSet, CountryCodes.USA, "010", "1");
            NewTelephoneCountryCode(dbSet, CountryCodes.IRELAND, "00", "353");

            NewTelephoneCountryCode(dbSet, CountryCodes.ZIMBABWE, "00", "263");
        }

        public static void AllMockCountries(DbSet<MockCountry> dbSet)
        {
            var uk = NewCountry(dbSet, "United Kingdom", CountryCodes.UK);
            var usa = NewCountry(dbSet, "United States", CountryCodes.USA);
            NewCountry(dbSet, "Ireland", CountryCodes.IRELAND);
        }

        public static TelephoneCountryCode NewTelephoneCountryCode(
            DbSet<TelephoneCountryCode> dbSet, string countryCode, string prefix, string callingCode,
            string displayFromOther = null,
            string displayFromSame = null,
            string dialFromOther = null,
            string dialFromSame = null
            )
        {
            var tcc = new TelephoneCountryCode()
            {
                ISOCountryCode = countryCode,
                InternationalCallPrefix = prefix,
                CountryCallingCode = callingCode,
                FormatDisplayNumberFromOtherCountry = displayFromOther,
                FormatDisplayNumberFromSameCountry = displayFromSame,
                FormatNumberToDialFromOtherCountry = dialFromOther,
                FormatNumberToDialFromSameCountry = dialFromSame
            };
            dbSet.Add(tcc);
            return tcc;
        }



        public static ICountry NewCountry(DbSet<MockCountry> dbSet, string name, string code)
        {
            var c = new MockCountry()
            {
                Name = name,
                ISOCode = code,
            };
            dbSet.Add(c);
            return c;
        }
   }
}


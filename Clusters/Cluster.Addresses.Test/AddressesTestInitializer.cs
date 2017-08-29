using System;
using System.Data.Entity;
using Cluster.Addresses.Impl;
using Cluster.Countries.Api;

namespace Cluster.Addresses.Test
{
	public class AddressesTestInitializer : DropCreateDatabaseAlways<AddressesTestDbContext>
	{
		protected override void Seed(AddressesTestDbContext context)
		{
			AllMockCountries(context.MockCountries);
			// Not required in versions NOF>6: NewTransaction(); //As countries are looked up when creating addresses

			// TODO: UKAddresses(context);
			// TODO: GenericAddresses(context);
			Associations(context);
		}

		#region AllMockCountries

		public static void AllMockCountries(DbSet<MockCountry> dbSet)
		{
			var uk = NewCountry(dbSet, "United Kingdom", CountryCodes.UK);
			var usa = NewCountry(dbSet, "United States", CountryCodes.USA);
			NewCountry(dbSet, "Ireland", CountryCodes.IRELAND);
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
		#endregion
		
		public void Associations(IAddressesDbContext context)
		{
			DbSet<AddressTypeForCountry> dbSet = context.AddressTypeForCountries;
			NewAddressTypeForCountry(dbSet, CountryCodes.UK, typeof(UKAddress));
		}

		public static AddressTypeForCountry NewAddressTypeForCountry(DbSet<AddressTypeForCountry> dbSet, string isoCode, Type addressType)
		{
			var atfc = new AddressTypeForCountry
			{
				CountryISOCode = isoCode,
				AddressType = addressType.FullName
			};
			dbSet.Add(atfc);
			return atfc;
		}

		#region TODO

		public void UKAddresses(IAddressesDbContext context)
		{
			DbSet<AbstractAddress> dbSet = context.Addresses; // TODO:
			NewUKAddress(dbSet, "Woodlark", "RG9 4LZ");
			NewUKAddress(dbSet, "Tigoni", "RG9 4LZ");
			NewUKAddress(dbSet, "Meadowcroft", "RG9 4LZ");
			NewUKAddress(dbSet, "Lilac Cottage", "RG9 4LY");
			NewUKAddress(dbSet, "Yew Tree Cottage", "RG9 4LY");
		}

		public void GenericAddresses(IAddressesDbContext context)
		{
			DbSet<AbstractAddress> dbSet = context.Addresses; // TODO:
			NewGenericAddress(dbSet, "101 Page Brook Road", "Carlisle", "MA 02139", CountryCodes.USA);
		}
		#endregion

		public UKAddress NewUKAddress(DbSet<AbstractAddress> dbSet, string line1, string postcode)
		{
			var a = new UKAddress
			{
				Line1 = line1,
				Line5 = postcode
			};
			dbSet.Add(a);
			return a;
		}

		public GenericAddress NewGenericAddress(DbSet<AbstractAddress> dbSet,
			string line1, string line2, string line3, string isoCode)
		{
			var a = new GenericAddress
			{
				Line1 = line1,
				Line2 = line2,
				Line3 = line3,
				ISOCode = isoCode
			};
			dbSet.Add(a);
			return a;
		}

	}
}

using System.Linq;
using Cluster.Countries.Api;
using NakedObjects;

namespace Cluster.Telephones.Test
{
    public class MockCountryService : ICountryService
    {
        public IDomainObjectContainer Container { set; protected get; }
  
        public IQueryable<ICountry> AllCountries()
        {
            return Container.Instances<MockCountry>();
        }

        public IQueryable<ICountry> FindCountryByName(string match)
        {
            return Container.Instances<MockCountry>().Where(x => x.Name.ToUpper().Contains(match.Trim().ToUpper()));
 
        }

        public ICountry FindCountryByCode(string exactMatch)
        {
            return Container.Instances<MockCountry>().Single(x => x.ISOCode == exactMatch);
        }

        public ICountry DefaultCountry()
        {
            //throw new NotImplementedException();
			return FindCountryByCode(AppSettings.DefaultCountryISOCode());
		}
	}

    public class MockCountry : ICountry
    {
		public IDomainObjectContainer Container { set; protected get; }
		
		public virtual int Id { get; set; }


        public virtual string Name { get; set; }

        
        public virtual string ISOCode { get; set; }

        
        public override string ToString()
        {
			ITitleBuilder t = Container.NewTitleBuilder(); // revised for NOF7
            t.Append(Name);
            return t.ToString();
        }
      
    }
}

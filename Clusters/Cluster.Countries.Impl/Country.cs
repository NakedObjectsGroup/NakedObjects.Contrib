using Cluster.Countries.Api;
using NakedObjects;

namespace Cluster.Countries.Impl
{
    [Bounded, Immutable]
    public class Country : ICountry
    {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public const string CountryNameRegEx = @"^[a-zA-Z\s]+$";

        [Title]
        public virtual string Name { get; set; }

        public const string ISOCountryCodeRegEx = @"^[A-Z\s]{1,3}$";


        public virtual string ISOCode { get; set; }     
    }
}

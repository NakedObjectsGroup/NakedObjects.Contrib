using System.ComponentModel;
using System.Configuration;
using System.Linq;
using Cluster.Names.Api;
using NakedObjects;
using NakedObjects.Util;

namespace Cluster.Names.Impl
{
    public class NameService : INameService
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region FindById
        public IClusterManagedName FindById(int nameId)
        {
            return Container.Instances<AbstractName>().Single(x => x.Id == nameId);
        }
        #endregion

        #region CreateNewName
        public IClusterManagedName CreateNewNameOfType([DefaultValue(NameTypes.WesternName)] NameTypes ofType)
        {
            //Ignoring ofType for the moment as there is only one impl.
            return Container.NewTransientInstance<WesternName>();
        }

        public IClusterManagedName CreateNewName()
        {
            string defaultType = AppSettings.DefaultNameType();
            return (IClusterManagedName) Container.NewTransientInstance(TypeUtils.GetType(defaultType));
        }

        #endregion

        #region Finding by name
        public IQueryable<T> FindByName<T>(string match) where T : class, IIndividualWithClusterManagedName
        {
            var nameMatches = FindMatchingNames(match);
            var targets = Container.Instances<T>();

            return from n in nameMatches
                   from t in targets
                   where t.NameId == n.Id
                   select t;
        }

        public IQueryable<IClusterManagedName> FindMatchingNames(string match)
        {
            var names = Container.Instances<AbstractName>();

            var matches = match.Split(' ');
            foreach (string part in matches)
            {
                //TODO: Take out all non-alpha characters
                names = names.Where(x => x.Searchable.Contains(part.Trim().ToUpper()));
            }
            return names;
        }
        #endregion



    }
}

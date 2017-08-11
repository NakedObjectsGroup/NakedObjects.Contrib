
using System.Linq;
namespace Cluster.Names.Api
{
    /// <summary>
    /// Defines a service that can create and retrieve cluster-managed names
    /// </summary>
   public  interface INameService
    {

       IClusterManagedName FindById(int nameId);

       /// <summary>
       /// Returns a transient object.  If the specified type is not explicitly supported, it will return a generic implementation
       /// </summary>
       IClusterManagedName CreateNewNameOfType(NameTypes type);

       /// <summary>
       /// Creates a new Name of the defautl type, which is specified in Config e.g:
       /// key="DefaultNameType" value="Cluster.Names.Impl.WesternName"
       /// </summary>
       /// <returns></returns>
       IClusterManagedName CreateNewName();

       /// <summary>
       /// Allows re-use of Name searching logic
       /// </summary>
       IQueryable<T> FindByName<T>(string match) where T : class, IIndividualWithClusterManagedName;

    }
}

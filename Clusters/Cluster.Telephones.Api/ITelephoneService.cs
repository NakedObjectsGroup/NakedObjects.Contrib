using System.Collections.Generic;
using System.Linq;

namespace Cluster.Telephones.Api
{
    public interface ITelephoneService
    {
        /// <summary>
        /// Returns a transient object
        /// </summary>
        /// <returns></returns>
        ITelephoneDetails CreateNewTelephoneDetails();

        ITelephoneDetails FindTelephoneDetailsById(int id);
    }
}

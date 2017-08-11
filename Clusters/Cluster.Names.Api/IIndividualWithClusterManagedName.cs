using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster.Names.Api
{

    /// <summary>
    /// Similar to IIndividual, but where the name is known to be managed by the Name cluster (thus allowing re-use of searching).
    /// </summary>
    public interface IIndividualWithClusterManagedName : IIndividual
    {
        //Persistent property holding the FK for the Name object.
        int NameId { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cluster.System.Api;

namespace Cluster.Names.Api
{
    /// <summary>
    /// Role interface implemented by any object that can provide an individual's name.
    /// </summary>
    public interface IIndividual : IDomainInterface
    {
        IName Name { get; }
    }
}

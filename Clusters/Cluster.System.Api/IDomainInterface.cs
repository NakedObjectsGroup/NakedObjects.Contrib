using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects;

namespace Cluster.System.Api
{
    public interface IDomainInterface : IHasIntegerId
    {
        string ToString();
    }
}

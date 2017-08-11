using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Batch.Api
{
    /// <summary>
    /// Batch process that is on a persistent object
    /// </summary>
    public interface IPersistentBatchProcess : IBatchProcess, IDomainInterface
    {
    }
}

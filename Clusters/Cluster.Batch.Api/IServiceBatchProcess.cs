using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects;

namespace Cluster.Batch.Api
{
    /// <summary>
    /// Role interface implemented by a service that has a method to be called periodically by the process scheduler
    /// </summary>
    public interface IServiceBatchProcess : IBatchProcess, IViewModel
    {
    }
}

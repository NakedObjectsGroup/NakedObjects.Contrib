using System;
using System.Collections.Generic;
using System.Linq;

namespace Cluster.Batch.Api
{
    public interface IBatchProcess
    {
        /// <summary>
        /// If successful, returns a success message which may summarise what happened (e.g. number of letters issued).
        /// If any exception is thrown, this will be caught by the calling function and recorded. 
        /// </summary>
        /// <returns></returns>
        string Invoke();
    }
}

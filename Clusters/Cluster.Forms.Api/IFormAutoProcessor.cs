using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;

namespace Cluster.Forms.Api
{
    /// <summary>
    /// Role interface implemented by a service in an external cluster, which can
    /// process a FormSubmission -  typically to extract data from it and create or
    /// update other domain objects in its own cluster.
    /// </summary>
    public interface IFormAutoProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formSubmission"></param>
        /// <param name="successful"></param>
        /// <param name="message">If process was not successful then there should be a message</param>
        /// <param name="result">Process may optionally return a result object to be returned to the user</param>
        void Process(IFormSubmission formSubmission, out bool successful, out string message, out object result );
    }
}

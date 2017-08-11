using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace Cluster.System.Api
{
    public interface IUpdateableEntity : IHasIntegerId
    {
        /// <summary>
        /// Must be updated by the domain object;
        /// should be marked up with [ConcurrencyCheck]
        /// </summary>
        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
         DateTime LastModified { get; set; }

        /// <summary>
        /// Within Persiting method, object should set LastModified
        /// </summary>
        void Persisting();

        /// <summary>
        /// Within Updating method, object should set LastModified
        /// </summary>
        void Updating();
      
    }
}

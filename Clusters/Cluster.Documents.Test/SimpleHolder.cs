using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using Cluster.Documents.Api;

namespace Cluster.Documents.Test
{
    /// <summary>
    /// Mock implementation of IDocumentHolder
    /// </summary>
    public class SimpleHolder : IDocumentHolder
    {
        #region Injected Services
        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service; 'injc' to add an injected Container

        #endregion
        #region Life Cycle Methods
        // This region should contain any of the 'life cycle' convention methods (such as
        // Created(), Persisted() etc) called by the framework at specified stages in the lifecycle.


        #endregion

        [Disabled]
        public virtual int Id { get; set; }

    }
}


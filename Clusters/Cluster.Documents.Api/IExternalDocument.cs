using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Documents.Api
{
    /// <summary>
    /// Role interface implemented by objects that can be deemed to be documents, and thus associated
    /// with an ExternalDocument
    /// </summary>
    public interface IExternalDocument : IDomainInterface
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;

namespace Cluster.Forms.Api
{
    /// <summary>
    /// Result interface representing a submitted eForm. 
    /// </summary>
    public interface IFormSubmission : IDomainInterface
    {
        string FormCode { get; }

        string Value(string fieldName);
    }
}

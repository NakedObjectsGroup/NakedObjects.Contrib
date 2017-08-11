using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Tasks.Api
{
    /// <summary>
    /// Role interface implemented by any domain object that needs to be associated with a Task - 
    /// typically representing an object that the assignee will need to examine or modify
    /// </summary>
    [Named("Context")]
    public interface ITaskContext : IDomainInterface
    {
    }
}

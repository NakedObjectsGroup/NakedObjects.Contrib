using System;
using Cluster.Tasks.Api;
using NakedObjects;

namespace Cluster.Tasks.Impl
{
    /// <summary>
    /// Records changes to a Task
    /// </summary>
    [Immutable(WhenTo.OncePersisted)]
    public class TaskHistory
    {
        [Hidden]
        public virtual int Id { get; set; }

        public virtual int TaskId { get; set; }
          
        [Hidden]
        public virtual Task Task { get; set; }

        [MemberOrder(10)]
        public virtual DateTime DateTime { get; set; }

        [MemberOrder(20)]
        public virtual TaskStatusValues Status { get; set; }

        [MemberOrder(30)]
        public virtual string AssignedTo { get; set; }

        [MemberOrder(40)]
        public virtual string ChangeMadeBy { get; set; }
      
    }
}

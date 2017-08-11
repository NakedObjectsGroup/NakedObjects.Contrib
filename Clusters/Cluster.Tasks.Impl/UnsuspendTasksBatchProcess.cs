using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Cluster.Batch.Api;
using Cluster.System.Api;
using Cluster.Tasks.Api;
using NakedObjects;

namespace Cluster.Tasks.Impl
{
    [NotPersisted, NotMapped]
    public class UnsuspendTasksBatchProcess : IServiceBatchProcess, IViewModel
    {
        #region InjectedServices
        public IDomainObjectContainer Container { set; protected get; }

        public IClock Clock { set; protected get; }

        #endregion
        public string UnsuspendTasksFor(DateTime date) {
            var targetDate = date.Date;
            //var due = Container.Instances<Task>();
            var due = Container.Instances<Task>().
                 Where(x => x.Status == TaskStatusValues.Suspended &&
                     x.SuspendedUntil <= targetDate);
            foreach (Task t in due)
            {
                t.Unsuspend(null);
            }
           return due.Count().ToString() + " Tasks unsuspended";
           
        }

        public string UnsuspendTasksForToday()
        {
            return UnsuspendTasksFor(Clock.Today());
        }

        public string Invoke()
        {
            return UnsuspendTasksForToday();
        }

        #region Implementation of IViewModel
        public string[] DeriveKeys()
        {
            throw new NotImplementedException();
        }

        public void PopulateUsingKeys(string[] keys)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

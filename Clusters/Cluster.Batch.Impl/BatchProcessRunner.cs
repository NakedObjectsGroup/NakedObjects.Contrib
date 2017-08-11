using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NakedObjects;
using IClock = Cluster.System.Api.IClock;
using NakedObjects.Async;

namespace Cluster.Batch.Impl {
    public class BatchProcessRunner {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        public IClock Clock { set; protected get; }

        public BatchRepository BatchRepository { set; protected get; }

        public AsyncService AsyncService { set; protected get; }

        #endregion

        public void DoWork() {
            RunAllProcessesDueBy(Clock.Today());
        }

        public void RunAllProcessesDueBy(DateTime dateTime) {
            IList<BatchProcessDefinition> due = BatchRepository.ListProcessesWithNextRunsDueBy(dateTime);
          
            var tasks = due.Select(bpd => AsyncService.RunAsync(() => BatchRepository.FindAndRunProcDef(bpd.Id))).ToArray();

            Task.WaitAll(tasks);
        }
    }
}
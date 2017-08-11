using Cluster.Batch.Impl;
using NakedObjects.Boot;

namespace App.ScheduledProcessRunner {
    public class StartPoint : IBatchStartPoint {

        public BatchProcessRunner BatchProcessRunner { protected get; set; }

        public void Execute() {
           BatchProcessRunner.DoWork();
        }
    }
}

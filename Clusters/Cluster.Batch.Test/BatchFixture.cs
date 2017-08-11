using System;
using System.Data.Entity;
using System.IO;
using Cluster.Batch.Impl;
using Cluster.Users.Mock;
using NakedObjects;

namespace Cluster.Batch.Test
{
    public class BatchFixture
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public Cluster.System.Api.IClock Clock { set; protected get; }

        #endregion
        public  void Install()
        {
            TestProcessDefinitions();
            TestPersistentProcesses();
        }

        public void TestProcessDefinitions()
        {
            NewProcessDefinition( "Process1", typeof(MockSP));
            NewProcessDefinition("Process2", typeof(MockSP));
            NewProcessDefinition("Process3", typeof(MockSPThrowsException));
            NewProcessDefinition("Process4", typeof(MockSP), null, Status.Archived);
            NewProcessDefinition("Process5", typeof(MockPersistentSP), 1);
            NewProcessDefinition("Process6", typeof(MockPersistentSP), 2);
            NewProcessDefinition("Process7", typeof(MockPersistentSP), 100);
        }

        public void TestPersistentProcesses()
        {
            NewMockPersistentSP( "Outcome1");
            NewMockPersistentSP( "Outcome2");
        }

        public  MockPersistentSP NewMockPersistentSP(string outcome)
        {
            MockPersistentSP mock = Container.NewTransientInstance<MockPersistentSP>();
            mock.SpecificOutcome = outcome;
            Container.Persist(ref mock);
            return mock;
        }

        public  BatchProcessDefinition NewProcessDefinition(string name, Type processType, int? instanceId = null, Status status = Status.Active)
        {
            BatchProcessDefinition p = Container.NewTransientInstance<BatchProcessDefinition>();

               p.Name = name;
                p.Status = status;
                p.Priority = 1;
               p.Frequency = Frequency.Daily;
                p.NumberOfAttemptsEachRun = 1;
                p.FirstRun = new DateTime(1999,1,1);
                p.NextRun = new DateTime(2000, 1, 1);
                p.LastRun = new DateTime(2000, 12, 1);
                p.ClassToInvoke = processType.FullName;
                p.ProcessInstanceId = instanceId;
               // p.LastModified = Clock.Now();
                Container.Persist(ref p);
            return p;
        }
   }
}


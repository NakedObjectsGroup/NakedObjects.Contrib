using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Batch.Impl
{
    /// <summary>
    /// Repository to provide access to BatchProcessDefinitions and associated BatchLogs
    /// </summary>
    [DisplayName("Batch")]
    public class BatchRepository
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public Cluster.System.Api.IClock Clock { set; protected get; }
        #endregion

        public BatchProcessDefinition CreateNewProcessDefinition()
        {
            return Container.NewTransientInstance<BatchProcessDefinition>();
        }

        #region FindByName
        public IQueryable<BatchProcessDefinition> FindProcessDefinitions([Optionally] string name, [Optionally] Status? status)
        {
            var q = from obj in Container.Instances<BatchProcessDefinition>()
                    where obj.Name.ToUpper().Contains(name.ToUpper())
                    select obj;
            if (status != null)
            {
                q = q.Where(pd => pd.Status == status);
            }
            return q;
        }
        #endregion

        #region FindById
        [NakedObjectsIgnore]
      public BatchProcessDefinition FindById(int id)
      {
          return Container.Instances<BatchProcessDefinition>().Single(x => x.Id == id);
      }
      #endregion
      
        [TableView(true, "Name", "NextRun")]
        public IQueryable<BatchProcessDefinition> ActiveProcesses()
        {
            return from obj in Container.Instances<BatchProcessDefinition>()
                   where obj.Status == Status.Active
                   orderby obj.NextRun
                   select obj;
        }

        [TableView(true, "Name", "NextRun")]
        public IList<BatchProcessDefinition> ListProcessesWithNextRunsDueBy(DateTime dateTime)
        {
            return (from obj in ActiveProcesses()
                   where obj.NextRun != null && obj.NextRun <= dateTime
                   orderby obj.NextRun
                   select obj).ToList();
            
        }

        public DateTime Default0ListProcessesWithNextRunsDueBy()
        {
            return Clock.Now();
        }

        internal  void FindAndRunProcDef(int id)
        {
            var bpd = FindById(id).RunProcessAndRecordOutcome();
            Console.WriteLine("Batch Process completed: " + id);
        }
    }
}

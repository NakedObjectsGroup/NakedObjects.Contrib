using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Cluster.Batch.Api;
using Cluster.System.Api;
using Cluster.Users.Api;
using NakedObjects;
using NakedObjects.Util;

namespace Cluster.Batch.Impl
{
    /// <summary>
    /// Defines a process that is to be run in batch mode
    /// </summary>
    public class BatchProcessDefinition : IUpdateableEntity
    {
        #region Injected Services
        public Cluster.System.Api.IClock Clock { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }

        public IUserService UserService { set; protected get; }

        #endregion
        #region LifeCycle methods
        public void Created()
        {
            Status = Status.Active;
        }
        public void Persisting()
        {
            CalculateNextRunDate(Clock.Now());
            LastModified = Clock.Now();
        }

        public void Updating()
        {
            LastModified = Clock.Now();
        }
        #endregion
        #region Title
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Name);
            return t.ToString();
        }
        #endregion

        public string DisablePropertyDefault()
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(Status == Status.Archived, "Archived Processes may not be edited");
            return rb.Reason;
        }

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(10), StringLength(50)]
        public virtual string Name { get; set; }

        #region Status and Status-change actions
        [MemberOrder(15), Disabled]
        public virtual Status Status { get; set; }

        [MemberOrder(10)]
        public void Suspend()
        {
            Status = Status.Suspended;
            NextRun = null;
        }

        public bool HideSuspend()
        {
            return Status != Status.Active;
        }

         [MemberOrder(12)]
        public void Resume()
        {
            Status = Status.Active;
            CalculateNextRunDate(Clock.Now());
        }

        public bool HideResume()
        {
            return Status != Status.Suspended;
        }

         [MemberOrder(14)]
        public void Archive()
        {
            Status = Status.Archived;
            NextRun = null;
            LastRun = null;
        }

        public bool HideArchive()
        {
            return Status == Status.Archived;
        }

        #endregion

        [MemberOrder(20), Optionally, StringLength(255)]
        public virtual string Description { get; set; }

        #region Process Class and InstanceId
        /// <summary>
        /// Fully-qualified name of the class, which must implement IScheduledProcess, that holds 
        /// the method to be run.
        /// </summary>
        [MemberOrder(30)]
        public virtual string ClassToInvoke { get; set; }

        private Type TypeToInvoke()
        {
            return TypeUtils.GetType(ClassToInvoke);
        }
        public string Validate(string classToInvoke, int? processInstanceId)
        {
            Type t = TypeUtils.GetType(classToInvoke);
            if (!ImplementsIBatchProcess(t))
            {
                return "Class specified does not implement IBatchProcess";
            }
            if (processInstanceId != null && !ImplementsIPersistentScheduledProcess(t))
            {
                return "If Process Instance Id is set, class must implement IPersistentScheduledProcess";
            }
            if (ImplementsIPersistentScheduledProcess(t) && processInstanceId == null)
            {
                return "If class implements IPersistentScheduledProcess, Process Instance Id must be specified";
            }
            if (processInstanceId != null && processInstanceId.Value > 9999)
            {
                return "Instance Id must be < 9999";
            }
            return null;
        }

        private static bool ImplementsIPersistentScheduledProcess(Type t)
        {
            return typeof(IPersistentBatchProcess).IsAssignableFrom(t);
        }

        private static bool ImplementsIBatchProcess(Type t)
        {
            return typeof(IBatchProcess).IsAssignableFrom(t);
        }

        [Description("Relevant only to Persistent Processes. If left blank, Process is instantiated afresh each time.")]
        [Optionally, MemberOrder(40)]
        public virtual int? ProcessInstanceId { get; set; }


        #endregion

        /// <summary>
        /// Priority determines the order in which processes due in the same call are run.
        /// Lowest number has highest priority.
        /// </summary>
        [Range(1, 999), MemberOrder(50), DefaultValue(1)]
        public virtual int Priority { get; set; }


        #region Dates and Frequency
        #region First Run
        [MemberOrder(70)]
        public virtual DateTime? FirstRun { get; set; }


        public virtual void ModifyFirstRun(DateTime? value)
        {
            FirstRun = value;
            CalculateNextRunDate(Clock.Now());
        }

        public DateTime DefaultFirstRun()
        {
            return Clock.Today();
        }
        #endregion

        #region Frequency
        [MemberOrder(71), DefaultValue(Frequency.ManualRunsOnly)]
        public virtual Frequency Frequency { get; set; }


        public virtual void ModifyFrequency(Frequency value)
        {
            Frequency = value;
            CalculateNextRunDate(Clock.Now());
        }
        #endregion

        #region Next Run
        [MemberOrder(72), Disabled]
        public virtual DateTime? NextRun { get; set; }

        [NakedObjectsIgnore]
        public void CalculateNextRunDate(DateTime asOf)
        {
            if (Frequency == Frequency.ManualRunsOnly)
            {
                NextRun = null;
                return;
            }
            if (asOf > LastRun)
            {
                NextRun = null;
                return;
            }
            if (asOf < FirstRun)
            {
                NextRun = FirstRun;
                return;
            }
            if (Frequency == Frequency.Daily)
            {
                NextRun = asOf.AddDays(1).Date;
                return;
            }
            if (Frequency == Frequency.WeeklyOnMonday)
            {
                if (asOf.StartOfWeek() == asOf.Date)
                {
                    NextRun = asOf.AddDays(1).Date;
                }
                else
                {
                    NextRun = asOf.StartOfWeek().AddDays(8).Date;
                }

                if (NextRun > LastRun) NextRun = null;
                return;
            }
            if (Frequency == Frequency.MonthlyOn1stOfMonth)
            {
                NextRun = asOf.AddMonths(1).StartOfMonth().Date;
                if (NextRun > LastRun) NextRun = null;
                return;
            }
        }

        #endregion

        #region Last Run
        /// <summary>
        /// Inclusive date/time.  The process will be run on the next call on or after the LastInvocation
        /// and then not afterwards.
        /// </summary>
        [MemberOrder(74), Optionally]
        public virtual DateTime? LastRun { get; set; }

        public virtual void ModifyLastRun(DateTime? value)
        {
            LastRun = value;
            CalculateNextRunDate(Clock.Now());
        }
        #endregion

        public string Validate(DateTime firstRun, DateTime? lastRun)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(firstRun < Clock.Today() || lastRun < Clock.Today(), "Dates cannot be before today");
            rb.AppendOnCondition(lastRun != null && lastRun.Value < firstRun, "Last Run cannot be before First Run");
            return rb.Reason;
        }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
        #endregion

        /// <summary>
        /// If the process does not execute successfully, it will be repeated up to this number of times.
        /// </summary>
        [MemberOrder(60), Range(1, 9), DefaultValue(1)]
        public virtual int NumberOfAttemptsEachRun { get; set; }

        #region Run Process

         [MemberOrder(20)]
        public BatchLog RunProcessManually()
        {
            return RunProcessAndRecordOutcome(true);
        }

        public bool HideRunProcessManually()
        {
            return Status == Status.Archived;
        }

        internal BatchLog RunProcessAndRecordOutcome(bool isManual = false)
        {
            string outcome = null;
            bool success = false;
            try
            {
                outcome = RunProcess();
                success = true;
            }
            catch (Exception e)
            {
                outcome = e.Message;
            }
            return RecordOutcome(outcome, 1, success, isManual);
        }

        private string RunProcess()
        {
            IBatchProcess process = null;
            var type = TypeToInvoke();
            if (ImplementsIPersistentScheduledProcess(type))
            {
                int id = this.ProcessInstanceId.Value;
                process = GetInstanceOfPersistentScheduledProcess(type, id);
                if (process == null)
                {
                    throw new Exception("No instance of type "+type+" with Id ="+id);
                }
            }
            else if (ImplementsIBatchProcess(type))
            {
                process = (IServiceBatchProcess) Container.NewViewModel(type);
            }
            else
            {
                throw new DomainException("Type is not a recognised Scheduled Process implementation");
            }
            string outcome = process.Invoke();
            LastRun = Clock.Now();
            CalculateNextRunDate(LastRun.Value);
            return outcome;
        }

        private IPersistentBatchProcess GetInstanceOfPersistentScheduledProcess(Type type, int id)
        {
            MethodInfo m = GetType().GetMethod("FindById");
            MethodInfo gm = m.MakeGenericMethod(new[] { type });
            return (IPersistentBatchProcess)gm.Invoke(this, new object[] { id });
        }

        /// <summary>
        /// Generic method that is created dynamically, elsewhere in code
        /// </summary>
        [NakedObjectsIgnore]
        public T FindById<T>(int id) where T : class, IPersistentBatchProcess
        {
            return Container.Instances<T>().FirstOrDefault(x => x.Id == id);
        }

        private BatchLog RecordOutcome(string outcome, int attemptNo = 1, bool successful = true, bool isManual = false)
        {
            var run = Container.NewTransientInstance<BatchLog>();
            run.ProcessDefinition = this;
            run.WhenRun = Clock.Now();
            run.Outcome = outcome;
            run.AttemptNumber = attemptNo;
            run.Successful = successful;
            if (isManual)
            {
                run.RunMode = RunModes.Manual;
                run.UserId = UserService.CurrentUser().Id;
            }
            else
            {
                run.RunMode = RunModes.AutoScheduled;
            }
            Container.Persist(ref run);
            return run;
        }

        #endregion

        #region Runs

         [MemberOrder(30)]
        [TableView(true, "RunMode", "Successful", "Outcome" )]
        public IQueryable<BatchLog> RecentLogs()
        {
            int id = this.Id;
            return Container.Instances<BatchLog>().Where(x => x.ProcessDefinition.Id == id).OrderByDescending(x => x.WhenRun);
        }

        #endregion


         [MemberOrder(40)]
         public IPersistentBatchProcess RetrievePersistentProcessObject()
         {
              return  GetInstanceOfPersistentScheduledProcess(TypeToInvoke(), ProcessInstanceId.Value);
          }

         public bool HideRetrievePersistentProcessObject()
         {
             return ProcessInstanceId == null;
         }
    }

    public enum Frequency
    {
        ManualRunsOnly,
        Daily,
        WeeklyOnMonday,
        MonthlyOn1stOfMonth
    }

    public enum Status
    {
        Active,
        Suspended,
        Archived
    }
}

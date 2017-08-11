using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cluster.Users.Api;
using Cluster.Tasks.Api;
using NakedObjects;
using NakedObjects.Services;
using Cluster.System.Api;

namespace Cluster.Tasks.Impl
{

    /// <summary>
    /// Represents an task that needs to be completed by an assignee.
    /// </summary>
    public class Task : ITask, IUpdateableEntity
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public IUserService UserFinder { set; protected get; }

        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        public IClock Clock { set; protected get; }

        #endregion
        #region Life Cycle Methods
        public void Created()
        {
            Status = TaskStatusValues.Unsaved;
        }

        public void Persisting()
        {
            Status = TaskStatusValues.Pending;
            History.Add(CreateNewTransientHistory());
            LastModified = Clock.Now();
        }

        public void Updating()
        {
            var hist = CreateNewTransientHistory();
            Container.Persist(ref hist);
            History.Add(hist);
            LastModified = Clock.Now();
        }

        #endregion
        #region Title
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append("Task #").Append(Id);
            return t.ToString();
        }
        #endregion

        #region Properties
        [Hidden]
        public virtual int Id { get; set; }

        [Disabled, MemberOrder(10)]
        public virtual TaskType Type { get; set; }

        [MemberOrder(20), Disabled]
        public virtual TaskStatusValues Status { get; set; }

        #region SuspendedUntil
        [MemberOrder(25), Disabled, Mask("d")]
        public virtual DateTime? SuspendedUntil { get; set; }

        public bool HideSuspendedUntil()
        {
            return Status != TaskStatusValues.Suspended;
        }

        #endregion

        #region AssignedTo Property of type IUser ('Result' interface)

        [Hidden()]
        public virtual int AssignedToId { get; set; }


        private IUser _AssignedTo;

        [NotPersisted(), MemberOrder(30)]
        public IUser AssignedTo
        {
            get
            {
                if (_AssignedTo == null && AssignedToId > 0)
                {
                    _AssignedTo = UserFinder.FindUserById(AssignedToId);
                }
                return _AssignedTo;
            }
            set
            {
                _AssignedTo = value;
                if (value == null)
                {
                    AssignedToId = 0;
                }
                else
                {
                    AssignedToId = value.Id;
                }
            }

        }

        public IQueryable<IUser> AutoCompleteAssignedTo(string match)
        {
            return UserFinder.FindUsersByRealOrUserName(match);
        }

        #endregion

        #region Due
        [MemberOrder(40), Optionally, Mask("d")]
        public virtual DateTime? Due { get; set; }

        public string DisableDue()
        {
            return DisableIfFinished();
        }


        #endregion

        #region Notes
        [MultiLine(NumberOfLines = 10,  Width=20), Optionally, MemberOrder(50)]
        public virtual string Notes { get; set; }

        public string DisableNotes()
        {
            return DisableIfFinished();
        }

        #endregion

        #region Contexts Collection of type ITaskContext

        private ICollection<TaskContextLink> _Context = new List<TaskContextLink>();

        [Hidden]
        public virtual ICollection<TaskContextLink> ContextLinks
        {
            get
            {
                return _Context;
            }
            set
            {
                _Context = value;
            }
        }

        [MemberOrder(110)]
        public void AddContext(ITaskContext value)
        {
            PolymorphicNavigator.AddLink<TaskContextLink, ITaskContext, Task>(value, this);
        }

        public bool HideAddContext()
        {
            return IsFinished();
        }


        [MemberOrder(120)]
        public void RemoveContext(ITaskContext value)
        {
            PolymorphicNavigator.RemoveLink<TaskContextLink, ITaskContext, Task>(value, this);
        }

        public bool HideRemoveContext()
        {
            return IsFinished();
        }

        public string DisableRemoveContext()
        {
            var rb = new ReasonBuilder();
            DisableIfNotAssignedToCurrentUser(rb);
            return rb.Reason;
        }

        [NotPersisted]
        public ICollection<ITaskContext> Contexts
        {
            get
            {
                return ContextLinks.Select(x => x.AssociatedRoleObject).ToList();
            }
        }
        #endregion

        #region History

        internal TaskHistory CreateNewTransientHistory()
        {
            IUser user = UserFinder.CurrentUser();
            var hist = Container.NewTransientInstance<TaskHistory>();
            hist.AssignedTo = AssignedTo.UserName;
            hist.ChangeMadeBy = user.UserName;
            hist.DateTime = Clock.Now();
            hist.Status = Status;
            return hist;
        }

        #region History (collection)
        private ICollection<TaskHistory> _History = new List<TaskHistory>();

        [MemberOrder(100)]
        [TableView(false, "DateTime", "Status", "AssignedTo", "ChangeMadeBy", "Comment")]
        public virtual ICollection<TaskHistory> History
        {
            get
            {
                return _History;
            }
            set
            {
                _History = value;
            }
        }
        #endregion

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
        #endregion
        #endregion

        #region Actions

        #region AssignTo

        [MemberOrder(10)]
        public void AssignTo(IUser user)
        {
            AssignedTo = user;
        }

        public string ValidateAssignTo(IUser user)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(user == AssignedTo, "Task is already assigned to this User");
            return rb.Reason;
        }

        public IQueryable<IUser> AutoComplete0AssignTo([MinLength(3)] string nameMatch)
        {
            return AutoCompleteAssignedTo(nameMatch);
        }

        public bool HideAssignTo()
        {
            return IsFinished();
        }

        [MemberOrder(20)]
        public void AssignToMe([Optionally] bool andStart)
        {
            AssignedTo = UserFinder.CurrentUser();
            if (andStart)
            {
                Status = TaskStatusValues.Started;
            }
        }

        public bool HideAssignToMe()
        {
            return IsFinished();
        }


        #endregion

        #region Start
        [MemberOrder(30)]
        public void Start()
        {
            Status = TaskStatusValues.Started;
        }

        public string DisableStart()
        {
            var rb = new ReasonBuilder();
            DisableIfNotAssignedToCurrentUser(rb);
            return rb.Reason;
        }

        public bool HideStart()
        {
            return Status == TaskStatusValues.Started || Status == TaskStatusValues.Completed || Status == TaskStatusValues.Cancelled;
        }

        protected void DisableIfNotAssignedToCurrentUser(ReasonBuilder rb)
        {
            rb.AppendOnCondition(!IsAssignedToCurrentUser(), "Action not valid unless Task is assigned to you");
        }


        protected bool IsAssignedToCurrentUser()
        {
            return AssignedTo == UserFinder.CurrentUser();
        }
        #endregion

        #region Suspend
        [MemberOrder(40)]
        public void Suspend([Optionally, Range(1,30)] int? days, [Optionally] DateTime? until, [Optionally] string comment)
        {
            if (comment != null)
            {
                Notes += "\nSuspended: " + comment;
            }
            Status = TaskStatusValues.Suspended;
            if (days != null)
            {
                SuspendedUntil = Clock.Today().AddDays(days.Value);
            }
            else
            {
                SuspendedUntil = until;
            }
        }


        public string ValidateSuspend(int? days, DateTime? until, string comment)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition((days == null && until == null) || (days != null && until != null), "Specify EITHER 'days' OR 'until'");
            if (until != null)
            {
                var tomorrow = Clock.Today().AddDays(1);
                var plus30 = Clock.Today().AddDays(30);
                var untilDate = until.Value;
                rb.AppendOnCondition(untilDate < tomorrow || untilDate > plus30, "Can only suspend for 1-30 days");
            }
            return rb.Reason;
        }


        public IList<int?> Choices0Suspend()
        {
            return new List<int?> { 7, 14, 21, 28 };
        }

        public bool HideSuspend()
        {
            return IsFinished() || Status == TaskStatusValues.Suspended;
        }

        [MemberOrder(45)]
        public void Unsuspend([Optionally] string comment)
        {
            if (comment != null)
            {
                Notes += "\nUnsuspended: " + comment;
            }
            Status = TaskStatusValues.Pending;
            SuspendedUntil = null;
        }

        public bool HideUnsuspend()
        {
            return Status != TaskStatusValues.Suspended;
        }

        #endregion

        #region Complete
        [MemberOrder(50)]
        public void Complete([Optionally] string addFinalNote)
        {
            if (addFinalNote != null)
            {
                Notes += "\n" + addFinalNote;
            }
            Status = TaskStatusValues.Completed;
        }

        public string DisableComplete()
        {
            var rb = new ReasonBuilder();
            DisableIfNotAssignedToCurrentUser(rb);
            rb.AppendOnCondition(Status != TaskStatusValues.Started, "Task must be Started before it can be Completed");
            return rb.Reason;
        }

        public bool HideComplete()
        {
            return IsFinished();
        }

        protected bool IsFinished()
        {
            return Status == TaskStatusValues.Completed || Status == TaskStatusValues.Cancelled;
        }

        protected string DisableIfFinished()
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(IsFinished(), "No longer editable");
            return rb.Reason;
        }
        #endregion

        #region Cancel

        [MemberOrder(60)]
        public void Cancel(string reason)
        {
            Notes += "\nReason cancelled: " + reason;
            Status = TaskStatusValues.Cancelled;
        }

        public bool HideCancel()
        {
            return IsFinished();
        }

        public string DisableCancel()
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(Status == TaskStatusValues.Started && !IsAssignedToCurrentUser(), "Can't cancel a Started task unless it is assigned to you");
            return rb.Reason;
        }

        #endregion

        #endregion
    }

    //Polymorphic Links
    public class TaskContextLink : PolymorphicLink<ITaskContext, Task> { }
}

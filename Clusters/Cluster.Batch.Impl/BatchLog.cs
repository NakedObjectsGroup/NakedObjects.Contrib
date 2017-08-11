using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Cluster.Users.Api;
using NakedObjects;

namespace Cluster.Batch.Impl
{
    /// <summary>
    /// Persisted object that holds details of a particular run of a BatchProcessDefinition
    /// </summary>
    [Immutable(WhenTo.OncePersisted)]
    public class BatchLog
    {
        #region Injected Services
        public IUserService UserService { set; protected get; }

        #endregion

        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(WhenRun);
            return t.ToString();
        }
      
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }
      
        [MemberOrder(10)]
        public virtual BatchProcessDefinition ProcessDefinition { get; set; }

          [MemberOrder(20)]
        public virtual DateTime WhenRun { get; set; }

          [MemberOrder(30)]
        public virtual RunModes RunMode { get; set; }
      

        /// <summary>
        /// Will be True unless an Exception was thrown by the process, in whish case the
        /// Exception's message will be captured in the outcome.
        /// </summary>
          [MemberOrder(40)]
        public virtual bool Successful { get; set; }

          [MemberOrder(50)]
        public virtual int AttemptNumber { get; set; }

        /// <summary>
        /// Either a successful outcome or a 
        /// </summary>
         [MemberOrder(60)]
        public virtual string Outcome { get; set; }

        
        #region User Property of type IUser ('Result' interface)

        [Hidden()]
        public virtual int UserId { get; set; }


        private IUser _User;

        /// <summary>
        /// Relevant only when RunMode is Manual, otherwise null.
        /// </summary>
        [NotPersisted(), NotMapped, Disabled, MemberOrder(70)]
        public IUser User
        {
            get
            {
                if (_User == null && UserId > 0)
                {
                    _User = UserService.FindUserById(UserId);
                }
                return _User;
            }
        }
        #endregion

    }

    public enum RunModes
    {
        AutoScheduled, Manual
    }
}

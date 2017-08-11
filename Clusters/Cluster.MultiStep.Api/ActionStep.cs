using System;
using System.Collections.Generic;
using System.Linq;
//using Cluster.System.Api;
using NakedObjects;

namespace Cluster.MultiStep.Api
{
    /// <summary>
    /// Models a user view of a step in completion of an Activity.
    /// </summary>
    public abstract class ActionStep : IViewModel
    {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        //public IClock Clock { set; protected get; }
        #endregion

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(ParentAction);
            if (Stage == Stages.InProcess)
            {
                t.Append(" step", StepNumber);
            }
            else
            {
                t.Append(Stage);
            }
            return t.ToString();
        }

        
        #region Stage & StepNumber
        [Disabled]
        public virtual Stages Stage { get; set; }

        public virtual bool HideStage()
        {
            return true;
        }

        [Disabled, MemberOrder(20)]
        public virtual int StepNumber { get; set; }

        public virtual bool HideStepNumber()
        {
            return true;
        }
        #endregion

        #region Holder
        [Disabled, MemberOrder(10)]
        public virtual MultiStepAction ParentAction { get; set; }

        /// <summary>
        /// Sub-classes may render the ParentAction visible
        /// </summary>
        /// <returns></returns>
        public virtual bool HideParentAction()
        {
            return true;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual string[] DeriveKeys()
        {
            return new string[] { ParentAction.Id.ToString(), Stage.ToString(), StepNumber.ToString() };
        }

        [NakedObjectsIgnore]
        public virtual void PopulateUsingKeys(string[] keys)
        {
            int holderId = int.Parse(keys[0]);
            ParentAction = GetParentActionById(holderId);
            Stage = (Stages)Enum.Parse(typeof(Stages), keys[1]);
            StepNumber = int.Parse(keys[2]);
        }

        protected abstract MultiStepAction GetParentActionById(int holderId);

        #region Actions
        #region GoBackToStart
        public ActionStep GoBackToStart()
        {
            ParentAction.GoBackToStart();
            return ParentAction.Continue();
        }

        public virtual bool HideGoBackToStart()
        {
            return Stage == Stages.Start;
        }
        #endregion

        #region Previous
        public ActionStep Previous()
        {
            ParentAction.GoBackOneStep();
            return ParentAction.Continue();
        }

        public virtual bool HidePrevious()
        {
            return Stage == Stages.Start;
        }
        #endregion

        #region Next
        public virtual ActionStep Next()
        {
            return ParentAction.ProcessSubmittedStep(this);
        }

        public virtual bool HideNext()
        {
            return Stage == Stages.End;
        }

        public virtual string DisableNext()
        {
            return null;
        }
        #endregion

        #region Finish
        public object Finish()
        {
            ParentAction.ProcessSubmittedStep(this); //TODO:  Not sure about this - will advance the step number!
            return ParentAction;
        }

        public virtual bool HideFinish()
        {
            return Stage != Stages.End;
        }
        #endregion
        #endregion
    }


}

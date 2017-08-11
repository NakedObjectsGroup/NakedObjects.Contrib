using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NakedObjects;
using Cluster.System.Api;
using System.ComponentModel.DataAnnotations;

namespace Cluster.MultiStep.Api
{
    public abstract class MultiStepAction : IUpdateableEntity
    {
        #region Injected Services

        public IClock Clock { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region LifeCycle Methods

        public virtual void Persisting()
        {
            LastModified = Clock.Now();
        }

        public virtual void Updating()
        {
            if (Container.IsPersistent(this))
            { //To guard against being called after Persisted()
                LastModified = Clock.Now();
            }
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [Disabled, MemberOrder(35)]
        public virtual int StepNumber { get; set; }

        #region Action: Continue
        public abstract ActionStep Continue();

        public bool HideContinue()
        {
            return false;
        }
        #endregion

        [NakedObjectsIgnore]
        public abstract ActionStep ProcessSubmittedStep(ActionStep fromStep);

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }

        #region Helpers
        protected void CheckStepNumberAndChangeIfValid(ActionStep fromStep)
        {
            if (fromStep.StepNumber > StepNumber)
            {
                throw new DomainException("Invalid step being submitted");
            }
            if (fromStep.StepNumber < StepNumber)
            {
                StepNumber = fromStep.StepNumber;
            }
        }

        protected void AssertStepNumberIs(int stepNumber)
        {
            if (this.StepNumber != stepNumber)
            {
                throw new DomainException("Step number is incorrect");
            }
        }

        protected T CheckStepIs<T>(ActionStep t)
            where T : ActionStep
        {
            if (!typeof(T).IsAssignableFrom(t.GetType()))
            {
                throw new DomainException("Step submitted is of the incorrect type for the current StepNumber of the Activity");
            }
            return t as T;
        }

        protected T CreateFirstStep<T>()
            where T : ActionStep, new()
        {
            return CreateNewStep<T>(Stages.Start, 1);
        }

        protected T CreateIntermediateStep<T>(int stepNumber)
            where T : ActionStep, new()
        {
            return CreateNewStep<T>(Stages.InProcess, stepNumber);
        }

        protected T CreateLastStep<T>(int stepNumber)
            where T : ActionStep, new()
        {
            return CreateNewStep<T>(Stages.End, stepNumber);
        }

        private T CreateNewStep<T>(Stages asStepType, int stepNumber)
            where T : ActionStep, new()
        {
            var step = Container.NewViewModel<T>();
            step.Stage = asStepType;
            step.StepNumber = stepNumber;
            step.ParentAction = this;
            return step;
        }

        [NakedObjectsIgnore]
        public void AdvanceStepNumber()
        {
            StepNumber += 1;
        }

        [NakedObjectsIgnore]
        public void GoBackOneStep()
        {
            StepNumber -= 1;
        }

        [NakedObjectsIgnore]
        public void GoBackToStart()
        {
            StepNumber = 1;
        }

        #endregion

    }
}

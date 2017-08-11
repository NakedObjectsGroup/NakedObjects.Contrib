using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.MultiStep.Api;
using NakedObjects;

namespace Cluster.MultiStep.Test
{
    public class Action1 : MultiStepAction
    {
       public override ActionStep Continue()
        {
           if (StepNumber == 0)  StepNumber =1 ;  //Initialise
            switch (StepNumber)
            {
                case 1:
                    return CreateFirstStep<Step1>();
                case 2:
                    return CreateIntermediateStep<Step2>(2);
                case 3:
                    return CreateLastStep<Step3>(3);
                default:
                    throw new DomainException("Not a valid Step number");
            }
        }

        public override ActionStep ProcessSubmittedStep(ActionStep fromStep)
        {
            CheckStepNumberAndChangeIfValid(fromStep);
            switch (StepNumber)
            {
                case 1:
                    var step = CheckStepIs<Step1>(fromStep);

                    AdvanceStepNumber();
                    return Continue();
                case 2:
                    CheckStepIs<Step2>(fromStep);
                    AdvanceStepNumber();
                    return Continue();
                default:
                    throw new DomainException("Not a valid Step number");
            }
        }
    }

    public class Step1 : TestStep
    {
    }

    public class Step2 : TestStep
    {
    }

    public class Step3 : TestStep
    {
    }


    public class TestStep : ActionStep
    {

        protected override MultiStepAction GetParentActionById(int holderId)
        {
            return Container.Instances<Action1>().Single(x => x.Id == holderId);
        }
    }
}

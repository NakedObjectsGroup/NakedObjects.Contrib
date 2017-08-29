using System;
using Cluster.System.Mock;
using Helpers.nof9;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Services;

namespace Cluster.MultiStep.Test
{
	[TestClass]
	public class MultiStepXATs : ClusterXAT<MultiStepTestDbContext>
	{
		#region Run settings

		protected override Type[] Types
		{
			get
			{
				return new Type[]
				{
					typeof(Step1),
					typeof(Step2),
					typeof(Step3)
				};
			}
		}

		protected override Type[] Services
		{
			get
			{
				return new Type[] 
				{
					typeof(SimpleRepository<Action1>),
					typeof(FixedClock) // TODO: new FixedClock(new DateTime(2000, 1, 1))
				};
			}
		}
		#endregion

		#region Test Methods

		[TestMethod, TestCategory("MultiStep")]
		public void InitiateActivity()
        {
            var a1 = GetTestService("Action1s").GetAction("New Instance").InvokeReturnObject().Save();

            a1.AssertIsPersistent();
            a1.GetPropertyByName("Step Number").AssertIsUnmodifiable().AssertValueIsEqual("0");

            var s1 = a1.GetAction("Continue").InvokeReturnObject().AssertIsType(typeof(Step1));

            var s2 = s1.GetAction("Next").InvokeReturnObject().AssertIsType(typeof(Step2));

            var s3 = s2.GetAction("Next").InvokeReturnObject();

			s3.AssertIsType(typeof(Step3));
		}
		#endregion
	}
}

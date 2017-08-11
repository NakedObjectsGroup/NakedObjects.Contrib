using System;
using Cluster.System.Mock;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;

namespace Cluster.MultiStep.Test
{
    [TestClass()]
    public class MultiStepXATs : ClusterXAT<MultiStepTestDbContext, MultiStepFixture>
    {
        #region Run configuration
        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new SimpleRepository<Action1>(),
                    new FixedClock(new DateTime(2000,1,1))
                    );
            }
        }
        #endregion

        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        #endregion


        [TestMethod]
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
    }
}

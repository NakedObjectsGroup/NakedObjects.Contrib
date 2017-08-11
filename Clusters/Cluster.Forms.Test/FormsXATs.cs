using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System;
using NakedObjects;
using Cluster.Forms.Impl;
using NakedObjects.Value;
using Cluster.Users.Api;
using Cluster.Forms.Api;
using Cluster.System.Mock;
using Helpers;

namespace Cluster.Forms.Test
{

    [TestClass()]
    public class FormsXATs : ClusterXAT<FormsTestDbContext, FormsFixture>
    {
        #region Run configuration
        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new FormRepository(), 
                    new SimpleRepository<FormDefinition>(),
                    new SimpleRepository<FormSubmission>(),
                    new SimpleRepository<FormSubmissionField>(),
                    new FormService(),
                    new FormServiceWrapper(),
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

        #region Form Repository
        [TestMethod]
        public void FormRepositoryActions()
        {
            var forms = GetTestService("Forms");
            Assert.AreEqual(4, forms.Actions.Count());

            var cnfd = forms.GetAction("Create New Form Definition");
           Assert.AreEqual(1, cnfd.Parameters.Count());
           Assert.AreEqual(1, cnfd.Parameters.Count());
           cnfd.Parameters[0].AssertIsMandatory().AssertIsNamed("Form Code");

           var ffdbc = forms.GetAction("Find Form Definition By Code");
           Assert.AreEqual(1, ffdbc.Parameters.Count());
           ffdbc.Parameters[0].AssertIsMandatory().AssertIsNamed("Form Code");

           var ffdbd = forms.GetAction("Find Form Definitions By Description");
           Assert.AreEqual(1, ffdbd.Parameters.Count());
           ffdbd.Parameters[0].AssertIsNamed("Matching").AssertIsMandatory();

           var rs = forms.GetAction("Recent Submissions");
           Assert.AreEqual(0, rs.Parameters.Count());
        }
        #endregion

        #region FormDefinitions
        [TestMethod]
        public void FormDefinitionPropertiesAndActions()
        {
            var fd = GetTestService("Form Definitions").GetAction("All Instances").InvokeReturnCollection().ElementAt(0);
            Assert.AreEqual(6, fd.Properties.Count());

            var fc = fd.GetPropertyByName("Form Code").AssertIsUnmodifiable();
            var desc = fd.GetPropertyByName("Description").AssertIsModifiable().AssertIsOptional();
            var form = fd.GetPropertyByName("Form").AssertIsUnmodifiable().AssertIsMandatory();
            var pp = fd.GetPropertyByName("Pre-populator").AssertIsModifiable().AssertIsOptional();
            pp.AssertFieldEntryIsValid("Cluster.Forms.Test.MockFormContentProvider");
            pp.AssertFieldEntryInvalid("Cluster.Forms.Test.xMockFormContentProvider")
                .AssertLastMessageContains("Class specified does not implement IFormContentProvider");

            var auto = fd.GetPropertyByName("Auto-processor").AssertIsModifiable().AssertIsOptional();
            auto.AssertFieldEntryIsValid("Cluster.Forms.Test.MockAutoProcessor");
            auto.AssertFieldEntryInvalid("Cluster.Forms.Test.xMockAutoProcessor")
                .AssertLastMessageContains("Class specified does not implement IFormAutoProcessor");

            var user = fd.GetPropertyByName("Upon Submission Generate Task For").AssertIsModifiable().AssertIsOptional();

            Assert.AreEqual(1, fd.Actions.Count());
            var act1 = fd.GetAction("Add Or Change Form");
            Assert.AreEqual(1, act1.Parameters.Count());
            var p1 =act1.Parameters[0].AssertIsNamed("New Attachment").AssertIsMandatory();
        }

        [TestMethod]
        public void CreateNewFormDefinition()
        {
            var action = GetTestService("Forms").GetAction("Create New Form Definition");   
            var formDef = action.InvokeReturnObject("x1");
            formDef.AssertIsType(typeof(FormDefinition)).AssertIsTransient();
            formDef.GetPropertyByName("Form Code").AssertTitleIsEqual("x1");
            formDef.GetPropertyByName("Description").AssertIsEmpty();
            formDef.AssertCanBeSaved();
            formDef.Save();
            formDef.AssertTitleEquals("x1");
        }

        [TestMethod]
        public void AttemptCreateNewFormDefinitionWithDuplicateCode()
        {
            var action = GetTestService("Forms").GetAction("Create New Form Definition");
            action.AssertIsInvalidWithParms("DB481").AssertLastMessageContains("A Form Definition for this code already exists");
            action.AssertIsValidWithParms("DB4812");
        }

        [TestMethod]
        public void FindFormDefinitionByCode()
        {
            var action = GetTestService("Forms").GetAction("Find Form Definition By Code");
            action.InvokeReturnObject("DB481").AssertTitleEquals("DB481");
            action.InvokeReturnObject("db481").AssertTitleEquals("DB481");
            action.InvokeReturnObject("Form2").AssertTitleEquals("Form2");
            Assert.IsNull(action.InvokeReturnObject("DC483"));
        }

        [TestMethod]
        public void FindFormsByDescription()
        {
            var action = GetTestService("Forms").GetAction("Find Form Definitions By Description");
            action.InvokeReturnCollection("xx").AssertCountIs(2);
            action.InvokeReturnCollection("xX").AssertCountIs(2);
            action.InvokeReturnCollection("x").AssertCountIs(3);
            action.InvokeReturnCollection("xxx").AssertCountIs(1);

        }

        #endregion

        #region Form Submissions

        [TestMethod]
        public void FormSubmissionPropertiesAndActions()
        {
            var fs = GetTestService("Form Submissions").GetAction("New Instance").InvokeReturnObject();
            Assert.AreEqual(5, fs.Properties.Count());
            fs.AssertIsImmutable().AssertIsType(typeof(FormSubmission));

            fs.GetPropertyByName("Form Code");
            fs.GetPropertyByName("Submitted");
            fs.GetPropertyByName("Form");
            var content = fs.GetPropertyByName("Form Fields").ContentAsCollection;
            fs.GetPropertyByName("Auto Processing");

            Assert.AreEqual(0, fs.Actions.Count());
        }

        [TestMethod]
        public void FormSubmissionFieldProperties()
        {
            var fsf = GetTestService("Form Submission Fields").GetAction("New Instance").InvokeReturnObject();
            Assert.AreEqual(3, fsf.Properties.Count());
            fsf.AssertIsImmutable().AssertIsType(typeof(FormSubmissionField));

            fsf.GetPropertyByName("Type");
            fsf.GetPropertyByName("Label");
            fsf.GetPropertyByName("Value");
        }

        [TestMethod]
        public void RecentSubmissions()
        {
            var results = GetTestService("Forms").GetAction("Recent Submissions").InvokeReturnPagedCollection(1);
        }


        #endregion

        #region FormService (tested via wrapper)
        [TestMethod]
        public void ProcessStreamAsPdfNoFormDefinition()
        {
            var processPdf = GetTestService("Form Service Wrapper").GetAction("Process Pdf Wrapper");
            var result = processPdf.InvokeReturnObject("Test1.pdf");
            result.AssertIsType(typeof(FormSubmission));
            var fields = result.GetPropertyByName("Form Fields").ContentAsCollection.AssertCountIs(3);
            var field0 = fields.ElementAt(0);
            field0.GetPropertyByName("Label").AssertValueIsEqual("Text2");
            field0.GetPropertyByName("Value").AssertValueIsEqual("Bar1");
        }

        [TestMethod]
        public void ProcessStreamAsPdfWithFormDefinitionAndAutoProcessSuccessfulButNoResult()
        {
            var processPdf = GetTestService("Form Service Wrapper").GetAction("Process Pdf Wrapper");
            var result = processPdf.InvokeReturnObject("Test2.pdf");
            result.AssertIsType(typeof(FormSubmission));
            var fields = result.GetPropertyByName("Form Fields").ContentAsCollection.AssertCountIs(2);
            var field0 = fields.ElementAt(0);
            field0.GetPropertyByName("Label").AssertValueIsEqual("FormCode");
            field0.GetPropertyByName("Value").AssertValueIsEqual("TestForm2");
            result.GetPropertyByName("Auto Processing").AssertValueIsEqual("Successful");
        }

        [TestMethod]
        public void ProcessStreamAsPdfWithFormDefinitionAndAutoProcessUnsuccessful()
        {
            var processPdf = GetTestService("Form Service Wrapper").GetAction("Process Pdf Wrapper");
            var result = processPdf.InvokeReturnObject("Test3.pdf");
            result.AssertIsType(typeof(FormSubmission));
            var fields = result.GetPropertyByName("Form Fields").ContentAsCollection.AssertCountIs(2);
            var field0 = fields.ElementAt(0);
            field0.GetPropertyByName("Label").AssertValueIsEqual("FormCode");
            field0.GetPropertyByName("Value").AssertValueIsEqual("TestForm3");
            result.GetPropertyByName("Auto Processing").AssertValueIsEqual("Failure Reason 3");
        }

        [TestMethod]
        public void ProcessStreamAsPdfWithFormDefinitionAndAutoProcessSuccessfulReturnsObject()
        {
            var processPdf = GetTestService("Form Service Wrapper").GetAction("Process Pdf Wrapper");
            var result = processPdf.InvokeReturnObject("Test4.pdf");
            result.AssertIsType(typeof(int)).AssertTitleEquals("4");
        }
        #endregion

    }

    public class MockAutoProcessor : IFormAutoProcessor
    {

        public void Process(IFormSubmission formSubmission, out bool successful, out string message, out object result)
        {
            result = null;
            successful = true;
            message = null;
        }
    }

    public class MockAutoProcessorFails : IFormAutoProcessor
    {

        public void Process(IFormSubmission formSubmission, out bool successful, out string message, out object result)
        {
            result = null;
            successful = false;
            message = "Failure Reason 3";
        }
    }

    public class MockAutoProcessorReturnsResult : IFormAutoProcessor
    {

        public void Process(IFormSubmission formSubmission, out bool successful, out string message, out object result)
        {
            result = 4;
            successful = true;
            message = null;
        }
    }
    public class MockFormContentProvider : IFormContentProvider {

        public void PopulateTextField(string fieldName, out string value, out string display)
        {
            throw new NotImplementedException();
        }

        public void PopulateListField(string fieldName, out string[] exportValues, out string[] displayValues, out string[] selected)
        {
            throw new NotImplementedException();
        }
    }
}
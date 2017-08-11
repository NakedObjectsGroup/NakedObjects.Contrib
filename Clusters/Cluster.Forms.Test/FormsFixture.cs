using NakedObjects;
using System;
using System.Data.Entity;
using System.IO;
using Cluster.Forms.Impl;
using Cluster.Tasks.Api;
using Cluster.Users.Api;
using Cluster.Forms.Api;
using System.Reflection;
using System.Text;
using NakedObjects.Core.Context;

namespace Cluster.Forms.Test
{
    public class FormsFixture
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public void Install()
        {
            NewFormDefinition("DB481", "Government");
            NewFormDefinition("Form2", "xxx");
            NewFormDefinition("Form3", "xxy");
            NewFormDefinition("Form4", "xyy");
            NewFormDefinition("Form5");
            NewFormDefinition("TestForm2",null, typeof(MockAutoProcessor));
            NewFormDefinition("TestForm3", null, typeof(MockAutoProcessorFails));
            NewFormDefinition("TestForm4", null, typeof(MockAutoProcessorReturnsResult));

            var abc1 = NewFormSubmission("ABC1", "TestPDF.pdf");
            abc1.AddFormField(FieldTypes.Text, "FirstName", "Richard");
            abc1.AddFormField(FieldTypes.Text, "LastName", "Pawson");
            Container.Persist(ref abc1);
        }

        public FormDefinition NewFormDefinition(string code, string description = null, Type autoProcess = null)
        {
            var fd = Container.NewTransientInstance<FormDefinition>();
            fd.FormCode = code;
            fd.Description = description;
            if (autoProcess != null)
            {
                fd.AutoProcessor = autoProcess.FullName;
            }
            Container.Persist(ref fd);
            return fd;
        }

        public FormSubmission NewFormSubmission(string code, string fileNameOfPdf, string autoProcessMessage = null)
        {
            var fs = Container.NewTransientInstance<FormSubmission>();
            fs.FormCode = code;
            fs.AutoProcessing = autoProcessMessage;
            fs.FormAsBytes = ReadPdfAsManifestResource(fileNameOfPdf);
            return fs;
        }

        internal const string pdfsResourceNamespace = "Cluster.Forms.Test.Pdfs.";

        internal static byte[] ReadPdfAsManifestResource(string resourceName, string resourceNamspace = pdfsResourceNamespace)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(pdfsResourceNamespace + resourceName);
            if (stream == null)
            {
                return new byte[] { };
            }
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }


    }
}


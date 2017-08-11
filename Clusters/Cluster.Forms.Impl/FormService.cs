
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cluster.Users.Api;
using Cluster.Tasks.Api;
using NakedObjects;
using NakedObjects.Services;
using NakedObjects.Value;
using Cluster.System.Api;
using iTextSharp.text.pdf;
using System.Collections;
using Cluster.Forms.Api;
using iTextSharp.text;

namespace Cluster.Forms.Impl
{
    public class FormService : IFormService
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public ITaskService TaskService { set; protected get; }

        public IClock Clock { set; protected get; }

        public FormRepository FormRepository { set; protected get; }
        #endregion

        #region ProcessPdf
        [NakedObjectsIgnore]
        public object ProcessPdf(byte[] pdf)
        {

            var pr = new PdfReader(pdf);
            return ProcessPdf(pr);
        }
        [NakedObjectsIgnore]
        public object ProcessPdf(Stream pdf)
        {

            var pr = new PdfReader(pdf);
            return ProcessPdf(pr);
        }
        [NakedObjectsIgnore]
        private object ProcessPdf(PdfReader pr)
        {

            AcroFields flds = pr.AcroFields;
            FormSubmission formSub = Container.NewTransientInstance<FormSubmission>();
            foreach (DictionaryEntry entry in flds.Fields)
            {
                string fieldName = entry.Key.ToString();
                FieldTypes type = (FieldTypes)flds.GetFieldType(fieldName);
                string content = flds.GetField(fieldName);
                formSub.AddFormField(type, fieldName, content);
            }

            //Following code will write the .pdf as a byte array
            var ims = new MemoryStream();
            var ps = new PdfStamper(pr, ims);
            ps.Close();
            formSub.FormAsBytes = ims.ToArray();

            //TODO:  Guard against 1) the FormCode field not existing 2) There being no FormDefinition for that code

            string formCode = flds.GetField(AppSettings.FieldLabelForFormCode());
            formSub.FormCode = formCode;
            FormDefinition formDef = FormRepository.FindFormDefinitionByCode(formCode);

            Container.Persist(ref formSub);

            //GenerateTaskIfApplicable(formSub, formDef);
            object result = null;
            if (formDef != null)
            {
                result = AutoProcessIfApplicable(formSub, formDef);  //TODO:  Or maybe form submission? Or some standard thank you acknowledgement?
            }
            return result ?? formSub;
        }

        private void GenerateTaskIfApplicable(FormSubmission formSub, FormDefinition formDef)
        {
            IUser user = formDef.UponSubmissionGenerateTaskFor;
            if (user != null)
            {
                //TODO:  Should be assigned to specified user, not to 'me'
                throw new NotImplementedException();
                //var task = TaskService.CreateNewTask(Cluster.Forms.Api.Constants.NewFormSubmission, true, null, formSub);
                //Container.Persist(ref task);
            }
        }

        private object AutoProcessIfApplicable(FormSubmission formSub, FormDefinition formDef)
        {
            string autoProc = formDef.AutoProcessor;
            if (autoProc != null)
            {
                var type = formDef.AutoProcessorType();
                var processor = (IFormAutoProcessor)Container.NewTransientInstance(type);
                bool successful = false;
                string message = null;
                object result = null;
                processor.Process(formSub, out successful, out message, out result);
                if (successful)
                {
                    formSub.AutoProcessing = "Successful";
                    return result;
                }
                else
                {
                    formSub.AutoProcessing = message;
                    return null;
                }
            }
            return null;
        }
        #endregion

        #region GenerateForm

        public FileAttachment GenerateForm(string formCode)
        {
            string fileName;
            byte[] bytes;
            GeneratePrePopulatedForm(formCode, out fileName, out bytes);
           // bytes = AddSubmitButton(bytes);
            return new FileAttachment(bytes, fileName, FormDefinition.pdfMimeType) { DispositionType = "inline" };
        }

        private void GeneratePrePopulatedForm(string formCode, out string fileName, out byte[] bytes)
        {
            var formDef = FormRepository.FindFormDefinitionByCode(formCode);
            if (formDef == null)
            {
                throw new DomainException("No Form Definition exists for code: " + formCode);
            }
            fileName = formDef.FileName;
            bytes = formDef.FormContent;
            if (bytes == null)
            {
                throw new DomainException("Form Definition has no file content");
            }
            if (formDef.PrePopulator != null)
            {
                IFormContentProvider prepop = (IFormContentProvider)Container.NewTransientInstance(formDef.PrePopulatorType());
                bytes = SetFields(bytes, prepop);
            }
        }

        public FileAttachment GenerateCustomPrepopulatedForm(string formCode, IFormContentProvider prePop)
        {
            string fileName;
            byte[] bytes;
            GeneratePrePopulatedForm(formCode, out fileName, out bytes);
            bytes = SetFields(bytes, prePop);
            return new FileAttachment(bytes, fileName, FormDefinition.pdfMimeType) { DispositionType = "inline" };
        }

        public static byte[] SetFields(byte[] pdfFile, IFormContentProvider contentProvider)
        {
            var pr = new PdfReader(pdfFile);
            var ims = new MemoryStream();
            var ps = new PdfStamper(pr, ims);
            AcroFields flds = ps.AcroFields;

            foreach (DictionaryEntry entry in flds.Fields)
            {
                string fieldName = entry.Key.ToString();
                var type = (FieldTypes)flds.GetFieldType(fieldName);
                switch (type)
                {
                    case FieldTypes.Text:
                        string value = null;
                        string display = null;
                        contentProvider.PopulateTextField(fieldName, out value, out display);
                        if (value != null)
                        {
                            flds.SetField(fieldName, value);
                        }
                        break;
                    case FieldTypes.List:
                        string[] exportValues = null;
                        string[] displayValues = null;
                        string[] selected = null;
                        contentProvider.PopulateListField(fieldName, out exportValues, out displayValues, out selected);
                        if (exportValues != null)
                        {
                            flds.SetListOption(fieldName, exportValues, displayValues);
                        }
                        if (selected != null)
                        {
                            flds.SetListSelection(fieldName, selected);                            
                        }
                        break;
                    default:
                        break;
                }
            }
             ps.Close();
            return ims.ToArray();
        }

        #endregion

        private static byte[] AddSubmitButton(byte[] pdfFile)
        {
            var pr = new PdfReader(pdfFile);
                var ims = new MemoryStream();
                var ps = new PdfStamper(pr, ims);

            string url = AppSettings.URLForFormSubmission();

            PushbuttonField button2 = new PushbuttonField(ps.Writer, new Rectangle(100, 1000, 150, 1030), "Submit");
            button2.BackgroundColor = new GrayColor(0.7f);
            button2.Text = "Submit";
            button2.Visibility = PushbuttonField.VISIBLE_BUT_DOES_NOT_PRINT;

            PdfFormField submit2 = button2.Field;
            submit2.Action = PdfAction.CreateSubmitForm(url, null, PdfAction.SUBMIT_PDF);
            ps.AddAnnotation(submit2, 1);

            ps.Close();
            return ims.ToArray();
        }

        public IFormSubmission FindFormSubmissionById(int id)
        {
            return Container.Instances<FormSubmission>().Single(x => x.Id == id);
        }
    }
}

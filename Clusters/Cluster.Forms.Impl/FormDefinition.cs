using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;
using NakedObjects.Value;
using System.Linq;
using Cluster.Users.Api;
using System.ComponentModel.DataAnnotations;
using Cluster.Tasks.Api;
using Cluster.Forms.Api;
using System;
using NakedObjects.Util;
using System.ComponentModel;

namespace Cluster.Forms.Impl
{
    [Bounded]  //TODO:  Not sure if this is justified if number of forms gets large
    public class FormDefinition
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public IUserService UserService { set; protected get; }
        #endregion

        internal const string pdfMimeType = "application/pdf";
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(FormCode);
            return t.ToString();
        }

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        #region Form Code
        [MemberOrder(10), Disabled]
        public virtual string FormCode { get; set; }
        #endregion

        [MemberOrder(20), Optionally]
        public virtual string Description { get; set; }

        #region Form
        [MemberOrder(30)]
        public virtual FileAttachment Form
        {
            get
            {
                if (FormContent == null) return null;
                return new FileAttachment(FormContent, FileName, pdfMimeType) { DispositionType = "inline" };
            }
        }

        public void AddOrChangeForm(FileAttachment newAttachment)
        {
            FormContent = newAttachment.GetResourceAsByteArray();
            FileName = newAttachment.Name;
        }

        public string ValidateAddOrChangeForm(FileAttachment form)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(form.MimeType != pdfMimeType, "File is not a PDF");
            return rb.Reason;
        }

        [NakedObjectsIgnore]
        public virtual string FileName { get; set; }

        [NakedObjectsIgnore]
        public virtual byte[] FormContent { get; set; }

        #endregion

        #region Pre-population
        /// <summary>
        /// Fully-qualified name of an implementation of IFormSubmissionProcessor that
        /// can handle the submission.  (Its 'Process' method will be called.
        /// </summary>
        [MemberOrder(40), DisplayName("Pre-populator"), Optionally, DescribedAs("Fully-qualified name of a service that can pre-populate this type of form automatically")]
        public virtual string PrePopulator { get; set; }

        internal Type PrePopulatorType()
        {
            return TypeUtils.GetType(PrePopulator);
        }
        public string ValidatePrePopulator(string classToInvoke)
        {
            Type t = TypeUtils.GetType(classToInvoke);
            if (t == null || !typeof(IFormContentProvider).IsAssignableFrom(t))
            {
                return "Class specified does not implement IFormContentProvider";
            }
            return null;
        }

        #endregion

        #region Auto-processing upon submission
        /// <summary>
        /// Fully-qualified name of an implementation of IFormSubmissionProcessor that
        /// can handle the submission.  (Its 'Process' method will be called.
        /// </summary>
        [MemberOrder(40), DisplayName("Auto-processor"),  Optionally, DescribedAs("Fully-qualified name of a service that can process this type of form automatically")]
        public virtual string AutoProcessor { get; set; }

        internal Type AutoProcessorType()
        {
            return TypeUtils.GetType(AutoProcessor);
        }
        public string ValidateAutoProcessor(string classToInvoke)
        {
            Type t = TypeUtils.GetType(classToInvoke);
            if (t == null || !typeof(IFormAutoProcessor).IsAssignableFrom(t))
            {
                return "Class specified does not implement IFormAutoProcessor";
            }
            return null;
        }

        #endregion

        #region Task Generation upon submission

        #region UponSubmissionGenerateTaskFor Property of type IUser ('Result' interface)

        [NakedObjectsIgnore]
        public virtual int UponSubmissionGenerateTaskForId { get; set; }


        private IUser _UponSubmissionGenerateTaskFor;

        [NotPersisted(), NotMapped, Optionally]
        public IUser UponSubmissionGenerateTaskFor
        {
            get
            {
                if (_UponSubmissionGenerateTaskFor == null && UponSubmissionGenerateTaskForId > 0)
                {
                    _UponSubmissionGenerateTaskFor = UserService.FindUserById(UponSubmissionGenerateTaskForId);
                }
                return _UponSubmissionGenerateTaskFor;
            }
            set
            {
                _UponSubmissionGenerateTaskFor = value;
                if (value == null)
                {
                    UponSubmissionGenerateTaskForId = 0;
                }
                else
                {
                    UponSubmissionGenerateTaskForId = value.Id;
                }
            }
        }

        #endregion
        public IQueryable<IUser> AutoCompleteUponSubmissionGenerateTaskFor([MinLength(3)] string nameMatch)
        {
            return UserService.FindUsersByRealOrUserName(nameMatch);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cluster.Forms.Api;
using Cluster.System.Api;
using Cluster.Tasks.Api;
using NakedObjects;
using NakedObjects.Value;

namespace Cluster.Forms.Impl
{
    /// <summary>
    /// An instance of FormSubmission is created each time a .pdf form is successfully
    /// submitted, via the appropriate controller method.  
    /// </summary>
    [Immutable]
    public class FormSubmission : ITaskContext, IFormSubmission
    {
        #region Injected Services
        public IClock Clock { set; protected get; }
        
        public IDomainObjectContainer Container { set; protected get; }
        #endregion
        #region LifeCycle Methods

        public void Persisting()
        {
            Submitted = Clock.Now();
        }
        #endregion

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(FormCode).Append(" submitted",Submitted);
            return t.ToString();
        }
      
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(5)]
        public virtual string FormCode { get; set; }
          
        [Disabled, MemberOrder(10)]
        public virtual DateTime Submitted { get; set; }

        [MemberOrder(40)]
        public virtual FileAttachment Form
        {
            get
            {
                if (FormAsBytes == null) return null;
                return new FileAttachment(FormAsBytes, "Form As Submitted", @"application/pdf") { DispositionType = "inline" };
            }
        }

        [NakedObjectsIgnore]
        public virtual byte[] FormAsBytes { get; set; }

        #region FormFields (collection)
        private ICollection<FormSubmissionField> _formFields = new List<FormSubmissionField>();

        [MemberOrder(30)]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false, "Type", "Label", "Value")]
        public virtual ICollection<FormSubmissionField> FormFields
        {
            get
            {
                return _formFields;
            }
            set
            {
                _formFields = value;
            }
        }

        [NakedObjectsIgnore]
        public void AddFormField(FieldTypes type, string fieldName, string content)
        {
            var fieldToSave = Container.NewTransientInstance<FormSubmissionField>();
            fieldToSave.Type = type;
            fieldToSave.Label = fieldName;
            fieldToSave.Value = content;
            _formFields.Add(fieldToSave);
        }
        #endregion

        [NakedObjectsIgnore]
        public string Value(string fieldName)
        {
            return FormFields.Where(x => x.Label.ToUpper() == fieldName.ToUpper()).Select(x => x.Value).SingleOrDefault();
        }

        [Disabled, MemberOrder(100)]
        public virtual string AutoProcessing { get; set; }



    }
}

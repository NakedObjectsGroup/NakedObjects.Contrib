using System;
using System.ComponentModel;
using System.Linq;
using Cluster.Users.Api;
using Cluster.Documents.Api;
using NakedObjects;
using NakedObjects.Services;
using NakedObjects.Value;

namespace Cluster.Forms.Impl
{
    [DisplayName("Forms")]
    public class FormRepository
    {
        #region Injected Service
        public FormService FormService { set; protected get; }

        public IUserService UserFinder { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region CreateNewFormDefinition
        public FormDefinition CreateNewFormDefinition(string formCode)
        {
            var fd = Container.NewTransientInstance<FormDefinition>();
            fd.FormCode = formCode;
            return fd;
        }

        public string ValidateCreateNewFormDefinition(string formCode)
        {
            var rb = new ReasonBuilder();
            var count = Container.Instances<FormDefinition>().Count(x => x.FormCode.ToUpper() == formCode.ToUpper().Trim());
            rb.AppendOnCondition(count > 0, "A Form Definition for this code already exists");
            return rb.Reason;
        }
        #endregion

        public FormDefinition FindFormDefinitionByCode(string formCode)
      {
      var query = from obj in Container.Instances<FormDefinition>()
		              where obj.FormCode.ToUpper() == formCode.ToUpper()
		              select obj;
                  
      return query.SingleOrDefault();  
      }

        public IQueryable<FormDefinition> FindFormDefinitionsByDescription(string matching)
      {
          return Container.Instances<FormDefinition>().Where(x => x.Description.ToUpper().Contains(matching.ToUpper()));
      }
      
      public IQueryable<FormSubmission> RecentSubmissions()
      {
          return Container.Instances<FormSubmission>().OrderByDescending(x => x.Submitted);
      }      
    }
}

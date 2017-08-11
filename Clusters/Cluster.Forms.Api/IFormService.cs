using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Value;

namespace Cluster.Forms.Api
{
    public interface IFormService
    {
                /// <summary>
        /// Returns a new .pdf as a FileAttachment, to be returned to user.
        /// If the FormDefinition defines a autoprepopulator this will be called;
        /// otherwise the form will be blank.
        /// </summary>
        /// <param name="formCode"></param>
        /// <returns></returns>
         FileAttachment GenerateForm(string formCode);

        /// <summary>
        /// This will first apply any generic pre-population (as if calling GenerateForm) and then pre-populate
        /// using the specified IFormContentProvider which may add-to, or override, the generic pre-population.
        /// </summary>
        /// <param name="formCode"></param>
        /// <param name="prePopulatedBy"></param>
        /// <returns></returns>
         FileAttachment GenerateCustomPrepopulatedForm(string formCode, IFormContentProvider prePopulatedBy);

         IFormSubmission FindFormSubmissionById(int id);
    }
}

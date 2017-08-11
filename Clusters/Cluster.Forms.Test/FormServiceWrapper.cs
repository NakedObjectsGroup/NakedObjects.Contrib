using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Forms.Impl;

namespace Cluster.Forms.Test
{
    /// <summary>
    /// Purpose of this class is to permit testing of the FormService methods, by (minimally) wrapping
    /// them in methods that use only types that can be handled by XAT.
    /// </summary>
    public class FormServiceWrapper
    {
        #region InjectedServices
        public FormService FormService { set; protected get; }
        #endregion

        public object ProcessPdfWrapper(string fileNameOfPdf)
        {
            byte[] pdf = FormsFixture.ReadPdfAsManifestResource(fileNameOfPdf);
            return FormService.ProcessPdf(pdf);
        }
    }
}

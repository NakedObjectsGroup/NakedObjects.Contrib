using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Forms.Impl;
using NakedObjects.Value;

namespace Cluster.Forms.Test
{
    /// <summary>
    /// Simple service to provide mechanism for kicking off various Forms
    /// </summary>
    public class MockFormInitiator
    {
        #region Injected Services
        public FormService FormService { set; protected get; }

        #endregion

        public FileAttachment CreateIBC1()
        {
            return FormService.GenerateForm("IBC1");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Value;

namespace Cluster.Documents.Impl
{
    public class DocumentWithFileAttachment : Document
    {
        #region Injected Services
        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service; 'injc' to add an injected Container

        #endregion
        #region Life Cycle Methods
        // This region should contain any of the 'life cycle' convention methods (such as
        // Created(), Persisted() etc) called by the framework at specified stages in the lifecycle.


        #endregion

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append("File Attachment");
            return t.ToString();
        }

        #region Properties
        [MemberOrder(20)]
        public virtual FileAttachment Attachment
        {
            get
            {
                if (AttContent == null) return null;
                return new FileAttachment(AttContent, AttName, AttMime) { DispositionType = "inline" };
            }
        }

        [DisplayName("Description"), Optionally, MemberOrder(30)]
        public override string Text { get; set; }

        [Hidden]
        public virtual byte[] AttContent { get; set; }

        [Hidden]
        public virtual string AttName { get; set; }

        [Hidden]
        public virtual string AttMime { get; set; }

        #endregion

    }
}


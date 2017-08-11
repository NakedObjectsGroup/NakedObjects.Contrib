using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Documents.Api;
using NakedObjects;

namespace Cluster.Documents.Impl
{
    /// <summary>
    /// Document that holds a polymorphic reference to an object (in another cluster) that implements IExternalDocument
    /// </summary>
    public class ExternalDocument : Document
    {
        #region LifeCycle methods 

        
        public override void Persisting()
        {
            base.Persisting();
            ContentLink = PolymorphicNavigator.NewTransientLink<ExternalDocumentContentLink, IExternalDocument, ExternalDocument>(_Content, this);
        }
        #endregion

        #region Content Property of type IExternalDocument ('role' interface)

        [Hidden]
        public virtual ExternalDocumentContentLink ContentLink { get; set; }

        private IExternalDocument _Content;

        [NotPersisted]
        public IExternalDocument Content
        {
            get
            {
                if (_Content == null)
                {
                    _Content = PolymorphicNavigator.RoleObjectFromLink(ref _Content, ContentLink, this);
                }
                return _Content;
            }
            set
            {
                _Content = value;
            }
        }

        //Called automatically by framework if the user modifies the value.
        public void ModifyContent(IExternalDocument value) {
            _Content = value;
            if (Container.IsPersistent(this)) {
                ContentLink = PolymorphicNavigator.UpdateAddOrDeleteLink(_Content, ContentLink, this);
            }
        }

        #endregion

    }
}

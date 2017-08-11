using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Cluster.Documents.Api;
using Cluster.System.Api;
using Cluster.Users.Api;
using NakedObjects;
using NakedObjects.Services;

namespace Cluster.Documents.Impl
{
    public abstract class Document : IDocument, IUpdateableEntity
    {
        #region Injected Services
        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }

        public IUserService UserFinder { set; protected get; }

        public IClock Clock { set; protected get; }

        #endregion

        #region LifeCycle Methods

        public virtual void Persisting()
        {
            User = UserFinder.CurrentUser();
            LastModified = Clock.Now();
        }

        public virtual void Updating()
        {
            if (Container.IsPersistent(this)) { //To guard against being called after Persisted()
            User = UserFinder.CurrentUser();
            LastModified = Clock.Now();
            }
        }


        [NotPersisted, NotMapped, Hidden]
        public IDocumentHolder CreatingHolder { get; set; }

        public void Persisted()
        {
           AddHolder(CreatingHolder);
        }
        #endregion





        #region Properties
        [Hidden]
        public virtual int Id { get; set; }

        [Disabled, MemberOrder(10)]
        public virtual DocumentType Type { get; set; }


        public virtual DocumentType DefaultType()
        {
            return DocumentType.Unknown;
        }


        /// <summary>
        /// Note that all documents may have descriptive text associated with them
        /// </summary>
        [MemberOrder(100)]
        public virtual string Text { get; set; }

        /// <summary>
        /// Does not disable Test;  implemented only to allow override in sub-classes
        /// </summary>
        /// <returns>Null</returns>
        public virtual string DisableText()
        {
            return null;
        }


        #region User Property of type IUser ('Result' interface)

        [Hidden()]
        public virtual int UserId { get; set; }


        private IUser _User;

        /// <summary>
        /// The user that created, or last modified the document
        /// </summary>
        [Disabled, Hidden(WhenTo.UntilPersisted), MemberOrder(210), NotPersisted()]
        public IUser User
        {
            get
            {
                if (_User == null && UserId > 0)
                {
                    _User = UserFinder.FindUserById(UserId);
                }
                return _User;
            }
            set
            {
                _User = value;
                if (value == null)
                {
                    UserId = 0;
                }
                else
                {
                    UserId = value.Id;
                }
            }

        }
        #endregion

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }

        #endregion



        #region Holders Collection of type IDocumentHolder

        private ICollection<DocumentHolderLink> _Holder = new List<DocumentHolderLink>();

        [Hidden]
        public virtual ICollection<DocumentHolderLink> HolderLinks
        {
            get
            {
                return _Holder;
            }
            set
            {
                _Holder = value;
            }
        }

        [Hidden]
        public void AddHolder(IDocumentHolder value)
        {
            PolymorphicNavigator.AddLink<DocumentHolderLink, IDocumentHolder, Document>(value, this);
        }

        [Hidden]
        public void RemoveHolder(IDocumentHolder value)
        {
            PolymorphicNavigator.RemoveLink<DocumentHolderLink, IDocumentHolder, Document>(value, this);
        }
        #endregion


        #region Actions
        [MemberOrder(100)]
        public void MoveTo(IDocumentHolder holder)
        {
            throw new NotImplementedException();
        }

        [MemberOrder(110)]
        public void CopyTo(IDocumentHolder holder)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

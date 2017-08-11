using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Cluster.System.Api;
using NakedObjects;
using NakedObjects.Services;

namespace Cluster.Audit.Impl
{
     [Immutable(WhenTo.OncePersisted)]
    public abstract class ObjectAuditedEvent : AuditedEvent
    {
        #region Injected Services
        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        #region LifeCycle methods
        public void Persisting()
        {
            TargetObjectLink = PolymorphicNavigator.NewTransientLink<ObjectAuditedEventTargetObjectLink, IDomainInterface, ObjectAuditedEvent>(_TargetObject, this);
        }
        #endregion

        #region TargetObject Property of type IAuditedObject ('role' interface)

        [Hidden]
        public virtual ObjectAuditedEventTargetObjectLink TargetObjectLink { get; set; }

        private IDomainInterface _TargetObject;

        [NotPersisted, NotMapped, MemberOrder(30)]
        public IDomainInterface Object
        {
            get
            {
                if (_TargetObject == null)
                {
                    _TargetObject = PolymorphicNavigator.RoleObjectFromLink(ref _TargetObject, TargetObjectLink, this);
                }
                return _TargetObject;
            }
            set
            {
                _TargetObject = value;
            }
        }
        #endregion
               
    }
}

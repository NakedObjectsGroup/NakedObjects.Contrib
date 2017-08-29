using System.ComponentModel.DataAnnotations.Schema;
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
            TargetObjectLink = PolymorphicNavigator.NewTransientLink<ObjectAuditedEventTargetObjectLink, IDomainInterface, ObjectAuditedEvent>(_targetObject, this);
        }
        #endregion

        #region TargetObject Property of type IAuditedObject ('role' interface)

        [Hidden(WhenTo.Always)]
        public virtual ObjectAuditedEventTargetObjectLink TargetObjectLink { get; set; }

        private IDomainInterface _targetObject;

        [NotPersisted, NotMapped, MemberOrder(30)]
        public IDomainInterface Object
        {
            get
            {
                if (_targetObject == null)
                {
                    _targetObject = PolymorphicNavigator.RoleObjectFromLink(ref _targetObject, TargetObjectLink, this);
                }
                return _targetObject;
            }
            set
            {
                _targetObject = value;
            }
        }
        #endregion
               
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Audit.Impl
{
    public class ObjectAuditedEventTargetObjectLink : PolymorphicLink<IDomainInterface, ObjectAuditedEvent>
    {
    }
}

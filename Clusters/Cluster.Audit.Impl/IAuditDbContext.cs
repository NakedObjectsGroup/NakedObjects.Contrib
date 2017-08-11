using System.Data.Entity;

namespace Cluster.Audit.Impl
{
    public interface IAuditDbContext
    {
         DbSet<AuditedEvent> AuditedEvents { get; set; }
         DbSet<ObjectAuditedEventTargetObjectLink> ObjectAuditedEventTargetObjectLinks { get; set; }
    }
}

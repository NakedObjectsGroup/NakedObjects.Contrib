using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Cluster.Emails.Impl;

namespace Cluster.Emails.Impl.Mapping
{
    internal class EmailMap : EntityTypeConfiguration<Email>
    {
        internal EmailMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Emails", "Emails");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.To).HasColumnName("To");
            this.Property(t => t.CC).HasColumnName("CC");
            this.Property(t => t.Bcc).HasColumnName("Bcc");
            this.Property(t => t.ReplyToList).HasColumnName("ReplyToList");
            this.Property(t => t.IsBodyHtml).HasColumnName("IsBodyHtml");
            this.Property(t => t.From).HasColumnName("From");
            this.Property(t => t.Sender).HasColumnName("Sender");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.Body).HasColumnName("Body");
            this.Property(t => t.DeliveryNotificationOptions).HasColumnName("DeliveryNotificationOptions");
            this.Property(t => t.Priority).HasColumnName("Priority");
        }
    }
}

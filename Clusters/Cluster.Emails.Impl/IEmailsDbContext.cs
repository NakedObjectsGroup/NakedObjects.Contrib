using System.Data.Entity;

namespace Cluster.Emails.Impl
{
    /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
    public interface IEmailsDbContext
    {
        DbSet<EmailDetails> EmailDetails { get; set; }

        DbSet<Email> Emails { get; set; }

        DbSet<EmailAttachment> EmailAttachments { get; set; }
    }
}

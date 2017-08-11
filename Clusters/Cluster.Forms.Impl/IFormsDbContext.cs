using System.Data.Entity;
namespace Cluster.Forms.Impl
{
        /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
    public interface IFormsDbContext
    {
        DbSet<FormDefinition> FormDefinitions { get; set; }
        DbSet<FormSubmission> FormSubmissions { get; set; }
        DbSet<FormSubmissionField> FormFields { get; set; }
     }
}

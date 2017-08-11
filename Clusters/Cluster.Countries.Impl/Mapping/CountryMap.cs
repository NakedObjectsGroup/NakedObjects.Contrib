using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Cluster.Countries.Impl;

namespace Cluster.Countries.Impl.Mapping
{
    internal class CountryMap : EntityTypeConfiguration<Country>
    {
        internal CountryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Countries", "Countries");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ISOCode).HasColumnName("ISOCode");
        }
    }
}

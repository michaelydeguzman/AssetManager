using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAM.Persistence.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(e => e.Id );
            builder.ToTable("Countries");
            builder.HasMany(j => j.Regions).WithOne(m => m.Country).IsRequired(false);
            builder.HasData(
                    new Country() { Id = 1, Name = "Australia", Description = "All states and territories"},
                    new Country() { Id = 2, Name = "UK", Description = "Excludes Ireland and Wales" }
                );
        }
    }
}

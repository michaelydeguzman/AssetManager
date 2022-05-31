using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAM.Persistence.Configurations
{
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasKey(e => new { e.Id });
            builder.ToTable("Regions");
            //builder.HasMany(j => j.Jobs).WithOne(m => m.Region).IsRequired(false);
            builder.HasOne(m => m.Country).WithMany(j => j.Regions).HasForeignKey(j => j.CountryId).IsRequired(false);
            //builder.HasMany(m => m.TemplateRegions).WithOne(j => j.Region).HasForeignKey(j => j.RegionId);
            builder.HasData(
                    new Region() { Id = 1, CountryId = 1, Description = "ACT" },
                    new Region() { Id = 2, CountryId = 1, Description = "Queensland" },
                    new Region() { Id = 3, CountryId = 1, Description = "New South Wales" },
                    new Region() { Id = 4, CountryId = 1, Description = "Northern Territory" },
                    new Region() { Id = 5, CountryId = 1, Description = "South Australia" },
                    new Region() { Id = 6, CountryId = 1, Description = "Tasmania" },
                    new Region() { Id = 7, CountryId = 1, Description = "Western Australia" },
                    new Region() { Id = 8, CountryId = 1, Description = "Victoria" },
                    new Region() { Id = 9, CountryId = 2, Description = "East of England" },
                    new Region() { Id = 10, CountryId = 2, Description = "Greater London" },
                    new Region() { Id = 11, CountryId = 2, Description = "Midlands" },
                    new Region() { Id = 12, CountryId = 2, Description = "North East" },
                    new Region() { Id = 13, CountryId = 2, Description = "North West" },
                    new Region() { Id = 14, CountryId = 2, Description = "Northern Ireland" },
                    new Region() { Id = 15, CountryId = 2, Description = "Scotland" },
                    new Region() { Id = 16, CountryId = 2, Description = "South East" },
                    new Region() { Id = 17, CountryId = 2, Description = "South West" },
                    new Region() { Id = 18, CountryId = 2, Description = "Wales" },
                    new Region() { Id = 19, CountryId = 2, Description = "Yorshire" }
                );
        }
    }
}

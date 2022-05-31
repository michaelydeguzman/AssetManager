using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Persistence.Configurations
{
    public class ThemeConfiguration : IEntityTypeConfiguration<Theme>
    {
        public void Configure(EntityTypeBuilder<Theme> builder)
        {
            builder.HasKey(e => new { e.Id });
            builder.ToTable("Themes");
            builder.HasData(
               new Theme() { Id = 1, Name = "Simple Asset Manager", PrimaryColor = "246,57,21", SecondaryColor="150,100,30", TertiaryColor = "246,20,41", IsApplied = true, Deleted = false, LogoFileName= "logo-simple", LogoUrl= "https://damblob1.blob.core.windows.net/logocontainer-dev-mikey/logo-simple.png?sp=r&st=2021-11-11T04:38:10Z&se=2022-11-11T12:38:10Z&spr=https&sv=2020-08-04&sr=b&sig=P60UQSs6gi2%2BobEvkcZp%2BoY5DRCbqQnOsYI%2Fse0VVmY%3D" },
               new Theme() { Id = 2, Name = "Simple Asset Manager Light", PrimaryColor = "201.29032258064515,72,91", SecondaryColor = "245.99999999999997,56,20", TertiaryColor = "27.835051546391764,0,0", IsApplied = false, Deleted = true, LogoFileName = "logo-simple", LogoUrl = "https://damblob1.blob.core.windows.net/logocontainer-dev-mikey/logo-simple.png?sp=r&st=2021-11-11T04:38:10Z&se=2022-11-11T12:38:10Z&spr=https&sv=2020-08-04&sr=b&sig=P60UQSs6gi2%2BobEvkcZp%2BoY5DRCbqQnOsYI%2Fse0VVmY%3D" }
           );
        }
    }
}

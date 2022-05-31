using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Persistence.Configurations
{
    public class LogoConfiguration : IEntityTypeConfiguration<Logo>
    {
        public void Configure(EntityTypeBuilder<Logo> builder)
        {
            builder.HasKey(e => new { e.Id });
            builder.ToTable("Logos");
            builder.HasData(
               new Logo () { Id = 1, FileName = "logo-simple.png", LogoUrl = "https://damblob1.blob.core.windows.net/logocontainer-dev-mikey/logo-simple.png?sp=r&st=2021-11-11T04:38:10Z&se=2022-11-11T12:38:10Z&spr=https&sv=2020-08-04&sr=b&sig=P60UQSs6gi2%2BobEvkcZp%2BoY5DRCbqQnOsYI%2Fse0VVmY%3D", IsApplied = true, IsDeleted = false}
           );
        }
    }
}     

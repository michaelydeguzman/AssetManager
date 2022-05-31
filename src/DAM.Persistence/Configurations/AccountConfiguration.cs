using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Persistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(e => new { e.Id });
            builder.ToTable("Accounts");
            builder.HasData(
                new Account() { Id = 1, Description = "Administration" },
                new Account() { Id = 2, Description = "Brand" },
                new Account() { Id = 3, Description = "Customer Services" },
                new Account() { Id = 4, Description = "Human Resources" },
                new Account() { Id = 5, Description = "Legal" },
                new Account() { Id = 6, Description = "Marketing" },
                new Account() { Id = 7, Description = "Operations" },
                new Account() { Id = 8, Description = "Sales" },
                new Account() { Id = 9, Description = "Sponsorship" }
                //new Account() { Id = 1, Description = "Restricted" },
                //new Account() { Id = 2, Description = "Internal" },
                //new Account() { Id = 3, Description = "External" },
                //new Account() { Id = 4, Description = "Customer" }
           );
        }
    }
}     

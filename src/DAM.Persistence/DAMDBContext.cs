using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DAM.Persistence.DAMDBContext;

namespace DAM.Persistence
{
    public class DAMDBContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        public DAMDBContext(DbContextOptions<DAMDBContext> options)
            : base(options)
        {
        }

        public DbSet<ApprovalTemplate> ApprovalTemplates { get; set; }
        public DbSet<ApprovalTemplateLevel> ApprovalTemplateLevels { get; set; }
        public DbSet<ApprovalTemplateLevelApprover> ApprovalTemplateLevelApprovers { get; set; }
        public DbSet<ApprovalLevel> ApprovalLevels { get; set; }
        public DbSet<ApprovalLevelApprover> ApprovalLevelApprovers { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetAudit> AssetAudit { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<AssetAccountMetaData> AssetAccounts { get; set; }
        public DbSet<AssetCountryRegionMetaData> AssetCountryRegions { get; set; }
        public DbSet<FolderAccountMetaData> FolderAccounts { get; set; }
        public DbSet<FolderCountryRegionMetaData> FolderCountryRegions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<DAMToDynamic> DAMToDynamics { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<FeatureFlag> FeatureFlags { get; set; }
        public DbSet<UserFolder> UserFolders { get; set; }
        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<ApplicationRole> AppRoles { get; set; }
        public DbSet<ApplicationUserRole> AppUserRoles { get; set; }
        public DbSet<AssetVersions> AssetVersions { get; set; }
        public DbSet<PinAsset> PinAssets { get; set; }
        public DbSet<PinFolder> PinFolders { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<UserOOO> UserOOO { get; set; }
        public DbSet<Watermark> Watermarks { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Logo> Logos { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectItem> ProjectItems { get; set; }
        public DbSet<ProjectComment> ProjectComments { get; set; }
        public DbSet<ProjectTeamFollower> ProjectTeamFollowers { get; set; }
        public DbSet<ProjectUserFollower> ProjectUserFollowers { get; set; }
        public DbSet<ProjectOwner> ProjectOwners { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }

        DatabaseFacade IDbContext.Database
        {
            get
            {
                return base.Database;
            }
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.RemovePluralizingTableNameConvention();
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DAMDBContext).Assembly);

            string DefaultAdminId = new Guid().ToString();

            ////create default user
            //var defaultAppAdmin = new ApplicationUser
            //{
            //    Id = DefaultAdminId,
            //    Email = "kirsten.cilliers@simple.io",
            //    NormalizedEmail = "KIRSTEN.CILLIERS@SIMPLE.IO",
            //    EmailConfirmed = true,
            //    UserName = "Kirsten Cilliers",
            //    NormalizedUserName = "KIRSTEN CILLIERS",
            //    PhoneNumber = "+61499999529",
            //    Active = true,
            //      UserRole = 1
            //};

            ////set default user password
            //PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            //defaultAppAdmin.PasswordHash = ph.HashPassword(defaultAppAdmin, "Simple5%");

            ////seed default admin user
            //modelBuilder.Entity<ApplicationUser>().HasData(defaultAppAdmin);

            //modelBuilder.Entity<ApplicationUser>(b =>
            //{
            //// Each User can have many UserClaims
            //b.HasMany(e => e.Claims)
            //    .WithOne(e => e.User)
            //    .HasForeignKey(uc => uc.UserId)
            //    .IsRequired();

            //// Each User can have many UserLogins
            //b.HasMany(e => e.Logins)
            //    .WithOne(e => e.User)
            //    .HasForeignKey(ul => ul.UserId)
            //    .IsRequired();

            //// Each User can have many UserTokens
            //b.HasMany(e => e.Tokens)
            //    .WithOne(e => e.User)
            //    .HasForeignKey(ut => ut.UserId)
            //    .IsRequired();

            //// Each User can have many entries in the UserRole join table
            //b.HasMany(e => e.UserRoles)
            //    .WithOne(e => e.User)
            //    .HasForeignKey(ur => ur.UserId)
            //    .IsRequired();
            //});

            //modelBuilder.Entity<ApplicationRole>(b =>
            //{
            // Each Role can have many entries in the UserRole join table
            //b.HasMany(e => e.UserRoles)
            //    .WithOne(e => e.Role)
            //    .HasForeignKey(ur => ur.RoleId)
            //    .IsRequired();

            //// Each Role can have many associated RoleClaims
            //b.HasMany(e => e.RoleClaims)
            //    .WithOne(e => e.Role)
            //    .HasForeignKey(rc => rc.RoleId)
            //    .IsRequired();
            //});

        }


        // Identity Configuration

    }
}
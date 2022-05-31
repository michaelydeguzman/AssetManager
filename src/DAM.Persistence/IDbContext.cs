using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Threading.Tasks;

namespace DAM.Persistence
{
    public interface IDbContext : IDisposable
    {
     
        DbSet<UserRole> UserRoles { get; set; }

        DbSet<Asset> Assets { get; set; }
        DbSet<AssetAccountMetaData> AssetAccounts { get; set; }
        DbSet<AssetCountryRegionMetaData> AssetCountryRegions { get; set; }
        DbSet<Folder> Folders { get; set; }
        DbSet<FolderAccountMetaData> FolderAccounts { get; set; }
        DbSet<FolderCountryRegionMetaData> FolderCountryRegions { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<Region> Regions { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<ApprovalTemplate> ApprovalTemplates { get; set; }
        DbSet<ApprovalTemplateLevel> ApprovalTemplateLevels { get; set; }
        DbSet<ApprovalTemplateLevelApprover> ApprovalTemplateLevelApprovers { get; set; }
        DbSet<ApprovalLevel> ApprovalLevels { get; set; }
        DbSet<ApprovalLevelApprover> ApprovalLevelApprovers { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<EmailTemplate> EmailTemplates { get; set; }
        DbSet<AssetAudit> AssetAudit { get; set; }
        DbSet<DAMToDynamic> DAMToDynamics { get; set; }
        DbSet<Company> Companies { get; set; }
        DbSet<FeatureFlag> FeatureFlags { get; set; }
        DbSet<UserFolder> UserFolders { get; set; }
        DbSet<ApplicationUser> AppUsers { get; set; }
        DbSet<AssetVersions> AssetVersions { get; set; }
        DbSet<PinAsset> PinAssets { get; set; }
        DbSet<PinFolder> PinFolders { get; set; }
        DbSet<UserOOO> UserOOO { get; set; }

        DatabaseFacade Database { get; }
        DbSet<Cart> Cart { get; set; }
        DbSet<CartItem> CartItems { get; set; }

        DbSet<Watermark> Watermarks { get; set; }

        DbSet<Theme> Themes { get; set; }
        DbSet<Logo> Logos { get; set; }

        DbSet<Team> Teams { get; set; }
        DbSet<TeamMember> TeamMembers { get; set; }

        DbSet<Project> Projects { get; set; }
        DbSet<ProjectItem> ProjectItems { get; set; }
        DbSet<ProjectComment> ProjectComments { get; set; }
        DbSet<ProjectTeamFollower> ProjectTeamFollowers { get; set; }
        DbSet<ProjectUserFollower> ProjectUserFollowers { get; set; }
        DbSet<ProjectOwner> ProjectOwners { get; set; }

        DbSet<EmailQueue> EmailQueues { get; set; }

        int SaveChanges();

        Task<int> SaveChangesAsync();

        EntityEntry Add(object entity);

        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;

        DbQuery<TQuery> Query<TQuery>() where TQuery : class;

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
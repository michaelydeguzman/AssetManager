using AutoMapper;
using DAM.Application.Accounts.Dtos;
using DAM.Application.Approval.Dtos;
using DAM.Application.Assets;
using DAM.Application.Assets.Dtos;
using DAM.Application.AuditTrail.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.FeatureFlags.Dtos;
using DAM.Application.Folders.Dtos;
using DAM.Application.Companies.Dtos;
using DAM.Application.UserRoles.Dtos;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using System.Linq;
using DAM.Application.Pin.Dtos;
using DAM.Application.Carts.Dtos;
using DAM.Application.Watermarks.Dto;
using DAM.Application.Styles.Dtos;
using DAM.Application.Logos.Dtos;
using DAM.Application.Teams.Dtos;
using DAM.Application.Projects.Dtos;
using DAM.Application.Emails.Dtos;

namespace DAM.Application.Common.Assemblers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            this.CreateMap<AssetDto, Asset>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.Title, c => c.MapFrom(c => c.Name));
            this.CreateMap<Asset, AssetDto>()
                .ForMember(x => x.Accounts, c => c.MapFrom(c => c.AssetAccounts.Select(aa => aa.Account)))
                .ForMember(x => x.Countries, c => c.MapFrom(c => c.AssetCountryRegions.Select(aa => aa.Country).Distinct()))
                .ForMember(x => x.Regions, c => c.MapFrom(c => c.AssetCountryRegions.Select(aa => aa.Region)))
                .ForMember(x => x.Name, c => c.MapFrom(c => c.Title))
                .ForMember(x => x.AssetVersions, c => c.MapFrom(c => c.AssetVersions))
                .ForMember(x => x.Comments, c => c.MapFrom(c => c.Description));

            this.CreateMap<Asset, AssetAuditDto>()
                .ForMember(x => x.Accounts, c => c.MapFrom(c => c.AssetAccounts.Select(aa => aa.Account)))
                .ForMember(x => x.Countries, c => c.MapFrom(c => c.AssetCountryRegions.Select(aa => aa.Country).Distinct()))
                .ForMember(x => x.Regions, c => c.MapFrom(c => c.AssetCountryRegions.Select(aa => aa.Region)))
                .ForMember(x => x.Name, c => c.MapFrom(c => c.Title))
                .ForMember(x => x.Comments, c => c.MapFrom(c => c.Description))
                .ForMember(x => x.CreatedById, opt => opt.Ignore())
                .ForMember(x => x.ModifiedById, opt => opt.Ignore())
                .ForMember(x => x.Status, opt => opt.Ignore());

            this.CreateMap<AssetVersions, AssetAuditDto>()
                .ForMember(x => x.Comments, c => c.MapFrom(c => c.Description))
                .ForMember(x => x.CreatedById, opt => opt.Ignore())
                .ForMember(x => x.ModifiedById, opt => opt.Ignore())
                .ForMember(x => x.Status, opt => opt.Ignore());

            this.CreateMap<AssetVersions, AssetVersionsDto>();
            this.CreateMap<AssetVersionsDto, AssetVersions>();

            this.CreateMap<AssetVersions, AssetDto>();
            this.CreateMap<AssetDto, AssetVersions>();

            this.CreateMap<AssetAuditTrailDto, AssetAudit>();
            this.CreateMap<AssetAudit, AssetAuditTrailDto>();

            this.CreateMap<TagDto, Tag>();
            this.CreateMap<Tag, TagDto>();

            this.CreateMap<Asset, UpdateAssetDto>()
                 .ForMember(x => x.Accounts, c => c.MapFrom(c => c.AssetAccounts.Select(aa => aa.Account)))
                 .ForMember(x => x.Countries, c => c.MapFrom(c => c.AssetCountryRegions.Select(aa => aa.Country).Distinct()))
                 .ForMember(x => x.Regions, c => c.MapFrom(c => c.AssetCountryRegions.Select(aa => aa.Region)))
                 .ForMember(x => x.Name, c => c.MapFrom(c => c.Title));

            this.CreateMap<FolderDto, Folder>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.Description, c => c.MapFrom(c => c.Comments));
            this.CreateMap<Folder, FolderDto>()
                .ForMember(x => x.Accounts, c => c.MapFrom(c => c.FolderAccounts.Select(fa => fa.Account)))
                .ForMember(x => x.Countries, c => c.MapFrom(c => c.FolderCountryRegions.Select(fa => fa.Country).Distinct()))
                .ForMember(x => x.Regions, c => c.MapFrom(c => c.FolderCountryRegions.Select(fa => fa.Region)))
                .ForMember(x => x.Comments, c => c.MapFrom(c => c.Description));

            this.CreateMap<AccountDto, Account>()
                .ForMember(x => x.Description, c => c.MapFrom(c => c.Name));
            this.CreateMap<Account, AccountDto>()
                .ForMember(x => x.Name, c => c.MapFrom(c => c.Description));

            this.CreateMap<CountryDto, Country>();
            this.CreateMap<Country, CountryDto>();
            //.ForMember(x => x.Regions, opt => opt.Ignore());

            this.CreateMap<RegionDto, Region>();
            //.ForMember(x => x.Description, c => c.MapFrom(c => c.Name));
            this.CreateMap<Region, RegionDto>();
            //.ForMember(x => x.Name, c => c.MapFrom(c => c.Description));

            this.CreateMap<RegionDto, Region>();
            this.CreateMap<Region, RegionDto>();

            this.CreateMap<FolderAccountMetaData, FolderAccountMetaDataDto>()
                  .ForMember(x => x.Folder, opt => opt.Ignore());
            this.CreateMap<FolderAccountMetaDataDto, FolderAccountMetaData>();

            this.CreateMap<FolderCountryRegionMetaData, FolderCountryRegionMetaDataDto>()
                 .ForMember(x => x.Folder, opt => opt.Ignore());
            this.CreateMap<FolderCountryRegionMetaDataDto, FolderCountryRegionMetaData>();

            this.CreateMap<UploadToDynamicsDto, DAMToDynamic>()
                .ForMember(t => t.UploadedBy, f => f.MapFrom(f => f.UploadedByUser));

            this.CreateMap<DAMToDynamic, UploadToDynamicsDto>()
                .ForMember(t => t.UploadedBy, f => f.MapFrom(f => f.UploadedBy.Id));

            this.CreateMap<ApprovalLevelDto, ApprovalLevel>()
                .ForMember(x => x.Approvers, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());
            this.CreateMap<ApprovalLevel, ApprovalLevelDto>();

            this.CreateMap<ApprovalLevelApproverDto, ApprovalLevelApprover>()
              .ForMember(x => x.ApprovalLevel, opt => opt.Ignore())
              .ForMember(x => x.CreatedDate, opt => opt.Ignore())
              .ForMember(x => x.ModifiedDate, opt => opt.Ignore());
            this.CreateMap<ApprovalLevelApprover, ApprovalLevelApproverDto>();

            this.CreateMap<UserRoleDto, UserRole>();
            this.CreateMap<UserRole, UserRoleDto>();

            this.CreateMap<FeatureFlagDto, FeatureFlag>();
            this.CreateMap<FeatureFlag, FeatureFlagDto>();

            this.CreateMap<CompanyDto, Company>();
            this.CreateMap<Company, CompanyDto>();

            this.CreateMap<UserFolderDto, UserFolder>();
            this.CreateMap<UserFolder, UserFolderDto>();
            this.CreateMap<UserDto, ApplicationUser>();
            this.CreateMap<ApplicationUser, UserDto>();

            this.CreateMap<ApplicationUserDto, ApplicationUser>();
            this.CreateMap<ApplicationUser, ApplicationUserDto>();

            this.CreateMap<UserRoleDto, ApplicationRole>();
            this.CreateMap<ApplicationRole, UserRoleDto>();
            this.CreateMap<PinAsset, PinAssetsDto>();
            this.CreateMap<PinAssetsDto, PinAsset>();
            this.CreateMap<PinFolder, PinFoldersDto>();
            this.CreateMap<PinFoldersDto, PinFolder>();
            this.CreateMap<AssetAccountMetaData, AccountDto>()
                .ForMember(x => x.Id, c => c.MapFrom(c => c.AccountId));
            this.CreateMap<AssetCountryRegionMetaData, CountryDto>()
                .ForMember(x => x.Id, c => c.MapFrom(c => c.CountryId));
            this.CreateMap<AssetCountryRegionMetaData, RegionDto>()
                .ForMember(x => x.Id, c => c.MapFrom(c => c.RegionId));

            this.CreateMap<ApprovalTemplate, ApprovalTemplateDto>();
            this.CreateMap<ApprovalTemplateDto, ApprovalTemplate>();

            this.CreateMap<ApprovalTemplateLevel, ApprovalTemplateLevelDto>();
            this.CreateMap<ApprovalTemplateLevelDto, ApprovalTemplateLevel>();

            this.CreateMap<ApprovalTemplateLevelApprover, ApprovalTemplateLevelApproverDto>();
            this.CreateMap<ApprovalTemplateLevelApproverDto, ApprovalTemplateLevelApprover>();

            this.CreateMap<CartDto, Cart>();
            this.CreateMap<Cart, CartDto>()
                .ForMember(dto => dto.AssetIds, map => map.MapFrom(c => c.CartItems.Select(ci => ci.AssetID)));
            this.CreateMap<UserOOO, UserOOODto>();
            this.CreateMap<UserOOODto, UserOOO>();

            this.CreateMap<WatermarkDto, Watermark>();
            this.CreateMap<Watermark, WatermarkDto>();

            this.CreateMap<ThemeDto, Theme>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());
            this.CreateMap<Theme, ThemeDto>();

            this.CreateMap<LogoDto, Logo>();
            this.CreateMap<Logo, LogoDto>();

            this.CreateMap<TeamDto, Team>();
            this.CreateMap<Team, TeamDto>();

            this.CreateMap<TeamMemberDto, TeamMember>();
            this.CreateMap<TeamMember, TeamMemberDto>();

            this.CreateMap<ProjectDto, Project>();
            this.CreateMap<Project, ProjectDto>();
            this.CreateMap<ProjectItemDto, ProjectItem>();
            this.CreateMap<ProjectItem, ProjectItemDto>();
            this.CreateMap<ProjectCommentDto, ProjectComment>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());
            this.CreateMap<ProjectComment, ProjectCommentDto>();
            this.CreateMap<ProjectTeamFollowerDto, ProjectTeamFollower>();
            this.CreateMap<ProjectTeamFollower, ProjectTeamFollowerDto>();
            this.CreateMap<ProjectUserFollowerDto, ProjectUserFollower>();
            this.CreateMap<ProjectUserFollower, ProjectUserFollowerDto>();

            this.CreateMap<ProjectOwnerDto, ProjectOwner>();
            this.CreateMap<ProjectOwner, ProjectOwnerDto>();

            this.CreateMap<EmailTemplateDto, EmailTemplate>();
            this.CreateMap<EmailTemplate, EmailTemplateDto>();
        }
    }
}
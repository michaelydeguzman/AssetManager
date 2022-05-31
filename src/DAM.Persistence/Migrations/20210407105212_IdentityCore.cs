using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class IdentityCore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    CanArchive = table.Column<bool>(nullable: true),
                    CanEdit = table.Column<bool>(nullable: true),
                    CanShare = table.Column<bool>(nullable: true),
                    CanApprove = table.Column<bool>(nullable: true),
                    CanAdd = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureFlags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureFlagNumber = table.Column<int>(nullable: false),
                    FeatureFlagName = table.Column<string>(nullable: true),
                    IsActivated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureFlags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(nullable: true),
                    CanArchive = table.Column<bool>(nullable: false),
                    CanEdit = table.Column<bool>(nullable: false),
                    CanShare = table.Column<bool>(nullable: false),
                    CanApprove = table.Column<bool>(nullable: false),
                    CanAdd = table.Column<bool>(nullable: false),
                    CanDelete = table.Column<bool>(nullable: false),
                    CanUpload = table.Column<bool>(nullable: false),
                    CanMove = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    RoleId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId1",
                        column: x => x.RoleId1,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Regions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    AssetId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    FolderId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalLevelApprovers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    ApprovalLevelId = table.Column<int>(nullable: false),
                    ApproverId = table.Column<string>(nullable: true),
                    ApprovalStatus = table.Column<int>(nullable: false),
                    ReviewDate = table.Column<DateTimeOffset>(nullable: true),
                    Comments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalLevelApprovers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true),
                    RoleId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId1",
                        column: x => x.RoleId1,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalLevels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    AssetId = table.Column<int>(nullable: false),
                    LevelNumber = table.Column<int>(nullable: false),
                    CompletedDate = table.Column<DateTimeOffset>(nullable: true),
                    IsActiveLevel = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "AssetAudit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(nullable: true),
                    AssetFileName = table.Column<string>(nullable: true),
                    FolderId = table.Column<int>(nullable: true),
                    FolderName = table.Column<string>(nullable: true),
                    AuditType = table.Column<int>(nullable: false),
                    AuditTypeText = table.Column<string>(nullable: true),
                    PreviousParameters = table.Column<string>(nullable: true),
                    NewParameters = table.Column<string>(nullable: true),
                    AuditCreatedByUserId = table.Column<string>(nullable: true),
                    AuditCreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    AuditCreatedByName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAudit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetCountryRegions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    AssetId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCountryRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetCountryRegions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetCountryRegions_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    FolderId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StatusUpdatedDate = table.Column<DateTimeOffset>(nullable: true),
                    Thumbnail = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DownloadUrl = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    FileSizeText = table.Column<string>(nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    DownloadCount = table.Column<int>(nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsCognitive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    AssetId = table.Column<int>(nullable: true),
                    FolderId = table.Column<int>(nullable: true),
                    CommentText = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    RootFolderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ImageFileExtension = table.Column<string>(nullable: true),
                    UserRoleId = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DAMToDynamics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(nullable: false),
                    UploadedById = table.Column<string>(nullable: true),
                    IsUploaded = table.Column<bool>(nullable: false),
                    Entity = table.Column<string>(nullable: true),
                    EntityId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAMToDynamics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DAMToDynamics_AspNetUsers_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    EmailTemplateKey = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Contents = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailTemplates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailTemplates_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    FolderName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentFolderId = table.Column<int>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Folders_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Folders_Folders_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FolderCountryRegions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    FolderId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderCountryRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderCountryRegions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderCountryRegions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolderCountryRegions_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderCountryRegions_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolderCountryRegions_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFolders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    FolderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFolders_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFolders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Administration" },
                    { 2, "Brand" },
                    { 3, "Customer Services" },
                    { 4, "Human Resources" },
                    { 5, "Legal" },
                    { 6, "Marketing" },
                    { 7, "Operations" },
                    { 8, "Sales" },
                    { 9, "Sponsorship" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 2, "Excludes Ireland and Wales", "UK" },
                    { 1, "All states and territories", "Australia" }
                });

            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Id", "FeatureFlagName", "FeatureFlagNumber", "IsActivated" },
                values: new object[,]
                {
                    { 1, "Approvals", 1, true },
                    { 2, "Partnership", 2, true },
                    { 3, "AssetVersioning", 3, true },
                    { 4, "VideoIndexer", 4, true },
                    { 5, "Tagging", 5, true },
                    { 6, "PowerAnnotate", 6, true },
                    { 7, "Archive", 7, true }
                });

            migrationBuilder.InsertData(
                table: "Folders",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "Deleted", "Description", "FolderName", "ModifiedById", "ModifiedDate", "ParentFolderId" },
                values: new object[] { 1, null, new DateTimeOffset(new DateTime(2021, 4, 7, 10, 52, 10, 424, DateTimeKind.Unspecified).AddTicks(54), new TimeSpan(0, 0, 0, 0, 0)), false, "root", "Dubber", null, null, null });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CanAdd", "CanApprove", "CanArchive", "CanDelete", "CanEdit", "CanMove", "CanShare", "CanUpload", "RoleName" },
                values: new object[,]
                {
                    { 2, true, false, true, true, true, true, true, true, "Partner Admin" },
                    { 1, true, true, true, true, true, true, true, true, "Administrator" },
                    { 3, false, false, false, false, false, false, true, false, "Subscriber" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "CountryId", "Description" },
                values: new object[,]
                {
                    { 1, 1, "ACT" },
                    { 17, 2, "South West" },
                    { 16, 2, "South East" },
                    { 15, 2, "Scotland" },
                    { 14, 2, "Northern Ireland" },
                    { 13, 2, "North West" },
                    { 12, 2, "North East" },
                    { 11, 2, "Midlands" },
                    { 18, 2, "Wales" },
                    { 10, 2, "Greater London" },
                    { 8, 1, "Victoria" },
                    { 7, 1, "Western Australia" },
                    { 6, 1, "Tasmania" },
                    { 5, 1, "South Australia" },
                    { 4, 1, "Northern Territory" },
                    { 3, 1, "New South Wales" },
                    { 2, 1, "Queensland" },
                    { 9, 2, "East of England" },
                    { 19, 2, "Yorshire" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLevelApprovers_ApprovalLevelId",
                table: "ApprovalLevelApprovers",
                column: "ApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLevelApprovers_ApproverId",
                table: "ApprovalLevelApprovers",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLevelApprovers_CreatedById",
                table: "ApprovalLevelApprovers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLevelApprovers_ModifiedById",
                table: "ApprovalLevelApprovers",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLevels_AssetId",
                table: "ApprovalLevels",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLevels_CreatedById",
                table: "ApprovalLevels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLevels_ModifiedById",
                table: "ApprovalLevels",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId1",
                table: "AspNetRoleClaims",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId1",
                table: "AspNetUserClaims",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId1",
                table: "AspNetUserLogins",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId1",
                table: "AspNetUserRoles",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId1",
                table: "AspNetUserRoles",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserRoleId",
                table: "AspNetUsers",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserTokens_UserId1",
                table: "AspNetUserTokens",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAccounts_AccountId",
                table: "AssetAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAccounts_AssetId",
                table: "AssetAccounts",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAccounts_CreatedById",
                table: "AssetAccounts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAccounts_ModifiedById",
                table: "AssetAccounts",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAudit_AssetId",
                table: "AssetAudit",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAudit_AuditCreatedByUserId",
                table: "AssetAudit",
                column: "AuditCreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAudit_FolderId",
                table: "AssetAudit",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCountryRegions_AssetId",
                table: "AssetCountryRegions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCountryRegions_CountryId",
                table: "AssetCountryRegions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCountryRegions_CreatedById",
                table: "AssetCountryRegions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCountryRegions_ModifiedById",
                table: "AssetCountryRegions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCountryRegions_RegionId",
                table: "AssetCountryRegions",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CreatedById",
                table: "Assets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_FolderId",
                table: "Assets",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ModifiedById",
                table: "Assets",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AssetId",
                table: "Comments",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedById",
                table: "Comments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_FolderId",
                table: "Comments",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ModifiedById",
                table: "Comments",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CreatedById",
                table: "Companies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ModifiedById",
                table: "Companies",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_RootFolderId",
                table: "Companies",
                column: "RootFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_DAMToDynamics_UploadedById",
                table: "DAMToDynamics",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_CreatedById",
                table: "EmailTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_ModifiedById",
                table: "EmailTemplates",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FolderAccounts_AccountId",
                table: "FolderAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderAccounts_CreatedById",
                table: "FolderAccounts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FolderAccounts_FolderId",
                table: "FolderAccounts",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderAccounts_ModifiedById",
                table: "FolderAccounts",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FolderCountryRegions_CountryId",
                table: "FolderCountryRegions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderCountryRegions_CreatedById",
                table: "FolderCountryRegions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FolderCountryRegions_FolderId",
                table: "FolderCountryRegions",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderCountryRegions_ModifiedById",
                table: "FolderCountryRegions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FolderCountryRegions_RegionId",
                table: "FolderCountryRegions",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_CreatedById",
                table: "Folders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ModifiedById",
                table: "Folders",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ParentFolderId",
                table: "Folders",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CountryId",
                table: "Regions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AssetId",
                table: "Tags",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFolders_FolderId",
                table: "UserFolders",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFolders_UserId",
                table: "UserFolders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAccounts_AspNetUsers_CreatedById",
                table: "AssetAccounts",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAccounts_AspNetUsers_ModifiedById",
                table: "AssetAccounts",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAccounts_Assets_AssetId",
                table: "AssetAccounts",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FolderAccounts_AspNetUsers_CreatedById",
                table: "FolderAccounts",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolderAccounts_AspNetUsers_ModifiedById",
                table: "FolderAccounts",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolderAccounts_Folders_FolderId",
                table: "FolderAccounts",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLevelApprovers_ApprovalLevels_ApprovalLevelId",
                table: "ApprovalLevelApprovers",
                column: "ApprovalLevelId",
                principalTable: "ApprovalLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLevelApprovers_AspNetUsers_ApproverId",
                table: "ApprovalLevelApprovers",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLevelApprovers_AspNetUsers_CreatedById",
                table: "ApprovalLevelApprovers",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLevelApprovers_AspNetUsers_ModifiedById",
                table: "ApprovalLevelApprovers",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId1",
                table: "AspNetUserRoles",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLevels_AspNetUsers_CreatedById",
                table: "ApprovalLevels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLevels_AspNetUsers_ModifiedById",
                table: "ApprovalLevels",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLevels_Assets_AssetId",
                table: "ApprovalLevels",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId1",
                table: "AspNetUserClaims",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId1",
                table: "AspNetUserLogins",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId1",
                table: "AspNetUserTokens",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAudit_AspNetUsers_AuditCreatedByUserId",
                table: "AssetAudit",
                column: "AuditCreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAudit_Assets_AssetId",
                table: "AssetAudit",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAudit_Folders_FolderId",
                table: "AssetAudit",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCountryRegions_AspNetUsers_CreatedById",
                table: "AssetCountryRegions",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCountryRegions_AspNetUsers_ModifiedById",
                table: "AssetCountryRegions",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCountryRegions_Assets_AssetId",
                table: "AssetCountryRegions",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AspNetUsers_CreatedById",
                table: "Assets",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AspNetUsers_ModifiedById",
                table: "Assets",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Folders_FolderId",
                table: "Assets",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_ModifiedById",
                table: "Comments",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Folders_FolderId",
                table: "Comments",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_CreatedById",
                table: "Companies",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_ModifiedById",
                table: "Companies",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Folders_RootFolderId",
                table: "Companies",
                column: "RootFolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_CreatedById",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_ModifiedById",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_AspNetUsers_CreatedById",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_AspNetUsers_ModifiedById",
                table: "Folders");

            migrationBuilder.DropTable(
                name: "ApprovalLevelApprovers");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AssetAccounts");

            migrationBuilder.DropTable(
                name: "AssetAudit");

            migrationBuilder.DropTable(
                name: "AssetCountryRegions");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "DAMToDynamics");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "FeatureFlags");

            migrationBuilder.DropTable(
                name: "FolderAccounts");

            migrationBuilder.DropTable(
                name: "FolderCountryRegions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "UserFolders");

            migrationBuilder.DropTable(
                name: "ApprovalLevels");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Folders");
        }
    }
}

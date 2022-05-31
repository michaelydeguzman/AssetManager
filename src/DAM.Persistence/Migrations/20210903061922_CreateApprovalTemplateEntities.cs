using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class CreateApprovalTemplateEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    TemplateName = table.Column<string>(nullable: true),
                    isDeleted = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplates_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalTemplateLevels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    ApprovalTemplateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplateLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateLevels_ApprovalTemplates_ApprovalTemplateId",
                        column: x => x.ApprovalTemplateId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateLevels_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateLevels_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalTemplateLevelApprovers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    ApprovalTemplateLevelId = table.Column<int>(nullable: false),
                    ApproverId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplateLevelApprovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateLevelApprovers_ApprovalTemplateLevels_ApprovalTemplateLevelId",
                        column: x => x.ApprovalTemplateLevelId,
                        principalTable: "ApprovalTemplateLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateLevelApprovers_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateLevelApprovers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateLevelApprovers_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateLevelApprovers_ApprovalTemplateLevelId",
                table: "ApprovalTemplateLevelApprovers",
                column: "ApprovalTemplateLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateLevelApprovers_ApproverId",
                table: "ApprovalTemplateLevelApprovers",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateLevelApprovers_CreatedById",
                table: "ApprovalTemplateLevelApprovers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateLevelApprovers_ModifiedById",
                table: "ApprovalTemplateLevelApprovers",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateLevels_ApprovalTemplateId",
                table: "ApprovalTemplateLevels",
                column: "ApprovalTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateLevels_CreatedById",
                table: "ApprovalTemplateLevels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateLevels_ModifiedById",
                table: "ApprovalTemplateLevels",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplates_CreatedById",
                table: "ApprovalTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplates_ModifiedById",
                table: "ApprovalTemplates",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalTemplateLevelApprovers");

            migrationBuilder.DropTable(
                name: "ApprovalTemplateLevels");

            migrationBuilder.DropTable(
                name: "ApprovalTemplates");
        }
    }
}

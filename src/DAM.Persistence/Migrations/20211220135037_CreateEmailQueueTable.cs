using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class CreateEmailQueueTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailQueues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    EmailTemplateKey = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    FolderId = table.Column<int>(nullable: true),
                    AssetId = table.Column<int>(nullable: true),
                    AssetVersionId = table.Column<int>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Contents = table.Column<string>(nullable: true),
                    ToAddress = table.Column<string>(nullable: true),
                    FromAddress = table.Column<string>(nullable: true),
                    Processed = table.Column<bool>(nullable: false),
                    RetryCount = table.Column<int>(nullable: false),
                    Error = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailQueues_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailQueues_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Subject",
                value: "Welcome To Simple Asset Manager!");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_CreatedById",
                table: "EmailQueues",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_ModifiedById",
                table: "EmailQueues",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailQueues");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Subject",
                value: "Welcome To  Simple Asset Manager!");
 
        }
    }
}

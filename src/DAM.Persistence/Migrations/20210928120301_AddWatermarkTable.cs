using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddWatermarkTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Watermarks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    WatermarkUrl = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    FolderId = table.Column<int>(nullable: true),
                    Opacity = table.Column<decimal>(nullable: false),
                    Size = table.Column<decimal>(nullable: false),
                    WatermarkPosition = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watermarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Watermarks_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Watermarks_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Watermarks_CreatedById",
                table: "Watermarks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Watermarks_ModifiedById",
                table: "Watermarks",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Watermarks");
        }
    }
}

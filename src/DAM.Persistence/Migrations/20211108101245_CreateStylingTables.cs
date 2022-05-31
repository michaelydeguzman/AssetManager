using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class CreateStylingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    IsApplied = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logos_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logos_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Styles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PrimaryColor = table.Column<string>(nullable: true),
                    SecondaryColor = table.Column<string>(nullable: true),
                    AltColor = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    IsApplied = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Styles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Styles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Styles_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logos_CreatedById",
                table: "Logos",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Logos_ModifiedById",
                table: "Logos",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Styles_CreatedById",
                table: "Styles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Styles_ModifiedById",
                table: "Styles",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logos");

            migrationBuilder.DropTable(
                name: "Styles");
        }
    }
}

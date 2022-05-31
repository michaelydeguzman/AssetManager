using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class CreateNewOOOTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserOOO",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTimeOffset>(nullable: false),
                    EndDate = table.Column<DateTimeOffset>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOOO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOOO_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOOO_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOOO_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserOOO_CreatedById",
                table: "UserOOO",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserOOO_ModifiedById",
                table: "UserOOO",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserOOO_UserId",
                table: "UserOOO",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOOO");
        }
    }
}

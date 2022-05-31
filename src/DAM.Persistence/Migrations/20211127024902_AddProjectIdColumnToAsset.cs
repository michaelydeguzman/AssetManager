using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddProjectIdColumnToAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Assets",
                nullable: true);

           
            migrationBuilder.CreateIndex(
                name: "IX_Assets_ProjectId",
                table: "Assets",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Projects_ProjectId",
                table: "Assets",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Projects_ProjectId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_ProjectId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Assets");

        }
    }
}

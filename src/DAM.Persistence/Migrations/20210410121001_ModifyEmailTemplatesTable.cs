using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class ModifyEmailTemplatesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplates_AspNetUsers_CreatedById",
                table: "EmailTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplates_AspNetUsers_ModifiedById",
                table: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_CreatedById",
                table: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_ModifiedById",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "EmailTemplates");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EmailTemplates",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "EmailTemplates",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "EmailTemplates",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedDate",
                table: "EmailTemplates",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_CreatedById",
                table: "EmailTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_ModifiedById",
                table: "EmailTemplates",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplates_AspNetUsers_CreatedById",
                table: "EmailTemplates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplates_AspNetUsers_ModifiedById",
                table: "EmailTemplates",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

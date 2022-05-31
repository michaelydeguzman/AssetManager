using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddRecipientTypeForEmailTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecipientType",
                table: "EmailTemplates",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "RecipientType",
                value: "New User");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "RecipientType",
                value: "All Users");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 3,
                column: "RecipientType",
                value: "Approver");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 4,
                column: "RecipientType",
                value: "Approver");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 5,
                column: "RecipientType",
                value: "Approver");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Message", "RecipientType" },
                values: new object[] { "An asset you have submitted for review has been approved.", "Asset Owner" });

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 7,
                column: "RecipientType",
                value: "Asset Owner");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 8,
                column: "RecipientType",
                value: "Project Owner");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 9,
                column: "RecipientType",
                value: "Project Collaborator");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 10,
                column: "RecipientType",
                value: "Project Owner");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 11,
                column: "RecipientType",
                value: "Project Collaborator");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 12,
                column: "RecipientType",
                value: "Project Owner, Project Collaborator");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 13,
                column: "RecipientType",
                value: "Project Owner, Project Collaborator");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 14,
                column: "RecipientType",
                value: "Project Owner");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 15,
                column: "RecipientType",
                value: "Project Collaborator");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 16,
                column: "RecipientType",
                value: "Approver");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 17,
                column: "RecipientType",
                value: "Asset Owner");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientType",
                table: "EmailTemplates");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 6,
                column: "Message",
                value: "An item you have submitted for review has been approved.");
        }
    }
}

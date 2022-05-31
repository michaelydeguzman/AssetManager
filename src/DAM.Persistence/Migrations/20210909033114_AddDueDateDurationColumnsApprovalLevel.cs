using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddDueDateDurationColumnsApprovalLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ApprovalDueDate",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DueDate",
                table: "ApprovalLevels",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "ApprovalLevels",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 9, 9, 3, 31, 13, 304, DateTimeKind.Unspecified).AddTicks(5928), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalDueDate",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "ApprovalLevels");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ApprovalLevels");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 9, 3, 12, 6, 22, 290, DateTimeKind.Unspecified).AddTicks(4774), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

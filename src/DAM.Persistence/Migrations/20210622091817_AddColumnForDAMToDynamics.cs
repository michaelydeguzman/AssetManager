using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddColumnForDAMToDynamics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMarketing",
                table: "DAMToDynamics",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 6, 22, 9, 18, 15, 230, DateTimeKind.Unspecified).AddTicks(4786), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMarketing",
                table: "DAMToDynamics");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 6, 9, 2, 21, 42, 69, DateTimeKind.Unspecified).AddTicks(2873), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

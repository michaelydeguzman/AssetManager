using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class add_version_CRCcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CRC32Code",
                table: "AssetVersions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 9, 24, 5, 16, 45, 127, DateTimeKind.Unspecified).AddTicks(6461), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CRC32Code",
                table: "AssetVersions");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 9, 23, 5, 8, 19, 671, DateTimeKind.Unspecified).AddTicks(3252), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

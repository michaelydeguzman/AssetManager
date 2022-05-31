using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddAssetVersionPreview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Thumbnail_Preview",
                table: "AssetVersions",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2022, 1, 4, 4, 42, 56, 888, DateTimeKind.Unspecified).AddTicks(9603), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2022, 1, 4, 4, 42, 56, 889, DateTimeKind.Unspecified).AddTicks(6508), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2022, 1, 4, 4, 42, 56, 892, DateTimeKind.Unspecified).AddTicks(5763), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2022, 1, 4, 4, 42, 56, 892, DateTimeKind.Unspecified).AddTicks(7789), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail_Preview",
                table: "AssetVersions");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 30, 11, 58, 27, 701, DateTimeKind.Unspecified).AddTicks(2703), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 30, 11, 58, 27, 702, DateTimeKind.Unspecified).AddTicks(5358), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 30, 11, 58, 27, 708, DateTimeKind.Unspecified).AddTicks(317), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 30, 11, 58, 27, 708, DateTimeKind.Unspecified).AddTicks(3178), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

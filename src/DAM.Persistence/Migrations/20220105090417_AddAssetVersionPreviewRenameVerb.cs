using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddAssetVersionPreviewRenameVerb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail_Preview",
                table: "AssetVersions");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPreview",
                table: "AssetVersions",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2022, 1, 5, 9, 4, 15, 510, DateTimeKind.Unspecified).AddTicks(1415), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2022, 1, 5, 9, 4, 15, 511, DateTimeKind.Unspecified).AddTicks(7228), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2022, 1, 5, 9, 4, 15, 516, DateTimeKind.Unspecified).AddTicks(2927), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2022, 1, 5, 9, 4, 15, 516, DateTimeKind.Unspecified).AddTicks(7212), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailPreview",
                table: "AssetVersions");

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail_Preview",
                table: "AssetVersions",
                type: "nvarchar(max)",
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
    }
}

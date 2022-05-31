using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class PromoteMRMFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Id", "FeatureFlagName", "FeatureFlagNumber", "IsActivated" },
                values: new object[] { 10, "PromoteMRM", 10, false });

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 8, 3, 4, 51, 23, 655, DateTimeKind.Unspecified).AddTicks(3420), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureFlags",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 7, 23, 1, 24, 57, 274, DateTimeKind.Unspecified).AddTicks(8234), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

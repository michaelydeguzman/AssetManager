using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class addcheckingDuplicateSwitch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Id", "FeatureFlagName", "FeatureFlagNumber", "IsActivated" },
                values: new object[] { 11, "CheckDuplicate", 11, true });

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 9, 28, 0, 2, 56, 453, DateTimeKind.Unspecified).AddTicks(7514), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureFlags",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 9, 24, 5, 16, 45, 127, DateTimeKind.Unspecified).AddTicks(6461), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

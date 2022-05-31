using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class addFeatureFlagInitialValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Id", "FeatureFlagName", "FeatureFlagNumber", "IsActivated" },
                values: new object[,]
                {
                    { 8, "AuditTrail", 8, true },
                    { 9, "Report", 9, true }
                });

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 5, 21, 11, 58, 49, 566, DateTimeKind.Unspecified).AddTicks(273), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "DAM Admin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureFlags",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "FeatureFlags",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 5, 4, 6, 10, 52, 246, DateTimeKind.Unspecified).AddTicks(8868), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Dubber Admin");
        }
    }
}

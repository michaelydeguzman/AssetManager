using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class RenameDubberToDAM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "FolderName" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 4, 22, 8, 46, 33, 158, DateTimeKind.Unspecified).AddTicks(3600), new TimeSpan(0, 0, 0, 0, 0)), "DAM" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "FolderName" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 4, 21, 6, 42, 17, 388, DateTimeKind.Unspecified).AddTicks(8462), new TimeSpan(0, 0, 0, 0, 0)), "Dubber" });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class folder_restructuring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 15, 8, 53, 6, 276, DateTimeKind.Unspecified).AddTicks(6862), new TimeSpan(0, 0, 0, 0, 0)));


            migrationBuilder.InsertData(
                table: "Folders",
                columns: new[] { "CreatedById", "CreatedDate", "Deleted", "Description", "FolderName", "ModifiedById", "ModifiedDate", "ParentFolderId" },
                values: new object[] { null, new DateTimeOffset(new DateTime(2021, 4, 15, 8, 53, 6, 276, DateTimeKind.Unspecified).AddTicks(6862), new TimeSpan(0, 0, 0, 0, 0)), false, "Partner Root", "Partners",null , null, 1 });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 15, 6, 57, 44, 341, DateTimeKind.Unspecified).AddTicks(112), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

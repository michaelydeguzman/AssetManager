using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddStylesCreateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Styles",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Styles",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Styles",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedDate",
                table: "Styles",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 1, 5, 14, 6, 507, DateTimeKind.Unspecified).AddTicks(4059), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Styles",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "Deleted", "IsApplied", "ModifiedById", "ModifiedDate", "Name", "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[] { 0, null, new DateTimeOffset(new DateTime(2021, 12, 1, 5, 14, 6, 510, DateTimeKind.Unspecified).AddTicks(8388), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, null, "Default", "246,57,21", "150,100,30", "246,20,41" });

            migrationBuilder.CreateIndex(
                name: "IX_Styles_CreatedById",
                table: "Styles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Styles_ModifiedById",
                table: "Styles",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Styles_AspNetUsers_CreatedById",
                table: "Styles",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Styles_AspNetUsers_ModifiedById",
                table: "Styles",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Styles_AspNetUsers_CreatedById",
                table: "Styles");

            migrationBuilder.DropForeignKey(
                name: "FK_Styles_AspNetUsers_ModifiedById",
                table: "Styles");

            migrationBuilder.DropIndex(
                name: "IX_Styles_CreatedById",
                table: "Styles");

            migrationBuilder.DropIndex(
                name: "IX_Styles_ModifiedById",
                table: "Styles");

            migrationBuilder.DeleteData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 0);

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Styles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Styles");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Styles");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Styles");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 11, 27, 8, 22, 54, 335, DateTimeKind.Unspecified).AddTicks(5371), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Styles",
                columns: new[] { "Id", "Deleted", "IsApplied", "Name", "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[] { 1, false, true, "Default", "246,57,21", "150,100,30", "246,20,41" });
        }
    }
}

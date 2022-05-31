using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddStyleAndLogoConfigs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logos_AspNetUsers_CreatedById",
                table: "Logos");

            migrationBuilder.DropForeignKey(
                name: "FK_Logos_AspNetUsers_ModifiedById",
                table: "Logos");

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

            migrationBuilder.DropIndex(
                name: "IX_Logos_CreatedById",
                table: "Logos");

            migrationBuilder.DropIndex(
                name: "IX_Logos_ModifiedById",
                table: "Logos");

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

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Logos");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Logos");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Logos");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Logos");

            migrationBuilder.InsertData(
                table: "Logos",
                columns: new[] { "Id", "FileName", "IsApplied", "IsDeleted", "LogoUrl" },
                values: new object[] { 1, "logo-simple.png", true, false, "https://damblob1.blob.core.windows.net/logocontainer-dev-mikey/logo-simple.png?sp=r&st=2021-11-11T04:38:10Z&se=2022-11-11T12:38:10Z&spr=https&sv=2020-08-04&sr=b&sig=P60UQSs6gi2%2BobEvkcZp%2BoY5DRCbqQnOsYI%2Fse0VVmY%3D" });

            migrationBuilder.InsertData(
                table: "Styles",
                columns: new[] { "Id", "AltColor", "Deleted", "IsApplied", "Name", "PrimaryColor", "SecondaryColor" },
                values: new object[] { 1, "#00aa55", false, true, "Default", "#1d1753", "#58547d" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Styles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Styles",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Styles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedDate",
                table: "Styles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Logos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Logos",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Logos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedDate",
                table: "Logos",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Styles_CreatedById",
                table: "Styles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Styles_ModifiedById",
                table: "Styles",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Logos_CreatedById",
                table: "Logos",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Logos_ModifiedById",
                table: "Logos",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Logos_AspNetUsers_CreatedById",
                table: "Logos",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logos_AspNetUsers_ModifiedById",
                table: "Logos",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddLogosCreateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Logos",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Logos",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Logos",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedDate",
                table: "Logos",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 1, 5, 25, 12, 45, DateTimeKind.Unspecified).AddTicks(1512), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Logos",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "FileName", "IsApplied", "IsDeleted", "LogoUrl", "ModifiedById", "ModifiedDate" },
                values: new object[] { 0, null, new DateTimeOffset(new DateTime(2021, 12, 1, 5, 25, 12, 46, DateTimeKind.Unspecified).AddTicks(112), new TimeSpan(0, 0, 0, 0, 0)), "logo-simple.png", true, false, "https://damblob1.blob.core.windows.net/logocontainer-dev-mikey/logo-simple.png?sp=r&st=2021-11-11T04:38:10Z&se=2022-11-11T12:38:10Z&spr=https&sv=2020-08-04&sr=b&sig=P60UQSs6gi2%2BobEvkcZp%2BoY5DRCbqQnOsYI%2Fse0VVmY%3D", null, null });

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 1, 5, 25, 12, 49, DateTimeKind.Unspecified).AddTicks(5522), new TimeSpan(0, 0, 0, 0, 0)));

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logos_AspNetUsers_CreatedById",
                table: "Logos");

            migrationBuilder.DropForeignKey(
                name: "FK_Logos_AspNetUsers_ModifiedById",
                table: "Logos");

            migrationBuilder.DropIndex(
                name: "IX_Logos_CreatedById",
                table: "Logos");

            migrationBuilder.DropIndex(
                name: "IX_Logos_ModifiedById",
                table: "Logos");

            migrationBuilder.DeleteData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0);

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

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 1, 5, 14, 6, 507, DateTimeKind.Unspecified).AddTicks(4059), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Logos",
                columns: new[] { "Id", "FileName", "IsApplied", "IsDeleted", "LogoUrl" },
                values: new object[] { 1, "logo-simple.png", true, false, "https://damblob1.blob.core.windows.net/logocontainer-dev-mikey/logo-simple.png?sp=r&st=2021-11-11T04:38:10Z&se=2022-11-11T12:38:10Z&spr=https&sv=2020-08-04&sr=b&sig=P60UQSs6gi2%2BobEvkcZp%2BoY5DRCbqQnOsYI%2Fse0VVmY%3D" });

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 1, 5, 14, 6, 510, DateTimeKind.Unspecified).AddTicks(8388), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

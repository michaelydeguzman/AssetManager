using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class DefaultThemeLogoUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 2, 6, 23, 419, DateTimeKind.Unspecified).AddTicks(5582), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 2, 6, 23, 420, DateTimeKind.Unspecified).AddTicks(1530), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "LogoFileName", "LogoUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 2, 6, 23, 423, DateTimeKind.Unspecified).AddTicks(8890), new TimeSpan(0, 0, 0, 0, 0)), "logo-simple", "https://damblob1.blob.core.windows.net/logocontainer-dev-mikey/logo-simple.png?sp=r&st=2021-11-11T04:38:10Z&se=2022-11-11T12:38:10Z&spr=https&sv=2020-08-04&sr=b&sig=P60UQSs6gi2%2BobEvkcZp%2BoY5DRCbqQnOsYI%2Fse0VVmY%3D" });

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "LogoFileName", "LogoUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 2, 6, 23, 424, DateTimeKind.Unspecified).AddTicks(155), new TimeSpan(0, 0, 0, 0, 0)), "logo-simple", "https://damblob1.blob.core.windows.net/logocontainer-dev-mikey/logo-simple.png?sp=r&st=2021-11-11T04:38:10Z&se=2022-11-11T12:38:10Z&spr=https&sv=2020-08-04&sr=b&sig=P60UQSs6gi2%2BobEvkcZp%2BoY5DRCbqQnOsYI%2Fse0VVmY%3D" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 0, 58, 2, 679, DateTimeKind.Unspecified).AddTicks(2245), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 0, 58, 2, 679, DateTimeKind.Unspecified).AddTicks(9108), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "LogoFileName", "LogoUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 0, 58, 2, 682, DateTimeKind.Unspecified).AddTicks(4555), new TimeSpan(0, 0, 0, 0, 0)), null, null });

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "LogoFileName", "LogoUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 0, 58, 2, 682, DateTimeKind.Unspecified).AddTicks(5450), new TimeSpan(0, 0, 0, 0, 0)), null, null });
        }
    }
}

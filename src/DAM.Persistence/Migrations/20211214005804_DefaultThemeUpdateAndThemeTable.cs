using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class DefaultThemeUpdateAndThemeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Styles");

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PrimaryColor = table.Column<string>(nullable: true),
                    SecondaryColor = table.Column<string>(nullable: true),
                    TertiaryColor = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    IsApplied = table.Column<bool>(nullable: false),
                    LogoFileName = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    LogoKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Themes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Themes_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "Themes",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "Deleted", "IsApplied", "LogoFileName", "LogoKey", "LogoUrl", "ModifiedById", "ModifiedDate", "Name", "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[,]
                {
                    { 1, null, new DateTimeOffset(new DateTime(2021, 12, 14, 0, 58, 2, 682, DateTimeKind.Unspecified).AddTicks(4555), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, null, null, null, null, "Simple Asset Manager Dark", "246,57,21", "150,100,30", "246,20,41" },
                    { 2, null, new DateTimeOffset(new DateTime(2021, 12, 14, 0, 58, 2, 682, DateTimeKind.Unspecified).AddTicks(5450), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, null, null, null, null, "Simple Asset Manager Light", "246,57,21", "150,100,30", "246,20,41" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Themes_CreatedById",
                table: "Themes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_ModifiedById",
                table: "Themes",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.CreateTable(
                name: "Styles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsApplied = table.Column<bool>(type: "bit", nullable: false),
                    LogoFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TertiaryColor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Styles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Styles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Styles_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 0, 33, 45, 515, DateTimeKind.Unspecified).AddTicks(1797), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 0, 33, 45, 515, DateTimeKind.Unspecified).AddTicks(8628), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Styles",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "Deleted", "IsApplied", "LogoFileName", "LogoUrl", "ModifiedById", "ModifiedDate", "Name", "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[,]
                {
                    { 1, null, new DateTimeOffset(new DateTime(2021, 12, 14, 0, 33, 45, 518, DateTimeKind.Unspecified).AddTicks(4445), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, null, null, null, "Simple Asset Manager Dark", "246,57,21", "150,100,30", "246,20,41" },
                    { 2, null, new DateTimeOffset(new DateTime(2021, 12, 14, 0, 33, 45, 518, DateTimeKind.Unspecified).AddTicks(5440), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, null, null, null, "Simple Asset Manager Light", "246,57,21", "150,100,30", "246,20,41" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Styles_CreatedById",
                table: "Styles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Styles_ModifiedById",
                table: "Styles",
                column: "ModifiedById");
        }
    }
}

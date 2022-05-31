using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class assetversions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetVersions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<string>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(nullable: true),
                    AssetId = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ActiveVersion = table.Column<int>(nullable: false),
                    StatusUpdatedDate = table.Column<DateTimeOffset>(nullable: true),
                    Thumbnail = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DownloadUrl = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    FileSizeText = table.Column<string>(nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetVersions_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetVersions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetVersions_AspNetUsers_ModifiedById",
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
                value: new DateTimeOffset(new DateTime(2021, 4, 13, 15, 37, 40, 782, DateTimeKind.Unspecified).AddTicks(5967), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_AssetVersions_AssetId",
                table: "AssetVersions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetVersions_CreatedById",
                table: "AssetVersions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssetVersions_ModifiedById",
                table: "AssetVersions",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetVersions");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 11, 8, 5, 33, 419, DateTimeKind.Unspecified).AddTicks(9901), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

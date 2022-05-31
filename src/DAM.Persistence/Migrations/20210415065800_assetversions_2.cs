using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class assetversions_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Assets_AssetId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_AssetId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "FileSizeText",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "AssetVersionsId",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DownloadCount",
                table: "AssetVersions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "Id", "Contents", "Deleted", "EmailTemplateKey", "Subject" },
                values: new object[] { 3, "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'><html xmlns = 'http://www.w3.org/1999/xhtml' >< head >< meta http - equiv = 'Content-Type' content = 'text/html; charset=UTF-8' >< meta name = 'viewport' content = 'width=device-width, initial-scale=1.0' >< title > Notification </ title >< style type = 'text/css' >    #outlook a {padding:0;}     body{width:100% !important; -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; margin:0; padding:0;}     .ExternalClass {width:100%;}    .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {line-height: 100%;}    table td {border-collapse: collapse;}   </style></head><body><table bgcolor='#003169' border='0' cellpadding='0' cellspacing='0' style='background-color:#003169; margin:0; padding:0; width:100% !important; line-height: 100%; text-align:center; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='100%'><tbody><tr><td style='border-collapse: collapse; padding:30px 0;' valign='top'><table align='center' bgcolor='#003169' border='0' cellpadding='0' cellspacing='0' style='background-color:#003169; border:0; text-align:center; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='600'><tbody><tr><img src='%%BaseUrl%%static/media/Dubber-logo-white.6fd30ff8.png' height='60' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; margin-bottom:30px' /></tr></br></br><tr><td style='border-collapse: collapse; padding:0;' valign='top'><table bgcolor='#174375' border='0' cellpadding='0' cellspacing='0' style='border:0; text-align:left; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;  border-radius:15px;' width='100%'><tbody><!-- <tr><td style='border-collapse: collapse; padding:0;' valign='top' width='30'><img alt='' src='%%EmailImagesUrl%%rounded-top-left.gif' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; display:block;'></td><td style='border-collapse: collapse; padding:0;' valign='top' width='540'>&nbsp;</td><td style='border-collapse: collapse; padding:0;' valign='top' width='30'><img alt='' src='%%EmailImagesUrl%%rounded-top-right.gif' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; display:block;'></td></tr> --><tr><td style='border-collapse: collapse; padding:0;' valign='top' width='30'>&nbsp;</td><td style='border-collapse: collapse; padding:0;' valign='top' width='540'><table border='0' cellpadding='0' cellspacing='0' style='border:0; text-align:left; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='100%'><tbody><tr><td style='border-collapse: collapse; padding:20px 0;' valign='top'><font style='font-family:Arial; font-size:13px; color:white; line-height:18px;'>Hi <strong> %%UserName%% </strong> ,<br><br>Your %%ClientName%% account has been %%UserStatus%%. Please contact your administrator for further information. Thank you!<br><br></td></tr><tr><td style='border-collapse: collapse; padding-top:15px; text-align: right;' valign='top'><font style='font-family:Arial; font-size:10px; color:white; line-height:25px;'> Powered by <a href='https://simple.io' style='color:white'> Simple </a></font></td></tr></tbody></table></td><td style='border-collapse: collapse; padding:0;' valign='top' width='30'>&nbsp;</td></tr><tr><td style='border-collapse: collapse; padding:0;' valign='top' width='30'><img alt='' src='%%EmailImagesUrl%%rounded-bot-left.gif' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; display:block;'></td><td style='border-collapse: collapse; padding:0;' valign='top' width='540'>&nbsp;</td><td style='border-collapse: collapse; padding:0;' valign='top' width='30'><img alt='' src='%%EmailImagesUrl%%rounded-bot-right.gif' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; display:block;'></td></tr></tbody></table></td></tr></tbody></table></br><table border='0' cellpadding='0' cellspacing='0' align='center' style='border:0; text-align:left; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='600'></table></td></tr></tbody></table></body></html>", false, "USER_STATUS_CHANGE", "Your %%ClientName%% access has been %%UserStatus%%." });

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 15, 6, 57, 44, 341, DateTimeKind.Unspecified).AddTicks(112), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AssetVersionsId",
                table: "Tags",
                column: "AssetVersionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AssetVersions_AssetVersionsId",
                table: "Tags",
                column: "AssetVersionsId",
                principalTable: "AssetVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AssetVersions_AssetVersionsId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_AssetVersionsId",
                table: "Tags");

            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "AssetVersionsId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "AssetVersions");

            migrationBuilder.AddColumn<int>(
                name: "DownloadCount",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileSizeText",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Assets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 13, 15, 37, 40, 782, DateTimeKind.Unspecified).AddTicks(5967), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AssetId",
                table: "Tags",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Assets_AssetId",
                table: "Tags",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

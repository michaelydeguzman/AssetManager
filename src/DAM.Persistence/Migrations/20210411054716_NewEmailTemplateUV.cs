using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class NewEmailTemplateUV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "Id", "Contents", "Deleted", "EmailTemplateKey", "Subject" },
                values: new object[] { 1, "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'><html xmlns='http://www.w3.org/1999/xhtml'><head><meta http-equiv='Content-Type' content='text/html; charset=UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'><title>Notification</title><style type='text/css'>    #outlook a {padding:0;}     body{width:100% !important; -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; margin:0; padding:0;}     .ExternalClass {width:100%;}    .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {line-height: 100%;}    table td {border-collapse: collapse;}   </style></head><body><table bgcolor='#003169' border='0' cellpadding='0' cellspacing='0' style='background-color:#003169; margin:0; padding:0; width:100% !important; line-height: 100%; text-align:center; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='100%'><tbody><tr><td style='border-collapse: collapse; padding:30px 0;' valign='top'><table align='center' bgcolor='#003169' border='0' cellpadding='0' cellspacing='0' style='background-color:#003169; border:0; text-align:center; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='600'><tbody><tr><img alt='logo' src='%%BaseUrl%%static/media/Dubber-logo-white.6fd30ff8.png' height='60' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; margin-bottom:30px' /></tr></br></br><tr><td style='border-collapse: collapse; padding:0;' valign='top'><table bgcolor='#174375' border='0' cellpadding='0' cellspacing='0' style='border:0; text-align:left; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;  border-radius:15px;' width='100%'><tbody><!-- <tr><td style='border-collapse: collapse; padding:0;' valign='top' width='30'><img alt='' src='%%EmailImagesUrl%%rounded-top-left.gif' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; display:block;'></td><td style='border-collapse: collapse; padding:0;' valign='top' width='540'>&nbsp;</td><td style='border-collapse: collapse; padding:0;' valign='top' width='30'><img alt='' src='%%EmailImagesUrl%%rounded-top-right.gif' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; display:block;'></td></tr> --><tr><td style='border-collapse: collapse; padding:0;' valign='top' width='30'>&nbsp;</td><td style='border-collapse: collapse; padding:0;' valign='top' width='540'><table border='0' cellpadding='0' cellspacing='0' style='border:0; text-align:left; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='100%'><tbody><tr><td style='border-collapse: collapse; padding:20px 0;' valign='top'><font style='font-family:Arial; font-size:13px; color:white; line-height:18px;'>Hi <strong> %%NewUser%% </strong> ,<br><br>You have been invited to join %%ClientName%% as a new %%NewUserRole%%. Please click the button below to confirm your email. Welcome! <br><br></td></tr><tr style='font-family:Arial, sans-serif; box-sizing: border-box; font-size: 14px; margin: 0; padding: 0;font-color: white;text-align:center'><td class='content-block' style='font-family:Arial, sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 5px;' valign='top'><a href='%%ConfirmEmailUrl%%' style='background-color:#ff5d1b; border: 1px solid #EB296D; border-radius:3px; color:#ffff;display:inline-block;font-family:Arial, sans-serif;font-size:13px;line-height:30px;text-align:center;text-decoration:none; width:110px;-webkit-text-size-adjust:none;'>Confirm</a></td></tr><tr><td style='border-collapse: collapse; padding-top:15px; text-align: right;' valign='top'><font style='font-family:Arial; font-size:10px; color:white; line-height:25px;'> Powered by <a href='https://simple.io' style='color:white'> Simple </a></font></td></tr></tbody></table></td><td style='border-collapse: collapse; padding:0;' valign='top' width='30'>&nbsp;</td></tr><tr><td style='border-collapse: collapse; padding:0;' valign='top' width='30'><img alt='' src='%%EmailImagesUrl%%rounded-bot-left.gif' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; display:block;'></td><td style='border-collapse: collapse; padding:0;' valign='top' width='540'>&nbsp;</td><td style='border-collapse: collapse; padding:0;' valign='top' width='30'><img alt='' src='%%EmailImagesUrl%%rounded-bot-right.gif' style='outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0; display:block;'></td></tr></tbody></table></td></tr></tbody></table></br><table border='0' cellpadding='0' cellspacing='0' align='center' style='border:0; text-align:left; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='600'></table></td></tr></tbody></table></body></html>", false, "USER_VERIFICATION", "You have been invited to join %%ClientName%%." });

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 11, 5, 47, 15, 508, DateTimeKind.Unspecified).AddTicks(3410), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 10, 12, 9, 59, 605, DateTimeKind.Unspecified).AddTicks(3338), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

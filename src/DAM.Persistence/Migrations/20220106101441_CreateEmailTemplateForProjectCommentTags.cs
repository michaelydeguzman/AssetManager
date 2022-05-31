using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class CreateEmailTemplateForProjectCommentTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "Id", "Category", "Classification", "Contents", "Deleted", "EmailTemplateKey", "EmailTemplateName", "Message", "RecipientType", "Subject" },
                values: new object[] { 18, 3, 1, "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>Announcement</title> <style type='text/css'> #outlook a { padding: 0; } body { width: 100% !important; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; margin: 0; padding: 0; } .ExternalClass { width: 100%; } .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div { line-height: 100%; } table td { border-collapse: collapse; height: 20px } p, span { font-family: Calibri; font-size: 16px; color: black; } .italic-text { font-style: italic } </style> </head> <body> <table border='0' cellpadding='0' cellspacing='0' style='margin:0; padding:0; width:100% !important; line-height: 100%; text-align:center; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='100%'> <tbody> <tr> <td style='border-collapse: collapse; padding:30px 0;' valign='top'> <table align='center' border='0' cellpadding='0' cellspacing='0' style='border:0; text-align:center; border-collapse:collapse; mso-table-lspace:1pt; mso-table-rspace:0pt;' width='800'> <tbody> <tr></tr> </br> </br> <tr> <td style='border-collapse: collapse; padding:0;' valign='top'> <table border='0' cellpadding='0' cellspacing='0' style='border:0;text-align:left;border-collapse:collapse;mso-table-lspace:0pt;mso-table-rspace:0pt;border-radius:15px;background: hsla(0,0%,100%,.058)' width='100%'> <tbody> <tr> <td style='border-collapse: collapse; padding:0px 0px 20px 0px;' valign='top' width='100%'> <img src='%%EmailHeader%%' style='width: 100%; height: auto; outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; border:0;' /> </td> </tr> <tr> <td style='border-collapse: collapse; padding:0;' valign='top' width='100%'> <table border='0' cellpadding='0' cellspacing='0' style='border:0; text-align:left; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;' width='100%'> <tbody> <tr> <td style='border-collapse: collapse;' valign='middle'> <p>Dear %%To%%,</p> </td> </tr> <tr> <td style='border-collapse: collapse;' valign='middle'> <p>%%Message%%</p> </td> </tr> <tr> <td> %%ProjectTable%% </td> </tr> <tr> <td style='border-collapse: collapse;' valign='middle'> <p>Cheers,</p> <p>- Simple Asset Manager</p> <br /> </td> </tr> <tr style='border-top: solid 1px'> <td style='border-collapse: collapse;' valign='middle'> <p class='italic-text'>This email is automated, please do not reply to this notification.</p> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> </br> </td> </tr> </tbody> </table> </body> </html>", false, "PROJECT_COMMENT_TAGGED", "Project Comment Tagged", "Someone mentioned you in a project comment.", "Project Owner, Project Collaborator", "Someone mentioned you in a project comment." });

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 18);

            
        }
    }
}

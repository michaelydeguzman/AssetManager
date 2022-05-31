using DAM.Application.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using static DAM.Application.Services.Interfaces.IHelperService;

namespace DAM.Application.Services
{
    public class HelperService : IHelperService
    {
        private EmailConfigDetails SetEmailConfigs()
        {
            EmailConfigDetails emailConfigDetails = new EmailConfigDetails();
            emailConfigDetails.SmtpHost = ConfigurationManager.AppSettings["SMTPHost"];
            emailConfigDetails.SmtpPassword = ConfigurationManager.AppSettings["SMTPPassword"];
            emailConfigDetails.SmtpUserName = ConfigurationManager.AppSettings["SMTPUserName"];
            emailConfigDetails.SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            emailConfigDetails.DefaultFromEmailAddress = ConfigurationManager.AppSettings["EmailFrom"];

            return emailConfigDetails;
        }

        /// <summary>
        /// Encodes the using M d5.
        /// </summary>
        /// <param name="originalString">The original string.</param>
        /// <returns></returns>
        public string EncodeUsingMD5(string originalString)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = UnicodeEncoding.Default.GetBytes(originalString);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            string passHash = BitConverter.ToString(encodedBytes);
            return passHash.Replace("-", string.Empty);
        }

        public string GenerateVerificationCode(int stringLength)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bitCount = (stringLength * 6);
                var byteCount = ((bitCount + 7) / 8); // rounded up
                var bytes = new byte[byteCount];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }

        public string GetFileSize(string fileName)
        {
            // Load all suffixes in an array  
            string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

            // Full file name  

            FileInfo file = new FileInfo(fileName);

            if (file.Exists)
            {
                int counter = 0;
                decimal number = (decimal)fileName.Length;
                while (Math.Round(number / 1024) >= 1)
                {
                    number = number / 1024;
                    counter++;
                }

                return string.Format("{0:n1}{1}", number, suffixes[counter]);
            }
            else
            {
                throw new Exception("File does not exist.");
            }
        }

        public string GetFileSize(long fileSize)
        {
            // Load all suffixes in an array  
            string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

            int counter = 0;
            decimal number = fileSize;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }

            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }

        public string GetJsonString(object details)
        {
            return JsonConvert.SerializeObject(details);
        }

        public DateTimeOffset GetDateWithBusinessDays(DateTimeOffset startDate, int businessDays)
        {
            if (businessDays < 0)
            {
                throw new ArgumentException("days cannot be negative", "days");
            }

            if (businessDays == 0) return startDate;

            if (startDate.DayOfWeek == DayOfWeek.Saturday)
            {
                startDate = startDate.AddDays(2);
                businessDays -= 1;
            }
            else if (startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startDate = startDate.AddDays(1);
                businessDays -= 1;
            }

            startDate = startDate.AddDays(businessDays / 5 * 7);
            int extraDays = businessDays % 5;

            if ((int)startDate.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return startDate.AddDays(extraDays);
        }
    }
}

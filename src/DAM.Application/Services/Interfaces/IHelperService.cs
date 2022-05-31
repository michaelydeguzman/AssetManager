using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Services.Interfaces
{
    public interface IHelperService
    {
        class EmailConfigDetails
        {
            public string DefaultFromEmailAddress { get; set; }
            public string SmtpHost { get; set; }
            public int SmtpPort { get; set; }
            public bool UseDefaultCredentials { get; set; }
            public string SmtpUserName { get; set; }
            public string SmtpPassword { get; set; }
            public string FromEmailAddress { get; set; }
        }

        public string EncodeUsingMD5(string originalString);

        public string GenerateVerificationCode(int stringLength);

        public string GetFileSize(string fileName);

        public string GetFileSize(long fileSize);

        public string GetJsonString(object details);

        public DateTimeOffset GetDateWithBusinessDays(DateTimeOffset startDate, int businessDays);
        
    }
}

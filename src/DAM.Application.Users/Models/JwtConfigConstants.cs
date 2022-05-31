using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Models
{
    public class JwtConfigConstants
    {
        public const string Issuer = "MVS";
        public const string Audience = "ApiUser";
        public const string Key = "12345678Simple5%";
        public const string AuthSchemes = JwtBearerDefaults.AuthenticationScheme;
    }
}

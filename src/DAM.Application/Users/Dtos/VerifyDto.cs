using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAM.Application.Users.Dtos
{
    public class VerifyDto
    {
        public string Email { get; set; }

        public string VerificationCode { get; set; }

        public bool Accepted { get; set; }

        public string ErrorMessage { get; set; }
    }
}

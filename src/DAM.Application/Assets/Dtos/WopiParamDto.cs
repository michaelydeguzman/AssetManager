using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class WopiParamDto
    {
        public string AccessToken { get; set; }
        public int AccessTokenTTL { get; set; }
        public string WopiUrlSrc { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class Theme: EntityBase
    {
        public string Name { get; set; }

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string TertiaryColor { get; set; }

        public bool Deleted { get; set; }

        public bool IsApplied { get; set; }
        public string LogoFileName { get; set; }
        public string LogoUrl { get; set; }
        public string LogoKey { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Styles.Dtos
{
    public class ThemeDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string TertiaryColor { get; set; }

        public bool? Deleted { get; set; }

        public bool? IsApplied { get; set; }

        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }

        public string ModifiedById { get; set; }
        public string ModifiedByName { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string LogoFileName { get; set; }
        public string LogoUrl { get; set; }
        public byte[] Logo { get; set; }
    }
}

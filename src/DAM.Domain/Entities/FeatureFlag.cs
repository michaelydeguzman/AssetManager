using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAM.Domain.Entities
{
    public class FeatureFlag
    {
        [Key]
        public int Id { get; set; }

        public int FeatureFlagNumber { get; set; }

        public string FeatureFlagName { get; set; }

        public bool IsActivated { get; set; }
    }
}

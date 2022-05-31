using DAM.Application.Accounts.Dtos;
using DAM.Application.Assets;
using DAM.Application.Assets.Dtos;
using DAM.Application.CountryRegions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.FeatureFlags.Dtos
{
    public class FeatureFlagDto
    {
        public int Id { get; set; }

        public int FeatureFlagNumber { get; set; }

        public string FeatureFlagName { get; set; }

        public bool IsActivated { get; set; }
    }
}

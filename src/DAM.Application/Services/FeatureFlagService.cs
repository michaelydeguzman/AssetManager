using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using DAM.Application.FeatureFlags.Enums;
using DAM.Application.Services.Interfaces;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DAM.Application.Services
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private IDbContext _dbContext;

        public FeatureFlagService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool IsApprovalsEnabled()
        {
            var result = _dbContext.FeatureFlags.First(x => x.FeatureFlagNumber == (int)FeatureFlagEnum.Approvals);
            return result.IsActivated;
        }

        public bool IsPartnershipEnabled()
        {
            var result = _dbContext.FeatureFlags.First(x => x.FeatureFlagNumber == (int)FeatureFlagEnum.Partnership);
            return result.IsActivated;
        }

        public bool isAssetVersioningEnabled()
        {
            var result = _dbContext.FeatureFlags.First(x => x.FeatureFlagNumber == (int)FeatureFlagEnum.AssetVersioning);
            return result.IsActivated;
        }

        public bool IsVideoIndexerEnabled()
        {
            var result = _dbContext.FeatureFlags.First(x => x.FeatureFlagNumber == (int)FeatureFlagEnum.VideoIndexer);
            return result.IsActivated;
        }
    }
}

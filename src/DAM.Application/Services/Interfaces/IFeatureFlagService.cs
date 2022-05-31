using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Application.Services.Interfaces
{
    public interface IFeatureFlagService
    {
       
        bool IsApprovalsEnabled();

        bool IsPartnershipEnabled();

        bool isAssetVersioningEnabled();

        bool IsVideoIndexerEnabled();
    }
}

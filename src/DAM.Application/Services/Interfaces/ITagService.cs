using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Application.Services.Interfaces
{
    public interface ITagService
    {
        class TagServiceConfig
        {
            public string CognitiveEndpoint { get; set; }
            public string CognitiveSubscriptionkey { get; set; }
        }

        Task<string[]> GetImageTags(IConfiguration configuration, Stream stream);
    }
}

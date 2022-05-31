using DAM.Application.Services.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DAM.Application.Services.Interfaces.ITagService;

namespace DAM.Application.Services
{
    public class TagService : ITagService
    {
        private TagServiceConfig SetTagServiceConfig(IConfiguration configuration)
        {
            return new TagServiceConfig()
            {
                CognitiveEndpoint = configuration["CognitiveEndpoint"],
                CognitiveSubscriptionkey = configuration["CognitiveSubscriptionkey"]
            };
        }

        private ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public async Task<string[]> GetImageTags(IConfiguration configuration, Stream stream)
        {
            TagServiceConfig config = SetTagServiceConfig(configuration);

            var mStream = stream;

            mStream.Seek(0, SeekOrigin.Begin);

            ComputerVisionClient client = Authenticate(config.CognitiveEndpoint, config.CognitiveSubscriptionkey);

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            ImageAnalysis results = await client.AnalyzeImageInStreamAsync(mStream, features);

            var tags = results.Tags.Where(x => x.Confidence >= .8).Select(x => x.Name).ToArray();
            var categories = results.Categories.Where(x => x.Score >= .8).Select(x => x.Name).ToArray();
            var objects = results.Objects.Where(x => x.Confidence >= .8).Select(x => x.ObjectProperty).ToArray();
            var brands = results.Brands.Where(x => x.Confidence >= .8).Select(x => x.Name).ToArray();
            var celebrities = results.Categories.Where(x => x.Detail?.Celebrities != null && x.Score >= .8).Select(x => x.Name).ToArray();
            var landmarks = results.Categories.Where(x => x.Detail?.Landmarks != null && x.Score >= .8).Select(x => x.Name).ToArray();

            return tags.Concat(categories).Concat(objects).Concat(brands).Concat(celebrities).Concat(landmarks).Distinct().ToArray();
        }
    }
}

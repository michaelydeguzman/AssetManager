using DAM.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace DAM.Application.Services.Interfaces
{
    public interface IConversionService
    {
        class ConversionServiceConfig
        {
            public int ThumbnailWidth { get; set; }
            public int ThumbnailHeight { get; set; }

            public string FilePath { get; set; }
            public string FFMpeg { get; set; }

            public string DocToPDFApiUrl { get; set; }
        }

        Stream GetImageThumbnail(IConfiguration configuration, Stream stream, string extensionName);

        Stream GetMagickImageThumbnail(IConfiguration configuration, Stream stream, string extensionName);
        Stream GetPDFThumbnail(IConfiguration configuration, Stream stream);

        Stream GetVideoThumbnail(IConfiguration configuration, Stream stream, string videoFile, string key);

        Stream GetDocumentThumbnail(Stream stream);

        Stream GetPDFThumbnail(Stream stream);

        Stream GetXLSThumbnail(Stream stream);

        Stream GetPPTThumbnail(Stream stream);

        Stream GetPNGFromTIFF(Stream stream);

        Stream ApplyWatermark(Watermark watermark, Stream assetsm, Stream watermarksm);

        Dictionary<string, Stream> ConvertPDFPagesToPNG(Stream input, string fileName, bool showWatermark = false, Stream watermarksm = null, Watermark watermark = null);

        Dictionary<string, Stream> ConvertPDFPagesToJpeg(Stream input, string fileName, bool showWatermark = false, Stream watermarksm = null, Watermark watermark = null);

        Stream ConvertImageToPdf(Stream input, string extension);

        HttpResponseMessage ConvertOfficeToPDF(IConfiguration configuration, Stream stream, string fileName, string extension);
    }
}

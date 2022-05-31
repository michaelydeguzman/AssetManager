using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DAM.Application.Services.Interfaces
{
    public interface IAzureStorageService
    {
        class AzureStorageServiceConfig
        {
            public string ConnectionString { get; set; }
            public string AccountName { get; set; }
            public string AccountKey { get; set; }
            public string AssetContainer { get; set; }
            public string AssetPreviewContainer { get; set; }
            public string UserContainer { get; set; }
            public string WatermarkContainer { get; set; }
            public string LogoContainer { get; set; }
            public string EmailHeaderContainer { get; set; }
        }

        string GetBlobSasUri(IConfiguration configuration, string blobName, string fileName, string fileType, bool isThumbnail, bool? getDownload = false);

        string GetBlobSasUri(IConfiguration configuration, string blobName);

        BlobClient GetBlobFileBytes(IConfiguration configuration, string blobName);

        Stream GetBlobStream(IConfiguration configuration, string blobName, out string contentType);

        BlobProperties UploadFileToStorage(IConfiguration configuration, Stream fileStream, string fileName, out Uri blobUri, bool? isThumbnail = false, bool? isUserImage = false);
        BlobProperties UpdateFileFromStorage(IConfiguration configuration, Stream fileStream, string fileName, out Uri blobUri, bool? isThumbnail = false);

        BlobProperties SaveDefaultWatermark(IConfiguration configuration, Stream fileStream, string fileName, out Uri blobUri);
        Stream GetWatermarkBlobStream(IConfiguration configuration, string blobName);
        string GetWatermarkUrl(IConfiguration configuration, string fileName);

        BlobProperties SaveLogo(IConfiguration configuration, Stream fileStream, string fileName, out Uri blobUri);
        string GetLogoUrl(IConfiguration configuration, string fileName);

        void CopyBlob(IConfiguration configuration, string blobName, string newKey, bool IsThumbnail = false);

        string GetEmailHeaderUrl(IConfiguration configuration, string fileName);
    }
}

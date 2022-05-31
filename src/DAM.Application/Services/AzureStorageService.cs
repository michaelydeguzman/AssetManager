using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using DAM.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static DAM.Application.Services.Interfaces.IAzureStorageService;

namespace DAM.Application.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private AzureStorageServiceConfig SetAzureStorageConfig(IConfiguration configuration)
        {
            return new AzureStorageServiceConfig()
            {
                ConnectionString = configuration["ConnectionStrings:AzureBlobConnectionString"],
                AccountName = configuration["AccountName"],
                AccountKey = configuration["AccountKey"],
                AssetContainer = configuration["AssetContainer"],
                AssetPreviewContainer = configuration["AssetPreviewContainer"],
                UserContainer = configuration["UserContainer"],
                WatermarkContainer = configuration["WatermarkContainer"],
                LogoContainer = configuration["LogoContainer"],
                EmailHeaderContainer = configuration["EmailHeadersContainer"]
            };
        }

        public string GetBlobSasUri(IConfiguration configuration, string blobName, string fileName, string fileType, bool isThumbnail, bool? getDownload = false)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
            BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            BlobContainerClient container = service.GetBlobContainerClient(isThumbnail ? config.AssetPreviewContainer : config.AssetContainer);

            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddYears(3), //forShare.Value ? DateTimeOffset.UtcNow.AddDays(1) : DateTimeOffset.UtcNow.AddYears(3),
                Protocol = SasProtocol.HttpsAndHttp,

            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            if (isThumbnail)
            {
                sasBuilder.ContentType = "image/jpeg";
            }
            else if (getDownload.Value)
            {
                sasBuilder.ContentDisposition = "attachment; filename=" + fileName;
            }
            else
            {
                var contentType = "text/plain";
                if (fileType != ""
                    && fileType != "text/xml"
                    && fileType != "text/css"
                    && fileType != "text/css"
                    && fileType != "text/javascript"
                    && fileType != "application/json")
                {
                    contentType = fileType;
                }
                sasBuilder.ContentType = contentType;
            }


            // Use the key to get the SAS token.
            string sasToken = sasBuilder.ToSasQueryParameters(storageCredentials).ToString();

            return container.GetBlockBlobClient(blobName).Uri + "?" + sasToken;
        }

        public BlobClient GetBlobFileBytes(IConfiguration configuration, string blobName)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
            BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            BlobContainerClient container = service.GetBlobContainerClient(config.AssetContainer);

            BlobClient blobClient = new BlobClient(config.ConnectionString, container.Name, blobName);

            // Read the blob
            //var b = blobClient.OpenRead();
            //byte[] bytes = new byte[b.Length];
            //b.Read(bytes, 0, (int)b.Length);
            return blobClient;
        }

        public Stream GetBlobStream(IConfiguration configuration, string blobName, out string contentType)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
            BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            BlobContainerClient container = service.GetBlobContainerClient(config.AssetContainer);

            BlobClient blobClient = new BlobClient(config.ConnectionString, container.Name, blobName);
            Stream sm = new MemoryStream();

            contentType = blobClient.GetProperties().Value.ContentType;
            blobClient.DownloadToAsync(sm).GetAwaiter().GetResult();

            return sm;
        }

        public BlobProperties UploadFileToStorage(IConfiguration configuration, Stream fileStream, string fileName, out Uri blobUri, bool? isThumbnail = false, bool? isUserImage = false)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);

            // Create a URI to the blob
            blobUri = new Uri("https://" +
                            config.AccountName +
                            ".blob.core.windows.net/" +
                            (isThumbnail.Value ? config.AssetPreviewContainer : config.AssetContainer) +
                            "/" + fileName);

            if ((bool)isUserImage)
            {
                blobUri = new Uri("https://" +
                            config.AccountName +
                            ".blob.core.windows.net/" +
                            config.UserContainer +
                            "/" + fileName);
            }

            // Create StorageSharedKeyCredentials object by reading
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            // Create the blob client.
            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            // Delete file from blob
            blobClient.DeleteIfExists();

            fileStream.Position = 0;

            // Upload the file
            blobClient.Upload(fileStream);

            return blobClient.GetProperties();
        }

        public string GetBlobSasUri(IConfiguration configuration, string blobName)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
            BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            BlobContainerClient container = service.GetBlobContainerClient(config.UserContainer);

            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(1),
                Protocol = SasProtocol.HttpsAndHttp,

            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            sasBuilder.ContentType = "image/jpeg";

            // Use the key to get the SAS token.
            string sasToken = sasBuilder.ToSasQueryParameters(storageCredentials).ToString();

            return container.GetBlockBlobClient(blobName).Uri + "?" + sasToken;
        }


        public BlobProperties SaveDefaultWatermark(IConfiguration configuration, Stream fileStream, string fileName, out Uri blobUri)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);

            // Create a URI to the blob
            blobUri = new Uri("https://" +
                            config.AccountName +
                            ".blob.core.windows.net/" +
                            config.WatermarkContainer +
                            "/" + fileName);

            // Create StorageSharedKeyCredentials object by reading
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            // Create the blob client.
            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            // Delete file from blob
            blobClient.DeleteIfExists();

            fileStream.Position = 0;

            // Upload the file
            blobClient.Upload(fileStream);

            return blobClient.GetProperties();
        }

        public string GetWatermarkUrl(IConfiguration configuration, string fileName)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
            BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            BlobContainerClient container = service.GetBlobContainerClient(config.WatermarkContainer);

            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddYears(3), //forShare.Value ? DateTimeOffset.UtcNow.AddDays(1) : DateTimeOffset.UtcNow.AddYears(3),
                Protocol = SasProtocol.HttpsAndHttp,

            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            sasBuilder.ContentType = $"image/{Path.GetExtension(fileName)}";

            // Use the key to get the SAS token.
            string sasToken = sasBuilder.ToSasQueryParameters(storageCredentials).ToString();

            return container.GetBlockBlobClient(fileName).Uri + "?" + sasToken;
        }

        public Stream GetWatermarkBlobStream(IConfiguration configuration, string blobName)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
            BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            BlobContainerClient container = service.GetBlobContainerClient(config.WatermarkContainer);

            BlobClient blobClient = new BlobClient(config.ConnectionString, container.Name, blobName);
            Stream sm = new MemoryStream();

            blobClient.DownloadToAsync(sm).GetAwaiter().GetResult();

            return sm;
        }


        public void CopyBlob(IConfiguration configuration, string blobName, string newKey, bool IsThumbnail = false) 
        {

            try
            {
                AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
                BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
                StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

                BlobContainerClient container = service.GetBlobContainerClient(IsThumbnail ? config.AssetPreviewContainer : config.AssetContainer);

                BlobClient blobClient = new BlobClient(config.ConnectionString, container.Name, blobName);

                // Create a BlobClient representing the source blob to copy.
                BlobClient sourceBlob = container.GetBlobClient(blobName);

                // Ensure that the source blob exists.
                if (blobClient.Exists())
                {
                    // Lease the source blob for the copy operation 
                    // to prevent another client from modifying it.
                    BlobLeaseClient lease = sourceBlob.GetBlobLeaseClient();

                    // Specifying -1 for the lease interval creates an infinite lease.
                    lease.Acquire(TimeSpan.FromSeconds(-1));

                    // Get the source blob's properties and display the lease state.
                    BlobProperties sourceProperties = sourceBlob.GetProperties();

                    // Get a BlobClient representing the destination blob with a unique name.
                    BlobClient destBlob =
                        container.GetBlobClient(newKey);

                    // Start the copy operation.
                    destBlob.StartCopyFromUri(sourceBlob.Uri);

                    // Update the source blob's properties.
                    sourceProperties = sourceBlob.GetProperties();

                    if (sourceProperties.LeaseState == LeaseState.Leased)
                    {
                        // Break the lease on the source blob.
                        lease.Break();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BlobProperties UpdateFileFromStorage(IConfiguration configuration, Stream fileStream, string fileName, out Uri blobUri, bool? isThumbnail = false)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);

            // Create a URI to the blob
            var fileUri = new Uri("https://" +
                            config.AccountName +
                            ".blob.core.windows.net/" +
                            (isThumbnail.Value ? config.AssetPreviewContainer : config.AssetContainer) +
                            "/" + fileName);

            // Create StorageSharedKeyCredentials object by reading
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            // Create the blob client.
            BlobClient blobClient = new BlobClient(fileUri, storageCredentials);

            // Delete file from blob
            blobClient.DeleteIfExists();

            // Upload updated file ro blob
            BlobProperties props = UploadFileToStorage(configuration, fileStream, fileName, out Uri thumbnailUri, isThumbnail);
            blobUri = thumbnailUri;

            return props;
        }

        public BlobProperties SaveLogo(IConfiguration configuration, Stream fileStream, string fileName, out Uri blobUri)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);

            // Create a URI to the blob
            blobUri = new Uri("https://" +
                            config.AccountName +
                            ".blob.core.windows.net/" +
                            config.LogoContainer +
                            "/" + fileName);

            // Create StorageSharedKeyCredentials object by reading
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            // Create the blob client.
            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            // Delete file from blob
            blobClient.DeleteIfExists();

            fileStream.Position = 0;

            // Upload the file
            blobClient.Upload(fileStream);

            return blobClient.GetProperties();
        }

        public string GetLogoUrl(IConfiguration configuration, string fileName)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
            BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            BlobContainerClient container = service.GetBlobContainerClient(config.LogoContainer);

            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddYears(3), //forShare.Value ? DateTimeOffset.UtcNow.AddDays(1) : DateTimeOffset.UtcNow.AddYears(3),
                Protocol = SasProtocol.HttpsAndHttp,
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            sasBuilder.ContentType = $"image/{Path.GetExtension(fileName)}";

            // Use the key to get the SAS token.
            string sasToken = sasBuilder.ToSasQueryParameters(storageCredentials).ToString();

            return container.GetBlockBlobClient(fileName).Uri + "?" + sasToken;
        }

        public string GetEmailHeaderUrl(IConfiguration configuration, string fileName)
        {
            AzureStorageServiceConfig config = SetAzureStorageConfig(configuration);
            BlobServiceClient service = new BlobServiceClient(config.ConnectionString);
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(config.AccountName, config.AccountKey);

            BlobContainerClient container = service.GetBlobContainerClient(config.EmailHeaderContainer);

            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddYears(3), //forShare.Value ? DateTimeOffset.UtcNow.AddDays(1) : DateTimeOffset.UtcNow.AddYears(3),
                Protocol = SasProtocol.HttpsAndHttp,
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            sasBuilder.ContentType = $"image/png";

            // Use the key to get the SAS token.
            string sasToken = sasBuilder.ToSasQueryParameters(storageCredentials).ToString();

            return container.GetBlockBlobClient(fileName).Uri + "?" + sasToken;
        }

    }
}


// -----------------------------------------------------------------------
// <copyright file="AzureStorage.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Options;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class AzureStorage : IAzureStorage
    {
        private readonly AzureStorageOptions _sOpts;
        private readonly IBaseDapperRepository _baseDapperRepository;

        public AzureStorage(
            IOptions<AzureStorageOptions> sOpts,
            IBaseDapperRepository baseDapperRepository
            )
        {
            this._sOpts = sOpts.Value;
            this._baseDapperRepository = baseDapperRepository;
        }

        /// <inheritdoc/>
        public async Task<Tuple<string, string, DateTime>> UploadImageAsync(Stream file, string containerName = "img", string prevBlobName = null, string ext = "jpg", string contentType = "image/jpeg")
        {
            try
            {
                // Retrieve storage account from connection string.
                var storageAccount = CloudStorageAccount.Parse(
                    this._sOpts.StorageConnectionString);

                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                var container = blobClient.GetContainerReference(containerName);

                // Generates Random Blob Name
                var randomName = $"{Guid.NewGuid().ToString()}.{ext}";
                var blockBlob = container.GetBlockBlobReference(randomName);

                // Sets the content type to image
                blockBlob.Properties.ContentType = contentType;

                // Deletes previous Blob from storage
                if (!string.IsNullOrEmpty(prevBlobName))
                {
                    var prevBlockBlob = container.GetBlobReference(prevBlobName);
                    if (await prevBlockBlob.ExistsAsync())
                    {
                        await prevBlockBlob.DeleteAsync();
                    }
                }

                // Uploads blob to storage
                await blockBlob.UploadFromStreamAsync(file);

                var fullUrl = blockBlob.Uri.AbsoluteUri;// $"{this._sOpts.StorageImageBaseUrl}{randomName}";
                DateTime imageTakenDate = new DateTime(1, 1, 1);
                // Tries to fetch taken date 
                try
                {
                    /* using (ExifReader reader = new ExifReader(file))
                     {
                         DateTime dateInfo;
                         if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out dateInfo))
                         {
                             imageTakenDate = dateInfo;
                         }
                     }*/
                }
                catch (Exception elEx)
                {
#if DEBUG
                    Console.WriteLine(elEx.Message);
#endif
                    // Fail silently on purpose
                }

                return new Tuple<string, string, DateTime>(randomName, fullUrl, imageTakenDate);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                throw ex;
            }
        }

        public async Task<Tuple<string, string, DateTime>> UploadFileAsync(Stream file, string ext, string contentType)
        {
            try
            {
                // Retrieve storage account from connection string.
                var storageAccount = CloudStorageAccount.Parse(
                    this._sOpts.StorageConnectionString);

                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                var container = blobClient.GetContainerReference("files");

                // Generates Random Blob Name
                var randomName = $"{Guid.NewGuid().ToString()}.{ext}";
                var blockBlob = container.GetBlockBlobReference(randomName);

                // Sets the content type to image
                blockBlob.Properties.ContentType = contentType;

                // Uploads blob to storage
                await blockBlob.UploadFromStreamAsync(file);

                var fullUrl = blockBlob.Uri.AbsoluteUri;

                return new Tuple<string, string, DateTime>(randomName, fullUrl, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                throw ex;
            }
        }

        /// <summary>
        ///     Deletes a file from azure storage
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImageAsync(string containerName = "img", string blobName = null)
        {
            try
            {
                string query = @"SELECT 
		                        (SELECT COUNT(*) FROM TicketAttachments WHERE BlobName = @blobName)
		                        +
		                        (SELECT COUNT(*) FROM InspectionItemAttachments WHERE BlobName = @blobName)
                                +           
		                        (SELECT COUNT(*) FROM WorkOrderAttachments WHERE BlobName = @blobName)
		                        +
		                        (SELECT COUNT(*) FROM CleaningReportItemAttachments WHERE BlobName = @blobName)";

                var pars = new DynamicParameters();
                pars.Add("@blobName", blobName);

                var attCount = await this._baseDapperRepository.QuerySingleOrDefaultAsync<int>(query, pars);
                if (attCount <= 1)
                {
                    // Retrieves storage account from connection string.
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                        _sOpts.StorageConnectionString);

                    // Creates the blob client.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    // Retrieves reference to a previously created container.
                    CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                    // Deletes the Blob objects if exists
                    if (!string.IsNullOrEmpty(blobName))
                    {
                        var blobObj = container.GetBlobReference(blobName);
                        if (await blobObj.ExistsAsync())
                        {
                            await blobObj.DeleteAsync();
                        }

                        return true;
                    }
                }
                // Returns false if file didn't exist (for some reason i guess)
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

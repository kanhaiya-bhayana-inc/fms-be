using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure;
using FMS.Services.AzueFileUploadAPI.Model.Dto;

namespace FMS.Services.AzueFileUploadAPI.Services
{
    public class AzureUploadFileService : IAzureUploadFileService
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;

        public AzureUploadFileService(IConfiguration configuration)
        {
            _storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
        }
        public async Task<AzureBlobResponseDto> UploadAsync(IFormFile blob)
        {
            AzureBlobResponseDto response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            try
            {
                BlobClient client = container.GetBlobClient(blob.FileName);

                await using (Stream? data = blob.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }

                response.Status = $"File Uploaded Successfully";
                response.Error = false;
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                response.Status = $"Failed to upload, File with name {blob.FileName} already exists. Please use another name to store your file.";
                response.Error = true;

                return response;
            }
            catch (RequestFailedException ex)
            {
                response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
                response.Error = true;
                return response;
            }

            return response;
        }
    }
}



using Azure.Storage.Blobs;
using SkyStore.Interfaces; 
namespace SkyStore.Services
{
    public class AzureStorageService : IStorageProvider
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "contenedorfortest";

        public AzureStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string userId)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobName = $"{userId}/{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString(); // Esto es lo que se guardará en Path
        }
    }
}

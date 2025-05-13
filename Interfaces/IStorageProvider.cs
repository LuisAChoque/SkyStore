namespace SkyStore.Interfaces
{
    public interface IStorageProvider
    {
        Task<string> UploadFileAsync(IFormFile file, string containerName);
        /*Task<string> GetFileUrlAsync(string fileName, string containerName);
        Task DeleteFileAsync(string fileName, string containerName);*/
    }
}
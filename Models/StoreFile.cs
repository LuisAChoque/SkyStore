namespace SkyStore.Models
{
    public class StoreFile
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? Path { get; set; }
        public long SizeInBytes { get; set; }
        public DateTime UploadAt { get; set; }
    }
}

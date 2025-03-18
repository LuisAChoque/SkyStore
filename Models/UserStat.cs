namespace SkyStore.Models
{
    public class UserStat
    {
        public string? Username { get; set; }
        public long StorageUsed { get; set; } = 0; // Espacio usado por el usuario
    }
}

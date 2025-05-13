namespace SkyStore.Interfaces
{
    public interface IStorageFactory
    {
        IStorageProvider CreateStorage(string providerName);
    }
}

using Microsoft.Extensions.DependencyInjection;
using SkyStore.Interfaces;
using SkyStore.Services;

namespace SkyStore.Factories
{
    public class StorageFactory : IStorageFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StorageFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStorageProvider CreateStorage(string providerName)
        {
            return providerName switch
            {
                //"AWS" => _serviceProvider.GetRequiredService<AwsStorageService>(),
                "Azure" => _serviceProvider.GetRequiredService<AzureStorageService>(),
                _ => throw new ArgumentException($"Unknown storage provider: {providerName}")
            };
        }
    }
}

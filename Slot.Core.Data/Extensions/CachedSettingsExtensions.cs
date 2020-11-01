using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slot.Core.Diagnostics;

namespace Slot.Core.Data.Extensions
{
    public static class CachedSettingsExtensions
    {
        public static IServiceCollection AddCachedSettings(this IServiceCollection services)
        {
            return services.AddSingleton(serviceProvider =>
                    {
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var monitoring = scope.ServiceProvider.GetRequiredService<IMonitoringService>();
                            var logger = scope.ServiceProvider
                                              .GetRequiredService<ILoggerFactory>()
                                              .CreateLogger<CachedSettings>();
                            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                            var caching = new CachedSettings(logger, configuration);
                            monitoring.TrackDependency("CachedSettings", "Load", caching.Load);
                            return caching;
                        }
                    });
        }
    }
}

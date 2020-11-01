using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using Slot.Core.Data.Extensions;
using Slot.Core.Diagnostics;
using Slot.Core.Modules.Infrastructure;
using Slot.Core.Services.Abstractions;
using Slot.Core.Services.Validation;


namespace Slot.Core.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSlotCore(this IServiceCollection services)
        {
            services.AddScoped<IBonusService, BonusService>()
                    .AddScoped<IGameHistoryService, GameHistoryService>()
                    .AddScoped<IGameTransactionService, GameTransactionService>()
                    .AddScoped<IPayoutService, PayoutService>()
                    .AddScoped<IGameService, GameService>()
                    .AddScoped<IValidationStrategy, ValidationStrategy>()
                    .AddScoped<IUserService, UserService>()
                    .AddScoped<IAuthenticationService, AuthenticationService>()
                    .AddScoped<ITournamentService, TournamentService>()
                    .AddScoped<IMonitoringService, MonitoringService>()
                    .AddSingleton<IDatabaseManager, DatabaseManager>()
                    .AddCachedSettings()
                    .AddSingleton(GameModuleCollectionFactory.Create)
                    .AddHttpClient();

            return services;
        }
    }
}

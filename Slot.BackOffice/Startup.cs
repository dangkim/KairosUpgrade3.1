using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Slot.BackOffice.Configs.AppSettings;
using Slot.BackOffice.Configs.Authentication;
using Slot.BackOffice.Data.History;
using Slot.BackOffice.Data.History.HistoryDecode;
using Slot.BackOffice.Data.Repositories;
using Slot.BackOffice.Filters;
using Slot.BackOffice.HttpClients;
using Slot.Core.Data;
using Slot.Core.Data.Extensions;
using Slot.Core.Diagnostics;
using System.Text;

namespace Slot.BackOffice
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });

            app.UseAuthentication();
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("swagger/v1/swagger.json", "Kairos BackOffice API v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseMemcached();

            app.UseMvc();
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureCoreServices(services);
            services.AddApplicationInsightsTelemetry();
        }

        public void ConfigureUatServices(IServiceCollection services)
        {
            ConfigureCoreServices(services);
        }

        public void ConfigureStagingServices(IServiceCollection services)
        {
            ConfigureCoreServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureCoreServices(services);
        }

        private void ConfigureTransientServices(IServiceCollection services)
        {

        }

        private void ConfigureScopedServices(IServiceCollection services)
        {
            services
                .AddScoped<IMonitoringService, MonitoringService>()
                .AddScoped<DashboardRepository>()
                .AddScoped<MembersRepository>()
                .AddScoped<ReportsRepository>()
                .AddScoped<TournamentRepository>();
        }

        private void ConfigureSingletonServices(IServiceCollection services)
        {
            services
                .AddSingleton<IDatabaseManager, DatabaseManager>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<GamePayoutEngine>()
                .AddSingleton<PaylineRepository>()
                .AddSingleton<GameInfoRepository>()
                .AddSingleton<SymbolRepository>()
                .AddSingleton<HistoryDecoderFactory>()
                .AddSingleton(Log.Logger)
                .AddCachedSettings();
        }

        private void ConfigureOptions(IServiceCollection services)
        {
            services
                .Configure<AppSettingsConfig>(Configuration)
                .Configure<AuthenticationConfig>(Configuration);
                
        }

        private void ConfigureHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<AzureMetricsClient>();
            services.AddHttpClient<CasClient>();
            services.AddHttpClient<GameServiceClient>();
            services.AddHttpClient<NominatimClient>();
        }

        private void ConfigureCoreServices(IServiceCollection services)
        {
            ConfigureTransientServices(services);
            ConfigureScopedServices(services);
            ConfigureSingletonServices(services);
            ConfigureOptions(services);
            ConfigureHttpClients(services);

            services
                .AddResponseCompression()
                .AddOptions()
                .AddCachedSettings()
                .AddAuthorization()
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                    {
                        Title = "Kairos BackOffice Api",
                        Version = "v1"
                    });
                });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidAudience = Configuration["Jwt:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        };
                    });

            services.AddMemcached(options => Configuration.GetSection("Memcached").Bind(options));

            services.AddMvc(setup =>
            {
                setup.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddJsonOptions(options => 
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                JsonConvert.DefaultSettings = () => options.SerializerSettings;
            })
            .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
        }
    }
}

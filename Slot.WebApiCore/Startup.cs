using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Omu.ValueInjecter.Injections;
using Slot.Core.Services;
using Slot.WebApiCore.AsyncLock;
using Slot.WebApiCore.Filters;
using Slot.WebApiCore.hub;
using Slot.WebApiCore.Models.Builders;
using System;
using System.Text;

namespace Slot.WebApiCore
{
    public class XXX : PropertyInjection
    {
        protected override void Inject(object source, object target)
        {
            throw new NotImplementedException();
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureCoreServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApiDocument();

            // add all services
            services.AddSlotCore()
                    .AddResponseCompression()
                    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                    .AddSingleton<LockerManager>();

            // add model buidlers
            services.AddScoped<SpinRequestBuilder>()
                    .AddScoped<BonusGameRequestBuilder>()
                    .AddScoped<GetBetsRequestBuilder>();

            services.AddCors(o => o.AddPolicy("AppPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
                       //.WithOrigins("http://localhost:60116", "http://localhost:3000")
                       //.AllowCredentials();
            }));

            services.AddCors();

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

            //services.AddSignalR();
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                //hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(2);
            });

            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddMvc(x =>
            {
                x.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddXmlSerializerFormatters();

            services.AddApplicationInsightsTelemetry();
        }

        private void ConfigureRedis(IServiceCollection services)
        {
            services.AddDistributedRedisCache(x =>
            {
                x.Configuration = Configuration.GetConnectionString("Redis");
                x.InstanceName = "RedisCache";
            });
        }

        private void ConfigureHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks()
                    .AddSqlServer(Configuration.GetConnectionString("WriteableDatabase"), name: "WritableDatabase")
                    .AddSqlServer(Configuration.GetConnectionString("ReadOnlyDatabase"), name: "ReadOnlyDatabase")
                    .AddRedis(Configuration.GetConnectionString("Redis"))
                    .AddUrlGroup(new Uri("http://proxy.slotwallet.com/health/ping"), "WalletProxy");
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureCoreServices(services);
            services.AddDistributedMemoryCache();
            ConfigureHealthChecks(services);
        }

        public void ConfigureUATServices(IServiceCollection services)
        {
            ConfigureCoreServices(services);
            ConfigureRedis(services);
            ConfigureHealthChecks(services);
        }

        public void ConfigureStagingServices(IServiceCollection services)
        {
            ConfigureCoreServices(services);
            ConfigureRedis(services);
            ConfigureHealthChecks(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureCoreServices(services);
            ConfigureRedis(services);
            ConfigureHealthChecks(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseCors("AppPolicy");
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseResponseCompression();

            app.UseHealthChecks("/kairos", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<RaceHub>("/racehub");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}

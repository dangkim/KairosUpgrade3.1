using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Slot.BackOffice
{
    public class Program
    {
        private static void ConfigureLogForContainer(LoggerConfiguration conf)
        {
            conf.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://logserver:9200"))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = "kairos-backoffice-{0:yyyy.MM.dd}"
            });
        }

        public static void Main(string[] args)
        {
            ConfigureLogger();

            try
            {
                Log.Information("Starting web host");
                BuildWebHost(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseSerilog()
                .ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                {
                    var environment = webHostBuilderContext.HostingEnvironment;

                    configurationBuilder
                            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                            .AddJsonFile($"authconfig.json", optional: true, reloadOnChange: true)
                                .AddJsonFile($"authconfig.{environment.EnvironmentName}.json", optional: true)
                            .AddEnvironmentVariables();
                });

        private static void ConfigureLogger()
        {
            var filePathPattern = "logs/log-.txt";
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            var conf = new LoggerConfiguration()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               .Enrich.FromLogContext();

            if (!string.IsNullOrEmpty(env) && env != EnvironmentName.Production)
            {
                conf.MinimumLevel.Debug().WriteTo.Console();
            }

            if(inContainer)
            {
                ConfigureLogForContainer(conf);
            }
            else
            {
                conf.WriteTo.File(filePathPattern, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 93);
            }

            Log.Logger = conf.CreateLogger();
        }
    }
}

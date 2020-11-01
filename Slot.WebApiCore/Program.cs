using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;


namespace Slot.WebApiCore
{
    public class Program
    {

        private static void ConfigreLogForContainer(LoggerConfiguration conf)
        {
            conf.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://logserver:9200"))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = "kairos-gameservice-{0:yyyy.MM.dd}"
            });
        }

        public static void Main(string[] args)
        {
            var conf = new LoggerConfiguration()
               .MinimumLevel.Information()
               .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
               .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .Enrich.WithMachineName();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrEmpty(env) && env != Environments.Production)
            {
                conf.MinimumLevel.Debug().WriteTo.Console();
            }

            var inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
            if (inContainer)
            {
                ConfigreLogForContainer(conf);
            }
            else
            {
                conf.WriteTo.File("logs/log-.txt",
                                 rollingInterval: RollingInterval.Day,
                                 retainedFileCountLimit: 93);
            }

            Log.Logger = conf.CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.CaptureStartupErrors(true);
                webBuilder.UseSerilog();
            });
    }
}

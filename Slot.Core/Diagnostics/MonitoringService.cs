using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Slot.Core.Extensions;
using System;
using System.Collections.Generic;

namespace Slot.Core.Diagnostics
{
    public class MonitoringService : IMonitoringService
    {
        private readonly TelemetryClient telemetryClient;
        private readonly ILogger<MonitoringService> logger;

        public MonitoringService(TelemetryClient telemetryClient,
                                 ILogger<MonitoringService> logger)
        {
            this.telemetryClient = telemetryClient;
            this.logger = logger;
        }

        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
        {
            telemetryClient.TrackDependency(dependencyTypeName, target, dependencyName, data, startTime, duration, resultCode, success);
        }

        public T TrackDependency<T>(string dependency, string command, Func<T> func)
        {
            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var success = true;
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                success = false;
                telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    ["OperationType"] = "DependencyCall",
                    ["Name"] = dependency,
                    ["Action"] = command,
                });
                throw ex;
            }
            finally
            {
                timer.Stop();
                logger.LogDebug($"{dependency} - {command} Elapsed: {timer.ElapsedMilliseconds}ms");
                telemetryClient.TrackDependency(dependency, command, null, startTime, timer.Elapsed, success);
            }
        }

        public void TrackDependency(string dependency, string command, Action action)
        {
            TrackDependency(dependency, command, action.ToFunc());
        }

        public void TrackException(Exception ex, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            telemetryClient.TrackException(ex, properties, metrics);
        }
    }
}

using System;
using System.Collections.Generic;

namespace Slot.Core.Diagnostics
{
    public interface IMonitoringService
    {
        void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success);
        T TrackDependency<T>(string dependency, string command, Func<T> func);
        void TrackDependency(string dependency, string command, Action action);
        void TrackException(Exception ex, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
    }
}
using System;

namespace Slot.Core.Modules.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModuleInfoAttribute : Attribute
    {
        public ModuleInfoAttribute(string key, double version = 1.0)
        {
            Key = key;
            Version = version;
        }

        /// <summary>
        /// A unique key for the game module.
        /// </summary>
        public string Key { get; }

        public double Version { get; }
    }
}

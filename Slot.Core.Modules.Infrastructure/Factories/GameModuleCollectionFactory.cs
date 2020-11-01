using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Slot.Core.Modules.Infrastructure
{
    public static class GameModuleCollectionFactory
    {
        private static IEnumerable<Assembly> LoadFromAppDomain()
        {
            var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
            foreach (var asm in assemblies)
            {
                if (asm.FullName.IndexOf("Slot.Games") >= 0)
                    yield return asm;
            }
        }

        private static IEnumerable<Assembly> LoadFromDirectory()
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Slot.Games.*.dll")
                            .Select(Assembly.LoadFrom);
        }

        private static IEnumerable<(string, Type)> ResolveModuleTypes(IServiceProvider serviceProvider, IEnumerable<Assembly> assemblies)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("GameModuleCollectionFactory");
                var t = typeof(IGameModule);
                return assemblies.SelectMany(x => x.GetTypes())
                        .Where(x => t.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                        .Where(x => x.GetCustomAttribute<ModuleInfoAttribute>() != null)
                        .Select(x =>
                        {
                            var attr = x.GetCustomAttribute<ModuleInfoAttribute>();
                            return (attr.Key, x);
                        });
            }
        }

        public static GameModuleCollection Create(IServiceProvider serviceProvider)
        {
            var assemblies = LoadFromAppDomain().Union(LoadFromDirectory());
            var moduleTypes = ResolveModuleTypes(serviceProvider, assemblies).ToDictionary(x => x.Item1, x => x.Item2);
            return new GameModuleCollection(serviceProvider, moduleTypes);
        }
    }
}
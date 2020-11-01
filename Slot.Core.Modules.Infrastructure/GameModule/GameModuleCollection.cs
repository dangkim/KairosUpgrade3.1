using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Slot.Core.Modules.Infrastructure
{
    public sealed class GameModuleCollection
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConcurrentDictionary<string, Type> moduleTypes;
        private readonly ConcurrentDictionary<string, Lazy<ObjectFactory>> moduleFactories;

        public GameModuleCollection(IServiceProvider serviceProvider, Dictionary<string, Type> moduleTypes)
        {
            this.serviceProvider = serviceProvider;
            this.moduleTypes = new ConcurrentDictionary<string, Type>(moduleTypes);
            moduleFactories = new ConcurrentDictionary<string, Lazy<ObjectFactory>>();
        }

        public IGameModule GetModule(string key)
        {
            if (moduleTypes.TryGetValue(key, out Type moduleType))
            {
                return GetOrCreate(key, moduleType);
            }
            return null;
        }

        public bool TryGetModule(string key, out IGameModule gameModule)
        {
            gameModule = GetModule(key);
            return gameModule != null;
        }

        public bool Contains(string key)
        {
            return moduleTypes.ContainsKey(key);
        }

        public IEnumerable<IGameModule> ListModules()
        {
            foreach (var kv in moduleTypes)
            {
                yield return GetOrCreate(kv.Key, kv.Value);
            }
        }

        private IGameModule GetOrCreate(string key, Type moduleType)
        {
            var factory = moduleFactories.GetOrAdd(key,
                     new Lazy<ObjectFactory>(() => ActivatorUtilities.CreateFactory(moduleType, Type.EmptyTypes)));
            return factory.Value(serviceProvider, Array.Empty<object>()) as IGameModule;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Slot.Model;
using GameId = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History.HistoryDecode
{
    public class HistoryDecoderFactory : BaseRepository
    {
        private static readonly Dictionary<GameId, Type> _assembliesResolve = new Dictionary<GameId, Type>();
        private readonly PaylineRepository paylineRepository;

        public HistoryDecoderFactory(PaylineRepository paylineRepository)
        {
            this.paylineRepository = paylineRepository;
        }

        private Tuple<GameId, Type> GetHistoryInfo(GameId gameId)
        {
            return executingAssembly
                            .GetTypes()
                            .Where(x => 
                                    x.IsClass && !x.IsAbstract 
                                    && x.GetCustomAttribute<HistoryInfoAttribute>() != null 
                                    && x.GetCustomAttribute<HistoryInfoAttribute>().GameId == gameId)
                            .Select(ele => Tuple.Create(ele.GetCustomAttribute<HistoryInfoAttribute>().GameId, ele))
                            .SingleOrDefault();
        }

        public IGameHistory Resolve(GameId gameId)
        {
            IGameHistory instance = null;

            if(!_assembliesResolve.TryGetValue(gameId, out var historyInfo))
            {
                var tuple = GetHistoryInfo(gameId);

                if (tuple != null)
                {
                    _assembliesResolve.Add(tuple.Item1, tuple.Item2);

                    instance = Activator.CreateInstance(tuple.Item2, new[] { paylineRepository }) as IGameHistory;
                }
            }
            else
            {
                instance = Activator.CreateInstance(historyInfo, new[] { paylineRepository }) as IGameHistory;
            }

            return instance;
        }
    }
}
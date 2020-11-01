using Microsoft.AspNetCore.Http;
using Slot.Model;
using Slot.Model.Entity;


namespace Slot.Core.Modules.Infrastructure.Models
{
    public class RequestContext<T>
    {
        public RequestContext(string sessionKey, string gameKey, PlatformType platform)
        {
            SessionKey = sessionKey;
            GameKey = gameKey;
            Platform = platform;
        }

        public string SessionKey { get; }
        public string GameKey { get; }
        public IQueryCollection Query { get; set; }
        public UserSession UserSession { get; set; }
        public UserGameState LastGameState { get; set; }
        public PlatformType Platform { get; set; }
        public UserGameKey UserGameKey { get; set; }
        public Currency Currency { get; set; }
        public Operator Operator { get; set; }
        public Game Game { get; set; }
        public GameSetting GameSetting { get; set; }
        public GameTransaction GameTransaction { get; set; }
        public long CurrentRound { get; set; }
        public T Parameters { get; set; }
    }
}

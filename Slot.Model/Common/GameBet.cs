using System;


namespace Slot.Model
{
    [Serializable]
    public abstract class GameBet
    {
        protected GameBet(UserGameKey userGameKey, PlatformType platformType)
        {
            this.UserGameKey = userGameKey;
            this.PlatformType = platformType;
        }
        protected GameBet()
        {
        }

        public PlatformType PlatformType { get; protected set; }

        public UserGameKey UserGameKey { get; protected set; }
    }
}
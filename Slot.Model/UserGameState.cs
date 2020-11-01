using System;

namespace Slot.Model
{
    [Serializable]
    public class UserGameState
    {
        public long LastRoundId;
        public GameStateType Type;

        public UserGameState(long roundId, GameStateType type)
        {
            LastRoundId = roundId;
            Type = type;
        }

        public static UserGameState InitialState()
        {
            return new UserGameState(0, GameStateType.SlotStateNormal);
        }

        public static UserGameState ForNormal(long roundId)
        {
            return new UserGameState(roundId, GameStateType.SlotStateNormal);
        }

        public static UserGameState ForBonus(long roundId)
        {
            return new UserGameState(roundId, GameStateType.SlotStateBonus);
        }
    }
}

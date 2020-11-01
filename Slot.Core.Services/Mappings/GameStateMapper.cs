using Slot.Model;
using Slot.Model.Entity;

namespace Slot.Core.Services.Mappings
{
    public class GameStateMapper
    {
        public static UserGameState Map(GameState state)
        {
            if (state.Type == GameStateType.SlotStateNormal)
            {
                return UserGameState.ForNormal(state.LastRoundId ?? 0);
            }
            return UserGameState.ForBonus(state.LastRoundId ?? 0);
        }
    }
}

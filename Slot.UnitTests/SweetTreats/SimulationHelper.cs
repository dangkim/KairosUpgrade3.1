using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.SweetTreats.Configuration;
using Slot.Games.SweetTreats.Models.Engines;
using Slot.Model;
using SpinResult = Slot.Games.SweetTreats.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.SweetTreats
{
    public static class SimulationHelper
    {
        public static string ToFormattedWheelString(this string wheelString)
        {
            return string.Join(',', wheelString.Split('|'));
        }

        public static SpinResult GenerateSpinResult(int level)
        {
            var config = new Configuration();
            var requestContext = new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = 1,
                    Multiplier = 1
                },
                Platform = PlatformType.All
            };

            return MainGameEngine.CreateSpinResult(level, requestContext, config);
        }

        public static SpinResult GenerateWinningSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win == 0)
                spinResult = GenerateSpinResult(level);

            return spinResult;
        }

        public static SpinResult GenerateNonWinningSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win > 0)
                spinResult = GenerateSpinResult(level);

            return spinResult;
        }
    }
}

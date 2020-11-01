using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.LionDance.Configuration;
using Slot.Games.LionDance.Engines;
using Slot.Games.LionDance.Models.GameResults.Spins;
using Slot.Model;
using SpinResult = Slot.Games.LionDance.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.LionDance
{
    public static class SpinsHelper
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
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
        }

        public static SpinResult GenerateNonWinningSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win != 0)
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
        }

        public static CollapsingSpinResult GenerateCollapsingSpinResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.Payline,
                                                                config.PayTable);

            return collapsingSpinResult;
        }

        public static CollapsingSpinResult GenerateNonWinningCollapsingSpinResult(int level)
        {
            var collapsingSpinResult = GenerateCollapsingSpinResult(level);

            while (collapsingSpinResult.Win > 0)
            {
                collapsingSpinResult = GenerateCollapsingSpinResult(level);
            }

            return collapsingSpinResult;
        }

        public static CollapsingSpinResult GenerateWinningCollapsingSpinResult(int level)
        {
            var collapsingSpinResult = GenerateCollapsingSpinResult(level);

            while (collapsingSpinResult.Win == 0)
            {
                collapsingSpinResult = GenerateCollapsingSpinResult(level);
            }

            return collapsingSpinResult;
        }
    }
}

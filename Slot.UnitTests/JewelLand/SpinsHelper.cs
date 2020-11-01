using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Model;
using System.Linq;
using SpinResult = Slot.Games.JewelLand.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.JewelLand
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

        public static SpinResult GenerateWinningNonBonusSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win == 0 || spinResult.IsBonus)
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
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

        public static SpinResult GenerateWithBonusSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (!spinResult.IsBonus)
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
        }

        public static SpinResult GenerateWithRespinSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (!spinResult.HasRespinBonus)
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
        }

        public static SpinResult GenerateWithMultiplierSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (!spinResult.HasMultiplierBonus)
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
        }
    }
}

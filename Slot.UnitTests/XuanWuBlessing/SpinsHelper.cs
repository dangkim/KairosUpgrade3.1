using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using Slot.Games.XuanWuBlessing.Models.GameResults.Spins;
using Slot.Model;
using System.Diagnostics;
using SpinResult = Slot.Games.XuanWuBlessing.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.XuanWuBlessing
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

        public static SpinResult GenerateWithBonusSpinResult(int level)
        {
            var generateCount = 0;
            var spinResult = GenerateWinningSpinResult(level);

            while (!spinResult.HasBonus)
            {
                spinResult = GenerateWinningSpinResult(level);
                generateCount++;
                Debug.WriteLine($"SpinResult Generate Count: {generateCount}");
            }

            return spinResult;
        }

        public static SpinResult GenerateNonWinningSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while(spinResult.Win > 0)
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

        public static SpinResult GenerateWinningNonBonusSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win == 0 || spinResult.HasBonus)
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
        }        

        public static FreeSpinResult GenerateFreeSpinResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            var requestContext = new RequestContext<BonusArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new BonusArgs(),
                Platform = PlatformType.All
            };
            var randomFreeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection));

            return FreeSpinBonusEngine.CreateFreeSpinResult(level, requestContext, randomFreeSpinMode, spinResult, config);
        }

        public static FreeSpinResult GenerateWithBonusFreeSpinResult(int level)
        {
            var generateCount = 0;
            var freeSpinResult = GenerateFreeSpinResult(level);

            while (!freeSpinResult.HasBonus)
            {
                freeSpinResult = GenerateFreeSpinResult(level);
                generateCount++;
                Debug.WriteLine($"SpinResult Generate Count: {generateCount}");
            }

            return freeSpinResult;
        }

        public static FreeSpinResult GenerateNonWinningFreeSpinResult(int level)
        {
            var freeSpinResult = GenerateFreeSpinResult(level);

            while (freeSpinResult.Win > 0)
            {
                freeSpinResult = GenerateFreeSpinResult(level);
            }

            return freeSpinResult;
        }

        public static FreeSpinResult GenerateNonWinningNonBonusFreeSpinResult(int level)
        {
            var freeSpinResult = GenerateFreeSpinResult(level);

            while (freeSpinResult.Win > 0 || freeSpinResult.IsBonus)
            {
                freeSpinResult = GenerateFreeSpinResult(level);
            }

            return freeSpinResult;
        }

        public static FreeSpinResult GenerateWinningFreeSpinResult(int level)
        {
            var freeSpinResult = GenerateFreeSpinResult(level);

            while (freeSpinResult.Win == 0)
            {
                freeSpinResult = GenerateFreeSpinResult(level);
            }

            return freeSpinResult;
        }

        public static FreeSpinResult GenerateWinningNonBonusFreeSpinResult(int level)
        {
            var freeSpinResult = GenerateFreeSpinResult(level);

            while (freeSpinResult.Win == 0 || freeSpinResult.HasBonus)
            {
                freeSpinResult = GenerateFreeSpinResult(level);
            }

            return freeSpinResult;
        }
    }
}

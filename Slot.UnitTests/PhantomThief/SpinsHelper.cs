using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Games.PhantomThief.Models.GameResults.Spins;
using Slot.Model;
using System.Diagnostics;
using SpinResult = Slot.Games.PhantomThief.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.PhantomThief
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

            while (spinResult.Win == 0 || spinResult.HasFeatureBonus)
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
            var generateCount = 0;
            var spinResult = GenerateWinningSpinResult(level);

            while (!spinResult.HasFeatureBonus)
            {
                spinResult = GenerateWinningSpinResult(level);
                generateCount++;
                Debug.WriteLine($"SpinResult Generate Count: {generateCount}");
            }

            return spinResult;
        }

        public static CollapsingSpinResult GenerateCollapsingSpinResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.Payline,
                                                                config.MainGamePayTable);

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

        public static CollapsingSpinResult GenerateNonWinningNonBonusCollapsingSpinResult(int level)
        {
            var collapsingSpinResult = GenerateCollapsingSpinResult(level);

            while (collapsingSpinResult.Win > 0 || collapsingSpinResult.HasFeatureBonus)
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

        public static CollapsingSpinResult GenerateWithBonusCollapsingSpinResult(int level)
        {
            var generateCount = 0;
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.Payline,
                                                                config.MainGamePayTable);

            while (!collapsingSpinResult.HasFeatureBonus)
            {
                spinResult = GenerateWinningSpinResult(level);
                targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
                collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.Payline,
                                                                config.MainGamePayTable);
                generateCount++;
                Debug.WriteLine($"CollapsingSpinResult Generate Count: {generateCount}");
            }

            return collapsingSpinResult;
        }

        public static CollapsingSpinResult GenerateWinningNonBonusCollapsingSpinResult(int level)
        {
            var collapsingSpinResult = GenerateCollapsingSpinResult(level);

            while (collapsingSpinResult.Win == 0 || collapsingSpinResult.HasFeatureBonus)
            {
                collapsingSpinResult = GenerateCollapsingSpinResult(level);
            }

            return collapsingSpinResult;
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

            return FreeSpinBonusEngine.CreateFreeSpinResult(level, requestContext, config);
        }

        public static FreeSpinResult GenerateWithBonusFreeSpinResult(int level)
        {
            var generateCount = 0;
            var freeSpinResult = GenerateFreeSpinResult(level);

            while (!freeSpinResult.HasFeatureBonus)
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

            while (freeSpinResult.Win > 0 || freeSpinResult.HasBonus)
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

            while (freeSpinResult.Win == 0 || freeSpinResult.HasFeatureBonus)
            {
                freeSpinResult = GenerateFreeSpinResult(level);
            }

            return freeSpinResult;
        }

        public static FreeSpinCollapsingResult GenerateFreeSpinCollapsingResult(int level)
        {
            var config = new Configuration();

            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.Payline,
                                                                config.FreeGamePayTable,
                                                                config.FreeGameScatterSymbols);

            return freeSpinCollapsingResult;
        }

        public static FreeSpinCollapsingResult GenerateNonWinningFreeSpinCollapsingResult(int level)
        {
            var freeSpinCollapsingResult = GenerateFreeSpinCollapsingResult(level);

            while (freeSpinCollapsingResult.Win > 0)
            {
                freeSpinCollapsingResult = GenerateFreeSpinCollapsingResult(level);
            }

            return freeSpinCollapsingResult;
        }

        public static FreeSpinCollapsingResult GenerateWinningFreeSpinCollapsingResult(int level)
        {
            var freeSpinCollapsingResult = GenerateFreeSpinCollapsingResult(level);

            while (freeSpinCollapsingResult.Win == 0)
            {
                freeSpinCollapsingResult = GenerateFreeSpinCollapsingResult(level);
            }

            return freeSpinCollapsingResult;
        }

        public static FreeSpinCollapsingResult GenerateWithBonusFreeSpinCollapsingResult(int level)
        {
            var generateCount = 0;
            var config = new Configuration();

            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.Payline,
                                                                config.FreeGamePayTable,
                                                                config.FreeGameScatterSymbols);

            while (!freeSpinCollapsingResult.HasFeatureBonus)
            {
                spinResult = GenerateWinningSpinResult(level);
                targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
                freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.Payline,
                                                                config.FreeGamePayTable,
                                                                config.FreeGameScatterSymbols);
                generateCount++;
                Debug.WriteLine($"CollapsingSpinResult Generate Count: {generateCount}");
            }

            return freeSpinCollapsingResult;
        }

        public static FreeSpinCollapsingResult GenerateWinningNonBonusFreeSpinCollapsingResult(int level)
        {
            var freeSpinCollapsingResult = GenerateFreeSpinCollapsingResult(level);

            while (freeSpinCollapsingResult.Win == 0 || freeSpinCollapsingResult.HasFeatureBonus)
            {
                freeSpinCollapsingResult = GenerateFreeSpinCollapsingResult(level);
            }

            return freeSpinCollapsingResult;
        }
    }
}

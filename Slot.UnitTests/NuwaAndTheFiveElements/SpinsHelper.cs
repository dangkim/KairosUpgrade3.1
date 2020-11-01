using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Games.NuwaAndTheFiveElements.Models.GameResults.Spins;
using Slot.Model;
using System.Diagnostics;

namespace Slot.UnitTests.NuwaAndTheFiveElements
{
    public static class SpinsHelper
    {
        public static Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult GenerateSpinResult(int level)
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

        public static Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult GenerateWinningNonBonusSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win == 0 || spinResult.HasFeatureBonus)
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
        }

        public static Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult GenerateWinningSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win == 0)
            {
                spinResult = GenerateSpinResult(level);
            }

            return spinResult;
        }

        public static Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult GenerateWithBonusSpinResult(int level)
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
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);

            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.SymbolCollapsePairs,
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

        public static CollapsingSpinResult GenerateWithBonusCollapsingSpinResult(int level)
        {
            var generateCount = 0;
            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);

            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.SymbolCollapsePairs,
                                                                config.Payline,
                                                                config.PayTable);

            while (!collapsingSpinResult.HasFeatureBonus)
            {
                spinResult = GenerateWinningSpinResult(level);
                collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.SymbolCollapsePairs,
                                                                config.Payline,
                                                                config.PayTable);
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

            return FreeSpinBonusEngine.CreateFreeSpinResult(level, requestContext, spinResult, config);
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
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);

            var spinResult = GenerateWinningSpinResult(level);
            var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.SymbolCollapsePairs,
                                                                config.Payline,
                                                                config.PayTable);

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
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);

            var spinResult = GenerateWinningSpinResult(level);
            var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.SymbolCollapsePairs,
                                                                config.Payline,
                                                                config.PayTable);

            while (!freeSpinCollapsingResult.HasFeatureBonus)
            {
                spinResult = GenerateWinningSpinResult(level);
                freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(
                                                                spinResult,
                                                                targetWheel,
                                                                config.SymbolCollapsePairs,
                                                                config.Payline,
                                                                config.PayTable);
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

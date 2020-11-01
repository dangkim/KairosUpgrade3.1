using Slot.Core;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.BullRush.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slot.Games.BullRush.BonusFeatures
{
    public static class BullRushJackPotFeature
    {
        public static BullRushJackpotBonus CreateBonus(BullRushSpinResult sr)
        {
            var bonus = new BullRushJackpotBonus
            {
                BonusType = BonusType.FreeSpin,
                Guid = Guid.NewGuid(),
                UserGameKey = sr.SpinBet.UserGameKey,
                SpinBet = sr.SpinBet,
                CurrentRacingStep = sr.CurrentRacingStep,
                CurrentJackpotStep = sr.CurrentJackpotStep,
                CurrentStep = sr.CurrentJackpotStep,
                NumOfJackpot = sr.NumOfJackpot,
                Counter = sr.CurrentJackpotCounter,
                SpinTransactionId = sr.SpinTransactionId ?? sr.TransactionId,
                CumulativeWin = sr.CumulativeWin,
                IsFreeSpin = sr.IsRacing,
                IsInnerWheelBonus = sr.IsInnerWheelBonus,
                GameResult = sr
            };

            return bonus;
        }

        public static Result<BonusResult, ErrorCode> Execute(int level, RequestContext<BonusArgs> requestContext, BullRushJackpotBonus bullRushJackpotBonus, BonusEntity bonusEntity)
        {
            if (bullRushJackpotBonus != null)
            {
                return ExecuteBullRushJackpot(level, requestContext, bullRushJackpotBonus, bonusEntity);
            }

            return ErrorCode.NonexistenceBonus;
        }

        private static Result<BonusResult, ErrorCode> ExecuteBullRushJackpot(int level, RequestContext<BonusArgs> requestContext, BullRushJackpotBonus bullRushJackpotBonus, BonusEntity bonusEntity)
        {
            bullRushJackpotBonus.IsStarted = true;

            var br = new BullRushJackpotBonusResult(requestContext.UserGameKey)
            {
                SpinTransactionId = bullRushJackpotBonus.SpinTransactionId,
                Level = level,
                PlatformType = requestContext.Platform,
                TotalSpin = bullRushJackpotBonus.NumOfJackpot,
                CumulativeWin = bullRushJackpotBonus.CumulativeWin,
                SpinResult = bullRushJackpotBonus.GameResult as BullRushSpinResult,
                Step = bullRushJackpotBonus.CurrentStep,
            };

            if (br.SpinResult == null)
            {
                if (bullRushJackpotBonus.GameResult is BullRushJackpotBonusResult)
                {
                    br.SpinResult = (bullRushJackpotBonus.GameResult as BullRushJackpotBonusResult).SpinResult;
                }
            }

            GenerateJackpotResult(requestContext, br, bullRushJackpotBonus);

            if (br.SpinResult.SelectedInnerWheelValue == BullRushConfiguration.TREASURE3
                || br.SpinResult.SelectedInnerWheelValue == BullRushConfiguration.TREASURE4
                || br.SpinResult.SelectedInnerWheelValue == BullRushConfiguration.TREASURE5)
            {
                for (int i = 0; i < br.SpinResult.SelectedInnerWheelValue; i++)
                {
                    br.SpinResult.InventoryList.Add(BullRushConfiguration.TREASURE);
                }
            }
            else if (br.SpinResult.SelectedInnerWheelValue == BullRushConfiguration.BONUS)
            {
                var bonusPrize = new Dictionary<double, int>(BullRushConfiguration.BonusRace);
                var prizeList = new List<decimal>();
                var random = RandomNumberEngine.NextDouble();

                var selectedCoinValue = bonusPrize.FirstOrDefault(item => random <= item.Key).Value;

                prizeList.Add(selectedCoinValue);

                for (int i = 0; i < BullRushConfiguration.VEHICLES - 1; i++)
                {
                    var randomShowingIndex = RandomNumberEngine.Next(0, bonusPrize.Count - 1);

                    var selectedShowingValue = bonusPrize.Values.ElementAt(randomShowingIndex);
                    var selectedShowingKey = bonusPrize.Keys.ElementAt(randomShowingIndex);

                    prizeList.Add(selectedShowingValue);
                    bonusPrize.Remove(selectedShowingKey);
                }

                br.SpinResult.BonusRacingPrizesList = prizeList.Shuffle();

                var selectedPrizeIndex = br.SpinResult.BonusRacingPrizesList.IndexOf(selectedCoinValue);

                br.SpinResult.SelectedBonusRacingPrizeIndex = selectedPrizeIndex;

                br.SpinResult.SelectedBonusRacingPrize = selectedCoinValue;

                br.SpinResult.IsBonusRacing = true;
            }

            br.SpinResult.IsRacing = true;

            bullRushJackpotBonus.GameResult = br;

            var variantWheel = new Wheel(new List<int>() { 105, 105, 105, 105 });
            var numberOfMagnet = br.SpinResult.InventoryList.Count(x => x == BullRushConfiguration.MAGNET);
            var numberOfVacuum = br.SpinResult.InventoryList.Count(x => x == BullRushConfiguration.VACUUM);

            if (numberOfMagnet == 0 && numberOfVacuum == 0)
            {
                variantWheel = BullRushConfiguration.VariantWheelsOne[level];
                br.SpinResult.VariantWheel = 1;
            }
            else if (numberOfMagnet == 0 && numberOfVacuum > 0)
            {
                var randomDouble = RandomNumberEngine.NextDouble();

                var isVariantTwo = (randomDouble <= BullRushConfiguration.VariantTwoWeight) ? true : false;

                if (isVariantTwo)
                {
                    variantWheel = BullRushConfiguration.VariantWheelsTwo[level];
                    br.SpinResult.VariantWheel = 2;
                }
                else
                {
                    variantWheel = BullRushConfiguration.VariantWheelsThree[level];
                    br.SpinResult.VariantWheel = 3;
                }
            }
            else if (numberOfMagnet > 0 && numberOfVacuum == 0)
            {
                variantWheel = BullRushConfiguration.VariantWheelsFour[level];
                br.SpinResult.VariantWheel = 4;
            }
            else if (numberOfMagnet > 0 && numberOfVacuum > 0)
            {
                variantWheel = BullRushConfiguration.VariantWheelsFive[level];
                br.SpinResult.VariantWheel = 5;
            }

            var listOfChestPosition = variantWheel[3].Select((value, index) => new { value, index }).Where(a => a.value > 0).Select(a => a.index).ToList();
            var chosenTreasureItems = new List<int>();

            for (int i = 0; i < BullRushConfiguration.WidthOfRace; i++)
            {
                int index = RandomNumberEngine.Next(0, listOfChestPosition.Count() - 1);
                chosenTreasureItems.Add(listOfChestPosition[index]);

                listOfChestPosition.RemoveAt(index);
            }

            if (br.SpinResult.IsBonusRacing)
            {
                var distributedAllRows = BullRushCommon.GenerateBonusRacing(br.SpinResult.SelectedBonusRacingPrize, br.SpinResult.SpinBet.LineBet, br.SpinResult.SpinBet.FunPlayDemoKey);

                br.SpinResult.DistributedAllRows = distributedAllRows;

                br.SpinResult.CurrentBonusRacingCounter = BullRushConfiguration.HeightBonusRacing;
            }
            else if (br.SpinResult.IsRacing)
            {
                var distributedAllRows = BullRushCommon.GenerateRacing(variantWheel, chosenTreasureItems, br.SpinResult);

                br.SpinResult.DistributedAllRows = distributedAllRows;

                br.SpinResult.CurrentRacingCounter = BullRushConfiguration.HeightOfRacing;
            }

            var freeSpinBonus = BullRushFreeSpinFeature.CreateBonus(br.SpinResult);

            br.SpinResult.BonusElement = new BullRushBonusElement { Id = freeSpinBonus.Id, Count = br.SpinResult.CurrentRacingCounter, Value = freeSpinBonus.Guid.ToString("N") };

            br.SpinResult.Bonus = new BonusStruct()
            {
                Id = freeSpinBonus.Id,
                Count = br.SpinResult.CurrentRacingCounter,
            };

            bonusEntity.BonusType = freeSpinBonus.GetType().Name;

            br.Bonus = freeSpinBonus;

            return br;
        }

        private static BullRushSpinResult GenerateJackpotResult(RequestContext<BonusArgs> requestContext, BullRushJackpotBonusResult br, BullRushJackpotBonus BullRushBonus)
        {
            var bullRushSpinResult = new BullRushSpinResult()
            {
                Level = br.SpinResult.Level,
                SpinBet = br.SpinResult.SpinBet,
                BonusPositions = br.SpinResult.BonusPositions,
                IsRacing = br.SpinResult.IsRacing,
                IsBonusRacing = br.SpinResult.IsBonusRacing,
                SelectedOuterWheelValue = br.SpinResult.SelectedOuterWheelValue,
                SelectedOuterWheelIndex = br.SpinResult.SelectedOuterWheelIndex,
                CurrentRacingCounter = br.SpinResult.CurrentRacingCounter,
                CurrentJackpotCounter = br.SpinResult.CurrentJackpotCounter,
                NumOfJackpot = br.SpinResult.NumOfJackpot,
                InventoryList = br.SpinResult.InventoryList,
                CumulativeWin = br.SpinResult.CumulativeWin,
                Bet = 0,
                SpinTransactionId = br.SpinTransactionId.HasValue
                ? br.SpinTransactionId
                : br.TransactionId,
                PlatformType = requestContext.Platform,
                RoundId = br.RoundId,
                CurrentRacingStep = br.SpinResult.CurrentRacingStep,
                CurrentJackpotStep = br.SpinResult.CurrentJackpotStep,
                InnerWheel = br.SpinResult.InnerWheel
            };

            var random = RandomNumberEngine.NextDouble();

            if (bullRushSpinResult.SpinBet.FunPlayDemoKey == 1)
            {
                random = 1;
            }

            var selectedCoinValue = BullRushConfiguration.InnerWheelWeight.FirstOrDefault(item => random <= item.Key).Value;

            bullRushSpinResult.SelectedInnerWheelIndex = BullRushConfiguration.InnerWheel.IndexOf(selectedCoinValue);

            bullRushSpinResult.SelectedInnerWheelValue = selectedCoinValue;

            br.SpinResult = bullRushSpinResult;

            return bullRushSpinResult;
        }

        public static List<BonusPosition> CreatePosition(Wheel wheel)
        {
            var rowPositions = wheel.Reels.Select(reel => reel.FindIndex(p => p == BullRushConfiguration.WHEEL) + 1).ToList();
            var bonusPosition = new BonusPosition() { RowPositions = rowPositions };
            return new List<BonusPosition>() { bonusPosition };
        }
    }
}

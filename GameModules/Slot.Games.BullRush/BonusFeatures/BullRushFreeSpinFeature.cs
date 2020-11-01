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
    public static class BullRushFreeSpinFeature
    {
        public static BullRushFreeSpinBonus CreateBonus(BullRushSpinResult bullRushSpinResult)
        {
            var bonus = new BullRushFreeSpinBonus
            {
                BonusType = BonusType.FreeSpin,
                Guid = Guid.NewGuid(),
                UserGameKey = bullRushSpinResult.SpinBet.UserGameKey,
                SpinBet = bullRushSpinResult.SpinBet,
                SpinTransactionId = bullRushSpinResult.SpinTransactionId ?? bullRushSpinResult.TransactionId,
                CurrentStep = bullRushSpinResult.CurrentRacingStep,
                NumOfFreeSpin = bullRushSpinResult.NumOfJackpot,
                RacingCounter = bullRushSpinResult.CurrentRacingCounter,
                BonusRacingCounter = bullRushSpinResult.CurrentBonusRacingCounter,
                CumulativeWin = bullRushSpinResult.CumulativeWin,
                PreviousPowerUp = bullRushSpinResult.PreviousPowerUp,
                Multiplier = 1,
                IsFreeSpin = bullRushSpinResult.IsRacing,
                GameResult = bullRushSpinResult
            };

            return bonus;
        }

        public static Result<BonusResult, ErrorCode> Execute(int level, RequestContext<BonusArgs> requestContext, BullRushFreeSpinBonus freeSpin, BonusEntity bonusEntity)
        {
            if (freeSpin != null)
            {
                return ExecuteFreeSpin(level, requestContext, freeSpin, bonusEntity);
            }

            return ErrorCode.NonexistenceBonus;

        }

        private static Result<BonusResult, ErrorCode> ExecuteFreeSpin(int level, RequestContext<BonusArgs> requestContext, BullRushFreeSpinBonus freeSpinBonus, BonusEntity bonusEntity)
        {
            freeSpinBonus.IsStarted = true;
            var fsr = new BullRushFreeSpinResult(requestContext.UserGameKey)
            {
                SpinTransactionId = freeSpinBonus.SpinTransactionId,
                Level = level,
                PlatformType = requestContext.Platform,
                Multiplier = freeSpinBonus.Multiplier,
                TotalSpin = freeSpinBonus.NumOfFreeSpin,
                Step = freeSpinBonus.CurrentStep,
                CumulativeWin = freeSpinBonus.CumulativeWin,
                SpinResult = freeSpinBonus.GameResult as BullRushSpinResult
            };

            if (fsr.SpinResult == null)
            {
                if (freeSpinBonus.GameResult is BullRushFreeSpinResult)
                {
                    fsr.SpinResult = (freeSpinBonus.GameResult as BullRushFreeSpinResult).SpinResult;
                }
            }

            GenerateFreeSpinResult(level, requestContext, fsr, freeSpinBonus);

            var selectedValue = Convert.ToInt32(fsr.SpinResult.DistributedAllRows[0][requestContext.Parameters.Param]);

            if (selectedValue == BullRushConfiguration.MAGNET)
            {
                fsr.SpinResult.NumberOfMagnetActiveRows = freeSpinBonus.CurrentStep + BullRushConfiguration.MagnetActiveRows;
                fsr.SpinResult.PreviousPowerUp = selectedValue;
                fsr.SpinResult.SelectedPowerUps.Add(selectedValue);
            }
            else if (selectedValue == BullRushConfiguration.VACUUM)
            {
                fsr.SpinResult.NumberOfVacuumActiveRows = freeSpinBonus.CurrentStep + BullRushConfiguration.VacuumActiveRows;
                fsr.SpinResult.PreviousPowerUp = selectedValue;
                fsr.SpinResult.SelectedPowerUps.Add(selectedValue);
            }
            else if (selectedValue == BullRushConfiguration.SHIELD)
            {
                fsr.SpinResult.NumberOfShieldActiveRows = freeSpinBonus.CurrentStep + BullRushConfiguration.ShieldActiveRows;
                fsr.SpinResult.PreviousPowerUp = selectedValue;
                fsr.SpinResult.SelectedPowerUps.Add(selectedValue);
            }
            else if (fsr.SpinResult.PreviousPowerUp == BullRushConfiguration.MAGNET && freeSpinBonus.CurrentStep <= fsr.SpinResult.NumberOfMagnetActiveRows)
            {
                for (int i = 0; i < fsr.SpinResult.DistributedAllRows[0].Count; i++)
                {
                    if (fsr.SpinResult.DistributedAllRows[0][i] == BullRushConfiguration.OBSTACLE
                        && fsr.SpinResult.CumulativeWin >= 25 * fsr.SpinResult.SpinBet.LineBet)
                    {
                        fsr.SpinResult.Win += (-25 * fsr.SpinResult.SpinBet.LineBet);
                    }
                    fsr.SpinResult.Win += fsr.SpinResult.DistributedAllRows[0][i];
                }
            }
            else if (fsr.SpinResult.PreviousPowerUp == BullRushConfiguration.VACUUM && freeSpinBonus.CurrentStep <= fsr.SpinResult.NumberOfVacuumActiveRows)
            {
                for (int i = 0; i < fsr.SpinResult.DistributedAllRows[0].Count; i++)
                {
                    // If Treasure
                    if (fsr.SpinResult.DistributedAllRows[0][requestContext.Parameters.Param] >= 25 * fsr.SpinResult.SpinBet.LineBet)
                    {
                        fsr.SpinResult.Win += fsr.SpinResult.DistributedAllRows[0][requestContext.Parameters.Param];
                    }
                    else if (fsr.SpinResult.DistributedAllRows[0][requestContext.Parameters.Param] == fsr.SpinResult.SpinBet.LineBet) // Attracts coins
                    {
                        fsr.SpinResult.Win += fsr.SpinResult.DistributedAllRows[0][i];
                    }
                    else if (fsr.SpinResult.DistributedAllRows[0][i] == BullRushConfiguration.OBSTACLE
                        && fsr.SpinResult.CumulativeWin >= 25 * fsr.SpinResult.SpinBet.LineBet)
                    {
                        fsr.SpinResult.Win += (-25 * fsr.SpinResult.SpinBet.LineBet);
                    }
                }
            }
            else if (fsr.SpinResult.PreviousPowerUp == BullRushConfiguration.SHIELD && freeSpinBonus.CurrentStep <= fsr.SpinResult.NumberOfShieldActiveRows)
            {
                for (int i = 0; i < fsr.SpinResult.DistributedAllRows[0].Count; i++)
                {
                    if (fsr.SpinResult.DistributedAllRows[0][i] != BullRushConfiguration.OBSTACLE)
                    {
                        fsr.SpinResult.Win += fsr.SpinResult.DistributedAllRows[0][i];
                    }
                }
            }
            else if (requestContext.Parameters.Param == BullRushConfiguration.OBSTACLE)
            {
                fsr.SpinResult.Win += (-25 * fsr.SpinResult.SpinBet.LineBet);
            }
            else if (fsr.SpinResult.SelectedBonusRacingPrize > 0 && fsr.SpinResult.IsBonusRacing)
            {
                var coinValue = (fsr.SpinResult.SelectedBonusRacingPrize / BullRushConfiguration.HeightBonusRacing) * fsr.SpinResult.SpinBet.LineBet;
                if (fsr.SpinResult.DistributedAllRows[0][requestContext.Parameters.Param] == coinValue)
                {
                    fsr.SpinResult.Win = coinValue;
                }

            }
            else if (fsr.SpinResult.DistributedAllRows[0][requestContext.Parameters.Param] == fsr.SpinResult.SpinBet.LineBet)
            {
                fsr.SpinResult.Win = fsr.SpinResult.SpinBet.LineBet;
            }

            fsr.SpinResult.DistributedAllRows.RemoveAt(0);

            fsr.Win = fsr.SpinResult.Win;
            fsr.CumulativeWin = freeSpinBonus.CumulativeWin + fsr.SpinResult.Win;
            fsr.SpinResult.CumulativeWin = freeSpinBonus.CumulativeWin + fsr.SpinResult.Win;

            freeSpinBonus.CumulativeWin += fsr.Win;

            fsr.Counter = fsr.SpinResult.IsBonusRacing ? --freeSpinBonus.BonusRacingCounter : --freeSpinBonus.RacingCounter;
            freeSpinBonus.CurrentStep++;
            fsr.SpinResult.CurrentRacingCounter = freeSpinBonus.RacingCounter;
            fsr.SpinResult.CurrentBonusRacingCounter = freeSpinBonus.BonusRacingCounter;
            fsr.SpinResult.CurrentRacingStep = freeSpinBonus.CurrentStep;

            freeSpinBonus.GameResult = fsr;

            fsr.Bonus = freeSpinBonus;

            if ((fsr.SpinResult.CurrentRacingCounter == 0 && freeSpinBonus.CurrentStep == 105) || (fsr.SpinResult.IsBonusRacing && fsr.SpinResult.CurrentBonusRacingCounter == 0 && freeSpinBonus.CurrentStep == 50))
            {
                fsr.IsCompleted = true;
            }
            else
            {
                bonusEntity.BonusType = freeSpinBonus.GetType().Name;
            }

            return fsr;
        }

        private static void GenerateFreeSpinResult(int level, RequestContext<BonusArgs> requestContext, BullRushFreeSpinResult fsr, BullRushFreeSpinBonus bonus)
        {
            var bullRushSpinResult = new BullRushSpinResult()
            {
                Level = fsr.SpinResult.Level,
                SpinBet = fsr.SpinResult.SpinBet,
                DistributedAllRows = fsr.SpinResult.DistributedAllRows,
                WinPositions = new List<BullRushWinPosition>(),
                BonusPositions = new List<BonusPosition>(),
                RowPositions = new List<int>(),
                CurrentRacingCounter = fsr.SpinResult.CurrentRacingCounter,
                IsRacing = fsr.SpinResult.IsRacing,
                IsBonusRacing = fsr.SpinResult.IsBonusRacing,
                CurrentRacingStep = fsr.SpinResult.CurrentRacingStep,
                PreviousPowerUp = fsr.SpinResult.PreviousPowerUp,
                NumberOfMagnetActiveRows = fsr.SpinResult.NumberOfMagnetActiveRows,
                InventoryList = fsr.SpinResult.InventoryList,
                SelectedBonusRacingPrize = fsr.SpinResult.SelectedBonusRacingPrize,
                CumulativeWin = fsr.SpinResult.CumulativeWin,
                NumOfJackpot = fsr.SpinResult.NumOfJackpot,
                Bet = 0,
                SpinTransactionId = fsr.SpinTransactionId.HasValue
                ? fsr.SpinTransactionId
                : fsr.TransactionId,
                PlatformType = requestContext.Platform,
                RoundId = fsr.RoundId
            };


            //bullRushSpinResult.Wheel = MegaMoneyCommon.GenerateFreeSpin(level, variantWheel, bullRushSpinResult.InventoryList, chosenPowerUpItems, fsr.SpinResult.SpinBet.FunPlayDemoKey);

            fsr.SpinResult = bullRushSpinResult;
        }

        public static List<BonusPosition> CreatePosition(Wheel wheel)
        {
            var rowPositions = wheel.Reels.Select(reel => reel.FindIndex(p => p == BullRushConfiguration.STAR) + 1).ToList();
            var bonusPosition = new BonusPosition() { RowPositions = rowPositions };
            return new List<BonusPosition>() { bonusPosition };
        }
    }
}

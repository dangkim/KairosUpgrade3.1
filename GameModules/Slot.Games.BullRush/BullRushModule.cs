using Microsoft.Extensions.Logging;
using Slot.Core;
using Slot.Core.Extensions;
using Slot.Core.Modules.Infrastructure;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.BullRush.BonusFeatures;
using Slot.Games.BullRush.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slot.Games.BullRush
{
    [ModuleInfo("bullrush", version: 1.0)]
    public class BullRushModule : IGameModule
    {
        private readonly ILogger<BullRushModule> logger;

        public virtual int GameId { get; protected set; }

        public int Lines => BullRushConfiguration.Lines;

        public BullRushModule(ILogger<BullRushModule> ilogger)
        {
            logger = ilogger;
            GameId = BullRushConfiguration.GameId;
        }

        public decimal CalculateTotalBet(UserGameSpinData userGameSpinData, RequestContext<SpinArgs> requestContext)
        {
            var args = requestContext.Parameters;
            return args.LineBet * BullRushConfiguration.Credit;
        }

        public Result<SpinResult, ErrorCode> ExecuteSpin(int level, UserGameSpinData userGameSpinData, RequestContext<SpinArgs> requestContext)
        {
            var lastSpin = userGameSpinData == null ? default : userGameSpinData.Data.FromByteArray<BullRushSpinResult>();

            var result = new BullRushSpinResult()
            {
                Level = level,
                SpinBet = new SpinBet(requestContext.UserGameKey, requestContext.Platform)
                {
                    CurrencyId = requestContext.Currency.Id,
                    GameSettingGroupId = requestContext.GameSetting.GameSettingGroupId,
                    LineBet = requestContext.Parameters.LineBet,
                    //Lines = BullRushConfiguration.Lines,
                    //Multiplier = BullRushConfiguration.Multiplier,
                    FunPlayDemoKey = requestContext.Parameters.FunPlayDemoKey,
                    Credits = BullRushConfiguration.Credit
                },

                PlatformType = requestContext.Platform
            };

            if (lastSpin != null)
            {
                var lastList = new List<int>(lastSpin.InventoryList);

                result.InventoryList = lastList;
            }

            var random = RandomNumberEngine.NextDouble();

            var selectedValue = BullRushConfiguration.OuterWheelWeight.FirstOrDefault(item => random <= item.Key).Value;

            //result.Wheel = new Wheel(1, 8, "4,0,-1,1,4,3,-1,2");

            result.SelectedOuterWheelIndex = BullRushConfiguration.OuterWheel.IndexOf(selectedValue);

            result.SelectedOuterWheelValue = selectedValue;

            if (selectedValue == BullRushConfiguration.RACE)
            {
                var inventoryOfThreePowerUps = new List<int>();

                var magnetIndex = result.InventoryList.IndexOf(BullRushConfiguration.MAGNET);
                if (magnetIndex != -1)
                {
                    inventoryOfThreePowerUps.Add(BullRushConfiguration.MAGNET);
                    result.InventoryList.RemoveAt(magnetIndex);
                }

                var vacuumIndex = result.InventoryList.IndexOf(BullRushConfiguration.VACUUM);
                if (vacuumIndex != -1)
                {
                    inventoryOfThreePowerUps.Add(BullRushConfiguration.VACUUM);
                    result.InventoryList.RemoveAt(vacuumIndex);
                }

                var shieldIndex = result.InventoryList.IndexOf(BullRushConfiguration.SHIELD);
                if (shieldIndex != -1)
                {
                    inventoryOfThreePowerUps.Add(BullRushConfiguration.SHIELD);
                    result.InventoryList.RemoveAt(shieldIndex);
                }

                result.IsBonus = true;
                result.IsInnerWheelBonus = true;
                result.InventoryOfThreePowerUps = new List<int>(inventoryOfThreePowerUps);
                result.CurrentJackpotStep = 1;
            }
            else if (selectedValue == BullRushConfiguration.SURPRISE)
            {
                var randomSuprise = RandomNumberEngine.NextDouble();

                var supriseValue = BullRushConfiguration.SupriseWeight.FirstOrDefault(x => randomSuprise <= x.Key).Value;

                if (supriseValue == BullRushConfiguration.BUNDLE)
                {
                    result.InventoryList.Add(BullRushConfiguration.MAGNET);
                    result.InventoryList.Add(BullRushConfiguration.VACUUM);
                    result.InventoryList.Add(BullRushConfiguration.SHIELD);
                }
                else if (supriseValue == BullRushConfiguration.COIN1)
                {
                    result.Win = 100 * result.SpinBet.LineBet;
                }
                else if (supriseValue == BullRushConfiguration.COIN2)
                {
                    result.Win = 200 * result.SpinBet.LineBet;
                }

            }
            else if (selectedValue != BullRushConfiguration.RACE && selectedValue != BullRushConfiguration.LOSE)
            {
                result.InventoryList.Add(selectedValue);
            }

            return result;
        }

        IWheel IGameModule.InitialRandomWheel()
        {
            var nullCombWheel = new Wheel(1, 8, "4,0,-1,1,4,3,-1,2");

            return nullCombWheel;
        }

        public Bonus ConvertToBonus(BonusEntity bonusEntity)
        {
            Bonus bonus = null;

            if (bonusEntity.Data != null)
            {
                if (bonusEntity.BonusType == nameof(BullRushFreeSpinBonus))
                {
                    bonus = bonusEntity.Data.FromByteArray<BullRushFreeSpinBonus>() ?? new BullRushFreeSpinBonus();
                }
                else if (bonusEntity.BonusType == nameof(BullRushJackpotBonus))
                {
                    bonus = bonusEntity.Data.FromByteArray<BullRushJackpotBonus>() ?? new BullRushJackpotBonus();
                }
            }

            return bonus;
        }

        public Result<Bonus, ErrorCode> CreateBonus(SpinResult spinResult)
        {
            var result = spinResult as BullRushSpinResult;
            Bonus bonus;

            try
            {
                if (result.IsRacing)
                {
                    bonus = BullRushFreeSpinFeature.CreateBonus(result);
                }
                else if (result.IsInnerWheelBonus)
                {
                    bonus = BullRushJackPotFeature.CreateBonus(result);
                }
                else
                {
                    return ErrorCode.NonexistenceBonus;
                }

                result.BonusElement = new BullRushBonusElement { Id = bonus.Id, Value = bonus.Guid.ToString("N") };

                result.Bonus = new BonusStruct()
                {
                    Id = bonus.Id,
                };

            }
            catch (Exception ex)
            {
                logger.LogError("[Determine Freespin Bonus]: Create bonus position wrongly");
                logger.LogError(ex, ex.Message);
                return ErrorCode.NonexistenceBonus;
            }

            return bonus;
        }

        public Result<BonusResult, ErrorCode> ExecuteBonus(int level, BonusEntity bonus, RequestContext<BonusArgs> requestContext)
        {
            var bns = bonus.Data.FromByteArray<Bonus>();

            if (bns is BullRushFreeSpinBonus fb)
            {
                return BullRushFreeSpinFeature.Execute(level, requestContext, fb, bonus);
            }
            else if (bns is BullRushJackpotBonus jpb)
            {
                return BullRushJackPotFeature.Execute(level, requestContext, jpb, bonus);
            }

            return ErrorCode.NonexistenceBonus;
        }

        public IExtraGameSettings GetExtraSettings(int level, UserGameSpinData userGameSpinData)
        {
            return new EmptyExtraGameSettings();
        }

        public Result<SpinResult, ErrorCode> UpdateUserLastSpinData(int level, UserGameSpinData userGameSpinData, RequestContext<BonusArgs> requestContext, BonusResult bonusResult)
        {
            throw new NotImplementedException();
        }
    }
}

using Slot.Core.RandomNumberGenerators;
using Slot.Games.BullRush.Models;
using Slot.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slot.Games.BullRush
{
    public static class BullRushCommon
    {
        public static List<List<decimal>> GenerateBonusRacing(decimal selectedBonusRacingPrize, decimal lineBet, int funplayKey)
        {
            var distributedAllRows = new List<List<decimal>>();

            for (int i = 0; i < BullRushConfiguration.HeightBonusRacing; i++)
            {
                var rowBonus = new List<decimal>() { 0, 0, 0 };

                var randomIndex = RandomNumberEngine.Next(0, 2);

                rowBonus[randomIndex] = (selectedBonusRacingPrize / BullRushConfiguration.HeightBonusRacing) * lineBet;

                distributedAllRows.Add(rowBonus);
            }

            return distributedAllRows;
        }

        public static List<List<decimal>> GenerateRacing(Wheel variantWheel, List<int> chosenTreasureItems, BullRushSpinResult sr)
        {
            var lineBet = sr.SpinBet.LineBet;
            var funplayKey = sr.SpinBet.FunPlayDemoKey;
            var w = new Wheel(BullRushConfiguration.Rows);
            var distributedAllRows = new List<List<decimal>>();
            var inventoryOfThreePowerUps = new List<int>(sr.InventoryOfThreePowerUps);

            for (int row = 0; row < variantWheel.Height; row++)
            {
                var distributeListed = new List<decimal>();

                // Coin
                if (variantWheel[0][row] > 0)
                {
                    for (int j = 0; j < variantWheel[0][row]; j++)
                    {
                        distributeListed.Add(lineBet);
                    }
                }

                // Obstacle
                if (variantWheel[1][row] > 0)
                {
                    for (int j = 0; j < variantWheel[1][row]; j++)
                    {
                        distributeListed.Add(BullRushConfiguration.OBSTACLE);
                    }
                }

                // Power up
                if (variantWheel[2][row] > 0 && inventoryOfThreePowerUps.Count > 0)
                {
                    var randomIndex = RandomNumberEngine.Next(0, inventoryOfThreePowerUps.Count() - 1);

                    var powerUp = inventoryOfThreePowerUps[randomIndex];

                    inventoryOfThreePowerUps.RemoveAt(randomIndex);

                    distributeListed.Add(powerUp);
                }

                // Chest-Treasure
                if (variantWheel[3][row] > 0 && chosenTreasureItems.IndexOf(row) != -1)
                {
                    var indexChest = sr.InventoryList.FindIndex(x => x == BullRushConfiguration.TREASURE);

                    if (indexChest != -1)
                    {
                        for (int j = 0; j < variantWheel[3][row]; j++)
                        {
                            var random = RandomNumberEngine.NextDouble();

                            var valueOfTreasure = BullRushConfiguration.TreasureDrawing.FirstOrDefault(x => random <= x.Key).Value;

                            var chestValue = valueOfTreasure * lineBet;

                            sr.InventoryList.Remove(indexChest);

                            distributeListed.Add(chestValue);
                        }
                    }

                }

                var currentItemsOfDistributed = distributeListed.Count;

                for (int i = 0; i < BullRushConfiguration.WidthOfRace - currentItemsOfDistributed; i++)
                {
                    distributeListed.Add(-1);
                }

                var randomDistributed = distributeListed.Shuffle();

                distributedAllRows.Add(randomDistributed);
            }

            return distributedAllRows;
        }
       
        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = RandomNumberEngine.Next(n);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static List<int> GetRange(List<int> list, int startIndex, int count)
        {
            var result = list.GetRange(startIndex - 1, count);

            return result;
        }
    }
}

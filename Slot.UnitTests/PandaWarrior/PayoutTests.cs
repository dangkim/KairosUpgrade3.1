using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PandaWarrior;
using Slot.Games.PandaWarrior.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Slot.Core.Extensions;
using Slot.Games.PandaWarrior.BonusFeatures;

namespace Slot.UnitTests.PandaWarrior
{
    [TestFixture]
    public class PayoutTests
    {
        private decimal TestPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var PandaWarriorModule = new PandaWarriorModule(null);
            var requestContext = new RequestContext<SpinArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            var arrstr = strwheel.Split(',');
            var arr = Array.ConvertAll(arrstr, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = PandaWarriorConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = PandaWarriorConfiguration.Lines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new PandaWarriorCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = PandaWarriorConfiguration.Lines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(PandaWarriorConfiguration.Width, PandaWarriorConfiguration.Height, arr)
            };

            var win = PandaWarriorCommon.CalculateWin(sr);

            Console.WriteLine("--- WIN POSITION ---");
            foreach (PandaWarriorWinPosition wp in sr.WinPositions)
                Console.WriteLine(String.Format("[LINE:{0} MUL:{1} WIN:{2}]", wp.Line, wp.Multiplier, wp.Win));

            Console.WriteLine();
            Console.WriteLine("--- WIN TABLE ---");
            foreach (PandaWarriorTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[CARD:{0} COUNT:{1} WILD:{2}]", tw.Card, tw.Count, tw.Wild));

            return win;
        }

        [TestCase("8,3,7,7,1,0,2,1,9,0,1,7,5,0,3", "3,38,10,38,22", 0.01, ExpectedResult = 1.1)]
        public decimal TestPandaWarriorPayout(string strwheel, string strIndices, decimal betperline)
        {
            return TestCollapsingPayout(strwheel, betperline, strIndices, MapWheelEncoding[WheelEncoding.Local]);
        }

        [TestCase("6,0,1,3,0,4,0,4,1,1,3,4,1,4,6", "25,8,16,18,6", 0.01, ExpectedResult = 1.9)]
        public decimal TestPandaWarriorFreespinPayout(string strwheel, string strIndices, decimal betperline)
        {
            return TestCollapsingFreespinPayout(strwheel, betperline, strIndices, MapWheelEncoding[WheelEncoding.Local]);
        }

        private decimal TestCollapsingPayout(string strwheel, decimal betperline, string strIndices, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var PandaWarriorModule = new PandaWarriorModule(null);
            var maxWin = 0m;
            var totalWin = 0m;
            var maxIndexPosition = new List<int>();
            var requestContext = new RequestContext<SpinArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);
            var summData = new SummaryData();

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            string[] arrstr = strwheel.Split(',');
            int[] arr = Array.ConvertAll(arrstr, int.Parse);

            string[] arrIndices = strIndices.Split(',');
            int[] indices = Array.ConvertAll(arrIndices, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = PandaWarriorConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = PandaWarriorConfiguration.Lines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new PandaWarriorCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = PandaWarriorConfiguration.Lines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(PandaWarriorConfiguration.Width, PandaWarriorConfiguration.Height, arr)
            };

            sr.TopIndices = new List<int>(indices);

            totalWin = PandaWarriorCommon.CalculateWin(sr);

            Console.WriteLine();
            Console.WriteLine("--- WIN POSITION ---");
            foreach (PandaWarriorWinPosition wp in sr.WinPositions)
                Console.WriteLine(String.Format("[LINE:{0} MUL:{1} WIN:{2}]", wp.Line, wp.Multiplier, wp.Win));

            Console.WriteLine();
            Console.WriteLine("--- WIN TABLE ---");
            foreach (PandaWarriorTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[CARD:{0} COUNT:{1} WILD:{2}]", tw.Card, tw.Count, tw.Wild));

            if (PandaWarriorCommon.CheckFreeSpin(sr.Wheel))
            {
                sr.IsBonus = true;
                sr.IsFreeSpin = true;
                sr.InitialBonusPositions = PandaWarriorFreeSpinFeature.CreatePosition(sr);
                sr.CurrentStep = 1;
                sr.CurrentFreeSpinCounter = PandaWarriorConfiguration.FreeSpinCount;
                sr.NumOfFreeSpin = PandaWarriorConfiguration.FreeSpinCount;
            }
            else if (sr.Collapse)
            {
                sr.IsBonus = true;
            }

            if (sr.HasBonus)
            {
                var bonusCreated = PandaWarriorModule.CreateBonus(sr);

                var bonus = bonusCreated.Value;

                bonus.SpinTransactionId = sr.TransactionId;
                bonus.GameResult = sr;

                var requestBonusContext = new RequestContext<BonusArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web)
                {
                    Currency = new Currency() { Id = sb.CurrencyId },
                    Parameters = new BonusArgs() { Bonus = "CollapsingSpin" },
                    GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId }
                };

                BonusResult bonusResult;
                int step = bonus.CurrentStep;

                do
                {
                    var entity = new BonusEntity
                    {
                        UserId = ugk.UserId,
                        GameId = PandaWarriorConfiguration.GameId,
                        Guid = bonus.Guid.ToString("N"),
                        Data = bonus.ToByteArray(),
                        BonusType = bonus.GetType().Name,
                        Version = 2,
                        IsOptional = bonus.IsOptional,
                        IsStarted = bonus.IsStarted,
                        RoundId = sr.RoundId
                    };

                    bonusResult = PandaWarriorModule.ExecuteBonus(PandaWarriorConfiguration.LevelOne, entity, requestBonusContext).Value;
                    var collapsingBonusResult = bonusResult as PandaWarriorCollapsingBonusResult;

                    var win = collapsingBonusResult.Win;

                    if (win > 0)
                    {
                        totalWin += win;
                    }

                    var maxTopIndices = collapsingBonusResult.SpinResult.TopIndices.ToList();

                    if (totalWin > maxWin)
                    {
                        maxWin = totalWin;

                        maxIndexPosition = maxTopIndices;
                    }

                    Console.WriteLine("--- WIN POSITION ---");
                    foreach (PandaWarriorWinPosition wp in sr.WinPositions)
                        Console.WriteLine(String.Format("[LINE:{0} MUL:{1} WIN:{2}]", wp.Line, wp.Multiplier, wp.Win));

                    Console.WriteLine();
                    Console.WriteLine("--- WIN TABLE ---");
                    foreach (PandaWarriorTableWin tw in sr.TableWins)
                        Console.WriteLine(String.Format("[CARD:{0} COUNT:{1} WILD:{2}]", tw.Card, tw.Count, tw.Wild));

                    bonus = bonusResult.Bonus;
                }
                while (!bonusResult.IsCompleted && bonusResult.Bonus != null);
            }

            Console.WriteLine($"Win            : {totalWin}");
            return totalWin;
        }


        private decimal TestCollapsingFreespinPayout(string strwheel, decimal betperline, string strIndices, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var PandaWarriorModule = new PandaWarriorModule(null);
            var maxWin = 0m;
            var totalWin = 0m;
            var maxIndexPosition = new List<int>();
            var requestContext = new RequestContext<SpinArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);
            var summData = new SummaryData();

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            string[] arrstr = strwheel.Split(',');
            int[] arr = Array.ConvertAll(arrstr, int.Parse);

            string[] arrIndices = strIndices.Split(',');
            int[] indices = Array.ConvertAll(arrIndices, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = PandaWarriorConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = PandaWarriorConfiguration.Lines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new PandaWarriorCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = PandaWarriorConfiguration.Lines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(PandaWarriorConfiguration.Width, PandaWarriorConfiguration.Height, arr)
            };

            sr.TopIndices = new List<int>(indices);

            totalWin = PandaWarriorCommon.CalculateWinFreeSpin(sr);

            Console.WriteLine();
            Console.WriteLine("--- WIN POSITION ---");
            foreach (PandaWarriorWinPosition wp in sr.WinPositions)
                Console.WriteLine(String.Format("[LINE:{0} MUL:{1} WIN:{2}]", wp.Line, wp.Multiplier, wp.Win));

            Console.WriteLine();
            Console.WriteLine("--- WIN TABLE ---");
            foreach (PandaWarriorTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[CARD:{0} COUNT:{1} WILD:{2}]", tw.Card, tw.Count, tw.Wild));

            if (sr.Collapse)
            {
                sr.IsBonus = true;
            }

            if (sr.HasBonus)
            {
                var bonusCreated = PandaWarriorModule.CreateBonus(sr);

                var bonus = bonusCreated.Value;
                sr.IsFreeSpin = true;
                bonus.SpinTransactionId = sr.TransactionId;
                bonus.GameResult = sr;


                var requestBonusContext = new RequestContext<BonusArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web)
                {
                    Currency = new Currency() { Id = sb.CurrencyId },
                    Parameters = new BonusArgs() { Bonus = "CollapsingSpin" },
                    GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId }
                };

                BonusResult bonusResult;
                int step = bonus.CurrentStep;
                var win = 0m;

                do
                {
                    var entity = new BonusEntity
                    {
                        UserId = ugk.UserId,
                        GameId = PandaWarriorConfiguration.GameId,
                        Guid = bonus.Guid.ToString("N"),
                        Data = bonus.ToByteArray(),
                        BonusType = bonus.GetType().Name,
                        Version = 2,
                        IsOptional = bonus.IsOptional,
                        IsStarted = bonus.IsStarted,
                        RoundId = sr.RoundId
                    };

                    bonusResult = PandaWarriorModule.ExecuteBonus(PandaWarriorConfiguration.LevelOne, entity, requestBonusContext).Value;
                    var collapsingBonusResult = bonusResult as PandaWarriorCollapsingBonusResult;

                    win = collapsingBonusResult.Win;

                    if (win > 0)
                    {
                        totalWin += win;
                    }

                    var maxTopIndices = collapsingBonusResult.SpinResult.TopIndices.ToList();

                    if (totalWin > maxWin)
                    {
                        maxWin = totalWin;

                        maxIndexPosition = maxTopIndices;
                    }

                    Console.WriteLine("--- WIN POSITION ---");
                    foreach (PandaWarriorWinPosition wp in collapsingBonusResult.SpinResult.WinPositions)
                        Console.WriteLine(String.Format("[LINE:{0} MUL:{1} WIN:{2}]", wp.Line, wp.Multiplier, wp.Win));

                    Console.WriteLine();
                    Console.WriteLine("--- WIN TABLE ---");
                    foreach (PandaWarriorTableWin tw in collapsingBonusResult.SpinResult.TableWins)
                        Console.WriteLine(String.Format("[CARD:{0} COUNT:{1} WILD:{2}]", tw.Card, tw.Count, tw.Wild));

                    bonus = bonusResult.Bonus;
                }
                while (win > 0);
            }

            Console.WriteLine($"Win            : {totalWin}");
            return totalWin;
        }

        private static readonly Dictionary<WheelEncoding, Func<int, int, int[], Wheel>> MapWheelEncoding = new Dictionary<WheelEncoding, Func<int, int, int[], Wheel>>()
        {
            { WheelEncoding.Local, WheelEncodingLocal },
        };

        public static Wheel WheelEncodingLocal(int width, int height, int[] arr)
        {
            int currentIndex = 0;
            var w = new Wheel(new List<int>() { 3, 3, 3, 3, 3 });
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < w.Rows[i]; ++j)
                {
                    if (i > 0)
                    {
                        w[i].Add(arr[currentIndex + j]);
                    }
                    else
                    {
                        w[i].Add(arr[j]);
                    }
                }

                currentIndex = currentIndex + w.Rows[i];
            }
            return w;
        }
    }
}

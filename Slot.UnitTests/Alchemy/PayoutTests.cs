using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.AlchemyReels;
using Slot.Games.AlchemyReels.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Slot.UnitTests.Alchemy
{
    [TestFixture]
    public class PayoutTests
    {
        private decimal TestPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var alchemyReelsModule = new AlchemyReelsModule(null);
            var requestContext = new RequestContext<SpinArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);

            AlchemyReelsCommon.CreateWheels(new List<int>() { 5, 5, 5 });

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            var arrstr = strwheel.Split(',');
            var arr = Array.ConvertAll(arrstr, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = AlchemyReelsConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = AlchemyReelsConfiguration.BettingLines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new AlchemyReelsCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = AlchemyReelsConfiguration.BettingLines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(AlchemyReelsConfiguration.Width, AlchemyReelsConfiguration.Height, arr)
            };

            var win = AlchemyReelsCommon.CalculateWin(sr);

            Console.WriteLine("--- WIN POSITION ---");
            foreach (AlcheryReelsWinPosition wp in sr.WinPositions)
                Console.WriteLine(String.Format("[LINE:{0} MUL:{1} WIN:{2}]", wp.Line, wp.Multiplier, wp.Win));

            Console.WriteLine();
            Console.WriteLine("--- WIN TABLE ---");
            foreach (AlchemyReelTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[CARD:{0} COUNT:{1} WILD:{2}]", tw.Card, tw.Count, tw.Wild));

            return win;
        }

        [TestCase("8,8,8,8,8,8,4,3,5", 1, ExpectedResult = 1593)]
        public decimal TestAlchemyReelsPayout(string strwheel, decimal betperline)
        {
            return TestCollapsingPayout(strwheel, betperline, MapWheelEncoding[WheelEncoding.Local]);
        }

        private decimal TestCollapsingPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var alchemyReelsModule = new AlchemyReelsModule(null);
            var maxWin = 0m;
            var totalWin = 0m;
            var maxIndexPosition = new List<int>();
            var requestContext = new RequestContext<SpinArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);
            var summData = new SummaryData();
            AlchemyReelsCommon.CreateWheels(new List<int>() { 3, 3, 3 });

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            string[] arrstr = strwheel.Split(',');
            int[] arr = Array.ConvertAll(arrstr, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = AlchemyReelsConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = AlchemyReelsConfiguration.BettingLines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new AlchemyReelsCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = AlchemyReelsConfiguration.BettingLines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(AlchemyReelsConfiguration.Width, AlchemyReelsConfiguration.Height, arr)
            };

            sr.TopIndices = new List<int>() { 6, 6, 49 };

            totalWin = AlchemyReelsCommon.CalculateWin(sr);

            Console.WriteLine();
            Console.WriteLine("--- POSITION TABLE ---");
            foreach (AlchemyReelTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[WIN:{0} SYM:{1} COUNT:{2}]", tw.Win, tw.Card, tw.Count));

            if (sr.HasBonus)
            {
                var bonusCreated = alchemyReelsModule.CreateBonus(sr);

                var bonus = bonusCreated.Value;

                bonus.SpinTransactionId = sr.TransactionId;
                bonus.GameResult = sr;
                                
                var requestBonusContext = new RequestContext<BonusArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);

                requestBonusContext.Currency = new Currency() { Id = sb.CurrencyId };
                requestBonusContext.Parameters = new BonusArgs() { Bonus = "CollapsingSpin" };
                requestBonusContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                BonusResult bonusResult;
                int step = bonus.CurrentStep;

                do
                {
                    var entity = new BonusEntity
                    {
                        UserId = ugk.UserId,
                        GameId = AlchemyReelsConfiguration.GameId,
                        Guid = bonus.Guid.ToString("N"),
                        Data = bonus.ToByteArray(),
                        BonusType = bonus.GetType().Name,
                        Version = 2,
                        IsOptional = bonus.IsOptional,
                        IsStarted = bonus.IsStarted,
                        RoundId = sr.RoundId
                    };

                    bonusResult = alchemyReelsModule.ExecuteBonus(AlchemyReelsConfiguration.LevelOne, entity, requestBonusContext).Value;
                    var alchemyFreeCollapsingSpinResult = bonusResult as AlchemyFreeCollapsingSpinResult;

                    var win = alchemyFreeCollapsingSpinResult.Win;

                    if (win > 0)
                    {
                        totalWin += win;
                    }

                    var maxTopIndices = alchemyFreeCollapsingSpinResult.SpinResult.TopIndices.ToList();

                    if (totalWin > maxWin)
                    {
                        maxWin = totalWin;

                        maxIndexPosition = maxTopIndices;
                    }

                    Console.WriteLine("--- POSITION TABLE ---");
                    foreach (AlchemyReelTableWin tw in alchemyFreeCollapsingSpinResult.SpinResult.TableWins)
                        Console.WriteLine(String.Format("[WIN:{0} SYM:{1} COUNT:{2}]", tw.Win, tw.Card, tw.Count));                    

                    bonus = bonusResult.Bonus;
                }
                while (!bonusResult.IsCompleted && bonusResult.Bonus != null);
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
            var w = new Wheel(new List<int>() { 3, 3, 3 });
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

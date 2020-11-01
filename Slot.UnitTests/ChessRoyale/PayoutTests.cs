using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slot.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Slot.Games.ChessRoyale;
using Slot.Games.ChessRoyale.Models;

namespace Slot.UnitTests.ChessRoyale
{
    [TestFixture]
    public class PayoutTests
    {
        [TestCase("2,6,3,6,0,1,2,6,1,4,5,1,3,7,1", 0.5, ExpectedResult = 17.50)]
        [TestCase("0,3,1,0,1,4,0,3,4,0,7,1,3,0,5", 0.5, ExpectedResult = 12.50)]
        public decimal TestChessRoyalePayout(string strwheel, decimal betperline)
        {
            return TestPayout(strwheel, betperline, MapWheelEncoding[WheelEncoding.Local]);
        }

        private decimal TestPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var sdt = DateTime.Now;

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<ChessRoyaleModule>();

            var chessRoyaleModule = new ChessRoyaleModule(logger);

            var totalWin = 0m;
            List<int> maxIndexPosition = new List<int>();
            RequestContext<SpinArgs> requestContext = new RequestContext<SpinArgs>("", ChessRoyaleConfiguration.GameName, PlatformType.Web);
            var summData = new SummaryData();

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            string[] arrstr = strwheel.Split(',');
            int[] arr = Array.ConvertAll(arrstr, int.Parse);

            UserGameKey ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = ChessRoyaleConfiguration.GameId,
                Level = 1
            };

            SpinBet sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = ChessRoyaleConfiguration.Lines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            ChessRoyaleSpinResult sr = new ChessRoyaleSpinResult(ugk)
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = ChessRoyaleConfiguration.Lines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(ChessRoyaleConfiguration.Width, ChessRoyaleConfiguration.Height, arr)
            };


            totalWin = ChessRoyaleCommon.CalculateWin(sr, 1);

            if (sr.HasBonus)
            {
                var bonusCreated = chessRoyaleModule.CreateBonus(sr);

                var bonus = bonusCreated.Value;

                bonus.SpinTransactionId = sr.TransactionId;
                bonus.GameResult = sr;

                RequestContext<BonusArgs> requestBonusContext = new RequestContext<BonusArgs>("", ChessRoyaleConfiguration.GameName, PlatformType.Web);

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
                        GameId = ChessRoyaleConfiguration.GameId,
                        Guid = bonus.Guid.ToString("N"),
                        Data = bonus.ToByteArray(),
                        BonusType = bonus.GetType().Name,
                        Version = 2,
                        IsOptional = bonus.IsOptional,
                        IsStarted = bonus.IsStarted,
                        RoundId = sr.RoundId
                    };

                    bonusResult = chessRoyaleModule.ExecuteBonus(ChessRoyaleConfiguration.LevelOne, entity, requestBonusContext).Value;
                    var chessRoyaleFreeSpinResult = bonusResult as ChessRoyaleFreeSpinResult;

                    var win = chessRoyaleFreeSpinResult.Win;

                    if (win > 0)
                    {
                        totalWin += win;
                    }

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
            Wheel w = new Wheel(new List<int>() { 3, 3, 3, 3, 3 });
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

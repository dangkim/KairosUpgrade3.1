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
using Slot.Games.FountainOfFortune;
using Slot.Games.FountainOfFortune.Models;

namespace Slot.UnitTests.WealthFountain
{
    [TestFixture]
    public class PayoutTests
    {
        [TestCase("7,10,1,11,1,6,11,7,5,11,0,8,6,4,0", 0.20, ExpectedResult = 26.00)]
        [TestCase("7,0,5,9,11,3,11,4,2,0,5,8,2,9,5", 0.20, ExpectedResult = 20.40)]
        public decimal TestFountainOfFortunePayout(string strwheel, decimal betperline)
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
            var logger = logFactory.CreateLogger<FountainOfFortuneModule>();

            var FountainOfFortuneModule = new FountainOfFortuneModule(logger);

            var maxWin = 0m;
            var totalWin = 0m;
            List<int> maxIndexPosition = new List<int>();
            RequestContext<SpinArgs> requestContext = new RequestContext<SpinArgs>("", FountainOfFortuneConfiguration.GameName, PlatformType.Web);
            var summData = new SummaryData();

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            string[] arrstr = strwheel.Split(',');
            int[] arr = Array.ConvertAll(arrstr, int.Parse);

            UserGameKey ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = FountainOfFortuneConfiguration.GameId,
                Level = 1
            };

            SpinBet sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = FountainOfFortuneConfiguration.Lines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            FountainOfFortuneSpinResult sr = new FountainOfFortuneSpinResult(ugk)
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = FountainOfFortuneConfiguration.Lines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(FountainOfFortuneConfiguration.Width, FountainOfFortuneConfiguration.Height, arr)
            };


            totalWin = FountainOfFortuneCommon.CalculateWin(sr, 1);

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

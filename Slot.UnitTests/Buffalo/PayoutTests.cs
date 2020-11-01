using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Slot.Games.Buffalo;
using Slot.Model;

namespace Slot.UnitTests.Buffalo
{
    [TestFixture]
    public class PayoutTests
    {
        private static EpicReel Encoding(int[] array)
        {
            var wheel = new EpicReel(6);
            for (var i = 0; i < 6; ++i)
            {
                var strip = new int[4];
                for (var j = 0; j < 4; ++j)
                    strip[j] = array[i * 4 + j];
                wheel.Reels.Add(strip);
            }

            return wheel;
        }

        [TestCase("0,1,13,13, 0,13,12,13, 0,13,13,13, 13,13,12,13, 12,13,13,13, 1,1,1,1", "1,2,3,1,5,1", TestName = "Wild Multiplier ", ExpectedResult = 700)]
        [TestCase("0,1,13,13, 0,13,13,13, 0,13,13,13, 13,13,13,13, 12,13,13,13, 1,1,1,1", "0,0,0,0,0,0", TestName = "Wild Multiplier ", ExpectedResult = 10)]
        public decimal PayoutWithWildMultiplier(string wheelString, string wildMultiplierString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var wildMultiplier = wildMultiplierString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            var result = Payout.Calculate(wheel.Reels, 1, wildMultiplier);
            return result.Payable;
        }

        [TestCase("0,1,13,13, 0,13,12,13, 0,13,13,13, 13,13,12,13, 12,13,13,13, 1,1,1,1",
            TestName = "Win Position case 1",
            ExpectedResult = "[1,1,1,3,1,0];[1,3,1,3,1,0]")]
        [TestCase("0,1,0,13, 0,13,13,13, 0,13,13,13, 13,13,12,13, 12,13,13,13, 1,1,1,1",
            TestName = "Win Position case 2",
            ExpectedResult = "[1,1,1,3,1,0];[3,1,1,3,1,0]")]
        public string WinPositionWithWildMultiplier(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var wildMultiplier = new List<int> { 1, 1, 1, 1, 1, 1 };
            var result = Payout.Calculate(wheel.Reels, 1, wildMultiplier);
            var position1 = $"[{string.Join(',',result.WinPositions[0].RowPositions)}]";
            var position2 = $"[{string.Join(',',result.WinPositions[1].RowPositions)}]";
            return $"{position1};{position2}";
        }
    }
}
namespace Slot.UnitTests.GeniesLuck
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    internal class PayoutTests
    {
        private static List<int[]> Encoding(int[] array)
        {
            var pattern = new[] { 3, 4, 4, 4, 3 };
            int skip = 0;
            var wheel = new List<int[]>();
            for (var i = 0; i < 5; ++i)
            {
                var height = pattern[i];
                var strip = new int[height];
                for (var j = 0; j < height; ++j)
                {
                    strip[j] = array[skip + j];
                }
                skip += height;
                wheel.Add(strip);
            }

            return wheel;
        }

        [TestCase("0,1,9, 12,10,10,10, 0,13,13,10, 13,9,12,10, 12,13,9", TestName = "Jack One", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12,10, 12,13,13,10, 13,13,9,10, 12,9,13", TestName = "Jack Two", ExpectedResult = 0)]
        [TestCase("0,-1,-1, 0,12,12,10, 0,12,13,10, 9,13,12,10, 12,9,13", TestName = "Jack Three", ExpectedResult = 2 + 2)]
        [TestCase("0,-1,-1, 0,12,12,10, 0,12,13,10, 0,13,12,10, 12,9,13", TestName = "Jack Four", ExpectedResult = 4 + 2)]
        [TestCase("0,-1,-1, 0,12,12,10,, 0,12,13,10, 0,13,12,10, 11,13,10", TestName = "Jack Five", ExpectedResult = 10 + 2)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Games.GeniesLuck.Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase("11,10,10, 0,10,10,10, 11,13,13,10, 0,10,12,10, 12,13,10", TestName = "Simple Payout", ExpectedResult = 6)]
        public decimal TestSimplePayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Games.GeniesLuck.Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase("0,10,10, 0,10,10,10, 11,13,13,10, 11,10,12,10, 11,13,10", TestName = "Both Way of Payout", ExpectedResult = 150)]
        public decimal TestBothWayOfPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Games.GeniesLuck.Payout.CalculateBothWay(wheel, 1);
            return result.win;
        }

        [TestCase(1, TestName = "Of Kind 1 Jack", ExpectedResult = 0)]
        [TestCase(2, TestName = "Of Kind 2 Jack", ExpectedResult = 0)]
        [TestCase(3, TestName = "Of Kind 3 Jack", ExpectedResult = 2)]
        [TestCase(4, TestName = "Of Kind 4 Jack", ExpectedResult = 4)]
        [TestCase(5, TestName = "Of Kind 5 Jack", ExpectedResult = 10)]
        public decimal TestPayout(int ofkind)
        {
            return Games.GeniesLuck.Payout.GetOdds(0, ofkind);
        }
    }
}
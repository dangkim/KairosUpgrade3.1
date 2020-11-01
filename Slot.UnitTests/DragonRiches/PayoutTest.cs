namespace Slot.UnitTests.DragonRiches
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    internal class PayoutTest
    {
        private static List<int[]> Encoding(int[] array)
        {
            var wheel = new List<int[]>();
            for (var i = 0; i < 5; ++i)
            {
                var strip = new int[3];
                for (var j = 0; j < 3; ++j)
                {
                    strip[j] = array[i * 3 + j];
                }

                wheel.Add(strip);
            }

            return wheel;
        }

        [TestCase("0,1,10, 0,13,12, 0,13,9, 13,13,12, 12,13,13", "", TestName = "Less Scatter", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12, 0,13,9, 13,13,12, 12,13,13", "", TestName = "Two Scatter", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12, 0,13,13, 13,9,12, 12,13,9", "3,0,0,2,3", TestName = "Three Scatter", ExpectedResult = 2 * 30)]
        [TestCase("0,1,9, 0,13,9, 0,13,13, 13,13,9, 12,9,13", "3,3,0,3,2", TestName = "Four Scatter", ExpectedResult = 15 * 30)]
        [TestCase("0,1,9, 0,9,12, 0,9,13, 9,13,12, 12,9,13", "3,2,2,1,2", TestName = "Five Scatter", ExpectedResult = 100 * 30)]
        public decimal TestScatterPayout(string wheelString, string position)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Slot.Games.DragonRiches.Payout.CalculateScatter(wheel, 1);
            Assert.AreEqual(result.positions.Any() ? string.Join(",", result.positions.First().RowPositions) : "", position);
            return result.win;
        }

        [TestCase("0,1,9, 12,13,12, 0,13,13, 13,9,12, 12,13,9", TestName = "Nine One", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12, 12,13,13, 13,13,9, 12,9,13", TestName = "Nine Two", ExpectedResult = 0)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 9,13,12, 12,9,13", TestName = "Nine Three", ExpectedResult = 5)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 0,12,13, 12,9,13", TestName = "Nine Four", ExpectedResult = 10)]
        [TestCase("0,-1,-1, 0,13,12, 0,11,9, 0,13,12, 0,13,10", TestName = "Nine Five", ExpectedResult = 50)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Games.DragonRiches.Payout.CalculateNonScatter(wheel, 1, 1);
            return result.win;
        }

        [TestCase("0,12,9, 0,11,12, 0,12,13, 11,9,11, 12,13,9", TestName = "Payout Nine - Scatter", ExpectedResult = 2 * 30 + 5)]
        public decimal TestSimplePayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            var result = Games.DragonRiches.Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase(1, TestName = "Of Kind 1 Nine", ExpectedResult = 0)]
        [TestCase(2, TestName = "Of Kind 2 Nine", ExpectedResult = 0)]
        [TestCase(3, TestName = "Of Kind 3 Nine", ExpectedResult = 5)]
        [TestCase(4, TestName = "Of Kind 4 Nine", ExpectedResult = 10)]
        [TestCase(5, TestName = "Of Kind 5 Nine", ExpectedResult = 50)]
        public decimal TestPayout(int ofkind)
        {
            return Games.DragonRiches.Payout.GetOdds(0, ofkind);
        }
    }
}
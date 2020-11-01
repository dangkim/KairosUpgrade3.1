namespace Slot.UnitTests.SevenWonders
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

        [TestCase("0,1,9, 12,13,12, 0,13,13, 13,9,12, 12,13,9", TestName = "TajMahal One", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12, 12,13,13, 13,13,9, 12,9,13", TestName = "TajMahal Two", ExpectedResult = 0)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 9,13,12, 12,9,13", TestName = "TajMahal Three", ExpectedResult = 5)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 0,12,13, 12,9,13", TestName = "TajMahal Four", ExpectedResult = 10)]
        [TestCase("0,-1,-1, 0,13,12, 0,11,9, 0,13,12, 0,13,10", TestName = "TajMahal Five", ExpectedResult = 25)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Games.SevenWonders.Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase("0,1,9, 0,1,12, 0,1,13, 11,9,11, 12,13,9", TestName = "Simple payout", ExpectedResult = 5 + 5)]
        public decimal TestSimplePayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            var result = Games.SevenWonders.Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase(1, TestName = "Of Kind 1 TajMahal", ExpectedResult = 0)]
        [TestCase(2, TestName = "Of Kind 2 TajMahal", ExpectedResult = 0)]
        [TestCase(3, TestName = "Of Kind 3 TajMahal", ExpectedResult = 5)]
        [TestCase(4, TestName = "Of Kind 4 TajMahal", ExpectedResult = 10)]
        [TestCase(5, TestName = "Of Kind 5 TajMahal", ExpectedResult = 25)]
        public decimal TestPayout(int ofkind)
        {
            return Games.SevenWonders.Payout.GetOdds(0, ofkind);
        }
    }
}
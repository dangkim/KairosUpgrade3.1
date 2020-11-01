namespace Slot.UnitTests.FortuneChimes
{
    using NUnit.Framework;
    using Slot.Games.FortuneChimes;
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
                    strip[j] = array[3 * i + j];
                }
                wheel.Add(strip);
            }

            return wheel;
        }

        [TestCase("0,1,9, 12,10,10, 0,13,13, 13,9,12, 12,13,9", TestName = "Ten One", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12, 12,13,13, 13,13,9, 12,9,13", TestName = "Ten Two", ExpectedResult = 0)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 9,13,12, 12,9,13", TestName = "Ten Three", ExpectedResult = 5)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 0,13,12, 12,9,13", TestName = "Ten Four", ExpectedResult = 15)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 0,13,12, 0,13,10", TestName = "Ten Five", ExpectedResult = 50)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase("0,1,9, 12,10,14, 10,13,13, 13,10,12, 12,13,9", TestName = "Scatter Payout", ExpectedResult = 30)]
        public decimal TestScatterPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase("0,7,11, 0,10,15, 0,7,10, 11,12,13, 11,12,13", TestName = "Simple Payout", ExpectedResult = 55)]
        public decimal TestSimplePayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase(0, 1, TestName = "Of Kind 1 Ten", ExpectedResult = 0)]
        [TestCase(0, 2, TestName = "Of Kind 2 Ten", ExpectedResult = 0)]
        [TestCase(0, 3, TestName = "Of Kind 3 Ten", ExpectedResult = 5)]
        [TestCase(0, 4, TestName = "Of Kind 4 Ten", ExpectedResult = 15)]
        [TestCase(0, 5, TestName = "Of Kind 5 Ten", ExpectedResult = 50)]
        [TestCase(1, 1, TestName = "Of Kind 1 Jack", ExpectedResult = 0)]
        [TestCase(1, 2, TestName = "Of Kind 2 Jack", ExpectedResult = 0)]
        [TestCase(1, 3, TestName = "Of Kind 3 Jack", ExpectedResult = 5)]
        [TestCase(1, 4, TestName = "Of Kind 4 Jack", ExpectedResult = 15)]
        [TestCase(1, 5, TestName = "Of Kind 5 Jack", ExpectedResult = 50)]
        [TestCase(2, 1, TestName = "Of Kind 1 Queen", ExpectedResult = 0)]
        [TestCase(2, 2, TestName = "Of Kind 2 Queen", ExpectedResult = 0)]
        [TestCase(2, 3, TestName = "Of Kind 3 Queen", ExpectedResult = 5)]
        [TestCase(2, 4, TestName = "Of Kind 4 Queen", ExpectedResult = 15)]
        [TestCase(2, 5, TestName = "Of Kind 5 Queen", ExpectedResult = 50)]
        [TestCase(3, 1, TestName = "Of Kind 1 King", ExpectedResult = 0)]
        [TestCase(3, 2, TestName = "Of Kind 2 King", ExpectedResult = 0)]
        [TestCase(3, 3, TestName = "Of Kind 3 King", ExpectedResult = 6)]
        [TestCase(3, 4, TestName = "Of Kind 4 King", ExpectedResult = 20)]
        [TestCase(3, 5, TestName = "Of Kind 5 King", ExpectedResult = 75)]
        [TestCase(4, 1, TestName = "Of Kind 1 Ace", ExpectedResult = 0)]
        [TestCase(4, 2, TestName = "Of Kind 2 Ace", ExpectedResult = 0)]
        [TestCase(4, 3, TestName = "Of Kind 3 Ace", ExpectedResult = 6)]
        [TestCase(4, 4, TestName = "Of Kind 4 Ace", ExpectedResult = 20)]
        [TestCase(4, 5, TestName = "Of Kind 5 Ace", ExpectedResult = 75)]
        [TestCase(5, 1, TestName = "Of Kind 1 Dagger", ExpectedResult = 0)]
        [TestCase(5, 2, TestName = "Of Kind 2 Dagger", ExpectedResult = 0)]
        [TestCase(5, 3, TestName = "Of Kind 3 Dagger", ExpectedResult = 8)]
        [TestCase(5, 4, TestName = "Of Kind 4 Dagger", ExpectedResult = 30)]
        [TestCase(5, 5, TestName = "Of Kind 5 Dagger", ExpectedResult = 100)]
        [TestCase(6, 1, TestName = "Of Kind 1 Scroll", ExpectedResult = 0)]
        [TestCase(6, 2, TestName = "Of Kind 2 Scroll", ExpectedResult = 0)]
        [TestCase(6, 3, TestName = "Of Kind 3 Scroll", ExpectedResult = 8)]
        [TestCase(6, 4, TestName = "Of Kind 4 Scroll", ExpectedResult = 30)]
        [TestCase(6, 5, TestName = "Of Kind 5 Scroll", ExpectedResult = 100)]
        [TestCase(7, 1, TestName = "Of Kind 1 OldMan", ExpectedResult = 0)]
        [TestCase(7, 2, TestName = "Of Kind 2 OldMan", ExpectedResult = 0)]
        [TestCase(7, 3, TestName = "Of Kind 3 OldMan", ExpectedResult = 10)]
        [TestCase(7, 4, TestName = "Of Kind 4 OldMan", ExpectedResult = 40)]
        [TestCase(7, 5, TestName = "Of Kind 5 OldMan", ExpectedResult = 150)]
        [TestCase(8, 1, TestName = "Of Kind 1 Prince", ExpectedResult = 0)]
        [TestCase(8, 2, TestName = "Of Kind 2 Prince", ExpectedResult = 0)]
        [TestCase(8, 3, TestName = "Of Kind 3 Prince", ExpectedResult = 15)]
        [TestCase(8, 4, TestName = "Of Kind 4 Prince", ExpectedResult = 50)]
        [TestCase(8, 5, TestName = "Of Kind 5 Prince", ExpectedResult = 200)]
        [TestCase(9, 1, TestName = "Of Kind 1 Lady", ExpectedResult = 0)]
        [TestCase(9, 2, TestName = "Of Kind 2 Lady", ExpectedResult = 0)]
        [TestCase(9, 3, TestName = "Of Kind 3 Lady", ExpectedResult = 20)]
        [TestCase(9, 4, TestName = "Of Kind 4 Lady", ExpectedResult = 75)]
        [TestCase(9, 5, TestName = "Of Kind 5 Lady", ExpectedResult = 300)]
        [TestCase(10, 1, TestName = "Of Kind 1 Scatter", ExpectedResult = 0)]
        [TestCase(10, 2, TestName = "Of Kind 2 Scatter", ExpectedResult = 0)]
        [TestCase(10, 3, TestName = "Of Kind 3 Scatter", ExpectedResult = 1)]
        [TestCase(10, 4, TestName = "Of Kind 4 Scatter", ExpectedResult = 0)]
        [TestCase(10, 5, TestName = "Of Kind 5 Scatter", ExpectedResult = 0)]
        [TestCase(11, 1, TestName = "Of Kind 1 Wild", ExpectedResult = 0)]
        [TestCase(11, 2, TestName = "Of Kind 2 Wild", ExpectedResult = 0)]
        [TestCase(11, 3, TestName = "Of Kind 3 Wild", ExpectedResult = 30)]
        [TestCase(11, 4, TestName = "Of Kind 4 Wild", ExpectedResult = 150)]
        [TestCase(11, 5, TestName = "Of Kind 5 Wild", ExpectedResult = 1000)]
        public decimal TestPayout(int card, int ofkind)
        {
            return Payout.Odds(card, ofkind);
        }
    }
}
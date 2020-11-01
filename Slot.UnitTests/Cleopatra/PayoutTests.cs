namespace Slot.UnitTests.Cleopatra
{
    using NUnit.Framework;
    using Slot.Games.Cleopatra;
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
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 9,13,12, 12,9,13", TestName = "Ten Three", ExpectedResult = 5 * 4)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 0,13,12, 12,9,13", TestName = "Ten Four", ExpectedResult = 10 * 3 + 5)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 0,13,12, 0,13,10", TestName = "Ten Five", ExpectedResult = 20 + 10 + 10 + 5)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase("0,7,10, 0,10,10, 0,7,10, 11,12,13, 11,12,13", TestName = "Simple Payout", ExpectedResult = 20 + 10 + 10 + 5)]
        public decimal TestSimplePayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase(0, 1, TestName = "Of Kind 1 Ten", ExpectedResult = 0)]
        [TestCase(0, 2, TestName = "Of Kind 2 Ten", ExpectedResult = 0)]
        [TestCase(0, 3, TestName = "Of Kind 3 Ten", ExpectedResult = 5)]
        [TestCase(0, 4, TestName = "Of Kind 4 Ten", ExpectedResult = 10)]
        [TestCase(0, 5, TestName = "Of Kind 5 Ten", ExpectedResult = 20)]
        [TestCase(1, 1, TestName = "Of Kind 1 Jack", ExpectedResult = 0)]
        [TestCase(1, 2, TestName = "Of Kind 2 Jack", ExpectedResult = 0)]
        [TestCase(1, 3, TestName = "Of Kind 3 Jack", ExpectedResult = 5)]
        [TestCase(1, 4, TestName = "Of Kind 4 Jack", ExpectedResult = 10)]
        [TestCase(1, 5, TestName = "Of Kind 5 Jack", ExpectedResult = 20)]
        [TestCase(2, 1, TestName = "Of Kind 1 Queen", ExpectedResult = 0)]
        [TestCase(2, 2, TestName = "Of Kind 2 Queen", ExpectedResult = 0)]
        [TestCase(2, 3, TestName = "Of Kind 3 Queen", ExpectedResult = 5)]
        [TestCase(2, 4, TestName = "Of Kind 4 Queen", ExpectedResult = 15)]
        [TestCase(2, 5, TestName = "Of Kind 5 Queen", ExpectedResult = 30)]
        [TestCase(3, 1, TestName = "Of Kind 1 King", ExpectedResult = 0)]
        [TestCase(3, 2, TestName = "Of Kind 2 King", ExpectedResult = 0)]
        [TestCase(3, 3, TestName = "Of Kind 3 King", ExpectedResult = 5)]
        [TestCase(3, 4, TestName = "Of Kind 4 King", ExpectedResult = 15)]
        [TestCase(3, 5, TestName = "Of Kind 5 King", ExpectedResult = 30)]
        [TestCase(4, 1, TestName = "Of Kind 1 Ace", ExpectedResult = 0)]
        [TestCase(4, 2, TestName = "Of Kind 2 Ace", ExpectedResult = 0)]
        [TestCase(4, 3, TestName = "Of Kind 3 Ace", ExpectedResult = 5)]
        [TestCase(4, 4, TestName = "Of Kind 4 Ace", ExpectedResult = 15)]
        [TestCase(4, 5, TestName = "Of Kind 5 Ace", ExpectedResult = 30)]
        [TestCase(5, 1, TestName = "Of Kind 1 Mirror", ExpectedResult = 0)]
        [TestCase(5, 2, TestName = "Of Kind 2 Mirror", ExpectedResult = 0)]
        [TestCase(5, 3, TestName = "Of Kind 3 Mirror", ExpectedResult = 10)]
        [TestCase(5, 4, TestName = "Of Kind 4 Mirror", ExpectedResult = 20)]
        [TestCase(5, 5, TestName = "Of Kind 5 Mirror", ExpectedResult = 50)]
        [TestCase(6, 1, TestName = "Of Kind 1 Ring", ExpectedResult = 0)]
        [TestCase(6, 2, TestName = "Of Kind 2 Ring", ExpectedResult = 0)]
        [TestCase(6, 3, TestName = "Of Kind 3 Ring", ExpectedResult = 10)]
        [TestCase(6, 4, TestName = "Of Kind 4 Ring", ExpectedResult = 20)]
        [TestCase(6, 5, TestName = "Of Kind 5 Ring", ExpectedResult = 50)]
        [TestCase(7, 1, TestName = "Of Kind 1 Ankh", ExpectedResult = 0)]
        [TestCase(7, 2, TestName = "Of Kind 2 Ankh", ExpectedResult = 0)]
        [TestCase(7, 3, TestName = "Of Kind 3 Ankh", ExpectedResult = 10)]
        [TestCase(7, 4, TestName = "Of Kind 4 Ankh", ExpectedResult = 25)]
        [TestCase(7, 5, TestName = "Of Kind 5 Ankh", ExpectedResult = 75)]
        [TestCase(8, 1, TestName = "Of Kind 1 Cleopatra", ExpectedResult = 0)]
        [TestCase(8, 2, TestName = "Of Kind 2 Cleopatra", ExpectedResult = 0)]
        [TestCase(8, 3, TestName = "Of Kind 3 Cleopatra", ExpectedResult = 20)]
        [TestCase(8, 4, TestName = "Of Kind 4 Cleopatra", ExpectedResult = 35)]
        [TestCase(8, 5, TestName = "Of Kind 5 Cleopatra", ExpectedResult = 100)]
        public decimal TestPayout(int card, int ofkind)
        {
            return Payout.GetOdds(card, ofkind);
        }
    }
}
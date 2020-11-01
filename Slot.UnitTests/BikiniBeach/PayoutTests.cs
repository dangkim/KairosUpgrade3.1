namespace Slot.UnitTests.BikiniBeach
{
    using NUnit.Framework;
    using Slot.Games.BikiniBeach;
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

        [TestCase("0,1,9, 12,10,10, 0,13,13, 13,9,12, 12,13,9", TestName = "Coconut One", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12, 12,13,13, 13,13,9, 12,9,13", TestName = "Coconut Two", ExpectedResult = 0)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 9,13,12, 12,9,13", TestName = "Coconut Three", ExpectedResult = 10)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 0,13,12, 12,9,13", TestName = "Coconut Four", ExpectedResult = 20)]
        [TestCase("0,-1,-1, 0,12,12, 0,12,13, 0,13,12, 0,13,10", TestName = "Coconut Five", ExpectedResult = 50)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.Calculate(wheel, -1, 1);
            return result.win;
        }

        [TestCase("0,7,10, 0,10,10, 0,7,10, 11,12,13, 11,12,13", TestName = "Simple Payout", ExpectedResult = 400 + 10)]
        public decimal TestSimplePayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.Calculate(wheel, 10, 1);
            return result.win;
        }

        [TestCase(1, TestName = "Of Kind 1 Coconut", ExpectedResult = 0)]
        [TestCase(2, TestName = "Of Kind 2 Coconut", ExpectedResult = 0)]
        [TestCase(3, TestName = "Of Kind 3 Coconut", ExpectedResult = 10)]
        [TestCase(4, TestName = "Of Kind 4 Coconut", ExpectedResult = 20)]
        [TestCase(5, TestName = "Of Kind 5 Coconut", ExpectedResult = 50)]
        public decimal TestPayout(int ofkind)
        {
            return Payout.GetOdds(0, ofkind);
        }
    }
}
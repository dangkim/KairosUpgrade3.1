namespace Slot.UnitTests.CaptainRabbit
{
    using Microsoft.FSharp.Collections;
    using NUnit.Framework;
    using Slot.Games.CaptainRabbit;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class PayoutTest
    {
        private static List<int[]> Encoding(int[] array)
        {
            var wheel = new List<int[]>();
            for (var i = 0; i < 5; ++i)
            {
                var strip = new int[3];
                for (var j = 0; j < 3; ++j)
                    strip[j] = array[i * 3 + j];
                wheel.Add(strip);
            }

            return wheel;
        }

        [TestCase("0,1,10, 0,13,12, 0,13,9, 13,13,12, 12,13,13", "", TestName = "Less Scatter", ExpectedResult = 0)]
        [TestCase("0,1,10, 0,13,12, 0,13,10, 13,13,12, 12,13,13", "3,0,3,0,0", TestName = "Two Scatter", ExpectedResult = 25)]
        [TestCase("0,1,10, 0,13,12, 0,13,13, 13,10,12, 12,13,10", "3,0,0,2,3", TestName = "Three Scatter", ExpectedResult = 5 * 25)]
        [TestCase("0,1,10, 0,13,10, 0,13,13, 13,13,10, 12,10,13", "3,3,0,3,2", TestName = "Four Scatter", ExpectedResult = 20 * 25)]
        [TestCase("0,1,10, 0,10,12, 0,10,13, 10,13,12, 12,10,13", "3,2,2,1,2", TestName = "Five Scatter", ExpectedResult = 100 * 25)]
        public decimal TestScatterPayout(string wheelString, string position)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.payScatter(1, 1, ArrayModule.OfSeq(wheel));
            Assert.AreEqual(result.WinPositions.Any() ? string.Join(",", result.WinPositions.First().RowPositions) : "", position);
            return result.Payable;
        }

        [TestCase("0,1,9, 12,13,12, 0,13,13, 13,9,12, 12,13,9", TestName = "Nine One", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12, 12,13,13, 13,13,9, 12,9,13", TestName = "Nine Two", ExpectedResult = 0)]
        [TestCase("0,1,12, 0,12,12, 0,12,13, 9,13,12, 12,9,13", TestName = "Nine Three", ExpectedResult = 5)]
        [TestCase("0,1,9, 0,12,12, 12,0,13, 12,12,0, 12,9,13", TestName = "Nine Four", ExpectedResult = 20)]
        [TestCase("0,1,9, 0,13,12, 10,0,9, 13,13,0, 12,13,0", TestName = "Nine Five", ExpectedResult = 100)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Payout.payNoneScatter(1, 1, ArrayModule.OfSeq(wheel));
            return result.Payable;
        }

        [TestCase("0,12,12, 11,10,12, 12,0,13, 10,9,10, 12,13,9", TestName = "Payout Nine - Scatter", ExpectedResult = 1 * 25 + 5)]
        public decimal TestSimplePayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            var result = Payout.calculate(1, 1, ArrayModule.OfSeq(wheel));
            return result.Payable;
        }

        [TestCase(1, TestName = "Of Kind 1 Nine", ExpectedResult = 0)]
        [TestCase(2, TestName = "Of Kind 2 Nine", ExpectedResult = 0)]
        [TestCase(3, TestName = "Of Kind 3 Nine", ExpectedResult = 5)]
        [TestCase(4, TestName = "Of Kind 4 Nine", ExpectedResult = 20)]
        [TestCase(5, TestName = "Of Kind 5 Nine", ExpectedResult = 100)]
        public decimal TestPayout(int ofkind)
        {
            return PayTables.getOdds(0, ofkind);
        }
    }
}
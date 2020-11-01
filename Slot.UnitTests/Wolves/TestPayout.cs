namespace Slot.UnitTests.Wolves
{
    using Microsoft.FSharp.Collections;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class TestPayout
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

        [TestCase("0,1,9, 0,13,12, 0,13,9, 13,13,12, 12,13,13", "", TestName = "Less Scatter", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,12, 0,13,13, 13,9,12, 12,13,9", "3,0,0,2,3", TestName = "Three Scatter", ExpectedResult = 125)]
        [TestCase("0,1,9, 0,13,9, 0,13,13, 13,13,9, 12,9,13", "3,3,0,3,2", TestName = "Four Scatter", ExpectedResult = 1250)]
        [TestCase("0,1,9, 0,9,12, 0,9,13, 9,13,12, 12,9,13", "3,2,2,1,2", TestName = "Five Scatter", ExpectedResult = 6250)]
        public decimal TestScatterPayout(string wheelString, string position)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Slot.Games.Wolves.Payout.payScatter(1, 1, ListModule.OfSeq(wheel));
            Assert.AreEqual(result.WinPositions.Any() ? string.Join(",", result.WinPositions.First().RowPositions) : "", position);
            return result.Payable;
        }

        [TestCase("0,1,9, 11,13,12, 0,13,13, 13,9,12, 12,13,9", "|", TestName = "Jack One", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,13,9, 11,13,13, 13,13,9, 12,9,13", "|", TestName = "Jack Two", ExpectedResult = 0)]
        [TestCase("0,1,9, 0,9,12, 0,9,13, 9,13,12, 12,9,13", "1,1,1,0,0", TestName = "Jack Three", ExpectedResult = 2)]
        [TestCase("0,1,9, 0,9,0, 0,9,13, 9,13,12, 12,9,13", "1,1,1,0,0|1,3,1,0,0", TestName = "Jack Three with 2 Win Line", ExpectedResult = 4)]
        [TestCase("0,1,9, 0,9,0, 0,9,10, 9,13,12, 12,9,13", "1,1,1,0,0|1,1,3,0,0|1,3,1,0,0|1,3,3,0,0", TestName = "Jack Three with 4 Win Line", ExpectedResult = 8)]
        [TestCase("0,1,9, 0,9,12, 0,9,13, 10,0,12, 12,9,13", "1,1,1,1,0|1,1,1,2,0", TestName = "Jack Four with 2 Win Line", ExpectedResult = 20)]
        [TestCase("0,1,9, 0,9,10, 0,9,13, 10,0,12, 12,9,13", "1,1,1,1,0|1,1,1,2,0|1,3,1,1,0|1,3,1,2,0", TestName = "Jack Four with 4 Win Line", ExpectedResult = 40)]
        [TestCase("0,1,9, 0,9,12, 0,9,13, 9,0,12, 12,9,13", "1,1,1,2,0", TestName = "Jack Four", ExpectedResult = 10)]
        [TestCase("0,1,9, 0,13,12, 0,13,9, 13,13,0, 12,13,0", "1,1,1,3,3", TestName = "Jack Five", ExpectedResult = 40)]
        [TestCase("0,1,9, 0,13,12, 0,13,9, 13,13,0, 10,13,0", "1,1,1,3,1|1,1,1,3,3", TestName = "Jack Five 2 Win Line", ExpectedResult = 80)]
        [TestCase("11,1,9, 11,13,12, 0,13,13, 13,9,12, 12,13,9", "|", TestName = "Queen One", ExpectedResult = 0)]
        [TestCase("11,1,9, 1,13,9, 11,13,13, 13,13,9, 12,9,13", "|", TestName = "Queen Two", ExpectedResult = 0)]
        [TestCase("11,1,9, 1,9,12, 1,9,13, 9,13,12, 12,9,13", "2,1,1,0,0", TestName = "Queen Three", ExpectedResult = 2)]
        [TestCase("11,1,9, 1,9,12, 1,9,13, 9,1,12, 12,9,13", "2,1,1,2,0", TestName = "Queen Four", ExpectedResult = 10)]
        [TestCase("11,1,9, 11,1,12, 11,9,10, 1,13,12, 12,1,13", "2,2,3,1,2", TestName = "Queen Five", ExpectedResult = 40)]
        [TestCase("11,2,9, 11,13,12, 0,13,13, 13,9,12, 12,13,9", "|", TestName = "King One", ExpectedResult = 0)]
        [TestCase("11,2,9, 2,13,9, 11,13,13, 13,13,9, 12,9,13", "|", TestName = "King Two", ExpectedResult = 0)]
        [TestCase("11,2,9, 2,9,12, 2,9,13, 9,13,12, 12,9,13", "2,1,1,0,0", TestName = "King Three", ExpectedResult = 8)]
        [TestCase("11,2,9, 2,9,12, 2,9,13, 9,2,12, 12,9,13", "2,1,1,2,0", TestName = "King Four", ExpectedResult = 20)]
        [TestCase("11,2,9, 11,2,12, 11,9,10, 2,13,12, 12,2,13", "2,2,3,1,2", TestName = "King Five", ExpectedResult = 50)]
        [TestCase("11,3,9, 11,13,12, 0,13,13, 13,9,12, 12,13,9", "|", TestName = "Ace One", ExpectedResult = 0)]
        [TestCase("11,3,9, 3,13,9, 11,13,13, 13,13,9, 12,9,13", "|", TestName = "Ace Two", ExpectedResult = 0)]
        [TestCase("11,3,9, 3,9,12, 3,9,13, 9,13,12, 12,9,13", "2,1,1,0,0", TestName = "Ace Three", ExpectedResult = 10)]
        [TestCase("11,3,9, 3,9,12, 3,9,13, 3,2,12, 12,9,13", "2,1,1,1,0", TestName = "Ace Four", ExpectedResult = 25)]
        [TestCase("11,3,9, 11,3,12, 11,9,10, 3,13,12, 12,3,13", "2,2,3,1,2", TestName = "Ace Five", ExpectedResult = 60)]
        [TestCase("11,4,9, 11,13,12, 0,13,13, 13,9,12, 12,13,9", "|", TestName = "Ace One", ExpectedResult = 0)]
        [TestCase("11,4,9, 4,13,9, 11,13,13, 13,13,9, 12,9,13", "|", TestName = "Ace Two", ExpectedResult = 0)]
        [TestCase("11,4,9, 4,9,12, 4,9,13, 9,13,12, 12,9,13", "2,1,1,0,0", TestName = "Ace Three", ExpectedResult = 15)]
        [TestCase("11,4,9, 4,9,12, 4,9,13, 4,2,12, 12,9,13", "2,1,1,1,0", TestName = "Ace Four", ExpectedResult = 30)]
        [TestCase("11,4,9, 11,4,12, 11,9,10, 4,13,12, 12,4,13", "2,2,3,1,2", TestName = "Ace Five", ExpectedResult = 70)]
        [TestCase("11,5,9, 11,13,12, 0,13,13, 13,9,12, 12,13,9", "|", TestName = "Messi One", ExpectedResult = 0)]
        [TestCase("11,5,9, 5,13,9, 11,13,13, 13,13,9, 12,9,13", "|", TestName = "Messi Two", ExpectedResult = 0)]
        [TestCase("11,5,9, 5,9,12, 5,9,13, 9,13,12, 12,9,13", "2,1,1,0,0", TestName = "Messi Three", ExpectedResult = 15)]
        [TestCase("11,5,9, 5,9,12, 5,9,13, 5,2,12, 12,9,13", "2,1,1,1,0", TestName = "Messi Four", ExpectedResult = 45)]
        [TestCase("11,5,9, 11,5,12, 11,9,10, 5,13,12, 12,5,13", "2,2,3,1,2", TestName = "Messi Five", ExpectedResult = 80)]
        [TestCase("11,6,9, 11,13,12, 0,13,13, 13,9,12, 12,13,9", "|", TestName = "Neymar One", ExpectedResult = 0)]
        [TestCase("11,6,9, 6,13,9, 11,13,13, 13,13,9, 12,9,13", "|", TestName = "Neymar Two", ExpectedResult = 0)]
        [TestCase("11,6,9, 6,9,12, 6,9,13, 9,13,12, 12,9,13", "2,1,1,0,0", TestName = "Neymar Three", ExpectedResult = 20)]
        [TestCase("11,6,9, 6,9,12, 6,9,13, 6,2,12, 12,9,13", "2,1,1,1,0", TestName = "Neymar Four", ExpectedResult = 50)]
        [TestCase("11,6,9, 11,6,12, 11,9,10, 6,13,12, 12,6,13", "2,2,3,1,2", TestName = "Neymar Five", ExpectedResult = 90)]
        public decimal TestNonScatterPayout(string wheelString, string positions)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var winpositions = positions.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).ToArray();
            var result = Slot.Games.Wolves.Payout.payNoneScatter(1, 1, ListModule.OfSeq(wheel));
            for (var index = 0; index < result.WinPositions.Length; index++)
            {
                Assert.AreEqual(string.Join(",", result.WinPositions[index].RowPositions), winpositions[index]);
            }
            return result.Payable;
        }

        [TestCase("0,1,9, 11,10,12, 0,13,13, 10,9,10, 12,13,9", "3,0,0,2,3|1,2,1,1,0|1,2,1,3,0", TestName = "Simple Payout Jack & Scatter", ExpectedResult = 145)]
        [TestCase("0,8,9, 11,10,12, 0,13,8, 10,9,10, 12,8,9",
            "3,0,0,2,3|1,2,1,1,0|1,2,1,3,0|2,2,3,1,2|2,2,3,3,2",
            TestName = "Simple Payout Jack, Trophy & Scatter",
            ExpectedResult = 345)]
        [TestCase("0,8,7, 10,9,7, 0,10,8, 10,9,10, 7,8,9",
            "0,2,0,2,3|1,1,1,1,0|1,1,1,3,0|1,1,2,1,0|1,1,2,3,0|2,1,2,1,2|2,1,2,3,2|2,1,3,1,2|2,1,3,3,2|3,1,2,1,1|3,1,2,3,1|3,3,2,1,1|3,3,2,3,1",
            TestName = "Simple Payout Jack, Ronaldo, Trophy & Scatter",
            ExpectedResult = 925)]
        public decimal TestSimplePayout(string wheelString, string positions)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var winpositions = positions.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var result = Slot.Games.Wolves.Payout.calculate(1, 1, ListModule.OfSeq(wheel));
            for (var index = 0; index < result.WinPositions.Length; index++)
            {
                Assert.AreEqual(string.Join(",", result.WinPositions[index].RowPositions), winpositions[index]);
            }
            return result.Payable;
        }
    }
}
namespace Slot.UnitTests.FuDaoLe
{
    using System.Linq;
    using System;
    using NUnit.Framework;
    using Slot.Games.FuDaoLe;
    using System.Collections.Generic;

    [TestFixture]
    internal class ParsheetTests
    {
        public static List<int[]> Encoding(int[] array)
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

        [TestCase("10, 0, 1", TestName = "Index of Envelope Nine", ExpectedResult = 1)]
        [TestCase("0, 11, 1", TestName = "Index of Envelope Ten", ExpectedResult = 2)]
        [TestCase("0, 0, 12", TestName = "Index of Envelope Jack", ExpectedResult = 3)]
        [TestCase("0, 1, 3", TestName = "Index of None Envelope Jackpot", ExpectedResult = 0)]
        public decimal TestGetEnvelopeIndex(string reelString)
        {
            var reelStrips = reelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            return ParSheet.GetEnvelopeIndex(reelStrips);
        }

        [TestCase(10, TestName = "Envelope Nine", ExpectedResult = true)]
        [TestCase(11, TestName = "Envelope Ten", ExpectedResult = true)]
        [TestCase(12, TestName = "Envelope Jack", ExpectedResult = true)]
        [TestCase(0, TestName = "Nine - None Envelope", ExpectedResult = false)]
        [TestCase(9, TestName = "Man - None Envelope", ExpectedResult = false)]
        public bool TestCardIsEnvelope(int card) => ParSheet.IsEnvelopeCard(card);

        [TestCase("10, 0, 1", 0, TestName = "Nine by Envelope Nine", ExpectedResult = 0)]
        [TestCase("0, 11, 1", 1, TestName = "Ten by Envelope Ten", ExpectedResult = 1)]
        [TestCase("0, 0, 12", 2, TestName = "Jack Envelope Jack", ExpectedResult = 2)]
        [TestCase("0, 1, 2", 0, TestName = "Nine", ExpectedResult = 0)]
        [TestCase("0, 1, 2", 1, TestName = "Ten", ExpectedResult = 1)]
        [TestCase("0, 1, 2", 2, TestName = "Jack", ExpectedResult = 2)]
        [TestCase("4, 0, 12", 0, TestName = "King", ExpectedResult = 4)]
        [TestCase("0, 5, 12", 1, TestName = "Ace", ExpectedResult = 5)]
        public int TestGetCardByIndexAndIncludeEnvelopeJackpot(string reelString, int index)
        {
            var reelStrips = reelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            return reelStrips.GetCard(index);
        }

        [TestCase("15, 15, 15", TestName = "Full Stack Wild", ExpectedResult = "15,15,15")]
        [TestCase("14, 5, 15", TestName = "Expanding Stack Wild Except Scatter", ExpectedResult = "14,15,15")]
        [TestCase("0, 5, 15", TestName = "Single Wild", ExpectedResult = "15,15,15")]
        [TestCase("15, 5, 15", TestName = "Two More Wilds", ExpectedResult = "15,15,15")]
        public string TestExpandingWild(string reelString)
        {
            var reelStrips = reelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            return string.Join(',', ParSheet.ExpandWild(reelStrips));
        }

        [TestCase(1, 0.14, TestName = "[LVL1] Index of Nine", ExpectedResult = 0)]
        [TestCase(1, 0.28, TestName = "[LVL1] Index of Ten", ExpectedResult = 1)]
        [TestCase(1, 0.41, TestName = "[LVL1] Index of Jack", ExpectedResult = 2)]
        [TestCase(1, 0.54, TestName = "[LVL1] Index of Queen", ExpectedResult = 3)]
        [TestCase(1, 0.67, TestName = "[LVL1] Index of King", ExpectedResult = 4)]
        [TestCase(1, 0.8, TestName = "[LVL1] Index of Ace", ExpectedResult = 5)]
        [TestCase(1, 0.85, TestName = "[LVL1] Index of Orange", ExpectedResult = 6)]
        [TestCase(1, 0.90, TestName = "[LVL1] Index of Lantern", ExpectedResult = 7)]
        [TestCase(1, 0.95, TestName = "[LVL1] Index of Ingot", ExpectedResult = 8)]
        [TestCase(1, 1.0, TestName = "[LVL1] Index of Man", ExpectedResult = 9)]

        public int TestGetMysteryIndex(int level, double ratio) => ParSheet.GetMainGameMysteryIndex(ratio);
    }
}
namespace Slot.UnitTests.FuDaoLe
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using NUnit.Framework;
    using Slot.Games.FuDaoLe;

    [TestFixture]
    internal class PayoutTests
    {
        [TestCase(0, 1, TestName = "1 Kind of Nine", ExpectedResult = 0)]
        [TestCase(0, 2, TestName = "2 Kind of Nine", ExpectedResult = 0)]
        [TestCase(0, 3, TestName = "3 Kind of Nine", ExpectedResult = 5)]
        [TestCase(0, 4, TestName = "4 Kind of Nine", ExpectedResult = 10)]
        [TestCase(0, 5, TestName = "5 Kind of Nine", ExpectedResult = 25)]
        [TestCase(1, 1, TestName = "1 Kind of Ten", ExpectedResult = 0)]
        [TestCase(1, 2, TestName = "2 Kind of Ten", ExpectedResult = 0)]
        [TestCase(1, 3, TestName = "3 Kind of Ten", ExpectedResult = 5)]
        [TestCase(1, 4, TestName = "4 Kind of Ten", ExpectedResult = 10)]
        [TestCase(1, 5, TestName = "5 Kind of Ten", ExpectedResult = 25)]
        [TestCase(2, 1, TestName = "1 Kind of Jack", ExpectedResult = 0)]
        [TestCase(2, 2, TestName = "2 Kind of Jack", ExpectedResult = 0)]
        [TestCase(2, 3, TestName = "3 Kind of Jack", ExpectedResult = 5)]
        [TestCase(2, 4, TestName = "4 Kind of Jack", ExpectedResult = 10)]
        [TestCase(2, 5, TestName = "5 Kind of Nine", ExpectedResult = 25)]
        [TestCase(3, 1, TestName = "1 Kind of Queen", ExpectedResult = 0)]
        [TestCase(3, 2, TestName = "2 Kind of Queen", ExpectedResult = 0)]
        [TestCase(3, 3, TestName = "3 Kind of Queen", ExpectedResult = 5)]
        [TestCase(3, 4, TestName = "4 Kind of Queen", ExpectedResult = 10)]
        [TestCase(3, 5, TestName = "5 Kind of Queen", ExpectedResult = 25)]
        [TestCase(4, 1, TestName = "1 Kind of King", ExpectedResult = 0)]
        [TestCase(4, 2, TestName = "2 Kind of King", ExpectedResult = 0)]
        [TestCase(4, 3, TestName = "3 Kind of King", ExpectedResult = 5)]
        [TestCase(4, 4, TestName = "4 Kind of King", ExpectedResult = 10)]
        [TestCase(4, 5, TestName = "5 Kind of King", ExpectedResult = 25)]
        [TestCase(5, 1, TestName = "1 Kind of Ace", ExpectedResult = 0)]
        [TestCase(5, 2, TestName = "2 Kind of Ace", ExpectedResult = 0)]
        [TestCase(5, 3, TestName = "3 Kind of Ace", ExpectedResult = 5)]
        [TestCase(5, 4, TestName = "4 Kind of Ace", ExpectedResult = 10)]
        [TestCase(5, 5, TestName = "5 Kind of Ace", ExpectedResult = 25)]
        [TestCase(6, 1, TestName = "1 Kind of Orange", ExpectedResult = 0)]
        [TestCase(6, 2, TestName = "2 Kind of Orange", ExpectedResult = 0)]
        [TestCase(6, 3, TestName = "3 Kind of Orange", ExpectedResult = 8)]
        [TestCase(6, 4, TestName = "4 Kind of Orange", ExpectedResult = 18)]
        [TestCase(6, 5, TestName = "5 Kind of Orange", ExpectedResult = 38)]
        [TestCase(7, 1, TestName = "1 Kind of Lantern", ExpectedResult = 0)]
        [TestCase(7, 2, TestName = "2 Kind of Lantern", ExpectedResult = 0)]
        [TestCase(7, 3, TestName = "3 Kind of Lantern", ExpectedResult = 8)]
        [TestCase(7, 4, TestName = "4 Kind of Lantern", ExpectedResult = 18)]
        [TestCase(7, 5, TestName = "5 Kind of Lantern", ExpectedResult = 38)]
        [TestCase(8, 1, TestName = "1 Kind of Ingot", ExpectedResult = 0)]
        [TestCase(8, 2, TestName = "2 Kind of Ingot", ExpectedResult = 0)]
        [TestCase(8, 3, TestName = "3 Kind of Ingot", ExpectedResult = 8)]
        [TestCase(8, 4, TestName = "4 Kind of Ingot", ExpectedResult = 18)]
        [TestCase(8, 5, TestName = "5 Kind of Ingot", ExpectedResult = 38)]
        [TestCase(9, 1, TestName = "1 Kind of Man", ExpectedResult = 0)]
        [TestCase(9, 2, TestName = "2 Kind of Man", ExpectedResult = 0)]
        [TestCase(9, 3, TestName = "3 Kind of Man", ExpectedResult = 8)]
        [TestCase(9, 4, TestName = "4 Kind of Man", ExpectedResult = 18)]
        [TestCase(9, 5, TestName = "5 Kind of Man", ExpectedResult = 38)]
        public int TestGetMysteryIndex(int card, int count) => Payout.GetOdds(card, count);

        [TestCase("1,2,3", 5, "15,16,17", TestName = "Non Combination", ExpectedResult = "")]
        [TestCase("1,5,3", 5, "15,16,17", TestName = "Single Combination", ExpectedResult = "2")]
        [TestCase("5,2,5", 5, "15,16,17", TestName = "Two Combination", ExpectedResult = "1,3")]
        [TestCase("5, 5, 5", 5, "15,16,17", TestName = "Three Combination", ExpectedResult = "1,2,3")]
        public string TestGetCombinationForSpecificCard(string reelString, int card, string wildsString)
        {
            //arrange
            var col = 1;
            var reel = reelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            var wilds = wildsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            // action
            var result = Payout.GetCombination(reel, card, col, wilds);
            var rowIndices = result.Select(item => item.Row);

            //assert
            return string.Join(',', rowIndices);
        }

        [TestCase("15,1,14", 5, "15,16,17", TestName = "Multiplier By One Included Scatter", ExpectedResult = 1)]
        [TestCase("1,2,15", 5, "15,16,17", TestName = "Multiplier By One Included Scatter", ExpectedResult = 1)]
        [TestCase("16,5,3", 5, "15,16,17", TestName = "Multiplier By Two", ExpectedResult = 2)]
        [TestCase("5,17,5", 5, "15,16,17", TestName = "Multiplier By Three", ExpectedResult = 3)]
        public int TestWildMultiplierAndCombinationForSpecificCard(string reelString, int card, string wildsString)
        {
            //arrange
            var col = 1;
            var reel = reelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            var wilds = wildsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            // action
            var result = Payout.GetCombination(reel, card, col, wilds);
            var multiplier = result.Select(item => item.Multiplier).Aggregate((m1, m2) => m1 * m2);
            var rowIndices = result.Select(item => item.Row);

            //assert
            Assert.AreEqual(true, rowIndices.Any());
            return multiplier;
        }

        [TestCase("15,1,2, 14,1,0, 1,1,1, 7,8,9, 4,2,1", TestName = "One Scatter", ExpectedResult = 0)]
        [TestCase("15,1,3, 14,1,0, 1,1,14, 7,8,9, 4,2,1", TestName = "Two Scatter", ExpectedResult = 0)]
        [TestCase("15,1,14, 14,1,0, 1,1,1, 7,8,9, 14,2,1", TestName = "Three Scatter", ExpectedResult = 38 * 1 * 2)]
        [TestCase("15,1,14, 14,1,0, 1,1,1, 14,8,9, 14,2,1", TestName = "Four Scatter", ExpectedResult = 0)]
        [TestCase("15,1,14, 14,1,0, 1,1,14, 7,14,9, 14,2,1", TestName = "Five Scatter", ExpectedResult = 0)]
        public decimal TesScatterPayout(string reelStripsString)
        {
            //arrange
            var lineBet = 1.0m;
            var reelStrips = ParsheetTests.Encoding(reelStripsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            // action
            var result = Payout.GetScatterPayout(reelStrips, lineBet);

            //assert
            return result.Payable;
        }

        [TestCase("10,1,2, 14,1,0, 1,1,1, 7,8,9, 4,2,11", TestName = "Envelope Nine & Envelope Ten", ExpectedResult = 20 * 38 * 1)]
        [TestCase("15,11,3, 14,1,0, 1,1,14, 7,8,9, 4,12,1", TestName = "Envelope Ten  & Envelope Jack", ExpectedResult = 20 * 38 * 1)]
        [TestCase("15,12,14, 14,1,0, 1,1,1, 7,8,9, 14,2,10", TestName = "Envelope Jack & Envelope Nine", ExpectedResult = 20 * 38 * 1)]
        [TestCase("15,10,14, 14,1,0, 1,1,1, 14,8,9, 14,2,10", TestName = "Envelope Nine & Envelope Nine", ExpectedResult = 20 * 38 * 1)]
        [TestCase("15,11,14, 14,1,0, 1,1,1, 14,8,9, 14,2,11", TestName = "Envelope Ten & Envelope Ten", ExpectedResult = 20 * 38 * 1)]
        [TestCase("15,12,14, 14,1,0, 1,1,1, 14,8,9, 14,12,1", TestName = "Envelope Jack & Envelope Jack", ExpectedResult = 20 * 38 * 1)]
        [TestCase("15,10,2, 14,1,0, 1,1,1, 7,8,9, 4,2,0", TestName = "Envelope Nine & Nine", ExpectedResult = 0)]
        [TestCase("15,11,3, 14,1,0, 1,1,14, 7,8,9, 4,2,1", TestName = "Envelope Ten  & Ten", ExpectedResult = 0)]
        [TestCase("15,12,14, 14,1,0, 1,1,1, 7,8,9, 14,2,2", TestName = "Envelope Jack & Jack", ExpectedResult = 0)]
        public decimal TesRedEnvelopeJackpotPayout(string reelStripsString)
        {
            //arrange
            var lineBet = 1.0m;
            var reelStrips = ParsheetTests.Encoding(reelStripsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            // action
            var result = Payout.GetRedEnvelopeJackpotPayout(reelStrips, lineBet);

            //assert
            return result.Payable;
        }

        [TestCase("0,1,2, 3,4,5, 5,0,6, 7,8,9, 4,2,11", TestName = "1 kind of Nine & None Scatter, Wild", ExpectedResult = 0 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,7,6, 7,8,9, 4,2,11", TestName = "2 kind of Nine & None Scatter, Wild", ExpectedResult = 0 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,0,6, 7,8,9, 4,2,11", TestName = "3 kind of Nine & None Scatter, Wild", ExpectedResult = 5 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,0,6, 0,8,9, 4,2,11", TestName = "4 kind of Nine & None Scatter, Wild", ExpectedResult = 10 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,0,6, 7,0,9, 4,0,11", TestName = "5 kind of Nine & None Scatter, Wild", ExpectedResult = 25 * 1)]
        [TestCase("0,1,2, 3,4,5, 5,0,6, 7,8,9, 4,2,11", TestName = "1 kind of Nine & None Scatter, Wild", ExpectedResult = 0 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,7,6, 7,8,9, 4,2,11", TestName = "2 kind of Nine & None Scatter, Wild", ExpectedResult = 0 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,0,6, 7,8,9, 4,2,11", TestName = "3 kind of Nine & None Scatter, Wild", ExpectedResult = 5 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,0,6, 0,8,9, 4,2,11", TestName = "4 kind of Nine & None Scatter, Wild", ExpectedResult = 10 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,0,6, 7,0,9, 4,0,11", TestName = "5 kind of Nine & None Scatter, Wild", ExpectedResult = 25 * 1)]
        [TestCase("0,1,2, 3,4,5, 5,0,6, 7,8,9, 4,2,14", TestName = "1 kind of Nine With Scatter and Non Wild", ExpectedResult = 0 * 1)]
        [TestCase("0,1,2, 3,4,14, 5,7,6, 7,8,9, 4,2,11", TestName = "2 kind of Nine With Scatter and Non Wild", ExpectedResult = 0 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,14,6, 7,8,9, 4,2,11", TestName = "3 kind of Nine With Scatter and Non Wild", ExpectedResult = 5 * 1)]
        [TestCase("0,1,2, 3,4,14, 5,0,6, 14,8,9, 4,2,11", TestName = "4 kind of Nine With Scatter and Non Wild", ExpectedResult = 10 * 1)]
        [TestCase("0,1,2, 3,4,0, 5,0,6, 7,0,9, 4,14,11", TestName = "5 kind of Nine With Scatter and Non Wild", ExpectedResult = 25 * 1)]
        [TestCase("0,1,2, 3,4,5, 5,15,6, 7,8,9, 4,2,14", TestName = "1 kind of Nine With Scatter and Wild", ExpectedResult = 0 * 1)]
        [TestCase("0,1,2, 3,4,14, 5,7,6, 7,8,16, 4,2,11", TestName = "2 kind of Nine With Scatter and Wild", ExpectedResult = 0 * 1)]
        [TestCase("0,18,18, 3,4,15, 5,14,6, 7,8,9, 4,2,11", TestName = "3 kind of Nine With Scatter and Wild", ExpectedResult = 5 * 1)]
        [TestCase("0,18,18, 3,4,14, 5,16,6, 14,8,9, 4,2,11", TestName = "4 kind of Nine With Scatter and Wild", ExpectedResult = 10 * 1 * 2)]
        [TestCase("0,18,18, 3,4,0, 5,0,6, 7,17,9, 4,14,11", TestName = "5 kind of Nine With Scatter and Wild", ExpectedResult = 25 * 1 * 3)]
        public decimal TesNonSpecialCardPayout(string reelStripsString)
        {
            //arrange
            var lineBet = 1.0m;
            var reelStrips = ParsheetTests.Encoding(reelStripsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            // action
            var result = Payout.GetNonSpecialCardPayout(reelStrips, lineBet, new int[] { 15, 16, 17 });

            //assert
            return result.Payable;
        }
    }
}
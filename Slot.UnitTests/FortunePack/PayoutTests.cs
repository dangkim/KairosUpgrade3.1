namespace Slot.UnitTests.FortunePack
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    internal class PayoutTest
    {
        private static List<List<int>> Encoding(int[] array)
        {
            var wheel = new List<List<int>>();
            for (var i = 0; i < 3; ++i)
            {
                var strip = new int[3];
                for (var j = 0; j < 3; ++j)
                {
                    strip[j] = array[i * 3 + j];
                }

                wheel.Add(strip.ToList());
            }

            return wheel;
        }

        [TestCase("12,1,10, 12,13,12, 8,13,9", TestName = "Floating Three", ExpectedResult = 3 * 2)]
        public decimal TestFloatingPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Slot.Games.FortunePack.Payout.Calculate(wheel, 1);

            return result.win;
        }

        [TestCase("0,1,9, 0,13,12, 0,13,13", TestName = "Scroll - Line Two", ExpectedResult = 10)]
        [TestCase("0,1,9, 11,0,12, 12,13,0", TestName = "Scroll - Line Seven", ExpectedResult = 10)]
        [TestCase("12,-1,-1, 0,0,0, 0,12,13", TestName = "Scroll - Line Five", ExpectedResult = 10)]
        public decimal TestNonFloatingPayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var result = Games.FortunePack.Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase("0,12,9, 0,11,8, 0,12,8", TestName = "Payout Scroll - Floating", ExpectedResult = 10 + 10 + 3 * 2)]
        public decimal TestSimplePayout(string wheelString)
        {
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            var result = Games.FortunePack.Payout.Calculate(wheel, 1);
            return result.win;
        }

        [TestCase(1, TestName = "Of Kind 1 Scroll", ExpectedResult = 0)]
        [TestCase(2, TestName = "Of Kind 2 Scroll", ExpectedResult = 0)]
        [TestCase(3, TestName = "Of Kind 3 Scroll", ExpectedResult = 10)]
        public decimal TestPayout(int ofkind)
        {
            return Games.FortunePack.Payout.GetOdds(0, ofkind);
        }

        [TestCase("3,0,8,0,3,2,2,3,3", TestName = "Test Win Line", ExpectedResult = "3,4,7,8")]
        public string TestWinLine(string wheelString)
        {
            // arrange
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            // action
            var payOuts = Games.FortunePack.Payout.Calculate(wheel, 1);

            // assert
            return string.Join(',', payOuts.positions.Select(ele => ele.Line));
        }
    }
}
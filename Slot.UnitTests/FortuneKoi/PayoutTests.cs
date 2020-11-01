namespace Slot.UnitTests.FortuneKoi
{
    using NUnit.Framework;
    using Slot.Games.FortuneKoi;
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

        [TestCase("3,0,8, 0,3,2, 2,4,3, 5,3,0, 3,5,6 ", TestName = "Test None Scatter Payout with No Wild Included", ExpectedResult = 40)]
        [TestCase("3,0,8, 3,0,2, 2,3,6, 1,5,6, 3,5,6 ", TestName = "Test None Scatter Both Way of Payout with No Wild Included", ExpectedResult = 50 + 8)]
        [TestCase("3,0,8, 3,0,2, 7,7,7, 1,5,6, 3,5,6 ", TestName = "Test None Scatter Both Way of Payout with Wild Included", ExpectedResult = 5 + 25 + 50 + 8 + 50 + 8)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            // arrange
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            // action
            var payOuts = Payout.Calculate(wheel, 1);

            // assert
            return payOuts.win;
        }

        [TestCase(0, 1, TestName = "Of Kind 1 G5", ExpectedResult = 0)]
        [TestCase(0, 2, TestName = "Of Kind 2 G5", ExpectedResult = 0)]
        [TestCase(0, 3, TestName = "Of Kind 3 G5", ExpectedResult = 5)]
        [TestCase(0, 4, TestName = "Of Kind 4 G5", ExpectedResult = 10)]
        [TestCase(0, 5, TestName = "Of Kind 5 G5", ExpectedResult = 15)]
        [TestCase(1, 1, TestName = "Of Kind 1 G4", ExpectedResult = 0)]
        [TestCase(1, 2, TestName = "Of Kind 2 G4", ExpectedResult = 0)]
        [TestCase(1, 3, TestName = "Of Kind 3 G4", ExpectedResult = 5)]
        [TestCase(1, 4, TestName = "Of Kind 4 G4", ExpectedResult = 10)]
        [TestCase(1, 5, TestName = "Of Kind 5 G4", ExpectedResult = 20)]
        [TestCase(2, 1, TestName = "Of Kind 1 G3", ExpectedResult = 0)]
        [TestCase(2, 2, TestName = "Of Kind 2 G3", ExpectedResult = 0)]
        [TestCase(2, 3, TestName = "Of Kind 3 G3", ExpectedResult = 7)]
        [TestCase(2, 4, TestName = "Of Kind 4 G3", ExpectedResult = 15)]
        [TestCase(2, 5, TestName = "Of Kind 5 G3", ExpectedResult = 25)]
        [TestCase(3, 1, TestName = "Of Kind 1 G2", ExpectedResult = 0)]
        [TestCase(3, 2, TestName = "Of Kind 2 G2", ExpectedResult = 0)]
        [TestCase(3, 3, TestName = "Of Kind 3 G2", ExpectedResult = 8)]
        [TestCase(3, 4, TestName = "Of Kind 4 G2", ExpectedResult = 20)]
        [TestCase(3, 5, TestName = "Of Kind 5 G2", ExpectedResult = 40)]
        [TestCase(4, 1, TestName = "Of Kind 1 G1", ExpectedResult = 0)]
        [TestCase(4, 2, TestName = "Of Kind 2 G1", ExpectedResult = 0)]
        [TestCase(4, 3, TestName = "Of Kind 3 G1", ExpectedResult = 10)]
        [TestCase(4, 4, TestName = "Of Kind 4 G1", ExpectedResult = 20)]
        [TestCase(4, 5, TestName = "Of Kind 5 G1", ExpectedResult = 50)]
        [TestCase(5, 1, TestName = "Of Kind 1 Seven", ExpectedResult = 0)]
        [TestCase(5, 2, TestName = "Of Kind 2 Seven", ExpectedResult = 0)]
        [TestCase(5, 3, TestName = "Of Kind 3 Seven", ExpectedResult = 25)]
        [TestCase(5, 4, TestName = "Of Kind 4 Seven", ExpectedResult = 50)]
        [TestCase(5, 5, TestName = "Of Kind 5 Seven", ExpectedResult = 100)]
        [TestCase(6, 1, TestName = "Of Kind 1 Bar", ExpectedResult = 0)]
        [TestCase(6, 2, TestName = "Of Kind 2 Bar", ExpectedResult = 0)]
        [TestCase(6, 3, TestName = "Of Kind 3 Bar", ExpectedResult = 50)]
        [TestCase(6, 4, TestName = "Of Kind 4 Bar", ExpectedResult = 200)]
        [TestCase(6, 5, TestName = "Of Kind 5 Bar", ExpectedResult = 250)]
        public decimal TestPayout(int card, int ofkind)
        {
            return Payout.Odds(card, ofkind);
        }
    }
}
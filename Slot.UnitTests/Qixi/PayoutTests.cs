namespace Slot.UnitTests.Qixi
{
    using NUnit.Framework;
    using Slot.Games.Qixi;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    internal class PayoutTest
    {
        private static List<List<int>> Encoding(int[] array)
        {
            var wheel = new List<List<int>>();
            for (var i = 0; i < 5; ++i)
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

        [TestCase(0, 1, TestName = "Of Kind 1 LoveJade", ExpectedResult = 0)]
        [TestCase(0, 2, TestName = "Of Kind 2 LoveJade", ExpectedResult = 0)]
        [TestCase(0, 3, TestName = "Of Kind 3 LoveJade", ExpectedResult = 5)]
        [TestCase(0, 4, TestName = "Of Kind 4 LoveJade", ExpectedResult = 15)]
        [TestCase(0, 5, TestName = "Of Kind 5 LoveJade", ExpectedResult = 100)]
        [TestCase(1, 1, TestName = "Of Kind 1 TaoHua", ExpectedResult = 0)]
        [TestCase(1, 2, TestName = "Of Kind 2 TaoHua", ExpectedResult = 0)]
        [TestCase(1, 3, TestName = "Of Kind 3 TaoHua", ExpectedResult = 5)]
        [TestCase(1, 4, TestName = "Of Kind 4 TaoHua", ExpectedResult = 15)]
        [TestCase(1, 5, TestName = "Of Kind 5 TaoHua", ExpectedResult = 100)]
        [TestCase(2, 1, TestName = "Of Kind 1 Lantern", ExpectedResult = 0)]
        [TestCase(2, 2, TestName = "Of Kind 2 Lantern", ExpectedResult = 0)]
        [TestCase(2, 3, TestName = "Of Kind 3 Lantern", ExpectedResult = 5)]
        [TestCase(2, 4, TestName = "Of Kind 4 Lantern", ExpectedResult = 20)]
        [TestCase(2, 5, TestName = "Of Kind 5 Lantern", ExpectedResult = 200)]
        [TestCase(3, 1, TestName = "Of Kind 1 Cow", ExpectedResult = 0)]
        [TestCase(3, 2, TestName = "Of Kind 2 Cow", ExpectedResult = 0)]
        [TestCase(3, 3, TestName = "Of Kind 3 Cow", ExpectedResult = 5)]
        [TestCase(3, 4, TestName = "Of Kind 4 Cow", ExpectedResult = 20)]
        [TestCase(3, 5, TestName = "Of Kind 5 Cow", ExpectedResult = 200)]
        [TestCase(4, 1, TestName = "Of Kind 1 Loom", ExpectedResult = 0)]
        [TestCase(4, 2, TestName = "Of Kind 2 Loom", ExpectedResult = 0)]
        [TestCase(4, 3, TestName = "Of Kind 3 Loom", ExpectedResult = 5)]
        [TestCase(4, 4, TestName = "Of Kind 4 Loom", ExpectedResult = 25)]
        [TestCase(4, 5, TestName = "Of Kind 5 Loom", ExpectedResult = 300)]
        [TestCase(5, 1, TestName = "Of Kind 1 Flute", ExpectedResult = 0)]
        [TestCase(5, 2, TestName = "Of Kind 2 Flute", ExpectedResult = 0)]
        [TestCase(5, 3, TestName = "Of Kind 3 Flute", ExpectedResult = 10)]
        [TestCase(5, 4, TestName = "Of Kind 4 Flute", ExpectedResult = 30)]
        [TestCase(5, 5, TestName = "Of Kind 5 Flute", ExpectedResult = 500)]
        [TestCase(6, 1, TestName = "Of Kind 1 Mother", ExpectedResult = 0)]
        [TestCase(6, 2, TestName = "Of Kind 2 Mother", ExpectedResult = 3)]
        [TestCase(6, 3, TestName = "Of Kind 3 Mother", ExpectedResult = 20)]
        [TestCase(6, 4, TestName = "Of Kind 4 Mother", ExpectedResult = 100)]
        [TestCase(6, 5, TestName = "Of Kind 5 Mother", ExpectedResult = 1000)]
        [TestCase(7, 1, TestName = "Of Kind 1 Child", ExpectedResult = 0)]
        [TestCase(7, 2, TestName = "Of Kind 2 Child", ExpectedResult = 3)]
        [TestCase(7, 3, TestName = "Of Kind 3 Child", ExpectedResult = 20)]
        [TestCase(7, 4, TestName = "Of Kind 4 Child", ExpectedResult = 100)]
        [TestCase(7, 5, TestName = "Of Kind 5 Child", ExpectedResult = 1000)]
        [TestCase(8, 1, TestName = "Of Kind 1 Female", ExpectedResult = 0)]
        [TestCase(8, 2, TestName = "Of Kind 2 Female", ExpectedResult = 5)]
        [TestCase(8, 3, TestName = "Of Kind 3 Female", ExpectedResult = 50)]
        [TestCase(8, 4, TestName = "Of Kind 4 Female", ExpectedResult = 250)]
        [TestCase(8, 5, TestName = "Of Kind 5 Female", ExpectedResult = 2500)]
        [TestCase(9, 1, TestName = "Of Kind 1 Male", ExpectedResult = 2)]
        [TestCase(9, 2, TestName = "Of Kind 2 Male", ExpectedResult = 10)]
        [TestCase(9, 3, TestName = "Of Kind 3 Male", ExpectedResult = 100)]
        [TestCase(9, 4, TestName = "Of Kind 4 Male", ExpectedResult = 500)]
        [TestCase(9, 5, TestName = "Of Kind 5 Male", ExpectedResult = 5000)]
        [TestCase(10, 1, TestName = "Of Kind 1 Bridge", ExpectedResult = 0)]
        [TestCase(10, 2, TestName = "Of Kind 2 Bridge", ExpectedResult = 1)]
        [TestCase(10, 3, TestName = "Of Kind 3 Bridge", ExpectedResult = 5)]
        [TestCase(10, 4, TestName = "Of Kind 4 Bridge", ExpectedResult = 10)]
        [TestCase(10, 5, TestName = "Of Kind 5 Bridge", ExpectedResult = 100)]
        public decimal TestPayout(int card, int ofkind)
        {
            return Payout.Odds(card, ofkind);
        }

        [TestCase("3,0,10,0,3,10,2,3,3, 1,2,3,4,5,6", TestName = "Test 2 Kind of  Scatter Payout", ExpectedResult = "3,3,0,0,0")]
        [TestCase("3,0,10, 0,3,10, 2,3,3, 1,2,10, 4,5,6", TestName = "Test 3 Kind of  Scatter Payout", ExpectedResult = "3,3,0,3,0")]
        [TestCase("3,0,10,0,3,10,2,3,3, 1,10,3,4,5,10", TestName = "Test 4 Kind of  Scatter Payout", ExpectedResult = "3,3,0,2,3")]
        [TestCase("3,0,10,0,3,10,2,3,10, 10,2,3,1,10,6", TestName = "Test 5 Kind of  Scatter Payout", ExpectedResult = "3,3,3,1,2")]
        public string TestScatterPayout(string wheelString)
        {
            // arrange
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            // action
            var payOuts = Payout.Calculate(wheel, 1);

            // assert
            return string.Join(',', payOuts.positions.Where(win => win.Line == 0).SelectMany(ele => ele.RowPositions));
        }

        [TestCase("3,0,8, 0,3,2, 2,4,3, 5,3,0, 3,5,6 ", TestName = "Test None Scatter Payout with No Wild Included", ExpectedResult = 200)]
        [TestCase("3,0,8, 0,3,2, 2,4,3, 5,6,0, 6,5,7 ", TestName = "Test None Scatter Both Way of Payout with No Wild Included", ExpectedResult = 5 + 3)]
        [TestCase("3,0,8, 0,3,2, 2,4,11, 5,6,0, 6,5,7 ", TestName = "Test None Scatter Both Way of Payout with Wild Included", ExpectedResult = 2 * 5 + 2 * 20)]
        public decimal TestNonScatterPayout(string wheelString)
        {
            // arrange
            var wheel = Encoding(wheelString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

            // action
            var payOuts = Payout.Calculate(wheel, 1);

            // assert
            return payOuts.win;
        }
    }
}
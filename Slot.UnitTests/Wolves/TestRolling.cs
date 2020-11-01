namespace Slot.UnitTests.Wolves
{
    using Microsoft.FSharp.Collections;
    using NUnit.Framework;
    using Slot.Games.Wolves;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class TestRolling
    {
        [TestCase(0, "1,0,0,0,0", TestName = "Simple Rolling on 1st reel with top rolling", ExpectedResult = "4,9,4|4")]
        [TestCase(0, "2,0,0,0,0", TestName = "Simple Rolling on 1st reel with middle rolling", ExpectedResult = "4,1,4|4")]
        [TestCase(0, "3,0,0,0,0", TestName = "Simple Rolling on 1st reel with bottom rolling", ExpectedResult = "4,1,9|4")]
        [TestCase(0, "1,0,0,0,0|2,0,0,0,0", TestName = "Simple Rolling on 1st reel with top & middle rolling", ExpectedResult = "0,4,4|0,4")]
        [TestCase(0, "1,0,0,0,0|3,0,0,0,0", TestName = "Simple Rolling on 1st reel with top & bottom rolling", ExpectedResult = "0,4,9|0,4")]
        [TestCase(0, "2,0,0,0,0|3,0,0,0,0", TestName = "Simple Rolling on 1st reel with middle & bottom rolling", ExpectedResult = "0,4,1|0,4")]
        [TestCase(0, "1,0,0,0,0|2,0,0,0,0|3,0,0,0,0", TestName = "Simple Rolling on 1st reel with top, middle & bottom rolling", ExpectedResult = "4,0,4|4,0,4")]

        [TestCase(1, "0,1,0,0,0", TestName = "Simple Rolling on 2nd reel with top rolling", ExpectedResult = "9,4,1|9")]
        [TestCase(1, "0,2,0,0,0", TestName = "Simple Rolling on 2nd reel with middle rolling", ExpectedResult = "9,3,1|9")]
        [TestCase(1, "0,3,0,0,0", TestName = "Simple Rolling on 2nd reel with bottom rolling", ExpectedResult = "9,3,4|9")]
        [TestCase(1, "0,1,0,0,0|0,2,0,0,0", TestName = "Simple Rolling on 2nd reel with top & middle rolling", ExpectedResult = "0,9,1|0,9")]
        [TestCase(1, "0,1,0,0,0|0,3,0,0,0", TestName = "Simple Rolling on 2nd reel with top & bottom rolling", ExpectedResult = "0,9,4|0,9")]
        [TestCase(1, "0,2,0,0,0|0,3,0,0,0", TestName = "Simple Rolling on 2nd reel with middle & bottom rolling", ExpectedResult = "0,9,3|0,9")]
        [TestCase(1, "0,1,0,0,0|0,2,0,0,0|0,3,0,0,0", TestName = "Simple Rolling on 2nd reel with top, middle & bottom rolling", ExpectedResult = "1,0,9|1,0,9")]

        [TestCase(2, "0,0,1,0,0", TestName = "Simple Rolling on 3rd reel with top rolling", ExpectedResult = "5,4,2|5")]
        [TestCase(2, "0,0,2,0,0", TestName = "Simple Rolling on 3rd reel with middle rolling", ExpectedResult = "5,1,2|5")]
        [TestCase(2, "0,0,3,0,0", TestName = "Simple Rolling on 3rd reel with bottom rolling", ExpectedResult = "5,1,4|5")]
        [TestCase(2, "0,0,1,0,0|0,0,2,0,0", TestName = "Simple Rolling on 3rd reel with top & middle rolling", ExpectedResult = "3,5,2|3,5")]
        [TestCase(2, "0,0,1,0,0|0,0,3,0,0", TestName = "Simple Rolling on 3rd reel with top & bottom rolling", ExpectedResult = "3,5,4|3,5")]
        [TestCase(2, "0,0,2,0,0|0,0,3,0,0", TestName = "Simple Rolling on 3rd reel with middle & bottom rolling", ExpectedResult = "3,5,1|3,5")]
        [TestCase(2, "0,0,1,0,0|0,0,2,0,0|0,0,3,0,0", TestName = "Simple Rolling on 3rd reel with top, middle & bottom rolling", ExpectedResult = "9,3,5|9,3,5")]

        [TestCase(3, "0,0,0,1,0", TestName = "Simple Rolling on fourth reel with top rolling", ExpectedResult = "5,1,7|5")]
        [TestCase(3, "0,0,0,2,0", TestName = "Simple Rolling on fourth reel with middle rolling", ExpectedResult = "5,2,7|5")]
        [TestCase(3, "0,0,0,3,0", TestName = "Simple Rolling on fourth reel with bottom rolling", ExpectedResult = "5,2,1|5")]
        [TestCase(3, "0,0,0,1,0|0,0,0,2,0", TestName = "Simple Rolling on fourth reel with top & middle rolling", ExpectedResult = "1,5,7|1,5")]
        [TestCase(3, "0,0,0,1,0|0,0,0,3,0", TestName = "Simple Rolling on fourth reel with top & bottom rolling", ExpectedResult = "1,5,1|1,5")]
        [TestCase(3, "0,0,0,2,0|0,0,0,3,0", TestName = "Simple Rolling on fourth reel with middle & bottom rolling", ExpectedResult = "1,5,2|1,5")]
        [TestCase(3, "0,0,0,1,0|0,0,0,2,0|0,3,0,3,0", TestName = "Simple Rolling on fourth reel with top, middle & bottom rolling", ExpectedResult = "4,1,5|4,1,5")]

        [TestCase(4, "0,0,0,0,1", TestName = "Simple Rolling on fifth reel with top rolling", ExpectedResult = "2,3,6|2")]
        [TestCase(4, "0,0,0,0,2", TestName = "Simple Rolling on fifth reel with middle rolling", ExpectedResult = "2,4,6|2")]
        [TestCase(4, "0,0,0,0,3", TestName = "Simple Rolling on fifth reel with bottom rolling", ExpectedResult = "2,4,3|2")]
        [TestCase(4, "0,0,0,0,1|0,0,0,0,2", TestName = "Simple Rolling on fifth reel with top & middle rolling", ExpectedResult = "1,2,6|1,2")]
        [TestCase(4, "0,1,0,0,1|0,0,0,0,3", TestName = "Simple Rolling on fifth reel with top & bottom rolling", ExpectedResult = "1,2,3|1,2")]
        [TestCase(4, "0,0,0,0,2|0,0,0,0,3", TestName = "Simple Rolling on fifth reel with middle & bottom rolling", ExpectedResult = "1,2,4|1,2")]
        [TestCase(4, "0,0,0,0,1|0,0,0,0,2|0,0,0,0,3", TestName = "Simple Rolling on fifth reel with top, middle & bottom rolling", ExpectedResult = "5,1,2|5,1,2")]

        public string TestSimpleRolling(int reel, string triggerOns)
        {
            var stripId = 6;
            var strips = MainGame.strips(stripId).ToList();
            var reel1 = strips[0];
            var reel2 = strips[1];
            var reel3 = strips[2];
            var reel4 = strips[3];
            var reel5 = strips[4];
            var reels = new List<int[]>();
            reels.Add(Global.takeRolling(1, 3, reel1));
            reels.Add(Global.takeRolling(3, 3, reel2));
            reels.Add(Global.takeRolling(5, 3, reel3));
            reels.Add(Global.takeRolling(7, 3, reel4));
            reels.Add(Global.takeRolling(9, 3, reel5));
            var reelRollings = new [] {
                 new Domain.Rolling(1,3),
                 new Domain.Rolling(3,3),
                 new Domain.Rolling(5,3),
                 new Domain.Rolling(7,3),
                 new Domain.Rolling(9,3)
            };
            var wheel = new Games.Wolves.Domain.WolvesWheel(1, ListModule.OfSeq(reels), ListModule.OfSeq(new List<int[]>()), ArrayModule.OfSeq(reelRollings));
            var regions = new List<Domain.Region>();
            foreach(var rolling in triggerOns.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                regions.Add(new Domain.Region(reel, rolling.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()));            
            };

            // Action
            var wheelRolling = Rolling.breakWinning(regions.ToArray(), wheel);
            var wheelFilled = Games.Wolves.ParSheet.fillUpForMainGame(stripId, wheelRolling);

            // Assert
            return string.Concat(string.Join(',',wheelFilled.Reels[reel]), "|",string.Join(',', wheelFilled.Collapsing[reel]));            
        }
    }
}
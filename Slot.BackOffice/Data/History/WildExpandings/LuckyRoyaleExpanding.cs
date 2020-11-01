using System.Linq;
using Slot.Model;

namespace Slot.BackOffice.Data.History.WildExpandings
{
    public class LuckyRoyaleExpanding : IWildExpanding
    {
        public void Expanding(WheelViewModel wheel)
        {
            WildSquareExpanding(wheel);
            WildDiagonalExpanding(wheel);
            WildVerticalExpanding(wheel);
            WildHorizontalExpanding(wheel);
        }

        #region Vertical

        private static void WildVerticalExpanding(WheelViewModel wheel)
        {
            for (var reel = 0; reel < 5; reel++)
                if (CheckWild(reel, (int) LuckyRoyaleSymbol.WildVertical, wheel))
                    for (var index = 0; index < 3; index++)
                    {
                        var sym = wheel.reels[reel][index].symbol;
                        if ((sym < 10) || (sym == (int) LuckyRoyaleSymbol.WildVertical))
                            wheel.reels[reel][index] = new Symbols
                            {
                                symbol = (int) LuckyRoyaleSymbol.WildBase
                            };
                    }
        }

        #endregion Vertical

        #region Horizontal

        private static void WildHorizontalExpanding(WheelViewModel wheel)
        {
            var wild = (int) LuckyRoyaleSymbol.WildHorizontal;
            for (var reel = 1; reel < 4; reel++)
                if (CheckWild(reel, wild, wheel))
                {
                    var index = 0;
                    for (; index < 3; index++)
                        if (wheel.reels[reel][index].symbol == wild)
                            break;
                    wheel.reels[reel][index] = new Symbols
                    {
                        symbol = (int) LuckyRoyaleSymbol.WildBase
                    };
                    if (wheel.reels[reel - 1][index].symbol < 10)
                        wheel.reels[reel - 1][index] = new Symbols
                        {
                            symbol = (int) LuckyRoyaleSymbol.WildBase
                        };

                    if (wheel.reels[reel + 1][index].symbol < 10)
                        wheel.reels[reel + 1][index] = new Symbols
                        {
                            symbol = (int) LuckyRoyaleSymbol.WildBase
                        };
                }
        }

        #endregion Horizontal

        #region Square

        private static void WildSquareExpanding(WheelViewModel wheel)
        {
            if (CheckSquareTop(wheel))
                WildSqureExpanding(1, wheel);
            else if (CheckSquareBottom(wheel))
                WildSqureExpanding(3, wheel);
            else if (CheckSquareMidle(wheel))
                WildSqureExpanding(2, wheel);
        }

        private static bool CheckSquareTop(WheelViewModel wheel)
        {
            return CheckWild(2, 0, (int) LuckyRoyaleSymbol.WildSquare, wheel);
        }

        private static bool CheckSquareMidle(WheelViewModel wheel)
        {
            return CheckWild(2, 1, (int) LuckyRoyaleSymbol.WildSquare, wheel);
        }

        private static bool CheckSquareBottom(WheelViewModel wheel)
        {
            return CheckWild(2, 2, (int) LuckyRoyaleSymbol.WildSquare, wheel);
        }

        private static void WildSqureExpanding(int position, WheelViewModel wheel)
        {
            var row = 0;
            var deep = 0;
            if (position == 1) //top
            {
                deep = 2;
            }
            else if (position == 2) // middle
            {
                deep = 3;
            }
            else
            {
                row = 1;
                deep = 3;
            }

            for (var col = 1; col < 4; col++)
                for (var index = row; index < deep; index++)
                {
                    var sym = wheel.reels[col][index].symbol;
                    if ((sym < 10) || (sym == (int) LuckyRoyaleSymbol.WildSquare))
                        wheel.reels[col][index] = new Symbols
                        {
                            symbol = (int) LuckyRoyaleSymbol.WildBase
                        };
                }
        }

        #endregion Square

        #region Diagonal

        private static void WildDiagonalExpanding(WheelViewModel wheel)
        {
            if (CheckDiagonalLeft(wheel))
                WildDiagonalExpanding(1, wheel);

            if (CheckDiagonalRight(wheel))
                WildDiagonalExpanding(3, wheel);
            if (CheckDiagonalMidle(wheel))
                WildDiagonalExpanding(2, wheel);
        }

        private static void WildDiagonalExpanding(int reel, WheelViewModel wheel)
        {
            if (wheel.reels[reel - 1][0].symbol < 10)
                wheel.reels[reel - 1][0] = new Symbols
                {
                    symbol = (int) LuckyRoyaleSymbol.WildBase
                };

            if (wheel.reels[reel + 1][0].symbol < 10)
                wheel.reels[reel + 1][0] = new Symbols
                {
                    symbol = (int) LuckyRoyaleSymbol.WildBase
                };

            wheel.reels[reel][1] = new Symbols
            {
                symbol = (int) LuckyRoyaleSymbol.WildBase
            };

            if (wheel.reels[reel - 1][2].symbol < 10)
                wheel.reels[reel - 1][2] = new Symbols
                {
                    symbol = (int) LuckyRoyaleSymbol.WildBase
                };

            if (wheel.reels[reel + 1][2].symbol < 10)
                wheel.reels[reel + 1][2] = new Symbols
                {
                    symbol = (int) LuckyRoyaleSymbol.WildBase
                };
        }

        private static bool CheckDiagonalLeft(WheelViewModel wheel)
        {
            return CheckWild(1, 1, (int) LuckyRoyaleSymbol.Wild, wheel);
        }

        private static bool CheckDiagonalRight(WheelViewModel wheel)
        {
            return CheckWild(3, 1, (int) LuckyRoyaleSymbol.Wild, wheel);
        }

        private static bool CheckDiagonalMidle(WheelViewModel wheel)
        {
            return CheckWild(2, 1, (int) LuckyRoyaleSymbol.Wild, wheel);
        }

        #endregion Diagonal

        #region Check Wild

        private static bool CheckWild(int col, int row, int wild, WheelViewModel wheel)
        {
            return wheel.reels[col][row].symbol == wild;
        }

        private static bool CheckWild(int reel, int wild, WheelViewModel wheel)
        {
            return wheel.reels[reel].Any(item => item.symbol == wild);
        }

        #endregion Check Wild
    }
}
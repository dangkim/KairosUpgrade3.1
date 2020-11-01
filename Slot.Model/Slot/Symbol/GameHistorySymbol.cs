using System;

namespace Slot.Model.Slot
{
    public class GameHistorySymbol
    {

        public int Symbol { get; set; }

        public int RowPosition { get; set; }

        public int Height { get; set; }

        public int ColPosition { get; set; }

        public int Width { get; set; }

        public GameHistorySymbol()
        {
            this.Symbol = -1;
            this.RowPosition = -1;
            this.Height = 1;
            this.ColPosition = -1;
            this.Width = 1;
        }

        public GameHistorySymbol(int symbol)
        {
            this.Symbol = symbol;
            this.RowPosition = -1;
            this.Height = 1;
            this.ColPosition = -1;
            this.Width = 1;
        }

        public GameHistorySymbol(int symbol, int height)
        {
            this.Symbol = symbol;
            this.RowPosition = -1;
            this.Height = height;
            this.ColPosition = -1;
            this.Width = 1;
        }

        public GameHistorySymbol(int symbol, int height, int rowPosition)
        {
            this.Symbol = symbol;
            this.RowPosition = rowPosition;
            this.Height = height;
            this.ColPosition = -1;
            this.Width = 1;
        }

        public GameHistorySymbol(int symbol, int height, int rowPosition, int width, int colPosition)
        {
            this.Symbol = symbol;
            this.RowPosition = rowPosition;
            this.Height = height;
            this.ColPosition = colPosition;
            this.Width = width;
        }

        
    }
}
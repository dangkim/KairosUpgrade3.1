using System;
using System.Text;
using System.Collections.Generic;

using Slot.Model.Utility;


namespace Slot.Model
{
    // Represents a collections of pay lines. The first line number is 1
    public class Payline
    {
        public Payline()
        {
            this.Lines = new Dictionary<int, List<PaylineConfig>>();
        }

        public Payline(Dictionary<int, byte[]> rawpaylines, int numlines, Func<int, int, PaylineConfig> plenconding)
        {
            this.Lines = new Dictionary<int, List<PaylineConfig>>();

            for (int i=1; i<numlines+1; ++i)
            {
                Lines[i] = new List<PaylineConfig>();

                for (int j=0; j<rawpaylines[i].Length; ++j)
                    Lines[i].Add(plenconding(j, rawpaylines[i][j]));
            }
        }

        public Dictionary<int, List<PaylineConfig>> Lines
        {
            get; set;
        }

        public List<PaylineConfig> this[int index]
        {
            get { return this.Lines[index]; }
            set { this.Lines[index] = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var line in this.Lines)
                sb.AppendLine(string.Format("[{0}] : {1}", line.Key, line.Value.ToCommaDelimitedString()));

            return sb.ToString();
        }

        public sbyte[] CreateCombination(Wheel wheel, int linenum)
        {
            sbyte[] line = new sbyte[wheel.Width];

            for (int r=0; r<line.Length; ++r)
            {
                PaylineConfig pc = Lines[linenum][r];
                line[r] = (sbyte)wheel[pc.Reel][pc.Position];
            }

            return line;
        }
        public int[] CreateCombinationInt(Wheel wheel, int linenum)
        {
            var line = new int[wheel.Width];

            for (var r = 0; r < line.Length; ++r)
            {
                var paylineConfig = Lines[linenum][r];
                line[r] = wheel[paylineConfig.Reel][paylineConfig.Position];
            }

            return line;
        }
    }

    public struct PaylineConfig
    {
        public readonly int Reel;

        public readonly int Position;

        public PaylineConfig(int reel, int position)
        {
            this.Position = position;
            this.Reel = reel;
        }
    }
}

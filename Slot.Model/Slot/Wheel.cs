using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Slot.Model
{
    [Serializable]
    public class Wheel :IWheel
    {
        private int height;
        private int width;

        private List<List<int>> reels;

        public Wheel()
        {
        }

        public Wheel(int width)
        {
            this.Width = width;
            this.SetupReels();
            FallDownIndexes = new List<int>(Enumerable.Repeat(-1, width));
            FallDownAmount = new List<int>(Enumerable.Repeat(0, width));
        }

        public Wheel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Rows = new List<int>();
            FallDownIndexes = new List<int>(Enumerable.Repeat(-1, width));
            FallDownAmount = new List<int>(Enumerable.Repeat(0, width));
            this.SetupReels();
        }

        public Wheel(int width, int height, string value)
        {
            this.Width = width;
            this.Height = height;
            this.Rows = new List<int>();

            this.SetupReels();

            var symbols = value.Split(',').Select(int.Parse).ToList();

            for (var reel = 0; reel < this.Width; reel++)
            {
                this.Reels[reel].AddRange(symbols.GetRange(reel * this.Height, this.Height));
            }
        }

        public Wheel(List<int> rows)
        {
            this.Width = rows.Count;
            this.Height = rows.Max();
            this.Rows = rows;

            this.SetupReels();
            FallDownIndexes = new List<int>(Enumerable.Repeat(-1, width));
            FallDownAmount = new List<int>(Enumerable.Repeat(0, width));
        }

        public Wheel(List<int> rows, string value)
        {
            this.Width = rows.Count;
            this.Height = rows.Max();
            this.Rows = rows;

            this.SetupReels();

            var symbols = value.Split(',').Select(int.Parse).ToList();

            for (var reel = 0; reel < this.Width; reel++)
            {
                this.Reels[reel].AddRange(symbols.GetRange(GetPosition(reel, 0), GetReelHeight(reel)));
            }
        }

        public Wheel(Wheel wheel)
        {
            FallDownAmount = wheel.FallDownAmount;
            FallDownIndexes = wheel.FallDownIndexes;
            Height = wheel.Height;
            Mode = wheel.Mode;
            ParSheetClsId = wheel.ParSheetClsId;
            Reels = wheel.Reels;
            ReelSets = wheel.ReelSets;
            ReelStripsId = wheel.ReelStripsId;
            Rows = wheel.Rows;
            Type = wheel.Type;
            Width = wheel.Width;
        }

        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        public List<List<int>> Reels
        {
            get { return reels; }
            set { reels = value; }
        }

        public WheelType Type { get; set; }

        public int ParSheetClsId { get; set; }
        public List<int> FallDownAmount { get; set; }
        public List<int> FallDownIndexes { get; set; }

        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public List<int> Rows { get; set; }

        public string ReelStripsId { get; set; }

        public int Mode { get; set; }

        public List<ReelSet> ReelSets { get; set; }

        /// <summary>
        /// Gets or sets the list of elements at the specified reel.
        /// </summary>
        /// <param name="reel">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Index is less than 0 or index is greater than the width of <see cref="Wheel"/>.
        /// </exception>
        /// <returns>The list of elements.</returns>
        public List<int> this[int reel]
        {
            get { return this.Reels[reel]; }

            set
            {
                if (reel >= 0 && reel < this.Width)
                {
                    // if (value.Count != this.Height) { throw new ArgumentException( "The length of
                    // List<int> is not consistent with the height of the wheel."); }
                    this.Reels[reel] = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public int Count(int symbol)
        {
            return this.Reels.SelectMany(s => s).Count(s => s == symbol);
        }

        public int CountDistinct(int symbol)
        {
            return this.Reels.Count(reel => reel.Any(s => s == symbol));
        }

        public int CountDistinct(List<int> symbols)
        {
            return this.Reels.Count(reel => reel.Any(symbols.Contains));
        }

        /// <summary>
        /// Get the range of elements for the specified reel from the start index. The number of
        /// elements returned is specified by the count.
        /// </summary>
        /// <param name="reel">The zero-based reel index.</param>
        /// <param name="startIndex">The zero-based start index.</param>
        /// <param name="count">The number of elements to get from the reel.</param>
        /// <returns>The list of elements.</returns>
        public List<int> GetRange(int reel, int startIndex, int count)
        {
            var list = this.Reels[reel];

            if (startIndex + count <= list.Count)
            {
                return list.GetRange(startIndex, count);
            }

            var result = list.GetRange(startIndex, list.Count - startIndex);

            result.AddRange(list.GetRange(0, count - (list.Count - startIndex)));

            return result;
        }

        /// <summary>
        /// Display the elements of a reel at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the reel.</param>
        /// <returns>
        /// The comma-delimited string of elements at the specified reel. e.g. [0] : 1,2,3
        /// </returns>
        public string ToCustomString(int index)
        {
            return string.Format("[{0}] : {1}", index, string.Join(",", this.Reels[index]));
        }

        /// <summary>
        /// Convert the wheel into a <see cref="List"/>.
        /// </summary>
        /// <returns>The <see cref="List"/>.</returns>
        public List<int> ToList()
        {
            var result = new List<int>();

            for (int i = 0; i < this.Width; i++)
            {
                result.AddRange(this.Reels[i]);
            }

            return result;
        }

        /// <summary>
        /// copy the wheel
        /// </summary>
        /// <returns>The <see cref="object"/>.</returns>
        public virtual Wheel Copy()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, this);
            memoryStream.Position = 0;
            return (Wheel)binaryFormatter.Deserialize(memoryStream);
        }

        private void SetupReels()
        {
            this.Reels = new List<List<int>>();
            for (int i = 0; i < this.Width; i++)
                this.Reels.Add(new List<int>());
        }

        private static Wheel WheelEncodingLocal(int width, int height, List<int> rows, List<int> arr)
        {
            var w = (rows?.Any() ?? false) ? new Wheel(rows) : new Wheel(width, height);

            for (var i = 0; i < w.Width; i++)
            {
                for (var j = 0; j < w.GetReelHeight(i); j++)
                {
                    w[i].Add(arr[w.GetPosition(i, j)]);
                }
            }

            return w;
        }

        private static Wheel WheelEncodingOutsource(int width, int height, List<int> rows, List<int> arr)
        {
            Wheel w = new Wheel(width, height);
            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                    w[j].Add(arr[(i * width) + j]);

            return w;
        }

        public int GetReelHeight(int reel)
        {
            if (this.Rows?.Any() ?? false)
                return this.Rows[reel];

            return this.Height;
        }

        public int GetPosition(int reel, int row)
        {
            if (this.Rows?.Any() ?? false)
            {
                var pos =
                    this.Rows.Select((item, index) => new { RowCount = item, Reel = index })
                    .Where(r => r.Reel < reel)
                    .Sum(r => r.RowCount);

                return pos + row;
            }

            return (reel * this.Height) + row;
        }

        public void FillSymbol(int symbol, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var reel = i / this.Height;
                var row = i % this.Height;

                this.Reels[reel][row] = symbol;
            }
        }
    }
}
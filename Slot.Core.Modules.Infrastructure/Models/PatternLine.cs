using Slot.Model.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slot.Core.Modules.Infrastructure.Models
{
    public class PatternLine
    {
        private readonly IList<int[]> lines;

        public int[] this[int line]
        {
            get
            {
                return lines[line];
            }
        }

        public static implicit operator PatternLine(IList<int>[] lines)
        {
            return new PatternLine(lines);
        }

        protected PatternLine(IList<int>[] items)
        {
            lines = new List<int[]>(items.Select(ele => ele.ToArray()));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var line in lines)
                sb.AppendLine(string.Format("[{0}] : {1}", lines.IndexOf(line), line.ToCommaDelimitedString()));

            return sb.ToString();
        }
    }
}
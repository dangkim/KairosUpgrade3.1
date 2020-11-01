namespace Slot.Core.Modules.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Slot.Core.Modules.Infrastructure.Models;
    using Line = System.Collections.Generic.IList<int>;
    using Odds = System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.IReadOnlyList<int>>;
    using Ratio = System.Collections.Generic.KeyValuePair<int, Models.SortedWeighted>;
    using Reel = System.Collections.Generic.IReadOnlyList<int>;
    using ReelStrip = System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.IReadOnlyList<System.Collections.Generic.IReadOnlyList<int>>>;
    using Symbol = System.Int32;
    using WildMultiplierReel = System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.IReadOnlyList<Models.SortedWeighted>>;
    public static class ConfigurationCreator
    {
        public static IReadOnlyList<Symbol> Reel(params Symbol[] reel)
        {
            return reel;
        }

        public static ReelStrip ReelStrip(int level, params Reel[] reels)
        {
            return new ReelStrip(level, reels);
        }

        public static Tuple<int, SortedWeighted, ListStrips> ReelStrip(int level, SortedWeighted weighted, ListStrips strips)
        {
            return Tuple.Create(level, weighted, strips);
        }

        public static ReelStrips ReelStrips(params ReelStrip[] reelStrips)
        {
            return reelStrips.ToDictionary(item => item.Key, item => item.Value);
        }

        public static ReelStripsWeighted ReelStrips(params Tuple<int, SortedWeighted, ListStrips>[] reelStrips)
        {
            var weighted = Weighted(reelStrips.Select(ele => new Ratio(ele.Item1, ele.Item2)).ToArray());
            var listStrips = reelStrips.ToDictionary(ele => ele.Item1, ele => ele.Item3 as IReadOnlyList<BaseStrips>);
            return Tuple.Create<Weighted, IDictionary<int, IReadOnlyList<BaseStrips>>>(weighted, listStrips);
        }

        public static IReadOnlyList<int> Odds(params int[] odds)
        {
            return odds;
        }

        public static Odds Pay(Symbol symbol, IReadOnlyList<int> odds)
        {
            return new Odds(symbol, odds);
        }

        public static PayTable PayTables(params Odds[] paytables)
        {
            return paytables.ToDictionary(item => item.Key, item => item.Value);
        }

        public static Ratio Ratio(int level, params double[] ratios)
        {
            return new Ratio(level, new SortedWeighted(ratios));
        }

        public static SortedWeighted Weighted(params double[] ratios)
        {
            return new SortedWeighted(ratios);
        }

        public static Weighted Weighted(params Ratio[] ratios)
        {
            return ratios.ToDictionary(item => item.Key, item => item.Value);
        }

        public static int[] Line(params int[] indexes)
        {
            return indexes;
        }

        public static PatternLine Paylines(params Line[] lines)
        {
            return lines.ToArray();
        }

        public static WildMultiplierReel Multiplier(int level, params SortedWeighted[] weighteds)
        {
            return new WildMultiplierReel(level, weighteds);
        }

        public static WildMultiplier WildMultipliers(params WildMultiplierReel[] reelMultipliers)
        {
            return reelMultipliers.ToDictionary(item => item.Key, item => item.Value);
        }

        public static IReadOnlyList<Symbol> Wheel(params Symbol[] symbols) => symbols;

    }
}
using Slot.Model;
using System.Collections.Generic;

namespace Slot.Core.Modules.Infrastructure.Models
{
    public struct Bets
    {
        public IWheel Wheel { get; set; }
        public IExtraGameSettings ExtraGameSettings { get; set; }
        public bool FunPlayDemo { get; set; }
        public IReadOnlyList<decimal> Coins { get; set; }
        public IReadOnlyList<int> Multipliers { get; set; }
        public Bonus Bonus { get; set; }
        /// <summary>
        /// unix milliseconds
        /// </summary>
        public long ServerTime { get; set; }
        /// <summary>
        /// last line bet
        /// </summary>
        public decimal Bet { get; set; }
    }
}
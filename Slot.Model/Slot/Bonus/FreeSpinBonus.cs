using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Slot.Model
{
    /// <summary>Represents the bonus information specific to free spin. The information stored in this class will be 'saved' in the server-side.</summary>
    [Serializable]
    public class FreeSpinBonus : Bonus
    {
        /// <summary>Gets or sets the bonus free spin items.</summary>
        public List<BonusFreeSpinItem> BonusFreeSpinItems { get; set; }

        /// <para>This counter indicates the number of free spin left. It will be decreased by 1 every time a free spin has completed. Counter = 0 indicates that there is no free spin left.</para>
        public int Counter { get; set; }

        public Dictionary<int, int> ReelSetsCounter { get; set; }

        /// <para>This is the sum of all the amount won during the free spin.</para>
        public decimal CumulativeWin { get; set; }

        /// <para>Certain free spin feature will have an optional bonus stage at the start to allow user a random chance to increase their free spin or multiplier. This is used to indicate the round number when the actual free spin will take place. </para>
        public int FreeSpinRound { get; set; }

        /// <para>This is the amount won during the main game that also triggered the free spin.</para>
        public decimal GameWin { get; set; }

        /// <summary>Gets or sets a value indicating whether has optional bonus stage.</summary>
        public bool HasOptionalBonusStage { get; set; }

        /// <summary>Gets or sets the max steps per round.</summary>
        public Dictionary<int, int> MaxStepsPerRound { get; set; }

        /// <para>This is the total free spin multiplier (including all free spin multiplier won during the optional bonus stage). All amounts won during free spin will be multiplied by this multiplier.</para>
        public int Multiplier { get; set; }

        /// <para>This is the total number of free spin (including all free spin won during the optional bonus stage).</para>
        public int NumOfFreeSpin { get; set; }

        /// <para>This free spin payout will be calculated based on this <see cref="SpinBet"/> information such as the line bet, lines and multiplier.</para>
        
        public SpinBet SpinBet { get; set; }

        public int RetriggerCount { get; set; }

        public double SelectedBonusWeight { get; set; }

        public Wheel PreviousWheel { get; set; }

        public int ReSpinCounter { get; set; }

        public int CurrentFreeSpinsNumber { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
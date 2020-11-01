using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slot.Model;

namespace Slot.Games.BullRush.Models
{
    /// <summary>Represents the bonus information specific to free spin. The information stored in this class will be 'saved' in the server-side.</summary>
    [Serializable]
    public class BullRushFreeSpinBonus : Bonus
    {
        /// <para>This counter indicates the number of free spin left. It will be decreased by 1 every time a free spin has completed. Counter = 0 indicates that there is no free spin left.</para>
        public int RacingCounter { get; set; }

        /// <para>This counter indicates the number of free spin left. It will be decreased by 1 every time a free spin has completed. Counter = 0 indicates that there is no free spin left.</para>
        public int BonusRacingCounter { get; set; }

        /// <para>This is the sum of all the amount won during the free spin.</para>
        public decimal CumulativeWin { get; set; }

        /// <para>This is the amount won during the main game that also triggered the free spin.</para>
        public decimal GameWin { get; set; }

        public bool IsFreeSpin { get; set; }

        /// <para>This is the total free spin multiplier (including all free spin multiplier won during the optional bonus stage). All amounts won during free spin will be multiplied by this multiplier.</para>
        public int Multiplier { get; set; }

        /// <para>This is the total number of free spin (including all free spin won during the optional bonus stage).</para>
        public int NumOfFreeSpin { get; set; }

        //public int CurrentStep { get; set; }

        public int CurrentFreeSpinStep { get; set; }

        /// <para>This free spin payout will be calculated based on this <see cref="SpinBet"/> information such as the line bet, lines and multiplier.</para>

        public SpinBet SpinBet { get; set; }

        public Wheel PreviousWheel { get; set; }

        public bool IsFromBaseGame { get; set; }

        public int RandomMultiplier { get; set; }

        public int PreviousPowerUp { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
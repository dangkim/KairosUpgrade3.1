using System;

using Newtonsoft.Json;


namespace Slot.Model
{
    /// <summary>Represents the spin bet that contains all the necessary information associated to the bet.</summary>
    [Serializable]
    public class SpinBet : GameBet
    {
        /// <summary>Initializes a new instance of the <see cref="SpinBet"/> class.</summary>
        public SpinBet(UserGameKey userGameKey, PlatformType platformType) : base(userGameKey, platformType)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SpinBet"/> class.</summary>
        [Obsolete("Used for XML serialization only.", true)]
        public SpinBet()
        {
        }

        public int CurrencyId { get; set; }

        public int GameSettingGroupId { get; set; }

        public bool IsAutoSpin { get; set; }

        public decimal LineBet { get; set; }

        public int Lines { get; set; }

        public int Credits { get; set; }

        public int Multiplier { get; set; }

        public decimal TotalBet => this.LineBet * this.Multiplier * (this.Credits > 0 ? this.Credits : this.Lines);

        public Wheel Wheel { get; set; }

        public int FunPlayDemoKey { get; set; }

        public int Reel { get; set; }

        public string ToCustomString()
        {
            return JsonConvert.SerializeObject(new { SpinBet = this }, Formatting.Indented);
        }
    }

    [Serializable]
    public class SpinBetX : SpinBet
    {
        /// <summary>Initializes a new instance of the <see cref="SpinBet"/> class.</summary>
        public SpinBetX(UserGameKey userGameKey, PlatformType platformType)
            : base(userGameKey, platformType)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SpinBet"/> class.</summary>
        [Obsolete("Used for XML serialization only.", true)]
        public SpinBetX()
        {
        }

        public bool IsSideBet { get; set; }
    }

    [Serializable]
    public class FreeRoundSpinBet : SpinBet
    {
        /// <summary>Initializes a new instance of the <see cref="FreeRoundSpinBet"/> class.</summary>
         public FreeRoundSpinBet(UserGameKey userGameKey, PlatformType platformType)
            : base(userGameKey, platformType)
        {
        }

        [Obsolete("Used for XML serialization only.", true)]
        public FreeRoundSpinBet()
        {
        }
    }
}
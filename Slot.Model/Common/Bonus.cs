using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Slot.Model
{
    [Serializable]
    public abstract class Bonus
    {
        [JsonIgnore]
        public BonusType BonusType { get; set; }
        [JsonIgnore]
        public int? CurrentStage { get; set; }
        [JsonIgnore]
        public int CurrentRound { get; set; }
        [JsonIgnore]
        public int CurrentStep { get; set; }
      
        public GameResult GameResult { get; set; }

        public Guid Guid { get; set; }

        public int Id => (int)this.BonusType;

        public virtual bool IsCompleted { get; set; }

        public bool IsOptional { get; set; }

        public bool IsStarted { get; set; }

        public int MaxRound { get; set; }

        public int MaxStepPerCurrentRound { get; set; }
        [JsonIgnore]
        public long SpinTransactionId { get; set; }

        [IgnoreDataMember]
        public UserGameKey UserGameKey { get; set; }

        /// <summary>Gets the version.
        /// <para>Returns the current version of the Bonus structure for serialization.</para>
        /// </summary>
        public int Version => 2;
    }
}
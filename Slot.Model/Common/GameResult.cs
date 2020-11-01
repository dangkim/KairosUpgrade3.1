using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json;
using Slot.Model.Utilities;

namespace Slot.Model
{
    [Serializable]
    public abstract class GameResult : IConvertibleToResponseXml, IGameResult
    {        
        [JsonIgnore]
        public int GameId { get; protected set; }

        [JsonIgnore]
        public long RoundId { get; set; }

        public Balance Balance { get; set; }

        public decimal Bet { get; set; }

        [JsonIgnore]
        public DateTime DateTimeUtc { get; set; }

        [JsonIgnore]
        public bool IsError => ErrorCode == ErrorCode.None;

        [JsonIgnore]
        public ErrorCode ErrorCode { get; set; }

        [JsonIgnore]
        public ErrorSource ErrorSource { get; set; }

        [JsonIgnore]
        public decimal? ExchangeRate { get; set; }

        [JsonIgnore]
        public abstract GameResultType GameResultType { get; }

        [JsonIgnore]
        public bool IsHistory { get; set; }

        [JsonIgnore]
        public bool IsReport { get; set; }

        [JsonIgnore]
        public int Level { get; set; }

        [JsonIgnore]
        public PlatformType PlatformType { get; set; }

        [JsonIgnore]
        public long? SpinTransactionId { get; set; }

        [JsonIgnore]
        public long TransactionId { get; set; }

        [JsonConverter(typeof(EncryptingJsonConverter), "#V*S8cr8t__V")]
        public string UniqueID { get; set; }

        [JsonIgnore]
        public GameTransactionType TransactionType { get; set; }

        // [IgnoreDataMember]
        // [JsonIgnore]
        // public UserGameKey UserGameKey { get; set; }

        public decimal Win { get; set; }

        public bool IsFreeGame { get; set; }

        [JsonIgnore]
        public int CampaignId { get; set; }

        [JsonIgnore]
        public bool FRExpired { get; set; }

        public List<BonusPosition> BonusPositions { get; set; }

        [JsonIgnore]
        public decimal FRWinLose { get; set; }

        public abstract XmlType XmlType { get; }

        public abstract XElement ToXElement();

        protected abstract ResponseXml ToXml(ResponseXmlFormat format);

        public ResponseXml ToResponseXml(ResponseXmlFormat format)
        {
            return this.ErrorCode == ErrorCode.None ? this.ToXml(format) : ErrorXml.Create(this.ErrorCode);
        }
    }
}
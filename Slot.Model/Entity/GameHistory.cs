using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Omu.ValueInjecter;
using Slot.Model.Utility;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("GameHistory")]
    public class GameHistory : BaseEntity<long>
    {
        [Column(Order = 6)]
        public decimal Bet { get; set; }

        [Column(Order = 1)]
        public DateTime DateTimeUtc { get; set; }

        [Column(Order = 8)]
        public decimal ExchangeRate { get; set; }

        public virtual Game Game { get; set; }

        [Column(Order = 4)]
        public int GameId { get; set; }

        [Column(Order = 9)]
        public GameResultType GameResultType { get; set; }

        public virtual GameTransaction GameTransaction { get; set; }

        [Column(Order = 2)]
        public long GameTransactionId { get; set; }

        [Column(Order = 12)]
        [MaxLength(12000)]
        public string HistoryXml { get; set; }

        [Column(Order = 13)]
        public bool IsHistory { get; set; }

        [Column(Order = 14)]
        public bool IsReport { get; set; }

        [Column(Order = 5)]
        public int Level { get; set; }

        [Column(Order = 15)]
        public PlatformType PlatformType { get; set; }

        [Column(Order = 11)]
        [MaxLength(12000)]
        public string ResponseXml { get; set; }

        public virtual GameTransaction SpinTransaction { get; set; }

        [Column(Order = 16)]
        public long? SpinTransactionId { get; set; }

        public virtual User User { get; set; }

        [Column(Order = 3)]
        public int UserId { get; set; }

        [Column(Order = 7)]
        public decimal Win { get; set; }

        [Column(Order = 10)]
        public XmlType XmlType { get; set; }

        [Column(TypeName = "bit")]
        [DefaultValue("0")]
        public bool IsFreeGame{ get; set; }

        [DefaultValue("0")]
        public long RoundId { get; set; }

        //[DefaultValue("0")]
        //public int FreeRoundId { get; set; }

        //[NotMapped]
        //public GameResult GameResult
        //{
        //    get
        //    {
        //        Type type = this.XmlType.ToType();
        //        MethodInfo method = typeof(XmlHelper).GetMethod("Deserialize");
        //        MethodInfo generic = method.MakeGenericMethod(type);

        //        var serializer = new XmlHelper();
        //        var responseXml = generic.Invoke(serializer, new object[] { this.ResponseXml });

        //        var userGameKey = new UserGameKey(this.UserId, (GameId)this.GameId);
        //        var gameResult = GameResultFactory.Create(this.GameResultType, userGameKey);

        //        gameResult.InjectFrom(responseXml);

        //        if (responseXml.GetType() == typeof(BonusXml))
        //            gameResult.InjectFrom<BonusXmlToGameResult>(responseXml);

        //        return gameResult;
        //    }
        //}
    }
}
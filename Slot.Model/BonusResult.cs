using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

using Omu.ValueInjecter;

namespace Slot.Model
{
    [Serializable]
    public abstract class BonusResult : GameResult
    {
        protected BonusResult(UserGameKey userGameKey)
        {
            this.Balance = new Balance();
            this.IsHistory = true;
            this.IsReport = true;
            this.IsFreeGame = userGameKey.IsFreeGame;
            this.CampaignId = userGameKey.CampaignId;
            this.FRExpired = userGameKey.FRExpired;
            this.FRWinLose = userGameKey.FRWinLose;
        }

        public bool IsCompleted { get; set; }

        public int Step { get; set; }

        public int? NextStep { get; set; }

        public Bonus Bonus { get; set; }

        public string Type { get; set; }

        public override XElement ToXElement()
        {
            var bonusXml = new BonusXml();

            bonusXml.InjectFrom(this);

            this.CreateBonusXElement(bonusXml);
            this.CreateDataXElement(bonusXml);

            return bonusXml.ToXElement();
        }

        protected abstract void CreateBonusXElement(BonusXml bonusXml);

        protected abstract void CreateDataXElement(BonusXml bonusXml);

        protected override ResponseXml ToXml(ResponseXmlFormat format)
        {
            var bonusXml = new BonusXml();

            bonusXml.InjectFrom(this);

            return bonusXml;
        }
    }
}
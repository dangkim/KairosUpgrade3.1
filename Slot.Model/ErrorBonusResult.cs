using System;


namespace Slot.Model
{
    [Serializable]
    public class ErrorBonusResult : BonusResult
    {
        public ErrorBonusResult(UserGameKey userGameKey)
            : base(userGameKey)
        {
            this.Type = "error";
        }

        public override GameResultType GameResultType => throw new NotImplementedException();

        public override XmlType XmlType => throw new NotImplementedException();

        protected override void CreateBonusXElement(BonusXml bonusXml)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDataXElement(BonusXml bonusXml)
        {
            throw new NotImplementedException();
        }
    }
}
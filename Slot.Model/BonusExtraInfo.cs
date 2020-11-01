using System;

namespace Slot.Model
{
    [Serializable]
    public class BonusExtraInfo
    {
        public BonusExtraInfo()
        {
            RoundId = 0;
            BetId = string.Empty;
            BonusClsId = 0;
            ClientId = 0;
            Order = 0;
            IsFreeGame = false;
        }

        public long RoundId { get; set; }

        public string BetId { get; set; }

        public int BonusClsId { get; set; }

        public int ClientId { get; set; }

        public int Order { get; set; }

        public bool IsFreeGame { get; set; }
    }
}
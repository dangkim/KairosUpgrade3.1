using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model;

namespace Slot.Games.BullRush.Models
{
    [Serializable]
    public class BullRushSpinResult : SpinResult
    {
        public BullRushSpinResult()
        {
            this.BonusElement = new BullRushBonusElement();
            this.TableWins = new HashSet<BullRushTableWin>();
            this.WinElement = new Win();
            this.WinPositions = new List<BullRushWinPosition>();
            this.IsHistory = true;
            this.IsReport = true;
            this.RowPositions = new List<int>();
            this.InventoryList = new List<int>();
            this.BonusRacingPrizesList = new List<decimal>();
            this.SelectedPowerUps = new List<int>();
            this.InventoryOfThreePowerUps = new List<int>();
            this.SimulationDatas = new Dictionary<object, object>();
            this.InnerWheel = new Wheel(1, 8, "0,3,4,3,5,3,4,3");
            this.OuterWheel = new Wheel(1, 8, "4,0,-1,1,4,3,-1,2");
            this.SelectedOuterWheelIndex = -1;
            this.SelectedInnerWheelIndex = -1;
            this.CurrentJackpotStep = 1;
        }

        [JsonIgnore]
        public Dictionary<object, object> SimulationDatas { get; set; }

        [JsonIgnore]
        public BullRushBonusElement BonusElement { get; set; }

        public Wheel InnerWheel { get; set; }

        public Wheel OuterWheel { get; set; }

        [JsonIgnore]
        public int RandomMultiplier { get; set; }

        [JsonIgnore]
        public int NumOfFreeSpin { get; set; }

        [JsonIgnore]
        public int NumOfJackpot { get; set; }

        public BonusStruct Bonus;

        [JsonIgnore]
        public bool IsBonusFG { get; set; }

        public int CurrentJackpotStep { get; set; }

        [JsonIgnore]
        public int PreviousPowerUp { get; set; }

        public int CurrentRacingStep { get; set; }

        public int CurrentRacingCounter { get; set; }

        public int CurrentBonusRacingCounter { get; set; }

        public int CurrentJackpotCounter { get; set; }

        public int VariantWheel { get; set; }

        public List<int> RowPositions { get; set; }

        public List<int> InventoryList { get; set; }

        public List<int> InventoryOfThreePowerUps { get; set; }

        public List<decimal> BonusRacingPrizesList { get; set; }

        public List<int> SelectedPowerUps { get; set; }

        [JsonIgnore]
        public int NumberOfMagnetActiveRows { get; set; }

        [JsonIgnore]
        public int NumberOfVacuumActiveRows { get; set; }

        [JsonIgnore]
        public int NumberOfShieldActiveRows { get; set; }

        public int SelectedOuterWheelValue { get; set; }

        public int SelectedOuterWheelIndex { get; set; }

        public int SelectedInnerWheelValue { get; set; }

        public int SelectedInnerWheelIndex { get; set; }

        public List<BonusPosition> MegaMoneyBonusPosition { get; set; }

        public List<BonusPosition> FreeSpinPosition { get; set; }

        public decimal CumulativeWin { get; set; }

        public int JackpotAndCreditsValue { get; set; }

        public bool IsRacing { get; set; }

        public bool IsBonusRacing { get; set; }

        public decimal SelectedBonusRacingPrize { get; set; }

        public decimal SelectedBonusRacingPrizeIndex { get; set; }

        public bool IsInnerWheelBonus { get; set; }

        public List<List<decimal>> DistributedAllRows { get; set; }

        public SpinBet SpinBet { get; set; }

        [JsonIgnore]
        public HashSet<BullRushTableWin> TableWins { get; set; }

        public bool IsBonus { get; set; }

        public override bool HasBonus { get { return IsBonus; } }

        [JsonIgnore]
        public decimal BaseGameWin { get; set; }

        [JsonIgnore]
        public Win WinElement { get; set; }

        [JsonIgnore]
        public List<BullRushWinPosition> WinPositions { get; set; }

        public override GameResultType GameResultType => GameResultType.SpinResult;

        public override XmlType XmlType => XmlType.SpinXml;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override XElement ToXElement()
        {
            var spinXml = new BullRushSpinXml
            {
                GameIdXml = SpinBet.UserGameKey.GameId
            };
            spinXml.InjectFrom(this);

            return spinXml.ToXElement();
        }

        protected override ResponseXml ToXml(ResponseXmlFormat format)
        {
            var spinXml = new BullRushSpinXml
            {
                GameIdXml = SpinBet.UserGameKey.GameId
            };

            spinXml.InjectFrom(this);
            spinXml.sbx = this.SpinBet as SpinBetX;

            return spinXml;
        }
    }
}

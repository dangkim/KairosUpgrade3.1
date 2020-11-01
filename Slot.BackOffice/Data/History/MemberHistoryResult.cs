using Newtonsoft.Json;
using Slot.BackOffice.Data.History.HistoryDecode;
using Slot.BackOffice.Models.Xml;
using Slot.Core.Data.Views;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Formatters;
using Slot.Model.Slot.Xml;
using Slot.Model.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameIdEnum = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History
{
    public partial class MemberHistoryResult
    {
        private readonly GameHistory gameHistory;
        private readonly SpinBetProfile spinBetProfile;
        private readonly GamePayoutEngine gamePayoutEngine;
        private readonly PaylineRepository paylineRepository;
        private readonly GameInfoRepository gameInfoRepository;
        private readonly HistoryDecoderFactory historyDecoderFactory;

        public MemberHistoryResult(GamePayoutEngine gamePayoutEngine, PaylineRepository paylineRepository, GameInfoRepository gameInfoRepository, HistoryDecoderFactory historyDecoderFactory)
        {
            this.gamePayoutEngine = gamePayoutEngine;
            this.paylineRepository = paylineRepository;
            this.gameInfoRepository = gameInfoRepository;
            this.historyDecoderFactory = historyDecoderFactory;

            WinTableByGame = new Dictionary<GameIdEnum, Func<Dictionary<int, List<PaylineConfig>>, SpinXml, List<byte[,]>>>
            {
                { GameIdEnum.FortuneTree, gamePayoutEngine.PayoutWays },
                { GameIdEnum.WorldSoccerSlot2, gamePayoutEngine.PayoutWays },
                { GameIdEnum.MoneyMonkey, gamePayoutEngine.PayoutWays },
                { GameIdEnum.JazzItUp, gamePayoutEngine.PayoutWays },
                { GameIdEnum.WolvesSlot, gamePayoutEngine.PayoutWays }
            };

            WinTableByPaylineType = new Dictionary<PaylineType, Func<Dictionary<int, List<PaylineConfig>>, SpinXml, List<byte[,]>>>
            {
                { PaylineType.ThreeByThree, gamePayoutEngine.PayoutNormalIndependent },
                { PaylineType.ThreeByThreeOutsource, gamePayoutEngine.PayoutNormalIndependent },
                { PaylineType.ThreeByThree27Ways, gamePayoutEngine.PayoutWaysIndependent },
                { PaylineType.ThreeByFive, gamePayoutEngine.PayoutNormal },
                { PaylineType.ThreeByFive2, gamePayoutEngine.PayoutNormal },
                { PaylineType.ThreeByFiveCollapsing, gamePayoutEngine.PayoutNormal },
                { PaylineType.FourByFive, gamePayoutEngine.PayoutNormal },
                { PaylineType.ThreeFourFiveSixSevenSevenReels, gamePayoutEngine.PayoutNormal },
                { PaylineType.ThreeByThreeMatchingReels, gamePayoutEngine.PayoutNormalIndependent },
                { PaylineType.SevenByFive, gamePayoutEngine.PayoutNormal }
            };
        }

        public MemberHistoryResult(
            GamePayoutEngine gamePayoutEngine,
            PaylineRepository paylineRepository,
            GameInfoRepository gameInfoRepository,
            GameHistory gameHistory,
            HistoryDecoderFactory historyDecoderFactory,
            SpinBetProfile spinBetProfile)
            : this(gamePayoutEngine, paylineRepository, gameInfoRepository, historyDecoderFactory)
        {
            this.gameHistory = gameHistory;
            this.spinBetProfile = spinBetProfile;

            RoundId = gameHistory.RoundId;
            TransactionId = GetTransactionId(this.gameHistory);
            TotalBet = GetTotalBet();

            GameId = gameHistory.Game.Id;
            Game = gameHistory.Game.Name;
            TotalWin = gameHistory.Win;
            GameResultType = gameHistory.GameResultType;
            XmlType = gameHistory.XmlType;
            DateTimeUtc = gameHistory.CreatedOnUtc;
            DateTime = gameHistory.CreatedOnUtc.ToLocalTime();
            IsSideBet = spinBetProfile?.IsSideBet == true;

            var historyXmlDecoder = historyDecoderFactory.Resolve((GameIdEnum)GameId);
            if(historyXmlDecoder != null)
            {
                historyXmlDecoder.AmendSpinHistory(this, gameHistory);
            }
            else
            {
                SetResultData();
            }
        }

        public long RoundId { get; }

        public long TransactionId { get; }

        public string Game { get; }

        public int GameId { get; }

        public decimal TotalBet { get; }

        public decimal TotalWin { get; }

        [JsonConverter(typeof(DataFormatter), Formats.DateTime)]
        public DateTime DateTimeUtc { get; }

        [JsonConverter(typeof(DataFormatter), Formats.DateTime)]
        public DateTime DateTime { get; }

        public Dictionary<int, List<PaylineConfig>> PayLine { get; set; }

        public SpinXml SpinXml { get; set; }

        public WheelViewModel Wheel { get; set; }

        public WheelViewModel PostWheel { get; set; }

        public IEnumerable<WinPosition> OriginalWinPosition { get; set; }

        public IEnumerable<WinPositionExpanding> ExpandingWinPosition { get; set; }

        public BonusXml BonusXml { get; set; }

        public CollapseXml CollapseXml { get; set; }

        public string BonusDataDetail { get; set; }

        public GameResultType GameResultType { get; set; }

        public string GameResultTypeString { get => DataConverter.Description(GameResultType); }

        public XmlType XmlType { get; set; }

        public List<History> History { get; set; }

        public HistoryType HistoryType { get; set; }

        public List<byte[,]> WinTable { get; set; }

        public bool IsSideBet { get; }

        public int AdditionalFreeSpin { get; set; }

        public string GameResultTypeDetail => DataConverter.Description(GameResultType);

        public Dictionary<int, ReelSetViewModel> ReelSets { get; set; }

        public int? RespunReel { get; set; }

        public static long GetTransactionId(GameHistory gameHistory)
        {
            return gameHistory.SpinTransactionId ?? gameHistory.GameTransactionId;
        }

        private decimal GetTotalBet()
        {
            var totalBet = 0m;

            if (!gameHistory.IsFreeGame)
            {
                if (spinBetProfile?.IsSideBet == true && gameHistory.GameResultType == GameResultType.SpinResult)
                {
                    totalBet = gameHistory.Bet * 2;
                }
                else if (gameHistory.GameResultType == GameResultType.CollapsingSpinResult || gameHistory.GameResultType == GameResultType.FreeSpinCollapsingSpinResult)
                {
                    totalBet = 0;
                }
                else
                {
                    totalBet = gameHistory.Bet;
                }
            }

            return totalBet;
        }

        private void SetResultData()
        {
            if (XmlType == XmlType.SpinXml)
            {
                DisplaySpinHistory(gameHistory);
            }
            else
            {
                switch (GameResultType)
                {
                    case GameResultType.DoubleUpResult:
                        DisplayDoubleUpHistory(gameHistory);
                        break;
                    case GameResultType.GambleResult:
                        DisplayGambleHistory(gameHistory);
                        break;
                    case GameResultType.FreeSpinResult:
                        DisplayFreeSpinHistory(gameHistory);
                        break;
                    case GameResultType.FreeSpinCollapsingSpinResult:
                    case GameResultType.CollapsingSpinResult:
                        XElement xml = XElement.Parse(gameHistory.HistoryXml);

                        if (xml != null && gameHistory.GameId == (int)GameIdEnum.AlchemistsSpell)
                        {
                            var spinXml = xml.Element("data").Element("spin");
                            DisplayBonusCollapsingSpin(spinXml);
                        }
                        else
                        {
                            if (xml != null)
                            {
                                var htype = xml.Attribute("type").Value;
                                var isfsCollapsing = "cs" == htype && null != xml.Element("data");
                                if (isfsCollapsing)
                                {
                                    DisplayCollapseSpinHistory(xml);
                                }
                            }

                            DisplayFreeSpinHistory(gameHistory);
                        }

                        break;
                    case GameResultType.BonusFreeSpinResult:
                        DisplayBonusFreeSpinHistory(gameHistory);
                        break;

                    case GameResultType.RevealResult:
                        DisplayRevealBonusHistory(gameHistory);
                        break;

                    case GameResultType.InstantWinResult:
                        HistoryType = HistoryType.Reveal;
                        History = new List<History>();
                        break;

                    case GameResultType.MultiModeResult:
                        if (gameHistory.HistoryXml.Contains("type=\"b\""))
                        {
                            GameResultType = GameResultType.RevealResult;
                            DisplayRevealBonusHistory(gameHistory);
                        }
                        DisplayFreeSpinHistory(gameHistory);
                        break;
                }
            }
        }
    }

    public class WheelViewModel
    {
        public List<List<Symbols>> reels;
    }

    public class Symbols
    {
        public Symbols()
        {
        }

        public Symbols(bool isEmpty)
        {
            IsEmpty = isEmpty;
        }

        public int symbol;
        public int height;
        public int width;
        public int OverprintWildSymbol;
        public int OverprintDiceSymbol;
        public bool IsEmpty;
    }

    public class History
    {
        public string selected;
        public string result;
        public string mul;
        public string value;
        public string bet;
        public string dcard;
        public string pcard;
    }

    public class ReelSetViewModel
    {
        public ReelSetViewModel()
        {
            this.Reels = new List<List<Symbols>>();
            this.WinTable = new List<byte[,]>();
        }

        public List<List<Symbols>> Reels { get; set; }

        public List<byte[,]> WinTable { get; set; }
    }
}

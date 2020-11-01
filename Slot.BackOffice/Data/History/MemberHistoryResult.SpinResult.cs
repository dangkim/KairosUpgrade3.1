using Newtonsoft.Json;
using Slot.BackOffice.Data.History.WildExpandings;
using Slot.BackOffice.Models.Xml;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using GameIdEnum = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History
{
    public partial class MemberHistoryResult
    {
        [JsonIgnore]
        public readonly Dictionary<int, Dictionary<int, List<int>>> WildPositions = new Dictionary<int, Dictionary<int, List<int>>>()
        {
            {
                (int)GameIdEnum.SkyStrikers, new  Dictionary<int, List<int>>()
                    {
                        { 1,    new List<int>() { 9, 12 } },
                        { 2,    new List<int>() { 0, 3  } },
                        { 3,    new List<int>() { 7, 8  } },
                        { 4,    new List<int>() { 10, 13} },
                        { 5,    new List<int>() { 1, 4  } },
                        { 6,    new List<int>() { 6, 8  } },
                        { 7,    new List<int>() { 11, 14} },
                        { 8,    new List<int>() { 2, 5  } },
                        { 9,    new List<int>() { 6, 7  } }
                    }
            }
        };

        [JsonIgnore]
        public readonly Dictionary<FortuneDiceSymbol, FortuneDiceSymbol> OverprintSymbols = new Dictionary<FortuneDiceSymbol, FortuneDiceSymbol>()
        {
            {FortuneDiceSymbol.Dice1, FortuneDiceSymbol.OverprintDice1},
            {FortuneDiceSymbol.Dice2, FortuneDiceSymbol.OverprintDice2},
            {FortuneDiceSymbol.Dice3, FortuneDiceSymbol.OverprintDice3},
            {FortuneDiceSymbol.Dice4, FortuneDiceSymbol.OverprintDice4},
            {FortuneDiceSymbol.Dice5, FortuneDiceSymbol.OverprintDice5},
            {FortuneDiceSymbol.Dice6, FortuneDiceSymbol.OverprintDice6},
            {FortuneDiceSymbol.Wild, FortuneDiceSymbol.OverprintWild},
        };

        private void EditSymbolWithOverprintSymbols()
        {
            var dices = SpinXml.DiceInfo.Dices.Select(x => x.Side).ToList();
            for (var c = 0; c < SpinXml.Wheel.Width; ++c)
                for (var r = 0; r < SpinXml.Wheel.Height; ++r)
                {
                    var wheelItem = Wheel.reels[c][r];
                    var symbol = SpinXml.Wheel.Reels[c][r];
                    var symbolDiceValue = symbol % 10;
                    if (dices.Any(x => x == symbolDiceValue))
                    {
                        wheelItem.OverprintWildSymbol = Convert.ToInt32(OverprintSymbols[FortuneDiceSymbol.Wild]);
                    }
                    if (symbolDiceValue > 0)
                    {
                        wheelItem.OverprintDiceSymbol =
                            Convert.ToInt32(OverprintSymbols[(FortuneDiceSymbol)symbolDiceValue]);
                    }
                    wheelItem.symbol = symbol / 10 * 10;
                }
        }

        private void DisplaySpinHistory(GameHistory history)
        {
            var xmlHelper = new XmlHelper();
            var xml = xmlHelper.Deserialize<SpinXml>(history.HistoryXml);
            SpinXml = xml;

            Wheel = new WheelViewModel();
            Wheel.reels = new List<List<Symbols>>();

            var wheel = SpinXml.Wheel;

            for (var c = 0; c < wheel.Reels.Count; c++)
            {
                Wheel.reels.Add(new List<Symbols>());

                for (var r = 0; r < wheel.Reels[c].Count; r++)
                {
                    var symbols = new Symbols { symbol = wheel.Reels[c][r], height = 1, width = 1 };
                    Wheel.reels[c].Add(symbols);
                }

                if (!(wheel.Rows?.Any() ?? false))
                    continue;

                for (var r = wheel.Rows[c]; r < wheel.Height; r++)
                {
                    var symbols = new Symbols(true) { symbol = -1, height = 1 };
                    Wheel.reels[c].Add(symbols);
                }
            }

            switch (history.GameId)
            {
                case (int)GameIdEnum.ForbiddenChamber:
                case (int)GameIdEnum.WorldOfWarlords:
                    if (Wheel.reels[2].Any(s => s.symbol == (int)ForbiddenChamberSymbol.Wild))
                    {
                        Wheel.reels[2] = new List<Symbols>()
                        {
                            new Symbols {symbol = (int) ForbiddenChamberSymbol.RExpandingWild, height = 3},
                            new Symbols {symbol = -1, height = 1},
                            new Symbols {symbol = -1, height = 1},
                        };
                    }

                    break;

                case (int)GameIdEnum.DesertOasis:
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)DesertOasisSymbol.Camel))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols {symbol = (int) DesertOasisSymbol.RExpandingWild, height = 3},
                                new Symbols {symbol = -1, height = 1},
                                new Symbols {symbol = -1, height = 1}
                            };
                        }
                    }

                    break;

                case (int)GameIdEnum.SevenWonders:
                case (int)GameIdEnum.FortuneKoi:
                case (int)GameIdEnum.FloraSecret:
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)SevenWondersSymbol.Wild))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols {symbol = (int) SevenWondersSymbol.RExpandingWild, height = 3},
                                new Symbols {symbol = -1, height = 1},
                                new Symbols {symbol = -1, height = 1}
                            };
                        }
                    }

                    break;

                case (int)GameIdEnum.CasinoRoyale:
                case (int)GameIdEnum.CasinoRoyalePro:
                case (int)GameIdEnum.RomanEmpire:
                    {
                        for (int i = 0; i < Wheel.reels.Count; i++)
                        {
                            if (Wheel.reels[i].Any(s => s.symbol == (int)CasinoRoyaleSymbol.Joker))
                            {
                                Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols {symbol = (int) CasinoRoyaleSymbol.RExpandingWild, height = 3},
                                new Symbols {symbol = -1, height = 1},
                                new Symbols {symbol = -1, height = 1}
                            };
                            }
                        }
                    }
                    break;
                case (int)GameIdEnum.FortuneDice:
                    EditSymbolWithOverprintSymbols();
                    break;
                case (int)GameIdEnum.FourGuardians:
                case (int)GameIdEnum.StripNRoll:
                    //TODO : later will be use DI
                    new FourGuardianExpanding().Expanding(Wheel);
                    break;
                case (int)GameIdEnum.LuckyRoyale:
                    new LuckyRoyaleExpanding().Expanding(Wheel);
                    break;
                case (int)GameIdEnum.FuLuShou:
                    new FuLuShouExpandingWild().Expanding(Wheel);
                    break;
                case (int)GameIdEnum.SkyStrikers:
                    {
                        var mode = SpinXml.BonusWheel != null ? SpinXml.BonusWheel.Mode : 0;
                        if (mode > 0)
                        {
                            var wildPos = WildPositions[history.GameId][mode];
                            foreach (var wp in wildPos)
                            {
                                var reel = wp / SpinXml.Wheel.Height;
                                var row = wp % SpinXml.Wheel.Height;
                                Wheel.reels[reel][row] =
                                    new Symbols { symbol = (int)SkyStrikersSymbol.SpecialWild, height = 1 };
                            }
                        }

                        break;
                    }

                case (int)GameIdEnum.TrickOrTreat:
                    {
                        if (Wheel.reels[2].Any(s => s.symbol == (int)TrickOrTreatSymbol.Wild))
                        {
                            Wheel.reels[2] = new List<Symbols>()
                        {
                            new Symbols {symbol = (int) TrickOrTreatSymbol.RExpandingWild, height = 3},
                            new Symbols {symbol = -1, height = 1},
                            new Symbols {symbol = -1, height = 1}
                        };
                        }

                        break;
                    }

                case (int)GameIdEnum.TokyoHunter:
                    {
                        for (var i = 0; i < Wheel.reels.Count; i++)
                        {
                            if (Wheel.reels[i].Any(s => s.symbol == (int)TokyoHunterSymbol.Wild))
                            {
                                Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols {symbol = (int) TokyoHunterSymbol.RExpandingWild, height = 3},
                                new Symbols {symbol = -1, height = 1},
                                new Symbols {symbol = -1, height = 1}
                            };
                            }
                        }

                        break;
                    }

                case (int)GameIdEnum.LionDance:
                    {
                        SpinXml.SpinBet.Lines = 27;
                        break;
                    }

                case (int)GameIdEnum.WuxiaPrincessMegaReels:
                    {
                        UpdateWheelViewModelSymbols(SpinXml.Wheel.Width, SpinXml.Wheel.Height,
                            SpinXml.Wheel.Rows, Wheel, SymbolReplacement[history.GameId]);
                        break;
                    }
                case (int)GameIdEnum.HulaGirl:
                    {
                        UpdateWheelViewModelSymbols(SpinXml.Wheel.Width, SpinXml.Wheel.Height, Wheel, SymbolReplacement[history.GameId]);
                        break;
                    }

                case (int)GameIdEnum.GeniesLuck:
                    {
                        UpdateWheelViewModelSymbols(SpinXml.Wheel.Width, SpinXml.Wheel.Height, SpinXml.Wheel.Rows, Wheel, SymbolReplacement[history.GameId], history.GameId);
                        break;
                    }
            }

            PayLine = paylineRepository.Get((GameIdEnum)history.GameId).Lines;
            WinTable = GetWinTable((GameIdEnum)history.GameId, PayLine, xml);
            RespunReel = xml.RespunReel;
            CreateReelSetsVM(history);
        }
    }
}

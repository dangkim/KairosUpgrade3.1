using Newtonsoft.Json;
using Slot.BackOffice.Data.History.WildExpandings;
using Slot.BackOffice.Models.Xml;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Slot;
using Slot.Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameIdEnum = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History
{
    public partial class MemberHistoryResult
    {
        [JsonIgnore]
        public readonly Dictionary<int, Dictionary<int, int>> FreeSpinReplacement = new Dictionary<int, Dictionary<int, int>>()
        {
            {
                (int)GameIdEnum.LuckyBomber, new  Dictionary<int, int>()
                    {
                        { (int)LuckyBomberSymbol.Jack,         (int)LuckyBomberSymbol.OrangeDiamond },
                        { (int)LuckyBomberSymbol.Queen,        (int)LuckyBomberSymbol.GreenEgg      },
                        { (int)LuckyBomberSymbol.King,         (int)LuckyBomberSymbol.BlueBomb      },
                        { (int)LuckyBomberSymbol.Ace,          (int)LuckyBomberSymbol.PurpleSphere  },
                        { (int)LuckyBomberSymbol.BluePyramid,  (int)LuckyBomberSymbol.BlueFace      },
                        { (int)LuckyBomberSymbol.GreenBox,     (int)LuckyBomberSymbol.RedFace       },
                        { (int)LuckyBomberSymbol.Sphere,       (int)LuckyBomberSymbol.YellowFace    },
                        { (int)LuckyBomberSymbol.Monster,      (int)LuckyBomberSymbol.Boss          }
                    }
            },
            {
                (int)GameIdEnum.PhantomThief, new  Dictionary<int, int>()
                    {
                        { (int)PhantomThiefSymbol.Jack,         (int)PhantomThiefSymbol.OrangeDiamond },
                        { (int)PhantomThiefSymbol.Queen,        (int)PhantomThiefSymbol.GreenEgg      },
                        { (int)PhantomThiefSymbol.King,         (int)PhantomThiefSymbol.BlueBomb      },
                        { (int)PhantomThiefSymbol.Ace,          (int)PhantomThiefSymbol.PurpleSphere  },
                        { (int)PhantomThiefSymbol.BluePyramid,  (int)PhantomThiefSymbol.BlueFace      },
                        { (int)PhantomThiefSymbol.GreenBox,     (int)PhantomThiefSymbol.RedFace       },
                        { (int)PhantomThiefSymbol.Sphere,       (int)PhantomThiefSymbol.YellowFace    },
                        { (int)PhantomThiefSymbol.Monster,      (int)PhantomThiefSymbol.Boss          }
                    }
            }
        };

        [JsonIgnore]
        public readonly Dictionary<int, Dictionary<GameHistorySymbol, GameHistorySymbol>> SymbolReplacement = new Dictionary<int, Dictionary<GameHistorySymbol, GameHistorySymbol>>()
        {
            {
                (int)GameIdEnum.HulaGirl, new Dictionary<GameHistorySymbol, GameHistorySymbol>()
                {
                    { new GameHistorySymbol((int)HulaGirlSymbol.Wild, 2), new GameHistorySymbol((int)HulaGirlSymbol.Wild_1x2, 2) },
                    { new GameHistorySymbol((int)HulaGirlSymbol.Wild, 3), new GameHistorySymbol((int)HulaGirlSymbol.Wild_1x3, 3) },
                    { new GameHistorySymbol((int)HulaGirlSymbol.ScatterA, 1, 0), new GameHistorySymbol((int)HulaGirlSymbol.ScatterABC, 3) },
                    { new GameHistorySymbol((int)HulaGirlSymbol.ScatterA, 1, 1), new GameHistorySymbol((int)HulaGirlSymbol.ScatterAB, 2) },
                    { new GameHistorySymbol((int)HulaGirlSymbol.ScatterB, 1, 0), new GameHistorySymbol((int)HulaGirlSymbol.ScatterBCD, 3) },
                    { new GameHistorySymbol((int)HulaGirlSymbol.ScatterC, 1, 0), new GameHistorySymbol((int)HulaGirlSymbol.ScatterCD, 2) }
                }
            },
            {
                (int)GameIdEnum.WuxiaPrincessMegaReels, new Dictionary<GameHistorySymbol, GameHistorySymbol>()
                {
                    { new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady1, 3), new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady1_1x3, 3) },
                    { new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady1, 2), new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady1_1x2, 2) },

                    { new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2, 6), new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2_1x6, 6) },
                    { new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2, 5), new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2_1x5, 5) },
                    { new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2, 4), new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2_1x4, 4) },
                    { new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2, 3), new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2_1x3, 3) },
                    { new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2, 2), new GameHistorySymbol((int)WuxiaPrincessMegaReelsSymbol.Lady2_1x2, 2) }
                }
            },
            {
                (int)GameIdEnum.GeniesLuck, new Dictionary<GameHistorySymbol, GameHistorySymbol>()
                {
                    { new GameHistorySymbol((int)GenieLuckSymbol.Genie, 3), new GameHistorySymbol((int)GenieLuckSymbol.Genie_1x3, 3) }
                }
            }
        };

        [JsonIgnore]
        public readonly Dictionary<GameIdEnum, Func<Dictionary<int, List<PaylineConfig>>, SpinXml, List<byte[,]>>> WinTableByGame;

        [JsonIgnore]
        public readonly Dictionary<int, Dictionary<int, Dictionary<GameHistorySymbol, GameHistorySymbol>>> ReelSetSymbolReplacement = new Dictionary<int, Dictionary<int, Dictionary<GameHistorySymbol, GameHistorySymbol>>>()
        {
            {
                (int)GameIdEnum.KungfuFurry, new Dictionary<int, Dictionary<GameHistorySymbol, GameHistorySymbol>>()
                {
                    {
                        1, new Dictionary<GameHistorySymbol, GameHistorySymbol>()
                        {
                            { new GameHistorySymbol((int)KungfuFurrySymbol.DogOrCat), new GameHistorySymbol((int)KungfuFurrySymbol.Dog) },
                            { new GameHistorySymbol((int)KungfuFurrySymbol.Blank, 3, 0, 3, 0), new GameHistorySymbol((int)KungfuFurrySymbol.DogEnd, 3, 0, 3, 0) }
                        }
                    },
                    {
                        2, new Dictionary<GameHistorySymbol, GameHistorySymbol>()
                        {
                            { new GameHistorySymbol((int)KungfuFurrySymbol.DogOrCat), new GameHistorySymbol((int)KungfuFurrySymbol.Cat) },
                            { new GameHistorySymbol((int)KungfuFurrySymbol.Blank, 3, 0, 3, 0), new GameHistorySymbol((int)KungfuFurrySymbol.CatEnd, 3, 0, 3, 0) }
                        }
                    }
                }
            }
        };

        [JsonIgnore]
        public readonly Dictionary<PaylineType, Func<Dictionary<int, List<PaylineConfig>>, SpinXml, List<byte[,]>>> WinTableByPaylineType;

        private void DisplayFreeSpinHistory(GameHistory history)
        {
            var xmlHelper = new XmlHelper();
            var xml = xmlHelper.Deserialize<BonusXml>(history.HistoryXml);
            BonusXml = xml;

            XElement spinXml = xml.Data.Element("spin") ?? xml.Data.Element("wheels").Element("stage").Element("spin");

            SpinXml = xmlHelper.Deserialize<SpinXml>(spinXml.ToString());
            PayLine = paylineRepository.Get((GameIdEnum)history.GameId).Lines;

            if (xml.Data.Element("spin") != null && xml.Data.Element("spin").Element("bonus") != null && xml.Data.Element("spin").Element("bonus").Attribute("addFSCount") != null)
            {
                AdditionalFreeSpin = (xml.Data.Element("spin").Element("bonus")).Attribute("addFSCount").Value.ToInt();
            }

            var fsReplacementSymbol = new Dictionary<int, int>();
            if (!FreeSpinReplacement.TryGetValue(history.GameId, out fsReplacementSymbol))
                fsReplacementSymbol = new Dictionary<int, int>();

            Wheel = new WheelViewModel
            {
                reels = new List<List<Symbols>>()
            };

            PostWheel = new WheelViewModel
            {
                reels = new List<List<Symbols>>()
            };

            var wheel = SpinXml.Wheel;
            var postWheel = SpinXml.PostWheel;

            CreateWheel(Wheel, wheel, fsReplacementSymbol);
            if (postWheel != null)
            {
                CreateWheel(PostWheel, postWheel, fsReplacementSymbol);
            }

            OriginalWinPosition = SpinXml.WinPositions.Where(w => !(w is WinPositionExpanding));
            ExpandingWinPosition = SpinXml.WinPositions.OfType<WinPositionExpanding>();

            switch (history.GameId)
            {
                case (int)GameIdEnum.ForbiddenChamber:
                case (int)GameIdEnum.WorldOfWarlords:
                    BonusXml = xmlHelper.Deserialize<BonusXml>(history.HistoryXml);

                    foreach (FeatureElement item in BonusXml.Feature)
                        Wheel.reels[item.Reel - 1][item.Row - 1].symbol += 13;

                    break;

                case (int)GameIdEnum.Genisys:
                    if (BonusXml.Data.Element("mode") != null && Convert.ToInt32(BonusXml.Data.Element("mode").Value) != (int)GenisysMode.Future) break;
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)GenisysSymbol.Wild))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)GenisysSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.DesertOasis:
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)DesertOasisSymbol.Camel))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)DesertOasisSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.GoldenWheel:
                    if (BonusXml.Type == "fs")
                    {
                        GameResultType = GameResultType.RespinResult;
                    }
                    break;

                case (int)GameIdEnum.SevenWonders:
                case (int)GameIdEnum.FloraSecret:
                    if (BonusXml.Type == "fs")
                    {
                        GameResultType = GameResultType.RespinResult;
                    }
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)SevenWondersSymbol.Wild))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)SevenWondersSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.FortuneKoi:
                    if (BonusXml.Type == "fs")
                    {
                        GameResultType = GameResultType.RespinResult;
                    }
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)SevenWondersSymbol.Wild))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)SevenWondersSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.AssaultOnTitan:
                    if (BonusXml.Data.Element("mode") != null && Convert.ToInt32(BonusXml.Data.Element("mode").Value) != 3) break;
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)AssaultOnTitanSymbol.Wild))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)AssaultOnTitanSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.FuLuShou:
                    if (BonusXml.Type == "fs")
                    {
                        GameResultType = GameResultType.RespinResult;
                    }
                    new FuLuShouExpandingWild().Expanding(Wheel);
                    break;
                case (int)GameIdEnum.FortuneDice:
                    EditSymbolWithOverprintSymbols();
                    break;
                case (int)GameIdEnum.FourBeauties:
                case (int)GameIdEnum.ThreeKingdoms:
                case (int)GameIdEnum.Soccer:
                    if (BonusXml.Data.Element("mode") != null && Convert.ToInt32(BonusXml.Data.Element("mode").Value) != 2) break;
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)FourBeautiesSymbol.Wild))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)FourBeautiesSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.LanternFestival:
                case (int)GameIdEnum.LanternFestivalPro:
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)LanternFestivalProSymbol.Scatter))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)LanternFestivalProSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.BikiniBeach:
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)BikiniBeachSymbol.Hammock))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)BikiniBeachSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.Baseball:
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)BaseballSymbol.Scoreboard))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)BaseballSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.MonkeyKing:
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)MonkeyKingSymbol.Monk))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)MonkeyKingSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.TrickOrTreat:
                    if (BonusXml.Data.Element("mode") != null && Convert.ToInt32(BonusXml.Data.Element("mode").Value) != 2) break;
                    for (int i = 0; i < Wheel.reels.Count; i++)
                    {
                        if (Wheel.reels[i].Any(s => s.symbol == (int)TrickOrTreatSymbol.Wild))
                        {
                            Wheel.reels[i] = new List<Symbols>()
                            {
                                new Symbols{ symbol = (int)TrickOrTreatSymbol.RExpandingWild, height = 3 },
                                new Symbols{ symbol = -1, height = 1 },
                                new Symbols{ symbol = -1, height = 1 }
                            };
                        }
                    }
                    break;

                case (int)GameIdEnum.WuxiaPrincessMegaReels:
                    {
                        UpdateWheelViewModelSymbols(SpinXml.Wheel.Width, SpinXml.Wheel.Height, SpinXml.Wheel.Rows, Wheel, SymbolReplacement[history.GameId]);
                        break;
                    }

                case (int)GameIdEnum.HulaGirl:
                    {
                        UpdateWheelViewModelSymbols(SpinXml.Wheel.Width, SpinXml.Wheel.Height, Wheel, SymbolReplacement[history.GameId]);
                        break;
                    }
                case (int)GameIdEnum.GeniesLuck:
                    {
                        var node = BonusXml.Data.Descendants().FirstOrDefault(n => n.Name == "mode");

                        if (node?.Value == "2")
                        {
                            GameResultType = GameResultType.RespinResult;
                        }

                        UpdateWheelViewModelSymbols(SpinXml.Wheel.Width, SpinXml.Wheel.Height, SpinXml.Wheel.Rows, Wheel, SymbolReplacement[history.GameId], history.GameId);
                        break;
                    }
            }

            WinTable = GetWinTable((GameIdEnum)history.GameId, PayLine, SpinXml);
            WinTable = GetWinTable((GameIdEnum)history.GameId, PayLine, SpinXml);

            CreateReelSetsVM(history);
        }

        private void CreateWheel(WheelViewModel wheelViewModel, Wheel wheel, Dictionary<int, int> fsReplacementSymbol)
        {
            for (var c = 0; c < wheel.Reels.Count; c++)
            {
                wheelViewModel.reels.Add(new List<Symbols>());

                for (var r = 0; r < wheel.Reels[c].Count; r++)
                {
                    var sym = wheel.Reels[c][r];
                    var symbols = new Symbols { symbol = (fsReplacementSymbol.ContainsKey(sym) ? fsReplacementSymbol[sym] : sym), height = 1, width = 1 };
                    wheelViewModel.reels[c].Add(symbols);
                }

                if (!(wheel.Rows?.Any() ?? false))
                    continue;

                for (var r = wheel.Rows[c]; r < wheel.Height; r++)
                {
                    var symbols = new Symbols(true) { symbol = -1, height = 1 };
                    wheelViewModel.reels[c].Add(symbols);
                }
            }
        }

        private void UpdateWheelViewModelSymbols(int wheelWidth, int wheelHeight, List<int> rows, WheelViewModel wheel, Dictionary<GameHistorySymbol, GameHistorySymbol> symbolReplacement)
        {
            for (var reel = 0; reel < wheelWidth; reel++)
            {
                var reelHeight = (rows?.Any() ?? false) ? rows[reel] : wheelHeight;
                var symbols = wheel.reels[reel].Select(s => s.symbol).ToList();
                var orderedReplacementList = symbolReplacement.OrderBy(s => s.Key.Height);
                foreach (var sr in orderedReplacementList)
                {
                    var findSym = sr.Key;
                    var replSym = sr.Value;

                    for (var row = (findSym.RowPosition > -1 ? findSym.RowPosition : 0); row < (findSym.RowPosition > -1 ? findSym.RowPosition + 1 : reelHeight); row++)
                    {
                        if (findSym.Symbol != symbols[row])
                            continue;

                        if (findSym.Height > (reelHeight - row))
                            continue;

                        if (findSym.Height > 1 && !symbols.GetRange(row, findSym.Height).TrueForAll(s => s == findSym.Symbol))
                            continue;

                        if (replSym.Height > (reelHeight - row))
                            continue;

                        wheel.reels[reel][row] = new Symbols() { symbol = replSym.Symbol, height = replSym.Height };

                        if (replSym.Height == 1)
                            continue;

                        for (var i = (row + 1); i < (row + replSym.Height); i++)
                        {
                            wheel.reels[reel][i] = new Symbols() { symbol = -1, height = 0 };
                        }

                        row += (replSym.Height - 1);
                    }
                }
            }
        }

        private void UpdateWheelViewModelSymbols(int wheelWidth, int wheelHeight, WheelViewModel wheel, Dictionary<GameHistorySymbol, GameHistorySymbol> symbolReplacement)
        {
            for (var reel = 0; reel < wheelWidth; reel++)
            {
                var symbols = wheel.reels[reel].Select(s => s.symbol).ToList();

                foreach (var sr in symbolReplacement)
                {
                    var findSym = sr.Key;
                    var replSym = sr.Value;

                    for (var row = (findSym.RowPosition > -1 ? findSym.RowPosition : 0); row < (findSym.RowPosition > -1 ? findSym.RowPosition + 1 : wheelHeight); row++)
                    {
                        if (findSym.Symbol != symbols[row])
                            continue;

                        if (findSym.Height > (wheelHeight - row))
                            continue;

                        if (findSym.Height > 1 && !symbols.GetRange(row, findSym.Height).TrueForAll(s => s == findSym.Symbol))
                            continue;

                        if (replSym.Height > (wheelHeight - row))
                            continue;

                        wheel.reels[reel][row] = new Symbols() { symbol = replSym.Symbol, height = replSym.Height };

                        if (replSym.Height == 1)
                            continue;

                        for (var i = (row + 1); i < (row + replSym.Height); i++)
                        {
                            wheel.reels[reel][i] = new Symbols() { symbol = -1, height = 0 };
                        }

                        row += (replSym.Height - 1);
                    }
                }
            }
        }

        private void UpdateWheelViewModelSymbols(int wheelWidth, int wheelHeight, List<int> rows, WheelViewModel wheel, Dictionary<GameHistorySymbol, GameHistorySymbol> symbolReplacement, int gameId = 0)
        {
            for (var reel = 0; reel < wheelWidth; reel++)
            {
                if (gameId == (int)GameIdEnum.GeniesLuck)
                {
                    if (reel != 0 && reel != (wheelWidth - 1))
                        continue;
                }

                var reelHeight = (rows?.Any() ?? false) ? rows[reel] : wheelHeight;
                var symbols = wheel.reels[reel].Select(s => s.symbol).ToList();

                foreach (var sr in symbolReplacement)
                {
                    var findSym = sr.Key;
                    var replSym = sr.Value;

                    for (var row = (findSym.RowPosition > -1 ? findSym.RowPosition : 0); row < (findSym.RowPosition > -1 ? findSym.RowPosition + 1 : reelHeight); row++)
                    {
                        if (findSym.Symbol != symbols[row])
                            continue;

                        if (findSym.Height > (reelHeight - row))
                            continue;

                        if (findSym.Height > 1 && !symbols.GetRange(row, findSym.Height).TrueForAll(s => s == findSym.Symbol))
                            continue;

                        if (replSym.Height > (reelHeight - row))
                            continue;

                        wheel.reels[reel][row] = new Symbols() { symbol = replSym.Symbol, height = replSym.Height };

                        if (replSym.Height == 1)
                            continue;

                        for (var i = (row + 1); i < (row + replSym.Height); i++)
                        {
                            wheel.reels[reel][i] = new Symbols() { symbol = -1, height = 0 };
                        }

                        row += (replSym.Height - 1);
                    }
                }
            }
        }

        private List<byte[,]> GetWinTable(GameIdEnum gameId, Dictionary<int, List<PaylineConfig>> payline, SpinXml xml)
        {
            return WinTableByGame.ContainsKey(gameId)
                ? WinTableByGame[gameId](payline, xml)
                : WinTableByPaylineType[gameInfoRepository.GetPayLineType(gameId)](payline, xml);
        }

        private void CreateReelSetsVM(GameHistory history)
        {
            if (SpinXml.Wheel.ReelSets == null)
                return;

            ReelSets = new Dictionary<int, ReelSetViewModel>();

            foreach (var reelSet in SpinXml.Wheel.ReelSets)
            {
                var reelSetVM = new ReelSetViewModel();

                for (var c = 0; c < reelSet.Width; c++)
                {
                    reelSetVM.Reels.Add(new List<Symbols>());

                    for (var r = 0; r < reelSet.Height; r++)
                    {
                        var symbol = new Symbols { symbol = reelSet.Reels[c][r], height = 1, width = 1 };
                        reelSetVM.Reels[c].Add(symbol);
                    }
                }

                UpdateWheelViewModelSymbols(reelSet.Width, reelSet.Height, new List<int>(), new WheelViewModel() { reels = reelSetVM.Reels }, ReelSetSymbolReplacement[history.GameId][reelSet.Id]);

                reelSetVM.WinTable = gamePayoutEngine.PayoutMultiWheelIndependent(PayLine, reelSet);

                ReelSets.Add(reelSet.Id, reelSetVM);
            }
        }
    }
}

using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model;
using System.Collections.Generic;
using System.Linq;
using static Slot.Core.Modules.Infrastructure.ConfigurationCreator;

namespace Slot.Games.BullRush
{
    public static class BullRushConfiguration
    {
        public const int GameId = 300;
        public const string GameName = "bullrush";
        public const int Width = 4;
        public const int WidthOfRace = 3;
        public const int HeightOfRacing = 105;
        public const int HeightBonusRacing = 50;
        public const int MagnetActiveRows = 11;
        public const int VacuumActiveRows = 5;
        public const int ShieldActiveRows = 25;
        public const int Lines = 5;
        public const int VEHICLES = 5;
        public const int Multiplier = 1;
        public const int LevelOne = 1;
        public const int LevelTwo = 2;
        public const int LevelThree = 3;

        public static List<int> Rows = new List<int>() { 105, 105, 105 };
        public static List<int> OuterWheel = new List<int>() { RACE, SURPRISE, LOSE, VACUUM, RACE, MAGNET, LOSE, SHIELD };
        public static List<int> InnerWheel = new List<int>() { BONUS, TREASURE3, TREASURE4, TREASURE3, TREASURE5, TREASURE3, TREASURE4, TREASURE3 };

        public const double VariantTwoWeight = 0.5;
        public const double VariantThreeWeight = 1.0000;

        public const int BLANK = 0;
        public const int WILD = 6;
        public const int WHEEL = 5;
        public const int BAR3 = 4;
        public const int BAR2 = 3;
        public const int BAR1 = 2;
        public const int STAR = 1;

        public const int RACE = 4;
        public const int MAGNET = -3;
        public const int SHIELD = -2;
        public const int VACUUM = -4;
        public const int SURPRISE = 0;
        public const int LOSE = -1;

        public const int BONUS = 0;
        public const int TREASURE5 = 5;
        public const int TREASURE4 = 4;
        public const int TREASURE3 = 3;

        public const int COIN = 6;
        public const int OBSTACLE = -5;
        public const int TREASURE = 7;

        public const int COIN1 = 1;
        public const int COIN2 = 2;
        public const int BUNDLE = 3;

        public const int Credit = 100;

        private static Dictionary<int, Wheel> variantWheelsOne;
        public static Dictionary<int, Wheel> VariantWheelsOne { get => variantWheelsOne; }

        private static Dictionary<int, Wheel> variantWheelsTwo;
        public static Dictionary<int, Wheel> VariantWheelsTwo { get => variantWheelsTwo; }

        private static Dictionary<int, Wheel> variantWheelsThree;
        public static Dictionary<int, Wheel> VariantWheelsThree { get => variantWheelsThree; }

        private static Dictionary<int, Wheel> variantWheelsFour;
        public static Dictionary<int, Wheel> VariantWheelsFour { get => variantWheelsFour; }

        private static Dictionary<int, Wheel> variantWheelsFive;
        public static Dictionary<int, Wheel> VariantWheelsFive { get => variantWheelsFive; }


        private static Payline payline;
        public static Payline Payline { get => payline; }

        public static PayTable PayTables = PayTables(
            Pay(BAR3, Odds(0, 0, 50)),
            Pay(BAR2, Odds(0, 0, 25)),
            Pay(BAR1, Odds(0, 0, 15)),
            Pay(STAR, Odds(0, 0, 40))
        );

        public static readonly Dictionary<double, int> OuterWheelWeight = new Dictionary<double, int>()
        {
             {0.1035, RACE }
            ,{0.2735, MAGNET}
            ,{0.3235, LOSE}
            ,{0.4935, SHIELD}
            ,{0.597, RACE}
            ,{0.78, SURPRISE}
            ,{0.95, VACUUM}
            ,{1, LOSE }
        };

        public static readonly Dictionary<double, int> InnerWheelWeight = new Dictionary<double, int>()
        {
             {0.2,  TREASURE3 }
            ,{0.28, TREASURE4}
            ,{0.48, TREASURE3}
            ,{0.51, TREASURE5}
            ,{0.71, TREASURE3}
            ,{0.79, TREASURE4}
            ,{0.99, TREASURE3}
            ,{1,    BONUS }
        };

        public static readonly Dictionary<double, int> BonusRace = new Dictionary<double, int>()
        {
             {0.578, 500}
            ,{0.81133, 600}
            ,{0.91133, 700}
            ,{0.96133, 800}
            ,{0.98133, 1000}
            ,{0.99133, 1200}
            ,{0.99633, 1500}
            ,{0.99867, 2000}
            ,{0.99967, 5000}
            ,{1   , 10000 }
        };

        public static readonly List<int> ListOfPowerUps = new List<int>() { MAGNET, SHIELD, VACUUM };

        public static readonly Dictionary<double, int> TreasureDrawing = new Dictionary<double, int>()
        {
             {0.6012, 25 }
            ,{0.8212, 50}
            ,{0.9212, 100}
            ,{0.9612, 200}
            ,{0.9812, 500}
            ,{0.9912, 1000}
            ,{0.9962, 1500}
            ,{0.9984, 2000}
            ,{0.9994, 5000}
            ,{0.9998, 10000}
            ,{1, 100000 }
        };

        public static readonly Dictionary<double, int> SupriseWeight = new Dictionary<double, int>()
        {
             {0.6, BUNDLE }
            ,{0.8, COIN1}
            ,{1, COIN2 }
        };

        
        public static readonly ReelStrips VariantOneReelStrip = ReelStrips(
            ReelStrip(LevelOne,
                Reel(1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 2, 2, 2, 0, 0, 0, 2, 2, 2, 0, 0, 2, 2, 2, 0, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 2, 0, 3, 3, 3, 0, 0, 0, 3, 3, 3, 0, 0, 3, 3, 3, 0, 3, 3, 3, 3, 0, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3),
                Reel(0, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0, 2, 2, 0, 0, 0, 2, 2, 2, 0, 0, 0, 2, 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)));

        public static readonly ReelStrips VariantTwoReelStrip = ReelStrips(
            ReelStrip(LevelOne,
                Reel(1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 2, 2, 2, 2, 2, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 2, 2, 2, 0, 0, 2, 2, 2, 0, 0, 2, 2, 2, 0, 2, 2, 2, 2, 2, 0, 0, 0, 3, 3, 3, 0, 0, 3, 3, 3, 3, 0, 0, 3, 3, 3, 0, 0, 2, 2, 2, 2, 2, 0),
                Reel(0, 0, 0, 2, 2, 0, 0, 0, 2, 0, 0, 0, 2, 1, 0, 0, 0, 1, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0, 0, 0, 2, 2, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 2, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)));

        public static readonly ReelStrips VariantThreeReelStrip = ReelStrips(
            ReelStrip(LevelOne,
                Reel(1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 3, 3, 3, 3, 3, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 3, 3, 3, 3, 3, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 2, 2, 2, 0, 3, 3, 3, 3, 3, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 0, 0, 0, 3, 3, 3, 3, 3, 0),
                Reel(0, 0, 0, 2, 2, 0, 0, 0, 2, 0, 0, 0, 2, 1, 0, 0, 0, 1, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0, 0, 0, 2, 2, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 2, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)));

        public static readonly ReelStrips VariantFourReelStrip = ReelStrips(
            ReelStrip(LevelOne,
                Reel(0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 3, 3, 3, 3, 0, 0, 1, 2, 2, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 2, 3, 3, 3, 1, 0, 2, 2, 2, 0, 0, 0, 1, 1, 1, 0, 0, 0, 2, 2, 0, 0, 2, 2, 0, 3, 2, 2, 2, 0, 2, 2, 2, 2, 2, 0, 0, 3, 3, 3, 0, 0, 3, 3, 3, 0, 0, 1, 1, 0, 0, 1, 2, 2, 0, 0, 3, 3, 3, 3, 0, 0, 0, 3, 3, 0),
                Reel(0, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0, 2, 2, 0, 0, 0, 0, 2, 2, 0, 0, 0, 2, 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)));

        public static readonly ReelStrips VariantFiveReelStrip = ReelStrips(
            ReelStrip(LevelOne,
                Reel(0, 1, 1, 1, 0, 0, 0, 3, 3, 3, 0, 0, 0, 1, 0, 2, 2, 2, 2, 2, 0, 0, 3, 0, 0, 2, 0, 0, 1, 0, 0, 1, 1, 0, 0, 2, 2, 2, 2, 2, 0, 1, 1, 0, 2, 3, 2, 0, 0, 0, 1, 1, 1, 0, 0, 2, 2, 2, 2, 2, 0, 3, 3, 0, 1, 2, 1, 0, 0, 0, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 0, 0, 3, 0, 1, 1, 0, 3, 3, 3, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 1, 1, 1, 1, 1),
                Reel(0, 0, 0, 0, 1, 2, 1, 0, 0, 0, 2, 2, 2, 0, 1, 0, 0, 0, 0, 0, 1, 2, 0, 1, 1, 0, 2, 2, 0, 1, 2, 0, 0, 2, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 1, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 2, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                Reel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)));


        static BullRushConfiguration()
        {
            variantWheelsOne = new Dictionary<int, Wheel>();
            variantWheelsTwo = new Dictionary<int, Wheel>();
            variantWheelsThree = new Dictionary<int, Wheel>();
            variantWheelsFour = new Dictionary<int, Wheel>();
            variantWheelsFive = new Dictionary<int, Wheel>();

            InitWheelsVariantOne(LevelOne);
            InitWheelsVariantTwo(LevelOne);
            InitWheelsVariantThree(LevelOne);
            InitWheelsVariantFour(LevelOne);
            InitWheelsVariantFive(LevelOne);
        }

        public static void InitWheelsVariantOne(int level)
        {
            var wheel = new Wheel(Width, HeightOfRacing)
            {
                Type = WheelType.Normal,
            };

            var reelStrips = VariantOneReelStrip[level];

            for (var reel = 0; reel < Width; reel++)
            {
                var reelStrip = reelStrips[reel];
                wheel[reel] = reelStrip.ToList();
            }

            variantWheelsOne.Add(level, wheel);
        }

        public static void InitWheelsVariantTwo(int level)
        {
            var wheel = new Wheel(Width, HeightOfRacing)
            {
                Type = WheelType.Normal,
            };

            var reelStrips = VariantTwoReelStrip[level];

            for (var reel = 0; reel < Width; reel++)
            {
                var reelStrip = reelStrips[reel];
                wheel[reel] = reelStrip.ToList();
            }

            variantWheelsTwo.Add(level, wheel);
        }

        public static void InitWheelsVariantThree(int level)
        {
            var wheel = new Wheel(Width, HeightOfRacing)
            {
                Type = WheelType.Normal,
            };

            var reelStrips = VariantThreeReelStrip[level];

            for (var reel = 0; reel < Width; reel++)
            {
                var reelStrip = reelStrips[reel];
                wheel[reel] = reelStrip.ToList();
            }

            variantWheelsThree.Add(level, wheel);
        }

        public static void InitWheelsVariantFour(int level)
        {
            var wheel = new Wheel(Width, HeightOfRacing)
            {
                Type = WheelType.Normal,
            };

            var reelStrips = VariantFourReelStrip[level];

            for (var reel = 0; reel < Width; reel++)
            {
                var reelStrip = reelStrips[reel];
                wheel[reel] = reelStrip.ToList();
            }

            variantWheelsFour.Add(level, wheel);
        }

        public static void InitWheelsVariantFive(int level)
        {
            var wheel = new Wheel(Width, HeightOfRacing)
            {
                Type = WheelType.Normal,
            };

            var reelStrips = VariantFiveReelStrip[level];

            for (var reel = 0; reel < Width; reel++)
            {
                var reelStrip = reelStrips[reel];
                wheel[reel] = reelStrip.ToList();
            }

            variantWheelsFive.Add(level, wheel);
        }
    }
}

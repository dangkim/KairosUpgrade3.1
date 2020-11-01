using System.ComponentModel.DataAnnotations;

namespace Slot.BackOffice.Data
{
    public static class Enums
    {
        public enum ReportFormat
        {
            None = 0,
            WinLose = 1,
            Currency = 2,
            Platform = 3,
            Merchant = 4
        }

        public enum FilterDateType
        {
            None = 0,
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
            Currency = 4,
            Game = 5,
            [Display(Name = "Show All")] All = 6
        }

        public enum TopWinnerFormat
        {
            None = 0,
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
            Game = 4
        }

        public enum WinLosePlatformFormat
        {
            Currency = 0,
            Game = 1
        }

        public enum GameId
        {
            None = 0,
            UnderwaterWorld = 1,
            MonkeyKing = 2,
            BikiniBeach = 3,
            Baseball = 4,
            LittleMonsters = 5,
            Fruitilicious = 6,
            LegendOfNezha = 7,
            LanternFestival = 8,
            DesertOasis = 9,
            DeepBlue = 10,
            Zeus = 11,
            Mafia = 12,
            ForbiddenChamber = 13,
            GodOfFortune = 14,
            GoldenEggs = 15,
            Boxing = 16,
            WorldOfWarlords = 17,
            Pharaoh = 18,
            CasinoRoyale = 19,
            RomanEmpire = 20,
            SamuraiSushi = 21,
            Qixi = 22,
            FortuneCat = 23,
            QueenBee = 24,
            Dimsumlicious = 25,
            Genisys = 26,
            PiratesTreasure = 27,
            GodOfGamblers = 28,
            AssaultOnTitan = 29,
            SevenWonders = 30,
            TokyoHunter = 31,
            FortuneKoi = 32,
            GoldenWheel = 33,
            FortuneTree = 34,
            SevenBrothers = 35,
            LadyFortune = 36,
            UnderwaterWorldPro = 37,
            BaseballPro = 38,
            RomanEmpirePro = 39,
            BoxingPro = 40,
            CasinoRoyalePro = 41,
            LanternFestivalPro = 42,
            ThreeKingdoms = 43,
            Klassik = 44,
            Panda = 45,
            FourGuardians = 46,
            Phoenix = 47,
            Soccer = 48,
            FourBeauties = 49,
            Candylicious = 50,
            Cleopatra = 51,
            FortuneDice = 53,
            FuLuShou = 55,
            RedChamber = 56,
            Nutcracker = 58,
            FloraSecret = 59,
            GolfTour = 60,
            KingsOfHighway = 61,
            GodOfFortuneGenerosity = 62,
            LuckyBomber = 63,
            WildsAndTheBeanstalk = 64,
            SweetTreats = 65,
            LuckyRoyale = 67,
            MonstersCash = 68,
            SkyStrikers = 69,
            TrickOrTreat = 70,
            LionDance = 71,
            BlossomGarden = 72,
            WinterWonderland = 73,
            HulaGirl = 74,
            Zodiac = 75,
            WuxiaPrincessMegaReels = 76,
            PhantomThief = 77,
            KungfuFurry = 79,
            GeniesLuck = 80,
            WorldSoccerSlot2 = 81,
            FrostDragon = 83,
            JewelLand = 84,
            MoneyMonkey = 85,
            GemForest = 86,
            AlchemistsSpell = 87,
            MoonRabbit = 88,
            ChessRoyale = 89,
            VikingsMegaReels = 90,
            JazzItUp = 91,
            SpaceNeon = 92,
            WolvesSlot = 93,
            FountainOfFortune = 94,
            FaFaZhu = 95,
            ThreeBeauties = 96,
            MagicPaper = 97,
            NuwaAndTheFiveElements = 98,
            LuckyGiant = 99,
            PandaWarrior = 100,
            DinoAge = 101,
            StripNRoll = 102,
            CaptainRabbit = 103,
            XuanWuBlessing = 105,
            FortuneHongBao = 107
        }
    }
}

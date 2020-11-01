using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Slot.Model
{
    #region Commmon

    public enum ErrorCode
    {
        [XmlEnum("0")] None = 0,
        [XmlEnum("1")] IncorrectLineNumber = 1,
        [XmlEnum("2")] IncorrectBet = 2,
        [XmlEnum("3")] IncorrectMultiplicand = 3,
        [XmlEnum("4")] NonexistenceBonus = 4,
        [XmlEnum("5")] WrongParameter = 5,
        [XmlEnum("6")] WrongBonusStep = 6,
        [XmlEnum("7")] SessionExpired = 7,
        [XmlEnum("8")] MissingRoom = 8,
        [XmlEnum("9")] MissingGroup = 9,
        [XmlEnum("10")] InsufficientCredit = 10,
        [XmlEnum("11")] SpinError = 11,
        [XmlEnum("12")] MissingGameSetting = 12,
        [XmlEnum("13")] NonexistenceHand = 13,
        [XmlEnum("14")] WrongStep = 14,
        [XmlEnum("15")] IncorrectRound = 15,
        [XmlEnum("16")] IncorrectState = 16,
        [XmlEnum("17")] InternalError = 17,
        [XmlEnum("18")] LoginExist = 18,
        [XmlEnum("19")] NonexistenceUserData = 19,
        [XmlEnum("20")] GameMaintenance = 20,
        [XmlEnum("21")] SystemMaintenance = 21,
        [XmlEnum("24")] InvalidTimeStamp = 24,
        [XmlEnum("25")] InvalidGame = 25,
        [XmlEnum("26")] RequestOnProcess = 26,
        [XmlEnum("27")] AccessDenied = 27,
        [XmlEnum("28")] InvalidCheckSum = 28,
        [XmlEnum("29")] NonExistenceFreeRound = 29,
        [XmlEnum("30")] NonActivationFreeRound = 30,
        [XmlEnum("31")] NotImplemented = 31
    }

    public enum UserRole
    {
        Player = 0,
    }

    public enum PlatformType
    {
        None = 0,
        Web = 1,
        Desktop = 2,
        Mobile = 3,
        Mini = 4,
        WebLD = 11,
        All = 100
    }

    public enum PlatformGroup
    {
        All = 0,
        Web = 1,
        WebSD = 2,
        WebLD = 3,
        Mobile = 4,
        Mini = 6,
        Download = 5
    }

    public enum CounterType
    {
        RoundId = 1,
    }

    #endregion Commmon

    #region Game

    public enum GameType
    {
        Slot = 1
    }

    public enum GameStateType
    {
        SlotStateNormal = 1,      // Normal state
        SlotStateBonus = 2      // Bonus state
    }

    public enum GameTransactionType
    {
        None = 0,
        Spin = 1,
        Gamble = 2,
        FreeSpin = 3,
        Reveal = 4,
        DoubleUp = 5,
        FreeGame = 6,
        CollapsingSpin = 7,
        CollapsingFreeSpin = 8,
        CollapsingReSpin = 10,
        ReSpin = 11
    }

    public enum GameResultType
    {
        None = 0,
        SpinResult = 1,
        GambleResult = 2,
        FreeSpinResult = 3,
        BonusFreeSpinResult = 4,
        RevealResult = 5,
        DoubleUpResult = 6,
        InstantWinResult = 7,
        MultiModeResult = 8,
        CollapsingSpinResult = 9,
        FreeSpinCollapsingSpinResult = 10,
        RespinResult = 11
    }

    #endregion Game

    #region In game enumerations

    public enum WheelType
    {
        Normal = 0,
        Independent = 1,
        StackWild = 2,
        JackpotCollapse = 3,
        Collapse = 4,
        Rolling = 5,
        Guaranteed = 6
    }

    public enum PaylineType
    {
        ThreeByThree,
        ThreeByFive,
        ThreeByThreeOutsource,
        FourByFive,
        ThreeByFiveCollapsing,
        ThreeByFive2,
        ThreeByThree27Ways,
        ThreeFourFiveSixSevenSevenReels,
        ThreeByThreeMatchingReels,
        SevenByFive = 10
    }

    public enum RevealType
    {
        None = 0,
        Choose1 = 1,
        Choose3 = 2,
        Dynamic = 3,
        MultipleStage = 4,
    }

    public enum BonusType
    {
        None = 0,
        Gamble = 1,
        DoubleUp = 1,
        FreeSpin = 2,
        RevealOne = 3,
        RevealThree = 4,
        BonusFreeSpin = 3,
        RevealMultiStages = 3,
        InstantWin = 5,
        SideBet = 5,
        MultiMode = 2,
        TrvFreeSpin = 2,
        WheelOfFortune = 3,
        MultiModeReveal = 3,
        SpinBonus = 6,
        CollapsingFreeSpin = 7
    }

    public enum BonusFreeSpinItemType
    {
        None = 0,
        Multiplier = 2,
        FreeSpin = 3
    }

    public enum WheelEncoding
    {
        Local,
        Outsource
    }

    #endregion In game enumerations

    #region Authentication

    public enum AuthProviderId
    {
        Unspecified = 0,
        MemberSite = 1,
        Credit = 2,
        Merchant = 3,
        QuickFire = 4,
        Proxy = 5,
        MerchantPost = 6,
    }

    #endregion Authentication

    #region Wallet

    public enum WalletTransactionType
    {
        Deduct = 1,
        Add = 2
    }

    public enum WalletApiId
    {
        None = 0,
        FunPlay = 1,
        Casino = 2,
        Credit = 3,
        QuickFire = 4,
        Kiosk = 5,
    }

    public enum WalletLogType
    {
        NoResponse = 1,
        Error = 2
    }

    public enum WalletLogStatus
    {
        DebitFailed = 1,
        DebitSucceed = 2,
        DebitHistory = 3,
        CreditFailed = 4,
        CreditSucceed = 5,
        CreditHistory = 6,
    }

    #endregion Wallet

    #region XML

    public enum XmlType
    {
        None = 0,
        SpinXml = 1,
        BonusXml = 2,
        ErrorXml = 3,
        BetXml = 4,
        AuthenticateXml = 5,
        GameHistoryXml = 6,
        ReportXml = 7,
        ReelsInfoXml = 8,
        CollapseXml = 9,
        JackpotCollapseXml = 10
    }

    [Flags]
    public enum ResponseXmlFormat
    {
        None = 0,
        Legacy = 1,
        History = 2
    }

    #endregion XML

    #region Report

    public enum ReportFormat
    {
        None = 0,
        WinLose = 1,
        Currency = 2,
        Platform = 3,
        Merchant = 4
    }

    public enum GenisysMode
    {
        Present = 1,
        Future = 2
    }

    public enum HistoryType
    {
        Gamble = 1,
        DoubleUp = 2,
        BFS = 3,
        Reveal = 4
    }

    public enum PaylinePos
    {
        None = 0,
        NotHit = 1,
        Hit = 2,
        Empty = 3
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

    #endregion Report

    #region Tournament

    public enum TournamentRelationType
    {
        Participant = 1,
        Game = 2,
        MinBet = 3,
        Platform = 4,
        Prize = 5,
        Operator = 6
    }

    public enum TournamentLanguage
    {
        English = 0,
        Chinese = 1,
        Viet = 2,
        Thai = 3,
        Bahasa = 4,
        Khmer = 5,
        Korean = 6,
        Japanese = 7,
        Count = 8
    }

    public enum TournamentCurrency
    {
        USD = 0,
        CNY = 1,
        MYR = 2,
        VND = 3,
        THB = 4,
        IDR = 5,
        KRW = 6,
        JPY = 7,
        Count = 8
    }

    [Flags]
    public enum TournamentFlag
    {
        AllMembers = 1,
    }

    [Flags]
    public enum TournamentLevel
    {
        Time = 1,
        Game = 2,
        Platform = 4,
        MinBet = 8,
        Accumulated = 16
    }

    public enum TournamentStatus
    {
        [Description("Completed")]
        Completed = 1,

        [Description("Ongoing")]
        Ongoing = 2,

        [Description("Upcoming")]
        Upcoming = 3,

        [Description("Cancelled")]
        Cancelled = 4
    }

    #endregion Tournament

    #region Free Round

    public enum FreeRoundLanguage
    {
        English = 0,
        Chinese = 1,
        Viet = 2,
        Thai = 3,
        Bahasa = 4,
        Khmer = 5,
        Korean = 6,
        Japanese = 7,
        Count = 8
    }

    [Flags]
    public enum PlatformFlag
    {
        Web = 1,
        Desktop = 2,
        Mobile = 4,
    }

    public enum FreeRoundStatus
    {
        Upcoming = 1,
        Ongoing = 2,
        Completed = 3,
        Cancelled = 4
    }

    #endregion Free Round

    public enum MultiModeRound
    {
        FreeSpin = 1,
        Reveal = 2,
    }

    public enum SpinDataType
    {
        Spin = 1,
        Bonus = 2,
        Wheel = 3,
        CollapsingSpin = 4,
        CollapsingFreeSpin = 5,
        ContinuitySpin = 6,
        ContinuityGameLevel = 7,
        ReelRespin = 8
    }

    public enum GetTransactionSummaryByCurrencyErrorCode
    {
        InvalidOperator = -6,
        AccessDenied = -5,
        InvalidAccountType = -4,
        EndDateGreaterThanStartDate = -3,
        IncorrectEndDate = -2,
        IncorrectStartDate = -1,
        None = 0
    }

    public enum GetTransactionHistoryErrorCode
    {
        ExceedMaximumPagesize,
        InvalidOperator,
        AccessDenied,
        IncorrectEndDate,
        IncorrectStartDate,
        EndDateGreaterThanStartDate,
        TimeRangeTooBig,
        RequestTooOften,
        None
    }

    public enum MegaWildRevealItemType
    {
        FreeSpin = 1,
        ExtraWild = 2,
        WildMultiplier = 3,
        DefaultWildMultiplier = 4
    }

    public enum TournamentErrorCode
    {
        None = 0,
        InvalidOperator = -1,
        AccessDenied = -2,
        InvalidTournament = -3,
        InvalidMember = -4,
        InvalidDetailType = -5,
        InvalidStatus = -6,
        InvalidTournamentOperator = -7,
        IneligibleMember = -8
    }

    public enum ErrorSource
    {
        Unknown = 0,
        Wallet = 1
    }

    public enum InstantWinType
    {
        Unknown = 0,
        Spin = 1,
        FreeSpin = 2
    }

    public enum JackpotCollapseState
    {
        Collapsing = 1,
        Executing = 2
    }

    public enum FreeSpinCurrentMode
    {
        FreeSpinMode = 1,
        RespinMode = 2
    }
}
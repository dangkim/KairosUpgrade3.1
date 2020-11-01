namespace Slot.Core.Data.Views.Tournament
{
    public class GlobalTournamentLeaderboard : GlobalTournamentLeaderboardBase
    {
        public GlobalTournamentLeaderboard(GlobalTournamentLeaderboardBase globalTournamentLeaderboardBase)
        {
            Operator = globalTournamentLeaderboardBase.Operator;
            UserId = globalTournamentLeaderboardBase.UserId;
            CurrencyId = globalTournamentLeaderboardBase.CurrencyId;
            Rank = globalTournamentLeaderboardBase.Rank;
            Points = globalTournamentLeaderboardBase.Points;
            MemberName = globalTournamentLeaderboardBase.MemberName;
            Bet = globalTournamentLeaderboardBase.Bet;
            Win = globalTournamentLeaderboardBase.Win;
            BetL = globalTournamentLeaderboardBase.BetL;
            WinLoseL = globalTournamentLeaderboardBase.WinLoseL;
            FirstBet = globalTournamentLeaderboardBase.FirstBet;
            LastBet = globalTournamentLeaderboardBase.LastBet;
        }

        public string Currency { get; set; }
    }
}

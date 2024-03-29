GO
/****** Object:  StoredProcedure [dbo].[BOGETLEADERBOARDDETAIL]    Script Date: 6/28/2018 4:21:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vincente
-- Create date:	9 May 2017
-- Description:	Get Backoffice tournament leader board detail
-- Path: Marketing > Tournament > Leaderboard
-- Modified: Kaidan Joseph
-- Change DENSE Rank
-- =============================================
ALTER PROCEDURE [dbo].[BOGETLEADERBOARDDETAIL] 
	@TournamentId INT
AS
BEGIN
	WITH CTE
	AS (SELECT UserId,
			TrxCount = SUM(TrxCount),
			BetL = SUM(BetL),
			WinLoseL = SUM(WinL),
			Bet = SUM(Bet),
			Win = SUM(Win),
			FB = MIN(TimeFirstBet),
			LB = MAX(TimeLastBet)
		FROM TOURNAMENTREPORTINFO rtr WITH (NOLOCK)
		WHERE rtr.TournamentId = @TournamentId AND Level = 15
		GROUP BY UserId)
	SELECT UserId = u.Id,
		CurrencyId = c.Id,
		MemberName = u.Name,
		Rank = CAST(DENSE_RANK() OVER(ORDER BY r.TrxCount DESC, r.BetL DESC) AS  INT),
		Points = r.TrxCount,
		BetL = r.BetL,
		WinLoseL = r.WinLoseL,
		Bet = r.Bet,
		Win = r.Win,
		FirstBet = DATEADD(hh, 8, r.FB),
		LastBet = DATEADD(hh, 8, r.LB)
	FROM CTE r
		INNER JOIN [USER] u WITH (NOLOCK) ON (r.UserId = u.Id AND u.[IsDemo] = 0)
		INNER JOIN [CURRENCY] c WITH (NOLOCK) ON u.CurrencyId = c.Id
		INNER JOIN [TOURNAMENT] t WITH (NOLOCK) ON t.Id = @TournamentId
	WHERE r.TrxCount >= t.MinHands;
END;

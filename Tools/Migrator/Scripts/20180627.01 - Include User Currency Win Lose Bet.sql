SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kaidan Joseph
-- Create date:	September 01, 2017
-- Description:	Get Backoffice global tournament leader board detail
-- Path: Marketing > Global Tournament > Leaderboard
-- =============================================
ALTER PROCEDURE [dbo].[BOGETGLOBALLEADERBOARDDETAIL] 
	@TournamentId INT, 
	@IncludeDemo BIT = NULL
AS
BEGIN
	DECLARE @MinBet Table (Id INT IDENTITY(1,1) , CurrencyId INT, MinBet DECIMAL(23,8),UNIQUE NONCLUSTERED(CurrencyId));
	DECLARE @merchants Table (Id INT IDENTITY(1,1) , OperatorId INT ,UNIQUE NONCLUSTERED(OperatorId));
	
	INSERT INTO @merchants
	SELECT tr.RelationId FROM TRelation tr WITH(NOLOCK)
	WHERE 
		tr.TournamentId =  @TournamentId
		AND tr.RelationType = 6

	INSERT INTO @MinBet
	SELECT 
		CurrencyId = tr.RelationId
		,MinBet  = tr.RelationValue
	FROM TRelation tr WITH(NOLOCK) 
	WHERE 
		TournamentId = @TournamentId 
		AND RelationType = 3 -- minbet

	;WITH CTE
		AS (SELECT UserId,
				TrxCount = SUM(TrxCount),
				BetL = SUM(BetL),
				WinLoseL = SUM(WinL),
				Bet = SUM(Bet),
				Win = SUM(Win),
				FB = MIN(TimeFirstBet),
				LB = MAX(TimeLastBet)
			FROM TOURNAMENTREPORTINFO tri WITH (NOLOCK)
			WHERE tri.TournamentId = @TournamentId 
			AND Level = 23
			AND EXISTS(SELECT 1 FROM @merchants mct WHERE mct.[OperatorId] =  tri.OperatorId)
			GROUP BY UserId)
		SELECT UserId = u.Id,
			CurrencyId = c.Id,
			MemberName = u.Name,
			Operator = op.Name,
			Rank = CAST(DENSE_RANK() OVER(ORDER BY CAST(r.Bet/mb.MinBet AS INT) DESC, r.TrxCount  DESC) AS  INT),
			Points = CAST(r.Bet/mb.MinBet AS INT),
			BetL = r.BetL,
			WinLoseL = r.WinLoseL,
			Bet = r.Bet,
			Win = r.Win,
			FirstBet = DATEADD(hh, 8, r.FB),
			LastBet = DATEADD(hh, 8, r.LB)
		FROM CTE r
			INNER JOIN [USER] u WITH (NOLOCK) ON (r.UserId = u.Id AND (@IncludeDemo = 1 OR u.[IsDemo] = 0))
			INNER JOIN Operator op WITH(NOLOCK) ON op.Id = u.OperatorId
			INNER JOIN [CURRENCY] c WITH (NOLOCK) ON u.CurrencyId = c.Id
			INNER JOIN [TOURNAMENT] t WITH (NOLOCK) ON t.Id = @TournamentId
			
			INNER JOIN @MinBet mb ON mb.CurrencyId = u.CurrencyId
		WHERE 
			 CAST(r.Bet/mb.MinBet AS INT) >= t.MinHands
END;

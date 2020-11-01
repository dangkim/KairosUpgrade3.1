SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vincente
-- Create date:	10 May 2017
-- Description:	Get Tournament leader board detail for Integration API specific member
-- Path: Integration > tournamentdetail > type = 1 and member name
-- Modided by Kaidan, Fixed LeaderBoard's rank showing not correct
-- =============================================
ALTER PROCEDURE [dbo].[GETLEADERBOARDMEMBERDETAIL] 
	@TournamentId INT, 
	@MemberName nvarchar(255)
AS
BEGIN
	 	DECLARE @table TABLE(Id INT IDENTITY(1,1), UserId BIGINT,Bet DECIMAL(28,6), Score INT);
 
	INSERT INTO @table 
	SELECT 
		tri.UserId,
		Bet = SUM(tri.BetL),
		Score = CAST(SUM(tri.TrxCount) AS INT)
	FROM dbo.TournamentReportInfo tri WITH (NOLOCK)
	WHERE 
		tri.TournamentId = @TournamentId
		AND tri.Level = 15
	GROUP BY tri.UserId

	;WITH MemberRanks AS (
			SELECT Rank = CAST(DENSE_RANK() OVER(ORDER BY t.Score DESC, t.Bet DESC ) AS INT),
			u.Name,
			t.Score
		FROM @table t
			INNER JOIN dbo.[User] u WITH (NOLOCK) ON (t.UserId = u.Id and u.[IsDemo] = 0)
			INNER JOIN dbo.Tournament t2 WITH (NOLOCK) ON (t2.Id = @TournamentId AND t2.IsDeleted = 0)
		WHERE
			t.Score >= t2.MinHands 
	)
	
	SELECT 
		* FROM MemberRanks mr 
	WHERE 
		mr.Name = @MemberName
END;
GO
-- =============================================
-- Author:		Vincente
-- Create date:	9 May 2017
-- Description:	Get Backoffice tournament report
-- Path: Marketing > Tournament > Report
-- =============================================

ALTER PROCEDURE [dbo].[BOGETTOURNAMENTREPORT] @TournamentId INT,
                                                  @UserId       INT = NULL,
                                                  @CurrencyId   INT = NULL,
                                                  @GameIds      NVARCHAR(128) = NULL
AS
     BEGIN
         WITH ltem
              AS (SELECT UserId,
                         WinLose = SUM(Win),
                         WinLoseL = SUM(WinL)
                  FROM TOURNAMENTREPORTINFO ltr WITH (NOLOCK)
                  WHERE ltr.TournamentId = @TournamentId
                        AND ltr.GameId IN
                  (
                      SELECT *
                      FROM dbo.fnSplitString(@GameIds, ',')
                  )
                  GROUP BY UserId),
              rtem
              AS (SELECT UserId,
                         EligibleTrxCount = SUM(CASE
                                                    WHEN Level = 15
                                                    THEN TrxCount
                                                    ELSE 0
                                                END),
                         TrxCount = SUM(TrxCount),
                         Bet = SUM(Bet),
                         BetL = SUM(BetL),
                         FB = MIN(TimeFirstBet),
                         LB = MAX(TimeLastBet)
                  FROM TOURNAMENTREPORTINFO rtr WITH (NOLOCK)
                  WHERE rtr.TournamentId = @TournamentId
                        AND rtr.GameId IN
                  (
                      SELECT *
                      FROM dbo.fnSplitString(@GameIds, ',')
                  )
                  GROUP BY UserId)
              SELECT UserId = u.Id,
                     MemberName = u.Name,
                     EligibleTrxCount = r.EligibleTrxCount,
                     TrxCount = r.TrxCount,
                     CurrencyId = c.Id,
                     Currency = c.IsoCode,
                     TotalBet = r.Bet,
                     TotalBetL = r.BetL,
                     WinLose = l.WinLose,
                     WinLoseL = l.WinLoseL,
                     FirstBet = DATEADD(hh, 8, r.FB),
                     LastBet = DATEADD(hh, 8, r.LB)
              FROM rtem r
                   INNER JOIN ltem l ON l.UserId = r.UserId 
                   INNER JOIN [USER] u WITH (NOLOCK) ON (r.UserId = u.Id AND u.[IsDemo] = 0)
                   INNER JOIN [CURRENCY] c WITH (NOLOCK) ON u.CurrencyId = c.Id
              WHERE(@CurrencyId IS NULL OR c.Id = @CurrencyId)
                   AND (@UserId IS NULL OR u.Id = @UserId)
              ORDER BY r.EligibleTrxCount DESC;
     END;
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
		FirstBet = DATEADD(hh, 8, r.FB),
		LastBet = DATEADD(hh, 8, r.LB)
	FROM CTE r
		INNER JOIN [USER] u WITH (NOLOCK) ON (r.UserId = u.Id AND u.[IsDemo] = 0)
		INNER JOIN [CURRENCY] c WITH (NOLOCK) ON u.CurrencyId = c.Id
		INNER JOIN [TOURNAMENT] t WITH (NOLOCK) ON t.Id = @TournamentId
	WHERE r.TrxCount >= t.MinHands;
END;
GO
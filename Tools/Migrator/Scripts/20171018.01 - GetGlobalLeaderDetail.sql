SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kaidan Joseph
-- Create date:	September 01, 2017
-- Description:	Get Tournament leader board detail for Integration API
-- Path: Integration > tournamentdetail > type = 1
-- Modify : 
-- 1. Update the script to get 100 players who have been got highest rank
-- =============================================
CREATE PROCEDURE [dbo].[GETGLOBALLEADERBOARDDETAIL] 
	@TournamentId INT,
	@IncludeDemo BIT = NULL
AS
BEGIN
	DECLARE @table TABLE(Id INT IDENTITY(1,1), UserId BIGINT,Bet DECIMAL(28,6), TrxCount INT);
	DECLARE @MinBet Table (Id INT IDENTITY(1,1) , CurrencyId INT, MinBet DECIMAL(23,8),UNIQUE NONCLUSTERED(CurrencyId));
	-------------------------------------------------------------------------------------------------------------------
	/*Get MinBet based on the currency*/
	INSERT INTO @MinBet
	SELECT 
		CurrencyId = tr.RelationId
		,MinBet  = tr.RelationValue
	FROM TRelation tr WITH(NOLOCK) 
	WHERE 
		TournamentId = @TournamentId 
		AND RelationType = 3


	INSERT INTO @table 
	SELECT 
		tri.UserId,
		Bet = SUM(tri.BetL),
		TrxCount = CAST(SUM(tri.TrxCount) AS INT)
	FROM dbo.TournamentReportInfo tri WITH (NOLOCK)
	WHERE 
		tri.TournamentId = @TournamentId
		AND tri.[Level] = 23
	GROUP BY tri.UserId

	SELECT TOP 100 Rank = CAST(DENSE_RANK() OVER(ORDER BY CAST(t.Bet/mb.MinBet AS INT) DESC, t.TrxCount  DESC) AS  INT),
			u.Name,
			Score = CAST(t.Bet/mb.MinBet AS INT)
	FROM @table t
		INNER JOIN dbo.[User] u WITH (NOLOCK) ON (t.UserId = u.Id and (@IncludeDemo = 1 OR u.[IsDemo] = 0))
		INNER JOIN dbo.Tournament t2 WITH (NOLOCK) ON (t2.Id = @TournamentId AND t2.IsDeleted = 0)
		INNER JOIN @MinBet mb ON mb.CurrencyId = u.CurrencyId
	WHERE
		t.TrxCount >= t2.MinHands 		
END;
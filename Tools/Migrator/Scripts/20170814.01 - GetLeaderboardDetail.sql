/****** Object:  StoredProcedure [dbo].[GETLEADERBOARDDETAIL]    Script Date: 8/11/2017 4:59:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vincente
-- Create date:	9 May 2017
-- Description:	Get Tournament leader board detail for Integration API
-- Path: Integration > tournamentdetail > type = 1
-- Modify : 
-- 1. Update the script to get 100 players who have been got highest rank
-- =============================================
ALTER PROCEDURE [dbo].[GETLEADERBOARDDETAIL] @TournamentId INT
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

	SELECT TOP 100 Rank = CAST(RANK() OVER(ORDER BY t.Score DESC, t.Bet DESC ) AS INT),
			u.Name,
			t.Score
	FROM @table t
		INNER JOIN dbo.[User] u WITH (NOLOCK) ON (t.UserId = u.Id and u.IsDemo = 0)
		INNER JOIN dbo.Tournament t2 WITH (NOLOCK) ON (t2.Id = @TournamentId AND t2.IsDeleted = 0)
	WHERE
		t.Score >= t2.MinHands 		
END;
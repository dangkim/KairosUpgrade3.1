SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[BOGETGLOBALTOURNAMENTREPORT] @TournamentId INT,
                                                  @UserId       INT = NULL,
                                                  @CurrencyId   INT = NULL,
                                                  @GameIds      NVARCHAR(128) = NULL, 
												  @IncludeDemo	BIT = NULL
AS
BEGIN
		
	DECLARE @MinBet Table (Id INT IDENTITY(1,1) , CurrencyId INT, MinBet DECIMAL(23,8),UNIQUE NONCLUSTERED(CurrencyId));

	INSERT INTO @MinBet
	SELECT 
		CurrencyId = tr.RelationId
		,MinBet  = tr.RelationValue
	FROM TRelation tr WITH(NOLOCK) 
	WHERE 
		TournamentId = @TournamentId 
		AND RelationType = 3 -- minbet
		
	SELECT 
		ti.UserId
		,MemberName = u.Name
		,Operator= op.Name
		,Currency = c.IsoCode
		,CurrencyId = c.Id
		,EligiblePoints = CAST(SUM(Bet)/mb.MinBet AS INT)
		,TrxCount = SUM(TrxCount)
		,TotalBet = SUM(Bet)
		,TotalBetL = SUM(BetL)
		,WinLose = SUM(Win)
		,WinLoseL = SUM(WinL)
		,FirstBet = DATEADD(hh, 8, MIN(TimeFirstBet)) 
		,LastBet = DATEADD(hh, 8, MAX(TimeLastBet)) 
	FROM  TOURNAMENTREPORTINFO ti WITH(NOLOCK)  
	INNER JOIN [USER] u WITH (NOLOCK) ON (ti.UserId = u.Id AND (@IncludeDemo = 1 OR u.[IsDemo] = 0))
	INNER JOIN [CURRENCY] c WITH (NOLOCK) ON u.CurrencyId = c.Id
	INNER JOIN Operator op WITH(NOLOCK) ON op.Id = u.OperatorId
	INNER JOIN @MinBet mb ON mb.CurrencyId = u.CurrencyId
	WHERE
		ti.TournamentId = @TournamentId
		AND ti.GameId IN (SELECT * FROM dbo.fnSplitString(@GameIds, ','))
		AND(@CurrencyId IS NULL OR c.Id = @CurrencyId)
		AND (@UserId IS NULL OR u.Id = @UserId)
	GROUP BY 
		ti.UserId
		,u.Name
		,op.Name
		,c.IsoCode
		,c.Id
		,mb.MinBet
END;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TGLOBALUSERHISTORY]
    @UserId [int],
    @Games [nvarchar](3084),
    @Platform [nvarchar](128),
    @TrxId [float],
    @STime [datetime],
    @ETime [datetime],
    @MinBet [decimal](23,8),
    @OffsetRows [int],
    @PageSize [int]
AS
BEGIN
    SELECT
    	gh.Id,
    	gh.GameTransactionId,
    	CreatedOnUtc = gh.DateTimeUtc,
    	[Type]=gh.GameResultType,
    	gh.HistoryXML,
    	gh.Bet,
    	gh.Win,
    	UserId = u.Id,
    	UserName = u.Name,
    	Currency = c.IsoCode,
    	GameId = g.Id,
    	GameName = g.Name,
    	OperatorTag = o.Tag,
    	gh.PlatformType
    FROM GameHistory AS gh WITH (NOLOCK)
    INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
    INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
    INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id
    INNER JOIN Operator o WITH (NOLOCK) ON o.Id = u.OperatorId
    WHERE 
    	gh.DateTimeUtc BETWEEN @STime AND @ETime
    	AND gh.GameId IN (SELECT * FROM dbo.fnSplitString(@Games, ',')) 
		AND gh.PlatformType%10 IN (SELECT * FROM dbo.fnSplitString(@Platform, ','))
    	AND gh.UserId = @UserId 
    	AND ((@MinBet = 0 AND gh.Bet >= 0) OR (@MinBet > 0 AND gh.Bet > 0))
    	AND (@TrxId IS NULL OR gh.GameTransactionId = @TrxId) 
    	AND gh.GameResultType <= CASE WHEN @MinBet = 0 THEN 10 ELSE 1 END 
		AND gh.IsFreeGame = 0
    ORDER BY gh.DatetimeUtc DESC OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
END
/***************************************************************************************************************************/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[BOGETGLOBALTOURNAMENTREPORT] @TournamentId INT,
                                                  @UserId       INT = NULL,
                                                  @CurrencyId   INT = NULL,
                                                  @GameIds      NVARCHAR(3084) = NULL, 
												  @IncludeDemo	BIT = NULL
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
		AND Level = 23
		AND EXISTS(SELECT 1 FROM @merchants mct WHERE mct.[OperatorId] =  ti.OperatorId)
	GROUP BY 
		ti.UserId
		,u.Name
		,op.Name
		,c.IsoCode
		,c.Id
		,mb.MinBet
END;

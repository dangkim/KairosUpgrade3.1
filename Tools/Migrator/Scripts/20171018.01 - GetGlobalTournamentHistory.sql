CREATE  PROCEDURE [dbo].[TGLOBALUSERHISTORY]
    @UserId [int],
    @Games [nvarchar](128),
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
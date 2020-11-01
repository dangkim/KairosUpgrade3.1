-- =============================================
-- Author:		John
-- Description:	Updated win lose reports to include CurrencyId and RMB columns.
-- =============================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEALL]
    @OperatorId [int],
    @GameId [int],
    @UserId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
    @IsFreeRounds [bit],
	@CurrencyId [int] = null,
	@Platform [int] = null
AS
BEGIN
    SELECT
		[Date]= CONVERT(VARCHAR(12), @StartDateInUTC, 106) +'-'+ CONVERT(VARCHAR(12),  @EndDateInUTC , 106), 
		Game = 'All',
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction =ISNULL(SUM(CONVERT(BIGINT, TrxCount)),0),
		NoOfSpin = ISNULL(SUM(CONVERT(BIGINT, SpinCount)), 0),
		AvgBet = ISNULL((SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount))), 0),
		TotalBet = ISNULL(SUM(IIF(IsFreeGame=1,0,TotalBetAmount)), 0),
		TotalWin = ISNULL(SUM(TotalWinAmount), 0),
		GameIncome = ISNULL((SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) - SUM(TotalWinAmount)), 0),
		AvgBetRMB = ISNULL((SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount))), 0),
		TotalBetRMB = ISNULL(SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)), 0),
		TotalWinRMB = ISNULL(SUM(TotalWinAmountRMB), 0),
		GameIncomeRMB = ISNULL((SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB)), 0),
		GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) 
							WHEN 0 THEN 1 
							ELSE ISNULL((SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB))), 0)
						END
    FROM
		PlatformReportInfo pri WITH(NOLOCK)
    INNER JOIN 
		[User] u WITH(NOLOCK) ON u.Id  = pri.userId
    WHERE
		pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)    
		AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
		AND (pri.GameId = @GameId OR @GameId IS NULL)
		AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
		AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
		AND (pri.PlatformId = @Platform OR @Platform IS NULL)
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEDAILY]
    @OperatorId [int],
    @GameId [int],
    @UserId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
    @IsFreeRounds [bit],
	@CurrencyId [int] = null,
	@Platform [int] = null
AS
BEGIN
    SELECT 
    Date = CONVERT(VARCHAR(32), pri.ChangeTime, 23),
    Game = 'All',
    NoOfPlayer = COUNT(DISTINCT UserId),
    NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
    NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
	AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
    TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
    TotalWin = SUM(pri.TotalWinAmount),
    GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(TotalWinAmount),
    AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
    TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
    TotalWinRMB = SUM(pri.TotalWinAmountRMB),
    GameIncomeRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
    GamePayoutPer = CASE SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) END
    FROM PlatformReportInfo pri WITH(NOLOCK)
    INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
    WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
    AND (u.Id = @UserId OR @UserId IS NULL)
    AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
    AND (pri.GameId = @GameId OR @GameId IS NULL)
    AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
	AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
	AND (pri.PlatformId = @Platform OR @Platform IS NULL)
    GROUP BY pri.ChangeTime
    ORDER BY pri.ChangeTime
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEWEEKLY]	
	@OperatorId		INT,
	@GameId			INT			= NULL,
	@UserId			INT			= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL,
	@CurrencyId [int] = null,
	@Platform [int] = null
AS
BEGIN
	SELECT [Week] = DATEPART(wk, pri.ChangeTime),
		[Year] = DATEPART(year, pri.ChangeTime),
		Game = 'All',
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
        NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
		AvgBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
		TotalBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)),
		TotalWin = SUM(TotalWinAmount),
		GameIncome = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) - SUM(TotalWinAmount),
        AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
		TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		TotalWinRMB = SUM(TotalWinAmountRMB),
		GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
		GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
	FROM PlatformReportInfo pri WITH(NOLOCK)
	INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
	WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		AND (u.Id = @UserId OR @UserId IS NULL)
		AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
		AND (pri.GameId = @GameId OR @GameId IS NULL)
		AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
		AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
		AND (pri.PlatformId = @Platform OR @Platform IS NULL)
	GROUP BY DATEPART(wk, pri.ChangeTime), DATEPART(year, pri.ChangeTime)
	ORDER BY [Year], [Week]
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEMONTHLY]
    @OperatorId [int],
    @GameId [int],
    @UserId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
    @IsFreeRounds [bit],
	@CurrencyId [int] = null,
	@Platform [int] = null
AS
BEGIN
    SELECT [Month] = DATEPART(month, pri.ChangeTime),
    [Year] = DATEPART(year, pri.ChangeTime),
    Game = 'All',
    NoOfPlayer = COUNT(DISTINCT UserId),
    NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
    NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
	AvgBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
    TotalBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)),
    TotalWin = SUM(TotalWinAmount),
    GameIncome = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) - SUM(TotalWinAmount),
    AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
    TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
    TotalWinRMB = SUM(TotalWinAmountRMB),
    GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
    GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
    FROM PlatformReportInfo pri WITH(NOLOCK)
    INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
    WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
    AND (u.Id = @UserId OR @UserId IS NULL)
    AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
    AND (pri.GameId = @GameId OR @GameId IS NULL)
    AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
	AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
	AND (pri.PlatformId = @Platform OR @Platform IS NULL)
    GROUP BY DATEPART(month, pri.ChangeTime), DATEPART(year, pri.ChangeTime)
    ORDER BY [Year], [Month]
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEOPERATOR]
    @OperatorId [int],
    @GameId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit] = NULL,
    @IsFreeRounds [bit] = NULL,
	@CurrencyId [int] = null,
	@Platform [int] = null
AS
BEGIN
    SELECT 
		OperatorId = o.Id, 
		OperatorTag = o.Tag,
		Game = 'All',
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
		NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
		AvgBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
		TotalBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)),
		TotalWin = SUM(TotalWinAmount),
		GameIncome = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) - SUM(TotalWinAmount),
		AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
		TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		TotalWinRMB = SUM(TotalWinAmountRMB),
		GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
		GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
	FROM PlatformReportInfo pri WITH(NOLOCK)
	INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
	INNER JOIN Operator o WITH(NOLOCK) on o.Id = u.OperatorId
	WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
		AND (pri.GameId = @GameId OR @GameId IS NULL)
		AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
		AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
		AND (pri.PlatformId = @Platform OR @Platform IS NULL)
	GROUP BY o.Id, o.Tag
	ORDER BY o.Tag
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEOPERATORGAME]	
	@OperatorId		INT,
	@GameId			INT			= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL,
	@CurrencyId [int] = null,
	@Platform [int] = null
AS
BEGIN
	SELECT 
		OperatorId = o.Id, 
		OperatorTag = o.Tag,
		GameId = pri.GameId,
		Game = g.Name,
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
        NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
		AvgBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
		TotalBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)),
		TotalWin = SUM(TotalWinAmount),
		GameIncome = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) - SUM(TotalWinAmount),
        AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
		TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		TotalWinRMB = SUM(TotalWinAmountRMB),
		GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
		GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
	FROM PlatformReportInfo pri WITH(NOLOCK)
	INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
	INNER JOIN Operator o WITH(NOLOCK) on o.Id = u.OperatorId
	INNER JOIN Game g WITH(NOLOCK) on g.Id = pri.GameId
	WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
		AND (pri.GameId = @GameId OR @GameId IS NULL)
		AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
		AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
		AND (pri.PlatformId = @Platform OR @Platform IS NULL)
	GROUP BY o.Id, o.Tag, pri.GameId, g.Name
	ORDER BY o.Tag, g.Name
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEGAME]
    @OperatorId [int],
    @GameId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
    @IsFreeRounds [bit],
	@CurrencyId [int] = null,
	@Platform [int] = null
AS
BEGIN
    SELECT pri.GameId,
    g.Name AS Game,
    NoOfPlayer = COUNT(DISTINCT(pri.UserId)),
    NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),
    NoOfSpin = SUM(CONVERT(BIGINT, pri.SpinCount)),
    AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
	TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
	TotalWin = SUM(pri.TotalWinAmount),
    GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(pri.TotalWinAmount),
    AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
    TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
    TotalWinRMB = SUM(pri.TotalWinAmountRMB),
    GameIncomeRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) - SUM(pri.TotalWinAmountRMB),
    GamePayoutPer = CASE SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) END
    FROM PlatformReportInfo pri WITH (NOLOCK)
    INNER JOIN Game g WITH (NOLOCK) ON pri.GameId = g.Id
    INNER JOIN [User] u WITH (NOLOCK) ON pri.UserId = u.Id
    WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
    AND (pri.GameId = @GameId OR @GameId IS NULL)
    AND pri.ChangeTime >= CONVERT(VARCHAR(32), @StartDateInUTC, 120)
    AND pri.ChangeTime < CONVERT(VARCHAR(32), @EndDateInUTC, 120)
    AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
	AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
	AND (pri.PlatformId = @Platform OR @Platform IS NULL)
    GROUP BY pri.GameId, g.Name
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEGAMEMEMBER]
    @OperatorId [int],
    @GameId [int],
    @CurrencyId [int],
    @UserId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
    @IsFreeRounds [bit],
	@Platform [int] = null
AS
BEGIN
    SELECT 
		MemberId = u.Id,
    	MemberName = u.Name, 
    	Operator = o.Name, 
    	Currency = c.IsoCode,
    	NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),
		NoOfSpin = SUM(CONVERT(BIGINT, pri.SpinCount)),
		AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
		TotalWin = SUM(pri.TotalWinAmount),
    	TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
		GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(pri.TotalWinAmount), 
		AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
    	TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
    	TotalWinRMB = SUM(pri.TotalWinAmountRMB),
    	GameIncomeRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) - SUM(pri.TotalWinAmountRMB),
    	GamePayoutPer = CASE SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) END
    FROM PlatformReportInfo pri WITH (NOLOCK)
    INNER JOIN [User] u WITH (NOLOCK) ON pri.UserId = u.Id
    INNER JOIN Operator o WITH (NOLOCK) ON u.OperatorId = o.Id
    INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id
    WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    	AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
    	AND (pri.GameId = @GameId OR @GameId IS NULL)
    	AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
    	AND (pri.UserId = @UserId OR @UserId IS NULL)
    	AND pri.ChangeTime >= CONVERT(VARCHAR(32), @StartDateInUTC, 120)
    	AND pri.ChangeTime < CONVERT(VARCHAR(32), @EndDateInUTC, 120)
    	AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
		AND (pri.PlatformId = @Platform OR @Platform IS NULL)
    GROUP BY u.Id, u.Name, o.Id, o.Name, c.IsoCode
    ORDER BY u.Name
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEGAMECURRENCY]
    @OperatorId [int],
    @GameId [int] = NULL,
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit] = NULL,
    @IsFreeRounds [bit] = NULL,
	@CurrencyId [int] = null,
	@Platform [int] = null
AS
BEGIN
    SELECT 
		CurrencyId,
    	Currency = c.IsoCode,
    	NoOfPlayer = COUNT(DISTINCT UserId),
    	NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),
		NoOfSpin = SUM(CONVERT(BIGINT, pri.SpinCount)),
		AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
    	TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
    	TotalWin = SUM(pri.TotalWinAmount),
    	GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(pri.TotalWinAmount),
    	AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
    	TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
    	TotalWinRMB = SUM(pri.TotalWinAmountRMB),
    	GameIncomeRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) - SUM(pri.TotalWinAmountRMB),
    	GamePayoutPer = CASE SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) END
    FROM [dbo].[PlatformReportInfo] pri WITH (NOLOCK)
    INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = pri.UserId
    INNER JOIN	[dbo].[Currency] c WITH (NOLOCK) ON c.Id = u.CurrencyId
    WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    	AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
    			AND (pri.GameId = @GameId OR @GameId IS NULL)
    	AND pri.ChangeTime >= CONVERT(VARCHAR(32), @StartDateInUTC, 120)
    	AND pri.ChangeTime < CONVERT(VARCHAR(32), @EndDateInUTC, 120)
    			AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
    	AND OffsetId = 42
		AND (pri.PlatformId = @Platform OR @Platform IS NULL)
		AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
    GROUP BY u.CurrencyId, c.IsoCode
END

GO
ALTER PROCEDURE [dbo].[REPORTWINLOSEPLATFORM]
	@OperatorId		INT,
	@GameId			INT			= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL,
	@CurrencyId [int] = null
AS
BEGIN
	SELECT 
		[Platform] = CASE pri.PlatformId % 10
			WHEN 1 THEN 'Web'
			WHEN 2 THEN 'Download'
			WHEN 3 THEN 'Mobile'
			WHEN 4 THEN 'Mini'
			ELSE 'None'
		END,
		Game = 'All',
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
        NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
		AvgBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
		TotalBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)),
		TotalWin = SUM(TotalWinAmount),
		GameIncome = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) - SUM(TotalWinAmount),
        AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
		TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		TotalWinRMB = SUM(TotalWinAmountRMB),
		GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
		GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
	FROM PlatformReportInfo pri WITH(NOLOCK)
	INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
	INNER JOIN Operator o WITH(NOLOCK) on o.Id = u.OperatorId
	WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
		AND (pri.GameId = @GameId OR @GameId IS NULL)
		AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
		AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
	GROUP BY pri.PlatformId % 10
	ORDER BY [Platform]
END


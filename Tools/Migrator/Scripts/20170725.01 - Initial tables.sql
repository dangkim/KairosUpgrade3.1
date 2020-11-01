
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Jackpot')
  BEGIN
    EXEC ('CREATE SCHEMA Jackpot;');
  END
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Profile')
  BEGIN
    EXEC ('CREATE SCHEMA Profile;');
  END
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Enum')
  BEGIN
    EXEC ('CREATE SCHEMA Enum;');
  END
GO

/****** Object:  StoredProcedure [dbo].[BOGETLEADERBOARDDETAIL]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vincente
-- Create date:	9 May 2017
-- Description:	Get Backoffice tournament leader board detail
-- Path: Marketing > Tournament > Leaderboard
-- =============================================
CREATE PROCEDURE [dbo].[BOGETLEADERBOARDDETAIL] @TournamentId INT
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
		  Rank = CAST(RANK() OVER(ORDER BY r.TrxCount DESC, r.BetL DESC) AS  INT),
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
/****** Object:  StoredProcedure [dbo].[BOGETTOURNAMENTREPORT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Vincente
-- Create date:	9 May 2017
-- Description:	Get Backoffice tournament report
-- Path: Marketing > Tournament > Report
-- =============================================

CREATE PROCEDURE [dbo].[BOGETTOURNAMENTREPORT] @TournamentId INT,
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
                   INNER JOIN [USER] u WITH (NOLOCK) ON (r.UserId = u.Id and u.IsDemo = 0)
                   INNER JOIN [CURRENCY] c WITH (NOLOCK) ON u.CurrencyId = c.Id
              WHERE(@CurrencyId IS NULL OR c.Id = @CurrencyId)
                   AND (@UserId IS NULL OR u.Id = @UserId)
              ORDER BY r.EligibleTrxCount DESC;
     END;

GO
/****** Object:  StoredProcedure [dbo].[CHARTAUPONDATE]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kaidan Joseph	
-- Create date: Jan 06, 2017
-- Description:	Get the total number of players that can be showing by day and total
-- =============================================
CREATE PROCEDURE [dbo].[CHARTAUPONDATE]
	@StartDate DATETIME,
	@EndDate DATETIME, 
	@Offset INT,	
	@Currency NVARCHAR(128)  = NULL, 
	@Merchant INT =  NULL, 
	@Member NVARCHAR(128) = NULL, 
	@Game INT = NULL, 
	@IsFreeRound BIT = NULL,
	@Platform NVARCHAR(128) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	DECLARE @UserId BigINT;
	DECLARE @result TABLE([Day] DATETIME, UserId INT);	
			
	SET NOCOUNT ON;	
	SELECT
		@UserId = u.Id 
	FROM	
		[User] u 
	WHERE u.Name = @Member;

   ;WITH MEMBER AS (	

			SELECT	
				[Day] = CONVERT(DATE, DATEADD(HOUR, @Offset, gh.DateTimeUtc))
				,gh.UserId				
			FROM
				GameHistory gh WITH(NOLOCK)				
			 WHERE 
				(@UserId IS NULL OR gh.UserId = @UserId) AND 
				(gh.[DateTimeUtc] BETWEEN  @StartDate AND @EndDate) AND			
				(@Game IS NULL OR gh.GameId = @Game) AND
				gh.PlatformType IN (SELECT * FROM fnSplitString(@Platform,',')) AND				
				(@IsFreeRound IS NULL OR gh.IsFreeGame = @IsFreeRound)
			GROUP BY 
					CONVERT(DATE, DATEADD(HOUR, @Offset, gh.DateTimeUtc)),
					gh.UserId)

	INSERT INTO @result
	SELECT
		aup.[Day], 
		aup.UserId
	
	FROM 
		MEMBER aup
	INNER JOIN 
			[User] u  WITH(NOLOCK) ON aup.UserId = u.Id	
	WHERE 	
		(@Merchant IS NULL OR u.OperatorId  = @Merchant) AND 
		(@Currency IS NULL OR u.CurrencyId IN (SELECT * FROM fnSplitString(@Currency,',')))
	GROUP BY
			aup.[Day], 		
			aup.UserId
	SELECT
		CONVERT(VARCHAR(12), rs.[Day], 107) AS [Day],	
		MemberAmount  = COUNT(UserId)
	FROM 
		@result rs
	GROUP BY 
		 rs.[Day]
	ORDER BY
		 rs.[Day]
END


GO
/****** Object:  StoredProcedure [dbo].[CHARTAUPONYEAR]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kaidan Joseph	
-- Create date: Jan 06, 2017
-- Description:	Get the total number of players that can be showing by month and total
-- =============================================
CREATE PROCEDURE [dbo].[CHARTAUPONYEAR]
	@StartDate DATETIME,
	@EndDate DATETIME, 
	@Offset INT,	
	@Currency NVARCHAR(128)  = NULL, 
	@Merchant INT =  NULL, 
	@Member NVARCHAR(128) = NULL, 
	@Game INT = NULL, 
	@IsFreeRound BIT = NULL,
	@Platform NVARCHAR(128) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	DECLARE @UserId BigINT;
	DECLARE @result TABLE([Month] INT, UserId INT);		
			
	SET NOCOUNT ON;
	SET @EndDate = DateAdd(Day , 1 , @EndDate);
	SELECT
		@UserId = u.Id 
	FROM	
		[User] u 
	WHERE u.Name = @Member;

   ;WITH MEMBER AS (	

			SELECT	
				[Month] = DATEPART(mm, DATEADD(HOUR, @Offset, gh.DateTimeUtc))
				,gh.UserId				
			FROM
				GameHistory gh WITH(NOLOCK)			
			WHERE 
				(@UserId IS NULL OR gh.UserId = @UserId) AND 
				(gh.[DateTimeUtc] BETWEEN  @StartDate AND @EndDate) AND			
				(@Game IS NULL OR gh.GameId = @Game) AND
				gh.PlatformType IN (SELECT * FROM fnSplitString(@Platform,',')) AND			
				(@IsFreeRound IS NULL OR gh.IsFreeGame = @IsFreeRound)
			GROUP BY 
					DATEPART(mm, DATEADD(HOUR, @Offset, gh.DateTimeUtc))
					,gh.UserId)

	INSERT INTO @result
	SELECT
		aup.[Month],		
		aup.UserId
	
	FROM 
		MEMBER aup
	INNER JOIN 
			[User] u  WITH(NOLOCK) ON aup.UserId = u.Id	
	WHERE 	
		(@Merchant IS NULL OR u.OperatorId  = @Merchant) AND 
		(@Currency IS NULL OR u.CurrencyId IN (SELECT * FROM fnSplitString(@Currency,',')))
	GROUP BY
			aup.[Month], 		
			aup.UserId
	SELECT
		DateName(Month, DateAdd(Month , rs.[Month] , -1 )) AS [Month],
		MemberAmount  = COUNT(rs.UserId)
	FROM 
		@result rs
	GROUP BY 
		[Month]
	ORDER BY
		rs.[Month] ASC
END


GO
/****** Object:  StoredProcedure [dbo].[CHARTCURRENCIESTRENDONDATE]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CHARTCURRENCIESTRENDONDATE]
		@StartDate DATETIME,
		@EndDate DATETIME , 
		@Offset INT,
		@Currency NVARCHAR(128)  = NULL, 
		@Merchant INT =  NULL, 
		@Member NVARCHAR(128) = NULL, 
		@Game INT = NULL, 
		@IsFreeRound BIT = NULL, 
		@Platform NVARCHAR(128) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from	
	select 'remove later' --add by dba,
	/*
	DECLARE @table table (Id BIGINT);
	SET NOCOUNT ON;

	INSERT INTO @table
	SELECT
		u.Id	
	FROM	
		[User] u 
	WHERE u.Name = @Member;

    ;WITH BetPerformance AS (
		SELECT	
			[Day] = CONVERT(DATE, DATEADD(HOUR, @Offset, gh.DateTimeUtc)),			
			gh.UserId,			
			SUM(IIF(ps.IsSideBet = 1 AND gh.GameResultType = 1,gh.Bet*2, gh.Bet)) AS TotalBet
		FROM
			GameHistory gh WITH(NOLOCK)
		LEFT JOIN 
			[PROFILE].SPINBET ps WITH(NOLOCK) ON ps.GameTransactionId = gh.GameTransactionId
		 WHERE 
			(@Member IS NULL OR gh.UserId IN (SELECT * FROM @table)) AND 
			(gh.[DateTimeUtc] BETWEEN  @StartDate AND @EndDate) AND			
			(@Game IS NULL OR gh.GameId = @Game) AND
			gh.PlatformType IN (SELECT * FROM fnSplitString(@Platform,',')) AND
			gh.GameResultType IN (1,2,6) AND /* <--- get the transaction that has the kind of spin is  doubleup, gamble or normal*/
			gh.Bet > 0  AND
			gh.IsFreeGame = 0
		GROUP BY 
				CONVERT(DATE, DATEADD(HOUR, @Offset, gh.DateTimeUtc)),	
				gh.UserId) 
	SELECT 
		CONVERT(VARCHAR(12), [Day], 107) AS [Day],
		u.CurrencyId, 
		SUM(bp.TotalBet) AS TotalBet		
	FROM 
		BetPerformance bp 
	INNER JOIN 
		[User] u  WITH(NOLOCK) ON bp.UserId = u.Id	
	WHERE 		
		(@Merchant IS NULL OR u.OperatorId  = @Merchant) AND 
		(@Currency IS NULL OR u.CurrencyId IN (SELECT * FROM fnSplitString(@Currency,',')))
	GROUP BY
		bp.[Day], 		
		u.CurrencyId	
	ORDER BY	
		bp.[Day] ,
		u.CurrencyId
		*/
END


GO
/****** Object:  StoredProcedure [dbo].[CHARTCURRENCIESTRENDONYEAR]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kaidan	
-- Create date:  Dec 19, 2016
-- Description: Get the Chart data of the Currency Trends (Currency Performance)
-- =============================================
CREATE PROCEDURE [dbo].[CHARTCURRENCIESTRENDONYEAR]
		@StartDate DATETIME,
		@EndDate DATETIME , 
		@Offset INT,
		@Currency NVARCHAR(128)  = NULL, 
		@Merchant INT =  NULL, 
		@Member NVARCHAR(128) = NULL, 
		@Game INT = NULL, 
		@IsFreeRound BIT = NULL, 
		@Platform NVARCHAR(128) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	DECLARE @UserId BigINT;
	SET NOCOUNT ON;	
	SELECT
		@UserId = u.Id 
	FROM	
		[User] u 
	WHERE u.Name = @Member;
	;WITH BetPerformance AS (
		SELECT	
			[Month] = DATEPART(mm, DATEADD(HOUR, @Offset, gh.DateTimeUtc)),		
			gh.UserId, 
			gh.GameId, 
			SUM(IIF(ps.IsSideBet = 1 AND gh.GameResultType = 1,gh.Bet*2, gh.Bet)) AS TotalBet
		FROM
			GameHistory gh WITH(NOLOCK)		
		LEFT JOIN 
			[PROFILE].SPINBET ps WITH(NOLOCK) ON ps.GameTransactionId = gh.Id
		 WHERE 
			(@UserId IS NULL OR gh.UserId = @UserId) AND 
			(gh.[DateTimeUtc] BETWEEN  @StartDate AND @EndDate) AND			
			(@Game IS NULL OR gh.GameId = @Game) AND
			gh.PlatformType IN (SELECT * FROM fnSplitString(@Platform,',')) AND
			gh.GameResultType IN (1,2,6) AND /* <--- get the transaction that has the kind of spin is doubleup or normal*/
			gh.Bet > 0  AND
			gh.IsFreeGame = 0
		GROUP BY 
				DATEPART(mm, DATEADD(HOUR, @Offset, gh.DateTimeUtc)),
				gh.UserId, 
				gh.GameId) 
	SELECT 
		DateName(Month, DateAdd(Month , bp.[Month] , -1 )) AS [Month], 	
		u.CurrencyId, 
		SUM(bp.TotalBet) AS TotalBet	
	FROM  BetPerformance bp
	INNER JOIN 
		[User] u WITH(NOLOCK) ON bp.UserId = u.Id	
	WHERE 
		(@Merchant IS NULL OR u.OperatorId  = @Merchant) AND 
		(@Currency IS NULL OR u.CurrencyId IN (SELECT * FROM fnSplitString(@Currency,',')))
	GROUP BY
		bp.[Month], 		
		u.CurrencyId	
	ORDER BY	
	 bp.[Month] ,
	 u.CurrencyId	
END


GO
/****** Object:  StoredProcedure [dbo].[COINDENOMINATIONREPORTINFOINSERT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[COINDENOMINATIONREPORTINFOINSERT](
    @ChangeTime        datetime,
    @OffsetId          int,
    @GameId            int,
    @UserId            int,
    @GroupId           int,
    @Level             int,
    @LineBet           decimal(23,8),
    @Multiplier        int,
    @TrxCount          int,
    @SpinCount         int,
    @FreeSpinCount     int,
    @BonusCount        int,
    @GambleCount       int,
    @TotalBet          decimal(23,8),
    @TotalWin          decimal(23,8),
    @TotalSpinWin      decimal(23,8),
    @TotalFreeSpinWin  decimal(23,8),
    @TotalBonusWin     decimal(23,8),
    @TotalGambleWin    decimal(23,8),
    @TotalBetL         decimal(23,8),
    @TotalWinL         decimal(23,8),
    @TotalSpinWinL     decimal(23,8),
    @TotalFreeSpinWinL decimal(23,8),
    @TotalBonusWinL    decimal(23,8),
    @TotalGambleWinL   decimal(23,8))
AS
BEGIN
    DECLARE @ISNEW INT;

    BEGIN TRAN
        UPDATE COINDENOMINATIONREPORTINFO WITH (SERIALIZABLE) SET
	        TrxCount = TrxCount + @TrxCount,
	        SpinCount = SpinCount + @SpinCount,
            FreeSpinCount = FreeSpinCount + @FreeSpinCount,
            BonusCount = BonusCount + @BonusCount,
            GambleCount = GambleCount + @GambleCount,
            TotalBet = TotalBet + @TotalBet,
            TotalWin = TotalWin + @TotalWin,
            TotalSpinWin = TotalSpinWin + @TotalSpinWin,
            TotalFreeSpinWin = TotalFreeSpinWin + @TotalFreeSpinWin,
            TotalBonusWin = TotalBonusWin + @TotalBonusWin,
            TotalGambleWin = TotalGambleWin + @TotalGambleWin,
            TotalBetL = TotalBetL + @TotalBetL,
            TotalWinL = TotalWinL + @TotalWinL,
            TotalSpinWinL = TotalSpinWinL + @TotalSpinWinL,
            TotalFreeSpinWinL = TotalFreeSpinWinL + @TotalFreeSpinWinL,
            TotalBonusWinL = TotalBonusWinL + @TotalBonusWinL,
            TotalGambleWinL = TotalGambleWinL + @TotalGambleWinL
        WHERE ChangeTime=@ChangeTime AND OffsetId=@OffsetId AND GameId=@GameId AND UserId=@UserId AND GroupId=@GroupId AND [Level]=@Level AND LineBet=@LineBet AND Multiplier=@Multiplier;	  
	  
        SET @ISNEW = @@ROWCOUNT;
		
        IF (@ISNEW = 0)
        BEGIN
            INSERT INTO COINDENOMINATIONREPORTINFO(ChangeTime, OffsetId, GameId, UserId, GroupId, [Level], LineBet, Multiplier, TrxCount, SpinCount, FreeSpinCount, BonusCount, GambleCount, TotalBet, TotalWin, TotalSpinWin, TotalFreeSpinWin, TotalBonusWin, TotalGambleWin, TotalBetL, TotalWinL, TotalSpinWinL, TotalFreeSpinWinL, TotalBonusWinL, TotalGambleWinL) VALUES
		    (@ChangeTime, @OffsetId, @GameId, @UserId, @GroupId, @Level, @LineBet, @Multiplier, @TrxCount, @SpinCount, @FreeSpinCount, @BonusCount, @GambleCount, @TotalBet, @TotalWin, @TotalSpinWin, @TotalFreeSpinWin, @TotalBonusWin, @TotalGambleWin, @TotalBetL, @TotalWinL, @TotalSpinWinL, @TotalFreeSpinWinL, @TotalBonusWinL, @TotalGambleWinL);
        END
        COMMIT TRAN

    RETURN @ISNEW;
END

GO
/****** Object:  StoredProcedure [dbo].[ELIGIBLEFREEROUND]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kaidan Joseph
-- Create date: April 18, 2017
-- Description:	Get the eligible freeround data for spercific user
-- =============================================
CREATE PROCEDURE [dbo].[ELIGIBLEFREEROUND]	
	@OperatorId INT,
	@UserId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @now DATETIME = GETUTCDATE();
	SELECT 

		GameId = fr.GameId, 
		StartDateUtc = fr.StartDateUtc, 
		EndDateUtc =fr.EndDateUtc,
		[Status] =	CASE  
						WHEN @now > fr.EndDateUtc THEN 'Completed'
						WHEN (fr.StartDateUtc <= @now AND @now < fr.EndDateUtc) THEN 'Ongoing'
						ELSE 'Upcoming'
					END

	FROM [dbo].[FreeRound] fr WITH(NOLOCK)
	LEFT JOIN [dbo].[FRPlayer] p WITH(NOLOCK) ON (fr.Id = p.FreeRoundId)
	LEFT JOIN [dbo].[FreeRoundData] frd WITH(NOLOCK) ON fr.Id  = frd.CampaignId AND p.UserId = frd.UserId
	WHERE 
		fr.OperatorId = @OperatorId AND
		frd.UserId = @UserId AND
		fr.IsCancelled = 0 AND 
		@now <= fr.EndDateUtc AND
		frd.[State] <> 3 AND -- none cancelled
		frd.[IsFinish] = 0	-- none finished 
END


GO
/****** Object:  StoredProcedure [dbo].[FREEROUNDCOUNT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FREEROUNDCOUNT]
	@OperatorId INT,
	@GameId INT,
	@StartDateInUTC DATETIME,
	@EndDateInUTC DATETIME,
	@FRName NVARCHAR(255),
	@Status INT
AS
BEGIN
	;WITH CTE_FreeRoundList AS (
		SELECT Id,
		FRStatus = CASE WHEN IsCancelled = 1 THEN 4 
						WHEN getutcdate() > EndDateUtc THEN 3 
						WHEN getutcdate() >= StartDateUtc and getutcdate() < EndDateUtc THEN 2 
						ELSE 1 END 
		FROM FREEROUND WITH(NOLOCK) 
		WHERE ((StartDateUtc BETWEEN @StartDateInUTC AND @EndDateInUTC) OR (@StartDateInUTC IS NULL OR @EndDateInUTC IS NULL)) 
		AND IsDeleted<>1 
		AND (@OperatorId IS NULL  OR OperatorId = @OperatorId) 
		AND (@GameId IS NULL OR GameId = @GameId) 
		AND (@FRName IS NULL OR Name LIKE '%' + @FRName + '%')
	)

	SELECT COUNT(1) FROM CTE_FreeRoundList
	WHERE (@Status IS NULL  OR FRStatus = @Status)
END

GO
/****** Object:  StoredProcedure [dbo].[FREEROUNDLIST]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Add new Column

-- Changes: 
-- Remove Parameters - OffsetRows and PageSize 
-- Add CTE_FreeRoundWinLose and CTE_FreeRoundList
-- Alter display result
CREATE PROCEDURE [dbo].[FREEROUNDLIST]
	@OperatorId INT,
	@GameId INT,
	@StartDateInUTC DATETIME,
	@EndDateInUTC DATETIME,
	@FRName NVARCHAR(255),
	@Status INT
AS
BEGIN
	;WITH CTE_FreeRoundWinLose AS (
	SELECT 
		FreeRoundId=fr.Id,
		BetL = SUM(ISNULL(fri.BetL,0)),
		WinLoseL=SUM(ISNULL(fri.WinL,0))
	FROM FreeRound fr WITH(NOLOCK)
	LEFT JOIN [FreeRoundReportInfo] fri WITH(NOLOCK) ON fr.Id=fri.FreeRoundId
	GROUP BY fr.Id
	),
	CTE_FreeRoundList AS (
		SELECT  
		fr.Id, Operator=o.Name, fr.Name, Game=g.Name, Lines=fr.Lines,
	    NoOfPlayers = ISNULL(Count(p.UserId),0),
	    NoFreeRounds = fr.LimitPerPlayer, 
	    TotalNoFreeRounds = (fr.LimitPerPlayer*ISNULL(Count(p.UserId),0)), 
	    StartDateUtc = DATEADD(hh,8,fr.StartDateUtc), EndDateUtc=DATEADD(hh,8,fr.EndDateUtc),
	    FRStatus = CASE WHEN fr.IsCancelled = 1 THEN 4 
					WHEN getutcdate() > fr.EndDateUtc THEN 3 
					WHEN getutcdate() >= fr.StartDateUtc and getutcdate() < fr.EndDateUtc THEN 2 
					ELSE 1 END,
	    NoOfPlayerClaimed = COUNT(frd.TimeClaimed),
	    NoOfClaimedFreeRounds = ISNULL(SUM(fr.LimitPerPlayer - frd.[Counter]),0),
	    NoOfUnclaimedFreeRounds = SUM(ISNULL(frd.[Counter],fr.LimitPerPlayer)),	
	    IsCancelled = fr.IsCancelled, 
        [Owner] = a.UserName
    FROM [dbo].[FreeRound] fr WITH(NOLOCK)
    INNER JOIN [dbo].[Operator] o WITH(NOLOCK) ON fr.OperatorId = o.Id
    INNER JOIN [dbo].[Game] g WITH(NOLOCK) ON fr.GameId = g.Id
    INNER JOIN [dbo].[Account] a with(nolock) ON fr.OwnerId = a.id
    LEFT JOIN [dbo].[FRPlayer] p WITH(NOLOCK) ON fr.Id = p.FreeRoundId 
    LEFT JOIN [dbo].[FreeRoundData] frd WITH(NOLOCK) ON fr.Id  = frd.CampaignId AND p.UserId = frd.UserId
    WHERE ((fr.StartDateUtc BETWEEN @StartDateInUTC AND @EndDateInUTC) OR (@StartDateInUTC IS NULL OR @EndDateInUTC IS NULL))
    AND fr.IsDeleted<>1 
    AND (@OperatorId IS NULL  OR fr.OperatorId = @OperatorId)
	AND (@GameId IS NULL OR fr.GameId = @GameId) AND (@FRName IS NULL OR fr.Name LIKE '%' + @FRName + '%')
    GROUP BY fr.Id, o.Name, fr.Name, g.Name, fr.Lines, fr.LimitPerPlayer, fr.StartDateUtc, fr.EndDateUtc, p.FreeRoundId, a.UserName, fr.IsCancelled    
	)

	SELECT 
		[No] = ROW_NUMBER() OVER (ORDER BY frl.FRStatus ASC, frl.StartDateUtc DESC)
		,frl.Id
		,frl.Operator
		,frl.Name
		,frl.Game
		,frl.Lines
		,frl.StartDateUtc
		,frl.EndDateUtc
		,[Status] = CASE WHEN frl.FRStatus = 1 THEN 'Upcoming' WHEN frl.FRStatus = 2 THEN 'Ongoing' WHEN frl.FRStatus = 3 THEN 'Completed' ELSE 'Cancelled' END
		,frl.NoFreeRounds
		,frl.NoOfPlayers
		,frl.TotalNoFreeRounds
		,frl.NoOfPlayerClaimed
		,frl.NoOfClaimedFreeRounds
		,frl.NoOfUnclaimedFreeRounds
		,TotalSpinValueL = fwl.BetL
		,TotalWinLoseL = fwl.WinLoseL
		,frl.[Owner]		
		,frl.IsCancelled
	FROM CTE_FreeRoundList frl
	INNER JOIN CTE_FreeRoundWinLose fwl ON frl.Id = fwl.FreeRoundId
	WHERE (@Status IS NULL  OR frl.FRStatus = @Status)
	ORDER BY frl.FRStatus ASC, frl.StartDateUtc DESC
END

GO
/****** Object:  StoredProcedure [dbo].[FREEROUNDMEMBERWINLOSE]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FREEROUNDMEMBERWINLOSE]
	@FreeRoundId [int],
    @UserId [int]
AS
BEGIN
    SELECT WinLose = ISNULL(SUM(ISNULL(gh.Win,0)),0)
    FROM FreeRoundGameHistory AS fgh WITH (NOLOCK)
	INNER JOIN GameHistory gh WITH(NOLOCK) ON gh.Id = fgh.GameHistoryId
    INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
    WHERE gh.IsFreeGame = 1
		AND fgh.FreeRoundId = @FreeRoundId  
		AND gh.UserId = @UserId  
    GROUP BY u.Id
END



/****** Object:  StoredProcedure [dbo].[FREEROUNDREPORT] ******/

GO
/****** Object:  StoredProcedure [dbo].[FREEROUNDREPORT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FREEROUNDREPORT]
	@FreeRoundId [int],
	@Username [nvarchar](255)
AS
BEGIN	
	SELECT 	
		MemberId=u.Id,
		MemberName=u.Name, 
		GameName=g.Name, 
		Operator=o.Name, 
		Currency=c.IsoCode, 
		NoOfSpin=CONVERT(BIGINT,SUM(ISNULL(fri.TrxCount,0))), 
		NoOfUnclaimedFreeRounds=CONVERT(BIGINT,frd.[Counter]),
		TotalBet=CONVERT(DECIMAL,0), 			
		TotalBetL=CONVERT(DECIMAL,0),
		TotalWin=SUM(ISNULL(fri.Win,0)), 
		TotalWinL=SUM(ISNULL(fri.WinL,0)), 
		TotalSpinValueL = SUM(ISNULL(fri.BetL,0)),
		FirstBet=DATEADD(hh,8,MIN(fri.TimeFirstBet)), 
		LastBet=DATEADD(hh,8,MAX(fri.TimeLastBet))
	FROM [FreeRoundData] frd WITH(NOLOCK)
	LEFT JOIN [FreeRoundReportInfo] fri WITH(NOLOCK) ON frd.CampaignId=fri.FreeRoundId AND frd.UserId=fri.UserId
	INNER JOIN [GAME] g WITH (NOLOCK)ON frd.GameId = g.Id
	INNER JOIN [USER] u WITH(NOLOCK) ON frd.UserId=u.Id
	INNER JOIN [OPERATOR] o WITH (NOLOCK) ON u.OperatorId = o.Id
	INNER JOIN [CURRENCY] c WITH(NOLOCK) ON u.CurrencyId=c.Id
	WHERE frd.CampaignId=@FreeRoundId 
	AND (u.Name = @Username OR @Username IS NULL)
	GROUP BY frd.CampaignId, frd.GameId, frd.UserId, frd.[Counter], g.Name, u.Id, u.Name, o.Name, c.IsoCode
	ORDER BY u.Name
END



/****** Object:  StoredProcedure [dbo].[FRUSERHISTORY] ******/
-- MODIFY STORED PROCEDURE
-- CHANGED PARAMS

GO
/****** Object:  StoredProcedure [dbo].[FREEROUNDREPORT_dba]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[FREEROUNDREPORT_dba]
	@FreeRoundId [int],
	@Username [nvarchar](255)
AS
BEGIN
	;WITH ftemp AS (
		SELECT 
			GameId=frd.GameId, 
			UserId=frd.UserId, 
			TrxCount=SUM(ISNULL(fri.TrxCount,0)),
			Unclaimed=frd.[Counter],
			WinLose=SUM(ISNULL(fri.Win,0)), 
			WinLoseL=SUM(ISNULL(fri.WinL,0)), 
			Bet=CONVERT(DECIMAL,0),--SUM(ISNULL(fri.Bet,0)), 
			BetL=CONVERT(DECIMAL,0),--SUM(ISNULL(fri.BetL,0)), 
			TotalSpinValueL = SUM(ISNULL(fri.BetL,0)),
			FB=MIN(fri.TimeFirstBet), 
			LB=MAX(fri.TimeLastBet) 
		FROM [FreeRoundData] frd WITH(NOLOCK)
		LEFT JOIN [FreeRoundReportInfo] fri WITH(NOLOCK) 
			ON frd.CampaignId=fri.FreeRoundId AND frd.UserId=fri.UserId
		WHERE frd.CampaignId=@FreeRoundId
		GROUP BY frd.CampaignId, frd.GameId, frd.UserId, frd.[Counter]
	)
	SELECT 	
		GameId=g.Id, 
		GameName=g.Name, 
		MemberId=u.Id,
		MemberName=u.Name, 
		Operator=o.Name, 
		Currency=c.IsoCode, 
		NoOfSpin=CONVERT(BIGINT,f.TrxCount), 
		NoOfUnclaimedFreeRounds=CONVERT(BIGINT,f.Unclaimed),
		TotalBet=f.Bet, 
		TotalWin=f.WinLose, 
		TotalBetL=f.BetL,
		TotalSpinValueL = f.TotalSpinValueL,
		TotalWinL=f.WinLoseL,
		FirstBet=DATEADD(hh,8,f.FB), 
		LastBet=DATEADD(hh,8,f.LB) 
	FROM ftemp f
	INNER JOIN [Game] g WITH (NOLOCK)ON f.GameId = g.Id
	INNER JOIN [USER] u WITH(NOLOCK) ON f.UserId=u.Id
	INNER JOIN [Operator] o WITH (NOLOCK) ON u.OperatorId = o.Id
	INNER JOIN [CURRENCY] c WITH(NOLOCK) ON u.CurrencyId=c.Id
	WHERE (u.Name = @Username OR @Username IS NULL) 
	ORDER BY u.Name
END

GO
/****** Object:  StoredProcedure [dbo].[FREEROUNDREPORTINFOINSERT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE PROCEDURE [dbo].[FREEROUNDREPORTINFOINSERT]
    @FreeRoundId [int],
    @OperatorId [int],
    @UserId [int],
    @GameId [int],
    @Platform [int],
    @TrxCount [int],
    @Bet [decimal](23, 8),
    @Win [decimal](23, 8),
    @BetL [decimal](23, 8),
    @WinL [decimal](23, 8),
    @TimeFirstBet [datetime],
    @TimeLastBet [datetime]
AS
BEGIN
    DECLARE @ISNEW INT;
    
    	            BEGIN TRAN
    		            UPDATE FREEROUNDREPORTINFO WITH (SERIALIZABLE) SET
    		            TrxCount = TrxCount + @TrxCount,
    		            Bet = Bet + @Bet,
    		            Win = Win + @Win,
    		            BetL = BetL + @BetL,
    		            WinL = WinL + @WinL,
    		            TimeLastBet = @TimeLastBet
    		            WHERE FreeRoundId=@FreeRoundId AND OperatorId=@OperatorId AND UserId=@UserId AND GameId=@GameId AND [Platform]=@Platform;
    
    		            SET @ISNEW = @@ROWCOUNT;
    		
    		            IF (@ISNEW = 0)
    		            BEGIN
    			            INSERT INTO FREEROUNDREPORTINFO(FreeRoundId, OperatorId, UserId, GameId, [Platform], TrxCount, Bet, Win, BetL, WinL, TimeFirstBet, TimeLastBet) VALUES
    			            (@FreeRoundId, @OperatorId, @UserId, @GameId, @Platform, @TrxCount, @Bet, @Win, @BetL, @WinL, @TimeFirstBet, @TimeLastBet);
    		            END
    	            COMMIT TRAN
    
    	            RETURN @ISNEW
END

GO
/****** Object:  StoredProcedure [dbo].[FRUSERHISTORY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FRUSERHISTORY]
    @FreeRoundId [int],
    @UserId [int],
    @OffsetRows [int],
    @PageSize [int]
AS
BEGIN
    SELECT
		UserName = u.Name,
		GameTransactionId = gh.GameTransactionId,
		OperatorTag = o.Tag,
		GameName = g.Name,
		[Type] = gh.GameResultType,
		Currency = c.IsoCode,
		Bet = gh.Bet,
		Win = gh.Win,
		CreatedOnUtc = gh.DateTimeUtc
	FROM FreeRoundGameHistory fgh WITH (NOLOCK)	
	INNER JOIN GameHistory gh WITH(NOLOCK) ON gh.Id = fgh.GameHistoryId	            
	INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
	INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
	INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id
	INNER JOIN Operator o WITH (NOLOCK) ON o.Id = u.OperatorId
	WHERE fgh.FreeRoundId = @FreeRoundId
		AND gh.UserId = @UserId
		AND gh.IsFreeGame=1
	ORDER BY gh.DatetimeUtc DESC OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
END


-- MODIFY STORED PROCEDURE
-- ADD PARAM: @FreeRoundId [int]

GO
/****** Object:  StoredProcedure [dbo].[FRUSERHISTORY_dba]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- MODIFY STORED PROCEDURE
-- CHANGED PARAMS
CREATE PROCEDURE [dbo].[FRUSERHISTORY_dba]
    @FreeRoundId [int],
    @UserId [int],
    @OffsetRows [int],
    @PageSize [int]
AS
BEGIN
    ;WITH tem AS(
		SELECT
			gh.Id,
			gh.GameTransactionId,
			CreatedOnUtc = gh.DateTimeUtc,
			gh.GameResultType,
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
		WHERE gh.FreeRoundId = @FreeRoundId
			AND gh.UserId = @UserId
		AND gh.IsFreeGame=1
		ORDER BY gh.DatetimeUtc DESC OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
	)
    SELECT Id, GameTransactionId,CreatedOnUtc, Type=GameResultType, Bet, Win, UserId, UserName, Currency, GameId, GameName, OperatorTag, PlatformType FROM tem
END

GO
/****** Object:  StoredProcedure [dbo].[GAMEALERTREPORT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GAMEALERTREPORT]
	@OperatorId     INT = NULL,
	@StartDateInUTC DATETIME,
	@EndDateInUTC   DATETIME,
	@Limit          DECIMAL
AS
BEGIN
	SELECT
		g.Name Game,
		o.Name Operator, 
		SUM(CONVERT(BIGINT, TrxCount)) NoOfTransaction, 
		SUM(pri.TotalBetAmountRMB) TotalBetRMB, 
		SUM(pri.TotalWinAmountRMB) TotalWinRMB, 
		SUM(pri.TotalBetAmountRMB - pri.TotalWinAmountRMB) TotalNetWinRMB, 
		CASE SUM(pri.TotalBetAmountRMB) WHEN 0 THEN 1 ELSE SUM(pri.TotalBetAmountRMB - pri.TotalWinAmountRMB) / SUM(pri.TotalBetAmountRMB) END TotalNetWin
	FROM [dbo].[PlatformReportInfo] pri WITH(NOLOCK)
		INNER JOIN [dbo].[Game] g WITH(NOLOCK) ON pri.GameId = g.Id
		INNER JOIN [dbo].[User] u WITH(NOLOCK) ON pri.UserId = u.Id
		INNER JOIN [dbo].[Operator] o WITH(NOLOCK) ON u.OperatorId = o.Id
	WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
		AND (pri.TotalBetAmountRMB - pri.TotalWinAmountRMB) < @Limit
		GROUP BY g.Name, o.Name
		ORDER BY SUM(pri.TotalBetAmountRMB - pri.TotalWinAmountRMB) asc 
END

GO
/****** Object:  StoredProcedure [dbo].[GETACTIVETOURNAMENTSCONSOLIDATOR]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Vincente
-- Create date:	11 May 2017
-- Description:	Get active tournaments for Tournament Consolidator
-- Path: TournamentReportConsolidator
-- =============================================
CREATE PROCEDURE [dbo].[GETACTIVETOURNAMENTSCONSOLIDATOR]
AS
     BEGIN
         DECLARE @UtcNow DATETIME;
         DECLARE @StartCheck DATETIME;
         DECLARE @EndCheck DATETIME;
         SET @UtcNow = GETUTCDATE();
         SET @StartCheck = DATEADD(hh, -1, @UtcNow);
         SET @EndCheck = DATEADD(hh, 1, @UtcNow);
         SELECT *
         FROM TOURNAMENT WITH (NOLOCK)
         WHERE IsDeleted <> 1
               AND IsCancelled <> 1
               AND StartTime < @EndCheck
               AND EndTime > @StartCheck
         ORDER BY ABS(DATEDIFF(millisecond, StartTime, @UtcNow));
     END;

GO
/****** Object:  StoredProcedure [dbo].[GETHISTORY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

         CREATE PROCEDURE [dbo].[GETHISTORY]	
	            @UserId	INT,
	            @GameId INT
            AS
				BEGIN
					SELECT TOP 50 *
					FROM GameHistory WITH(NOLOCK)
					WHERE UserId = @UserId AND GameId = @GameId
					AND IsHistory = 1
					AND IsDeleted = 0
					ORDER BY Id DESC
					OPTION(RECOMPILE)
				END

GO

/****** Object:  StoredProcedure [dbo].[GETHISTORYCOUNT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GETHISTORYCOUNT]
@UserId	    INT,
@GameId		INT,
@Platform   INT,
@StartDate  DATETIME,
@EndDate	DATETIME
AS
BEGIN
SELECT COUNT(*) 
FROM GameHistory WITH(NOLOCK,index(IX_DateTimeUtc)) 
WHERE UserId=@UserId 
AND GameId=@GameId 
AND DateTimeUtc>=@StartDate AND DateTimeUtc<@EndDate 
AND IsHistory=1
AND (@Platform = 0 OR PlatformType % 10 = @Platform)
END;
GO
/****** Object:  StoredProcedure [dbo].[GETLEADERBOARDDETAIL]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vincente
-- Create date:	9 May 2017
-- Description:	Get Tournament leader board detail for Integration API
-- Path: Integration > tournamentdetail > type = 1
-- =============================================
CREATE PROCEDURE [dbo].[GETLEADERBOARDDETAIL] @TournamentId INT
AS
     BEGIN
         WITH tem1
              AS (SELECT TOP 100 Rank = CAST(RANK() OVER(ORDER BY SUM(TrxCount) DESC, SUM(BetL) DESC) AS  INT),
                                 UserId,
                                 Score = CAST(SUM(TrxCount) AS INT)
                  FROM TOURNAMENTREPORTINFO WITH (NOLOCK)
                  WHERE TournamentId = @TournamentId
                        AND Level = 15
                  GROUP BY UserId)
              SELECT t.Rank,
                     ui.Name,
                     t.Score
              FROM tem1 t
                   INNER JOIN [USER] ui WITH (NOLOCK) ON (t.UserId = ui.Id AND ui.IsDemo = 0)
                   INNER JOIN TOURNAMENT b WITH (NOLOCK) ON b.Id = @TournamentId
              WHERE t.Score >= b.MinHands;
     END;
GO
/****** Object:  StoredProcedure [dbo].[GETLEADERBOARDMEMBERDETAIL]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vincente
-- Create date:	10 May 2017
-- Description:	Get Tournament leader board detail for Integration API specific member
-- Path: Integration > tournamentdetail > type = 1 and member name
-- =============================================
CREATE PROCEDURE [dbo].[GETLEADERBOARDMEMBERDETAIL] @TournamentId INT, @MemberName nvarchar(255)
AS
     BEGIN
         WITH CTE
              AS (SELECT Rank = CAST(RANK() OVER(ORDER BY SUM(tri.TrxCount) DESC, SUM(tri.BetL) DESC) AS  INT),
                                 tri.UserId,
                                 Score = CAST(SUM(tri.TrxCount) AS INT)
                  FROM dbo.TournamentReportInfo tri WITH (NOLOCK)
                  WHERE tri.TournamentId = @TournamentId
                        AND tri.Level = 15
                  GROUP BY tri.UserId)
              SELECT t.Rank,
                     u.Name,
                     t.Score
              FROM CTE t
                   INNER JOIN dbo.[User] u WITH (NOLOCK) ON (t.UserId = u.Id AND u.IsDemo = 0)
                   INNER JOIN dbo.Tournament t2 WITH (NOLOCK) ON t2.Id = @TournamentId
              WHERE t.Score >= t2.MinHands AND u.Name = @MemberName;
     END;
GO
/****** Object:  StoredProcedure [dbo].[GETONREPORTINGTOURNAMENTS]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Vincente
-- Create date:	10 May 2017
-- Description:	Get on reporting tournaments for Tournament Consolidator
-- Path: DatabaseService.Tournament
-- =============================================
CREATE PROCEDURE [dbo].[GETONREPORTINGTOURNAMENTS]
AS
     BEGIN
         SELECT Id,
                Name,
                Description,
                OperatorId,
                StartTime = DATEADD(hh, 8, StartTime),
                EndTime = DATEADD(hh, 8, EndTime),
                Flags,
                MinHand = MinHands,
                Status =
         (
             SELECT 
				CASE
					WHEN IsCancelled = 1 THEN 4
				    WHEN GETUTCDATE() > EndTime THEN 1
					WHEN GETUTCDATE() >= StartTime AND GETUTCDATE() < EndTime THEN 2
					ELSE 3
				END
         )
         FROM TOURNAMENT WITH (NOLOCK)
         WHERE IsDeleted = 0
		 ORDER BY StartTime DESC;
     END;

GO
/****** Object:  StoredProcedure [dbo].[GETRANGEHISTORY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
					CREATE PROCEDURE [dbo].[GETRANGEHISTORY]
	            @UserId	    INT,
	            @GameId     INT,
	            @StartDate  DATETIME,
	            @EndDate    DATETIME,
				@Platform	INT,
	            @OffSetRows INT,
	            @PageSize   INT
            AS
            BEGIN
	            SELECT *
					FROM GameHistory WITH(NOLOCK)
					WHERE UserId=@UserId 
						AND GameId=@GameId 
						AND DateTimeUtc>=@StartDate AND DateTimeUtc<@EndDate
						AND IsHistory = 1
						AND IsDeleted = 0
						AND (@Platform = 0 OR PlatformType % 10 = @Platform)
					ORDER BY Id DESC 
					OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY 
            END

GO
/****** Object:  StoredProcedure [dbo].[GETTOURNAMENTDATA]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Vincente	
-- Create date:  May 2, 2017
-- Description: Get Tournament data
-- Path: Marketing > Tournament List
-- =============================================
CREATE PROCEDURE [dbo].[GETTOURNAMENTDATA] @StartDateInUtc DATETIME,
                                              @EndDateInUtc   DATETIME,
                                              @OperatorId     INT,
                                              @TournamentName NVARCHAR(128) = NULL,
                                              @Platform       NVARCHAR(128) = NULL
AS
     BEGIN
         SELECT 
                No = ROW_NUMBER() OVER(ORDER BY t.StartTime DESC),
                Id = t.Id,
                Name = t.Name,
                Description = t.[Description],
                StartTime = DATEADD(hh, 8, t.StartTime),
                EndTime = DATEADD(hh, 8, t.EndTime),
                Operator = o.NAME,
                [Owner] = a.UserName,
                [Status] = (CASE
                                WHEN t.IsCancelled = 1 THEN 4
                                WHEN GETUTCDATE() > t.EndTime THEN 1
                                WHEN GETUTCDATE() >= t.StartTime AND GETUTCDATE() < t.EndTime THEN 2
                                ELSE 3
                            END),
                Platforms =
         STUFF((
             SELECT ',' + CONVERT( VARCHAR(10), tr.RelationId)
             FROM TRelation tr
             WHERE tr.TournamentId = t.Id AND tr.RelationType = 4
             FOR XML PATH('')
         ), 1, 1, '')
         FROM dbo.Tournament t WITH (NOLOCK)
              INNER JOIN dbo.Operator o WITH (NOLOCK) ON t.OperatorId = o.id
              INNER JOIN dbo.Account a WITH (NOLOCK) ON t.OwnerId = a.id
              INNER JOIN dbo.TRelation tr WITH (NOLOCK) ON t.Id = tr.TournamentId
                                                           AND tr.RelationType = 4
         WHERE t.IsDeleted != 1
               AND tr.RelationId IN
         (
             SELECT *
             FROM dbo.fnSplitString(@Platform, ',')
         )
              AND ((@StartDateInUTC IS NULL OR @EndDateInUTC IS NULL) OR (t.StartTime BETWEEN @StartDateInUTC AND @EndDateInUTC))
              AND (@OperatorId IS NULL OR t.OperatorId = @OperatorId)
		    AND (@TournamentName IS NULL OR t.Name LIKE '%'+@TournamentName+'%')
	GROUP BY t.Id, t.Name, t.[Description], t.StartTime, t.EndTime, o.Name, a.Username, t.IsCancelled
     END;

GO
/****** Object:  StoredProcedure [dbo].[GETTOURNAMENTDATAGETONREPORTINGTOURNAMENTS]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Vincente
-- Create date:	10 May 2017
-- Description:	Get on reporting tournaments for Tournament Consolidator
-- Path: DatabaseService.Tournament
-- =============================================
CREATE PROCEDURE [dbo].[GETTOURNAMENTDATAGETONREPORTINGTOURNAMENTS]
AS
     BEGIN
         SELECT Id,
                Name,
                Description,
                OperatorId,
                StartTime = DATEADD(hh, 8, StartTime),
                EndTime = DATEADD(hh, 8, EndTime),
                Flags,
                MinHand = MinHands,
                Status =
         (
             SELECT CASE
                        WHEN IsCancelled = 1 THEN 4
				    WHEN GETUTCDATE() > EndTime THEN 1
                        WHEN GETUTCDATE() >= StartTime AND GETUTCDATE() < EndTime THEN 2
                        ELSE 3
                    END
         )
         FROM TOURNAMENT WITH (NOLOCK)
         WHERE IsDeleted = 0
               AND DATEDIFF(dd, EndTime, GETUTCDATE()) < 2
         ORDER BY StartTime DESC;
     END;

GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
         CREATE PROCEDURE [dbo].[GETTRANSACTIONHISTORY]	
	            @OperatorTag	NVARCHAR(16),
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
	                DECLARE	@OperatorID INT
	                SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)

	                SELECT *, COUNT(*) OVER () as TotalRecords
	                FROM
	                (
		                SELECT	OperationCode = gh.GameTransactionId,
			                UserId = u.ExternalId, 
			                CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'),'+08:00'),0) AS [ChangeTime],
			                CASE gh.GameResultType 
							    WHEN 1 THEN 'Result' 
							    WHEN 2 THEN 'Gamble' 
							    WHEN 3 THEN 'Free Spin' 
							    WHEN 4 THEN 'Bonus' 
							    WHEN 5 THEN 'Bonus' 
							    WHEN 6 THEN 'Gamble' 
							    WHEN 7 THEN 'Bonus' 
								WHEN 8 THEN 'Free Spin'
						    END AS [ChangeType],
			                GameName = gi.Name, 
							IsFreeRound = gh.IsFreeGame,
							Bet = IIF(gh.IsFreeGame=1, 0, IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)),
			                [Return] = gh.Win, 
			                [Changes] = gh.win - IIF(gh.IsFreeGame=1, 0, IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)),
			                EndBalance = 0.0,
			                Operator = @OperatorTag,
			                TransactionId = '',
							JackpotCon = 0.0,
			                JackpotWin = 0.0,
			                [Platform] = gh.PlatformType,
			                [Version] = 1
					    FROM	[dbo].GameHistory gh WITH(NOLOCK,index(IX_DateTimeUtc))
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                WHERE u.OperatorId = @OperatorID
			                AND gh.IsReport = 1
                            AND gh.DateTimeUtc BETWEEN @StartDateInUTC AND @EndDateInUTC
			                AND gh.IsDeleted = 0
	                ) AS RESULT
	                ORDER BY OperationCode
	                OFFSET @OffsetRows ROWS
	                FETCH NEXT @PageSize ROWS ONLY
                END
GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY_01]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
			CREATE PROCEDURE [dbo].[GETTRANSACTIONHISTORY_01]	
	            @OperatorTag	NVARCHAR(16),
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
	                DECLARE	@OperatorID INT
	                SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)

		                SELECT	OperationCode = gh.GameTransactionId,
			                UserId = u.ExternalId, 
			                CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'),'+08:00'),0) AS [ChangeTime],
			                CASE gh.GameResultType 
							    WHEN 1 THEN 'Result' 
							    WHEN 2 THEN 'Gamble' 
							    WHEN 3 THEN 'Free Spin' 
							    WHEN 4 THEN 'Bonus' 
							    WHEN 5 THEN 'Bonus' 
							    WHEN 6 THEN 'Gamble' 
							    WHEN 7 THEN 'Bonus' 
						    END AS [ChangeType],
			                GameName = gi.Name, 
			                Bet = IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet),
			                [Return] = gh.Win, 
			                [Changes] = gh.win - IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet),
			                EndBalance = 0.0,
			                Operator = @OperatorTag,
			                --TransactionId = ISNULL(wt1.WalletProviderTransactionId,'') + ',' + ISNULL(wt2.WalletProviderTransactionId,''),
			                TransactionId = '',
							JackpotCon = 0.0,
			                JackpotWin = 0.0,
			                [Platform] = gh.PlatformType,
			                [Version] = 1
					    FROM	[dbo].GameHistory gh WITH(NOLOCK)
		                --INNER JOIN	[dbo].[GameHistory] gh WITH(NOLOCK) ON gh.GameTransactionId = gt.Id
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                --LEFT JOIN [dbo].[WalletTransaction] wt1 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt1.GameTransactionId = gt.Id AND wt1.[Type] = 1
		                --LEFT JOIN [dbo].[WalletTransaction] wt2 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt2.GameTransactionId = gt.Id AND wt2.[Type] = 2
		                WHERE u.OperatorId = @OperatorID
			                AND gh.IsReport = 1
                            AND gh.DateTimeUtc BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
			                AND gh.IsDeleted = 0
                END
GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY_20160805]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
            CREATE PROCEDURE [dbo].[GETTRANSACTIONHISTORY_20160805]	
	            @OperatorTag	NVARCHAR(16),
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
	                DECLARE	@OperatorID INT
	                SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)

	                SELECT *, COUNT(*) OVER () as TotalRecords
	                FROM
	                (
		                SELECT	OperationCode = gt.Id,
			                UserId = u.ExternalId, 
			                CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'),'+08:00'),0) AS [ChangeTime],
			                CASE gh.GameResultType 
							    WHEN 1 THEN 'Result' 
							    WHEN 2 THEN 'Gamble' 
							    WHEN 3 THEN 'Free Spin' 
							    WHEN 4 THEN 'Bonus' 
							    WHEN 5 THEN 'Bonus' 
							    WHEN 6 THEN 'Gamble' 
							    WHEN 7 THEN 'Bonus' 
						    END AS [ChangeType],
			                GameName = gi.Name, 
			                Bet = IIF(sbp.IsSideBet=1,gh.bet*2,gh.bet),
			                [Return] = gh.Win, 
			                [Changes] = gh.win - IIF(sbp.IsSideBet=1,gh.bet*2,gh.bet),
			                EndBalance = 0.0,
			                Operator = @OperatorTag,
			                TransactionId = ISNULL(wt1.WalletProviderTransactionId,'') + ',' + ISNULL(wt2.WalletProviderTransactionId,''),
			                JackpotCon = 0.0,
			                JackpotWin = 0.0,
			                [Platform] = 1,
			                [Version] = 1
					    FROM	[dbo].[GameTransaction] gt WITH(NOLOCK)
		                INNER JOIN	[dbo].[GameHistory] gh WITH(NOLOCK) ON gh.GameTransactionId = gt.Id
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                LEFT JOIN	[dbo].[WalletTransaction] wt1 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt1.GameTransactionId = gh.Id AND wt1.[Type] = 1
		                LEFT JOIN	[dbo].[WalletTransaction] wt2 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt2.GameTransactionId = gh.Id AND wt2.[Type] = 2
		                WHERE u.OperatorId = @OperatorID
			                AND gh.IsReport = 1
			                AND gh.DateTimeUtc BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
			                AND gh.IsDeleted = 0
	                ) AS RESULT
	                ORDER BY OperationCode
	                OFFSET @OffsetRows ROWS
	                FETCH NEXT @PageSize ROWS ONLY
                END
GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY_20160805_2]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
      
			
			CREATE PROCEDURE [dbo].[GETTRANSACTIONHISTORY_20160805_2]	
	            @OperatorTag	NVARCHAR(16),
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
	                DECLARE	@OperatorID INT
	                SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)

	                SELECT *, COUNT(*) OVER () as TotalRecords
	                FROM
	                (
		                SELECT	OperationCode = gt.Id,
			                UserId = u.ExternalId, 
			                CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'),'+08:00'),0) AS [ChangeTime],
			                CASE gh.GameResultType 
							    WHEN 1 THEN 'Result' 
							    WHEN 2 THEN 'Gamble' 
							    WHEN 3 THEN 'Free Spin' 
							    WHEN 4 THEN 'Bonus' 
							    WHEN 5 THEN 'Bonus' 
							    WHEN 6 THEN 'Gamble' 
							    WHEN 7 THEN 'Bonus' 
						    END AS [ChangeType],
			                GameName = gi.Name, 
			                Bet = IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet),
			                [Return] = gh.Win, 
			                [Changes] = gh.win - IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet),
			                EndBalance = 0.0,
			                Operator = @OperatorTag,
			                TransactionId = ISNULL(wt1.WalletProviderTransactionId,'') + ',' + ISNULL(wt2.WalletProviderTransactionId,''),
			                JackpotCon = 0.0,
			                JackpotWin = 0.0,
			                [Platform] = 1,
			                [Version] = 1
					    FROM	[dbo].[GameTransaction] gt WITH(NOLOCK)
		                INNER JOIN	[dbo].[GameHistory] gh WITH(NOLOCK) ON gh.GameTransactionId = gt.Id
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                LEFT JOIN [dbo].[WalletTransaction] wt1 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt1.GameTransactionId = gh.Id AND wt1.[Type] = 1
		                LEFT JOIN [dbo].[WalletTransaction] wt2 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt2.GameTransactionId = gh.Id AND wt2.[Type] = 2
		                WHERE u.OperatorId = @OperatorID
			                AND gh.IsReport = 1
                            AND gh.DateTimeUtc BETWEEN @StartDateInUTC AND @EndDateInUTC
			                AND gh.IsDeleted = 0
	                ) AS RESULT
	                ORDER BY OperationCode
	                OFFSET @OffsetRows ROWS
	                FETCH NEXT @PageSize ROWS ONLY
                END
GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY_20161215]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
            create PROCEDURE [dbo].[GETTRANSACTIONHISTORY_20161215]	
	            @OperatorTag	NVARCHAR(16),
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
	                DECLARE	@OperatorID INT
	                SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)

	                SELECT *, COUNT(*) OVER () as TotalRecords
	                FROM
	                (
		                SELECT	OperationCode = gh.GameTransactionId,
			                UserId = u.ExternalId, 
			                CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'),'+08:00'),0) AS [ChangeTime],
			                CASE gh.GameResultType 
							    WHEN 1 THEN 'Result' 
							    WHEN 2 THEN 'Gamble' 
							    WHEN 3 THEN 'Free Spin' 
							    WHEN 4 THEN 'Bonus' 
							    WHEN 5 THEN 'Bonus' 
							    WHEN 6 THEN 'Gamble' 
							    WHEN 7 THEN 'Bonus' 
						    END AS [ChangeType],
			                GameName = gi.Name, 
							Bet = IIF(gh.IsFreeGame=1, 0, IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)),
			                [Return] = gh.Win, 
			                [Changes] = gh.win - IIF(gh.IsFreeGame=1, 0, IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)),
			                EndBalance = 0.0,
			                Operator = @OperatorTag,
			                TransactionId = '',
							JackpotCon = 0.0,
			                JackpotWin = 0.0,
			                [Platform] = gh.PlatformType,
			                [Version] = 1
					    FROM	[dbo].GameHistory gh WITH(NOLOCK)
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                WHERE u.OperatorId = @OperatorID
			                AND gh.IsReport = 1
                            AND gh.DateTimeUtc BETWEEN @StartDateInUTC AND @EndDateInUTC
			                AND gh.IsDeleted = 0
	                ) AS RESULT
	                ORDER BY OperationCode
	                OFFSET @OffsetRows ROWS
	                FETCH NEXT @PageSize ROWS ONLY
                END

GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY_21060804]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GETTRANSACTIONHISTORY_21060804]	
	            @OperatorTag	NVARCHAR(16),
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
	                DECLARE	@OperatorID INT
	                SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)
					
					;WITH tem AS
					(
						SELECT	gh.Id, gh.GameTransactionId, gh.UserId, gh.GameId,
							CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'),'+08:00'),0) AS [ChangeTime],
							CASE gh.GameResultType 
								WHEN 1 THEN 'Result' 
								WHEN 2 THEN 'Gamble' 
								WHEN 3 THEN 'Free Spin' 
								WHEN 4 THEN 'Bonus' 
								WHEN 5 THEN 'Bonus' 
								WHEN 6 THEN 'Gamble' 
								WHEN 7 THEN 'Bonus' 
								WHEN 8 THEN 'Free Spin' 
							END AS [ChangeType],
							Bet = gh.Bet, 
							[Return] = gh.Win, 
							[Changes] = (gh.Win - gh.Bet),
							EndBalance = 0.0,
							Operator = @OperatorTag,
							JackpotCon = 0.0,
							JackpotWin = 0.0,
							[Platform] = gh.PlatformType,
							[Version] = 1
		               FROM	[dbo].[GameHistory] gh WITH(NOLOCK)
					   WHERE gh.DateTimeUtc BETWEEN @StartDateInUTC AND @EndDateInUTC 
			                AND gh.IsReport = 1
			                AND gh.IsDeleted = 0
					)
	                SELECT *, COUNT(*) OVER () as TotalRecords
	                FROM
	                (
							SELECT	OperationCode = gt.Id,
								UserId = u.ExternalId, 
								gh.[ChangeTime],
								gh.[ChangeType],
								GameName = gi.Name, 
								gh.Bet, 
								gh.[Return], 
								gh.[Changes],
								EndBalance = 0.0,
								Operator = @OperatorTag,
								TransactionId = ISNULL(wt1.WalletProviderTransactionId,'') + ',' + ISNULL(wt2.WalletProviderTransactionId,''),
								JackpotCon = 0.0,
								JackpotWin = 0.0,
								gh.[Platform],
								GameType = gi.GameType,
								[Version] = 1
							FROM	[dbo].[GameTransaction] gt WITH(NOLOCK)
		                INNER JOIN	tem gh WITH(NOLOCK) ON gh.GameTransactionId = gt.Id
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                LEFT JOIN	[dbo].[WalletTransaction] wt1 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt1.GameTransactionId = gh.Id AND wt1.[Type] = 1
		                LEFT JOIN	[dbo].[WalletTransaction] wt2 WITH(NOLOCK) ON wt2.GameTransactionId = gh.Id AND wt2.[Type] = 2
		                WHERE u.OperatorId = @OperatorID
	                ) AS RESULT
	                ORDER BY OperationCode
	                OFFSET @OffsetRows ROWS
	                FETCH NEXT @PageSize ROWS ONLY
					
                END
GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY_new]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[GETTRANSACTIONHISTORY_new]	
	            @OperatorTag	NVARCHAR(16),
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
	                DECLARE	@OperatorID INT
	                SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)

	                SELECT OperationCode,UserId,ChangeTime,ChangeType,GameName,Bet,[Return],[Changes],EndBalance,Operator,TransactionId,JackpotCon,JackpotWin,Platform,Version, COUNT(*) OVER () as TotalRecords
	                FROM
	                (
		                SELECT gh.Id, OperationCode = gh.GameTransactionId,
			                UserId = u.ExternalId, 
			                CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'),'+08:00'),0) AS [ChangeTime],
			                CASE gh.GameResultType 
							    WHEN 1 THEN 'Result' 
							    WHEN 2 THEN 'Gamble' 
							    WHEN 3 THEN 'Free Spin' 
							    WHEN 4 THEN 'Bonus' 
							    WHEN 5 THEN 'Bonus' 
							    WHEN 6 THEN 'Gamble' 
							    WHEN 7 THEN 'Bonus' 
						    END AS [ChangeType],
			                GameName = gi.Name, 
			                Bet = IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet),
			                [Return] = gh.Win, 
			                [Changes] = gh.win - IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet),
			                EndBalance = 0.0,
			                Operator = @OperatorTag,
			                --TransactionId = ISNULL(wt1.WalletProviderTransactionId,'') + ',' + ISNULL(wt2.WalletProviderTransactionId,''),
			                TransactionId = '',
							JackpotCon = 0.0,
			                JackpotWin = 0.0,
			                [Platform] = gh.PlatformType,
			                [Version] = 1
					    FROM	[dbo].GameHistory gh WITH(NOLOCK)
		                --INNER JOIN	[dbo].[GameHistory] gh WITH(NOLOCK) ON gh.GameTransactionId = gt.Id
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                --LEFT JOIN [dbo].[WalletTransaction] wt1 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt1.GameTransactionId = gt.Id AND wt1.[Type] = 1
		                --LEFT JOIN [dbo].[WalletTransaction] wt2 WITH(NOLOCK,INDEX(IX_GameTransactionId)) ON wt2.GameTransactionId = gt.Id AND wt2.[Type] = 2
		                WHERE u.OperatorId = @OperatorID
			                AND gh.IsReport = 1
                            AND gh.DateTimeUtc BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
			                AND gh.IsDeleted = 0
	                ) AS RESULT
	                ORDER BY Id
	                OFFSET @OffsetRows ROWS
	                FETCH NEXT @PageSize ROWS ONLY
                END
GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY_old]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GETTRANSACTIONHISTORY_old]	
	            @OperatorTag	NVARCHAR(16),
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
	                DECLARE	@OperatorID INT
	                SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)
					/*
	                SELECT *, COUNT(*) OVER () as TotalRecords
	                FROM
	                (
		                SELECT	OperationCode = gt.Id,
			                UserId = u.ExternalId, 
			                CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'),'+08:00'),0) AS [ChangeTime],
			                CASE gh.GameResultType 
							    WHEN 1 THEN 'Result' 
							    WHEN 2 THEN 'Gamble' 
							    WHEN 3 THEN 'Free Spin' 
							    WHEN 4 THEN 'Bonus' 
							    WHEN 5 THEN 'Bonus' 
							    WHEN 6 THEN 'Gamble' 
							    WHEN 7 THEN 'Bonus' 
							    WHEN 8 THEN 'Free Spin' 
						    END AS [ChangeType],
			                GameName = gi.Name, 
			                Bet = gh.Bet, 
			                [Return] = gh.Win, 
			                [Changes] = (gh.Win - gh.Bet),
			                EndBalance = 0.0,
			                Operator = @OperatorTag,
			                TransactionId = ISNULL(wt1.WalletProviderTransactionId,'') + ',' + ISNULL(wt2.WalletProviderTransactionId,''),
			                JackpotCon = 0.0,
			                JackpotWin = 0.0,
			                [Platform] = gh.PlatformType,
			                [Version] = 1
					    FROM	[dbo].[GameTransaction] gt WITH(NOLOCK)
		                INNER JOIN	[dbo].[GameHistory] gh WITH(NOLOCK) ON gh.GameTransactionId = gt.Id
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                LEFT JOIN	[dbo].[WalletTransaction] wt1 WITH(NOLOCK) ON wt1.GameTransactionId = gh.Id AND wt1.[Type] = 1
		                LEFT JOIN	[dbo].[WalletTransaction] wt2 WITH(NOLOCK) ON wt2.GameTransactionId = gh.Id AND wt2.[Type] = 2
		                WHERE u.OperatorId = @OperatorID
			                AND gh.IsReport = 1
			                AND gh.DateTimeUtc BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
			                AND gh.IsDeleted = 0
	                ) AS RESULT
	                ORDER BY OperationCode
	                OFFSET @OffsetRows ROWS
	                FETCH NEXT @PageSize ROWS ONLY
					*/
                END
GO
/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONSUMMARYBYCURRENCY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GETTRANSACTIONSUMMARYBYCURRENCY]
    @OperatorTag [nvarchar](16),
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @AccType [nvarchar](16)
AS
BEGIN
    
    DECLARE	@OperatorID INT
    	            SET @OperatorID = (SELECT Id FROM [dbo].[Operator] WHERE Tag = @OperatorTag)
    					
    	            DECLARE	@IsDemo BIT
    		        IF @AccType			= 'demo'	SET @IsDemo = 1
    		        ELSE IF @AccType	= 'real'	SET @IsDemo = 0
    		        ELSE							SET @IsDemo = NULL
    
    		        SELECT CurrencyId, 
    			        Currency = c.IsoCode, 
    			        UAP = COUNT(DISTINCT pri.UserId),
    			        TrxCount = SUM(pri.TrxCount),        
    			        Bet = SUM(IIF(pri.IsFreeGame=1, 0, pri.TotalBetAmount)),
    			        NetWin = SUM(IIF(pri.IsFreeGame=1, pri.TotalNetWinAmount+pri.TotalBetAmount, pri.TotalNetWinAmount)),
    			        BetRMB = SUM(IIF(pri.IsFreeGame=1, 0, pri.TotalBetAmountRMB)),
    			        NetWinRMB = SUM(IIF(pri.IsFreeGame=1, pri.TotalNetWinAmountRMB+pri.TotalBetAmountRMB, pri.TotalNetWinAmountRMB))
    		        FROM	[dbo].[PlatformReportInfo] pri WITH (NOLOCK) 
    		        INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = pri.UserId
    		        INNER JOIN	[dbo].[Currency] c WITH (NOLOCK) ON c.Id = u.CurrencyId
    		        WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    			        AND u.OperatorId = @OperatorID
    			        AND pri.ChangeTime >= @StartDateInUTC
    			        AND pri.ChangeTime < @EndDateInUTC
    			        AND OffsetId = 42
    		        GROUP BY u.CurrencyId, c.IsoCode
END

GO
/****** Object:  StoredProcedure [dbo].[GETTXNSUMMARYBYCURRENCY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GETTXNSUMMARYBYCURRENCY]
    @StartDateUTC [datetime],
    @EndDateUTC [datetime],
    @AccType [nvarchar](16)
AS
BEGIN
    
    DECLARE	@IsDemo BIT
    	            IF @AccType	     = 'demo' SET @IsDemo = 1
    	            ELSE IF @AccType = 'real' SET @IsDemo = 0
    	            ELSE SET @IsDemo = NULL
    
    	            SELECT 
    		            Operator=o.Tag, 
    		            Currency=c.IsoCode, 
    		            UAP = COUNT(DISTINCT pri.UserId),
    		            TrxCount=SUM(pri.TrxCount), 
    		            Bet = SUM(IIF(pri.IsFreeGame=1, 0, pri.TotalBetAmount)),
    		            NetWin = SUM(IIF(pri.IsFreeGame=1, pri.TotalNetWinAmount+pri.TotalBetAmount, pri.TotalNetWinAmount)),
    		            BetRMB = SUM(IIF(pri.IsFreeGame=1, 0, pri.TotalBetAmountRMB)),
    		            NetWinRMB = SUM(IIF(pri.IsFreeGame=1, pri.TotalNetWinAmountRMB+pri.TotalBetAmountRMB, pri.TotalNetWinAmountRMB))
    	            FROM [dbo].[PLATFORMREPORTINFO] pri WITH(NOLOCK) 
    	            INNER JOIN [dbo].[USER] u WITH(NOLOCK) ON u.Id=pri.UserId
    	            INNER JOIN [dbo].[CURRENCY] c WITH(NOLOCK) ON c.Id=u.CurrencyId
    	            INNER JOIN [dbo].[OPERATOR] o WITH(NOLOCK) ON o.Id=u.OperatorId
    	            WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    		            AND pri.ChangeTime >= @StartDateUTC
    		            AND pri.ChangeTime < @EndDateUTC
    		            AND OffsetId = 42
    	            GROUP BY o.Tag, c.IsoCode
    	            ORDER BY o.Tag, c.IsoCode
END



GO
/****** Object:  StoredProcedure [dbo].[GETUNFINISHEDGAMES]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GETUNFINISHEDGAMES]
	@operatorId int,
	@memberId nvarchar(255)
AS
BEGIN
	;WITH Merchant(Id) AS (
	        SELECT ur.Id FROM [dbo].[User] ur WITH(NOLOCK)
	        INNER JOIN  [dbo].[Operator] opr WITH(NOLOCK) ON opr.Id = ur.OperatorId
	        WHERE opr.[Id] = @operatorId AND ur.[ExternalId]=@memberId AND ur.[IsDeleted]=0 AND ur.[IsBlocked]=0 AND opr.[IsDeleted]=0
      ) 
	  SELECT ge.[Name] AS [Game], bs.[CreatedOnUtc] AS [CreatedTime], CAST(ISNULL(bs.[UpdatedOnUtc], '1900-01-01') AS DATETIME) AS [UpdatedTime] 
      FROM [dbo].[Bonus] bs WITH(NOLOCK)
      INNER JOIN [dbo].[Game] ge WITH(NOLOCK) ON bs.GameId =ge.Id 
      INNER JOIN Merchant mer WITH(NOLOCK) ON bs.UserId=mer.Id
      WHERE ge.[IsDeleted] = 0 
	  AND (bs.BonusType NOT IN('GambleBonus','DoubleUpBonus','DoubleUpSmallBigBonus') OR bs.IsStarted = 1)
END
GO
/****** Object:  StoredProcedure [dbo].[INITIALIZEFREEGAMEDATA]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[INITIALIZEFREEGAMEDATA]
    @CampaignId [int],
    @UserId [int],
    @GameId [int],
    @OperatorId [int]
AS
BEGIN
    
    	            -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    	            SET NOCOUNT ON;
    
    	            DECLARE @Now  DATETIME =  GEtUTCDATE();
    DECLARE @Bet  DECIMAL(23,8);
    DECLARE @InitializeFreeGameData VARCHAR(64) = N'InitializeFreeGameData';  
    
    BEGIN TRANSACTION @InitializeFreeGameData;    
    BEGIN TRY  
    
    ;WITH FreeGame(
    		                [CampaignId],
    		                [UserId],
    		                [GameId],
    		                [TimeStart],
    		                [TimeEnd],
    		                [Bet],
    		                [Multiplier],
    		                [Lines],
    		                [Counter],
    		                [State],
    		                [IsFinish],
    		                [CreatedBy]
    ) AS (SELECT 
    				                fg.Id									AS [CampaignId],
    				                @UserId									AS [UserId],
    				                @GameId									AS [GameId],
    				                fg.StartDateUtc							AS [TimeStart], 
    				                fg.EndDateUtc							AS [TimeEnd],
    				                (SELECT TOP 1 
    					                fgcs.[LineBet] 
    				                FROM [dbo].[FRCoinSetting] fgcs
    				                WHERE	
    					                fgcs.CurrencyId	= u.CurrencyId AND 
    					                fgcs.[FreeRoundId]	= fg.Id					
    				                )										AS [Bet],  
    				                fg.Multiplier							AS [Multiplier],
    				                fg.Lines								AS [Lines],
    				                fg.LimitPerPlayer						AS [Counter],
    				                2										AS [State], -- (State = Active)
    				                0										AS [IsFinish],
    				                fg.CreatedBy							AS [CreatedBy]
    				
    			                FROM [dbo].[FreeRound] fg WITH(NOLOCK)
    			                INNER JOIN [dbo].[FRPlayer] fgp WITH(NOLOCK) ON fgp.FreeRoundId  = fg.Id 
    			                INNER JOIN [dbo].[User] u WITH(NOLOCK) ON fgp.UserId  = u.Id --AND u.OperatorId = @OperatorId
    			                WHERE 
    				                fgp.UserId		= @UserId		AND 
    				                fg.GameId		= @GameId		AND
    				                fg.OperatorId	= @OperatorId	AND
    				                fg.IsDeleted	= 0				AND
    fg.IsCancelled  = 0				
    					            AND fg.Id = @CampaignId)
    
    INSERT INTO [dbo].[FreeRoundData]
    ([CampaignId],
    		                [UserId],
    		                [GameId],
    		                [TimeStart],
    		                [TimeEnd],
    		                [Bet],
    		                [Multiplier],
    		                [Lines],
    		                [Counter],
    		                [State],
    		                [IsFinish],
    		                [CreatedBy])
    SELECT * FROM FreeGame;
    	                -- commit transaction
    	                COMMIT TRANSACTION @InitializeFreeGameData
    END TRY  
    BEGIN CATCH  
    SELECT   
    ERROR_NUMBER() AS ErrorNumber  
    ,ERROR_SEVERITY() AS ErrorSeverity  
    ,ERROR_STATE() AS ErrorState  
    ,ERROR_PROCEDURE() AS ErrorProcedure  
    ,ERROR_LINE() AS ErrorLine  
    ,ERROR_MESSAGE() AS ErrorMessage; 
    ROLLBACK TRANSACTION @InitializeFreeGameData;  
    END CATCH;
END

GO
/****** Object:  StoredProcedure [dbo].[INITIALIZEFREEROUNDDATA]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[INITIALIZEFREEROUNDDATA] @CampaignId INT
AS
     BEGIN
	SET NOCOUNT ON;
         DECLARE @Now DATETIME= GETUTCDATE();
         DECLARE @Bet DECIMAL(8, 4);
             -- Delete from FreeRoundData
             DELETE FROM dbo.FreeRoundData
             WHERE CampaignId = @CampaignId;
		   
             WITH FreeGame([CampaignId],
                           [UserId],
                           [GameId],
                           [TimeStart],
                           [TimeEnd],
                           [Bet],
                           [Multiplier],
                           [Lines],
                           [Counter],
                           [State],
                           [IsFinish],
                           [CreatedBy])
                  AS (SELECT fg.Id AS [CampaignId],
                             fgp.UserId AS [UserId],
                             fg.GameId AS [GameId],
                             fg.StartDateUtc AS [TimeStart],
                             fg.EndDateUtc AS [TimeEnd],
                      b.LineBet AS [Bet],
                             fg.Multiplier AS [Multiplier],
                             fg.Lines AS [Lines],
                             fg.LimitPerPlayer AS [Counter],
                             2 AS [State], -- (State 2 = Active)
                             0 AS [IsFinish],
                             fg.CreatedBy AS [CreatedBy]
                      FROM [dbo].[FreeRound] fg WITH (NOLOCK)
                           INNER JOIN [dbo].[FRPlayer] fgp WITH (NOLOCK) ON fgp.FreeRoundId = fg.Id
                           INNER JOIN [dbo].[User] u WITH (NOLOCK) ON fgp.UserId = u.Id
					  CROSS APPLY (
					  SELECT TOP 1 fgcs.[LineBet]
                          FROM [dbo].[FRCoinSetting] fgcs
                          WHERE fgcs.CurrencyId = u.CurrencyId
                                AND fgcs.[FreeRoundId] = fg.Id
					  ) b
                      WHERE fg.IsDeleted = 0
                            AND fg.IsCancelled = 0
                            AND fg.Id = @CampaignId)

                  -- Insert into FreeRoundData
                  INSERT INTO [dbo].[FreeRoundData]
                  ([CampaignId],
                   [UserId],
                   [GameId],
                   [TimeStart],
                   [TimeEnd],
                   [Bet],
                   [Multiplier],
                   [Lines],
                   [Counter],
                   [State],
                   [IsFinish],
                   [CreatedBy]
                  )
                         SELECT *
                         FROM FreeGame;
     END;

GO
/****** Object:  StoredProcedure [dbo].[MEMBERALERTREPORT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MEMBERALERTREPORT]
    @OperatorId     INT = NULL,
    @StartDateInUTC DATETIME,
    @EndDateInUTC   DATETIME,
    @Limit          DECIMAL
AS
BEGIN
    SELECT
		u.Name Member,
		o.Name Operator,
		g.Name Game, 
		SUM(CONVERT(BIGINT, TrxCount)) NoOfTransaction, 
		SUM(pri.TotalBetAmountRMB) TotalBetRMB, 
		SUM(pri.TotalWinAmountRMB) TotalWinRMB, 
		SUM(pri.TotalBetAmountRMB - pri.TotalWinAmountRMB) TotalNetWinRMB, 
		CASE SUM(pri.TotalBetAmountRMB) WHEN 0 THEN 1 ELSE SUM(pri.TotalBetAmountRMB - pri.TotalWinAmountRMB) / SUM(pri.TotalBetAmountRMB) END TotalNetWin
    FROM [dbo].[PlatformReportInfo] pri WITH(NOLOCK)
		INNER JOIN [dbo].[Game] g WITH(NOLOCK) ON pri.GameId = g.Id
		INNER JOIN [dbo].[User] u WITH(NOLOCK) ON pri.UserId = u.Id
		INNER JOIN [dbo].[Operator] o WITH(NOLOCK) ON u.OperatorId = o.Id
	WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
		AND (pri.TotalBetAmountRMB - pri.TotalWinAmountRMB) < @Limit
        GROUP BY u.name,o.Name,g.Name
		ORDER BY SUM(pri.TotalBetAmountRMB - pri.TotalWinAmountRMB) asc 
END
GO
/****** Object:  StoredProcedure [dbo].[MEMBERPERFORMANCEREPORT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kaidan Joseph	
-- Create date: January 10, 2017
-- Description:	Get the performance report of member, the Platform value can be having.
--				1. Web + WebLD
--				2. Desktop
--				3. Mobile
--				4. Mini
				
-- =============================================
CREATE PROCEDURE [dbo].[MEMBERPERFORMANCEREPORT]
	@StartDate DATETIME,
	@EndDate DATETIME , 
	@ExternalId NVARCHAR(256), 
	@OperatorId INT,
	@IsFreeRound BIT = NULL, 
	@Platform INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE 
			@MemberId  INT, 
			@Currency NVARCHAR(8);
	DECLARE
			@table TABLE (UserId INT, Currency NVARCHAR(8), Bet DECIMAL, NetWin DECIMAL);

	;WITH UserInfo AS (
		SELECT TOP 1
			u.Id,
			u.CurrencyId
		FROM
			[User] u WITH(NOLOCK) 
		WHERE 
			u.ExternalId = @ExternalId AND 
			u.OperatorId  = @OperatorId)
	SELECT 
		@MemberId = ui.Id,
		@Currency = (SELECT TOP 1  c.IsoCode
			FROM 
				Currency c WITH(NOLOCK) 
			WHERE 
				c.Id = ui.CurrencyId)
	FROM 
		UserInfo ui

	;WITH WinInfo AS (
		SELECT 			
			@MemberId AS UserId, 
			@Currency AS Currency,
			SUM(Bet) AS Bet, 
			SUM(Win) AS Win
		FROM   
			dbo.GameHistory gh WITH(NOLOCK)
		WHERE 
			gh.UserId = @MemberId AND
			(gh.[DateTimeUtc] BETWEEN  @StartDate AND @EndDate) AND
			(@IsFreeRound IS NULL OR gh.IsFreeGame = @IsFreeRound) AND 
			(@Platform IS NULL 
				OR gh.[PlatformType] = @Platform 
				OR (@Platform = 1 AND gh.[PlatformType] IN (1,11)))
		GROUP BY 
			UserId)

	INSERT INTO @table
	SELECT 
		@MemberId,
		@Currency,			
		bet,
		netwin = wi.Win  - wi.Bet
	FROM 
		WinInfo wi

	IF(EXISTS(SELECT TOP 1 1 FROM @table))
	BEGIN 
		SELECT 
		'turnover' AS '@type',		
		currency = wi.Currency,			
		bet = wi.Bet,
		netwin = wi.NetWin
		FROM 
			@table wi
		FOR XML PATH('report'), TYPE
	END
	ELSE
	BEGIN 
		SELECT 
		'turnover' AS '@type',		
		currency = @Currency,			
		bet = 0,
		netwin = 0
		FOR XML PATH('report'), TYPE
	END
END

GO
/****** Object:  StoredProcedure [dbo].[MEMBERPERFORMANCEREPORT_dba]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kaidan Joseph	
-- Create date: January 10, 2017
-- Description:	Get the performance report of member, the Platform value can be having.
--				1. Web + WebLD
--				2. Desktop
--				3. Mobile
--				4. Mini
				
-- =============================================
CREATE PROCEDURE [dbo].[MEMBERPERFORMANCEREPORT_dba]
	@StartDate DATETIME,
	@EndDate DATETIME , 
	@ExternalId NVARCHAR(256), 
	@OperatorId INT,
	@IsFreeRound BIT = NULL, 
	@Platform INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE 
			@MemberId  INT, 
			@Currency NVARCHAR(8);
	DECLARE
			@table TABLE (UserId INT, Currency NVARCHAR(8), Bet DECIMAL, NetWin DECIMAL);

	;WITH UserInfo AS (
		SELECT TOP 1
			u.Id,
			u.CurrencyId
		FROM
			[User] u WITH(NOLOCK) 
		WHERE 
			u.ExternalId = @ExternalId AND 
			u.OperatorId  = @OperatorId)
	SELECT 
		@MemberId = ui.Id,
		@Currency = (SELECT TOP 1  c.IsoCode
			FROM 
				Currency c WITH(NOLOCK) 
			WHERE 
				c.Id = ui.CurrencyId)
	FROM 
		UserInfo ui with(nolock)

	;WITH WinInfo AS (
		SELECT 			
			@MemberId AS UserId, 
			@Currency AS Currency,
			SUM(Bet) AS Bet, 
			SUM(Win) AS Win
		FROM   
			dbo.GameHistory gh WITH(NOLOCK,index(IX_DateTimeUtc))
		WHERE 
			gh.UserId = @MemberId AND
			(gh.[DateTimeUtc] BETWEEN  @StartDate AND @EndDate) AND
			(@IsFreeRound IS NULL OR gh.IsFreeGame = @IsFreeRound) AND 
			(@Platform IS NULL 
				OR gh.[PlatformType] = @Platform 
				OR (@Platform = 1 AND gh.[PlatformType] IN (1,11)))
		GROUP BY 
			UserId)

	INSERT INTO @table
	SELECT 
		@MemberId,
		@Currency,			
		bet,
		netwin = wi.Win  - wi.Bet
	FROM 
		WinInfo wi

	IF(EXISTS(SELECT TOP 1 1 FROM @table))
	BEGIN 
		SELECT 
		'turnover' AS '@type',		
		currency = wi.Currency,			
		bet = wi.Bet,
		netwin = wi.NetWin
		FROM 
			@table wi
		FOR XML PATH('report'), TYPE
	END
	ELSE
	BEGIN 
		SELECT 
		'turnover' AS '@type',		
		currency = @Currency,			
		bet = 0,
		netwin = 0
		FOR XML PATH('report'), TYPE
	END
END

GO
/****** Object:  StoredProcedure [dbo].[MEMBERPERFORMANCEREPORTARCHIVE]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kaidan Joseph	
-- Create date: February 20, 2017
-- Description:	Get the performance report of member, the Platform value can be having.
--				1. Web + WebLD
--				2. Desktop
--				3. Mobile
--				4. Mini
-- =============================================
CREATE PROCEDURE [dbo].[MEMBERPERFORMANCEREPORTARCHIVE]
	@StartDate DATETIME,
	@EndDate DATETIME , 
	@ExternalId NVARCHAR(256), 
	@OperatorId Int,
	@IsFreeRound BIT = NULL,
	@Platform INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE 
			@MemberId  INT, 
			@Currency NVARCHAR(8);
	DECLARE
			@table TABLE (UserId INT, Currency NVARCHAR(8), Bet DECIMAL, NetWin DECIMAL);

	;WITH UserInfo AS (
		SELECT TOP 1
			u.Id,
			u.CurrencyId
		FROM
			[User] u WITH(NOLOCK) 
		WHERE 
			u.ExternalId = @ExternalId AND 
			u.OperatorId  = @OperatorId)
	SELECT 
		@MemberId = ui.Id,
		@Currency = (SELECT TOP 1  c.IsoCode
			FROM 
				Currency c WITH(NOLOCK) 
			WHERE 
				c.Id = ui.CurrencyId)
	FROM 
		UserInfo ui

	;WITH WinInfo AS (
		SELECT 			
			@MemberId AS UserId, 
			@Currency AS Currency,
			SUM(Bet) AS Bet, 
			SUM(Win) AS Win
		FROM   
			dbo.GameHistory_35days gh WITH(NOLOCK)
		WHERE 
			gh.UserId = @MemberId AND
			(gh.[DateTimeUtc] BETWEEN  @StartDate AND @EndDate) AND
			(@Platform IS NULL 
				OR gh.[PlatformType] = @Platform 
				OR (@Platform = 1 AND gh.[PlatformType] IN (1,11)))
		GROUP BY 
			UserId)

	INSERT INTO @table
	SELECT 
		@MemberId,
		@Currency,			
		bet,
		netwin = wi.Win  - wi.Bet
	FROM 
		WinInfo wi

	IF(EXISTS(SELECT TOP 1 1 FROM @table))
	BEGIN 
		SELECT 
		'turnover' AS '@type',		
		currency = wi.Currency,			
		bet = wi.Bet,
		netwin = wi.NetWin
		FROM 
			@table wi
		FOR XML PATH('report'), TYPE
	END
	ELSE
	BEGIN 
		SELECT 
		'turnover' AS '@type',		
		currency = @Currency,			
		bet = 0,
		netwin = 0
		FOR XML PATH('report'), TYPE
	END
END


GO
/****** Object:  StoredProcedure [dbo].[PLATFORMREPORTINFOINSERT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PLATFORMREPORTINFOINSERT]
    @ChangeTime [datetime],
    @OffsetId [int],
    @GameId [int],
    @UserId [int],
    @PlatformId [int],
    @IsFreeGame [bit],
    @FreeRoundId [int],
    @IsSideBet [bit],
    @FirstTrxId [int],
    @LastTrxId [int],
    @TrxCount [int],
    @SpinCount [int],
    @FreeSpinCount [int],
    @BonusCount [int],
    @GambleCount [int],
    @TotalBetAmount [decimal](23, 8),
    @SpinBetAmount [decimal](23, 8),
    @GambleBetAmount [decimal](23, 8),
    @TotalWinAmount [decimal](23, 8),
    @TotalNetWinAmount [decimal](23, 8),
    @SpinWin [decimal](23, 8),
    @FreeSpinWin [decimal](23, 8),
    @BonusWin [decimal](23, 8),
    @GambleWin [decimal](23, 8),
    @TotalBetAmountRMB [decimal](23, 8),
    @SpinBetAmountRMB [decimal](23, 8),
    @GambleBetAmountRMB [decimal](23, 8),
    @TotalWinAmountRMB [decimal](23, 8),
    @TotalNetWinAmountRMB [decimal](23, 8),
    @SpinWinRMB [decimal](23, 8),
    @FreeSpinWinRMB [decimal](23, 8),
    @BonusWinRMB [decimal](23, 8),
    @GambleWinRMB [decimal](23, 8)
AS
BEGIN
    
    DECLARE @ISNEW INT;
    
    BEGIN TRAN
		UPDATE PLATFORMREPORTINFO WITH (SERIALIZABLE) SET
			FirstTrxId = IIF(FirstTrxId>@FirstTrxId, @FirstTrxId, FirstTrxId),
			LastTrxId = IIF(LastTrxId<@LastTrxId, @LastTrxId, LastTrxId),
			TrxCount = TrxCount + @TrxCount,
			SpinCount = SpinCount + @SpinCount,
			FreeSpinCount = FreeSpinCount + @FreeSpinCount,
			BonusCount = BonusCount + @BonusCount,
			GambleCount = GambleCount + @GambleCount,
			TotalBetAmount = TotalBetAmount + @TotalBetAmount,
			SpinBetAmount = SpinBetAmount + @SpinBetAmount,
			GambleBetAmount = GambleBetAmount + @GambleBetAmount,
			TotalWinAmount = TotalWinAmount + @TotalWinAmount,
			TotalNetWinAmount = TotalNetWinAmount + @TotalNetWinAmount,
			SpinWin = SpinWin + @SpinWin,
			FreeSpinWin = FreeSpinWin + @FreeSpinWin,
			BonusWin = BonusWin + @BonusWin,
			GambleWin = GambleWin + @GambleWin,
			TotalBetAmountRMB = TotalBetAmountRMB + @TotalBetAmountRMB,
			SpinBetAmountRMB = SpinBetAmountRMB + @SpinBetAmountRMB,
			GambleBetAmountRMB = GambleBetAmountRMB + @GambleBetAmountRMB,
			TotalWinAmountRMB = TotalWinAmountRMB + @TotalWinAmountRMB, 
			TotalNetWinAmountRMB = TotalNetWinAmountRMB	+ @TotalNetWinAmountRMB,
			SpinWinRMB = SpinWinRMB + @SpinWinRMB,
			FreeSpinWinRMB = FreeSpinWinRMB + @FreeSpinWinRMB,
			BonusWinRMB = BonusWinRMB + @BonusWinRMB,
			GambleWinRMB = GambleWinRMB + @GambleWinRMB
		WHERE ChangeTime=@ChangeTime AND UserId=@UserId AND GameId=@GameId AND PlatformId=@PlatformId AND OffsetId=@OffsetId AND IsFreeGame=@IsFreeGame AND FreeRoundId=@FreeRoundId AND IsSideBet=@IsSideBet;
    
		SET @ISNEW = @@ROWCOUNT;
    
		IF (@ISNEW = 0)
		BEGIN
			INSERT INTO PLATFORMREPORTINFO(ChangeTime, UserId, GameId, PlatformId, OffsetId, FirstTrxId, LastTrxId, TrxCount, SpinCount, FreeSpinCount, BonusCount, GambleCount, TotalBetAmount, SpinBetAmount, GambleBetAmount, TotalWinAmount, TotalNetWinAmount, SpinWin, FreeSpinWin, BonusWin, GambleWin, TotalBetAmountRMB, SpinBetAmountRMB, GambleBetAmountRMB, TotalWinAmountRMB, TotalNetWinAmountRMB, SpinWinRMB, FreeSpinWinRMB, BonusWinRMB, GambleWinRMB, IsFreeGame, FreeRoundId, IsSideBet) VALUES
			(@ChangeTime, @UserId, @GameId, @PlatformId, @OffsetId, @FirstTrxId, @LastTrxId, @TrxCount, @SpinCount, @FreeSpinCount, @BonusCount, @GambleCount, @TotalBetAmount, @SpinBetAmount, @GambleBetAmount, @TotalWinAmount, @TotalNetWinAmount, @SpinWin, @FreeSpinWin, @BonusWin, @GambleWin, @TotalBetAmountRMB, @SpinBetAmountRMB, @GambleBetAmountRMB, @TotalWinAmountRMB, @TotalNetWinAmountRMB, @SpinWinRMB, @FreeSpinWinRMB, @BonusWinRMB, @GambleWinRMB, @IsFreeGame, @FreeRoundId, @IsSideBet);
		END
    COMMIT TRAN
    
    RETURN @ISNEW;
END
GO
/****** Object:  StoredProcedure [dbo].[PLATFORMREPORTINFOINSERT_20160805_3]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PLATFORMREPORTINFOINSERT_20160805_3](
	            @ChangeTime           datetime,
	            @UserId               int,
	            @GameId               int,
	            @PlatformId           int,
	            @OffsetId             int,
	            @FirstTrxId           bigint,
	            @LastTrxId            bigint,
	            @TrxCount             int,
	            @SpinCount            int,
	            @FreeSpinCount        int,
	            @BonusCount           int,
	            @GambleCount          int,
	            @TotalBetAmount       decimal(23,8),
	            @SpinBetAmount        decimal(23,8),
	            @GambleBetAmount      decimal(23,8),
	            @TotalWinAmount       decimal(23,8),
	            @TotalNetWinAmount    decimal(23,8),
	            @SpinWin              decimal(23,8),
	            @FreeSpinWin          decimal(23,8),
	            @BonusWin             decimal(23,8),
	            @GambleWin            decimal(23,8),
	            @TotalBetAmountRMB    decimal(23,8),
	            @SpinBetAmountRMB     decimal(23,8),
	            @GambleBetAmountRMB   decimal(23,8),
	            @TotalWinAmountRMB    decimal(23,8),
	            @TotalNetWinAmountRMB decimal(23,8),
	            @SpinWinRMB           decimal(23,8),
	            @FreeSpinWinRMB       decimal(23,8),
	            @BonusWinRMB          decimal(23,8),
	            @GambleWinRMB         decimal(23,8)
            )
            AS
            BEGIN
	            DECLARE @ISNEW INT;
	
	            BEGIN TRAN
		            UPDATE PLATFORMREPORTINFO WITH (SERIALIZABLE) SET
                        FirstTrxId = IIF(FirstTrxId>@FirstTrxId, @FirstTrxId, FirstTrxId),
			            LastTrxId = IIF(LastTrxId<@LastTrxId, @LastTrxId, LastTrxId),
			            TrxCount = TrxCount + @TrxCount,
			            SpinCount = SpinCount + @SpinCount,
		                FreeSpinCount = FreeSpinCount + @FreeSpinCount,
		                BonusCount = BonusCount + @BonusCount,
		                GambleCount = GambleCount + @GambleCount,
		                TotalBetAmount = TotalBetAmount + @TotalBetAmount,
		                SpinBetAmount = SpinBetAmount + @SpinBetAmount,
		                GambleBetAmount = GambleBetAmount + @GambleBetAmount,
		                TotalWinAmount = TotalWinAmount + @TotalWinAmount,
		                TotalNetWinAmount = TotalNetWinAmount + @TotalNetWinAmount,
		                SpinWin = SpinWin + @SpinWin,
		                FreeSpinWin = FreeSpinWin + @FreeSpinWin,
		                BonusWin = BonusWin + @BonusWin,
		                GambleWin = GambleWin + @GambleWin,
		                TotalBetAmountRMB = TotalBetAmountRMB + @TotalBetAmountRMB,
		                SpinBetAmountRMB = SpinBetAmountRMB + @SpinBetAmountRMB,
		                GambleBetAmountRMB = GambleBetAmountRMB + @GambleBetAmountRMB,
		                TotalWinAmountRMB = TotalWinAmountRMB + @TotalWinAmountRMB, 
		                TotalNetWinAmountRMB = TotalNetWinAmountRMB	+ @TotalNetWinAmountRMB,
		                SpinWinRMB = SpinWinRMB + @SpinWinRMB,
		                FreeSpinWinRMB = FreeSpinWinRMB + @FreeSpinWinRMB,
		                BonusWinRMB = BonusWinRMB + @BonusWinRMB,
		                GambleWinRMB = GambleWinRMB + @GambleWinRMB
		            WHERE ChangeTime=@ChangeTime AND UserId=@UserId AND GameId=@GameId AND PlatformId=@PlatformId AND OffsetId=@OffsetId;

		            SET @ISNEW = @@ROWCOUNT;
		
		            IF (@ISNEW = 0)
		            BEGIN
			            INSERT INTO PLATFORMREPORTINFO(ChangeTime, UserId, GameId, PlatformId, OffsetId, FirstTrxId, LastTrxId, TrxCount, SpinCount, FreeSpinCount, BonusCount, GambleCount, TotalBetAmount, SpinBetAmount, GambleBetAmount, TotalWinAmount, TotalNetWinAmount, SpinWin, FreeSpinWin, BonusWin, GambleWin, TotalBetAmountRMB, SpinBetAmountRMB, GambleBetAmountRMB, TotalWinAmountRMB, TotalNetWinAmountRMB, SpinWinRMB, FreeSpinWinRMB, BonusWinRMB, GambleWinRMB) VALUES
			            (@ChangeTime, @UserId, @GameId, @PlatformId, @OffsetId, @FirstTrxId, @LastTrxId, @TrxCount, @SpinCount, @FreeSpinCount, @BonusCount, @GambleCount, @TotalBetAmount, @SpinBetAmount, @GambleBetAmount, @TotalWinAmount, @TotalNetWinAmount, @SpinWin, @FreeSpinWin, @BonusWin, @GambleWin, @TotalBetAmountRMB, @SpinBetAmountRMB, @GambleBetAmountRMB, @TotalWinAmountRMB, @TotalNetWinAmountRMB, @SpinWinRMB, @FreeSpinWinRMB, @BonusWinRMB, @GambleWinRMB);
		            END
	            COMMIT TRAN

	            RETURN @ISNEW;
            END
GO
/****** Object:  StoredProcedure [dbo].[PLATFORMREPORTINFOINSERT_dba]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- MODIFY STORED PROCEDURE
-- ADD PARAM: @FreeRoundId [int]
CREATE PROCEDURE [dbo].[PLATFORMREPORTINFOINSERT_dba]
    @ChangeTime [datetime],
    @OffsetId [int],
    @GameId [int],
    @UserId [int],
    @PlatformId [int],
    @IsFreeGame [bit],
    @FreeRoundId [int],
    @IsSideBet [bit],
    @FirstTrxId [int],
    @LastTrxId [int],
    @TrxCount [int],
    @SpinCount [int],
    @FreeSpinCount [int],
    @BonusCount [int],
    @GambleCount [int],
    @TotalBetAmount [decimal](23, 8),
    @SpinBetAmount [decimal](23, 8),
    @GambleBetAmount [decimal](23, 8),
    @TotalWinAmount [decimal](23, 8),
    @TotalNetWinAmount [decimal](23, 8),
    @SpinWin [decimal](23, 8),
    @FreeSpinWin [decimal](23, 8),
    @BonusWin [decimal](23, 8),
    @GambleWin [decimal](23, 8),
    @TotalBetAmountRMB [decimal](23, 8),
    @SpinBetAmountRMB [decimal](23, 8),
    @GambleBetAmountRMB [decimal](23, 8),
    @TotalWinAmountRMB [decimal](23, 8),
    @TotalNetWinAmountRMB [decimal](23, 8),
    @SpinWinRMB [decimal](23, 8),
    @FreeSpinWinRMB [decimal](23, 8),
    @BonusWinRMB [decimal](23, 8),
    @GambleWinRMB [decimal](23, 8)
AS
BEGIN
    
    DECLARE @ISNEW INT;
    
    BEGIN TRAN
		UPDATE PLATFORMREPORTINFO WITH (SERIALIZABLE) SET
			FirstTrxId = IIF(FirstTrxId>@FirstTrxId, @FirstTrxId, FirstTrxId),
			LastTrxId = IIF(LastTrxId<@LastTrxId, @LastTrxId, LastTrxId),
			TrxCount = TrxCount + @TrxCount,
			SpinCount = SpinCount + @SpinCount,
			FreeSpinCount = FreeSpinCount + @FreeSpinCount,
			BonusCount = BonusCount + @BonusCount,
			GambleCount = GambleCount + @GambleCount,
			TotalBetAmount = TotalBetAmount + @TotalBetAmount,
			SpinBetAmount = SpinBetAmount + @SpinBetAmount,
			GambleBetAmount = GambleBetAmount + @GambleBetAmount,
			TotalWinAmount = TotalWinAmount + @TotalWinAmount,
			TotalNetWinAmount = TotalNetWinAmount + @TotalNetWinAmount,
			SpinWin = SpinWin + @SpinWin,
			FreeSpinWin = FreeSpinWin + @FreeSpinWin,
			BonusWin = BonusWin + @BonusWin,
			GambleWin = GambleWin + @GambleWin,
			TotalBetAmountRMB = TotalBetAmountRMB + @TotalBetAmountRMB,
			SpinBetAmountRMB = SpinBetAmountRMB + @SpinBetAmountRMB,
			GambleBetAmountRMB = GambleBetAmountRMB + @GambleBetAmountRMB,
			TotalWinAmountRMB = TotalWinAmountRMB + @TotalWinAmountRMB, 
			TotalNetWinAmountRMB = TotalNetWinAmountRMB	+ @TotalNetWinAmountRMB,
			SpinWinRMB = SpinWinRMB + @SpinWinRMB,
			FreeSpinWinRMB = FreeSpinWinRMB + @FreeSpinWinRMB,
			BonusWinRMB = BonusWinRMB + @BonusWinRMB,
			GambleWinRMB = GambleWinRMB + @GambleWinRMB
		WHERE ChangeTime=@ChangeTime AND UserId=@UserId AND GameId=@GameId AND PlatformId=@PlatformId AND OffsetId=@OffsetId AND IsFreeGame=@IsFreeGame AND FreeRoundId=@FreeRoundId AND IsSideBet=@IsSideBet;
    
		SET @ISNEW = @@ROWCOUNT;
    
		IF (@ISNEW = 0)
		BEGIN
			INSERT INTO PLATFORMREPORTINFO(ChangeTime, UserId, GameId, PlatformId, OffsetId, FirstTrxId, LastTrxId, TrxCount, SpinCount, FreeSpinCount, BonusCount, GambleCount, TotalBetAmount, SpinBetAmount, GambleBetAmount, TotalWinAmount, TotalNetWinAmount, SpinWin, FreeSpinWin, BonusWin, GambleWin, TotalBetAmountRMB, SpinBetAmountRMB, GambleBetAmountRMB, TotalWinAmountRMB, TotalNetWinAmountRMB, SpinWinRMB, FreeSpinWinRMB, BonusWinRMB, GambleWinRMB, IsFreeGame, FreeRoundId, IsSideBet) VALUES
			(@ChangeTime, @UserId, @GameId, @PlatformId, @OffsetId, @FirstTrxId, @LastTrxId, @TrxCount, @SpinCount, @FreeSpinCount, @BonusCount, @GambleCount, @TotalBetAmount, @SpinBetAmount, @GambleBetAmount, @TotalWinAmount, @TotalNetWinAmount, @SpinWin, @FreeSpinWin, @BonusWin, @GambleWin, @TotalBetAmountRMB, @SpinBetAmountRMB, @GambleBetAmountRMB, @TotalWinAmountRMB, @TotalNetWinAmountRMB, @SpinWinRMB, @FreeSpinWinRMB, @BonusWinRMB, @GambleWinRMB, @IsFreeGame, @FreeRoundId, @IsSideBet);
		END
    COMMIT TRAN
    
    RETURN @ISNEW;
END
GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCE]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCE]	
	            @OperatorId		INT			= NULL,
	            @GameId			INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
				SELECT GameId, Game = g.Name,
					NoOfPlayer = COUNT(DISTINCT UserId),
					NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
                    NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
					AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(pri.SpinCount)),
					TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
                    TotalWinRMB = SUM(TotalWinAmountRMB),
					GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
					GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
				FROM PlatformReportInfo pri WITH(NOLOCK)
				INNER JOIN [Game] g WITH(NOLOCK) ON g.Id = pri.GameId
				INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
				WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
					AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
					AND (pri.GameId = @GameId OR @GameId IS NULL)
					AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
				GROUP BY pri.GameId, g.Name
				ORDER BY pri.GameId
            END


GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCE2]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCE2]	
	            @OperatorId		INT			= NULL,
	            @GameId			INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
				SELECT GameId, Game = g.Name,
					NoOfPlayer = COUNT(DISTINCT UserId),
					NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
					TotalBetRMB = SUM(TotalBetAmountRMB),
					TotalWinRMB = SUM(TotalWinAmountRMB),
					GameIncomeRMB = SUM(TotalBetAmountRMB) - SUM(TotalWinAmountRMB),
					GamePayoutPer = CASE SUM(TotalBetAmountRMB) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(TotalBetAmountRMB) END
				FROM PlatformReportInfo pri WITH(NOLOCK)
				INNER JOIN [Game] g WITH(NOLOCK) ON g.Id = pri.GameId
				INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
				WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
					AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
					AND (pri.GameId = @GameId OR @GameId IS NULL)
					AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
				GROUP BY pri.GameId, g.Name
				ORDER BY pri.GameId
            END
GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCECURRENCY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCECURRENCY]
	            @OperatorId		INT			= NULL,
	            @GameId			INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
	                SELECT 
		            CurrencyId = c.Id, Currency = c.IsoCode,
		            NoOfPlayer = COUNT(DISTINCT UserId),
		            NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
		            TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		            NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
		            AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(pri.SpinCount)),
		            TotalWinRMB = SUM(TotalWinAmountRMB),
		            GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
		            GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
	            FROM PlatformReportInfo pri WITH(NOLOCK)
	            INNER JOIN [Game] g WITH(NOLOCK) ON g.Id = pri.GameId
	            INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
	            INNER JOIN Currency c WITH(NOLOCK) ON c.Id = u.CurrencyId
	            WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		            AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		            AND (pri.GameId = @GameId OR @GameId IS NULL)
		            AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
	            GROUP BY c.Id, c.IsoCode
	            ORDER BY c.IsoCode 
            END



GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCEDAILY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCEDAILY]
	            @OperatorId		INT			= NULL,
	            @GameId			INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
	            SELECT Date = CONVERT(VARCHAR(32), ChangeTime, 23),
		            NoOfPlayer = COUNT(DISTINCT UserId),
		            NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
		            TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		            NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
		            AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(pri.SpinCount)),
		            TotalWinRMB = SUM(TotalWinAmountRMB),
		            GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
		            GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
	            FROM PlatformReportInfo pri WITH(NOLOCK)
	            INNER JOIN [Game] g WITH(NOLOCK) ON g.Id = pri.GameId
	            INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
	            WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		            AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		            AND (pri.GameId = @GameId OR @GameId IS NULL)
		            AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
	            GROUP BY pri.ChangeTime
	            ORDER BY pri.ChangeTime
            END


GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCEGAMEDATE]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCEGAMEDATE]	
	            @OperatorId		INT			= NULL,
	            @GameId			INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
				SELECT GameId, Game = g.Name,
                    Date = CONVERT(VARCHAR(32), ChangeTime, 23),
					NoOfPlayer = COUNT(DISTINCT UserId),
					NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
                    NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
					AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(pri.SpinCount)),
                    TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
					TotalWinRMB = SUM(TotalWinAmountRMB),
					GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
					GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
				FROM PlatformReportInfo pri WITH(NOLOCK)
				INNER JOIN [Game] g WITH(NOLOCK) ON g.Id = pri.GameId
				INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
				WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
					AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
					AND (pri.GameId = @GameId OR @GameId IS NULL)
					AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
				GROUP BY pri.GameId, g.Name, CONVERT(VARCHAR(32), ChangeTime, 23)
	            ORDER BY pri.GameId, CONVERT(VARCHAR(32), ChangeTime, 23)
            END


GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCEMEMBER]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCEMEMBER]	
	            @OperatorId		INT			= NULL,				
	            @GameId			INT,
				@CurrencyId		INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
				SELECT ROW_NUMBER() OVER (ORDER BY u.Name ASC) AS No, 
					UserId,
		            MemberName = u.Name,
		            u.CurrencyId,
		            u.OperatorId,
		            Operator = o.Name,
		            Currency = c.IsoCode,
		            GameId,
		            Game = g.Name,
		            NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
					TotalBet = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)),
					TotalWin = SUM(TotalWinAmount),
					GameIncome = SUM(IIF(IsFreeGame=1,0,TotalBetAmount)) - SUM(TotalWinAmount),
		            TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		            TotalWinRMB = SUM(TotalWinAmountRMB),
		            GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
					GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
				FROM	PlatformReportInfo pri WITH(NOLOCK)
	            INNER JOIN Game g WITH(NOLOCK) ON g.Id = pri.GameId
	            INNER JOIN [User] u WITH(NOLOCK) ON u.Id = pri.UserId
	            INNER JOIN Currency c WITH(NOLOCK) ON c.Id = u.CurrencyId
	            INNER JOIN Operator o WITH(NOLOCK) ON o.Id = u.OperatorId
				WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		            AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
					AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
					AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
		            AND pri.GameId = @GameId
	            GROUP BY pri.UserId, u.Name, u.CurrencyId, u.OperatorId, o.Name, c.IsoCode, pri.GameId, g.Name
				ORDER BY u.Name
			END


GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCEMEMBER2]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCEMEMBER2]	
	            @OperatorId		INT			= NULL,
	            @GameId			INT,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
				SELECT UserId,
		            MemberName = u.Name,
		            u.CurrencyId,
		            u.OperatorId,
		            Operator = o.Name,
		            Currency = c.IsoCode,
		            GameId,
		            Game = g.Name,
		            NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
					TotalBet = SUM(TotalBetAmount),
					TotalWin = SUM(TotalWinAmount),
					GameIncome = SUM(TotalBetAmount) - SUM(TotalWinAmount),
		            TotalBetRMB = SUM(TotalBetAmountRMB),
		            TotalWinRMB = SUM(TotalWinAmountRMB),
		            GameIncomeRMB = SUM(TotalBetAmountRMB) - SUM(TotalWinAmountRMB),
					GamePayoutPer = CASE SUM(TotalBetAmountRMB) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(TotalBetAmountRMB) END
				FROM	PlatformReportInfo pri WITH(NOLOCK)
	            INNER JOIN Game g WITH(NOLOCK) ON g.Id = pri.GameId
	            INNER JOIN [User] u WITH(NOLOCK) ON u.Id = pri.UserId
	            INNER JOIN Currency c WITH(NOLOCK) ON c.Id = u.CurrencyId
	            INNER JOIN Operator o WITH(NOLOCK) ON o.Id = u.OperatorId
				WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		            AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
					AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
		            AND pri.GameId = @GameId
	            GROUP BY pri.UserId, u.Name, u.CurrencyId, u.OperatorId, o.Name, c.IsoCode, pri.GameId, g.Name
				ORDER BY u.Name
			END
GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCEMONTHLY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCEMONTHLY]
	            @OperatorId		INT			= NULL,
	            @GameId			INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
	            SELECT [Month] = DATEPART(month, pri.ChangeTime),
					[Year] = DATEPART(year, pri.ChangeTime),
		            NoOfPlayer = COUNT(DISTINCT UserId),
		            NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
		            TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		            NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
		            AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(pri.SpinCount)),
		            TotalWinRMB = SUM(TotalWinAmountRMB),
		            GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
		            GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
	            FROM PlatformReportInfo pri WITH(NOLOCK)
	            INNER JOIN [Game] g WITH(NOLOCK) ON g.Id = pri.GameId
	            INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
	            WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		            AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		            AND (pri.GameId = @GameId OR @GameId IS NULL)
		            AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
	            GROUP BY DATEPART(month, pri.ChangeTime), DATEPART(year, pri.ChangeTime)
				ORDER BY [Year], [Month]
            END


GO
/****** Object:  StoredProcedure [dbo].[REPORTGAMEPERFORMANCEWEEKLY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTGAMEPERFORMANCEWEEKLY]
	            @OperatorId		INT			= NULL,
	            @GameId			INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
            BEGIN
	            SELECT [Week] = DATEPART(wk, pri.ChangeTime),
					[Year] = DATEPART(year, pri.ChangeTime),
		            NoOfPlayer = COUNT(DISTINCT UserId),
		            NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
		            TotalBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)),
		            NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
		            AvgBetRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) / IIF(SUM(SpinCount)=0, 1, SUM(pri.SpinCount)),
		            TotalWinRMB = SUM(TotalWinAmountRMB),
		            GameIncomeRMB = SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) - SUM(TotalWinAmountRMB),
		            GamePayoutPer = CASE SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(IsFreeGame=1,0,TotalBetAmountRMB)) END
	            FROM PlatformReportInfo pri WITH(NOLOCK)
	            INNER JOIN [Game] g WITH(NOLOCK) ON g.Id = pri.GameId
	            INNER JOIN [User] u WITH(NOLOCK) ON u.Id  = pri.userId
	            WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
		            AND (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
		            AND (pri.GameId = @GameId OR @GameId IS NULL)
		            AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
	            GROUP BY DATEPART(wk, pri.ChangeTime), DATEPART(year, pri.ChangeTime)
				ORDER BY [Year], [Week]
            END
GO
/****** Object:  StoredProcedure [dbo].[REPORTTOPWINNER]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTTOPWINNER]	
	@OperatorId		INT				= NULL,
	@GameId			INT				= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@Username		NVARCHAR(255)	= NULL,
	@Top			INT
AS
BEGIN
    WITH tem AS (
		SELECT TOP (@Top) pri.UserId, 
			u.Name, 
			Operator = o.Name,
			Currency = c.DisplayCode,
			NoOfTransaction = SUM(pri.TrxCount), 
			NoOfSpin = SUM(SpinCount),
			AvgBet = SUM(TotalBetAmountRMB) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
			TotalBet = SUM(TotalBetAmountRMB),
			TotalNetWin = SUM(TotalNetWinAmountRMB), 
			CompanyWLPercentage = IIF(SUM(TotalBetAmountRMB)=0, 1, -(SUM(TotalNetWinAmountRMB)) / SUM(TotalBetAmountRMB)),
			JoinDate = u.CreatedOnUtc
		FROM	PlatformReportInfo pri WITH (NOLOCK)
		INNER JOIN	[User] u	WITH (NOLOCK) ON pri.UserId = u.Id
		INNER JOIN	Operator o	WITH (NOLOCK) ON u.OperatorId = o.Id
		INNER JOIN  Game g		WITH (NOLOCK) ON pri.GameId = g.Id
		INNER JOIN  Currency c	WITH (NOLOCK) ON u.CurrencyId = c.Id
		WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
			AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
			AND (pri.GameId = @GameId OR @GameId IS NULL)
			AND (u.Name like '%' + @Username + '%' OR @Username IS NULL)
		GROUP BY pri.UserId, u.Name, o.Name, c.DisplayCode, u.CreatedOnUtc
		ORDER BY TotalNetWin DESC
	)
	SELECT tem.*, 
		apri.AllTimeNoOfTransaction,
		apri.AllTimeNoOfSpin,
		apri.AllTimeAvgBet,
		apri.AllTimeTotalBet,
		apri.AllTimeTotalNetWin,
		apri.AllTimeCompanyWLPercentage
	FROM tem
	INNER JOIN
	(
		SELECT UserId, 
			AllTimeNoOfTransaction = SUM(TrxCount),
			AllTimeNoOfSpin	= SUM(SpinCount),
			AllTimeAvgBet = SUM(TotalBetAmountRMB) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
			AllTimeTotalBet = SUM(TotalBetAmountRMB), 
			AllTimeTotalNetWin = SUM(TotalNetWinAmountRMB),
			AllTimeCompanyWLPercentage = -(SUM(TotalNetWinAmountRMB) /IIF(SUM(TotalBetAmountRMB) = 0, 1,SUM(TotalBetAmountRMB)))
		FROM PlatformReportInfo 
		GROUP BY UserId
	) apri ON apri.UserId = tem.UserId
	ORDER BY TotalNetWin DESC
END


GO
/****** Object:  StoredProcedure [dbo].[REPORTTOPWINNERDAILY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTTOPWINNERDAILY]	
	            @OperatorId		INT				= NULL,
	            @GameId			INT				= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @UserId 		INT
            AS
                BEGIN
					SELECT [Date] = CONVERT(VARCHAR(32), pri.ChangeTime, 23),
						UserId = @UserId,
						Operator = o.Name,
						Currency = c.DisplayCode,
						NoOfTransaction = SUM(pri.TrxCount), 
						NoOfSpin = SUM(SpinCount),
						AvgBet = SUM(TotalBetAmountRMB) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
						TotalBet = SUM(TotalBetAmountRMB),
						TotalNetWin = SUM(TotalNetWinAmountRMB), 
						CompanyWLPercentage = IIF(SUM(TotalBetAmountRMB)=0, 1, -(SUM(TotalNetWinAmountRMB)) / SUM(TotalBetAmountRMB))
					FROM	PlatformReportInfo pri WITH (NOLOCK)
					INNER JOIN	[User] u	WITH (NOLOCK) ON pri.UserId = u.Id
					INNER JOIN	Operator o	WITH (NOLOCK) ON u.OperatorId = o.Id
					INNER JOIN  Game g		WITH (NOLOCK) ON pri.GameId = g.Id
					INNER JOIN  Currency c	WITH (NOLOCK) ON u.CurrencyId = c.Id
					WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
						AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
						AND (pri.GameId = @GameId OR @GameId IS NULL)
						AND (u.Id = @UserId)
					GROUP BY pri.ChangeTime, UserId, o.Name, c.DisplayCode
					ORDER BY pri.ChangeTime
			    END
GO
/****** Object:  StoredProcedure [dbo].[REPORTTOPWINNERGAME]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTTOPWINNERGAME]	
	            @OperatorId		INT				= NULL,
	            @GameId			INT				= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @UserId 		INT
            AS
                BEGIN
					SELECT 
						Game = g.Name,
						UserId = @UserId,
						Operator = o.Name,
						Currency = c.DisplayCode,
						NoOfTransaction = SUM(pri.TrxCount), 
						NoOfSpin = SUM(SpinCount),
						AvgBet = SUM(TotalBetAmountRMB) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
						TotalBet = SUM(TotalBetAmountRMB),
						TotalNetWin = SUM(TotalNetWinAmountRMB), 
						CompanyWLPercentage = IIF(SUM(TotalBetAmountRMB)=0, 1, -(SUM(TotalNetWinAmountRMB)) / SUM(TotalBetAmountRMB))
					FROM	PlatformReportInfo pri WITH (NOLOCK)
					INNER JOIN	[User] u	WITH (NOLOCK) ON pri.UserId = u.Id
					INNER JOIN	Operator o	WITH (NOLOCK) ON u.OperatorId = o.Id
					INNER JOIN  Game g		WITH (NOLOCK) ON pri.GameId = g.Id
					INNER JOIN  Currency c	WITH (NOLOCK) ON u.CurrencyId = c.Id
					WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
						AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
						AND (pri.GameId = @GameId OR @GameId IS NULL)
						AND (u.Id = @UserId)
					GROUP BY pri.GameId, g.Name, UserId, o.Name, c.DisplayCode
					ORDER BY pri.GameId DESC
			    END
GO
/****** Object:  StoredProcedure [dbo].[REPORTTOPWINNERMONTHLY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTTOPWINNERMONTHLY]	
	            @OperatorId		INT				= NULL,
	            @GameId			INT				= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @UserId 		INT
            AS
                BEGIN
					SELECT [Month] = DATEPART(month, pri.ChangeTime),
						[Year] = DATEPART(year, pri.ChangeTime),
						UserId = @UserId,
						Operator = o.Name,
						Currency = c.DisplayCode,
						NoOfTransaction = SUM(pri.TrxCount), 
						NoOfSpin = SUM(SpinCount),
						AvgBet = SUM(TotalBetAmountRMB) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
						TotalBet = SUM(TotalBetAmountRMB),
						TotalNetWin = SUM(TotalNetWinAmountRMB), 
						CompanyWLPercentage = IIF(SUM(TotalBetAmountRMB)=0, 1, -(SUM(TotalNetWinAmountRMB)) / SUM(TotalBetAmountRMB))
					FROM	PlatformReportInfo pri WITH (NOLOCK)
					INNER JOIN	[User] u	WITH (NOLOCK) ON pri.UserId = u.Id
					INNER JOIN	Operator o	WITH (NOLOCK) ON u.OperatorId = o.Id
					INNER JOIN  Game g		WITH (NOLOCK) ON pri.GameId = g.Id
					INNER JOIN  Currency c	WITH (NOLOCK) ON u.CurrencyId = c.Id
					WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
						AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
						AND (pri.GameId = @GameId OR @GameId IS NULL)
						AND (u.Id = @UserId)
						GROUP BY DATEPART(month, pri.ChangeTime), DATEPART(year, pri.ChangeTime), UserId, o.Name, c.DisplayCode
						ORDER BY [Year], [Month]
			    END
GO
/****** Object:  StoredProcedure [dbo].[REPORTTOPWINNERWEEKLY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTTOPWINNERWEEKLY]	
	            @OperatorId		INT				= NULL,
	            @GameId			INT				= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @UserId 		INT
            AS
                BEGIN
					SELECT [Week] = DATEPART(wk, pri.ChangeTime),
						[Year] = DATEPART(year, pri.ChangeTime),
						UserId = @UserId,
						Operator = o.Name,
						Currency = c.DisplayCode,
						NoOfTransaction = SUM(pri.TrxCount), 
						NoOfSpin = SUM(SpinCount),
						AvgBet = SUM(TotalBetAmountRMB) / IIF(SUM(SpinCount)=0, 1, SUM(SpinCount)),
						TotalBet = SUM(TotalBetAmountRMB),
						TotalNetWin = SUM(TotalNetWinAmountRMB), 
						CompanyWLPercentage = IIF(SUM(TotalBetAmountRMB)=0, 1, -(SUM(TotalNetWinAmountRMB)) / SUM(TotalBetAmountRMB))
					FROM	PlatformReportInfo pri WITH (NOLOCK)
					INNER JOIN	[User] u	WITH (NOLOCK) ON pri.UserId = u.Id
					INNER JOIN	Operator o	WITH (NOLOCK) ON u.OperatorId = o.Id
					INNER JOIN  Game g		WITH (NOLOCK) ON pri.GameId = g.Id
					INNER JOIN  Currency c	WITH (NOLOCK) ON u.CurrencyId = c.Id
					WHERE pri.ChangeTime BETWEEN CONVERT(VARCHAR(32), @StartDateInUTC, 120) AND CONVERT(VARCHAR(32),@EndDateInUTC,120)
						AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
						AND (pri.GameId = @GameId OR @GameId IS NULL)
						AND (u.Id = @UserId)
						GROUP BY DATEPART(wk, pri.ChangeTime), DATEPART(year, pri.ChangeTime), UserId, o.Name, c.DisplayCode
						ORDER BY [Year], [Week]
			    END
GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEALL]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[REPORTWINLOSEALL]
    @OperatorId [int],
    @GameId [int],
    @UserId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
    @IsFreeRounds [bit]
AS
BEGIN
    SELECT
		[Date]= CONVERT(VARCHAR(12), @StartDateInUTC, 106) +'-'+ CONVERT(VARCHAR(12),  @EndDateInUTC , 106), 
		Game = 'All',
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction =ISNULL(SUM(CONVERT(BIGINT, TrxCount)),0),
		NoOfSpin = ISNULL(SUM(CONVERT(BIGINT, SpinCount)), 0),
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
END

GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEDAILY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
			CREATE PROCEDURE [dbo].[REPORTWINLOSEDAILY]	
                @OperatorId		INT,
	            @GameId			INT			= NULL,
	            @UserId			INT			= NULL,
                @StartDateInUTC	DATETIME,
                @EndDateInUTC	DATETIME,
                @IsDemo			BIT			= NULL,
				@IsFreeRounds	BIT			= NULL
            AS
            BEGIN
                SELECT 
                    Date = CONVERT(VARCHAR(32), pri.ChangeTime, 23),
                    Game = 'All',
                    NoOfPlayer = COUNT(DISTINCT UserId),
                    NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
		            TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
		            TotalWin = SUM(pri.TotalWinAmount),
		            GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(pri.TotalWinAmount),
                    NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
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
                GROUP BY pri.ChangeTime
                ORDER BY pri.ChangeTime
            END
GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAME]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
			CREATE PROCEDURE [dbo].[REPORTWINLOSEGAME]
	            @OperatorId		INT,
				@GameId			INT			= NULL,
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
				@IsFreeRounds	BIT			= NULL
            AS
                BEGIN
					SELECT pri.GameId,
						g.Name AS Game,
						NoOfPlayer = COUNT(DISTINCT(pri.UserId)),
						NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),
                        NoOfSpin = SUM(CONVERT(BIGINT, pri.SpinCount)),
                        AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
						TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
						AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
                        TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
						TotalWin = SUM(pri.TotalWinAmount),
						TotalWinRMB = SUM(pri.TotalWinAmountRMB),
						GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(pri.TotalWinAmount),
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
					GROUP BY pri.GameId, g.Name
                END
GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMECURRENCY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[REPORTWINLOSEGAMECURRENCY]
    @OperatorId		INT,
	@GameId			INT			= NULL,
    @StartDateInUTC DATETIME,
    @EndDateInUTC	DATETIME,
    @IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL
AS
    BEGIN
        SELECT CurrencyId,
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
        GROUP BY u.CurrencyId, c.IsoCode
    END


GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEDATE]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[REPORTWINLOSEGAMEDATE]
	            @OperatorId		INT,
				@GameId			INT,
				@CurrencyId		INT,
				@UserId			INT,
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL
            AS
                BEGIN
					SELECT 
						Date = CONVERT(VARCHAR(32), pri.ChangeTime, 23),
						pri.GameId,
						g.Name AS Game,
						NoOfPlayer = COUNT(DISTINCT(pri.UserId)),
						NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),
                        NoOfSpin = SUM(CONVERT(BIGINT, pri.SpinCount)),
                        AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
						TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
						AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
                        TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
						TotalWin = SUM(pri.TotalWinAmount),
						TotalWinRMB = SUM(pri.TotalWinAmountRMB),
						GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) - SUM(pri.TotalWinAmount),
						GameIncomeRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) - SUM(pri.TotalWinAmountRMB),
						GamePayoutPer = CASE SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) END
					FROM PlatformReportInfo pri WITH (NOLOCK)
					INNER JOIN Game g WITH (NOLOCK) ON pri.GameId = g.Id
					INNER JOIN [User] u WITH (NOLOCK) ON pri.UserId = u.Id
		            WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
			            AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
			            AND (pri.GameId = @GameId OR @GameId IS NULL)
			            AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
			            AND (pri.UserId = @UserId OR @UserId IS NULL)
			            AND pri.ChangeTime >= CONVERT(VARCHAR(32), @StartDateInUTC, 120)
			            AND pri.ChangeTime < CONVERT(VARCHAR(32), @EndDateInUTC, 120)
					GROUP BY pri.GameId, g.Name, pri.ChangeTime
                END


GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEMEMBER]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

			CREATE PROCEDURE [dbo].[REPORTWINLOSEGAMEMEMBER]
	            @OperatorId		INT,
				@GameId			INT,
				@CurrencyId		INT,
				@UserId			INT,
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
				@IsFreeRounds	BIT			= NULL
            AS
                BEGIN
					SELECT MemberId = u.Id,
						MemberName = u.Name, 
						Operator = o.Name, 
						Currency = c.IsoCode,
						NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),
                        NoOfSpin = SUM(CONVERT(BIGINT, pri.SpinCount)),
                        AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
						TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
                        AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
						TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
						TotalWin = SUM(pri.TotalWinAmount),
						TotalWinRMB = SUM(pri.TotalWinAmountRMB),
						GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(pri.TotalWinAmount), 
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
					GROUP BY u.Id, u.Name, o.Id, o.Name, c.IsoCode
					ORDER BY u.Name
				END
            
GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEMEMBERDETAIL]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
            CREATE PROCEDURE [dbo].[REPORTWINLOSEGAMEMEMBERDETAIL]	
	            @OperatorId		INT,
				@GameId			INT,
				@CurrencyId		INT,
				@UserId			INT,
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
				@IsFreeRounds	BIT			= NULL,
				@OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
					SELECT ROW_NUMBER() OVER (ORDER BY gh.Id DESC) AS No, 
						gh.Id,
						gh.GameTransactionId,
						MemberName = u.Name,
						Currency = c.IsoCode,
						Game = g.Name,
						gt.[Type], 
						Bet = IIF(gh.IsFreeGame=1,0,IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)), 
						BetRMB = IIF(gh.IsFreeGame=1,0,IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)) * gh.ExchangeRate, 
						Win = gh.Win,
						WinRMB = gh.Win * gh.ExchangeRate,
						TransactionTime = gh.DateTimeUtc,
						IsVoid = gh.IsDeleted,
						IsSideBet = ISNULL(sbp.IsSideBet,0),
						IsFreeGame = gh.IsFreeGame,
						COUNT(gh.Id) OVER () as TotalRecords
					FROM GameHistory gh WITH (NOLOCK)
					LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
					INNER JOIN GameTransaction gt WITH (NOLOCK) ON gh.GameTransactionId = gt.Id
					INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
					INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
					INNER JOIN Operator o WITH (NOLOCK) ON u.OperatorId = o.Id
					INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id 
		            WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
			            AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
			            AND (gh.GameId = @GameId OR @GameId IS NULL)
			            AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
			            AND (gh.UserId = @UserId OR @UserId IS NULL)
			            AND gh.DateTimeUtc >= @StartDateInUTC
			            AND gh.DateTimeUtc < @EndDateInUTC
						AND (gh.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
					ORDER BY gh.Id DESC
					OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
				END
GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEMEMBERDETAIL_20160808]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

				CREATE PROCEDURE [dbo].[REPORTWINLOSEGAMEMEMBERDETAIL_20160808]	
	            @OperatorId		INT,
				@GameId			INT,
				@CurrencyId		INT,
				@UserId			INT,
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
				@OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
					SELECT ROW_NUMBER() OVER (ORDER BY gh.Id DESC) AS No, 
						gh.Id,
						gh.GameTransactionId,
						MemberName = u.Name,
						Currency = c.IsoCode,
						Game = g.Name,
						gt.[Type], 
						Bet = gh.Bet, 
						BetRMB = gh.Bet * gh.ExchangeRate, 
						Win = gh.Win,
						WinRMB = gh.Win * gh.ExchangeRate,
						TransactionTime = gh.DateTimeUtc,
						IsVoid = gh.IsDeleted,
						COUNT(gh.Id) OVER () as TotalRecords
					FROM GameHistory gh WITH (NOLOCK)
					INNER JOIN GameTransaction gt WITH (NOLOCK) ON gh.GameTransactionId = gt.Id
					INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
					INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
					INNER JOIN Operator o WITH (NOLOCK) ON u.OperatorId = o.Id
					INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id 
		            WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
			            AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
			            AND (gh.GameId = @GameId OR @GameId IS NULL)
			            AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
			            AND (gh.UserId = @UserId OR @UserId IS NULL)
			            AND gh.DateTimeUtc >= @StartDateInUTC
			            AND gh.DateTimeUtc < @EndDateInUTC
					ORDER BY gh.Id DESC
					OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
				END

GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEMEMBERDETAIL_dba]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
            create PROCEDURE [dbo].[REPORTWINLOSEGAMEMEMBERDETAIL_dba]	
	            @OperatorId		INT,
				@GameId			INT,
				@CurrencyId		INT,
				@UserId			INT,
	            @StartDateInUTC DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
				@IsFreeRounds	BIT			= NULL,
				@OffsetRows		INT,
	            @PageSize		INT
            AS
                BEGIN
					SELECT ROW_NUMBER() OVER (ORDER BY gh.Id DESC) AS No, 
						gh.Id,
						gh.GameTransactionId,
						MemberName = u.Name,
						Currency = c.IsoCode,
						Game = g.Name,
						gt.[Type], 
						Bet = IIF(gh.IsFreeGame=1,0,IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)), 
						BetRMB = IIF(gh.IsFreeGame=1,0,IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)) * gh.ExchangeRate, 
						Win = gh.Win,
						WinRMB = gh.Win * gh.ExchangeRate,
						TransactionTime = gh.DateTimeUtc,
						IsVoid = gh.IsDeleted,
						IsSideBet = ISNULL(sbp.IsSideBet,0),
						IsFreeGame = gh.IsFreeGame,
						COUNT(gh.Id) OVER () as TotalRecords
					FROM GameHistory gh WITH (NOLOCK)
					LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
					INNER JOIN GameTransaction gt WITH (NOLOCK) ON gh.GameTransactionId = gt.Id
					INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
					INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
					INNER JOIN Operator o WITH (NOLOCK) ON u.OperatorId = o.Id
					INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id 
		            WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
			            AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
			            AND (gh.GameId = @GameId OR @GameId IS NULL)
			            AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
			            AND (gh.UserId = @UserId OR @UserId IS NULL)
			            AND gh.DateTimeUtc >= @StartDateInUTC
			            AND gh.DateTimeUtc < @EndDateInUTC
						AND (gh.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
					ORDER BY gh.Id DESC
					OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
				END
GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEPLATFORMBYCURRENCY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTWINLOSEGAMEPLATFORMBYCURRENCY]
    @OperatorId [int],
    @GameId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
	@IsFreeRounds [bit],
    @PlatformType [nvarchar](255)
AS
BEGIN
    
    DECLARE @sql    NVARCHAR(MAX)
    DECLARE @def    NVARCHAR(MAX)
    
    SET @sql = N'SELECT CurrencyId,
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
    	AND (u.OperatorId = @OperatorId OR @OperatorId IS NULL)
    	AND (pri.GameId = @GameId OR @GameId IS NULL)
    	AND (pri.PlatformId IN ('+ @PlatformType  + N'))
    	AND pri.ChangeTime >= CONVERT(VARCHAR(32), @StartDateInUTC, 120)
    	AND pri.ChangeTime < CONVERT(VARCHAR(32), @EndDateInUTC, 120)
		AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
    	AND OffsetId = 42
    GROUP BY u.CurrencyId, c.IsoCode';
    
    SET @def = N'@OperatorId INT, @GameId INT, @StartDateInUTC DATETIME, @EndDateInUTC	DATETIME, @IsDemo BIT, @IsFreeRounds BIT';
    EXECUTE sp_executesql @sql, @def, @OperatorId=@OperatorId, @GameId=@GameId, @StartDateInUTC=@StartDateInUTC, @EndDateInUTC=@EndDateInUTC, @IsDemo=@IsDemo, @IsFreeRounds=@IsFreeRounds
END

GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEPLATFORMBYGAME]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTWINLOSEGAMEPLATFORMBYGAME]
    @OperatorId [int],
    @GameId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
	@IsFreeRounds [bit],
    @PlatformType [nvarchar](255)
AS
BEGIN
    
    DECLARE @sql    NVARCHAR(MAX)
    DECLARE @def    NVARCHAR(MAX)
    
    SET @sql = N'SELECT pri.GameId,
    			Game = g.Name,
    			NoOfPlayer = COUNT(DISTINCT(pri.UserId)),
    			NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),
    			NoOfSpin = SUM(CONVERT(BIGINT, pri.SpinCount)),
    			AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
    			TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
    			AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
    			TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
    			TotalWin = SUM(pri.TotalWinAmount),
    			TotalWinRMB = SUM(pri.TotalWinAmountRMB),
    			GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(pri.TotalWinAmount),
    			GameIncomeRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) - SUM(pri.TotalWinAmountRMB),
    			GamePayoutPer = CASE SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) END
    		FROM PlatformReportInfo pri WITH (NOLOCK)
    		INNER JOIN Game g WITH (NOLOCK) ON pri.GameId = g.Id
    		INNER JOIN [User] u WITH (NOLOCK) ON pri.UserId = u.Id
    		WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    			AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
    			AND (pri.GameId = @GameId OR @GameId IS NULL)
    			AND (pri.PlatformId IN ('+ @PlatformType  + N'))
    			AND pri.ChangeTime >= CONVERT(VARCHAR(32), @StartDateInUTC, 120)
    			AND pri.ChangeTime < CONVERT(VARCHAR(32), @EndDateInUTC, 120)
				AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
    		GROUP BY pri.GameId, g.Name';
    
    SET @def = N'@OperatorId INT, @GameId INT, @StartDateInUTC DATETIME, @EndDateInUTC	DATETIME, @IsDemo BIT, @IsFreeRounds BIT';
    EXECUTE sp_executesql @sql, @def, @OperatorId=@OperatorId, @GameId=@GameId, @StartDateInUTC=@StartDateInUTC, @EndDateInUTC=@EndDateInUTC, @IsDemo=@IsDemo, @IsFreeRounds=@IsFreeRounds
END

GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEPLATFORMMEMBER]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTWINLOSEGAMEPLATFORMMEMBER]
    @OperatorId [int],
    @GameId [int],
    @CurrencyId [int],
    @UserId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
	@IsFreeRounds [bit],
    @PlatformType [nvarchar](255)
AS
BEGIN
    DECLARE @sql    NVARCHAR(MAX)
    DECLARE @def    NVARCHAR(MAX)
    
    SET @sql = N'SELECT MemberId = u.Id,
    	MemberName = u.Name, 
    	Operator = o.Name, 
    	Currency = c.IsoCode,
    	NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),
		NoOfSpin = SUM(CONVERT(BIGINT, pri.SpinCount)),
		AvgBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
    	TotalBet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
		AvgBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB))/IIF(SUM(pri.SpinCount)=0, 1, SUM(pri.SpinCount)),
    	TotalBetRMB = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
    	TotalWin = SUM(pri.TotalWinAmount),
    	TotalWinRMB = SUM(pri.TotalWinAmountRMB),
    	GameIncome = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)) - SUM(pri.TotalWinAmount), 
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
    	AND (pri.PlatformId IN ('+ @PlatformType  + N'))
		AND (pri.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
    GROUP BY u.Id, u.Name, o.Id, o.Name, c.IsoCode
    ORDER BY u.Name';
    
    SET @def = N'@OperatorId INT, @GameId INT, @CurrencyId INT, @UserId	INT, @StartDateInUTC DATETIME, @EndDateInUTC DATETIME, @IsDemo BIT, @IsFreeRounds BIT';
    	
    EXECUTE sp_executesql @sql, @def, @OperatorId=@OperatorId, @GameId=@GameId, @CurrencyId=@CurrencyId, @UserId=@UserId, @StartDateInUTC=@StartDateInUTC, @EndDateInUTC=@EndDateInUTC, @IsDemo=@IsDemo, @IsFreeRounds=@IsFreeRounds
END

GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEGAMEPLATFORMMEMBERDETAIL]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTWINLOSEGAMEPLATFORMMEMBERDETAIL]
    @OperatorId [int],
    @GameId [int],
    @CurrencyId [int],
    @UserId [int],
    @StartDateInUTC [datetime],
    @EndDateInUTC [datetime],
    @IsDemo [bit],
	@IsFreeRounds [bit],
    @OffsetRows [int],
    @PageSize [int],
    @PlatformType [nvarchar](255)
AS
BEGIN
    DECLARE @sql    NVARCHAR(MAX)
    DECLARE @def    NVARCHAR(MAX)
    
    SET @sql = N'SELECT ROW_NUMBER() OVER (ORDER BY gh.Id DESC) AS No, 
    				gh.Id,
    				gh.GameTransactionId,
    				MemberName = u.Name,
    				Currency = c.IsoCode,
    				Game = g.Name,
    				gt.[Type], 
    				Bet = IIF(gh.IsFreeGame=1,0,IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)), 
    				BetRMB = IIF(gh.IsFreeGame=1,0,IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)) * gh.ExchangeRate, 
    				Win = gh.Win,
    				WinRMB = gh.Win * gh.ExchangeRate,
    				TransactionTime = gh.DateTimeUtc,
    				IsVoid = gh.IsDeleted,
    				IsSideBet = ISNULL(sbp.IsSideBet,0),
    				IsFreeGame = gh.IsFreeGame,
    				COUNT(gh.Id) OVER () as TotalRecords
    			FROM GameHistory gh WITH (NOLOCK)
    			LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
    			INNER JOIN GameTransaction gt WITH (NOLOCK) ON gh.GameTransactionId = gt.Id
    			INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
    			INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
    			INNER JOIN Operator o WITH (NOLOCK) ON u.OperatorId = o.Id
    			INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id 
    		    WHERE (u.IsDemo = @IsDemo OR @IsDemo IS NULL)
    			    AND (u.OperatorId = @OperatorID OR @OperatorID IS NULL)
    			    AND (gh.GameId = @GameId OR @GameId IS NULL)
    			    AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
    			    AND (gh.UserId = @UserId OR @UserId IS NULL)
    			    AND gh.DateTimeUtc >= @StartDateInUTC
    			    AND gh.DateTimeUtc < @EndDateInUTC
    				AND (gh.PlatformType IN ('+ @PlatformType  + N'))
					AND (gh.IsFreeGame = @IsFreeRounds OR @IsFreeRounds IS NULL)
    			ORDER BY gh.Id DESC
    			OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY';
    
    SET @def = N'@OperatorId INT, @GameId INT, @CurrencyId INT, @UserId INT, @StartDateInUTC DATETIME, @EndDateInUTC DATETIME, @IsDemo BIT, @IsFreeRounds BIT, @OffsetRows INT, @PageSize INT';
    
    EXECUTE sys.sp_executesql @sql, @def, @OperatorId=@OperatorId,@GameId=@GameId,@CurrencyId=@CurrencyId,@UserId=@UserId,@StartDateInUTC=@StartDateInUTC,@EndDateInUTC=@EndDateInUTC,@IsDemo=@IsDemo,@IsFreeRounds=@IsFreeRounds,@OffsetRows=@OffsetRows,@PageSize=@PageSize
END

GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEMONTHLY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
			CREATE PROCEDURE [dbo].[REPORTWINLOSEMONTHLY]	
	            @OperatorId		INT,
				@GameId			INT			= NULL,
				@UserId			INT			= NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
				@IsFreeRounds	BIT			= NULL
            AS
            BEGIN
				SELECT [Month] = DATEPART(month, pri.ChangeTime),
					[Year] = DATEPART(year, pri.ChangeTime),
					Game = 'All',
					NoOfPlayer = COUNT(DISTINCT UserId),
					NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
                    NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
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
				GROUP BY DATEPART(month, pri.ChangeTime), DATEPART(year, pri.ChangeTime)
				ORDER BY [Year], [Month]
            END
GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEOPERATOR]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[REPORTWINLOSEOPERATOR]	
	@OperatorId		INT,
	@GameId			INT			= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL
AS
BEGIN
	SELECT OperatorId = o.Id, 
		OperatorTag = o.Tag,
		Game = 'All',
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
        NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
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
	GROUP BY o.Id, o.Tag
	ORDER BY o.Tag
END


GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEOPERATORGAME]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[REPORTWINLOSEOPERATORGAME]	
	@OperatorId		INT,
	@GameId			INT			= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL
AS
BEGIN
	SELECT OperatorId = o.Id, 
		OperatorTag = o.Tag,
		GameId = pri.GameId,
		Game = g.Name,
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
        NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
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
	GROUP BY o.Id, o.Tag, pri.GameId, g.Name
	ORDER BY o.Tag, g.Name
END


GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEPLATFORM]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTWINLOSEPLATFORM]
	@OperatorId		INT,
	@GameId			INT			= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL
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
	GROUP BY pri.PlatformId % 10
	ORDER BY [Platform]
END
GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEPLATFORMRES]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[REPORTWINLOSEPLATFORMRES]	
	@OperatorId		INT,
	@GameId			INT			= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL
AS
BEGIN
	SELECT pri.PlatformId, 
		[Platform] = CASE pri.PlatformId % 10
			WHEN 1 THEN 'Web'
			WHEN 2 THEN 'Download'
			WHEN 3 THEN 'Mobile'
			ELSE 'None'
		END,
		[Res] = CASE (pri.PlatformId / 10)
			WHEN 1 THEN 'LD'
			ELSE 'SD'
		END,
		Game = 'All',
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
        NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
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
	GROUP BY pri.PlatformId
	ORDER BY [Platform], [Res]
END


GO
/****** Object:  StoredProcedure [dbo].[REPORTWINLOSEWEEKLY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[REPORTWINLOSEWEEKLY]	
	@OperatorId		INT,
	@GameId			INT			= NULL,
	@UserId			INT			= NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@IsFreeRounds	BIT			= NULL
AS
BEGIN
	SELECT [Week] = DATEPART(wk, pri.ChangeTime),
		[Year] = DATEPART(year, pri.ChangeTime),
		Game = 'All',
		NoOfPlayer = COUNT(DISTINCT UserId),
		NoOfTransaction = SUM(CONVERT(BIGINT, TrxCount)),
        NoOfSpin = SUM(CONVERT(BIGINT, SpinCount)),
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
	GROUP BY DATEPART(wk, pri.ChangeTime), DATEPART(year, pri.ChangeTime)
	ORDER BY [Year], [Week]
END
GO


/****** Object:  StoredProcedure [dbo].[spAutoArcGameHistoryFrom15To35Days]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[spAutoArcGameHistoryFrom15To35Days]
as
 BEGIN

TRUNCATE table tmpArcGHfrom15to35Days;
INSERT into [GameHistory_35days](
		[Id]
	  ,[DateTimeUtc]
      ,[UserId]
      ,[GameId]
      ,[Level]
      ,[Bet]
      ,[Win]
      ,[ExchangeRate]
      ,[GameResultType]
      ,[XmlType]
      ,[ResponseXml]
      ,[HistoryXml]
      ,[IsHistory]
      ,[IsReport]
      ,[PlatformType]
      ,[IsDeleted]
      ,[CreatedBy]
      ,[CreatedOnUtc]
      ,[UpdatedBy]
      ,[UpdatedOnUtc]
      ,[DeletedBy]
      ,[DeletedOnUtc]
      ,[GameTransactionId]
      ,[SpinTransactionId]
      ,[IsFreeGame]
	 )
OUTPUT INSERTED.[Id] INTO tmpArcGHfrom15to35Days
SELECT TOP (10000) [Id]
      ,[DateTimeUtc]
      ,[UserId]
      ,[GameId]
      ,[Level]
      ,[Bet]
      ,[Win]
      ,[ExchangeRate]
      ,[GameResultType]
      ,[XmlType]
      ,[ResponseXml]
      ,[HistoryXml]
      ,[IsHistory]
      ,[IsReport]
      ,[PlatformType]
      ,[IsDeleted]
      ,[CreatedBy]
      ,[CreatedOnUtc]
      ,[UpdatedBy]
      ,[UpdatedOnUtc]
      ,[DeletedBy]
      ,[DeletedOnUtc]
      ,[GameTransactionId]
      ,[SpinTransactionId]
      ,[IsFreeGame]
  FROM [slots].[dbo].[GameHistory] with(nolock)
	WHERE  DateTimeUtc <= dateadd(day,-15,getdate())
		order by DateTimeUtc asc



		 DELETE A from [GameHistory] A with(nolock)
		  inner join tmpArcGHfrom15to35Days B with(nolock) on A.[Id] = B.[Id]


 END;


GO
/****** Object:  StoredProcedure [dbo].[spAutoArcGameTransFrom15To35Days]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[spAutoArcGameTransFrom15To35Days]
as
 BEGIN

TRUNCATE table tmpArcGTfrom15to35Days;

INSERT into dbo.[GameTransaction_35days](
		[Id]
      ,[DateTimeUtc]
      ,[UserId]
      ,[GameId]
      ,[Type]
      ,[IsDeleted]
      ,[CreatedBy]
      ,[CreatedOnUtc]
      ,[UpdatedBy]
      ,[UpdatedOnUtc]
      ,[DeletedBy]
      ,[DeletedOnUtc]
	 )
		  
OUTPUT INSERTED.[Id] INTO tmpArcGTfrom15to35Days

SELECT TOP (10000) [Id]
      ,[DateTimeUtc]
      ,[UserId]
      ,[GameId]
      ,[Type]
      ,[IsDeleted]
      ,[CreatedBy]
      ,[CreatedOnUtc]
      ,[UpdatedBy]
      ,[UpdatedOnUtc]
      ,[DeletedBy]
      ,[DeletedOnUtc]
  FROM [slots].[dbo].[GameTransaction] with(nolock)
  WHERE  DateTimeUtc <= dateadd(day,-15,getdate())
  order by DateTimeUtc asc



DELETE A from [GameTransaction] A with(nolock)
inner join tmpArcGTfrom15to35Days B with(nolock) on A.[Id] = B.[Id]


 END;
GO
/****** Object:  StoredProcedure [dbo].[spAutoArcWalletTransFrom15To35Days]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[spAutoArcWalletTransFrom15To35Days]
as
 BEGIN

TRUNCATE table tmpArcWTfrom15to35Days;

INSERT into dbo.[WalletTransaction_35days](
		[Id]
      ,[Guid]
      ,[Type]
      ,[Amount]
      ,[GameTransactionId]
      ,[WalletProviderId]
      ,[WalletProviderTransactionId]
      ,[WalletProviderResponse]
      ,[IsError]
      ,[ErrorMessage]
      ,[ElapsedSeconds]
      ,[IsDeleted]
      ,[CreatedBy]
      ,[CreatedOnUtc]
      ,[UpdatedBy]
      ,[UpdatedOnUtc]
      ,[DeletedBy]
      ,[DeletedOnUtc]
	 )
		  
		   OUTPUT INSERTED.[Id] INTO tmpArcWTfrom15to35Days

SELECT TOP (10000) [Id]
      ,[Guid]
      ,[Type]
      ,[Amount]
      ,[GameTransactionId]
      ,[WalletProviderId]
      ,[WalletProviderTransactionId]
      ,[WalletProviderResponse]
      ,[IsError]
      ,[ErrorMessage]
      ,[ElapsedSeconds]
      ,[IsDeleted]
      ,[CreatedBy]
      ,[CreatedOnUtc]
      ,[UpdatedBy]
      ,[UpdatedOnUtc]
      ,[DeletedBy]
      ,[DeletedOnUtc]
  FROM [slots].[dbo].[WalletTransaction] with(nolock)
	WHERE  [CreatedOnUtc] <= dateadd(day,-15,getdate())
		order by [CreatedOnUtc] asc



		 DELETE A from [WalletTransaction] A with(nolock)
		  inner join tmpArcWTfrom15to35Days B with(nolock) on A.[Id] = B.[Id]


 END;
GO
/****** Object:  StoredProcedure [dbo].[TOURNAMENTCHECK]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TOURNAMENTCHECK](
    @Id          int,
	@StartTime   datetime,
	@EndTime     datetime,
	@OperatorId  int)
AS
    DECLARE @cnt INT
BEGIN
	SELECT @cnt=COUNT(1) FROM TOURNAMENT ti WITH(NOLOCK) WHERE ti.IsDeleted = 0 AND StartTime<@EndTime AND @StartTime<EndTime AND Id<>@Id AND OperatorId=@OperatorId
	RETURN @cnt;
END
GO
/****** Object:  StoredProcedure [dbo].[TOURNAMENTREPORTINFOINSERT]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TOURNAMENTREPORTINFOINSERT](
	@TournamentId     int,
	@OperatorId       int,
	@UserId           int,
	@GameId           int,
	@Platform         int,
	@Level            int,
	@TrxCount         int,
	@Bet              decimal(23,8),
	@Win              decimal(23,8),
	@BetL             decimal(23,8),
	@WinL             decimal(23,8),
	@DateTimeUtc      datetime)
AS
BEGIN
	DECLARE @ISNEW INT;
	
	BEGIN TRAN
		UPDATE TOURNAMENTREPORTINFO WITH (SERIALIZABLE) SET
		TrxCount = TrxCount + @TrxCount,
		Bet = Bet + @Bet,
		Win = Win + @Win,
		BetL = BetL + @BetL,
		WinL = WinL + @WinL,
		TimeLastBet = @DateTimeUtc
		WHERE TournamentId=@TournamentId AND OperatorId=@OperatorId AND UserId=@UserId AND GameId=@GameId AND [Platform]=@Platform AND [Level]=@Level;

		SET @ISNEW = @@ROWCOUNT;
		
		IF (@ISNEW = 0)
		BEGIN
			INSERT INTO TOURNAMENTREPORTINFO(TournamentId, OperatorId, UserId, GameId, [Platform], [Level], TrxCount, Bet, Win, BetL, WinL, TimeFirstBet, TimeLastBet) VALUES
			(@TournamentId, @OperatorId, @UserId, @GameId, @Platform, @Level, @TrxCount, @Bet, @Win, @BetL, @WinL, @DateTimeUtc, @DateTimeUtc);
		END
	COMMIT TRAN

	RETURN @ISNEW;
END
GO
/****** Object:  StoredProcedure [dbo].[TRELATIONCHECK]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[TRELATIONCHECK](
    @TournyId      int,
	@RelationType  int,
	@RelationId    int,
	@StartTime     datetime,
	@EndTime       datetime,
	@ParamStr      nvarchar(64)
)
AS
    DECLARE @cnt    INT
	DECLARE @UserId INT
BEGIN
	IF @RelationType=1
	BEGIN
		SELECT 
			@UserId=Id 
		FROM [USER] WITH(NOLOCK) 
		WHERE Name=@ParamStr AND OperatorId=@RelationId

		SELECT 
			@cnt=COUNT(DISTINCT RelationId) 
		FROM TOURNAMENT ti WITH(NOLOCK) 
		INNER JOIN TRELATION tr WITH(NOLOCK) ON ti.Id=tr.TournamentId 
		WHERE 
			tr.RelationType=1 
			AND ti.IsDeleted = 0
			AND RelationId=@UserId 
			AND Id<>@TournyId 
			AND ((StartTime<=@StartTime AND EndTime>=@StartTime) OR (StartTime<=@EndTime AND EndTime>=@EndTime))		
		
		IF @cnt>0 RETURN 1;
	END
	ELSE
	BEGIN
	    RETURN 2;
	END

	RETURN 0;
END
GO
/****** Object:  StoredProcedure [dbo].[TUSERHISTORY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TUSERHISTORY]
    @UserId [int],
    @Games [nvarchar](128),
    @Platform [nvarchar](128),
    @TrxId [float],
    @STime [datetime],
    @ETime [datetime],
    @MinBet [decimal](18, 0),
    @OffsetRows [int],
    @PageSize [int]
AS
BEGIN
    
    DECLARE @sql    NVARCHAR(MAX)
    DECLARE @def    NVARCHAR(MAX)
    
    SET @sql = N';WITH tem AS(
    		    SELECT
    			    gh.Id,
    			    gh.GameTransactionId,
    			    CreatedOnUtc = gh.DateTimeUtc,
    			    gh.GameResultType,
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
    			    AND gh.GameId IN (' + @Games + N') AND gh.PlatformType%10 IN (' + @Platform + N')
    			    AND gh.UserId = @UserId 
    			    AND gh.Bet >= @MinBet
    			    AND (@TrxId IS NULL OR gh.GameTransactionId = @TrxId) 
    			    AND gh.GameResultType <= CASE WHEN @MinBet = 0 THEN 8 ELSE 1 END 
					AND gh.IsFreeGame = 0
    		    ORDER BY gh.DatetimeUtc DESC OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY)
    		    SELECT Id, GameTransactionId,CreatedOnUtc, Type=GameResultType, Bet, Win, UserId, UserName, Currency, GameId, GameName, OperatorTag, PlatformType FROM tem';
    
    	    SET	@def = N'@STime DATETIME, @ETime DATETIME, @UserId INT, @MinBet DECIMAL, @OffsetRows INT, @PageSize INT, @TrxId	BIGINT';
    
    	    EXECUTE sp_executesql @sql, @def, @STime=@STime, @ETime=@ETime, @UserId=@UserId, @MinBet=@MinBet, @OffsetRows=@OffsetRows, @PageSize=@PageSize, @TrxId=@TrxId
END

GO
/****** Object:  StoredProcedure [dbo].[USERHISTORY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[USERHISTORY]
	@OperatorId		INT = NULL,
	@GameId			INT = NULL,
	@UserId			INT = NULL,
	@UserName		NVARCHAR(255) = NULL,
	@TrxId			BIGINT = NULL,
	@GameTrxType	INT = NULL,
	@StartDateInUTC	DATETIME,
	@EndDateInUTC	DATETIME,
	@IsDemo			BIT			= NULL,
	@PlatformType	NVARCHAR(255),
	@OffsetRows		INT,
	@PageSize		INT
AS
BEGIN
	DECLARE @List NVARCHAR(255) = @PlatformType
	DECLARE @Delimiter  NVARCHAR(255) = ','
	DECLARE @ll INT = LEN(@List) + 1, @ld INT = LEN(@Delimiter);

	WITH cte AS
		(
			SELECT
				[start] = 1,
				[end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							@List, 1), 0), @ll),
				[value] = SUBSTRING(@List, 1, 
							COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							@List, 1), 0), @ll) - 1)
			UNION ALL
			SELECT
				[start] = CONVERT(INT, [end]) + @ld,
				[end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							@List, [end] + @ld), 0), @ll),
				[value] = SUBSTRING(@List, [end] + @ld, 
							COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							@List, [end] + @ld), 0), @ll)-[end]-@ld)
			FROM cte
			WHERE [end] < @ll
		),
		tem AS
		(
			SELECT
				gh.Id,
				gh.GameTransactionId,
				CreatedOnUtc = gh.DateTimeUtc,
				gh.GameResultType,
				gh.HistoryXML,
				Bet = 
					(
						CASE
							WHEN sbp.IsSideBet = 1 AND gh.GameResultType = 1 THEN gh.bet * 2
							WHEN gh.IsFreeGame = 1 THEN 0
							WHEN gh.GameResultType IN (9, 10) THEN 0
							ELSE gh.bet
						END
					),
				gh.Win,
				UserId = u.Id,
				UserName = u.Name,
				Currency = c.IsoCode,
				GameId = g.Id,
				GameName = g.Name,
				OperatorTag = o.Tag,
				gh.PlatformType,
				IsVoid = gh.IsDeleted,
				IsFreeGame = gh.IsFreeGame,
				IsSideBet=ISNULL(sbp.IsSideBet,0)
			FROM GameHistory AS gh WITH (NOLOCK,index(IX_DateTimeUtc))
			LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
			INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
			INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
			INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id
			INNER JOIN Operator o WITH (NOLOCK) ON o.Id = u.OperatorId
			WHERE 
				gh.DateTimeUtc BETWEEN @StartDateInUTC AND @EndDateInUTC
				AND (@IsDemo IS NULL OR u.IsDemo = @IsDemo)
				AND (@OperatorId IS NULL  OR u.OperatorId = @OperatorId)
				AND (@GameId IS NULL OR gh.GameId = @GameId)
				AND (@UserId IS NULL OR gh.UserId = @UserId)
				AND (@TrxId IS NULl OR gh.GameTransactionId = @TrxId)
				AND (@GameTrxType IS NULL OR (gh.GameResultType = @GameTrxType AND @GameTrxType = 1) OR (gh.GameResultType > 1 AND @GameTrxType > 1))
				AND gh.PlatformType IN (SELECT [value] FROM cte WHERE LEN([value]) > 0)
			ORDER BY gh.DatetimeUtc DESC OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
		)

		SELECT 
			Id,
			GameTransactionId,
			CreatedOnUtc,
			Type = CASE WHEN GameResultType = 8 AND SUBSTRING(HistoryXML, 8, 8) = 'type="b"' THEN 5 ELSE GameResultType END,
			Bet,
			Win,
			UserId,
			UserName,
			Currency,
			GameId,
			GameName,
			OperatorTag,
			PlatformType,
			IsVoid,
			IsFreeGame,
			IsSideBet
		FROM tem
END

GO
/****** Object:  StoredProcedure [dbo].[USERHISTORY_20170101]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
        create PROCEDURE [dbo].[USERHISTORY_20170101]
	            @OperatorId		INT = NULL,
	            @GameId			INT = NULL,
	            @UserId			INT = NULL,
	            @UserName		NVARCHAR(255) = NULL,
	            @TrxId			BIGINT = NULL,
	            @GameTrxType	INT = NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
	            @PlatformType	NVARCHAR(255),
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
            BEGIN

        declare @OperatorId_n		INT ,
	            @GameId_n			INT,
	            @UserId_n			INT,
	            @UserName_n		NVARCHAR(255),
	            @TrxId_n			BIGINT,
	            @GameTrxType_n	INT,
	            @StartDateInUTC_n	DATETIME,
	            @EndDateInUTC_n	DATETIME,
	            @IsDemo_n			BIT,
	            @PlatformType_n	NVARCHAR(255),
	            @OffsetRows_n		INT,
	            @PageSize_n		INT

			set @OperatorId_n=@OperatorId;
			set @GameId_n=@GameId;
			set @UserId_n=@UserId;
			set @UserName_n=@UserName;
			set @TrxId_n=@TrxId;
			set @GameTrxType_n=@GameTrxType;
			set @StartDateInUTC_n=@StartDateInUTC;
			set @EndDateInUTC_n=@EndDateInUTC;
			set @IsDemo_n=@IsDemo;
			set @PlatformType_n=@PlatformType;
			set @OffsetRows_n=@OffsetRows;
			set @PageSize_n=@PageSize;

	            DECLARE @List NVARCHAR(255) = @PlatformType_n
	            DECLARE @Delimiter  NVARCHAR(255) = ','
	            DECLARE @ll INT = LEN(@List) + 1, @ld INT = LEN(@Delimiter);

	            WITH cte AS
		            (
			            SELECT
				            [start] = 1,
				            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, 1), 0), @ll),
				            [value] = SUBSTRING(@List, 1, 
							            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, 1), 0), @ll) - 1)
			            UNION ALL
			            SELECT
				            [start] = CONVERT(INT, [end]) + @ld,
				            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, [end] + @ld), 0), @ll),
				            [value] = SUBSTRING(@List, [end] + @ld, 
							            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, [end] + @ld), 0), @ll)-[end]-@ld)
			            FROM cte
			            WHERE [end] < @ll
		            ),
		            tem AS
		            (
			            SELECT
				            gh.Id,
				            gh.GameTransactionId,
				            CreatedOnUtc = gh.DateTimeUtc,
				            gh.GameResultType,
				            gh.HistoryXML,
							Bet = IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,IIF(gh.IsFreeGame=0,gh.bet,0)),
				            gh.Win,
				            UserId = u.Id,
				            UserName = u.Name,
				            Currency = c.IsoCode,
				            GameId = g.Id,
				            GameName = g.Name,
				            OperatorTag = o.Tag,
				            gh.PlatformType,
				            IsVoid = gh.IsDeleted,
							IsFreeGame = gh.IsFreeGame,
							IsSideBet=ISNULL(sbp.IsSideBet,0)
			            FROM GameHistory AS gh WITH (NOLOCK,index(IX_DateTimeUtc))
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
			            INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
			            INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
			            INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id
			            INNER JOIN Operator o WITH (NOLOCK) ON o.Id = u.OperatorId
			            WHERE 
				            gh.DateTimeUtc BETWEEN @StartDateInUTC_n AND @EndDateInUTC_n
				            AND (@IsDemo_n IS NULL OR u.IsDemo = @IsDemo_n)
				            AND (@OperatorId_n IS NULL  OR u.OperatorId = @OperatorId_n)
				            AND (@GameId_n IS NULL OR gh.GameId = @GameId_n)
				            AND (@UserId_n IS NULL OR gh.UserId = @UserId_n)
				            AND (@TrxId_n IS NULl OR gh.GameTransactionId = @TrxId_n)
							AND (@GameTrxType_n IS NULL OR (gh.GameResultType = @GameTrxType_n AND @GameTrxType_n = 1) OR (gh.GameResultType > 1 AND @GameTrxType_n > 1))
				            AND gh.PlatformType IN (SELECT [value] FROM cte WHERE LEN([value]) > 0)
			            ORDER BY gh.DatetimeUtc DESC OFFSET @OffsetRows_n ROWS FETCH NEXT @PageSize_n ROWS ONLY 
		            )

		            SELECT 
			            Id,
			            GameTransactionId,
			            CreatedOnUtc,
			            Type = CASE WHEN GameResultType = 8 AND SUBSTRING(HistoryXML, 8, 8) = 'type=""b""' THEN 5 ELSE GameResultType END,
			            Bet,
			            Win,
			            UserId,
			            UserName,
			            Currency,
			            GameId,
			            GameName,
			            OperatorTag,
			            PlatformType,
			            IsVoid,
						IsFreeGame,
						IsSideBet
		            FROM tem 
            END
GO
/****** Object:  StoredProcedure [dbo].[USERHISTORY_dba]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
			create PROCEDURE [dbo].[USERHISTORY_dba]
	            @OperatorId		INT = NULL,
	            @GameId			INT = NULL,
	            @UserId			INT = NULL,
	            @UserName		NVARCHAR(255) = NULL,
	            @TrxId			BIGINT = NULL,
	            @GameTrxType	INT = NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
	            @PlatformType	NVARCHAR(255),
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
            BEGIN
	            DECLARE @List NVARCHAR(255) = @PlatformType
	            DECLARE @Delimiter  NVARCHAR(255) = ','
	            DECLARE @ll INT = LEN(@List) + 1, @ld INT = LEN(@Delimiter);

	            WITH cte AS
		            (
			            SELECT
				            [start] = 1,
				            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, 1), 0), @ll),
				            [value] = SUBSTRING(@List, 1, 
							            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, 1), 0), @ll) - 1)
			            UNION ALL
			            SELECT
				            [start] = CONVERT(INT, [end]) + @ld,
				            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, [end] + @ld), 0), @ll),
				            [value] = SUBSTRING(@List, [end] + @ld, 
							            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, [end] + @ld), 0), @ll)-[end]-@ld)
			            FROM cte
			            WHERE [end] < @ll
		            ),
		            tem AS
		            (
			            SELECT
				            gh.Id,
				            gh.GameTransactionId,
				            CreatedOnUtc = gh.DateTimeUtc,
				            gh.GameResultType,
				            gh.HistoryXML,
				            Bet = IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet),
				            gh.Win,
				            UserId = u.Id,
				            UserName = u.Name,
				            Currency = c.IsoCode,
				            GameId = g.Id,
				            GameName = g.Name,
				            OperatorTag = o.Tag,
				            gh.PlatformType,
				            IsVoid = gh.IsDeleted,
							sbp.IsSideBet
			            FROM GameHistory AS gh WITH (NOLOCK,index(IX_DateTimeUtc))
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
			            INNER JOIN GameTransaction gt WITH (NOLOCK) ON gh.GameTransactionId = gt.Id
			            INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
			            INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
			            INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id
			            INNER JOIN Operator o WITH (NOLOCK) ON o.Id = u.OperatorId
			            WHERE 
				            gh.DateTimeUtc BETWEEN @StartDateInUTC AND @EndDateInUTC
				            AND (@IsDemo IS NULL OR u.IsDemo = @IsDemo)
				            AND (@OperatorId IS NULL  OR u.OperatorId = @OperatorId)
				            AND (@GameId IS NULL OR gh.GameId = @GameId)
				            AND (@UserId IS NULL OR gh.UserId = @UserId)
				            AND (@TrxId IS NULL OR gt.Id = @TrxId)
				            AND (@GameTrxType IS NULL OR (gt.[Type] = @GameTrxType AND @GameTrxType = 1) OR (gt.[Type] > 1 AND @GameTrxType > 1))
				            AND gh.PlatformType IN (SELECT [value] FROM cte WHERE LEN([value]) > 0)
			            ORDER BY gh.DatetimeUtc DESC OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
		            )

		            SELECT 
			            Id,
			            GameTransactionId,
			            CreatedOnUtc,
			            Type = CASE WHEN GameResultType = 8 AND SUBSTRING(HistoryXML, 8, 8) = 'type=""b""' THEN 5 ELSE GameResultType END,
			            Bet,
			            Win,
			            UserId,
			            UserName,
			            Currency,
			            GameId,
			            GameName,
			            OperatorTag,
			            PlatformType,
			            IsVoid,
						IsSideBet
		            FROM tem
            END
GO
/****** Object:  StoredProcedure [dbo].[USERHISTORY_dba02]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
        CREATE PROCEDURE [dbo].[USERHISTORY_dba02]
	            @OperatorId		INT = NULL,
	            @GameId			INT = NULL,
	            @UserId			INT = NULL,
	            @UserName		NVARCHAR(255) = NULL,
	            @TrxId			BIGINT = NULL,
	            @GameTrxType	INT = NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
	            @PlatformType	NVARCHAR(255),
	            @OffsetRows		INT,
	            @PageSize		INT
            AS
            BEGIN

        declare @OperatorId_n		INT ,
	            @GameId_n			INT,
	            @UserId_n			INT,
	            @UserName_n		NVARCHAR(255),
	            @TrxId_n			BIGINT,
	            @GameTrxType_n	INT,
	            @StartDateInUTC_n	DATETIME,
	            @EndDateInUTC_n	DATETIME,
	            @IsDemo_n			BIT,
	            @PlatformType_n	NVARCHAR(255),
	            @OffsetRows_n		INT,
	            @PageSize_n		INT

			set @OperatorId_n=@OperatorId;
			set @GameId_n=@GameId;
			set @UserId_n=@UserId;
			set @UserName_n=@UserName;
			set @TrxId_n=@TrxId;
			set @GameTrxType_n=@GameTrxType;
			set @StartDateInUTC_n=@StartDateInUTC;
			set @EndDateInUTC_n=@EndDateInUTC;
			set @IsDemo_n=@IsDemo;
			set @PlatformType_n=@PlatformType;
			set @OffsetRows_n=@OffsetRows;
			set @PageSize_n=@PageSize;

	            DECLARE @List NVARCHAR(255) = @PlatformType_n
	            DECLARE @Delimiter  NVARCHAR(255) = ','
	            DECLARE @ll INT = LEN(@List) + 1, @ld INT = LEN(@Delimiter);

	            WITH cte AS
		            (
			            SELECT
				            [start] = 1,
				            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, 1), 0), @ll),
				            [value] = SUBSTRING(@List, 1, 
							            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, 1), 0), @ll) - 1)
			            UNION ALL
			            SELECT
				            [start] = CONVERT(INT, [end]) + @ld,
				            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, [end] + @ld), 0), @ll),
				            [value] = SUBSTRING(@List, [end] + @ld, 
							            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
							            @List, [end] + @ld), 0), @ll)-[end]-@ld)
			            FROM cte
			            WHERE [end] < @ll
		            ),
		            tem AS
		            (
			            SELECT
				            gh.Id,
				            gh.GameTransactionId,
				            CreatedOnUtc = gh.DateTimeUtc,
				            gh.GameResultType,
				            gh.HistoryXML,
							Bet = IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,IIF(gh.IsFreeGame=0,gh.bet,0)),
				            gh.Win,
				            UserId = u.Id,
				            UserName = u.Name,
				            Currency = c.IsoCode,
				            GameId = g.Id,
				            GameName = g.Name,
				            OperatorTag = o.Tag,
				            gh.PlatformType,
				            IsVoid = gh.IsDeleted,
							IsFreeGame = gh.IsFreeGame,
							IsSideBet=ISNULL(sbp.IsSideBet,0)
			            FROM GameHistory AS gh WITH (NOLOCK,index(IX_DateTimeUtc))
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
			            INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
			            INNER JOIN Game g WITH (NOLOCK) ON gh.GameId = g.Id
			            INNER JOIN Currency c WITH (NOLOCK) ON u.CurrencyId = c.Id
			            INNER JOIN Operator o WITH (NOLOCK) ON o.Id = u.OperatorId
			            WHERE 
				            gh.DateTimeUtc BETWEEN @StartDateInUTC_n AND @EndDateInUTC_n
				            AND (@IsDemo_n IS NULL OR u.IsDemo = @IsDemo_n)
				            AND (@OperatorId_n IS NULL  OR u.OperatorId = @OperatorId_n)
				            AND (@GameId_n IS NULL OR gh.GameId = @GameId_n)
				            AND (@UserId_n IS NULL OR gh.UserId = @UserId_n)
				            AND (@TrxId_n IS NULl OR gh.GameTransactionId = @TrxId_n)
							AND (@GameTrxType_n IS NULL OR (gh.GameResultType = @GameTrxType_n AND @GameTrxType_n = 1) OR (gh.GameResultType > 1 AND @GameTrxType_n > 1))
				            AND gh.PlatformType IN (SELECT [value] FROM cte WHERE LEN([value]) > 0)
			            ORDER BY gh.DatetimeUtc DESC OFFSET @OffsetRows_n ROWS FETCH NEXT @PageSize_n ROWS ONLY 
		            )

		            SELECT 
			            Id,
			            GameTransactionId,
			            CreatedOnUtc,
			            Type = CASE WHEN GameResultType = 8 AND SUBSTRING(HistoryXML, 8, 8) = 'type=""b""' THEN 5 ELSE GameResultType END,
			            Bet,
			            Win,
			            UserId,
			            UserName,
			            Currency,
			            GameId,
			            GameName,
			            OperatorTag,
			            PlatformType,
			            IsVoid,
						IsFreeGame,
						IsSideBet
		            FROM tem 
            END
GO
/****** Object:  StoredProcedure [dbo].[USERHISTORYSUMMARY]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USERHISTORYSUMMARY]
	            @OperatorId		INT = NULL,
	            @GameId			INT = NULL,
	            @UserId			INT = NULL,
	            @UserName		NVARCHAR(255) = NULL,
	            @TrxId			BIGINT = NULL,
	            @GameTrxType	INT = NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
	            @PlatformType	NVARCHAR(255)
            AS
            BEGIN

	        declare @OperatorId_n		INT,
					@GameId_n			INT,
					@UserId_n			INT ,
					@UserName_n		NVARCHAR(255),
					@TrxId_n		BIGINT,
					@GameTrxType_n	INT,
					@StartDateInUTC_n	DATETIME,
					@EndDateInUTC_n	DATETIME,
					@IsDemo_n			BIT,
					@PlatformType_n	NVARCHAR(255)


					set @OperatorId_n=@OperatorId;
					set @GameId_n=@GameId;
					set @UserId_n=@UserId
					set @UserName_n=@UserName
					set @TrxId_n=@TrxId
					set @GameTrxType_n=@GameTrxType
					set @StartDateInUTC_n=@StartDateInUTC;
					set @EndDateInUTC_n=@EndDateInUTC;
					set @IsDemo_n=@IsDemo
					set @PlatformType_n=@PlatformType;



	            DECLARE @List NVARCHAR(255) = @PlatformType_n
	            DECLARE @Delimiter  NVARCHAR(255) = ','
	            DECLARE @ll INT = LEN(@List) + 1, @ld INT = LEN(@Delimiter);
 
	            WITH cte AS
	            (
		            SELECT
			            [start] = 1,
			            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
						            @List, 1), 0), @ll),
			            [value] = SUBSTRING(@List, 1, 
						            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
						            @List, 1), 0), @ll) - 1)
		            UNION ALL
		            SELECT
			            [start] = CONVERT(INT, [end]) + @ld,
			            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
						            @List, [end] + @ld), 0), @ll),
			            [value] = SUBSTRING(@List, [end] + @ld, 
						            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
						            @List, [end] + @ld), 0), @ll)-[end]-@ld)
		            FROM cte
		            WHERE [end] < @ll
	            )

	            SELECT
		            TotalRecords = COUNT(gh.Id), 
		            TotalBet = SUM(IIF(gh.IsFreeGame=1, 0, IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet))),
		            TotalWin = SUM(gh.Win),
		            TotalBetRMB = SUM(IIF(gh.IsFreeGame=1, 0, IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)) * gh.ExchangeRate),
		            TotalWinRMB = SUM(gh.Win * gh.ExchangeRate)
	            FROM GameHistory AS gh WITH (NOLOCK,index(IX_DateTimeUtc))
				LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
	            --INNER JOIN GameTransaction gt WITH (NOLOCK) ON gh.GameTransactionId = gt.Id
	            INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
	            WHERE 
		            gh.DateTimeUtc BETWEEN @StartDateInUTC_n AND @EndDateInUTC_n
		            AND (@IsDemo_n IS NULL OR u.IsDemo = @IsDemo_n)
		            AND (@OperatorId_n IS NULL OR u.OperatorId = @OperatorId_n)
		            AND (@GameId_n IS NULL OR gh.GameId = @GameId_n)
		            AND (@UserId_n IS NULL OR gh.UserId = @UserId_n)
		            --AND (@TrxId IS NULL OR gt.Id = @TrxId)
					AND (@TrxId_n IS NULl OR gh.GameTransactionId = @TrxId_n)
		            AND (@GameTrxType_n IS NULL OR (gh.GameResultType = @GameTrxType_n AND @GameTrxType_n = 1) OR (gh.GameResultType > 1 AND @GameTrxType_n > 1))
		            AND gh.PlatformType IN (SELECT [value] FROM cte WHERE LEN([value]) > 0)
            END
GO
/****** Object:  StoredProcedure [dbo].[USERHISTORYSUMMARY_dba]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
			create PROCEDURE [dbo].[USERHISTORYSUMMARY_dba]
	            @OperatorId		INT = NULL,
	            @GameId			INT = NULL,
	            @UserId			INT = NULL,
	            @UserName		NVARCHAR(255) = NULL,
	            @TrxId			BIGINT = NULL,
	            @GameTrxType	INT = NULL,
	            @StartDateInUTC	DATETIME,
	            @EndDateInUTC	DATETIME,
	            @IsDemo			BIT			= NULL,
	            @PlatformType	NVARCHAR(255)
            AS
            BEGIN

	declare 	    @OperatorId_n		INT,
					@GameId_n			INT,
					@UserId_n			INT ,
					@UserName_n		NVARCHAR(255),
					@TrxId_n		BIGINT,
					@GameTrxType_n	INT,
					@StartDateInUTC_n	DATETIME,
					@EndDateInUTC_n	DATETIME,
					@IsDemo_n			BIT,
					@PlatformType_n	NVARCHAR(255)


					set @OperatorId_n=@OperatorId;
					set @GameId_n=@GameId;
					set @UserId_n=@UserId
					set @UserName_n=@UserName
					set @TrxId_n=@TrxId
					set @GameTrxType_n=@GameTrxType
					set @StartDateInUTC_n=@StartDateInUTC;
					set @EndDateInUTC_n=@EndDateInUTC;
					set @IsDemo_n=@IsDemo
					set @PlatformType_n=@PlatformType;



	            DECLARE @List NVARCHAR(255) = @PlatformType_n
	            DECLARE @Delimiter  NVARCHAR(255) = ','
	            DECLARE @ll INT = LEN(@List) + 1, @ld INT = LEN(@Delimiter);
 
	            WITH cte AS
	            (
		            SELECT
			            [start] = 1,
			            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
						            @List, 1), 0), @ll),
			            [value] = SUBSTRING(@List, 1, 
						            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
						            @List, 1), 0), @ll) - 1)
		            UNION ALL
		            SELECT
			            [start] = CONVERT(INT, [end]) + @ld,
			            [end]   = COALESCE(NULLIF(CHARINDEX(@Delimiter, 
						            @List, [end] + @ld), 0), @ll),
			            [value] = SUBSTRING(@List, [end] + @ld, 
						            COALESCE(NULLIF(CHARINDEX(@Delimiter, 
						            @List, [end] + @ld), 0), @ll)-[end]-@ld)
		            FROM cte
		            WHERE [end] < @ll
	            )

	            SELECT
		            TotalRecords = COUNT(gh.Id), 
		            TotalBet = SUM(IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet)),
		            TotalWin = SUM(gh.Win),
		            TotalBetRMB = SUM(IIF(sbp.IsSideBet=1 AND gh.GameResultType=1,gh.bet*2,gh.bet) * gh.ExchangeRate),
		            TotalWinRMB = SUM(gh.Win * gh.ExchangeRate)
	            FROM GameHistory AS gh WITH (NOLOCK,index(IX_DateTimeUtc))
				LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
	            --INNER JOIN GameTransaction gt WITH (NOLOCK) ON gh.GameTransactionId = gt.Id
	            INNER JOIN [User] u WITH (NOLOCK) ON gh.UserId = u.Id
	            WHERE 
		            gh.DateTimeUtc BETWEEN @StartDateInUTC_n AND @EndDateInUTC_n
		            AND (@IsDemo_n IS NULL OR u.IsDemo = @IsDemo_n)
		            AND (@OperatorId_n IS NULL OR u.OperatorId = @OperatorId_n)
		            AND (@GameId_n IS NULL OR gh.GameId = @GameId_n)
		            AND (@UserId_n IS NULL OR gh.UserId = @UserId_n)
		            --AND (@TrxId IS NULL OR gt.Id = @TrxId)
					AND (@TrxId_n IS NULl OR gh.GameTransactionId = @TrxId_n)
		            AND (@GameTrxType_n IS NULL OR (gh.GameResultType = @GameTrxType_n AND @GameTrxType_n = 1) OR (gh.GameResultType > 1 AND @GameTrxType_n > 1))
		            AND gh.PlatformType IN (SELECT [value] FROM cte WHERE LEN([value]) > 0)
            END
GO
/****** Object:  UserDefinedFunction [dbo].[Concate]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[Concate] (
      @InputString                  VARCHAR(8000),
      @Delimiter                    VARCHAR(50),
	  @Multiplier					DECIMAL (23,2)
)

RETURNS VARCHAR(8000)

AS
BEGIN

	Declare @Result varchar(8000);

      IF @Delimiter = ' '
      BEGIN
            SET @Delimiter = ','
            SET @InputString = REPLACE(@InputString, ' ', @Delimiter)
      END

      IF (@Delimiter IS NULL OR @Delimiter = '')
            SET @Delimiter = ','

--INSERT INTO @Items VALUES (@Delimiter) -- Diagnostic
--INSERT INTO @Items VALUES (@InputString) -- Diagnostic

      DECLARE @Item                 VARCHAR(8000)
      DECLARE @ItemList       VARCHAR(8000)
      DECLARE @DelimIndex     INT

      SET @ItemList = @InputString
      SET @DelimIndex = CHARINDEX(@Delimiter, @ItemList, 0)
      WHILE (@DelimIndex != 0)
      BEGIN
            SET @Item = SUBSTRING(@ItemList, 0, @DelimIndex)
            SET @Result = CONCAT(@Result, CAST(ROUND(CAST(@Item AS DECIMAL(23,2)) * @Multiplier, 0) AS INT), @Delimiter)

            -- Set @ItemList = @ItemList minus one less item
            SET @ItemList = SUBSTRING(@ItemList, @DelimIndex+1, LEN(@ItemList)-@DelimIndex)
            SET @DelimIndex = CHARINDEX(@Delimiter, @ItemList, 0)
      END -- End WHILE

      IF @Item IS NOT NULL -- At least one delimiter was encountered in @InputString
      BEGIN
			SET @Result = CONCAT(@Result, CAST(ROUND(CAST(@ItemList AS DECIMAL(23,2)) * @Multiplier, 0) AS INT))
      END

      -- No delimiters were encountered in @InputString, so just return @InputString
      ELSE 
		  SET @Result = CAST(ROUND(CAST(@InputString AS DECIMAL(23,2)) * @Multiplier, 0) AS INT)

      RETURN @Result

END -- End Function
GO
/****** Object:  UserDefinedFunction [dbo].[fnSplitString]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnSplitString] 
( 
    @string NVARCHAR(MAX), 
    @delimiter CHAR(1) 
) 
RETURNS @output TABLE(splitdata NVARCHAR(MAX) 
) 
BEGIN 
    DECLARE @start INT, @end INT 
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) 
    WHILE @start < LEN(@string) + 1 BEGIN 
        IF @end = 0  
            SET @end = LEN(@string) + 1
       
        INSERT INTO @output (splitdata)  
        VALUES(SUBSTRING(@string, @start, @end - @start)) 
        SET @start = @end + 1 
        SET @end = CHARINDEX(@delimiter, @string, @start)
        
    END 
    RETURN 
END

GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Account]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OperatorId] [int] NOT NULL,
	[Username] [nvarchar](128) NULL,
	[Password] [nvarchar](32) NULL,
	[RealName] [nvarchar](255) NULL,
	[RoleId] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[LastLoginUtc] [datetime] NULL,
	[DefaultOffSet] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.Account] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Bonus]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Bonus](
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Guid] [nvarchar](32) NULL,
	[Data] [varbinary](max) NULL,
	[BonusType] [nvarchar](128) NULL,
	[Version] [int] NOT NULL,
	[IsOptional] [bit] NOT NULL,
	[IsStarted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[JsonData] [nvarchar](max) NULL,
	[BnsClsId] [int] NOT NULL DEFAULT ((0)),
	[ClientId] [int] NOT NULL DEFAULT ((0)),
	[RoundId] [bigint] NOT NULL DEFAULT ((0)),
	[BetReference] [varchar](255) NULL,
	[Order] [int] NOT NULL DEFAULT ((0)),
	[IsFreeGame] [bit] NOT NULL DEFAULT ((0)),
	[CampaignId] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.Bonus] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BonusIncomplete]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BonusIncomplete](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Guid] [nvarchar](32) NULL,
	[Data] [varbinary](max) NULL,
	[BonusType] [nvarchar](128) NULL,
	[Version] [int] NOT NULL,
	[IsOptional] [bit] NOT NULL,
	[IsStarted] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[JsonData] [nvarchar](max) NULL,
	[BnsClsId] [int] NOT NULL DEFAULT ((0)),
	[ClientId] [int] NOT NULL DEFAULT ((0)),
	[RoundId] [bigint] NOT NULL DEFAULT ((0)),
	[BetReference] [varchar](255) NULL,
	[Order] [int] NOT NULL DEFAULT ((0)),
	[IsFreeGame] [bit] NOT NULL DEFAULT ((0)),
	[CampaignId] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.BonusIncomplete] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Captcha]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Captcha](
	[Key] [nvarchar](128) NOT NULL,
	[Code] [nvarchar](4) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.Captcha] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CoinDenominationReportInfo]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CoinDenominationReportInfo](
	[ChangeTime] [datetime] NOT NULL,
	[OffsetId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[LineBet] [decimal](23, 8) NOT NULL,
	[TrxCount] [int] NOT NULL,
	[SpinCount] [int] NOT NULL,
	[FreeSpinCount] [int] NOT NULL,
	[BonusCount] [int] NOT NULL,
	[GambleCount] [int] NOT NULL,
	[TotalBet] [decimal](23, 8) NOT NULL,
	[TotalWin] [decimal](23, 8) NOT NULL,
	[TotalSpinWin] [decimal](23, 8) NOT NULL,
	[TotalFreeSpinWin] [decimal](23, 8) NOT NULL,
	[TotalBonusWin] [decimal](23, 8) NOT NULL,
	[TotalGambleWin] [decimal](23, 8) NOT NULL,
	[TotalBetL] [decimal](23, 8) NOT NULL,
	[TotalWinL] [decimal](23, 8) NOT NULL,
	[TotalSpinWinL] [decimal](23, 8) NOT NULL,
	[TotalFreeSpinWinL] [decimal](23, 8) NOT NULL,
	[TotalBonusWinL] [decimal](23, 8) NOT NULL,
	[TotalGambleWinL] [decimal](23, 8) NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[Multiplier] [int] NOT NULL DEFAULT ((1)),
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_dbo.CoinDenominationReportInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ConfigurationSetting]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigurationSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NULL,
	[Value] [nvarchar](1024) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.ConfigurationSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Counter]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Counter](
	[Id] [int] NOT NULL,
	[Value] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.Counter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Currency]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsoCode] [nvarchar](8) NULL,
	[DisplayCode] [nvarchar](8) NULL,
	[Description] [nvarchar](128) NULL,
	[ExchangeRateToCredit] [decimal](23, 8) NOT NULL,
	[IsVisible] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.Currency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExchangeRate]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExchangeRate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EffectiveTimeUtc] [datetime] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[CurrencyCode] [nvarchar](8) NULL,
	[TargetCurrencyId] [int] NOT NULL,
	[Rate] [decimal](23, 8) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.ExchangeRate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FaultTransaction]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FaultTransaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GameTransactionId] [bigint] NOT NULL,
	[ErrorMessage] [nvarchar](54) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_FaultTransaction] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FRCoinSetting]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FRCoinSetting](
	[FreeRoundId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[LineBet] [decimal](23, 8) NOT NULL,
	[TotalBet] [decimal](23, 8) NOT NULL,
 CONSTRAINT [PK_dbo.FRCoinSetting] PRIMARY KEY CLUSTERED 
(
	[FreeRoundId] ASC,
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FreeRound]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreeRound](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[OperatorId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Lines] [int] NOT NULL,
	[Multiplier] [int] NOT NULL,
	[LimitPerPlayer] [int] NOT NULL,
	[RelativeDuration] [int] NOT NULL,
	[Platform] [int] NOT NULL,
	[StartDateUtc] [datetime] NOT NULL,
	[EndDateUtc] [datetime] NOT NULL,
	[Template] [nvarchar](max) NULL,
	[MessageTitle] [nvarchar](max) NULL,
	[MessageContent] [nvarchar](max) NULL,
	[OwnerId] [int] NOT NULL,
	[IsCancelled] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.FreeRound] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FreeRoundData]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreeRoundData](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CampaignId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[TimeStart] [datetime] NOT NULL DEFAULT (getutcdate()),
	[TimeEnd] [datetime] NOT NULL DEFAULT (getutcdate()),
	[TimeClaimed] [datetime] NULL,
	[Bet] [decimal](23, 8) NOT NULL,
	[Multiplier] [int] NOT NULL,
	[Lines] [int] NOT NULL,
	[Counter] [int] NOT NULL,
	[State] [tinyint] NOT NULL,
	[IsFinish] [bit] NOT NULL DEFAULT ((0)),
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.FreeRoundData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FreeRoundGameHistory]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreeRoundGameHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GameHistoryId] [bigint] NOT NULL,
	[FreeRoundId] [int] NOT NULL,
 CONSTRAINT [PK_FreeRoundGameHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FreeRoundReportInfo]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreeRoundReportInfo](
	[FreeRoundId] [int] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Platform] [int] NOT NULL,
	[TrxCount] [int] NOT NULL,
	[Bet] [decimal](23, 8) NOT NULL,
	[Win] [decimal](23, 8) NOT NULL,
	[BetL] [decimal](23, 8) NOT NULL,
	[WinL] [decimal](23, 8) NOT NULL,
	[TimeFirstBet] [datetime] NOT NULL,
	[TimeLastBet] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.FreeRoundReportInfo] PRIMARY KEY CLUSTERED 
(
	[FreeRoundId] ASC,
	[OperatorId] ASC,
	[UserId] ASC,
	[GameId] ASC,
	[Platform] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FRPlayer]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FRPlayer](
	[FreeRoundId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.FRPlayer] PRIMARY KEY CLUSTERED 
(
	[FreeRoundId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Game]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Game](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GameType] [int] NOT NULL,
	[Name] [nvarchar](128) NULL,
	[Lines] [int] NOT NULL,
	[RtpLevel] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[IsDisabled] [bit] NOT NULL DEFAULT ((0)),
	[DisableOperators] [varchar](max) NOT NULL DEFAULT (''),
	[IsBetAllLines] [bit] NOT NULL DEFAULT ((0)),
	[Params] [nvarchar](255) NULL,
	[IsSideBet] [bit] NOT NULL DEFAULT ((0)),
	[IsFreeRoundEnabled] [bit] NOT NULL DEFAULT ((1)),
 CONSTRAINT [PK_dbo.Game] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GameHistory]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DateTimeUtc] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[Bet] [decimal](23, 8) NOT NULL,
	[Win] [decimal](23, 8) NOT NULL,
	[ExchangeRate] [decimal](23, 8) NOT NULL,
	[GameResultType] [int] NOT NULL,
	[XmlType] [int] NOT NULL,
	[ResponseXml] [nvarchar](max) NULL,
	[HistoryXml] [nvarchar](max) NULL,
	[IsHistory] [bit] NOT NULL,
	[IsReport] [bit] NOT NULL,
	[PlatformType] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[GameTransactionId] [bigint] NOT NULL DEFAULT ((0)),
	[SpinTransactionId] [bigint] NULL,
	[IsFreeGame] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.GameHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameHistory_35days]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameHistory_35days](
	[Id] [bigint] NOT NULL,
	[DateTimeUtc] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[Bet] [decimal](23, 8) NOT NULL,
	[Win] [decimal](23, 8) NOT NULL,
	[ExchangeRate] [decimal](23, 8) NOT NULL,
	[GameResultType] [int] NOT NULL,
	[XmlType] [int] NOT NULL,
	[ResponseXml] [nvarchar](max) NULL,
	[HistoryXml] [nvarchar](max) NULL,
	[IsHistory] [bit] NOT NULL,
	[IsReport] [bit] NOT NULL,
	[PlatformType] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL CONSTRAINT [DF__GameHisto__Creat__57C7FD4B]  DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[GameTransactionId] [bigint] NOT NULL CONSTRAINT [DF__GameHisto__GameT__58BC2184]  DEFAULT ((0)),
	[SpinTransactionId] [bigint] NULL,
	[IsFreeGame] [bit] NOT NULL CONSTRAINT [DF__GameHisto__IsFre__59B045BD]  DEFAULT ((0)),
 CONSTRAINT [PK_dbo.GameHistory_35days] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameRtp]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameRtp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[RtpLevel] [int] NOT NULL,
	[Rtp] [decimal](23, 8) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.GameRtp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameSetting]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameSetting](
	[GameSettingGroupId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[CoinsDenomination] [nvarchar](max) NOT NULL,
	[CoinsMultiplier] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[GambleMinValue] [decimal](23, 8) NOT NULL DEFAULT ((0)),
	[GambleMaxValue] [decimal](23, 8) NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.GameSetting] PRIMARY KEY CLUSTERED 
(
	[GameSettingGroupId] ASC,
	[GameId] ASC,
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameSettingGroup]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameSettingGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.GameSettingGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameState]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameState](
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Type] [int] NOT NULL,
 CONSTRAINT [PK_dbo.GameState] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameTransaction]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameTransaction](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DateTimeUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.GameTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameTransaction_35days]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameTransaction_35days](
	[Id] [bigint] NOT NULL,
	[DateTimeUtc] [datetime] NOT NULL CONSTRAINT [DF__GameTrans__DateT__5C8CB268]  DEFAULT (getutcdate()),
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL CONSTRAINT [DF__GameTrans__Creat__5D80D6A1]  DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.GameTransaction_35days] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameTransactionError]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameTransactionError](
	[GameTransactionId] [bigint] NOT NULL,
	[Message] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.GameTransactionError] PRIMARY KEY CLUSTERED 
(
	[GameTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Operator]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operator](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tag] [nvarchar](max) NULL,
	[Name] [nvarchar](255) NULL,
	[AuthenticationProviderId] [int] NOT NULL,
	[WalletProviderId] [int] NOT NULL,
	[GameSettingGroupId] [int] NOT NULL,
	[JackpotSettingGroupId] [int] NOT NULL,
	[FunPlayCurrencyId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[FunPlayInitialBalance] [decimal](23, 8) NOT NULL DEFAULT ((0)),
	[FunPlayDemo] [bit] NOT NULL DEFAULT ((0)),
	[AuthenticateURL] [nvarchar](255) NULL,
	[AuthenticateParam] [nvarchar](255) NULL,
	[LoginURL] [nvarchar](255) NULL,
	[OperatorCode] [nvarchar](16) NULL,
	[BackOfficeURL] [nvarchar](255) NULL,
	[EnableRollback] [bit] NOT NULL DEFAULT ((0)),
	[EnableEndGame] [bit] NOT NULL DEFAULT ((0)),
	[UseRMB] [bit] NOT NULL DEFAULT ((0)),
	[FunPlay] [bit] NOT NULL DEFAULT ((0)),
	[ExtraInfo] [nvarchar](max) NULL,
	[EncodeToken] [bit] NOT NULL DEFAULT ((0)),
	[AuthenticateMemberURL] [nvarchar](255) NULL,
	[HasDownload] [bit] NOT NULL,
	[UseOtp][bit] NOT NULL
 CONSTRAINT [PK_dbo.Operator] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlatformReportInfo]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlatformReportInfo](
	[ChangeTime] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[PlatformId] [int] NOT NULL,
	[OffsetId] [int] NOT NULL,
	[FirstTrxId] [bigint] NOT NULL,
	[LastTrxId] [bigint] NOT NULL,
	[TrxCount] [int] NOT NULL,
	[SpinCount] [int] NOT NULL,
	[FreeSpinCount] [int] NOT NULL,
	[BonusCount] [int] NOT NULL,
	[GambleCount] [int] NOT NULL,
	[TotalBetAmount] [decimal](23, 8) NOT NULL,
	[TotalWinAmount] [decimal](23, 8) NOT NULL,
	[TotalNetWinAmount] [decimal](23, 8) NOT NULL,
	[SpinWin] [decimal](23, 8) NOT NULL,
	[FreeSpinWin] [decimal](23, 8) NOT NULL,
	[BonusWin] [decimal](23, 8) NOT NULL,
	[GambleWin] [decimal](23, 8) NOT NULL,
	[TotalBetAmountRMB] [decimal](23, 8) NOT NULL,
	[TotalWinAmountRMB] [decimal](23, 8) NOT NULL,
	[TotalNetWinAmountRMB] [decimal](23, 8) NOT NULL,
	[SpinWinRMB] [decimal](23, 8) NOT NULL,
	[FreeSpinWinRMB] [decimal](23, 8) NOT NULL,
	[BonusWinRMB] [decimal](23, 8) NOT NULL,
	[GambleWinRMB] [decimal](23, 8) NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[SpinBetAmount] [decimal](23, 8) NOT NULL DEFAULT ((0)),
	[GambleBetAmount] [decimal](23, 8) NOT NULL DEFAULT ((0)),
	[SpinBetAmountRMB] [decimal](23, 8) NOT NULL DEFAULT ((0)),
	[GambleBetAmountRMB] [decimal](23, 8) NOT NULL DEFAULT ((0)),
	[IsSideBet] [bit] NOT NULL DEFAULT ((0)),
	[IsFreeGame] [bit] NOT NULL DEFAULT ((0)),
	[FreeRoundId] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_PlatformReportInfo] PRIMARY KEY CLUSTERED 
(
	[ChangeTime] ASC,
	[UserId] ASC,
	[GameId] ASC,
	[PlatformId] ASC,
	[OffsetId] ASC,
	[IsFreeGame] ASC,
	[FreeRoundId] ASC,
	[IsSideBet] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlatformReportInfoTemp]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlatformReportInfoTemp](
	[ChangeTime] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[PlatformId] [int] NOT NULL,
	[OffsetId] [int] NOT NULL,
	[FirstTrxId] [bigint] NOT NULL,
	[LastTrxId] [bigint] NOT NULL,
	[TrxCount] [int] NOT NULL,
	[SpinCount] [int] NOT NULL,
	[FreeSpinCount] [int] NOT NULL,
	[BonusCount] [int] NOT NULL,
	[GambleCount] [int] NOT NULL,
	[TotalBetAmount] [decimal](23, 8) NOT NULL,
	[TotalWinAmount] [decimal](23, 8) NOT NULL,
	[TotalNetWinAmount] [decimal](23, 8) NOT NULL,
	[SpinWin] [decimal](23, 8) NOT NULL,
	[FreeSpinWin] [decimal](23, 8) NOT NULL,
	[BonusWin] [decimal](23, 8) NOT NULL,
	[GambleWin] [decimal](23, 8) NOT NULL,
	[TotalBetAmountRMB] [decimal](23, 8) NOT NULL,
	[TotalWinAmountRMB] [decimal](23, 8) NOT NULL,
	[TotalNetWinAmountRMB] [decimal](23, 8) NOT NULL,
	[SpinWinRMB] [decimal](23, 8) NOT NULL,
	[FreeSpinWinRMB] [decimal](23, 8) NOT NULL,
	[BonusWinRMB] [decimal](23, 8) NOT NULL,
	[GambleWinRMB] [decimal](23, 8) NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[SpinBetAmount] [decimal](23, 8) NOT NULL,
	[GambleBetAmount] [decimal](23, 8) NOT NULL,
	[SpinBetAmountRMB] [decimal](23, 8) NOT NULL,
	[GambleBetAmountRMB] [decimal](23, 8) NOT NULL,
	[IsSideBet] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.PlatformReportInfoTemp] PRIMARY KEY CLUSTERED 
(
	[ChangeTime] ASC,
	[UserId] ASC,
	[GameId] ASC,
	[PlatformId] ASC,
	[OffsetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportInfo]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[StartId] [bigint] NOT NULL,
	[LastId] [bigint] NOT NULL,
	[CountItem] [bigint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[OffsetId] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.ReportInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportInfoTemp]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportInfoTemp](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[StartId] [bigint] NOT NULL,
	[LastId] [bigint] NOT NULL,
	[CountItem] [bigint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[OffsetId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ReportInfotemp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Role]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Active] [bit] NOT NULL,
	[Name] [nvarchar](128) NULL,
	[OperatorId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tournament]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tournament](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Flags] [int] NOT NULL,
	[MinHands] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[ErrorTitle] [nvarchar](max) NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[IsCancelled] [bit] NOT NULL DEFAULT ((0)),
	[CancelledOnUtc] [datetime] NULL,
	[PrizeCopyFrom] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.Tournament] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TournamentReportInfo]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TournamentReportInfo](
	[TournamentId] [int] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Platform] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[TrxCount] [int] NOT NULL,
	[Bet] [decimal](23, 8) NOT NULL,
	[Win] [decimal](23, 8) NOT NULL,
	[BetL] [decimal](23, 8) NOT NULL,
	[WinL] [decimal](23, 8) NOT NULL,
	[TimeFirstBet] [datetime] NOT NULL,
	[TimeLastBet] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TournamentReportInfo] PRIMARY KEY CLUSTERED 
(
	[TournamentId] ASC,
	[OperatorId] ASC,
	[UserId] ASC,
	[GameId] ASC,
	[Platform] ASC,
	[Level] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TournamentReportX]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TournamentReportX](
	[Id] [int] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[SummaryJson] [nvarchar](max) NULL,
	[DetailJson] [nvarchar](max) NULL,
	[LeaderboardJson] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.TournamentReportX] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TRelation]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TRelation](
	[TournamentId] [int] NOT NULL,
	[RelationType] [int] NOT NULL,
	[RelationId] [int] NOT NULL,
	[RelationValue] [decimal](18, 0) NULL,
	[Count] [int] NULL,
	[Rank] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[CurrencyId] [int] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[UserRole] [int] NOT NULL,
	[IsDemo] [bit] NOT NULL,
	[IsBlocked] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[Password] [char](32) NULL,
	[IsBonus] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserGameData]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGameData](
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[TimeStamp] [nvarchar](32) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[Bet] [decimal](23, 8) NOT NULL DEFAULT ((0)),
	[MP] [int] NOT NULL DEFAULT ((0)),
	[Lines] [int] NOT NULL DEFAULT ((0)),
	[GameMode] [int] NOT NULL DEFAULT ((1)),
 CONSTRAINT [PK_dbo.UserGameData] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserGameRtpSetting]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGameRtpSetting](
	[UserId] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.UserGameRtpSetting] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserGameSpinData]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserGameSpinData](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Data] [varbinary](max) NULL,
 CONSTRAINT [PK_UserGameSpinData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserSession]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSession](
	[UserId] [int] NOT NULL,
	[SessionKey] [nvarchar](512) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[ExtraInfo] [nvarchar](1024) NULL,
 CONSTRAINT [PK_dbo.UserSession] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserSessionLog]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSessionLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SessionKey] [nvarchar](512) NULL,
	[IpAddress] [nvarchar](128) NULL,
	[PlatformType] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.UserSessionLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UtcTimeOffset]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UtcTimeOffset](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Offset] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[IsDisabled] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.UtcTimeOffset] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WalletLog]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ServerId] [int] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Retry] [int] NOT NULL,
	[MemberId] [nvarchar](128) NOT NULL,
	[TrxId] [nvarchar](128) NOT NULL,
	[Request] [nvarchar](max) NULL,
	[Rollback] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.WalletLog] PRIMARY KEY CLUSTERED 
(
	[ServerId] ASC,
	[OperatorId] ASC,
	[MemberId] ASC,
	[TrxId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WalletProvider]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletProvider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
	[URL] [nvarchar](max) NULL,
	[MerchantId] [nvarchar](max) NULL,
	[MerchantPwd] [nvarchar](max) NULL,
	[ApiId] [int] NOT NULL DEFAULT ((0)),
	[UseInternalRate] [bit] NOT NULL DEFAULT ((0)),
	[GameIdFormat] [nvarchar](64) NULL,
	[UseGameName] [bit] NOT NULL DEFAULT ((0)),
	[NotifyTrxId] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Enum.WalletProvider] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WalletTransaction]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletTransaction](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Guid] [nvarchar](128) NULL,
	[Type] [int] NOT NULL,
	[Amount] [decimal](23, 8) NOT NULL,
	[GameTransactionId] [bigint] NOT NULL,
	[WalletProviderId] [int] NOT NULL,
	[WalletProviderTransactionId] [nvarchar](max) NULL,
	[WalletProviderResponse] [nvarchar](max) NULL,
	[IsError] [bit] NOT NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[ElapsedSeconds] [float] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.WalletTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WalletTransaction_35days]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletTransaction_35days](
	[Id] [bigint] NOT NULL,
	[Guid] [nvarchar](128) NULL,
	[Type] [int] NOT NULL,
	[Amount] [decimal](23, 8) NOT NULL,
	[GameTransactionId] [bigint] NOT NULL,
	[WalletProviderId] [int] NOT NULL,
	[WalletProviderTransactionId] [nvarchar](max) NULL,
	[WalletProviderResponse] [nvarchar](max) NULL,
	[IsError] [bit] NOT NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[ElapsedSeconds] [float] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL CONSTRAINT [DF__WalletTra__Creat__605D434C]  DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_dbo.WalletTransaction_35days] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Enum].[CodeTable]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[CodeTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Category] [nvarchar](255) NULL,
	[Enum] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_Enum.CodeTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Jackpot].[Jackpot]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Jackpot].[Jackpot](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JackpotCategoryId] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[ResetValue] [decimal](23, 8) NOT NULL,
	[Minimum] [decimal](23, 8) NOT NULL,
	[Maximum] [decimal](23, 8) NOT NULL,
	[ContributionPercentage] [decimal](23, 8) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_Jackpot.Jackpot] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Jackpot].[JackpotCategory]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Jackpot].[JackpotCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_Jackpot.JackpotCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Jackpot].[JackpotSetting]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Jackpot].[JackpotSetting](
	[JackpotSettingGroupId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[JackpotCategoryId] [int] NOT NULL,
 CONSTRAINT [PK_Jackpot.JackpotSetting] PRIMARY KEY CLUSTERED 
(
	[JackpotSettingGroupId] ASC,
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Jackpot].[JackpotSettingGroup]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Jackpot].[JackpotSettingGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_Jackpot.JackpotSettingGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Jackpot].[RealJackpot]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Jackpot].[RealJackpot](
	[JackpotId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[Value] [decimal](23, 8) NOT NULL,
 CONSTRAINT [PK_Jackpot.RealJackpot] PRIMARY KEY CLUSTERED 
(
	[JackpotId] ASC,
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Jackpot].[TestJackpot]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Jackpot].[TestJackpot](
	[JackpotId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[Value] [decimal](23, 8) NOT NULL,
 CONSTRAINT [PK_Jackpot.TestJackpot] PRIMARY KEY CLUSTERED 
(
	[JackpotId] ASC,
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Profile].[SpinBet]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Profile].[SpinBet](
	[GameTransactionId] [bigint] NOT NULL,
	[LineBet] [decimal](23, 8) NOT NULL,
	[Lines] [int] NOT NULL,
	[Multiplier] [int] NOT NULL,
	[TotalBet] [decimal](23, 8) NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[IsAutoSpin] [bit] NOT NULL DEFAULT ((0)),
	[IsSideBet] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Profile.SpinBet] PRIMARY KEY CLUSTERED 
(
	[GameTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+00:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+00:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+00:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+00:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+00:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+00:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+00:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+00:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+01:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+01:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+01:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+01:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+01:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+01:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+01:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+01:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+02:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+02:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+02:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory]
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+02:00'));
GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+02:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+02:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+02:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+02:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+03:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+03:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+03:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+03:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+03:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+03:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+03:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+03:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+04:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+04:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+04:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+04:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+04:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+04:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+04:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+04:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+05:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+05:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+05:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+05:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+05:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+05:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+05:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+05:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+05:45]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+05:45] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+05:45')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+05:45'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+06:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+06:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+06:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+06:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+06:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+06:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+06:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+06:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+07:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+07:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+07:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+07:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+07:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+07:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+07:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+07:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+08:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+08:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+08:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+08:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+08:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+08:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+08:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+08:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+08:45]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+08:45] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+08:45')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+08:45'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+09:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+09:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+09:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+09:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+09:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+09:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+09:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+09:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+10:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+10:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+10:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+10:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+10:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+10:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+10:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+10:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+11:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+11:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+11:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+11:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+11:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+11:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+11:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+11:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+12:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+12:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+12:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+12:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+12:45]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+12:45] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+12:45')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory]
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+12:45'));
GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+13:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+13:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+13:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+13:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+13:45]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+13:45] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+13:45')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+13:45'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc+14:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc+14:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+14:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'+14:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-00:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-00:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-00:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-00:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-01:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-01:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-01:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-01:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-01:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-01:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-01:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-01:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-02:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-02:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-02:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-02:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-02:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-02:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-02:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-02:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-03:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-03:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-03:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-03:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-03:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-03:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-03:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-03:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-04:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-04:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-04:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-04:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-04:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-04:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-04:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-04:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-05:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-05:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-05:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-05:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-05:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-05:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-05:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-05:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-06:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-06:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-06:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-06:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-06:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-06:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-06:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-06:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-07:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-07:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-07:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-07:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-07:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-07:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-07:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-07:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-08:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-08:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-08:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-08:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-08:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-08:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-08:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-08:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-09:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-09:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-09:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-09:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-09:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-09:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-09:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-09:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-10:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-10:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-10:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-10:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-10:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-10:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-10:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-10:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-11:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-11:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-11:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-11:00'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-11:30]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-11:30] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-11:30')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-11:30'));

GO
/****** Object:  View [dbo].[DailyUserGameReportViewUtc-12:00]    Script Date: 7/25/2017 10:28:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


                    CREATE VIEW [dbo].[DailyUserGameReportViewUtc-12:00] WITH SCHEMABINDING AS
    
                    SELECT  [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-12:00')) AS [Date], SUM([Bet]) AS [Bet], SUM([Win]) AS [Win], SUM([Bet]*[ExchangeRate]) AS [BetRMB], SUM([Win]*[ExchangeRate]) AS [WinRMB], COUNT_BIG(*) AS [BigCount]
                    FROM    [dbo].[GameHistory] with(nolock)
                    GROUP BY    [UserId], [GameId], CONVERT(date, SWITCHOFFSET(CONVERT(datetimeoffset, DateTimeUtc),'-12:00'));

GO
ALTER TABLE [dbo].[PlatformReportInfoTemp] ADD  DEFAULT (getutcdate()) FOR [CreatedOnUtc]
GO
ALTER TABLE [dbo].[PlatformReportInfoTemp] ADD  DEFAULT ((0)) FOR [SpinBetAmount]
GO
ALTER TABLE [dbo].[PlatformReportInfoTemp] ADD  DEFAULT ((0)) FOR [GambleBetAmount]
GO
ALTER TABLE [dbo].[PlatformReportInfoTemp] ADD  DEFAULT ((0)) FOR [SpinBetAmountRMB]
GO
ALTER TABLE [dbo].[PlatformReportInfoTemp] ADD  DEFAULT ((0)) FOR [GambleBetAmountRMB]
GO
ALTER TABLE [dbo].[ReportInfoTemp] ADD  DEFAULT (getutcdate()) FOR [CreatedOnUtc]
GO
ALTER TABLE [dbo].[ReportInfoTemp] ADD  DEFAULT ((0)) FOR [OffsetId]
GO
ALTER TABLE [Jackpot].[Jackpot] ADD  DEFAULT (getutcdate()) FOR [CreatedOnUtc]
GO
ALTER TABLE [Jackpot].[JackpotCategory] ADD  DEFAULT (getutcdate()) FOR [CreatedOnUtc]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Account_dbo.Operator_OperatorId] FOREIGN KEY([OperatorId])
REFERENCES [dbo].[Operator] ([Id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_dbo.Account_dbo.Operator_OperatorId]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Account_dbo.Role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_dbo.Account_dbo.Role_RoleId]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Account_dbo.UtcTimeOffset_DefaultOffSet] FOREIGN KEY([DefaultOffSet])
REFERENCES [dbo].[UtcTimeOffset] ([Id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_dbo.Account_dbo.UtcTimeOffset_DefaultOffSet]
GO
ALTER TABLE [dbo].[Bonus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Bonus_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Bonus] CHECK CONSTRAINT [FK_dbo.Bonus_dbo.User_UserId]
GO
ALTER TABLE [dbo].[BonusIncomplete]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BonusIncomplete_dbo.Game_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([Id])
GO
ALTER TABLE [dbo].[BonusIncomplete] CHECK CONSTRAINT [FK_dbo.BonusIncomplete_dbo.Game_GameId]
GO
ALTER TABLE [dbo].[BonusIncomplete]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BonusIncomplete_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BonusIncomplete] CHECK CONSTRAINT [FK_dbo.BonusIncomplete_dbo.User_UserId]
GO
ALTER TABLE [dbo].[FreeRoundData]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FreeRoundData_dbo.FreeRound_CampaignId] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[FreeRound] ([Id])
GO
ALTER TABLE [dbo].[FreeRoundData] CHECK CONSTRAINT [FK_dbo.FreeRoundData_dbo.FreeRound_CampaignId]
GO
ALTER TABLE [dbo].[FreeRoundData]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FreeRoundData_dbo.Game_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([Id])
GO
ALTER TABLE [dbo].[FreeRoundData] CHECK CONSTRAINT [FK_dbo.FreeRoundData_dbo.Game_GameId]
GO
ALTER TABLE [dbo].[FreeRoundData]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FreeRoundData_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[FreeRoundData] CHECK CONSTRAINT [FK_dbo.FreeRoundData_dbo.User_UserId]
GO
ALTER TABLE [dbo].[FreeRoundData]  WITH CHECK ADD  CONSTRAINT [FK_dbo_FreeRoundData_dbo_FreeRound_CampaignId] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[FreeRound] ([Id])
GO
ALTER TABLE [dbo].[FreeRoundData] CHECK CONSTRAINT [FK_dbo_FreeRoundData_dbo_FreeRound_CampaignId]
GO
ALTER TABLE [dbo].[FreeRoundData]  WITH CHECK ADD  CONSTRAINT [FK_dbo_FreeRoundData_dbo_Game_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([Id])
GO
ALTER TABLE [dbo].[FreeRoundData] CHECK CONSTRAINT [FK_dbo_FreeRoundData_dbo_Game_GameId]
GO
ALTER TABLE [dbo].[FreeRoundData]  WITH CHECK ADD  CONSTRAINT [FK_dbo_FreeRoundData_dbo_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[FreeRoundData] CHECK CONSTRAINT [FK_dbo_FreeRoundData_dbo_User_UserId]
GO
ALTER TABLE [dbo].[GameHistory]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GameHistory_dbo.Game_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([Id])
GO
ALTER TABLE [dbo].[GameHistory] CHECK CONSTRAINT [FK_dbo.GameHistory_dbo.Game_GameId]
GO
ALTER TABLE [dbo].[GameHistory]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GameHistory_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[GameHistory] CHECK CONSTRAINT [FK_dbo.GameHistory_dbo.User_UserId]
GO
ALTER TABLE [dbo].[GameSetting]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GameSetting_dbo.Currency_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[GameSetting] CHECK CONSTRAINT [FK_dbo.GameSetting_dbo.Currency_CurrencyId]
GO
ALTER TABLE [dbo].[GameSetting]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GameSetting_dbo.Game_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([Id])
GO
ALTER TABLE [dbo].[GameSetting] CHECK CONSTRAINT [FK_dbo.GameSetting_dbo.Game_GameId]
GO
ALTER TABLE [dbo].[GameSetting]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GameSetting_dbo.GameSettingGroup_GameSettingGroupId] FOREIGN KEY([GameSettingGroupId])
REFERENCES [dbo].[GameSettingGroup] ([Id])
GO
ALTER TABLE [dbo].[GameSetting] CHECK CONSTRAINT [FK_dbo.GameSetting_dbo.GameSettingGroup_GameSettingGroupId]
GO
ALTER TABLE [dbo].[GameState]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GameState_dbo.Game_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([Id])
GO
ALTER TABLE [dbo].[GameState] CHECK CONSTRAINT [FK_dbo.GameState_dbo.Game_GameId]
GO
ALTER TABLE [dbo].[Operator]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Operator_dbo.Currency_FunPlayCurrencyId] FOREIGN KEY([FunPlayCurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[Operator] CHECK CONSTRAINT [FK_dbo.Operator_dbo.Currency_FunPlayCurrencyId]
GO
ALTER TABLE [dbo].[Operator]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Operator_dbo.GameSettingGroup_GameSettingGroupId] FOREIGN KEY([GameSettingGroupId])
REFERENCES [dbo].[GameSettingGroup] ([Id])
GO
ALTER TABLE [dbo].[Operator] CHECK CONSTRAINT [FK_dbo.Operator_dbo.GameSettingGroup_GameSettingGroupId]
GO
ALTER TABLE [dbo].[Operator]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Operator_Enum.WalletProvider_WalletProviderId] FOREIGN KEY([WalletProviderId])
REFERENCES [dbo].[WalletProvider] ([Id])
GO
ALTER TABLE [dbo].[Operator] CHECK CONSTRAINT [FK_dbo.Operator_Enum.WalletProvider_WalletProviderId]
GO
ALTER TABLE [dbo].[Operator]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Operator_Jackpot.JackpotSettingGroup_JackpotSettingGroupId] FOREIGN KEY([JackpotSettingGroupId])
REFERENCES [Jackpot].[JackpotSettingGroup] ([Id])
GO
ALTER TABLE [dbo].[Operator] CHECK CONSTRAINT [FK_dbo.Operator_Jackpot.JackpotSettingGroup_JackpotSettingGroupId]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_dbo.User_dbo.Currency_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_dbo.User_dbo.Currency_CurrencyId]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_dbo.User_dbo.Operator_OperatorId] FOREIGN KEY([OperatorId])
REFERENCES [dbo].[Operator] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_dbo.User_dbo.Operator_OperatorId]
GO
ALTER TABLE [dbo].[UserGameData]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserTimeStamp_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserGameData] CHECK CONSTRAINT [FK_dbo.UserTimeStamp_dbo.User_UserId]
GO
ALTER TABLE [dbo].[UserGameRtpSetting]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserGameRtpSetting_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserGameRtpSetting] CHECK CONSTRAINT [FK_dbo.UserGameRtpSetting_dbo.User_UserId]
GO
ALTER TABLE [dbo].[UserSession]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserSession_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserSession] CHECK CONSTRAINT [FK_dbo.UserSession_dbo.User_UserId]
GO
ALTER TABLE [dbo].[UserSessionLog]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserSessionLog_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserSessionLog] CHECK CONSTRAINT [FK_dbo.UserSessionLog_dbo.User_UserId]
GO
ALTER TABLE [dbo].[WalletTransaction]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WalletTransaction_Enum.WalletProvider_WalletProviderId] FOREIGN KEY([WalletProviderId])
REFERENCES [dbo].[WalletProvider] ([Id])
GO
ALTER TABLE [dbo].[WalletTransaction] CHECK CONSTRAINT [FK_dbo.WalletTransaction_Enum.WalletProvider_WalletProviderId]
GO
ALTER TABLE [Jackpot].[Jackpot]  WITH CHECK ADD  CONSTRAINT [FK_Jackpot.Jackpot_Jackpot.JackpotCategory_JackpotCategoryId] FOREIGN KEY([JackpotCategoryId])
REFERENCES [Jackpot].[JackpotCategory] ([Id])
GO
ALTER TABLE [Jackpot].[Jackpot] CHECK CONSTRAINT [FK_Jackpot.Jackpot_Jackpot.JackpotCategory_JackpotCategoryId]
GO
ALTER TABLE [Jackpot].[JackpotSetting]  WITH CHECK ADD  CONSTRAINT [FK_Jackpot.JackpotSetting_dbo.Game_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([Id])
GO
ALTER TABLE [Jackpot].[JackpotSetting] CHECK CONSTRAINT [FK_Jackpot.JackpotSetting_dbo.Game_GameId]
GO
ALTER TABLE [Jackpot].[JackpotSetting]  WITH CHECK ADD  CONSTRAINT [FK_Jackpot.JackpotSetting_Jackpot.JackpotCategory_JackpotCategoryId] FOREIGN KEY([JackpotCategoryId])
REFERENCES [Jackpot].[JackpotCategory] ([Id])
GO
ALTER TABLE [Jackpot].[JackpotSetting] CHECK CONSTRAINT [FK_Jackpot.JackpotSetting_Jackpot.JackpotCategory_JackpotCategoryId]
GO
ALTER TABLE [Jackpot].[JackpotSetting]  WITH CHECK ADD  CONSTRAINT [FK_Jackpot.JackpotSetting_Jackpot.JackpotSettingGroup_JackpotSettingGroupId] FOREIGN KEY([JackpotSettingGroupId])
REFERENCES [Jackpot].[JackpotSettingGroup] ([Id])
GO
ALTER TABLE [Jackpot].[JackpotSetting] CHECK CONSTRAINT [FK_Jackpot.JackpotSetting_Jackpot.JackpotSettingGroup_JackpotSettingGroupId]
GO
ALTER TABLE [Jackpot].[RealJackpot]  WITH CHECK ADD  CONSTRAINT [FK_Jackpot.RealJackpot_dbo.Currency_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [Jackpot].[RealJackpot] CHECK CONSTRAINT [FK_Jackpot.RealJackpot_dbo.Currency_CurrencyId]
GO
ALTER TABLE [Jackpot].[RealJackpot]  WITH CHECK ADD  CONSTRAINT [FK_Jackpot.RealJackpot_Jackpot.Jackpot_JackpotId] FOREIGN KEY([JackpotId])
REFERENCES [Jackpot].[Jackpot] ([Id])
GO
ALTER TABLE [Jackpot].[RealJackpot] CHECK CONSTRAINT [FK_Jackpot.RealJackpot_Jackpot.Jackpot_JackpotId]
GO
ALTER TABLE [Jackpot].[TestJackpot]  WITH CHECK ADD  CONSTRAINT [FK_Jackpot.TestJackpot_dbo.Currency_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [Jackpot].[TestJackpot] CHECK CONSTRAINT [FK_Jackpot.TestJackpot_dbo.Currency_CurrencyId]
GO
ALTER TABLE [Jackpot].[TestJackpot]  WITH CHECK ADD  CONSTRAINT [FK_Jackpot.TestJackpot_Jackpot.Jackpot_JackpotId] FOREIGN KEY([JackpotId])
REFERENCES [Jackpot].[Jackpot] ([Id])
GO
ALTER TABLE [Jackpot].[TestJackpot] CHECK CONSTRAINT [FK_Jackpot.TestJackpot_Jackpot.Jackpot_JackpotId]
GO

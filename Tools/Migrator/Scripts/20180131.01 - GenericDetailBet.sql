/****** Object:  StoredProcedure [dbo].[GETTRANSACTIONHISTORY]    Script Date: 17/01/2018 12:56:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GETGENERICBETSDETAIL]	
	            @OperatorTag	NVARCHAR(16),
				@ExternalId	    NVARCHAR(255) = NULL,
				@GameTransactionId	    BIGINT = NULL,
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
							    WHEN 1 THEN 'Spin' 
							    WHEN 2 THEN 'Gamble' 
							    WHEN 3 THEN 'Free Spin' 
							    WHEN 4 THEN 'Bonus' 
							    WHEN 5 THEN 'Reveal' 
							    WHEN 6 THEN 'Double Up' 
							    WHEN 7 THEN 'Instant Win' 
								WHEN 8 THEN 'Multi Mode'
								WHEN 9 THEN 'Collapsing Spin'
								WHEN 10 THEN 'Free Spin Collapsing Spin'
						    END AS [ChangeType],
			                GameName = gi.Name, 
							GameType = gi.GameType,
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
					    FROM	[dbo].GameHistory gh WITH(NOLOCK)
						LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
		                INNER JOIN	[dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
		                INNER JOIN	[dbo].[Game] gi WITH(NOLOCK)ON gi.id = gh.GameId
		                WHERE u.OperatorId = @OperatorID
							AND (u.ExternalId = @ExternalId OR @ExternalId is NULL)
							AND (gh.GameTransactionId = @GameTransactionId OR @GameTransactionId is NULL)
			                AND gh.IsReport = 1
                            AND gh.DateTimeUtc BETWEEN @StartDateInUTC AND @EndDateInUTC
			                AND gh.IsDeleted = 0
	                ) AS RESULT
	                ORDER BY OperationCode
	                OFFSET @OffsetRows ROWS
	                FETCH NEXT @PageSize ROWS ONLY
                END

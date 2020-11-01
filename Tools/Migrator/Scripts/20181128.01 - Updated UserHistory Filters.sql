SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
09-08-2017: Added IsDeleted filter on GameHistory
*/

ALTER PROCEDURE [dbo].[USERHISTORY]
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
				AND gh.IsDeleted = 0
				AND (@IsDemo IS NULL OR u.IsDemo = @IsDemo)
				AND (@OperatorId IS NULL  OR u.OperatorId = @OperatorId)
				AND (@GameId IS NULL OR gh.GameId = @GameId)
				AND (@UserId IS NULL OR gh.UserId = @UserId)
				AND (@TrxId IS NULl OR gh.GameTransactionId = @TrxId)
				AND (@GameTrxType IS NULL OR (gh.GameResultType IN (1, 9) AND @GameTrxType = 1) OR (((gh.GameResultType >= 2 AND gh.GameResultType <= 8) OR gh.GameResultType = 10) AND @GameTrxType > 1))
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

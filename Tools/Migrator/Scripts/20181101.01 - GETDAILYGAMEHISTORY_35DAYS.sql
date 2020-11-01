SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GETDAILYGAMEHISTORY_35DAYS]
	-- Add the parameters for the stored procedure here
	@BatchSize INT = 3000,
	@LastId BIGINT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @now DATETIME2 = GETUTCDATE()
	DECLARE @createdTime DATETIME2 = DATEADD(mi, -1, @now)
	DECLARE @lBatchSize INT = @BatchSize, 
			@lLastId BIGINT = @LastId	
	SET ROWCOUNT @lBatchSize
		
	SELECT TOP (@lBatchSize) *
	INTO #GH_35days
	FROM GameHistory_35days gh_35 with(nolock)
	WHERE 
			gh_35.Id > @lLastId
		AND	DateTimeUtc < @createdTime
		AND gh_35.IsDeleted = 0
	ORDER BY gh_35.Id
	
	CREATE index cinndex on #GH_35days(Id);

	SELECT 
		Id = gh.Id, 
		DateTimeUtc, 
		OperatorId= u.OperatorId, 
		CurrencyId, 
		UserId, 
		GameId, 
		[Level],
		Bet,
		Win, 
		ExchangeRate, 
		GameResultType, 
		IsHistory, 
		IsReport, 
		PlatformType,
		GameTransactionId=gh.GameTransactionId,
		SpinTransactionId=gh.SpinTransactionId,
		LineBet = CASE WHEN sbp.LineBet IS NULL THEN
								CASE WHEN gh.GameResultType IN (1,9) THEN 
									CAST(gh.ResponseXml as xml).value('(spin/@bet)[1]', 'decimal(23,8)')
								ELSE 
									CASE WHEN CAST(gh.ResponseXml as xml).value('(bonus/data/spin/@bet)[1]', 'decimal(23,8)') IS NULL THEN
										ISNULL((SELECT CAST(ResponseXml as xml).value('(spin/@bet)[1]', 'decimal(23,8)') AS LineBet FROM #GH_35days WITH(NOLOCK) WHERE GameTransactionId = gh.SpinTransactionId), 0)
									ELSE 
										CAST(gh.ResponseXml as xml).value('(bonus/data/spin/@bet)[1]', 'decimal(23,8)')
									END 
								END 
							ELSE 
								sbp.LineBet
							END,
		Multiplier = CASE WHEN sbp.Multiplier IS NULL THEN
						CASE WHEN gh.GameResultType IN (1,9) THEN 
							CAST(gh.ResponseXml as xml).value('(spin/@multiplier)[1]', 'int')
						ELSE 
							CASE WHEN CAST(gh.ResponseXml as xml).value('(bonus/data/spin/@multiplier)[1]', 'int') IS NULL THEN
								ISNULL((SELECT CAST(ResponseXml as xml).value('(spin/@multiplier)[1]', 'int') AS LineBet FROM #GH_35days WITH(NOLOCK) WHERE GameTransactionId = gh.SpinTransactionId), 0)
							ELSE 
								CAST(gh.ResponseXml as xml).value('(bonus/data/spin/@multiplier)[1]', 'int')
							END 
						END 
					ELSE 
						sbp.Multiplier
					END,
		IsFreeGame=gh.IsFreeGame,
		FreeRoundId=ISNULL(fgh.FreeRoundId,0),
		IsSideBet=ISNULL(sbp.IsSideBet, 0)
		FROM #GH_35days gh WITH(NOLOCK)
		INNER JOIN [USER] u WITH(NOLOCK) ON u.Id=gh.UserId
		LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)		
		LEFT JOIN [FREEROUNDGAMEHISTORY] fgh WITH(NOLOCK) ON fgh.GameHistoryId = gh.Id

	if object_id('tempdb..#GH_35days') is not null 
	drop table #GH_35days
END
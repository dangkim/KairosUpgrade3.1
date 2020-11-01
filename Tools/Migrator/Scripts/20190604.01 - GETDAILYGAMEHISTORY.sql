-- =============================================
-- Author:		Kaidan
-- Create date: October 31, 2017
-- Description:	Get daily the game history with the batch size as input condition
-- Modify : 
   /*
	1. changed the logic to get LineBet, Multiplier, 
	2. Remove Operator Inner join
	3. Increase the delay time to 2m
   */
-- =============================================
ALTER PROCEDURE [dbo].[GETDAILYGAMEHISTORY]
	-- Add the parameters for the stored procedure here
	@BatchSize INT = 5000,
	@LastId BIGINT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	DECLARE @now datetime = GETUTCDATE()
	SET NOCOUNT ON;
	SET ROWCOUNT @BatchSize
	
	SELECT 
		Id = gh.Id, 
		DateTimeUtc, 
		OperatorId= u.OperatorId, 
		CurrencyId, 
		UserId, 
		GameId, 
		Level,
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
										ISNULL((SELECT CAST(ResponseXml as xml).value('(spin/@bet)[1]', 'decimal(23,8)') AS LineBet FROM GAMEHISTORY WITH(NOLOCK) WHERE GameTransactionId = gh.SpinTransactionId), 0)
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
								ISNULL((SELECT CAST(ResponseXml as xml).value('(spin/@multiplier)[1]', 'int') AS LineBet FROM GAMEHISTORY WITH(NOLOCK) WHERE GameTransactionId = gh.SpinTransactionId), 0)
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
		FROM GAMEHISTORY gh WITH(NOLOCK)
		INNER JOIN [USER] u WITH(NOLOCK) ON u.Id=gh.UserId
		LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)		
		LEFT JOIN [FREEROUNDGAMEHISTORY] fgh WITH(NOLOCK) ON fgh.GameHistoryId = gh.Id
		WHERE 
			DateTimeUtc < DATEADD(mi, -2, @now)		
			AND gh.Id > @LastId
			AND gh.IsDeleted = 0
		ORDER BY Id	
END


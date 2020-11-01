/****** Object:  StoredProcedure [dbo].[GETDAILYGAMEHISTORY]    Script Date: 1/3/2018 6:24:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kaidan
-- Create date: October 31, 2017
-- Description:	Get daily the game history with the batch size as input condition
-- Modify : 
   /*
	1. changed the logic to get LineBet, Multiplier
   */
-- =============================================
CREATE PROCEDURE [dbo].[GETDAILYGAMEHISTORY]
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
		OperatorId=o.Id, 
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
		LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
		INNER JOIN [USER] u WITH(NOLOCK) ON u.Id=gh.UserId
		INNER JOIN [OPERATOR] o WITH(NOLOCK) ON o.Id=u.OperatorId
		LEFT JOIN [FREEROUNDGAMEHISTORY] fgh WITH(NOLOCK) ON fgh.GameHistoryId = gh.Id
		WHERE 
			gh.IsDeleted = 0 
			AND gh.Id > @LastId
			AND DateTimeUtc < DATEADD(mi, -1, @now)
		ORDER BY Id
END

-- =============================================
-- Author:		John
-- Description:	Updated Top Winner to include CurrencyId parameter.
-- =============================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	ALTER PROCEDURE [dbo].[REPORTTOPWINNER]	
		@OperatorId		INT				= NULL,
		@GameId			INT				= NULL,
		@StartDateInUTC	DATETIME,
		@EndDateInUTC	DATETIME,
		@Username		NVARCHAR(255)	= NULL,
		@Top			INT,
		@CurrencyId 	INT	= NULL
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
					CompanyWLPercentage = IIF(SUM(TotalBetAmountRMB) = 0, 1, -(SUM(TotalNetWinAmountRMB)) / IIF(SUM(TotalBetAmountRMB) = 0, 1, SUM(TotalBetAmountRMB))),
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
					AND (u.CurrencyId = @CurrencyId OR @CurrencyId IS NULL)
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
					AllTimeCompanyWLPercentage = -(SUM(TotalNetWinAmountRMB) / IIF(SUM(TotalBetAmountRMB) = 0, 1, SUM(TotalBetAmountRMB)))
				FROM PlatformReportInfo
				GROUP BY UserId
			) apri ON apri.UserId = tem.UserId
			ORDER BY TotalNetWin DESC
		END
GO
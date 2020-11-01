SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kaidan Joseph
-- Create date:	10 May 2017
-- Modify date:	September 01, 2017
-- Description:	Get on reporting tournaments for Tournament Consolidator
-- Path: DatabaseService.Tournament
-- =============================================
ALTER PROCEDURE [dbo].[GETONREPORTINGTOURNAMENTS]
AS
BEGIN

	DECLARE @Now DATETIME =  GETUTCDATE();
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
			WHEN @Now > EndTime THEN 1
				WHEN @Now >= StartTime AND @Now < EndTime THEN 2
				ELSE 3
			END
	)
	FROM TOURNAMENT t WITH (NOLOCK)
	WHERE 
		IsDeleted = 0
		AND (t.IsCancelled = 1 OR DATEDIFF(dd,  GETUTCDATE(), EndTime) > -7)
		ORDER BY StartTime DESC;
END;
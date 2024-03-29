SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vincente
-- Create date:	11 May 2017
-- Description:	Get active tournaments for Tournament Consolidator
-- Path: TournamentReportConsolidator
-- Modified : Appends the IsGlobal field. (Kaidan Joseph),
-- Modified - June 26, 2018, cast endtime into mini seconds
-- =============================================
ALTER PROCEDURE [dbo].[GETACTIVETOURNAMENTSCONSOLIDATOR]
AS
BEGIN
    DECLARE @UtcNow DATETIME;
    DECLARE @StartCheck DATETIME;
    DECLARE @EndCheck DATETIME;
    SET @UtcNow = GETUTCDATE();
    SET @StartCheck = DATEADD(hh, -1, @UtcNow);
    SET @EndCheck = DATEADD(hh, 1, @UtcNow);

    SELECT 
		[Id]
		,[StartTime]
		,[EndTime] = CASE  -- Tournamnet's endtime not showing in millisecond incase the time of  second is ##:##:59.000
						WHEN FORMAT([EndTime], 'ss.fff') ='59.000' THEN  DATEADD (ss,1,[EndTime])
						ELSE  [EndTime]
					 END
		,[OperatorId]
		,IsAllMembers = CAST(CASE CONVERT(INT, (t.Flags & 1)) WHEN 0 THEN 0 ELSE 1 END AS BIT)
		,[MinHands]	
		,IsGlobal = CAST(ISNULL((SELECT 1 FROM TRelation tr WITH(NOLOCK) WHERE tr.TournamentId = t.Id AND tr.[RelationType] = 6 GROUP BY tr.TournamentId), 0) AS BIT)
    FROM TOURNAMENT t WITH (NOLOCK)
	WHERE IsDeleted <> 1
        AND IsCancelled <> 1
        AND StartTime < @EndCheck
        AND EndTime > @StartCheck
    ORDER BY ABS(DATEDIFF(millisecond, StartTime, @UtcNow));
END;
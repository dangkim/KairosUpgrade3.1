SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ISGLOBALTOURNAMENTGAMEOVERLAP](
										@Id  INT,
                                        @StartTime  DATETIME,
                                        @EndTime    DATETIME,
                                        @Merchants NVARCHAR(MAX), 
										@Games NVARCHAR(32))
AS
DECLARE @cnt INT;
BEGIN
  	;WITH OverLap AS (
		SELECT ti.Id
		FROM TOURNAMENT ti WITH (NOLOCK)
		WHERE ti.IsDeleted <> 1
			AND ti.IsCancelled <> 1
			AND GETUTCDATE() <= ti.EndTime 
			AND StartTime < @EndTime
			AND @StartTime < EndTime
			AND Id <> @Id
			AND ti.OperatorId = 0
	)
	
	SELECT @cnt = COUNT(1) FROM 
	(SELECT 
		tr.TournamentId , tr.RelationType, tr.RelationId
	FROM TRelation tr WITH(NOLOCK) 
	WHERE
		EXISTS(SELECT TOP 1 1 FROM  OverLap ol WHERE ol.Id  = tr.TournamentId)
		AND 1 = 
			CASE WHEN  
				(tr.RelationType = 6 
				AND tr.RelationId IN (SELECT REPLACE(REPLACE(REPLACE(fn.splitdata, CHAR(9), N''),CHAR(13), N''), CHAR(10),N'') FROM fnSplitString(@Merchants, ',') fn)) 
				OR
				(tr.RelationType = 2 
				AND tr.RelationId IN (SELECT REPLACE(REPLACE(REPLACE(fn.splitdata, CHAR(9), N''),CHAR(13), N''), CHAR(10),N'') FROM fnSplitString(@Games, ',') fn)) 
				THEN 1
			ELSE 0 END)
	p PIVOT (
		COUNT(RelationId) for RelationType in ([2], [6])
	) as pvt 
	WHERE [2] > 0 and  [6] > 0
	RETURN @cnt;
END;
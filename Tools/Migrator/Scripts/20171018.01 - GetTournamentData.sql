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
ALTER PROCEDURE [dbo].[GETTOURNAMENTDATA] @StartDateInUtc DATETIME,
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
              AND ((@StartDateInUTC IS NULL OR @EndDateInUTC IS NULL) OR ( @StartDateInUTC < t.StartTime AND t.StartTime < @EndDateInUTC))
              AND (@OperatorId IS NULL OR t.OperatorId = @OperatorId)
		    AND (@TournamentName IS NULL OR t.Name LIKE '%'+@TournamentName+'%')
	GROUP BY t.Id, t.Name, t.[Description], t.StartTime, t.EndTime, o.Name, a.Username, t.IsCancelled
     END;
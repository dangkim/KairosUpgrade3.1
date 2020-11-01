SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GETEXISTINGGLOBALTOURNAMENTPARTICIPANT](@Id  INT,
                                        @StartTime  DATETIME,
                                        @EndTime    DATETIME,
                                        @Merchants NVARCHAR(MAX))
AS
DECLARE @IncludedParticipant NVARCHAR(MAX);
BEGIN
    DECLARE @Participants TABLE(Id INT IDENTITY(1,1), TournamentId INT, OperatorId INT , Name NVARCHAR(256));
	DECLARE @IsOverlap BIT = 0;

	INSERT INTO @Participants
	SELECT 
		tr.TournamentId, o.Id, o.Name 		
	FROM TRelation tr WITH(NOLOCK) 
	INNER JOIN Operator o WITH(NOLOCK) ON o.Id = tr.RelationId
	WHERE
		tr.RelationType = 6
		AND tr.TournamentId = @Id
		AND o.Id IN (SELECT REPLACE(REPLACE(REPLACE(fn.splitdata, CHAR(9), N''),CHAR(13), N''), CHAR(10),N'') FROM fnSplitString(@Merchants, ',') fn)

	SELECT @IsOverlap = 1  
	FROM TOURNAMENT ti WITH (NOLOCK)
	WHERE ti.IsDeleted <> 1
		AND ti.IsCancelled <> 1
		AND StartTime < @EndTime
		AND @StartTime < EndTime
		AND Id <> @Id
		AND NOT EXISTS(SELECT TOP 1 1 FROM  @Participants pt WHERE pt.TournamentId = @Id)

	IF @IsOverlap = 1 
	BEGIN 
		;WITH  tmp AS (
			SELECT 
				Participants = (SELECT pt.Name + ', ' AS [text()] FROM @Participants pt 
			ORDER BY pt.Name
			FOR XML PATH('')))
		SELECT @IncludedParticipant = LEFT(tmp.Participants, LEN(tmp.Participants) - 1) FROM tmp
	END 
	ELSE 
	BEGIN 
		SET @IncludedParticipant = ''
	END 
    RETURN @IncludedParticipant;
END;
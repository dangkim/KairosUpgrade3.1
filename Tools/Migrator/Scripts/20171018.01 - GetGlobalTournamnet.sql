SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kaidan Joseph	
-- Create date: August 22, 2017
-- Description:	Gets all the global tonament
-- =============================================
CREATE PROCEDURE [dbo].[GETGLOBALTOURNAMENT]
	@StartDateInUtc DATETIME = NULL,
    @EndDateInUtc   DATETIME = NULL,
    @Operator		NVARCHAR(1024) = NULL,
    @Name			NVARCHAR(128) = NULL,
    @Platform       NVARCHAR(128) = NULL,
	@OffsetRows		INT = 0,
    @PageSize		INT = 500, 
	@OrderBy		INT = NULL,
	@Dir			NVARCHAR(8) = 'desc'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @TournamentPlatform table (Id INT IDENTITY(1,1), TournamentId INT NOT NULL, [Platforms] NVARCHAR(64),UNIQUE NONCLUSTERED (TournamentId));
	DECLARE @TournamentOperator table (Id INT IDENTITY(1,1), TournamentId INT NOT NULL, Operators NVARCHAR(MAX),UNIQUE NONCLUSTERED (TournamentId));
	DECLARE @PlatformFilter  TABLE (Id INT IDENTITY (1,1), [Platform] TINYINT NOT NULL);
	DECLARE @OperatorFilter  TABLE (Id INT IDENTITY (1,1), [Operator] INT NOT NULL);

	-- populate platform filter
	INSERT INTO @PlatformFilter
	SELECT 
		LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(fn.splitdata, CHAR(9), N''),CHAR(13), N''), CHAR(10),N'')))
	FROM fnSplitString(@Platform, ',') fn

	-- populate operator filter
	INSERT INTO @OperatorFilter
	SELECT 
		LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(fn.splitdata, CHAR(9), N''),CHAR(13), N''), CHAR(10),N'')))
	FROM fnSplitString(@Operator, ',') fn

	-- get all platforms by individual tournament
	INSERT INTO @TournamentPlatform
	SELECT 
		a.TournamentId
		,[Platform] = STUFF((SELECT ',' + CASE tr.RelationId 
												WHEN	1 THEN 'Web'
												WHEN	2 THEN 'Desktop'
												ELSE	'Moblie'
											END
			   FROM TRelation tr
			   WHERE 
				tr.TournamentId = a.TournamentId 
				AND tr.RelationType = 4	
				
			  FOR XML PATH('')), 1,1,'')
	FROM TRelation a
	WHERE
		a.RelationType = 4
		AND EXISTS(SELECT 1 FROM @PlatformFilter pf WHERE pf. [Platform] =  a.RelationId)	
	GROUP BY a. TournamentId


	-- get all operators by individual tournament
	INSERT INTO @TournamentOperator
	SELECT 
		a.TournamentId
		,Operator = STUFF((SELECT ',' + o.Name
			   FROM TRelation tr
			   INNER JOIN Operator o WITH(NOLOCK) ON o.Id = tr.RelationId
			   WHERE 
				tr.TournamentId = a.TournamentId 
				AND tr.RelationType = 6
				
			  FOR XML PATH('')), 1,1,'')
	FROM TRelation a
	WHERE 
		a.RelationType = 6
		AND EXISTS(SELECT 1 FROM @OperatorFilter pf WHERE pf.[Operator] =  a.RelationId)	
	GROUP BY a. TournamentId
	----------------------------------------------------------------------------------------------
	/*
	Gets all global tournament from CTE table
	*/

	;WITH GlobalTournament AS (

		SELECT * FROM (
			SELECT 
				
				Operators = ISNULL(ot.Operators, '') 
				,t.Id
				,Name = SUBSTRING(Name, 1, CHARINDEX('|', Name) - 1)   
				,[Description]= SUBSTRING([Description], 1, CHARINDEX('|', [Description]) - 1) 
				,t.StartTime
				,t.EndTime
				,[Owner] = a.UserName
				,[Status] = (CASE
								WHEN t.IsCancelled = 1 THEN 'Cancelled'
								WHEN GETUTCDATE() > t.EndTime THEN 'Completed'
								WHEN GETUTCDATE() >= t.StartTime AND GETUTCDATE() < t.EndTime THEN 'Ongoing'
								ELSE 'Upcoming'
							END)
				,Platforms = tpf.[Platforms]
				,Total = COUNT(*) OVER()
			FROM tournament t WITH(NOLOCK)
			INNER JOIN dbo.Account a WITH (NOLOCK) ON t.OwnerId = a.Id	 
			INNER JOIN @TournamentPlatform tpf ON tpf.TournamentId = t.Id
			INNER JOIN @TournamentOperator  ot ON ot.TournamentId = t.Id
			WHERE 
				((@StartDateInUTC IS NULL OR @EndDateInUTC IS NULL) OR ( @StartDateInUTC < t.StartTime AND t.StartTime < @EndDateInUTC)) 
				AND (@Name IS NULL OR SUBSTRING(Name, 1, CHARINDEX('|', Name) - 1) LIKE '%'+@Name+'%')) t
			
			ORDER BY 
				CASE WHEN @OrderBy = 1 AND @Dir = 'asc'		THEN t.Operators END ASC,
				CASE WHEN @OrderBy = 1 AND @Dir = 'desc'	THEN t.Operators END DESC,
				CASE WHEN @OrderBy = 2 AND @Dir = 'asc'		THEN t.Id END ASC,
				CASE WHEN @OrderBy = 2 AND @Dir = 'desc'	THEN t.Id END DESC,
				CASE WHEN @OrderBy = 3 AND @Dir = 'asc'		THEN t.Name END ASC,
				CASE WHEN @OrderBy = 3 AND @Dir = 'desc'	THEN t.Name END DESC,
				CASE WHEN @OrderBy = 4 AND @Dir = 'asc'		THEN t.StartTime END ASC,
				CASE WHEN @OrderBy = 4 AND @Dir = 'desc'	THEN t.StartTime END DESC,
				CASE WHEN @OrderBy = 5 AND @Dir = 'asc'		THEN t.EndTime END ASC,
				CASE WHEN @OrderBy = 5 AND @Dir = 'desc'	THEN t.EndTime END DESC,
				CASE WHEN @OrderBy = 6 AND @Dir = 'asc'		THEN t.[Status] END ASC,
				CASE WHEN @OrderBy = 6 AND @Dir = 'desc'	THEN t.[Status] END DESC,
				CASE WHEN @OrderBy = 7 AND @Dir = 'asc'		THEN t.[Owner] END ASC,
				CASE WHEN @OrderBy = 7 AND @Dir = 'desc'	THEN t.[Owner] END DESC,
				CASE WHEN @OrderBy = 8 AND @Dir = 'asc'		THEN t.Platforms END ASC,
				CASE WHEN @OrderBy = 8 AND @Dir = 'desc'	THEN t.Platforms END DESC,
				CASE WHEN @OrderBy = 9 AND @Dir = 'asc'		THEN t.[Description] END ASC,
				CASE WHEN @OrderBy = 9 AND @Dir = 'desc'	THEN t.[Description] END DESC
			 OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY
		)

	SELECT 
		[No] = ROW_NUMBER() OVER(ORDER BY (SELECT 1))
		,gt.Operators
		,gt.Id
		,gt.Name
		,StartTime = FORMAT(DATEADD(hh, 8, gt.StartTime),'dd-MM-yyyy HH:mm:ss')
		,EndTime = FORMAT( DATEADD(hh, 8, gt.EndTime),'dd-MM-yyyy HH:mm:ss')
		,gt.[Status]
		,[Owner] = gt.[Owner]
		,Platforms = gt.[Platforms]
		,[Description] = gt.[Description]
		,Total
	FROM GlobalTournament gt;
END
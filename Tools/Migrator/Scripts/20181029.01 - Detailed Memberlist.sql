SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John
-- Create date: October 29, 2018
-- Description:	Get Memberlist with details and pagination
-- =============================================
CREATE PROCEDURE [dbo].[GETMEMBERLISTDETAILS]
	@MemberId INT = NULL,
	@MemberName NVARCHAR(255) = NULL,
	@OperatorId INT = NULL,
	@CurrencyId INT = NULL,
	@IsDemoAccount BIT = NULL,
	@PageNumber INT,
	@PageSize INT
AS
BEGIN
	WITH MembersTbl AS (
		SELECT
			us.Id [MemberId], 
			us.Name [MemberName], 
			op.Name [OperatorTag],
			us.OperatorId,
			cu.IsoCode [Currency],
			us.CurrencyId,
			us.IsDemo [IsDemoAccount],
			us.CreatedOnUtc
		FROM [user] us WITH (NOLOCK)
			INNER JOIN Operator op WITH (NOLOCK) ON us.OperatorId = op.Id
			INNER JOIN Currency cu WITH (NOLOCK) ON us.CurrencyId = cu.Id
		WHERE 
			(@MemberId IS NULL OR us.Id = @MemberId)
			AND (@MemberName IS NULL OR us.Name = @MemberName) 
			AND (@OperatorId IS NULL OR us.OperatorId = @OperatorId)
			AND (@CurrencyId IS NULL OR us.CurrencyId = @CurrencyId)
			AND (@IsDemoAccount IS NULL OR us.IsDemo = @IsDemoAccount)
		ORDER BY us.Id
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY
	),
	LastLoginTbl AS (
		SELECT 
			UserId [MemberId], 
			MAX(UserSessionLog.CreatedOnUtc) [LastLoginUtc] 
		FROM UserSessionLog WITH (NOLOCK)
		INNER JOIN MembersTbl WITH (NOLOCK) ON
			UserSessionLog.UserId = MembersTbl.MemberId
		GROUP BY UserSessionLog.UserId
	)
	SELECT 
		MembersTbl.*, 
		LastLoginTbl.LastLoginUtc 
	FROM MembersTbl WITH (NOLOCK)
		LEFT OUTER JOIN LastLoginTbl WITH (NOLOCK) ON
			MembersTbl.MemberId = LastLoginTbl.MemberId
END

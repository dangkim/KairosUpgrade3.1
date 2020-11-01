--Modified by Kaidan:	September 06, 2018 - Add three more game result type 
ALTER PROCEDURE [dbo].[GETTRANSACTIONHISTORY]
  @OperatorTag NVARCHAR(16),
  @StartDateInUTC DATETIME,
  @EndDateInUTC DATETIME,
  @OffsetRows  INT,
  @PageSize  INT
AS
BEGIN

  DECLARE @OperatorID INT
  SET @OperatorID = (SELECT Id
  FROM [dbo].[Operator]
  WHERE Tag = @OperatorTag)
  SELECT * , COUNT( * )OVER ()as TotalRecords
  FROM
    (
SELECT
      OperationCode = gh.GameTransactionId,
      UserId = u.ExternalId,
      CONVERT(DATETIME, SWITCHOFFSET(TODATETIMEOFFSET(gh.DateTimeUtc, '+00:00'), '+08:00'), 0)AS [ChangeTime],
      CASE gh.GameResultType 
        WHEN 1 THEN 'Result'
        WHEN 2 THEN 'Gamble' 
        WHEN 3 THEN 'Free Spin' 
        WHEN 4 THEN 'Bonus' 
        WHEN 5 THEN 'Bonus'
        WHEN 6 THEN 'Gamble' 
        WHEN 7 THEN 'Bonus' 
        WHEN 8 THEN 'Free Spin'
        WHEN 9 THEN 'Collapse'
        WHEN 10 THEN 'Free Spin - Collapse'
        WHEN 11 THEN 'Respin'
        END AS [ChangeType],
      GameName = gi.Name,
      IsFreeRound = gh.IsFreeGame,
      Bet = IIF(gh.IsFreeGame = 1, 0, IIF(sbp.IsSideBet = 1 AND gh.GameResultType = 1, gh.bet * 2, gh.bet)),
      [Return] = gh.Win,
      [Changes] = gh.win - IIF(gh.IsFreeGame = 1, 0, IIF(sbp.IsSideBet = 1 AND gh.GameResultType = 1, gh.bet * 2, gh.bet)),
      EndBalance = 0.0,
      Operator = @OperatorTag,
      TransactionId = '',
      JackpotCon = 0.0,
      JackpotWin = 0.0,
      [Platform] = gh.PlatformType,
      [Version] = 1

    FROM [dbo].GameHistory gh WITH(NOLOCK, index(IX_DateTimeUtc))
      LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId = ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
      INNER JOIN [dbo].[User] u WITH(NOLOCK) ON u.Id = gh.UserId
      INNER JOIN [dbo].[Game] gi WITH(NOLOCK) ON gi.id = gh.GameId
    WHERE u.OperatorId = @OperatorID
      AND gh.IsReport = 1
      AND gh.DateTimeUtc BETWEEN @StartDateInUTC AND @EndDateInUTC
      AND gh.IsDeleted = 0
  )AS RESULT

  ORDER BY OperationCode
  OFFSET @OffsetRows ROWS
  FETCH NEXT @PageSize ROWS ONLY
END
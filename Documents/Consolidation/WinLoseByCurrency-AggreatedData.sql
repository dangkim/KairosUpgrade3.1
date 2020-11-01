declare @sd datetime = 'january 23, 2019'

SELECT
		--o.Tag,
		DataSource = 'Aggreated Data',
    	Currency = c.IsoCode,
    	NoOfPlayer = COUNT(DISTINCT UserId),
    	NoOfTransaction = SUM(CONVERT(BIGINT, pri.TrxCount)),	
		Bet = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmount)),
    	Win = SUM(pri.TotalWinAmount),
    	[Bet (RMB)] = SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)),
    	[Win (RMB)] = SUM(pri.TotalWinAmountRMB),
    	RTP = CONVERT(VARCHAR(20),(CASE SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) WHEN 0 THEN 1 ELSE SUM(TotalWinAmountRMB) / SUM(IIF(pri.IsFreeGame=1,0,pri.TotalBetAmountRMB)) END * 100)) + '%'
   FROM [dbo].[PlatformReportInfo] pri WITH (NOLOCK)
   inner join	[dbo].[user] u with(nolock) on u.id = pri.userid and u.OperatorId =1
   inner join	[dbo].[currency] c with (nolock) on c.id = u.currencyid 
  
    WHERE   
		pri.ChangeTime =@sd
       	AND (pri.IsFreeGame = 0)
    	AND OffsetId = 42		
    GROUP BY c.IsoCode
	order by c.IsoCode
	
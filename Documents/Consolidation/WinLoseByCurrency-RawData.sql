declare @sd datetime = 'january 23, 2019'
	, @ed datetime = 'january 24, 2019';
	
declare @operatorId int = 220; --TCGGAMING03


-- convert to utc
SET @sd = dateadd(hh, -8, @sd)
SET @ed = dateadd(hh, -8, @ed)

-- select @OperatorId = o.Id from Operator o with(nolock) where o.tag = 'TCGGAMING03' or o.Name = 'TCGGAMING03'

select 
	UserId =u.Id, Currency = c.IsoCode
into #userInfo
from [user] u with(nolock) 
inner join currency c with(nolock) on c.Id = u.CurrencyId
where
	u.OperatorId=@OperatorId
group by u.Id,c.IsoCode
create index cinndex on  #userInfo(UserId,Currency);


SELECT	
	gh.DateTimeUtc
	,gh.Id
	,gh.UserId 	
	,u.Currency
	,Bet = 
		(
			CASE
				WHEN sbp.IsSideBet = 1 AND gh.GameResultType = 1 THEN gh.bet * 2
				WHEN gh.IsFreeGame = 1 THEN 0
				WHEN gh.GameResultType IN (9, 10) THEN 0
				ELSE gh.bet
			END
		)
	,BetRMB = gh.ExchangeRate * (
			CASE
				WHEN sbp.IsSideBet = 1 AND gh.GameResultType = 1 THEN gh.bet * 2
				WHEN gh.IsFreeGame = 1 THEN 0
				WHEN gh.GameResultType IN (9, 10) THEN 0
				ELSE gh.bet
			END
		)
	,Win = gh.Win
	,WinRMB = gh.Win * gh.ExchangeRate

into #ghinfo
FROM gamehistory gh WITH(NOLOCK,index(IX_DateTimeUtc))
INNER JOIN #userInfo u  ON (u.UserId=gh.UserId)
LEFT JOIN [PROFILE].SPINBET sbp WITH(NOLOCK) ON sbp.GameTransactionId=ISNULL(gh.SpinTransactionId, gh.GameTransactionId)
WHERE 	
	(@sd <= gh.DateTimeUtc and gh.DateTimeUtc<@ed)
	AND gh.IsDeleted = 0
	AND IsReport = 1		
ORDER BY gh.Id

select 
	DataSource = 'Raw Data'
	,Currency
	,NoOfPlayer = COUNT(DISTINCT gh.UserId)	
	,NoOfTransaction = count(gh.Id)	
	,Bet = Sum(gh.Bet)
	,Win = Sum(gh.Win)
	,[Bet (RMB)] = Sum(gh.BetRMB)
	,[Win (RMB)] = Sum(gh.WinRMB)
	,RTP = CAST((Sum(gh.WinRMB)/ IIF(sum(gh.BetRMB)=0, 1,sum(gh.BetRMB)) * 100) AS decimal(23,2))
from #ghinfo gh
group by 
	gh.Currency
order by gh.Currency

if object_id('tempdb..#userInfo') is not null 
drop table #userInfo


if object_id('tempdb..#ghinfo') is not null 
drop table #ghinfo

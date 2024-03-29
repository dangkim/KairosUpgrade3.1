DECLARE @Id int

SET @Id = 169

/******** Game *********/
SET IDENTITY_INSERT CURRENCY ON

IF NOT EXISTS(SELECT 1 FROM Currency WHERE Id = @Id)
BEGIN
	INSERT [dbo].[Currency] 
	([Id], 
	[IsoCode], 
	[DisplayCode], 
	[Description], 
	[ExchangeRateToCredit], 
	[IsVisible], 
	[IsDeleted], 
	[CreatedBy], 
	[CreatedOnUtc], 
	[UpdatedBy], 
	[UpdatedOnUtc], 
	[DeletedBy], 
	[DeletedOnUtc]) VALUES 
	(
		169, 
		N'ZAR', 
		N'ZAR', 
		N'Indonesia Rupiah', 
		CAST(0.00000000 AS Decimal(23, 8)), 0, 0, 
		NULL, 
		CAST(N'2018-06-07 12:00:00.000' AS DateTime), 
		NULL, 
		CAST(N'2018-06-07 12:00:00.000' AS DateTime), 
		NULL, 
		NULL)
END

SET IDENTITY_INSERT CURRENCY OFF

DECLARE @GameSetting TABLE (
	[GameSettingGroupId] [int] NOT NULL,
	[GameId] [int] NOT NULL,	
	[CoinsDenomination] [nvarchar](max) NOT NULL,
	[CoinsMultiplier] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[GambleMinValue] [decimal](23, 8) NOT NULL,
	[GambleMaxValue] [decimal](23, 8) NOT NULL
 )
-------------------------Coin Settings------------------------------------------
INSERT INTO @GameSetting
(
	GameSettingGroupId
	,GameId	
	,CoinsDenomination
	,CoinsMultiplier
	,CreatedOnUtc
	,GambleMinValue
	,GambleMaxValue
)
SELECT	GameSettingGroupId 
		,GameId 		
		,gs.CoinsDenomination
		,gs.CoinsMultiplier
		,CreatedOnUtc = GETUTCDATE()
		,gs.GambleMinValue
		,gs.GambleMaxValue
FROM	
	[dbo].[GameSetting] gs WITH(NOLOCK)  
WHERE 
	CurrencyId = 62
	AND GameSettingGroupId  = 2 -- Merchant
----------------------------------------Update the Coin Setting---------------------------------------------------

MERGE GAMESETTING AS T
USING @GameSetting AS S
ON (T.GameSettingGroupId = S.GameSettingGroupId AND T.GameId  = S.GameId AND T.CurrencyId = @Id)
WHEN NOT MATCHED BY TARGET
	THEN 
		INSERT(GameSettingGroupId,GameId,CurrencyId,CoinsDenomination,CoinsMultiplier,CreatedOnUtc,GambleMinValue,GambleMaxValue) 
		VALUES(S.GameSettingGroupId,S.GameId,@Id,S.CoinsDenomination,S.CoinsMultiplier,S.CreatedOnUtc,S.GambleMinValue,S.GambleMaxValue)
WHEN MATCHED 
	THEN UPDATE SET T.CoinsDenomination = S.CoinsDenomination	
OUTPUT $action, Inserted.*;
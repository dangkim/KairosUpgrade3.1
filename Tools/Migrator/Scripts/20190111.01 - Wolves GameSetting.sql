DECLARE @GameSetting TABLE (
	[GameSettingGroupId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[CoinsDenomination] [nvarchar](max) NOT NULL,
	[CoinsMultiplier] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[GambleMinValue] [decimal](23, 8) NOT NULL,
	[GambleMaxValue] [decimal](23, 8) NOT NULL,
 UNIQUE 
(
	[GameSettingGroupId] ASC,
	[GameId] ASC,
	[CurrencyId] ASC
))

-----------------------------Game Rtp------------------------------------------------
DECLARE @GameRtp TABLE (
	[GameId] [int] NOT NULL,
	[RtpLevel] [int] NOT NULL,
	[Rtp] [decimal](23, 8) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	
 UNIQUE
(
	[GameId] ASC, [RtpLevel] ASC
)) 


DECLARE @GameId int
SET @GameId = 91
IF NOT EXISTS(SELECT Id FROM Game WHERE Id = @GameId)
BEGIN
	SET IDENTITY_INSERT GAME ON
	
	INSERT INTO GAME
	(
		Id
		,GameType
		,Name
		,Lines
		,RtpLevel
		,IsDeleted
		,CreatedOnUtc
		,IsDisabled
		,DisableOperators
		,IsBetAllLines
		,IsSideBet
		,IsFreeRoundEnabled
	)
	VALUES
	(
		@GameId
		,1
		,'Wolves Slot'
		,25
		,2
		,0
		,GETUTCDATE()
		,0
		,''
		,1
		,0
		,1
	)

	SET IDENTITY_INSERT GAME OFF	
END


-------------------------Coin Settings------------------------------------------
INSERT INTO @GameSetting
(
	GameSettingGroupId
	,GameId
	,CurrencyId
	,CoinsDenomination
	,CoinsMultiplier
	,CreatedOnUtc
	,GambleMinValue
	,GambleMaxValue
)
SELECT	GameSettingGroupId = 1
		,GameId = @GameId
		,gs.CurrencyId
		,gs.CoinsDenomination
		,gs.CoinsMultiplier
		,CreatedOnUtc = GETUTCDATE()
		,gs.GambleMinValue
		,gs.GambleMaxValue
FROM	(
		SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.10;0.20;0.80;1.00;2.00;3.00;5.00;8.00;10.00;15.00;20.00;25.00;30.00;50.00;80.00;100.00;120.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.01;0.02;0.03;0.08;0.10;0.20;0.40;0.50;0.80;1.00;1.50;2.00;3.00;5.00;8.00;10.00;12.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.01;0.02;0.03;0.08;0.10;0.20;0.40;0.50;0.80;1.00;1.50;2.00;3.00;5.00;8.00;10.00;12.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.06;0.08;0.10;0.40;0.60;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '10.00;20.00;30.00;40.00;50.00;80.00;100.00;200.00;250.00;300.00;400.00;500.00;600.00;700.00;800.00;1000.00;1200.00;1500.00;2000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.30;0.60;0.80;1.00;2.00;3.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;80.00;100.00;125.00;150.00;200.00;250.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.20;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00;100.00;120.00;150.00;120.00;150.00;200.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '1.00;2.00;3.00;8.00;10.00;20.00;40.00;50.00;80.00;100.00;150.00;200.00;300.00;500.00;800.00;1,000.00;1,200.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '10.00;20.00;30.00;80.00;100.00;200.00;400.00;500.00;800.00;1,000.00;1,500.00;2,000.00;3,000.00;5,000.00;8,000.00;10,000.00;12,000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '60.00;80.00;100.00;400.00;600.00;1,000.00;2,000.00;3,000.00;4,000.00;5,000.00;8,000.00;10,000.00;15,000.00;20,000.00;25,000.00;50,000.00;60,000.00;80,000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.20;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00;100.00;120.00;150.00;120.00;150.00;200.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0) gs

INSERT INTO @GameSetting
(
	GameSettingGroupId
	,GameId
	,CurrencyId
	,CoinsDenomination
	,CoinsMultiplier
	,CreatedOnUtc
	,GambleMinValue
	,GambleMaxValue
)
SELECT	GameSettingGroupId = 2
		,GameId = @GameId
		,gs.CurrencyId
		,gs.CoinsDenomination
		,gs.CoinsMultiplier
		,CreatedOnUtc = GETUTCDATE()
		,gs.GambleMinValue
		,gs.GambleMaxValue
FROM	(SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.10;0.20;0.80;1.00;2.00;3.00;5.00;8.00;10.00;15.00;20.00;25.00;30.00;50.00;80.00;100.00;120.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.01;0.02;0.03;0.08;0.10;0.20;0.40;0.50;0.80;1.00;1.50;2.00;3.00;5.00;8.00;10.00;12.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.04;0.06;0.10;0.20;0.40;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;30.00;40.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.06;0.08;0.10;0.40;0.60;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '10.00;20.00;30.00;40.00;50.00;80.00;100.00;200.00;250.00;300.00;400.00;500.00;600.00;700.00;800.00;1000.00;1200.00;1500.00;2000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.30;0.60;0.80;1.00;2.00;3.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;80.00;100.00;125.00;150.00;200.00;250.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.20;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00;100.00;120.00;150.00;120.00;150.00;200.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '4.00;6.00;10.00;20.00;40.00;80.00;100.00;200.00;300.00;400.00;500.00;800.00;1,000.00;1,500.00;2,000.00;2,500.00;3,000.00;4,000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '40.00;60.00;100.00;200.00;400.00;800.00;1,000.00;2,000.00;3,000.00;4,000.00;5,000.00;8,000.00;10,000.00;15,000.00;20,000.00;25,000.00;30,000.00;40,000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '60.00;80.00;100.00;400.00;600.00;1,000.00;2,000.00;3,000.00;4,000.00;5,000.00;8,000.00;10,000.00;15,000.00;20,000.00;25,000.00;50,000.00;60,000.00;80000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.20;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00;100.00;120.00;150.00;120.00;150.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 168, IsoCode = 'KR1',  CoinsDenomination = '0.01;0.02;0.03;0.04;0.05;0.08;0.10;0.20;0.25;0.30;0.40;0.50;0.60;0.70;0.80;1.00;1.20;1.50;2.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0) gs

INSERT INTO @GameSetting
(
	GameSettingGroupId
	,GameId
	,CurrencyId
	,CoinsDenomination
	,CoinsMultiplier
	,CreatedOnUtc
	,GambleMinValue
	,GambleMaxValue
)
SELECT	GameSettingGroupId = 3
		,GameId = @GameId
		,gs.CurrencyId
		,gs.CoinsDenomination
		,gs.CoinsMultiplier
		,CreatedOnUtc = GETUTCDATE()
		,gs.GambleMinValue
		,gs.GambleMaxValue
FROM	(SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.10;0.20;0.80;1.00;2.00;3.00;5.00;8.00;10.00;15.00;20.00;25.00;30.00;50.00;80.00;100.00;120.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.01;0.02;0.03;0.08;0.10;0.20;0.40;0.50;0.80;1.00;1.50;2.00;3.00;5.00;8.00;10.00;12.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.01;0.02;0.03;0.08;0.10;0.20;0.40;0.50;0.80;1.00;1.50;2.00;3.00;5.00;8.00;10.00;12.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.06;0.08;0.10;0.40;0.60;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '10.00;20.00;30.00;40.00;50.00;80.00;100.00;200.00;250.00;300.00;400.00;500.00;600.00;700.00;800.00;1000.00;1200.00;1500.00;2000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.30;0.60;0.80;1.00;2.00;3.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;80.00;100.00;125.00;150.00;200.00;250.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.20;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00;100.00;120.00;150.00;120.00;150.00;200.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.20;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00;100.00;120.00;150.00;120.00;150.00;200.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '1.00;2.00;3.00;8.00;10.00;20.00;40.00;50.00;80.00;100.00;150.00;200.00;300.00;500.00;800.00;1,000.00;1,200.00;0.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '10.00;20.00;30.00;80.00;100.00;200.00;400.00;500.00;800.00;1000.00;1500.00;2000.00;3000.00;5000.00;8000.00;10000.00;12000.00;0.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '60.00;80.00;100.00;400.00;600.00;1000.00;2000.00;3000.00;4000.00;5000.00;8000.00;10000.00;15000.00;20000.00;25000.00;50000.00;60000.00;80000.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.02;0.03;0.05;0.10;0.20;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;12.00;15.00;18.00;20.00;25.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
		SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.20;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;60.00;80.00;100.00;120.00;150.00;120.00;150.00;200.00', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0) gs
-------------------------------------------------------Game Rtp---------------------------------------------------------------

INSERT INTO @GameRtp (GameId, RtpLevel, Rtp, IsDeleted) 
SELECT rtp.*
FROM (
			SELECT GameId = @GameId, RtpLevel=1, Rtp=97.11, IsDeleted=0 UNION
			SELECT GameId = @GameId, RtpLevel=2, Rtp=96.56, IsDeleted=0 UNION
			SELECT GameId = @GameId, RtpLevel=3, Rtp=96.06, IsDeleted=0 UNION
			SELECT GameId = @GameId, RtpLevel=4, Rtp=95.08, IsDeleted=0 
	) rtp 

----------------------------------------Update the Coin Setting & Game RTP---------------------------------------------------

MERGE GAMESETTING AS T
USING @GameSetting AS S
ON (T.GameSettingGroupId = S.GameSettingGroupId AND T.GameId  = S.GameId AND T.CurrencyId =S.CurrencyId)
WHEN NOT MATCHED BY TARGET
	THEN 
		INSERT(GameSettingGroupId,GameId,CurrencyId,CoinsDenomination,CoinsMultiplier,CreatedOnUtc,GambleMinValue,GambleMaxValue) 
		VALUES(S.GameSettingGroupId,S.GameId,S.CurrencyId,S.CoinsDenomination,S.CoinsMultiplier,S.CreatedOnUtc,S.GambleMinValue,S.GambleMaxValue)
WHEN MATCHED 
	THEN UPDATE SET T.CoinsDenomination = S.CoinsDenomination	
OUTPUT $action, Inserted.*;

MERGE GAMERTP AS T
USING @GameRtp AS S
ON (T.GameId = S.GameId AND T.RtpLevel  = S.RtpLevel)
WHEN NOT MATCHED BY TARGET
	THEN 
		INSERT(GameId, RtpLevel, Rtp, IsDeleted) 
		VALUES(S.GameId,S.RtpLevel,S.Rtp,S.IsDeleted)
WHEN MATCHED 
	THEN UPDATE SET T.Rtp = S.Rtp	
OUTPUT $action, Inserted.*;
GO


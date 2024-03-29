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
SET @GameId = 96
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
		,'Three Beauties'
		,1
		,1
		,0
		,GETUTCDATE()
		,0
		,''
		,1
		,0
		,0
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
			SELECT CurrencyId = 9, CoinsDenomination = '0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;50.00;100.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 19, CoinsDenomination = '0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;50.00;100.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 27, CoinsDenomination = '0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;50.00;100.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 31, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00;1000.00;1500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 45, CoinsDenomination = '0.03;0.05;0.08;0.10;0.20;0.30;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;50.00;100.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 62, CoinsDenomination = '0.40;0.50;0.60;0.70;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;100.00;200.00;250.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 65, CoinsDenomination = '2.00;3.00;4.00;5.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;150.00;300.00;400.00;500.00;1000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 72, CoinsDenomination = '3.00;4.00;5.00;8.00;9.00;10.00;20.00;25.00;30.00;50.00;75.00;100.00;150.00;200.00;250.00;500.00;1000.00;1250.00;1500.00;2000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 78, CoinsDenomination = '30.00;50.00;100.00;250.00;500.00;700.00;800.00;900.00;1000.00;1500.00;2000.00;3000.00;5000.00;7500.00;10000.00;15000.00;20000.00;50000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 93, CoinsDenomination = '40.00;50.00;60.00;70.00;80.00;100.00;200.00;300.00;400.00;500.00;800.00;1,000.00;1,500.00;2,000.00;2,500.00;5,000.00;10,000.00;20,000.00;25,000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 101, CoinsDenomination = '0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 106, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00;1,000.00;1,500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 120, CoinsDenomination = '2.00;3.00;4.00;5.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;150.00;300.00;400.00;500.00;1000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 126, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00;1,000.00;1,500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 137, CoinsDenomination = '2.00;3.00;4.00;5.00;8.00;10.00;20.00;25.00;30.00;45.00;50.00;100.00;125.00;150.00;250.00;500.00;1000.00;1500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 149, CoinsDenomination = '0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;50.00;100.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 153, CoinsDenomination = '0.50;0.60;1.00;2.00;3.00;4.00;5.00;10.00;15.00;20.00;50.00;70.00;80.00;100.00;125.00;150.00;175.00;200.00;250.00;500.00;1000.00;1500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 165, CoinsDenomination = '400.00;500.00;600.00;700.00;800.00;1000.00;2000.00;3000.00;4000.00;5000.00;8000.00;10000.00;15000.00;20000.00;25000.00;50000.00;100000.00;200000.00;250000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 166, CoinsDenomination = '0.50;0.60;1.00;2.00;3.00;4.00;5.00;10.00;15.00;20.00;50.00;70.00;80.00;100.00;125.00;150.00;175.00;200.00;250.00;500.00;1000.00;1500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0
		) gs
		
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
FROM	(
			SELECT CurrencyId = 9, CoinsDenomination = '0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;50.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 19, CoinsDenomination = '0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 27, CoinsDenomination = '0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;100.00;250.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 31, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 45, CoinsDenomination = '0.03;0.05;0.08;0.10;0.20;0.30;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;50.00;100.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 62, CoinsDenomination = '0.40;0.50;0.60;0.70;0.80;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;100.00;200.00;250.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 65, CoinsDenomination = '2.00;3.00;4.00;5.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;150.00;300.00;400.00;500.00;1000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 72, CoinsDenomination = '3.00;4.00;5.00;8.00;9.00;10.00;20.00;25.00;30.00;50.00;75.00;100.00;150.00;200.00;250.00;500.00;1000.00;1250.00;1500.00;2000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 78, CoinsDenomination = '30.00;50.00;100.00;250.00;500.00;700.00;800.00;900.00;1,000.00;1,500.00;2,000.00;3,000.00;5,000.00;7,500.00;10,000.00;15,000.00;20,000.00;50,000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 93, CoinsDenomination = '40.00;50.00;60.00;70.00;80.00;100.00;200.00;300.00;400.00;500.00;800.00;1,000.00;1,500.00;2,000.00;2,500.00;5,000.00;10,000.00;20,000.00;25,000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 101, CoinsDenomination = '0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 106, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 120, CoinsDenomination = '2.00;3.00;4.00;5.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;150.00;300.00;400.00;500.00;1000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 126, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 137, CoinsDenomination = '2.00;3.00;4.00;5.00;8.00;10.00;20.00;25.00;30.00;45.00;50.00;100.00;125.00;150.00;250.00;500.00;1000.00;1500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 142, CoinsDenomination = '0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 149, CoinsDenomination = '0.03;0.05;0.08;0.10;0.20;0.30;0.40;0.50;1.00;2.00;3.00;4.00;5.00;12.00;15.00;20.00;25.00;50.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 153, CoinsDenomination = '0.50;0.60;1.00;2.00;3.00;4.00;5.00;10.00;15.00;20.00;50.00;70.00;80.00;100.00;125.00;150.00;175.00;200.00;250.00;500.00;1000.00;1500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 165, CoinsDenomination = '400.00;500.00;600.00;700.00;800.00;1000.00;2000.00;3000.00;4000.00;5000.00;8000.00;10000.00;15000.00;20000.00;25000.00;50000.00;100000.00;200000.00;250000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 166, CoinsDenomination = '500.00;600.00;1000.00;2000.00;3000.00;4000.00;5000.00;10000.00;15000.00;20000.00;50000.00;70000.00;80000.00;100000.00;125000.00;150000.00;175000.00;200000.00;250000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 168, CoinsDenomination = '0.03;0.05;0.10;0.25;0.50;0.70;0.80;0.90;1.00;1.50;2.00;3.00;5.00;7.50;10.00;15.00;20.00;50.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0
		) gs
		
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
FROM	(
			SELECT CurrencyId = 27, CoinsDenomination = '0.03;0.05;0.08;0.10;0.25;0.50;1.00;2.00;3.00;4.00;5.00;10.00;15.00;20.00;25.00;50.00;60.00;70.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 31, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 45, CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1.00;1.20;1.30;1.40;1.50;2.00;3.00;4.00;5.00;6.00;7.00;8.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 65, CoinsDenomination = '0.50;1.00;2.00;3.00;4.00;5.00;10.00;25.00;50.00;75.00;100.00;200.00;250.00;500.00;1000.00;2500.00;5000.00;1000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 72, CoinsDenomination = '2.00;3.00;4.00;5.00;8.00;10.00;15.00;25.00;50.00;80.00;100.00;200.00;250.00;500.00;800.00;1000.00;2000.00;3000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 106, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 120, CoinsDenomination = '0.50;1.00;2.00;3.00;4.00;5.00;10.00;25.00;50.00;75.00;100.00;200.00;250.00;500.00;1000.00;2500.00;5000.00;1000.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 126, CoinsDenomination = '0.20;0.50;1.00;2.00;3.00;4.00;5.00;6.00;7.00;8.00;10.00;15.00;20.00;25.00;50.00;75.00;100.00;250.00;500.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT CurrencyId = 149, CoinsDenomination = '0.03;0.05;0.08;0.10;0.25;0.50;1.00;2.00;3.00;4.00;5.00;10.00;15.00;20.00;25.00;50.00;60.00;70.00', CoinsMultiplier = '1;2;3', GambleMinValue = 0, GambleMaxValue = 0
		) gs
-------------------------------------------------------Game Rtp---------------------------------------------------------------
INSERT INTO @GameRtp (GameId, RtpLevel, Rtp, IsDeleted) 
SELECT rtp.*
FROM (
			SELECT GameId = @GameId, RtpLevel=1, Rtp=97.65, IsDeleted=0 UNION
			SELECT GameId = @GameId, RtpLevel=2, Rtp=96.82, IsDeleted=0 UNION
			SELECT GameId = @GameId, RtpLevel=3, Rtp=93.43, IsDeleted=0
	) rtp 
----------------------------------------Update Coin Setting---------------------------------------------------	
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
----------------------------------------Update Game RTP---------------------------------------------------
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
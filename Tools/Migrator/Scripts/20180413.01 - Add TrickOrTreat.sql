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
SET @GameId = 70
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
		,'Trick Or Treat'
		,20
		,1
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

INSERT INTO @GameRtp (GameId, RtpLevel, Rtp, IsDeleted) 
SELECT rtp.*
FROM (
			SELECT GameId = @GameId, RtpLevel=1, Rtp=97.02, IsDeleted=0 UNION
			SELECT GameId = @GameId, RtpLevel=2, Rtp=95.13, IsDeleted=0 UNION
			SELECT GameId = @GameId, RtpLevel=3, Rtp=92.14, IsDeleted=0 UNION
			SELECT GameId = @GameId, RtpLevel=4, Rtp=90.14, IsDeleted=0

	) rtp 

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


-------------------------Coin Settings------------------------------------------
GO
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 9, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 19, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 27, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 31, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(5000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 45, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 48, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2018-01-12 08:51:12.013' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 62, N'0.05;0.1;0.25;0.5;0.6;0.8;1;1.5;3;5;10;25;50;75;100;125;150', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(10000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 65, N'0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;750;800', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(50000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 72, N'0.45;0.75;1;2;3;4;5;10;15;25;50;100;150;250;500;750;800', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(100000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 78, N'4;5;7;10;20;25;50;75;100;200;225;450;900;1000;1500;2000;2500', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 93, N'5;10;25;50;60;80;100;150;300;500;1000;2500;5000;7500;10000;12500;15000', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 101, N'0.02;0.03;0.05;0.1;0.15;0.25;0.3;0.5;0.75;1;1.5;2;2.5;5;10;25;30', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(3000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 106, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(5000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 120, N'0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;750;800', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(50000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 126, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(5000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 137, N'0.15;0.25;0.75;1;1.5;2.5;3;7.5;15;30;50;75;100;125;150;300', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(25000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 149, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 153, N'0.1;0.15;0.5;0.75;1;1.5;1.75;2;5;10;20;40;50;100;120;150;200', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(20000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 165, N'50;100;250;500;600;800;1000;1500;3000;5000;10000;25000;50000;75000;100000;125000;150000', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(10000000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (1, 70, 166, N'100;150;500;750;1000;1500;1750;2000;5000;10000;20000;40000;50000;100000;120000;150000;200000', N'1', NULL, CAST(N'2017-10-18 10:58:00.300' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(20000000.00000000 AS Decimal(23, 8)))

INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 9, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 19, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 27, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 31, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(5000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 45, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 48, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2018-01-12 08:51:12.013' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 62, N'0.05;0.1;0.25;0.5;0.6;0.8;1;1.5;3;5;10;25;50;75;100;125', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(10000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 65, N'0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;600', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(50000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 72, N'0.45;0.75;1;2;3;4;5;10;15;25;50;100;150;250;500;750;800', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(100000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 78, N'4;5;7;10;20;25;50;75;100;200;225;450;900;1000;1500;2000;2500', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 93, N'5;10;25;50;60;80;100;150;300;500;1000;2500;5000;7500;10000;12500', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 101, N'0.02;0.03;0.05;0.1;0.15;0.25;0.3;0.5;0.75;1;1.5;2;2.5;5;10;25;30', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(3000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 106, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(5000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 120, N'0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;600', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(50000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 126, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(5000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 137, N'0.15;0.25;0.75;1;1.5;2.5;3;7.5;15;30;50;75;100;125;150;200', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(25000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 149, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(1000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 153, N'0.1;0.15;0.5;0.75;1;1.5;1.75;2;5;10;20;40;50;100;120;150', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(20000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 165, N'50;100;250;500;600;800;1000;1500;3000;5000;10000;25000;50000;75000;100000;125000', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(10000000.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (2, 70, 166, N'100;150;500;750;1000;1500;1750;2000;5000;10000;20000;40000;50000;100000;120000;150000', N'1', NULL, CAST(N'2017-10-18 10:58:00.310' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(20000000.00000000 AS Decimal(23, 8)))

INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 27, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 31, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;60;65', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 45, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 48, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8', N'1', NULL, CAST(N'2018-01-12 08:51:12.013' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 65, N'0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;600', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 72, N'0.45;0.75;1;2;3;4;5;10;15;25;50;100;150;250;500;750;800', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 106, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;60;65', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 120, N'0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;600', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 126, N'0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;60;65', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))
INSERT [dbo].[GameSetting] ([GameSettingGroupId], [GameId], [CurrencyId], [CoinsDenomination], [CoinsMultiplier], [CreatedBy], [CreatedOnUtc], [UpdatedBy], [UpdatedOnUtc], [GambleMinValue], [GambleMaxValue]) VALUES (3, 70, 149, N'0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', N'1', NULL, CAST(N'2017-10-18 10:58:00.320' AS DateTime), NULL, NULL, CAST(0.00000000 AS Decimal(23, 8)), CAST(0.00000000 AS Decimal(23, 8)))

-------------------------------------------------------Game Rtp---------------------------------------------------------------
GO


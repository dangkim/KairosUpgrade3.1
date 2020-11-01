DECLARE @GameId int

DECLARE @RtpLevel Table
(
	GameId int
	,RtpLevel int
	,Rtp decimal(23,8)
)

DECLARE @CoinSetting TABLE
(
	GameSettingGroupId int
	,GameId int
	,CurrencyId int
	,CoinsDenomination nvarchar(MAX)
	,CoinsMultiplier nvarchar(MAX)
	,GambleMinValue decimal(23,8)
	,GambleMaxValue decimal(23,8)
)

SET @GameId = 64

/******** Game *********/
SET IDENTITY_INSERT GAME ON

IF NOT EXISTS(SELECT 1 FROM Game WHERE Id = @GameId)
BEGIN
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
		,'Wilds and the Beanstalk'
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
END
ELSE
BEGIN
	UPDATE	GAME
	SET		Name = 'Wilds and the Beanstalk'
	WHERE	Id = @GameId
END

SET IDENTITY_INSERT GAME OFF

/******** Game Rtp *********/

INSERT INTO @RtpLevel VALUES (@GameId, 1, 96.53)

UPDATE	gr
SET		gr.Rtp = rl.Rtp
FROM	GameRtp gr
		INNER JOIN @RtpLevel rl
			ON gr.GameId = rl.GameId
			AND gr.RtpLevel = rl.RtpLevel

INSERT INTO GameRtp
(
	GameId
	,RtpLevel
	,Rtp
	,IsDeleted
)
SELECT	rl.GameId
		,rl.RtpLevel
		,rl.Rtp
		,IsDeleted = 0
FROM	@RtpLevel rl
		LEFT OUTER JOIN GameRtp gr
			ON gr.GameId = rl.GameId
			AND gr.RtpLevel = rl.RtpLevel
WHERE	gr.Id IS NULL

/******* Coin Settings *******/
INSERT INTO @CoinSetting
(
	GameSettingGroupId
	,GameId
	,CurrencyId
	,CoinsDenomination
	,CoinsMultiplier
	,GambleMinValue
	,GambleMaxValue
)
SELECT	cs.GameSettingGroupId
		,GameId = @GameId
		,cs.CurrencyId
		,cs.CoinsDenomination
		,cs.CoinsMultiplier
		,cs.GambleMinValue
		,cs.GambleMaxValue
FROM	(
			SELECT GameSettingGroupId = 1, CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.1;0.2;0.5;0.1;0.2;0.3;0.4;0.6;1', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.01;0.02;0.05;0.1;0.2;0.3;0.4;0.6;1', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.05;0.1;0.25;0.5;1;1.5;2;3;5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '2.5;5;12.5;25;50;75;100;150;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '2.5;5;12.5;25;50;75;100;150;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '25;50;125;250;500;750;1000;1500;2500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '50;100;250;500;1000;1500;2000;3000;5000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION

			SELECT GameSettingGroupId = 2, CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.1;0.2;0.5;0.1;0.2;0.3;0.4;0.6;1', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.01;0.02;0.05;0.1;0.2;0.3;0.4;0.6;1', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.05;0.1;0.25;0.5;1;1.5;2;3;5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '2.5;5;12.5;25;50;75;100;150;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '2.5;5;12.5;25;50;75;100;150;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '25;50;125;250;500;750;1000;1500;2500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '50;100;250;500;1000;1500;2000;3000;5000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION

			SELECT GameSettingGroupId = 3, CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.025;0.05;0.125;0.25;0.5;0.75;1;1.5;2.5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.025;0.005;0.0125;0.05;0.1;0.075;0.1;0.15;0.25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.25;0.5;1.25;2.5;5;7.5;10;15;25', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
		) cs

UPDATE	gs
SET		gs.CoinsDenomination = cs.CoinsDenomination
		,gs.CoinsMultiplier = cs.CoinsMultiplier
		,gs.UpdatedOnUtc = GETUTCDATE()
		,gs.GambleMinValue = cs.GambleMinValue
		,gs.GambleMaxValue = cs.GambleMaxValue
FROM	GAMESETTING gs
		INNER JOIN @CoinSetting cs
			ON cs.GameSettingGroupId = gs.GameSettingGroupId
			AND cs.GameId = gs.GameId
			AND cs.CurrencyId = gs.CurrencyId

INSERT INTO GAMESETTING
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
SELECT	cs.GameSettingGroupId
		,cs.GameId
		,cs.CurrencyId
		,cs.CoinsDenomination
		,cs.CoinsMultiplier
		,CreatedOnUtc = GETUTCDATE()
		,cs.GambleMinValue
		,cs.GambleMaxValue
FROM	@CoinSetting cs
		LEFT OUTER JOIN GAMESETTING gs
			ON gs.GameSettingGroupId = cs.GameSettingGroupId
			AND gs.GameId = cs.GameId
			AND gs.CurrencyId = cs.CurrencyId
WHERE	gs.GameId IS NULL
GO
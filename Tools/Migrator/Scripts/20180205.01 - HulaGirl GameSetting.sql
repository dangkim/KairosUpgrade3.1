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

SET @GameId = 74

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
		,'Hula Girl'
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
	SET		Name = 'Hula Girl'
	WHERE	Id = @GameId
END

SET IDENTITY_INSERT GAME OFF

/******** Game Rtp *********/

INSERT INTO @RtpLevel VALUES (@GameId, 1, 96.01)

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
			SELECT GameSettingGroupId = 1, CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30;50', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5;6', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.15;0.25;0.5;1;1.5;2;2.5;5;8;12;25;30;40;50;60;100;125;150', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.02;0.03;0.06;0.1;0.15;0.2;0.25;0.5;0.75;1;1.5;3;5;8;10;12;15;20', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.06;0.1;0.2;0.4;0.5;1;1.5;2;2.5;3;5;10;15;20;25;50;75;80', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.1;0.15;0.5;0.75;1;1.5;2;2.5;5;8;10;15;20;25;50;100;125;150', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '4;8;10;25;50;75;80;100;250;500;800;1000;1200;1500;2000;3000;4000;5000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.5;0.8;1;2;3;4;8;10;15;25;50;75;80;100;125;200;300;500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5;6', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.3;0.5;1;2;3;5;10;15;20;25;50;75;100;125;150;200;250;300', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '6;10;20;40;50;100;150;200;250;300;500;1000;1500;2000;2500;5000;7500;8000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '60;100;200;400;500;1000;1500;2000;2500;3000;5000;10000;15000;20000;25000;50000;75000;80000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '100;150;500;750;1000;1500;2000;2500;5000;8000;10000;15000;20000;25000;50000;100000;125000;150000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5;6', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5;6', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30;50', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30;50', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5;6', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.3;0.5;1;2;3;5;10;15;20;25;50;75;100;125;150;200;250;300', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 1, CurrencyId = 48, IsoCode = 'GBP',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5;6', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			--SELECT GameSettingGroupId = 1, CurrencyId = 169, IsoCode = 'KR1',  CoinsDenomination = '', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION

			SELECT GameSettingGroupId = 2, CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.15;0.25;0.5;1;2;2.5;5;8;10;25;30;40;50;60;100;125;150', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.02;0.03;0.06;0.1;0.15;0.2;0.25;0.5;0.75;1;1.5;3;5;8;10;12;15', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.06;0.1;0.2;0.4;0.5;1;1.5;2;2.5;3;5;10;15;20;25;50;60', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.15;0.5;0.75;1;1.5;2;2.5;5;8;10;15;20;25;50;100', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '4;5;8;10;20;25;50;75;100;200;250;500;800;1000;2500;4000;4500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.5;0.8;1;2;3;5;10;15;25;50;75;80;100;125;250;300;450', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.3;0.5;1;2;3;5;10;15;20;25;50;100;125;150;200;250;300', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '6;10;20;40;50;100;150;200;250;300;500;1000;1500;2000;2500;5000;6000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '60;100;200;400;500;1000;1500;2000;2500;3000;5000;10000;15000;20000;25000;50000;60000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '150;500;750;1000;1500;2000;2500;5000;8000;10000;15000;20000;25000;50000;100000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.3;0.5;1;2;3;5;10;15;20;25;50;100;125;150;200;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 2, CurrencyId = 48, IsoCode = 'GBP',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;3;3.5;4;5', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			--SELECT GameSettingGroupId = 2, CurrencyId = 169, IsoCode = 'KR1',  CoinsDenomination = '', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION

			SELECT GameSettingGroupId = 3, CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30;50;60', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.1;0.25;0.5;0.75;1;2;2.5;5;6;8;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.15;0.25;0.5;1;1.5;2;2.5;5;8;12;25;30;40;50;60;100;125;150;300', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.02;0.03;0.05;0.1;0.15;0.2;0.25;0.5;1;1.5;2.5;5;7.5;8;10;15;20;25;30', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.06;0.1;0.25;0.5;1;1.5;2;2.5;3;5;10;15;20;25;50;75;90;100;120', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.1;0.15;0.5;0.75;1;1.5;2;2.5;5;8;10;15;20;25;50;100;125;150;200', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '4;8;10;25;50;75;100;250;500;800;1000;1250;1500;2500;5000;7500;8000;9000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.5;0.75;1;2;2.5;5;8;10;25;50;75;80;100;125;250;500;700;800;900', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.1;0.25;0.5;0.75;1;2;2.5;5;6;8', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.3;0.5;1;2;3;5;10;20;25;50;75;100;125;150;200;250;400;500;600', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30;50;60', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.03;0.05;0.1;0.2;0.3;0.5;1;2;3;4;5;8;10;15;20;25;30;50;60', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.1;0.25;0.5;0.75;1;2;2.5;5;6;8;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.3;0.5;1;2;3;5;10;20;25;50;75;100;125;150;200;250;400;500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
			SELECT GameSettingGroupId = 3, CurrencyId = 48, IsoCode = 'GBP',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.1;0.25;0.5;0.75;1;2;2.5;5;6;8', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
			--SELECT GameSettingGroupId = 3, CurrencyId = 169, IsoCode = 'KR1',  CoinsDenomination = '', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
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
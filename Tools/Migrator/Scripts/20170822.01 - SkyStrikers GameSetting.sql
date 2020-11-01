SET IDENTITY_INSERT GAME ON

DECLARE @GameId int

SET @GameId = 69

IF NOT EXISTS(SELECT Id FROM Game WHERE Id = @GameId)
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
		,'Sky Strikers'
		,20
		,1
		,0
		,GETUTCDATE()
		,0
		,''
		,1
		,0
		,0
	)
END

SET IDENTITY_INSERT GAME OFF

--Game RTP
IF NOT EXISTS(SELECT Id FROM GameRtp WHERE GameId = @GameId)
BEGIN
	INSERT INTO GameRtp (GameId, RtpLevel, Rtp, IsDeleted) VALUES (@GameId, 1, 97.33, 0)
	INSERT INTO GameRtp (GameId, RtpLevel, Rtp, IsDeleted) VALUES (@GameId, 2, 95.53, 0)
	INSERT INTO GameRtp (GameId, RtpLevel, Rtp, IsDeleted) VALUES (@GameId, 3, 93.25, 0)
	INSERT INTO GameRtp (GameId, RtpLevel, Rtp, IsDeleted) VALUES (@GameId, 4, 91.47, 0)
END

--Coin Settings
IF NOT EXISTS(SELECT GameId FROM GAMESETTING WHERE GameId = @GameId)
BEGIN
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
	SELECT	GameSettingGroupId = 1
			,GameId = @GameId
			,gs.CurrencyId
			,gs.CoinsDenomination
			,gs.CoinsMultiplier
			,CreatedOnUtc = GETUTCDATE()
			,gs.GambleMinValue
			,gs.GambleMaxValue
	FROM	(
				SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;75;200;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.25;0.3;0.4;0.5;1;2;2.5;5;8;10;20;30;50;100;250;500;800;1000;1500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.03;0.04;0.05;0.06;0.08;1;2;3;4;5;6;8;10;12;15;20;25;50;100', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.1;0.2;0.3;0.5;0.8;1;2;3;4;5;10;25;30;50;100;150;200;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.3;0.4;0.5;0.8;1;2;3;4;5;10;15;25;50;100;200;250;300;500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '10;20;25;30;50;80;100;125;250;500;750;1000;2000;3000;4000;5000;8000;10000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.8;1;2;3;5;10;20;25;50;80;100;200;250;500;800;1000;2000;3000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;1.2;1.3;1.4;1.5;2;3;4;5;6;7;8;9', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.5;0.75;1;2;3;4;5;10;25;50;75;100;200;250;500;1000;2000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '10;20;30;50;80;100;200;300;400;500;1000;2500;3000;5000;10000;15000;20000;25000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '100;200;300;500;800;1000;2000;3000;4000;5000;10000;25000;30000;50000;100000;150000;200000;250000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '300;400;500;800;1000;2000;3000;4000;5000;10000;15000;25000;50000;100000;200000;250000;300000;500000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;75;200;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;75;200;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.5;0.75;1;2;3;4;5;10;25;50;75;100;200;250;500;1000;2000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
			) gs

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
	SELECT	GameSettingGroupId = 2
			,GameId = @GameId
			,gs.CurrencyId
			,gs.CoinsDenomination
			,gs.CoinsMultiplier
			,CreatedOnUtc = GETUTCDATE()
			,gs.GambleMinValue
			,gs.GambleMaxValue
	FROM	(
				SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;75;100;120', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.25;0.3;0.4;0.5;1;2;2.5;5;8;10;20;25;50;75;100;125;250;500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.03;0.04;0.05;0.06;0.08;1;2;3;4;5;6;8;10;12;15;20;25;50', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.1;0.2;0.3;0.5;0.8;1;2;3;4;5;10;25;30;50;100;150;175;200', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.3;0.4;0.5;0.8;1;2;3;4;5;10;15;25;50;100;200;250;275;300', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '10;20;25;30;50;80;100;125;250;500;750;1000;2000;3000;4000;5000;8000;10000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.8;1;2;3;5;10;20;25;50;80;100;200;250;500;800;1000;1200;1500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;1.2;1.3;1.4;1.5;2;3;4;5;6;7;8;9', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.5;0.75;1;2;3;4;5;10;25;50;75;100;200;250;500;1000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '10;20;30;50;80;100;200;300;400;500;1000;2500;3000;5000;10000;15000;17500;20000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '100;200;300;500;800;1000;2000;3000;4000;5000;10000;25000;30000;50000;100000;150000;175000;200000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '300;400;500;800;1000;2000;3000;4000;5000;10000;15000;25000;50000;100000;200000;250000;275000;300000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;75;100;120', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;75;100;120', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10;50', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.5;0.75;1;2;3;4;5;10;25;50;75;100;200;250;500;1000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
			) gs

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
	SELECT	GameSettingGroupId = 3
			,GameId = @GameId
			,gs.CurrencyId
			,gs.CoinsDenomination
			,gs.CoinsMultiplier
			,CreatedOnUtc = GETUTCDATE()
			,gs.GambleMinValue
			,gs.GambleMaxValue
	FROM	(
				SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;60;65;70', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.8;1;2;3;5;10;20;25;50;80;100;200;250;500;800;1000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;1.2;1.3;1.4;1.5;2;3;4;5;6;7;8;9', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.5;0.75;1;2;3;4;5;10;25;50;75;100;200;250;500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;60;65;70', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.05;0.06;0.08;0.1;0.25;0.3;0.5;1;2;3;5;8;10;25;50;60;65;70', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;1;2;3;4;5;6;7;8;9;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.5;0.75;1;2;3;4;5;10;25;50;75;100;200;250;500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
			) gs
END
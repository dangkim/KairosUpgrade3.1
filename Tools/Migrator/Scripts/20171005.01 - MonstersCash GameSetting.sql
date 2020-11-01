DECLARE @GameId int

SET @GameId = 68

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
		,'Monsters Cash'
		,10
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

	--Coin Settings
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
				SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100;125', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10;12;15', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.15;0.25;0.75;1;1.5;2.5;3;7.5;15;30;50;75;100;125;150;300;400;500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.02;0.03;0.05;0.1;0.15;0.25;0.3;0.5;0.75;1;1.5;2;2.5;5;10;25;30;40;50', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.05;0.1;0.25;0.5;0.6;0.8;1;1.5;3;5;10;25;50;75;100;125;150;175;200', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.1;0.15;0.5;0.75;1;1.5;1.75;2;5;10;20;40;50;100;120;150;200;250;300', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '4;5;7;10;20;25;50;75;100;200;225;450;900;1000;1500;2000;2500;3000;5000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.45;0.75;1;2;3;4;5;10;15;25;50;100;150;250;500;750;800;900;1000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10;12;15', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;750;800;900;1000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '5;10;25;50;60;80;100;150;300;500;1000;2500;5000;7500;10000;12500;15000;17500;20000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '50;100;250;500;600;800;1000;1500;3000;5000;10000;25000;50000;75000;100000;125000;150000;175000;200000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '100;150;500;750;1000;1500;1750;2000;5000;10000;20000;40000;50000;100000;120000;150000;200000;250000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10;12;15', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10;12;15', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100;125', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100;125', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10;12;15', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;750;800;900;1000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
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
				SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10;12', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 137, IsoCode = 'THB',  CoinsDenomination = '0.15;0.25;0.75;1;1.5;2.5;3;7.5;15;30;50;75;100;125;150;200;250', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 101, IsoCode = 'MYR',  CoinsDenomination = '0.02;0.03;0.05;0.1;0.15;0.25;0.3;0.5;0.75;1;1.5;2;2.5;5;10;25;30', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 62, IsoCode = 'IDR',  CoinsDenomination = '0.05;0.1;0.25;0.5;0.6;0.8;1;1.5;3;5;10;25;50;75;100;125', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 153, IsoCode = 'VND',  CoinsDenomination = '0.1;0.15;0.5;0.75;1;1.5;1.75;2;5;10;20;40;50;100;120;150;175', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 78, IsoCode = 'KRW',  CoinsDenomination = '4;5;7;10;20;25;50;75;100;200;225;450;900;1000;1500;2000;2500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.45;0.75;1;2;3;4;5;10;15;25;50;100;150;250;500;750;800', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;600;800', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 93, IsoCode = 'MMK',  CoinsDenomination = '5;10;25;50;60;80;100;150;300;500;1000;2500;5000;7500;10000;12500', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 165, IsoCode = 'ID2',  CoinsDenomination = '50;100;250;500;600;800;1000;1500;3000;5000;10000;25000;50000;75000;100000;125000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 166, IsoCode = 'VN2',  CoinsDenomination = '100;150;500;750;1000;1500;1750;2000;5000;10000;20000;40000;50000;100000;120000;150000;175000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 19, IsoCode = 'BND',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 9, IsoCode = 'AUD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10;12', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;0.6;1.5;2;3;4;5;8;10;25;50;75;100', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;2.5;2.75;3;5;8;10;12', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.3;0.5;1.5;2;3;5;6;15;30;65;120;165;330;500;600;800', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
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
				SELECT CurrencyId = 31, IsoCode = 'RMB',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;1;2;3;4;5;10;15;20;25;50;100;150', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 149, IsoCode = 'USD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;3;4;5;10;15;20', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 72, IsoCode = 'JPY',  CoinsDenomination = '0.5;1;2;3;4;5;10;25;50;75;100;150;200;250;500;750;1000;1500;2000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 45, IsoCode = 'EUR',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;3;4;5;8;10;12;15', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 65, IsoCode = 'INR',  CoinsDenomination = '0.3;0.5;1;2;3;5;10;15;20;25;50;100;200;300;400;500;800;1000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 126, IsoCode = 'SEK',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;1;2;3;4;5;10;15;20;25;50;100;150', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 106, IsoCode = 'NOK',  CoinsDenomination = '0.03;0.05;0.15;0.2;0.3;0.5;1;2;3;4;5;10;15;20;25;50;100;150', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 27, IsoCode = 'CAD',  CoinsDenomination = '0.01;0.02;0.03;0.05;0.08;0.1;0.25;0.5;1;2;3;4;5;10;15;20', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0 UNION
				SELECT CurrencyId = 120, IsoCode = 'RUB',  CoinsDenomination = '0.3;0.5;1;2;3;5;10;15;20;25;50;100;200;300;400;500;800;1000', CoinsMultiplier = 1, GambleMinValue = 0, GambleMaxValue = 0
			) gs

	--Game RTP
	INSERT INTO GameRtp (GameId, RtpLevel, Rtp, IsDeleted) VALUES (@GameId, 1, 96.97, 0)
END

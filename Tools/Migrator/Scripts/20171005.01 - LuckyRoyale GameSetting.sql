SET IDENTITY_INSERT GAME ON

DECLARE @GameId int

SET @GameId = 67

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
		,'Lucky Royale'
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

	--Game RTP
	INSERT INTO GameRtp (GameId, RtpLevel, Rtp, IsDeleted) VALUES (@GameId, 1, 96.99, 0)
	INSERT INTO GameRtp (GameId, RtpLevel, Rtp, IsDeleted) VALUES (@GameId, 2, 95.00, 0)
	INSERT INTO GameRtp (GameId, RtpLevel, Rtp, IsDeleted) VALUES (@GameId, 3, 94.00, 0)

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
	SELECT	gs.GameSettingGroupId
			,@GameId
			,gs.CurrencyId
			,gs.CoinsDenomination
			,gs.CoinsMultiplier
			,CreatedOnUtc = GETUTCDATE()
			,gs.GambleMinValue
			,gs.GambleMaxValue
	FROM	GAMESETTING gs WITH(NOLOCK)
			INNER JOIN GAME g WITH(NOLOCK)
				ON g.Id = gs.GameId
	WHERE	g.Id = 46 --Four Guardians
END
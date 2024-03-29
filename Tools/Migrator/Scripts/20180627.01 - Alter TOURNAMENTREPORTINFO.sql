
ALTER TABLE dbo.TOURNAMENTREPORTINFO 
ADD ChangeTime DateTIme NOT NULL DEFAULT(GETDATE());
GO

ALTER PROCEDURE [dbo].[TOURNAMENTREPORTINFOINSERT](
	@ChangeTime		  datetime,
	@TournamentId     int,
	@OperatorId       int,
	@UserId           int,
	@GameId           int,
	@Platform         int,
	@Level            int,
	@TrxCount         int,
	@Bet              decimal(23,8),
	@Win              decimal(23,8),
	@BetL             decimal(23,8),
	@WinL             decimal(23,8),
	@TimeFristBet     datetime,
	@TimeLastBet      datetime)
AS
BEGIN
	DECLARE @ISNEW INT;
	
	BEGIN TRAN
		UPDATE TOURNAMENTREPORTINFO WITH (SERIALIZABLE) SET
		TrxCount = TrxCount + @TrxCount,
		Bet = Bet + @Bet,
		Win = Win + @Win,
		BetL = BetL + @BetL,
		WinL = WinL + @WinL,
		TimeLastBet = CASE WHEN TimeLastBet < @TimeLastBet THEN  @TimeLastBet
		ELSE 
			TimeLastBet
		END 
		WHERE 
			ChangeTime = @ChangeTime 
			AND TournamentId=@TournamentId 
			AND OperatorId=@OperatorId 
			AND UserId=@UserId 
			AND GameId=@GameId 
			AND [Platform]=@Platform 
			AND [Level]=@Level;

		SET @ISNEW = @@ROWCOUNT;
		
		IF (@ISNEW = 0)
		BEGIN
			INSERT INTO TOURNAMENTREPORTINFO(ChangeTime, TournamentId, OperatorId, UserId, GameId, [Platform], [Level], TrxCount, Bet, Win, BetL, WinL, TimeFirstBet, TimeLastBet) VALUES
			(@ChangeTime, @TournamentId, @OperatorId, @UserId, @GameId, @Platform, @Level, @TrxCount, @Bet, @Win, @BetL, @WinL, @TimeFristBet, @TimeLastBet);
		END
	COMMIT TRAN

	RETURN @ISNEW;
END
DECLARE @GameId int

DECLARE @RtpLevel Table
(
	GameId int
	,RtpLevel int
	,Rtp decimal(23,8)
)

SET @GameId = 63

/******** Game Rtp *********/

INSERT INTO @RtpLevel VALUES (@GameId, 5, 96.71)

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
GO
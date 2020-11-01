DECLARE @RtpLevel Table
(
	GameId int
	,RtpLevel int
	,Rtp decimal(23,8)
)

--Little Monsters
INSERT INTO @RtpLevel VALUES (5, 1, 97.00)
INSERT INTO @RtpLevel VALUES (5, 2, 94.91)
INSERT INTO @RtpLevel VALUES (5, 3, 92.65)
INSERT INTO @RtpLevel VALUES (5, 4, 89.92)

--Deep Blue
INSERT INTO @RtpLevel VALUES (10, 1, 97.00)
INSERT INTO @RtpLevel VALUES (10, 2, 94.91)
INSERT INTO @RtpLevel VALUES (10, 3, 92.65)
INSERT INTO @RtpLevel VALUES (10, 4, 89.92)

--Golden Eggs
INSERT INTO @RtpLevel VALUES (15, 1, 97.00)
INSERT INTO @RtpLevel VALUES (15, 2, 94.91)
INSERT INTO @RtpLevel VALUES (15, 3, 92.65)
INSERT INTO @RtpLevel VALUES (15, 4, 89.92)

--God Of Gamblers
INSERT INTO @RtpLevel VALUES (28, 1, 97.00)
INSERT INTO @RtpLevel VALUES (28, 2, 94.91)
INSERT INTO @RtpLevel VALUES (28, 3, 92.65)
INSERT INTO @RtpLevel VALUES (28, 4, 89.92)

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
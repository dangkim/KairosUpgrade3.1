DECLARE @GameId INT = 45;

UPDATE GameRtp SET Rtp = 97.65 WHERE GameId = @GameId AND RtpLevel = 1;
UPDATE GameRtp SET Rtp = 96.82 WHERE GameId = @GameId AND RtpLevel = 2;
UPDATE GameRtp SET Rtp = 93.43 WHERE GameId = @GameId AND RtpLevel = 3;
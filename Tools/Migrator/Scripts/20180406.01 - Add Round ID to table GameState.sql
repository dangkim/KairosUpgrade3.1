ALTER TABLE dbo.GameState 
ADD LastRoundId bigint NOT NULL DEFAULT(0);
GO

ALTER TABLE dbo.GameHistory 
ADD RoundId bigint NULL;
GO

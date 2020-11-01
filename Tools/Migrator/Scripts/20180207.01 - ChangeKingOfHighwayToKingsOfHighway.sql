UPDATE	Game
SET		Name = 'Kings of Highway'
		,UpdatedOnUtc = GetUtcDate()
WHERE	Id = 61
		AND Name = 'King of Highway'
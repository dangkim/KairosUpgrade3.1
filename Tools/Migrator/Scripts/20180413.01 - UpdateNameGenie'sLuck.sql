UPDATE	Game
SET		Name = 'Genies Luck'
		,UpdatedOnUtc = GetUtcDate()
WHERE	Id = 80
		AND Name = 'Genie Luck'
UPDATE	Game
SET		Name = 'Lady Fortune'
		,UpdatedOnUtc = GetUtcDate()
WHERE	Id = 36
		AND Name = 'Lady Luck'
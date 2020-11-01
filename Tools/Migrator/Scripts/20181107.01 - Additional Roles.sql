SET IDENTITY_INSERT [Role] ON;

INSERT INTO [Role] (
	Id,
	Active,
	Name,
	OperatorId,
	IsDeleted,
	CreatedBy,
	CreatedOnUtc,
	UpdatedBy,
	UpdatedOnUtc,
	DeletedBy,
	DeletedOnUtc)
VALUES
	(9, 1, 'Finance', 1, 0, NULL, GETUTCDATE(), NULL, GETUTCDATE(), NULL, NULL),
	(10, 1, 'Minimum', 1, 0, NULL, GETUTCDATE(), NULL, GETUTCDATE(), NULL, NULL);

SET IDENTITY_INSERT [Role] OFF;
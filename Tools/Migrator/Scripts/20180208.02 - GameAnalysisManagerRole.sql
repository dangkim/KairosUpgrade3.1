--USE [Slots]
DECLARE @Id int
SET @Id = 8
SET IDENTITY_INSERT [Role] ON

IF NOT EXISTS(SELECT 1 FROM [Role] WHERE Id = @Id)
BEGIN
	INSERT INTO [Role]
	(
		Id
		,Active
		,Name
		,OperatorId
		,IsDeleted
		,CreatedOnUtc
		,UpdatedOnUtc
	)
	VALUES
	(
		8
		,1
		,'Game Analysis - Manager'
		,1
		,0
		,GetUtcDate()
		,GetUtcDate()
	)
END

SET IDENTITY_INSERT [Role] OFF
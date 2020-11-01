DECLARE @Id int
DECLARE @IsoCode nvarchar(8)

SET @Id = 168
SET @IsoCode = 'KR1'
/******** Game *********/
SET IDENTITY_INSERT CURRENCY ON

IF NOT EXISTS(SELECT 1 FROM Currency WHERE Id = @Id)
BEGIN
	INSERT INTO CURRENCY
	(
		Id
		,IsoCode
		,DisplayCode
		,[Description]
		,ExchangeRateToCredit
		,IsVisible
		,IsDeleted
		,CreatedBy
		,CreatedOnUtc
		,UpdatedBy
		,UpdatedOnUtc
		,DeletedBy
		,DeletedOnUtc
	)
	VALUES
	(
		@Id
		,@IsoCode
		,@IsoCode
		,'Korea (South) Won 1'
		,0.0055
		,1
		,0
		,NULL
		,GETUTCDATE()
		,NULL
		,GETUTCDATE()
		,NULL
		,NULL
	)
END

SET IDENTITY_INSERT CURRENCY OFF
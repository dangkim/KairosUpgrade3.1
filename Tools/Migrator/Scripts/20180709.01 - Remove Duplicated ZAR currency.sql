DELETE FROM [GameSetting] WHERE [CurrencyId] = 169;
GO

UPDATE [Currency] SET IsDeleted=1, IsoCode='ZAR_DEL', DisplayCode='ZAR_DEL' WHERE [Id] = 169;

alter table [dbo].[Operator]
alter column Tag nvarchar(255)
GO
alter table [dbo].[Operator]
ADD CONSTRAINT AK_Tag UNIQUE (Tag);   
GO
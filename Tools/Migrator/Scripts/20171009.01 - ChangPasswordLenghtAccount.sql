alter table account 
alter column [Password] nvarchar(256)
 
update acc
	set acc.[password] = LOWER(CONVERT(nvarchar(256), hashbytes('sha2_256', cast(acc.[password] as varchar(256))),2)) 
from  Account acc

where acc.OperatorId = 1 and acc.[password] is NOT null

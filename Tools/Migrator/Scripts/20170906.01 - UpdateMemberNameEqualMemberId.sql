declare @tag  nvarchar(64)='{tag}';
declare @OperatorId INT;
select @OperatorId = o.Id from operator o with(nolock) where o.name =@tag or o.tag = @tag


--update member 
update u 
set u.Name = u.externalId
from [user] u with(nolock) 
where u.OperatorId = @OperatorId

-- get members after updated
select 
	[No] = row_number() over(order by (select 1)), 
	[Member Name]= u.Name, 
	[Member Id] = u.externalId

from [user] u with(nolock) where u.OperatorId = @OperatorId
	

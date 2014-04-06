
select u.Name, u.Email, r.Name as UserRole from dbo.UsersInRoles uir
	inner join dbo.Users u on u.Id = uir.UserID
	inner join dbo.Roles r on r.id = uir.RoleID;
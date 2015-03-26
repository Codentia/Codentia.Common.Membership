if exists (select * from sysobjects where id = object_id(N'[dbo].[SystemUser_SetRole]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemUser_SetRole]
GO

CREATE PROCEDURE [dbo].[SystemUser_SetRole]

					(@SystemUserId INT,
					 @RoleName NVARCHAR(50))

AS

SET NOCOUNT ON

DECLARE @UserId uniqueidentifier
DECLARE @RoleId uniqueidentifier

SELECT @UserId=UserId FROM dbo.SystemUser WHERE SystemUserId=@SystemUserId
SELECT @RoleId=RoleId FROM dbo.aspnet_Roles WHERE RoleName=@RoleName

-- if already exists return
IF EXISTS (SELECT * FROM dbo.aspnet_UsersInRoles WHERE UserId=@UserId AND RoleId=@RoleId)
	BEGIN
			RETURN	
	END

ELSE
	-- if user exists but different role
	IF EXISTS (SELECT 1 FROM dbo.aspnet_UsersInRoles WHERE UserId=@UserId)
		BEGIN
				UPDATE dbo.aspnet_UsersInRoles
				SET RoleId=@RoleId
				WHERE UserId=@UserId
		END
		
	ELSE
		-- user/role relationship not set up yet
		BEGIN
				INSERT INTO dbo.aspnet_UsersInRoles (UserId, RoleId)
				VALUES (@UserId, @RoleId)
		END		




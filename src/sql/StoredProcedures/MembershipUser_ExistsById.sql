IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.MembershipUser_ExistsById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.MembershipUser_ExistsById
	END
GO

CREATE PROCEDURE dbo.MembershipUser_ExistsById
	@UserId			UNIQUEIDENTIFIER,
	@Exists			BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS ( SELECT 1 FROM dbo.aspnet_Users WHERE UserId = @UserId )
		BEGIN
			SET @Exists = 1
		END
	ELSE
		BEGIN
			SET @Exists = 0
		END
END
GO 
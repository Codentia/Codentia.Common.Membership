IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.SystemUser_SetForcePassword') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_SetForcePassword
	END
GO

CREATE PROCEDURE dbo.SystemUser_SetForcePassword
	@SystemUserId			INT
AS
BEGIN
	SET NOCOUNT ON

	UPDATE	dbo.SystemUser
	SET		ForcePassword			= 1
	WHERE	SystemUserId = @SystemUserId
END
GO
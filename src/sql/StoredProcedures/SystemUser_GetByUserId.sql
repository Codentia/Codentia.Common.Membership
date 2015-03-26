IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.SystemUser_GetByUserId') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_GetByUserId
	END
GO

CREATE PROCEDURE dbo.SystemUser_GetByUserId
	@UserId			UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SystemUserId INT

	SELECT @SystemUserId = SystemUserId FROM dbo.SystemUser WHERE UserId = @UserId

	EXEC dbo.SystemUser_GetById
						@SystemUserId	= @SystemUserId
END
GO
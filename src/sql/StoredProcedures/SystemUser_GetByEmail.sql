IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.SystemUser_GetByEmail') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_GetByEmail
	END
GO

CREATE PROCEDURE dbo.SystemUser_GetByEmail
	@EmailAddress			NVARCHAR(200)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SystemUserId	INT
	DECLARE @EmailAddressId INT

	SELECT @EmailAddressId = EmailAddressId FROM dbo.EmailAddress WHERE EmailAddress = @EmailAddress
	SELECT @SystemUserId = SystemUserId FROM dbo.SystemUser_EmailAddress WHERE EmailAddressId = @EmailAddressId

	EXEC dbo.SystemUser_GetById
						@SystemUserId	= @SystemUserId

END
GO
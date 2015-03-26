IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('SystemUser_DissociateFromEmailAddress') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_DissociateFromEmailAddress
	END
GO

CREATE PROCEDURE dbo.SystemUser_DissociateFromEmailAddress
	@SystemUserId			INT,
	@EmailAddressId			INT		
AS
BEGIN
	SET NOCOUNT ON

	DELETE
	FROM	dbo.SystemUser_EmailAddress
	WHERE	SystemUserId = @SystemUserId
			AND EmailAddressId = @EmailAddressId
END
GO
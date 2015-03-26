IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('SystemUser_AssociateToEmailAddress') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_AssociateToEmailAddress
	END
GO

CREATE PROCEDURE dbo.SystemUser_AssociateToEmailAddress
	@SystemUserId			INT,
	@EmailAddressId			INT,		
	@EmailAddressOrder		INT
	
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS (SELECT 1 FROM dbo.SystemUser_EmailAddress WHERE SystemUserId=@SystemUserId AND EmailAddressOrder=@EmailAddressOrder)
		BEGIN

				DELETE
				FROM	dbo.SystemUser_EmailAddress
				WHERE	SystemUserId = @SystemUserId AND EmailAddressOrder=@EmailAddressOrder
		END
				
		INSERT INTO dbo.SystemUser_EmailAddress
		(
			SystemUserId,
			EmailAddressId,
			EmailAddressOrder
		)
		VALUES
		(
			@SystemUserId,
			@EmailAddressId,
			@EmailAddressOrder
		)
							
END
GO
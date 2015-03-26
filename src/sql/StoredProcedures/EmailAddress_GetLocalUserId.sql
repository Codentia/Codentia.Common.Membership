IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.EmailAddress_GetSystemUserId') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.EmailAddress_GetSystemUserId
	END
GO

CREATE PROCEDURE dbo.EmailAddress_GetSystemUserId
	@EmailAddressId		INT,
	@SystemUserId		INT OUTPUT
			
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT @SystemUserId=SystemUserId
	FROM dbo.SystemUser_EmailAddress
	WHERE EmailAddressId=@EmailAddressId
	
	
END
GO
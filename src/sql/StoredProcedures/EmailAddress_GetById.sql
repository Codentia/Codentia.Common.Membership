IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.EmailAddress_GetById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.EmailAddress_GetById
	END
GO

CREATE PROCEDURE dbo.EmailAddress_GetById
	@EmailAddressId		INT		
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT lue.EmailAddressId, EmailAddress, SystemUserId, EmailAddressOrder, IsConfirmed, ConfirmGuid
	FROM dbo.SystemUser_EmailAddress lue
	INNER JOIN dbo.EmailAddress e
			ON e.EmailAddressId=lue.EmailAddressId
	WHERE e.EmailAddressId=@EmailAddressId
	UNION ALL
	SELECT EmailAddressId, EmailAddress, NULL, NULL, IsConfirmed, ConfirmGuid
	FROM dbo.EmailAddress e
	WHERE EmailAddressId=@EmailAddressId AND
	NOT EXISTS (SELECT 1 FROM dbo.SystemUser_EmailAddress WHERE EmailAddressId=e.EmailAddressId)
	 
		
END
GO
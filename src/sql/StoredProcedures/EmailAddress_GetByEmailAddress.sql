IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.EmailAddress_GetByAddress') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.EmailAddress_GetByAddress
	END
GO

CREATE PROCEDURE dbo.EmailAddress_GetByAddress
	@EmailAddress		NVARCHAR(255)		
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT lue.EmailAddressId, EmailAddress, SystemUserId, EmailAddressOrder, IsConfirmed, ConfirmGuid
	FROM dbo.SystemUser_EmailAddress lue
	INNER JOIN dbo.EmailAddress e
			ON e.EmailAddressId=lue.EmailAddressId
	WHERE e.EmailAddress=@EmailAddress			
	UNION ALL
	SELECT EmailAddressId, EmailAddress, NULL, NULL, IsConfirmed, ConfirmGuid
	FROM dbo.EmailAddress e
	WHERE EmailAddress=@EmailAddress AND
	NOT EXISTS (SELECT 1 FROM dbo.SystemUser_EmailAddress WHERE EmailAddressId=e.EmailAddressId)
	 
		
END
GO
IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.EmailAddress_GetByCookie') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.EmailAddress_GetByCookie
	END
GO

CREATE PROCEDURE dbo.EmailAddress_GetByCookie
	@Cookie		UNIQUEIDENTIFIER	
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT lue.EmailAddressId, EmailAddress, SystemUserId, EmailAddressOrder, IsConfirmed, ConfirmGuid
	FROM dbo.SystemUser_EmailAddress lue
	INNER JOIN dbo.EmailAddress e
			ON e.EmailAddressId=lue.EmailAddressId
	WHERE e.ConfirmGuid=@Cookie			
	UNION ALL
	SELECT EmailAddressId, EmailAddress, NULL, NULL, IsConfirmed, ConfirmGuid
	FROM dbo.EmailAddress e
	WHERE ConfirmGuid=@Cookie AND
	NOT EXISTS (SELECT 1 FROM dbo.SystemUser_EmailAddress WHERE EmailAddressId=e.EmailAddressId)
	 
		
END
GO
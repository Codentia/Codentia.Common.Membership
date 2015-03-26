IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.SystemUser_GetEmailAddresses') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_GetEmailAddresses
	END
GO

CREATE PROCEDURE dbo.SystemUser_GetEmailAddresses
	@SystemUserId			INT
AS
BEGIN
	SET NOCOUNT ON

	SELECT le.EmailAddressId, EmailAddress, SystemUserId, EmailAddressOrder, IsConfirmed, ConfirmGuid
	FROM	dbo.SystemUser_EmailAddress le
	INNER JOIN dbo.EmailAddress e
			ON e.EmailAddressId=le.EmailAddressId
	WHERE	SystemUserId = @SystemUserId
	ORDER BY EmailAddressOrder
END
GO
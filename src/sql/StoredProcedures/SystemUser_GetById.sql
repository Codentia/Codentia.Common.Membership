IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.SystemUser_GetById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_GetById
	END
GO

CREATE PROCEDURE dbo.SystemUser_GetById
	@SystemUserId			INT
AS
BEGIN
	SET NOCOUNT ON

	SELECT	SystemUserId,
			FirstName,
			Surname,
			UserId,
			ForcePassword,
			HasNewsLetter,
			'' PhoneNumber
	FROM	dbo.SystemUser
	WHERE PhoneNumberId IS NULL AND SystemUserId = @SystemUserId
	UNION ALL
	SELECT	SystemUserId,
			FirstName,
			Surname,
			UserId,
			ForcePassword,
			HasNewsLetter,
			PhoneNumber
	FROM	dbo.SystemUser SU
	INNER JOIN dbo.PhoneNumber PN
		    ON PN.PhoneNumberId = SU.PhoneNumberId
	WHERE	SystemUserId = @SystemUserId

	EXEC dbo.SystemUser_GetEmailAddresses
						@SystemUserId	= @SystemUserId

END
GO
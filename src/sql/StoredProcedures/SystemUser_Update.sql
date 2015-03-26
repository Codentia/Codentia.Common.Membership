IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.SystemUser_Update') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_Update
	END
GO

CREATE PROCEDURE dbo.SystemUser_Update
	@SystemUserId			INT,
	@FirstName				NVARCHAR(100),
	@Surname				NVARCHAR(100),	
	@HasNewsLetter			BIT,
	@PhoneNumberId			INT
AS
BEGIN
	SET NOCOUNT ON

	UPDATE	dbo.SystemUser
	SET		FirstName				= @FirstName,
			Surname					= @Surname,
			HasNewsLetter			= @HasNewsLetter,
			PhoneNumberId			= @PhoneNumberId
	WHERE	SystemUserId = @SystemUserId
END
GO
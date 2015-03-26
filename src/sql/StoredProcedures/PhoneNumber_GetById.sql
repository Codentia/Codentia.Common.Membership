IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.PhoneNumber_GetById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.PhoneNumber_GetById
	END
GO

CREATE PROCEDURE dbo.PhoneNumber_GetById
	@PhoneNumberId		INT,
	@PhoneNumber		NVARCHAR(11) OUTPUT
AS
BEGIN
	SET NOCOUNT ON

	SELECT @PhoneNumber = PhoneNumber FROM dbo.PhoneNumber WHERE PhoneNumberId = @PhoneNumberId
END
GO 
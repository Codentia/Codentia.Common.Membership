IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.PhoneNumber_Create') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.PhoneNumber_Create
	END
GO

CREATE PROCEDURE dbo.PhoneNumber_Create
	@PhoneNumber		NVARCHAR(15),
	@PhoneNumberId		INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON

	SELECT	@PhoneNumberId = PhoneNumberId FROM dbo.PhoneNumber WHERE PhoneNumber = @PhoneNumber
	
	IF @PhoneNumberId IS NULL
		BEGIN
			INSERT INTO dbo.PhoneNumber
			(
				PhoneNumber
			)
			VALUES
			(
				@PhoneNumber
			)
			
			SET @PhoneNumberId = SCOPE_IDENTITY()		
		END
END
GO
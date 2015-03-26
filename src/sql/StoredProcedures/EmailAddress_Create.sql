IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.EmailAddress_Create') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.EmailAddress_Create
	END
GO

CREATE PROCEDURE dbo.EmailAddress_Create
	@EmailAddress	NVARCHAR(256),	
	@EmailAddressId		INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
			INSERT INTO dbo.EmailAddress
			(
				EmailAddress				
			)
			VALUES
			(
				@EmailAddress				
			)		
			
			SET @EmailAddressId = SCOPE_IDENTITY()
END
GO
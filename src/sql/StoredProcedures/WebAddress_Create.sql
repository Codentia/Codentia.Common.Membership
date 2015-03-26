IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.WebAddress_Create') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.WebAddress_Create
	END
GO

CREATE PROCEDURE dbo.WebAddress_Create	
	@URL				NVARCHAR(300),
	@WebAddressId		INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
				
			INSERT INTO dbo.WebAddress
			(
				URL				
			)
			VALUES
			(
				@URL				
			)		
			
			SET @WebAddressId = SCOPE_IDENTITY()				
			
END
GO
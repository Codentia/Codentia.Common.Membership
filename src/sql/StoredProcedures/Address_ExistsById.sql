IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Address_ExistsById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Address_ExistsById
	END
GO

CREATE PROCEDURE dbo.Address_ExistsById
	@AddressId			INT,
	@Exists				BIT OUTPUT
AS

BEGIN
	SET NOCOUNT ON
	
	IF EXISTS ( SELECT 1 FROM dbo.Address WHERE AddressId = @AddressId )
		BEGIN
			SET @Exists = 1
		END
	ELSE
		BEGIN
			SET @Exists = 0
		END			
END
GO 
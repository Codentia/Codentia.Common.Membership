 IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.WebAddress_ExistsById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.WebAddress_ExistsById
	END
GO

CREATE PROCEDURE dbo.WebAddress_ExistsById
	@WebAddressId			INT,
	@Exists					BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
		
		IF EXISTS ( SELECT * FROM dbo.WebAddress WHERE WebAddressId = @WebAddressId)
			BEGIN
				SET @Exists = 1
			END
		ELSE
			BEGIN
				SET @Exists = 0
			END
		
				
END
GO
 IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.EmailAddress_ExistsByAddress') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.EmailAddress_ExistsByAddress
	END
GO

CREATE PROCEDURE dbo.EmailAddress_ExistsByAddress
	@EmailAddress			NVARCHAR(255),
	@Exists					BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON	
		
		IF EXISTS ( SELECT * FROM dbo.EmailAddress WHERE EmailAddress = @EmailAddress)
			BEGIN
				SET @Exists = 1
			END
		ELSE
			BEGIN
				SET @Exists = 0
			END
		
				
END
GO
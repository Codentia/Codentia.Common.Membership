 IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.WebAddress_ExistsByURL') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.WebAddress_ExistsByURL
	END
GO

CREATE PROCEDURE dbo.WebAddress_ExistsByURL
	@URL				NVARCHAR(300),
	@Exists					BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
		
		IF EXISTS ( SELECT * FROM dbo.WebAddress WHERE URL = @URL)
			BEGIN
				SET @Exists = 1
			END
		ELSE
			BEGIN
				SET @Exists = 0
			END
		
				
END
GO
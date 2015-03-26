IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Country_ExistsByDisplayText') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Country_ExistsByDisplayText
	END
GO

CREATE PROCEDURE dbo.Country_ExistsByDisplayText
	@DisplayText	NVARCHAR(100),
	@Exists			BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON 

	IF EXISTS ( SELECT 1 FROM dbo.Country WHERE DisplayText = @DisplayText )
		BEGIN
			SET @Exists = 1
		END
	ELSE
		BEGIN
			SET @Exists = 0
		END		
END
GO  
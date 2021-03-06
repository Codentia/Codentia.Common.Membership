IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Country_ExistsById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Country_ExistsById
	END
GO

CREATE PROCEDURE dbo.Country_ExistsById
	@CountryId			INT,
	@Exists				BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON 

	IF EXISTS ( SELECT 1 FROM dbo.Country WHERE CountryId = @CountryId )
		BEGIN
			SET @Exists = 1
		END
	ELSE
		BEGIN
			SET @Exists = 0
		END
END
GO
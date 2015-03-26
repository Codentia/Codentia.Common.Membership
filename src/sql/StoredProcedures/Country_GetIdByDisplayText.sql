IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Country_GetIdByDisplayText') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Country_GetIdByDisplayText
	END
GO

CREATE PROCEDURE dbo.Country_GetIdByDisplayText
	@DisplayText		NVARCHAR(100),
	@CountryId			INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON 

	SELECT	@CountryId = CountryId 
	FROM	dbo.Country 
	WHERE	DisplayText = @DisplayText

	IF @CountryId IS NULL
		BEGIN
			SET @CountryId = 0
		END
END
GO
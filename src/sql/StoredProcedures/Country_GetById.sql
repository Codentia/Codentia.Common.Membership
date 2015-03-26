IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Country_GetById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Country_GetById
	END
GO

CREATE PROCEDURE dbo.Country_GetById
	@CountryId			INT
AS
BEGIN
	SET NOCOUNT ON 

	SELECT	@CountryId,
			DisplayText
	FROM	dbo.Country
	WHERE	CountryId = @CountryId
END
GO
IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Country_GetAll') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Country_GetAll
	END
GO

CREATE PROCEDURE dbo.Country_GetAll
AS
BEGIN
	SET NOCOUNT ON 

		SELECT	CountryId,
				DisplayText
		FROM	dbo.Country		
		ORDER BY DisplayText
	
END
GO
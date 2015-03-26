IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.WebAddress_GetById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.WebAddress_GetById
	END
GO

CREATE PROCEDURE dbo.WebAddress_GetById
	@WebAddressId		INT		
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT WebAddressId, URL, IsDead
	FROM dbo.WebAddress
	WHERE WebAddressId=@WebAddressId
		
END
GO
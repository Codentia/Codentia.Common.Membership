IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.WebAddress_GetByURL') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.WebAddress_GetByURL
	END
GO

CREATE PROCEDURE dbo.WebAddress_GetByURL
	@URL		NVARCHAR(300)		
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT WebAddressId, URL, IsDead
	FROM dbo.WebAddress
	WHERE URL=@URL
		
END
GO
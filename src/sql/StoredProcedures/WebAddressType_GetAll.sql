IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.WebAddressType_GetAll') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.WebAddressType_GetAll
	END
GO

CREATE PROCEDURE dbo.WebAddressType_GetAll
AS
BEGIN
	SET NOCOUNT ON 

		SELECT	WebAddressTypeId,
				DisplayText
		FROM	dbo.WebAddressType		
		ORDER BY DisplayText
	
END
GO
IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.WebAddress_UpdateAsDead') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.WebAddress_UpdateAsDead
	END
GO

CREATE PROCEDURE dbo.WebAddress_UpdateAsDead	
	@WebAddressId		INT	
AS
BEGIN
	SET NOCOUNT ON
				
	UPDATE dbo.WebAddress SET IsDead=1 WHERE WebAddressId=@WebAddressId						
			
END
GO
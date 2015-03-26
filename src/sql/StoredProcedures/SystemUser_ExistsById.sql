IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.SystemUser_ExistsById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_ExistsById
	END
GO

CREATE PROCEDURE dbo.SystemUser_ExistsById
	@SystemUserId			INT,
	@Exists					BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS ( SELECT 1 FROM dbo.SystemUser WHERE SystemUserId = @SystemUserId )
		BEGIN
			SET @Exists = 1
		END
	ELSE
		BEGIN
			SET @Exists = 0
		END
END
GO
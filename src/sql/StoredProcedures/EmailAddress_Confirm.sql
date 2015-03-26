IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.EmailAddress_Confirm') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.EmailAddress_Confirm
	END
GO

CREATE PROCEDURE dbo.EmailAddress_Confirm
	@EmailAddress	NVARCHAR(255),	
	@ConfirmGuid	UNIQUEIDENTIFIER,
	@IsConfirmed	BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
SET @IsConfirmed=0	
	
	IF EXISTS (SELECT 1 FROM dbo.EmailAddress WHERE ConfirmGuid=@ConfirmGuid AND EmailAddress=@EmailAddress)
		BEGIN
			UPDATE dbo.EmailAddress 
			SET IsConfirmed=1
			WHERE ConfirmGuid=@ConfirmGuid AND EmailAddress=@EmailAddress
			
			SET @IsConfirmed=1
			
		END			
END
GO
IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Address_ExistsByEmailAndAddressIds') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Address_ExistsByEmailAndAddressIds
	END
GO

CREATE PROCEDURE dbo.Address_ExistsByEmailAndAddressIds
	@AddressCookie		UNIQUEIDENTIFIER,
	@EmailAddressCookie	UNIQUEIDENTIFIER,
	@Exists				BIT OUTPUT
AS

BEGIN
	SET NOCOUNT ON
	
	IF EXISTS ( SELECT 1 FROM dbo.Address A WHERE Cookie = @AddressCookie 
				AND EXISTS (SELECT 1 FROM dbo.EmailAddress
							WHERE ConfirmGuid=@EmailAddressCookie AND EmailAddressId=A.EmailAddressId))
		BEGIN
			SET @Exists = 1
		END
	ELSE
		BEGIN
			SET @Exists = 0
		END			
END
GO 
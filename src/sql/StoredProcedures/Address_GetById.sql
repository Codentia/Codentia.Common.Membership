IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Address_GetById') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Address_GetById
	END
GO

CREATE PROCEDURE dbo.Address_GetById
	@AddressId			INT,
	@AssemblyVersion	NVARCHAR(10)='1.1.1'
AS
BEGIN
	SET NOCOUNT ON

	IF @AssemblyVersion = '1.1.1'
		BEGIN
			SELECT	AddressId,	
					FirstName AS Recipient,
					HouseName,
					Street,
					Town,
					City,
					County,
					Postcode,
					CountryId,
					Cookie,
					EmailAddressId
			FROM	dbo.Address
			WHERE AddressId=@AddressId
		END
	ELSE
		BEGIN
			SELECT	AddressId,	
					Title,
					FirstName,
					LastName,
					HouseName,
					Street,
					Town,
					City,
					County,
					Postcode,
					CountryId,
					Cookie,
					EmailAddressId
			FROM	dbo.Address
			WHERE AddressId=@AddressId
		END
END
GO
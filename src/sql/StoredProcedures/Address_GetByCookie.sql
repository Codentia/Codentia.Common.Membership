IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Address_GetByCookie') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Address_GetByCookie
	END
GO

CREATE PROCEDURE dbo.Address_GetByCookie
	@Cookie			UNIQUEIDENTIFIER,
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
			WHERE Cookie=@Cookie
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
			WHERE Cookie=@Cookie
		END
END
GO
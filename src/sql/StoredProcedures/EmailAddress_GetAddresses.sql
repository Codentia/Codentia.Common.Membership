IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.EmailAddress_GetAddresses') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.EmailAddress_GetAddresses
	END
GO

CREATE PROCEDURE dbo.EmailAddress_GetAddresses
	@EmailAddressId		INT,
	@AssemblyVersion	NVARCHAR(10)='1.1.1'
AS
BEGIN
	SET NOCOUNT ON
	
	IF @AssemblyVersion = '1.1.1'
		BEGIN
			SELECT  A.AddressId,	
					A.FirstName AS Recipient,
					A.HouseName,
					A.Street,
					A.Town,
					A.City,
					A.County,
					A.Postcode,
					A.CountryId,
					A.Cookie,
					@EmailAddressId EmailAddressId
			FROM	dbo.Address A
			WHERE	EmailAddressId = @EmailAddressId
		END
	ELSE	 
		BEGIN
			SELECT  A.AddressId,	
					A.Title,
					A.FirstName,
					A.LastName,
					A.HouseName,
					A.Street,
					A.Town,
					A.City,
					A.County,
					A.Postcode,
					A.CountryId,
					A.Cookie,
					@EmailAddressId EmailAddressId
			FROM	dbo.Address A
			WHERE	EmailAddressId = @EmailAddressId
		END
END
GO
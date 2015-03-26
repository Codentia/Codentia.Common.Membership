IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Address_Update') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Address_Update
	END
GO

CREATE PROCEDURE dbo.Address_Update
	@Recipient				NVARCHAR(150)=NULL,
	@Title					NVARCHAR(25)=NULL,
	@FirstName				NVARCHAR(150)=NULL,
	@LastName				NVARCHAR(150)=NULL,
	@HouseName				NVARCHAR(100),
	@Street					NVARCHAR(100),
	@Town					NVARCHAR(100),
	@City					NVARCHAR(100),
	@County					NVARCHAR(100),
	@Postcode				NVARCHAR(15),
	@CountryId				INT,
	@AddressId				INT,
	@AssemblyVersion		NVARCHAR(10)='1.1.1'
AS
BEGIN
	SET NOCOUNT ON

	IF @AssemblyVersion = '1.1.1'
		BEGIN
			UPDATE	dbo.Address
			SET		FirstName = @Recipient,
					HouseName = @HouseName,
					Street = @Street,
					Town = @Town,
					City = @City,
					County = @County,
					Postcode = @Postcode,
					CountryId = @CountryId
			WHERE	@AddressId = AddressId
		END
	ELSE
		BEGIN
			UPDATE	dbo.Address
			SET		Title = @Title,
					FirstName = @FirstName,
					LastName = @LastName,
					HouseName = @HouseName,
					Street = @Street,
					Town = @Town,
					City = @City,
					County = @County,
					Postcode = @Postcode,
					CountryId = @CountryId
			WHERE	@AddressId = AddressId
		END
END
GO
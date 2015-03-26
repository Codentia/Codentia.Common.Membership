IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.Address_Create') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.Address_Create
	END
GO

CREATE PROCEDURE dbo.Address_Create
	@Recipient			NVARCHAR(150)=NULL,
	@Title				NVARCHAR(25)=NULL,
	@FirstName			NVARCHAR(150)=NULL,
	@LastName			NVARCHAR(150)=NULL,
	@HouseName			NVARCHAR(100),
	@Street				NVARCHAR(100),
	@Town				NVARCHAR(100),
	@City				NVARCHAR(100),
	@County				NVARCHAR(100),
	@Postcode			NVARCHAR(15),
	@CountryId			INT,
	@EmailAddressId		INT,
	@AssemblyVersion	NVARCHAR(10)='1.1.1',
	@AddressId			INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
	IF @AssemblyVersion = '1.1.1'
		BEGIN
			IF (@Recipient<>'' AND @HouseName<>'' AND @Street<>'')
	
				BEGIN

					SELECT	@AddressId = AddressId
					FROM	dbo.Address
					WHERE	FirstName = @Recipient
							AND HouseName = @HouseName
							AND Street = @Street
							AND Town = @Town
							AND City = @City
							AND County = @County
							AND Postcode = @Postcode
							AND CountryId = @CountryId
							AND EmailAddressId=@EmailAddressId
				END
			
			IF @AddressId IS NULL
				BEGIN
					INSERT INTO dbo.Address
					(
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
						EmailAddressId
					)
					VALUES
					(
						NULL,
						@Recipient,
						NULL,
						@HouseName,
						@Street,
						@Town,
						@City,
						@County,
						@Postcode,				
						@CountryId,
						@EmailAddressId
					)		
			
					SET @AddressId = SCOPE_IDENTITY()
				END
		END
	ELSE
		BEGIN
			IF (@Recipient<>'' AND @HouseName<>'' AND @Street<>'')
	
				BEGIN

					SELECT	@AddressId = AddressId
					FROM	dbo.Address
					WHERE	Title = @Title
							AND FirstName = @FirstName
							AND LastName = @LastName
							AND HouseName = @HouseName
							AND Street = @Street
							AND Town = @Town
							AND City = @City
							AND County = @County
							AND Postcode = @Postcode
							AND CountryId = @CountryId
							AND EmailAddressId=@EmailAddressId
				END
			
			IF @AddressId IS NULL
				BEGIN
					INSERT INTO dbo.Address
					(
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
						EmailAddressId
					)
					VALUES
					(
						@Title,
						@FirstName,
						@LastName,
						@HouseName,
						@Street,
						@Town,
						@City,
						@County,
						@Postcode,				
						@CountryId,
						@EmailAddressId
					)		
			
					SET @AddressId = SCOPE_IDENTITY()
				END
		END

END
GO
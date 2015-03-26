CREATE TABLE dbo.Address
(
	AddressId		INT IDENTITY(1,1),
	Title			NVARCHAR(25),
	FirstName		NVARCHAR(150),
	LastName		NVARCHAR(150),
	HouseName		NVARCHAR(100) NOT NULL,
	Street			NVARCHAR(100) NOT NULL,
	Town			NVARCHAR(100),
	City			NVARCHAR(100) NOT NULL,
	County			NVARCHAR(100) NOT NULL,
	Postcode		NVARCHAR(15) NOT NULL,
	CountryId		INT NOT NULL,
	Region			NVARCHAR(50),
	Cookie			UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	EmailAddressId	INT NOT NULL
) 
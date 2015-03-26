CREATE TABLE dbo.SystemUser
(
	SystemUserId			INT IDENTITY(1,1) NOT NULL,	
	FirstName				NVARCHAR(100),
	Surname					NVARCHAR(100),
	UserId					UNIQUEIDENTIFIER,
	ForcePassword			BIT NOT NULL DEFAULT 1,
	HasNewsLetter			BIT NOT NULL,
	PhoneNumberId			INT NULL,	
	InsertDateTime			DATETIME NOT NULL DEFAULT GETDATE()
)
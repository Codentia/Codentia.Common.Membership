CREATE TABLE dbo.SystemUser_EmailAddress
(
	SystemUserEmailAddressId		INT IDENTITY(1,1),
	SystemUserId					INT NOT NULL,
	EmailAddressId				INT NOT NULL,
	EmailAddressOrder			INT	NOT NULL
) 
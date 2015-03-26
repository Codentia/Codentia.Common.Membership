ALTER TABLE dbo.Address ADD CONSTRAINT FK_Address_EmailAddressId FOREIGN KEY (EmailAddressId) REFERENCES dbo.EmailAddress (EmailAddressId)
GO
 
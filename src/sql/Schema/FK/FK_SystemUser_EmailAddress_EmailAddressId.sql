ALTER TABLE dbo.SystemUser_EmailAddress ADD CONSTRAINT FK_SystemUserEmailAddress_EmailAddressId FOREIGN KEY (EmailAddressId) REFERENCES dbo.EmailAddress (EmailAddressId)
GO
 
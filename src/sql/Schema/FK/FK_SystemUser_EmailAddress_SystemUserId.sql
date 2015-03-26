ALTER TABLE dbo.SystemUser_EmailAddress ADD CONSTRAINT FK_SystemUser_EmailAddress_SystemUserId FOREIGN KEY (SystemUserId) REFERENCES dbo.SystemUser (SystemUserId)
GO
 
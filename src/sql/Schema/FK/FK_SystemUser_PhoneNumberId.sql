ALTER TABLE dbo.SystemUser ADD CONSTRAINT FK_SystemUser_PhoneNumberId FOREIGN KEY (PhoneNumberId) REFERENCES dbo.PhoneNumber (PhoneNumberId)
GO
 
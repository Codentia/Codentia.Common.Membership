ALTER TABLE dbo.Address ADD CONSTRAINT FK_Address_CountryId FOREIGN KEY (CountryId) REFERENCES dbo.Country (CountryId)
GO
 
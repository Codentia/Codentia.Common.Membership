using Codentia.Test;

namespace Codentia.Common.Membership.Test.Queries
{
    /// <summary>
    /// AddressData Queries
    /// </summary>
    [CoverageExclude]
    public static class AddressDataQueries
    {
        /// <summary>
        /// SELECT COUNT(AddressId) FROM Address
        /// </summary>
        public const string Address_Count = "SELECT COUNT(AddressId) FROM Address";

        /// <summary>
        /// INSERT INTO dbo.Address (Recipient, HouseName, Street, Town, City, County, Postcode, CountryId, EmailAddressId) VALUES ('','','','','','','', {0}, {1})
        /// </summary>
        public const string Address_CountryOnly_Insert = "INSERT INTO dbo.Address (Title, FirstName, LastName, HouseName, Street, Town, City, County, Postcode, CountryId, EmailAddressId) VALUES ('','','','','','','','','', {0}, {1})";

        /// <summary>
        /// SELECT COUNT(*) FROM Address A  WHERE CountryId NOT IN ( SELECT CountryId FROM Country )
        /// </summary>
        public const string Address_Get_AddressWithoutExistingCountries = "SELECT COUNT(*) FROM Address A  WHERE CountryId NOT IN ( SELECT CountryId FROM Country )";

        /// <summary>
        /// SELECT TOP 1 AddressId, Recipient, HouseName, Street, Town, City, County, Postcode, CountryId, Cookie,  EmailAddressId FROM Address WHERE Cookie IS NOT NULL ORDER BY NEWID()
        /// </summary>
        public const string Address_Get_Random_NotNullCookie = "SELECT TOP 1 AddressId, Title, FirstName, LastName, HouseName, Street, Town, City, County, Postcode, CountryId, Cookie,  EmailAddressId FROM Address WHERE Cookie IS NOT NULL ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 10 AddressId, Recipient, HouseName, Street, Town, City, County, Postcode, CountryId, Region, Cookie,  EmailAddressId FROM Address ORDER BY NEWID()
        /// </summary>
        public const string Address_Get_Random10 = "SELECT TOP 10 AddressId, Title, FirstName, LastName, HouseName, Street, Town, City, County, Postcode, CountryId, Region, Cookie,  EmailAddressId FROM Address ORDER BY NEWID()";

        /// <summary>
        /// INSERT INTO dbo.Address (Recipient, HouseName, Street, Town, City, County, Postcode, CountryId, EmailAddressId) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}', {7}, {8})
        /// </summary>
        public const string Address_Insert = "INSERT INTO dbo.Address (Title, FirstName, LastName, HouseName, Street, Town, City, County, Postcode, CountryId, EmailAddressId) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}', {9}, {10})";

        /// <summary>
        /// SELECT AddressId, Recipient, HouseName, Street, Town, City, County, Postcode, CountryId, Cookie,  EmailAddressId FROM Address WHERE AddressId={0}
        /// </summary>
        public const string Address_Select_ByAddressId = "SELECT AddressId, Title, FirstName, LastName, HouseName, Street, Town, City, County, Postcode, CountryId, Cookie,  EmailAddressId FROM Address WHERE AddressId={0}";

        /// <summary>
        /// SELECT  A.AddressId,  A.Recipient, A.HouseName, A.Street, A.Town, A.City, A.County, A.Postcode, A.CountryId, A.Cookie,  EmailAddressId FROM dbo.Address A WHERE EmailAddressId = {0}
        /// </summary>
        public const string Addresses_GetForEmailAddress = "SELECT  A.AddressId,  A.Title, A.FirstName, A.LastName, A.HouseName, A.Street, A.Town, A.City, A.County, A.Postcode, A.CountryId, A.Cookie,  EmailAddressId FROM dbo.Address A WHERE EmailAddressId = {0}";

        /// <summary>
        /// SELECT Cookie FROM Address WHERE EmailAddressId IN (SELECT EmailAddressId FROM EmailAddress WHERE ConfirmGuid='{0}')
        /// </summary>
        public const string AddressGuidForEmailAddressGuid = "SELECT Cookie FROM Address WHERE EmailAddressId IN (SELECT EmailAddressId FROM EmailAddress WHERE ConfirmGuid='{0}')";

        /// <summary>
        /// SELECT AddressId, CountryId FROM Address WHERE Recipient='' AND HouseName='' AND Street='' ORDER BY NEWID()
        /// </summary>
        public const string AddressId_Get_CountryOnlyAddress = "SELECT AddressId, CountryId FROM Address WHERE FirstName='' AND HouseName='' AND Street='' ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 AddressId FROM Address WHERE CountryId={0} ORDER BY NEWID()
        /// </summary>
        public const string AddressId_Get_ForCountry = "SELECT TOP 1 AddressId FROM Address WHERE CountryId={0} ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 AddressId FROM Address WHERE EmailAddressId={0} ORDER BY NEWID()
        /// </summary>
        public const string AddressId_Get_ForEmailAddressId = "SELECT TOP 1 AddressId FROM Address WHERE EmailAddressId={0} ORDER BY NEWID()";

        /// <summary>
        /// SELECT AddressId FROM Address WHERE Recipient!='' AND HouseName!='' AND Street!='' ORDER BY NEWID()
        /// </summary>
        public const string AddressId_Get_NonCountryOnlyAddress = "SELECT AddressId FROM Address WHERE FirstName!='' AND HouseName!='' AND Street!='' ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 AddressId FROM Address WHERE EmailAddressId!={0} ORDER BY NEWID()
        /// </summary>
        public const string AddressId_Get_NotForEmailAddressId = "SELECT TOP 1 AddressId FROM Address WHERE EmailAddressId!={0} ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 AddressId FROM Address WHERE ISNULL(Town, '')='' AND ISNULL(recipient, '') != '' ORDER BY NEWID()
        /// </summary>
        public const string AddressId_Get_NoTown_Random = "SELECT TOP 1 AddressId FROM Address WHERE ISNULL(Town, '')='' AND ISNULL(FirstName, '') != '' ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 AddressId FROM Address WHERE ISNULL(recipient, '') != '' ORDER BY NEWID()
        /// </summary>
        public const string AddressId_Get_WithRecipient_Random = "SELECT TOP 1 AddressId FROM Address WHERE ISNULL(FirstName, '') != '' ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 AddressId FROM Address WHERE ISNULL(Town, '')!='' AND ISNULL(recipient, '') != '' ORDER BY NEWID()
        /// </summary>
        public const string AddressId_Get_WithTown_Random = "SELECT TOP 1 AddressId FROM Address WHERE ISNULL(Town, '')!='' AND ISNULL(FirstName, '') != '' ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 AddressId FROM Address WHERE CountryId={0} ORDER BY NEWID()
        /// </summary>
        public const string AddressId_GetRandomByCountryId = "SELECT TOP 1 AddressId FROM Address WHERE CountryId={0} ORDER BY NEWID()";

        /// <summary>
        /// SELECT ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM Address WHERE Cookie='{0}')
        /// </summary>
        public const string ConfirmGuid_Get_ForAddressCookie = "SELECT ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM Address WHERE Cookie='{0}')";

        /// <summary>
        /// SELECT ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM Address WHERE Cookie!='{0}')
        /// </summary>
        public const string ConfirmGuid_Get_NotForAddressCookie = "SELECT ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM Address WHERE Cookie!='{0}')";

        /// <summary>
        /// SELECT TOP 1 Cookie FROM Address ORDER BY NEWID()
        /// </summary>
        public const string Cookie_Get_Random = "SELECT TOP 1 Cookie FROM Address ORDER BY NEWID()";

        /// <summary>
        /// SELECT EmailAddressId FROM Address WHERE AddressId={0}
        /// </summary>
        public const string EmailAddressId_Get_ForAddress = "SELECT EmailAddressId FROM Address WHERE AddressId={0}";

        /// <summary>
        /// SELECT EmailAddressId FROM Address ORDER BY NEWID()
        /// </summary>
        public const string EmailAddressId_Get_RandomFromAddress = "SELECT EmailAddressId FROM Address ORDER BY NEWID()";
    }
}

using Codentia.Test;

namespace Codentia.Common.Membership.Test.Queries
{
    /// <summary>
    /// ContactData Queries
    /// </summary>
    [CoverageExclude]
    public static class ContactDataQueries
    {
        /// <summary>
        /// SELECT ConfirmGuid FROM EmailAddress ORDER BY NEWID()
        /// </summary>
        public const string ConfirmGuid_Get_Random = "SELECT ConfirmGuid FROM EmailAddress ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress) ORDER BY NEWID()
        /// </summary>
        public const string ConfirmGuid_InSystemUser_EmailAddress_Get_Random = "SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress) ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress WHERE SystemUserId IN (SELECT SystemUserId FROM SystemUser_Address)) ORDER BY NEWID()
        /// </summary>
        public const string ConfirmGuid_InSystemUser_EmailAddressAndAddress_Get_Random = "SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress WHERE SystemUserId IN (SELECT SystemUserId FROM SystemUser_Address)) ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress WHERE  SystemUserId IN (SELECT SystemUserId FROM ApplicationUser)) ORDER BY NEWID()
        /// </summary>
        public const string ConfirmGuid_InSystemUser_EmailAddressAndAppUser_Get_Random = "SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress WHERE  SystemUserId IN (SELECT SystemUserId FROM ApplicationUser)) ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddressId NOT IN (SELECT EmailAddressId FROM SystemUser_EmailAddress) ORDER BY NEWID()
        /// </summary>
        public const string ConfirmGuid_NotInSystemUser_EmailAddress_Get_Random = "SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddressId NOT IN (SELECT EmailAddressId FROM SystemUser_EmailAddress) ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddress!='{0}'
        /// </summary>
        public const string ConfirmGuid_Select = "SELECT TOP 1 ConfirmGuid FROM EmailAddress WHERE EmailAddress!='{0}'";

        /// <summary>
        /// SELECT TOP 1 EA.ConfirmGuid, SE.SystemUserId FROM EmailAddress EA INNER JOIN SystemUser_EmailAddress SE ON SE.EmailAddressId = EA.EmailAddressId WHERE SE.SystemUserId={0}
        /// </summary>
        public const string ConfirmGuid_SelectBySystemUserId = "SELECT TOP 1 EA.ConfirmGuid, SE.SystemUserId FROM EmailAddress EA INNER JOIN SystemUser_EmailAddress SE ON SE.EmailAddressId = EA.EmailAddressId WHERE SE.SystemUserId={0}";

        /// <summary>
        /// SELECT TOP 1 EA.ConfirmGuid, SE.SystemUserId FROM EmailAddress EA INNER JOIN SystemUser_EmailAddress SE ON SE.EmailAddressId = EA.EmailAddressId ORDER BY NEWID()
        /// </summary>
        public const string ConfirmGuid_SystemUserId_InSystemUser_EmailAddress_Get_Random = "SELECT TOP 1 EA.ConfirmGuid, SE.SystemUserId FROM EmailAddress EA INNER JOIN SystemUser_EmailAddress SE ON SE.EmailAddressId = EA.EmailAddressId ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 EmailAddressId, EmailAddress,  IsConfirmed, ConfirmGuid FROM EmailAddress ORDER BY NEWID()
        /// </summary>
        public const string EmailAddress_All_Get_Random = "SELECT TOP 1 EmailAddressId, EmailAddress,  IsConfirmed, ConfirmGuid FROM EmailAddress ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 EmailAddress FROM EmailAddress ORDER BY NEWID()
        /// </summary>
        public const string EmailAddress_Get_Random = "SELECT TOP 1 EmailAddress FROM EmailAddress ORDER BY NEWID()";

        /// <summary>
        /// INSERT INTO EmailAddress (EmailAddress) VALUES('{0}')
        /// </summary>
        public const string EmailAddress_Insert = "INSERT INTO EmailAddress (EmailAddress) VALUES('{0}')";

        /// <summary>
        /// SELECT EmailAddress FROM EmailAddress WHERE EmailAddressid={0}
        /// </summary>
        public const string EmailAddress_Select = "SELECT EmailAddress FROM EmailAddress WHERE EmailAddressid={0}";

        /// <summary>
        /// SELECT lem.EmailAddressId, EmailAddress, EmailAddressOrder FROM SystemUser_EmailAddress lem INNER JOIN EmailAddress e ON e.EmailAddressId=lem.EmailAddressId WHERE SystemUserId={0}
        /// </summary>
        public const string EmailAddress_Select_AssociatedToSystemUser = "SELECT lem.EmailAddressId, EmailAddress, SystemUserId, EmailAddressOrder, IsConfirmed, ConfirmGuid FROM SystemUser_EmailAddress lem INNER JOIN EmailAddress e ON e.EmailAddressId=lem.EmailAddressId WHERE SystemUserId={0}";

        /// <summary>
        /// SELECT ConfirmGuid FROM EmailAddress WHERE EmailAddressid={0}
        /// </summary>
        public const string EmailAddress_SelectConfirmGuid_ById = "SELECT ConfirmGuid FROM EmailAddress WHERE EmailAddressid={0}";

        /// <summary>
        /// SELECT TOP 1 EmailAddress, ConfirmGuid FROM EmailAddress WHERE IsConfirmed=0 ORDER BY NEWID()
        /// </summary>
        public const string EmailAddress_Unconfirmed_Get_Random = "SELECT TOP 1 EmailAddress, ConfirmGuid FROM EmailAddress WHERE IsConfirmed=0 ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 EmailAddress, ConfirmGuid FROM EmailAddress WHERE ConfirmGuid IS NOT NULL ORDER BY NEWID()
        /// </summary>
        public const string EmailAddressAndConfirmGuid_Get_Random = "SELECT TOP 1 EmailAddress, ConfirmGuid FROM EmailAddress WHERE ConfirmGuid IS NOT NULL ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 EmailAddressId FROM dbo.SystemUser_EmailAddress WHERE SystemUserId IN ( SELECT SystemUserId FROM ApplicationUser ) AND EmailAddressId IN ( SELECT EmailAddressId FROM Address) ORDER BY NEWID()
        /// </summary>
        public const string EmailAddressId_Random_AssociatedToApplicationUser_WithAddress = "SELECT TOP 1 EmailAddressId FROM dbo.SystemUser_EmailAddress WHERE SystemUserId IN ( SELECT SystemUserId FROM ApplicationUser ) AND EmailAddressId IN ( SELECT EmailAddressId FROM Address) ORDER BY NEWID()";

        /// <summary>
        /// SELECT EmailAddressId FROM EmailAddress WHERE EmailAddress='{0}'
        /// </summary>
        public const string EmailAddressId_Select = "SELECT EmailAddressId FROM EmailAddress WHERE EmailAddress='{0}'";

        /// <summary>
        /// SELECT TOP 1 EmailAddressId FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress) ORDER BY NEWID()
        /// </summary>
        public const string EmailAddressId_Select_AssociatedToSystemUser = "SELECT TOP 1 EmailAddressId FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress) ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 EmailAddressId FROM EmailAddress WHERE EmailAddressId NOT IN (SELECT EmailAddressId FROM SystemUser_EmailAddress) ORDER BY NEWID()
        /// </summary>
        public const string EmailAddressId_Select_NotAssociatedToSystemUser = "SELECT TOP 1 EmailAddressId FROM EmailAddress WHERE EmailAddressId NOT IN (SELECT EmailAddressId FROM SystemUser_EmailAddress) ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 EmailAddressId FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress WHERE SystemUserId={0}) ORDER BY NEWID()
        /// </summary>
        public const string EmailAddressId_SelectForSystemUser = "SELECT TOP 1 EmailAddressId FROM EmailAddress WHERE EmailAddressId IN (SELECT EmailAddressId FROM SystemUser_EmailAddress WHERE SystemUserId={0}) ORDER BY NEWID()";

        /// <summary>
        /// SELECT TOP 1 EmailAddressId, ConfirmGuid FROM EmailAddress WHERE ConfirmGuid IS NOT NULL ORDER BY NEWID()
        /// </summary>
        public const string EmailAddressIdAndConfirmGuid_Get_Random = "SELECT TOP 1 EmailAddressId, ConfirmGuid FROM EmailAddress WHERE ConfirmGuid IS NOT NULL ORDER BY NEWID()";

        /// <summary>
        /// SELECT IsConfirmed FROM EmailAddress WHERE EmailAddress='{0}'
        /// </summary>
        public const string IsConfirmed_Select = "SELECT IsConfirmed FROM EmailAddress WHERE EmailAddress='{0}'";

        /// <summary>
        /// UPDATE EmailAddress SET IsConfirmed=0 WHERE EmailAddress='{0}'
        /// </summary>
        public const string IsConfirmed_Update = "UPDATE EmailAddress SET IsConfirmed=0 WHERE EmailAddress='{0}'";

        /// <summary>
        /// SELECT PhoneNumber FROM dbo.PhoneNumber WHERE PhoneNumberId = {0}
        /// </summary>
        public const string PhoneNumber_Get_ByPhoneNumberId = "SELECT PhoneNumber FROM dbo.PhoneNumber WHERE PhoneNumberId = {0}";

        /// <summary>
        /// SELECT TOP 1 PhoneNumber FROM dbo.PhoneNumber ORDER BY NEWID()
        /// </summary>
        public const string PhoneNumber_Get_Random = "SELECT TOP 1 PhoneNumber FROM dbo.PhoneNumber ORDER BY NEWID()";

        /// <summary>
        /// SELECT PhoneNumberId FROM PhoneNumber WHERE PhoneNumber = '{0}'
        /// </summary>
        public const string PhoneNumberId_Get_ByPhoneNumber = "SELECT PhoneNumberId FROM PhoneNumber WHERE PhoneNumber = '{0}'";

        /// <summary>
        /// SELECT PhoneNumberId FROM SystemUser WHERE SystemUserId = {0}
        /// </summary>
        public const string PhoneNumberId_Get_ForSystemUser = "SELECT PhoneNumberId FROM SystemUser WHERE SystemUserId = {0}";
    }
}

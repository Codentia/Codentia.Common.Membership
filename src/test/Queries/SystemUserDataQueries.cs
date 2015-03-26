using Codentia.Test;

namespace Codentia.Common.Membership.Test.Queries
{
	 /// <summary>
	/// SystemUserData Queries
	/// </summary>
	[CoverageExclude]
	public static class SystemUserDataQueries
	{
		/// <summary>
		/// SELECT EmailAddressOrder FROM SystemUser_EmailAddress WHERE SystemUserId={0} AND EmailAddressId={1}
		/// </summary>
		public const string EmailAddressOrder_Select_BySystemUserAndEmailAddress = "SELECT EmailAddressOrder FROM SystemUser_EmailAddress WHERE SystemUserId={0} AND EmailAddressId={1}";

		/// <summary>
		/// SELECT TOP 1 RoleName from aspnet_roles ORDER BY NEWID()
		/// </summary>
		public const string GetRandomRoleName = "SELECT TOP 1 RoleName from aspnet_roles ORDER BY NEWID()";

		/// <summary>
		/// SELECT password FROM aspnet_membership WHERE email = '{0}'
		/// </summary>
		public const string Password_Get = "SELECT password FROM aspnet_membership WHERE email = '{0}'";

		/// <summary>
		/// SELECT 1 from aspnet_usersinroles uir INNER JOIN SystemUser lu ON lu.userid=uir.userid INNER JOIN aspnet_roles r ON r.roleid=uir.roleid WHERE SystemUserid={0} AND roleName='{1}'
		/// </summary>
		public const string RoleCheck = "SELECT 1 from aspnet_usersinroles uir INNER JOIN SystemUser lu ON lu.userid=uir.userid INNER JOIN aspnet_roles r ON r.roleid=uir.roleid WHERE SystemUserid={0} AND roleName='{1}'";

		/// <summary>
		/// DELETE FROM SystemUser_Address WHERE AddressId={0}
		/// </summary>
		public const string SystemUser_Address_Delete_ByAddress = "DELETE FROM SystemUser_Address WHERE AddressId={0}";

		/// <summary>
		/// DELETE FROM SystemUser_Address WHERE SystemUserId={0}
		/// </summary>
		public const string SystemUser_Address_Delete_BySystemUser = "DELETE FROM SystemUser_Address WHERE SystemUserId={0}";

		/// <summary>
		/// SELECT AddressId FROM SystemUser_Address  WHERE SystemUserId ={0}
		/// </summary>
		public const string SystemUser_Address_Select_BySystemUser = "SELECT AddressId FROM SystemUser_Address  WHERE SystemUserId ={0}";

		/// <summary>
		/// SELECT AddressId FROM SystemUser_Address sua INNER JOIN SystemUser su ON su.SystemUserId=sua.SystemUserId WHERE UserId ='{0}'
		/// </summary>
		public const string SystemUser_Address_Select_ByUserId = "SELECT AddressId FROM SystemUser_Address sua INNER JOIN SystemUser su ON su.SystemUserId=sua.SystemUserId WHERE UserId ='{0}'";

		/// <summary>
		/// SELECT TOP 1 sue.EmailAddressId, SystemUserId, EmailAddressOrder, ConfirmGuid FROM SystemUser_EmailAddress sue INNER JOIN EmailAddress ea ON ea.EmailAddressId=sue.EmailAddressId ORDER BY NEWID()
		/// </summary>
		public const string SystemUser_EmailAddress_All_Get_Random = "SELECT TOP 1 sue.EmailAddressId, SystemUserId, EmailAddressOrder, ConfirmGuid FROM SystemUser_EmailAddress sue INNER JOIN EmailAddress ea ON ea.EmailAddressId=sue.EmailAddressId ORDER BY NEWID()";

		/// <summary>
		/// SELECT COUNT(sue.EmailAddressId) FROM SystemUser_EmailAddress sue INNER JOIN EmailAddress ea ON ea.EmailAddressId=sue.EmailAddressId ORDER BY NEWID()
		/// </summary>
		public const string SystemUser_EmailAddress_All_Get_Random_Count = "SELECT COUNT(sue.EmailAddressId) FROM SystemUser_EmailAddress sue INNER JOIN EmailAddress ea ON ea.EmailAddressId=sue.EmailAddressId ORDER BY NEWID()";

		/// <summary>
		/// DELETE FROM SystemUser_EmailAddress WHERE EmailAddressId={0}
		/// </summary>
		public const string SystemUser_EmailAddress_DeleteByEmailAddress = "DELETE FROM SystemUser_EmailAddress WHERE EmailAddressId={0}";

		/// <summary>
		/// DELETE FROM SystemUser_EmailAddress WHERE SystemUserId={0}
		/// </summary>
		public const string SystemUser_EmailAddress_DeleteBySystemUser = "DELETE FROM SystemUser_EmailAddress WHERE SystemUserId={0}";

		/// <summary>
		/// SELECT TOP 1 EmailAddress FROM EmailAddress E WHERE EXISTS ( SELECT 1 FROM SystemUser_EmailAddress WHERE EmailAddressId = E.EmailAddressId ) ORDER BY NEWID()
		/// </summary>
		public const string SystemUser_EmailAddress_ExistingEmailAddress = "SELECT TOP 1 EmailAddress FROM EmailAddress E WHERE EXISTS ( SELECT 1 FROM SystemUser_EmailAddress WHERE EmailAddressId = E.EmailAddressId ) ORDER BY NEWID()";

		 /// <summary>
		/// INSERT INTO dbo.SystemUser_EmailAddress  (SystemUserId, EmailAddressId, EmailAddressOrder) VALUES ({0}, {1}, {2})
		/// </summary>
		public const string SystemUser_EmailAddress_Insert = "INSERT INTO dbo.SystemUser_EmailAddress  (SystemUserId, EmailAddressId, EmailAddressOrder) VALUES ({0}, {1}, {2})";        

		/// <summary>
		/// SELECT TOP 1 SystemUserId FROM SystemUser_EmailAddress ORDER BY NEWID()
		/// </summary>
		public const string SystemUser_EmailAddress_SystemUser_Get_Random = "SELECT TOP 1 SystemUserId FROM SystemUser_EmailAddress ORDER BY NEWID()";

		/// <summary>
		/// SELECT FirstName, Surname FROM SystemUser WHERE SystemUserId={0}
		/// </summary>
		public const string SystemUser_FirstAndLastNames = "SELECT FirstName, Surname FROM SystemUser WHERE SystemUserId={0}";

		/// <summary>
		/// SELECT ForcePassword FROM SystemUser WHERE SystemUserId={0}
		/// </summary>
		public const string SystemUser_ForcePassword = "SELECT ForcePassword FROM SystemUser WHERE SystemUserId={0}";
		
		/// <summary>
		/// SELECT SystemUserId, FirstName, Surname, UserId, ForcePassword, HasNewsLetter, PhoneNumberId, InsertDateTime FROM dbo.SystemUser WHERE SystemUserId = {0}
		/// </summary>
		public const string SystemUser_Get_ById = "SELECT SystemUserId, FirstName, Surname, UserId, ForcePassword, HasNewsLetter, PhoneNumberId, InsertDateTime FROM dbo.SystemUser WHERE SystemUserId = {0}";

		/// <summary>
		/// SELECT TOP 1 SystemUserId FROM SystemUser WHERE UserId IS NOT NULL ORDER BY NEWID()
		/// </summary>
		public const string SystemUser_Get_HasUserId_Random = "SELECT TOP 1 SystemUserId FROM SystemUser WHERE UserId IS NOT NULL ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 1 SystemUserID FROM SystemUser WHERE UserId IN (SELECT UserId FROM aspnet_Users) ORDER BY NEWID()
		/// </summary>
		public const string SystemUser_Get_NotInAspNetUsers_Random = "SELECT TOP 1 SystemUserID FROM SystemUser WHERE UserId IN (SELECT UserId FROM aspnet_Users) ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 1 SystemUserId, FirstName, Surname, UserId, ForcePassword, HasNewsLetter, PhoneNumberId, InsertDateTime FROM SystemUser ORDER BY NEWID()
		/// </summary>
		public const string SystemUser_Get_Random = "SELECT TOP 1 SystemUserId, FirstName, Surname, UserId, ForcePassword, HasNewsLetter, PhoneNumberId, InsertDateTime FROM SystemUser ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 10 SystemUserId FROM SystemUser ORDER BY NEWID()
		/// </summary>
		public const string SystemUser_Get_Random_10 = "SELECT TOP 10 SystemUserId FROM SystemUser ORDER BY NEWID()";

		/// <summary>
		/// INSERT INTO dbo.SystemUser (UserId, FirstName, Surname, HasNewsLetter) VALUES ('{0}', '{1}', '{2}', {3})
		/// </summary>
		public const string SystemUser_InsertNoPhoneNumber = "INSERT INTO dbo.SystemUser (UserId, FirstName, Surname, HasNewsLetter) VALUES ('{0}', '{1}', '{2}', {3})";
		
		/// <summary>
		/// SELECT SystemUserAddressId FROM SystemUser_Address WHERE SystemUserId={0} AND AddressId={1}
		/// </summary>
		public const string SystemUserAddressId_Select_BySystemUserAndAddress = "SELECT SystemUserAddressId FROM SystemUser_Address WHERE SystemUserId={0} AND AddressId={1}";

		/// <summary>
		/// SELECT SystemUserId FROM SystemUser
		/// </summary>
		public const string SystemUserId_Get_All = "SELECT SystemUserId FROM SystemUser";

		/// <summary>
		/// SELECT SystemUserId FROM SystemUser WHERE UserId = '{0}'
		/// </summary>
		public const string SystemUserId_Get_ByUserId = "SELECT SystemUserId FROM SystemUser WHERE UserId = '{0}'";

		/// <summary>
		/// SELECT TOP 1 SystemUserId FROM SystemUser WHERE UserId IS NOT NULL ORDER BY NEWID()
		/// </summary>
		public const string SystemUserId_Get_HasUserId = "SELECT TOP 1 SystemUserId FROM SystemUser WHERE UserId IS NOT NULL ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 1 SystemUserId FROM ApplicationUser ORDER BY NEWID()
		/// </summary>
		public const string SystemUserId_Get_RandomForAppUser = "SELECT TOP 1 SystemUserId FROM ApplicationUser ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 1 UserId FROM SystemUser WHERE UserId IS NOT NULL ORDER BY NEWID()
		/// </summary>
		public const string SystemUserId_Get_RandomUserId = "SELECT TOP 1 UserId FROM SystemUser WHERE UserId IS NOT NULL ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 1 SU.SystemUserId FROM dbo.aspnet_users U INNER JOIN dbo.SystemUser SU ON SU.UserId = U.UserId WHERE SU.UserId IS NOT NULL  ORDER BY NEWID()
		/// </summary>
		public const string SystemUserId_InAspNetUsers_Get_Random = "SELECT TOP 1 SU.SystemUserId FROM dbo.aspnet_users U INNER JOIN dbo.SystemUser SU ON SU.UserId = U.UserId WHERE SU.UserId IS NOT NULL  ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 1 SystemUserId FROM SystemUser_Address ORDER BY NEWID()
		/// </summary>
		public const string SystemUserId_SystemUser_Address_Get_Random = "SELECT TOP 1 SystemUserId FROM SystemUser_Address ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 1 SystemUserId, AddressId FROM dbo.SystemUser_Address ORDER BY NEWID()
		/// </summary>
		public const string SystemUserId_WithAddress = "SELECT TOP 1 SystemUserId, AddressId FROM dbo.SystemUser_Address ORDER BY NEWID()";

		/// <summary>
		/// SELECT TOP 1 UserName from aspnet_Users ORDER BY NEWID()
		/// </summary>
		public const string UserName_AspNet_Users_Get = "SELECT TOP 1 UserName from aspnet_Users ORDER BY NEWID()";
	}		
}

using System;
using System.Data;
using System.Web.Security;
using Codentia.Common.Data;
using Codentia.Common.Helper;
using Codentia.Common.Logging;
using Codentia.Common.Logging.BL;
using Codentia.Common.Membership.Providers;

namespace Codentia.Common.Membership
{
    /// <summary>
    /// This class exposes methods to create, read, update and delete data related to Users   
    /// </summary>
    public static class SystemUserData
    {
        /// <summary>
        /// Get All email addresses for a system user
        /// </summary>
        /// <param name="systemUserId">Id of the user to retrieve an address for</param>
        /// <returns>DataTable - EmailAddresses</returns>
        public static DataTable GetEmailAddressesForSystemUser(int systemUserId)
        {
            return GetEmailAddressesForSystemUser(Guid.Empty, systemUserId);
        }

        /// <summary>
        /// Get All email addresses for a system user
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="systemUserId">Id of the user to retrieve an address for</param>
        /// <returns>DataTable - EmailAddresses</returns>
        public static DataTable GetEmailAddressesForSystemUser(Guid txnId, int systemUserId)
        {
            // TODO: This should be made obsolete in a future version
            ParameterCheckHelper.CheckIsValidId(systemUserId, "systemUserId");

            DbParameter[] spParams =
                {
                    new DbParameter("@SystemUserId", DbType.Int32, systemUserId),                    
                };

            DataTable result = DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_GetEmailAddresses", spParams, txnId);

            if (result == null || result.Rows.Count == 0)
            {
                throw new ArgumentException(string.Format("systemUserId: {0} does not exist", systemUserId));
            }

            return result;
        }

        /// <summary>
        /// Gets the system user.
        /// </summary>
        /// <param name="systemUserId">The system user id.</param>
        /// <returns>DataTable - SystemUser</returns>
        public static DataSet GetSystemUser(int systemUserId)
        {
            return GetSystemUser(Guid.Empty, systemUserId);
        }

        /// <summary>
        /// Retrieve a specific system user.
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="systemUserId">The SystemUserId</param>
        /// <returns>DataTable - Systemuser</returns>
        public static DataSet GetSystemUser(Guid txnId, int systemUserId)
        {
            ParameterCheckHelper.CheckIsValidId(systemUserId, "systemUserId");

            DbParameter[] spParams =
                {
                    new DbParameter("@SystemUserId", DbType.Int32, systemUserId)
                };

            DataSet result = DbInterface.ExecuteProcedureDataSet(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_GetById", spParams, txnId);

            if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
            {
                throw new ArgumentException(string.Format("systemUserId: {0} does not exist", systemUserId));
            }

            return result;
        }

        /// <summary>
        /// Retrieve a specific system user
        /// </summary>
        /// <param name="emailAddress">Primary email address to retrieve user for</param>
        /// <returns>DataTable - SystemUser</returns>
        public static DataSet GetSystemUser(string emailAddress)
        {
            return GetSystemUser(Guid.Empty, emailAddress);
        }

        /// <summary>
        /// Retrieve a specific system user
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="emailAddress">Primary email address to retrieve user for</param>
        /// <returns>DataTable - SystemUser</returns>
        public static DataSet GetSystemUser(Guid txnId, string emailAddress)
        {
            ParameterCheckHelper.CheckIsValidEmailAddress(emailAddress, 255, "emailAddress");

            DbParameter[] spParams =
            {
                new DbParameter("@EmailAddress", DbType.StringFixedLength, 255, emailAddress)
            };

            DataSet result = DbInterface.ExecuteProcedureDataSet(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_GetByEmail", spParams, txnId);

            if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
            {
                throw new ArgumentException(string.Format("emailAddress: {0} does not exist", emailAddress));
            }

            return result;
        }

        /// <summary>
        /// Check if a specified system user exists or not
        /// </summary>
        /// <param name="systemUserId">Id of the user to check</param>
        /// <returns>true if SystemUser exists</returns>
        public static bool SystemUserExists(int systemUserId)
        {
            return SystemUserExists(Guid.Empty, systemUserId);
        }

        /// <summary>
        /// Check if a specified system user exists or not
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="systemUserId">Id of the user to check</param>
        /// <returns>true if SystemUser exists</returns>
        public static bool SystemUserExists(Guid txnId, int systemUserId)
        {
            ParameterCheckHelper.CheckIsValidId(systemUserId, "systemUserId");

            DbParameter[] spParams =
                {
                    new DbParameter("@SystemUserId", DbType.Int32, systemUserId),
                    new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_ExistsById", spParams, txnId);

            return Convert.ToBoolean(spParams[1].Value);
        }

        /// <summary>
        /// Sets the role.
        /// </summary>
        /// <param name="systemUserId">The system user id.</param>
        /// <param name="roleName">Name of the role.</param>
        public static void SetRole(int systemUserId, string roleName)
        {
            SetRole(Guid.Empty, systemUserId, roleName);
        }

        /// <summary>
        /// Sets the role.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="systemUserId">The system user id.</param>
        /// <param name="roleName">Name of the role.</param>
        public static void SetRole(Guid txnId, int systemUserId, string roleName)
        {
            ParameterCheckHelper.CheckIsValidId(systemUserId, "systemUserId");
            ParameterCheckHelper.CheckIsValidString(roleName, "roleName", false);

            if (!Roles.RoleExists(roleName))
            {
                throw new ArgumentException(string.Format("roleName: {0} does not exist", roleName));
            }

            DbParameter[] spParams =
            {
                new DbParameter("@SystemUserId", DbType.Int32, systemUserId),
                new DbParameter("@RoleName", DbType.StringFixedLength, 50, roleName)
            };

            spParams[0].Value = systemUserId;
            spParams[1].Value = roleName;

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_SetRole", spParams, txnId);

            LogManager.Instance.AddToLog(LogMessageType.Information, "SystemUserData", string.Format("SetRole SystemUser Id={0}, roleName={1}", systemUserId, roleName));
        }

        /// <summary>
        /// Creates the system user.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="membershipUserId">The membership user id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="surname">The surname.</param>
        /// <param name="hasNewsLetter">if set to <c>true</c> [has news letter].</param>
        /// <param name="primaryEmailAddressId">The primary email address id.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>The systemUserId</returns>
        public static int CreateSystemUser(Guid txnId, Guid membershipUserId, string firstName, string surname, bool hasNewsLetter, int primaryEmailAddressId, string phoneNumber)
        {
            int systemUserId = 0;

            // validate membership user
            if (!MembershipUserExists(txnId, membershipUserId))
            {
                throw new ArgumentException(string.Format("MembershipUser {0} does not exist", membershipUserId));
            }

            // nullable parameter check (validate other parameters)
            ParameterCheckHelper.CheckIsValidId(primaryEmailAddressId, "primaryEmailAddressId");

            if (!ContactData.EmailAddressExists(txnId, primaryEmailAddressId))
            {
                throw new ArgumentException(string.Format("primaryEmailAddressId: {0} does not exist", primaryEmailAddressId));
            }

            int alreadyAssociatedSystemUserId = ContactData.GetSystemUserIdForEmailAddress(txnId, primaryEmailAddressId);
            if (alreadyAssociatedSystemUserId > 0)
            {
                throw new ArgumentException("Email address has already been registered");
            }

            try
            {
                DbParameter[] spParams =
                {                    
                    new DbParameter("@UserId", DbType.Guid, membershipUserId),
                    new DbParameter("@FirstName", DbType.StringFixedLength, 100, firstName),
                    new DbParameter("@Surname", DbType.StringFixedLength, 100, surname),                    
                    new DbParameter("@PrimaryEmailAddressId", DbType.Int32, primaryEmailAddressId),
                    new DbParameter("@HasNewsLetter", DbType.Boolean, hasNewsLetter),
                    new DbParameter("@PhoneNumberId", DbType.Int32),
                    new DbParameter("@SystemUserId", DbType.Int32, ParameterDirection.Output, 0)
                };

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    spParams[5].Value = ContactData.CreatePhoneNumber(txnId, phoneNumber);
                }
                else
                {
                    spParams[5].Value = DBNull.Value;
                }

                DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_Create", spParams, txnId);
                systemUserId = Convert.ToInt32(spParams[6].Value);
            }
            catch (Exception ex)
            {
                LogManager.Instance.AddToLog(LogMessageType.Information, "SystemUserData", string.Format("ROLLED BACK: Created SystemUser Id={0} with parameters firstName={1}, surname={2}, hasNewsLetter={3}, primaryEmailAddressId={4}, phoneNumber={5}  errorText={6}", systemUserId, firstName, surname, hasNewsLetter, primaryEmailAddressId, phoneNumber, ex.Message));
                throw ex;
            }

            LogManager.Instance.AddToLog(LogMessageType.Information, "SystemUserData", string.Format("Created SystemUser Id={0} with parameters firstName={1}, surname={2}, hasNewsLetter={3}, primaryEmailAddressId={4}, phoneNumber={5}", systemUserId, firstName, surname, hasNewsLetter, primaryEmailAddressId, phoneNumber));

            return systemUserId;
        }

        /// <summary>
        /// Gets the system user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>DataTable - SystemUser</returns>
        public static DataTable GetSystemUser(Guid userId)
        {
            ParameterCheckHelper.CheckIsValidGuid(userId, "userId");

            DbParameter[] spParams =
                {
                    new DbParameter("@UserId", DbType.Guid, userId)
                };

            DataTable systemUserData = DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_GetByUserId", spParams);

            return systemUserData;
        }

        /// <summary>
        /// Updates the system user.
        /// </summary>
        /// <param name="systemUserId">The system user id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="surname">The surname.</param>
        /// <param name="hasNewsLetter">if set to <c>true</c> [has news letter].</param>
        /// <param name="phoneNumber">The phone number.</param>
        public static void UpdateSystemUser(int systemUserId, string firstName, string surname, bool hasNewsLetter, string phoneNumber)
        {
            ParameterCheckHelper.CheckIsValidId(systemUserId, "systemUserId");
            ParameterCheckHelper.CheckIsValidString(firstName, "first name", false);
            ParameterCheckHelper.CheckIsValidString(surname, "surname", false);

            Guid txnId = Guid.NewGuid();

            try
            {
                DbParameter[] spParams =
                {
                    new DbParameter("@SystemUserId", DbType.Int32, systemUserId),                    
                    new DbParameter("@FirstName", DbType.StringFixedLength, 100, firstName),
                    new DbParameter("@Surname", DbType.StringFixedLength, 100, surname),                    
                    new DbParameter("@HasNewsLetter", DbType.Boolean, hasNewsLetter),
                    new DbParameter("@PhoneNumberId", DbType.Int32)
                };

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    spParams[4].Value = ContactData.CreatePhoneNumber(txnId, phoneNumber);
                }

                DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_Update", spParams, txnId);
                DbInterface.CommitTransaction(txnId);
            }
            catch (Exception ex)
            {
                DbInterface.RollbackTransaction(txnId);
                LogManager.Instance.AddToLog(LogMessageType.Information, "SystemUserData", string.Format("ROLLED BACK: Updated SystemUser Id={0} with parameters firstName={1}, surname={2}, hasNewsLetter={3}, phoneNumber={4}: ErrorText: {5}", systemUserId, firstName, surname, hasNewsLetter, phoneNumber, ex.Message));
                throw ex;
            }

            LogManager.Instance.AddToLog(LogMessageType.Information, "SystemUserData", string.Format("Updated SystemUser Id={0} with parameters firstName={1}, surname={2}, hasNewsLetter={3}, phoneNumber={4}", systemUserId, firstName, surname, hasNewsLetter, phoneNumber));
        }

        /// <summary>
        /// Sets the force password.
        /// </summary>
        /// <param name="systemUserId">The system user id.</param>
        public static void SetForcePassword(int systemUserId)
        {
            ParameterCheckHelper.CheckIsValidId(systemUserId, "systemUserId");

            DbParameter[] spParams =
                {
                    new DbParameter("@SystemUserId", DbType.Int32, systemUserId)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_SetForcePassword", spParams);
        }

        /// <summary>
        /// Associates the system user to email address.
        /// </summary>
        /// <param name="emailAddressId">The email address id.</param>
        /// <param name="systemUserId">The system user id.</param>
        /// <param name="emailAddressOrder">The email address order.</param>
        public static void AssociateSystemUserToEmailAddress(int emailAddressId, int systemUserId, int emailAddressOrder)
        {
            ParameterCheckHelper.CheckIsValidId(emailAddressId, "emailAddressId");
            ParameterCheckHelper.CheckIsValidId(systemUserId, "systemUserId");
            ParameterCheckHelper.CheckIsValidId(emailAddressOrder, "emailAddressOrder");

            if (!ContactData.EmailAddressExists(emailAddressId))
            {
                throw new ArgumentException(string.Format("emailAddressId: {0} does not exist", emailAddressId));
            }

            DbParameter[] spParams =
                {
                    new DbParameter("@SystemUserId", DbType.Int32, systemUserId),
                    new DbParameter("@EmailAddressId", DbType.Int32, emailAddressId),                                        
                    new DbParameter("@EmailAddressOrder", DbType.Int32, emailAddressOrder)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_AssociateToEmailAddress", spParams);
        }

        /// <summary>
        /// Dissociates the system user from email address.
        /// </summary>
        /// <param name="emailAddressId">The email address identifier.</param>
        /// <param name="systemUserId">The system user identifier.</param>
        /// <exception cref="System.ArgumentException">Email address does not exist</exception>
        public static void DissociateSystemUserFromEmailAddress(int emailAddressId, int systemUserId)
        {
            ParameterCheckHelper.CheckIsValidId(emailAddressId, "emailAddressId");
            ParameterCheckHelper.CheckIsValidId(systemUserId, "systemUserId");

            if (!ContactData.EmailAddressExists(emailAddressId))
            {
                throw new ArgumentException(string.Format("emailAddressId: {0} does not exist", emailAddressId));
            }

            DbParameter[] spParams =
                {
                    new DbParameter("@SystemUserId", DbType.Int32, systemUserId),
                    new DbParameter("@EmailAddressId", DbType.Int32, emailAddressId) 
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.SystemUser_DissociateFromEmailAddress", spParams);
        }

        private static bool MembershipUserExists(Guid txnId, Guid userId)
        {
            ParameterCheckHelper.CheckIsValidGuid(userId, "userId");

            bool exists = false;
            DbParameter[] spParams =
            {
                new DbParameter("@UserId", DbType.Guid, userId),
                new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
            };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.MembershipUser_ExistsById", spParams, txnId);

            exists = Convert.ToBoolean(spParams[1].Value);

            return exists;
        }
    }
}

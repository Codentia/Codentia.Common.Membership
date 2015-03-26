using System;
using System.Data;
using Codentia.Common.Data;
using Codentia.Common.Helper;
using Codentia.Common.Logging;
using Codentia.Common.Logging.BL;
using Codentia.Common.Membership.Providers;

namespace Codentia.Common.Membership
{
    /// <summary>
    /// This class exposes methods to create, read, update data related to Email Addresses 
    /// </summary>
    public static class ContactData
    {
        /// <summary>
        /// Check if a specified email address exists or not
        /// </summary>
        /// <param name="emailAddress">emailAddress to check</param>       
        /// <returns>Does Exist</returns>
        public static bool EmailAddressExists(string emailAddress)
        {
            return EmailAddressExists(Guid.Empty, emailAddress);
        }

        /// <summary>
        /// Check if a specified email address exists or not
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="emailAddress">emailAddress to check</param>       
        /// <returns>Does Exist</returns>
        public static bool EmailAddressExists(Guid txnId, string emailAddress)
        {
            ParameterCheckHelper.CheckIsValidEmailAddress(emailAddress, 255, "emailAddress");

            DbParameter[] spParams =
                {
                    new DbParameter("@EmailAddress", DbType.StringFixedLength, 255, emailAddress),                   
                    new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_ExistsByAddress", spParams, txnId);

            return Convert.ToBoolean(spParams[1].Value);
        }

        /// <summary>
        /// Check if a specified email address Id exists or not
        /// </summary>
        /// <param name="emailAddressId">emailAddressId to check</param>       
        /// <returns>Does Exist</returns>
        public static bool EmailAddressExists(int emailAddressId)
        {
            return EmailAddressExists(Guid.Empty, emailAddressId);
        }

        /// <summary>
        /// Check if a specified email address Id exists or not
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="emailAddressId">emailAddressId to check</param>       
        /// <returns>Does Exist</returns>
        public static bool EmailAddressExists(Guid txnId, int emailAddressId)
        {
            ParameterCheckHelper.CheckIsValidId(emailAddressId, "emailAddressId");

            DbParameter[] spParams =
                {
                    new DbParameter("@EmailAddressId", DbType.Int32, emailAddressId),                   
                    new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_ExistsById", spParams, txnId);

            return Convert.ToBoolean(spParams[1].Value);
        }

        /// <summary>
        /// CreateEmailAddress and return the new id
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>
        /// The emailAddressId
        /// </returns>
        public static int CreateEmailAddress(string emailAddress)
        {
            return CreateEmailAddress(Guid.Empty, emailAddress);
        }

        /// <summary>
        /// CreateEmailAddress and return the new id (in a transaction)
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>
        /// The emailAddressId
        /// </returns>
        public static int CreateEmailAddress(Guid txnId, string emailAddress)
        {
            int emailAddressId;
            ParameterCheckHelper.CheckIsValidEmailAddress(emailAddress, 256, "emailAddress");

            if (EmailAddressExists(txnId, emailAddress))
            {
                throw new ArgumentException(string.Format("emailAddress: {0} already exists", emailAddress));
            }

            DbParameter[] spParams =
                {
                    new DbParameter("@EmailAddress", DbType.StringFixedLength, 256, emailAddress),                
                    new DbParameter("@EmailAddressId", DbType.Int32, ParameterDirection.Output, 0)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_Create", spParams, txnId);

            emailAddressId = Convert.ToInt32(spParams[1].Value);

            LogManager.Instance.AddToLog(LogMessageType.Information, "EmailAddressData", string.Format("CreateEmailAddress: emailAddress={0}", emailAddress));

            return emailAddressId;
        }

        /// <summary>
        /// Retrieve the SystemUser (if any) associated to an email address
        /// </summary>
        /// <param name="emailAddressId">email addressId</param>
        /// <returns>The systemUserId</returns>
        public static int GetSystemUserIdForEmailAddress(int emailAddressId)
        {
            return GetSystemUserIdForEmailAddress(Guid.Empty, emailAddressId);
        }

        /// <summary>
        /// Retrieve the SystemUser (if any) associated to an email address
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="emailAddressId">email addressId</param>
        /// <returns>The systemUserId</returns>
        public static int GetSystemUserIdForEmailAddress(Guid txnId, int emailAddressId)
        {
            if (!EmailAddressExists(txnId, emailAddressId))
            {
                throw new ArgumentException(string.Format("emailAddressId: {0} does not exist", emailAddressId));
            }

            DbParameter[] spParams =
                {
                    new DbParameter("@EmailAddressId", DbType.Int32, emailAddressId),                    
                    new DbParameter("@SystemUserId", DbType.Int32, ParameterDirection.Output, 0)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_GetSystemUserId", spParams, txnId);

            int systemUserId = 0;

            if (spParams[1].Value != DBNull.Value)
            {
                systemUserId = Convert.ToInt32(spParams[1].Value);
            }

            return systemUserId;
        }

        /// <summary>
        /// Gets the email address data.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>DataTable - EmailAddress</returns>
        public static DataTable GetEmailAddressData(string emailAddress)
        {
            return GetEmailAddressData(Guid.Empty, emailAddress);
        }

        /// <summary>
        /// Retrieve email address data (by address) in a transaction
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="emailAddress">The email address</param>
        /// <returns>DataTable - Email Address</returns>
        public static DataTable GetEmailAddressData(Guid txnId, string emailAddress)
        {
            ParameterCheckHelper.CheckIsValidString(emailAddress, "emailAddress", false);

            DbParameter[] spParams =
            {
                new DbParameter("@EmailAddress", DbType.StringFixedLength, 255, emailAddress)
            };

            DataTable result = DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_GetByAddress", spParams, txnId);

            if (result == null || result.Rows.Count == 0)
            {
                throw new ArgumentException(string.Format("emailAddress: {0} does not exist", emailAddress));
            }

            return result;
        }

        /// <summary>
        /// Retrieve email address data (by Id)
        /// </summary>
        /// <param name="emailAddressId">The emailAddressId</param>
        /// <returns>DataTable - EmailAddress</returns>
        public static DataTable GetEmailAddressData(int emailAddressId)
        {
            return GetEmailAddressData(Guid.Empty, emailAddressId);
        }

        /// <summary>
        /// Gets the email address data.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="emailAddressId">The email address id.</param>
        /// <returns>DataTable - EmailAddress</returns>
        public static DataTable GetEmailAddressData(Guid txnId, int emailAddressId)
        {
            ParameterCheckHelper.CheckIsValidId(emailAddressId, "emailAddressId");

            if (!EmailAddressExists(txnId, emailAddressId))
            {
                throw new ArgumentException(string.Format("emailAddressId: {0} does not exist", emailAddressId));
            }

            DbParameter[] spParams =
            {
                new DbParameter("@EmailAddressId", DbType.Int32, emailAddressId)
            };

            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_GetById", spParams, txnId);
        }

        /// <summary>
        /// Confirm Email Address (from a confirmation email link)
        /// </summary>
        /// <param name="emailAddress">email address to confirm</param>
        /// <param name="confirmGuid">Guid from parameter string</param>
        /// <returns>bool - true = successful, false - unsuccessful</returns>
        public static bool Confirm(string emailAddress, Guid confirmGuid)
        {
            ParameterCheckHelper.CheckIsValidString(emailAddress, "emailAddress", 255, false);
            ParameterCheckHelper.CheckIsValidGuid(confirmGuid, "confirmGuid");

            DataTable dt = ContactData.GetEmailAddressData(confirmGuid);

            if (dt.Rows.Count == 0)
            {
                throw new ArgumentException(string.Format("confirmGuid: {0} does not exist", confirmGuid));
            }
            else
            {
                if (emailAddress != Convert.ToString(dt.Rows[0]["EmailAddress"]))
                {
                    throw new ArgumentException(string.Format("confirmGuid: {0} exists for another emailaddress", confirmGuid));
                }
            }

            DbParameter[] spParams =
                {
                    new DbParameter("@EmailAddress", DbType.StringFixedLength, 255, emailAddress),                    
                    new DbParameter("@ConfirmGuid", DbType.Guid, confirmGuid),
                    new DbParameter("@IsConfirmed", DbType.Boolean, ParameterDirection.Output, false)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_Confirm", spParams);

            LogManager.Instance.AddToLog(LogMessageType.Information, "EmailAddressData", string.Format("ConfirmEmailAddress: emailAddress={0}, confirmGuid={1}", emailAddress, confirmGuid));

            return Convert.ToBoolean(spParams[2].Value);
        }

        /// <summary>
        /// Retrieve email address data (by Cookie)
        /// </summary>
        /// <param name="emailAddressCookie">The emailAddressCookie</param>
        /// <returns>DataTable - EmailAddress</returns>
        public static DataTable GetEmailAddressData(Guid emailAddressCookie)
        {
            ParameterCheckHelper.CheckIsValidGuid(emailAddressCookie, "emailAddressCookie");

            DbParameter[] spParams =
                {
                    new DbParameter("@Cookie", DbType.Guid, emailAddressCookie)
                };

            DataTable systemUserData = DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_GetByCookie", spParams);

            return systemUserData;
        }

        /// <summary>
        /// Create a new PhoneNumber record (or return extant, matching id if already created)
        /// Always updated as part of another process so needs a transaction Id
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="phoneNumber">PhoneNumber to create entry for</param>
        /// <returns>The phoneNumberId</returns>
        public static int CreatePhoneNumber(Guid txnId, string phoneNumber)
        {
            ParameterCheckHelper.CheckIsValidString(phoneNumber, "phoneNumber", false);

            phoneNumber = phoneNumber.Replace("(", string.Empty);
            phoneNumber = phoneNumber.Replace(")", string.Empty);
            phoneNumber = phoneNumber.Replace(" ", string.Empty);

            ParameterCheckHelper.CheckStringIsValidPhoneNumber(phoneNumber, 13, "phoneNumber");

            DbParameter[] spParams =
            {
                new DbParameter("@PhoneNumber", DbType.StringFixedLength, 13, phoneNumber),                    
                new DbParameter("@PhoneNumberId", DbType.Int32, ParameterDirection.Output, 0)
            };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.PhoneNumber_Create", spParams, txnId);

            return Convert.ToInt32(spParams[1].Value);
        }

        /// <summary>
        /// Retrieve an existing PhoneNumber entry.
        /// </summary>
        /// <param name="phoneNumberId">The phonenumberId</param>
        /// <returns>string - phone number</returns>
        public static string GetPhoneNumber(int phoneNumberId)
        {
            string phoneNumber;

            ParameterCheckHelper.CheckIsValidId(phoneNumberId, "phoneNumberId");

            DbParameter[] spParams =
            {
                new DbParameter("@PhoneNumberId", DbType.Int32, phoneNumberId),
                new DbParameter("@PhoneNumber", DbType.StringFixedLength, 11, ParameterDirection.Output, DBNull.Value)
            };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.PhoneNumber_GetById", spParams);

            phoneNumber = Convert.ToString(spParams[1].Value);

            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentException(string.Format("phoneNumberId: {0} does not exist", phoneNumberId));
            }

            return phoneNumber;
        }

        /// <summary>
        /// Retrieve all addresses associated to an email Address
        /// </summary>
        /// <param name="emailAddressId">email Address Id</param>
        /// <returns>DataTable - EmailAddress</returns>
        public static DataTable GetAddressesForEmailAddress(int emailAddressId)
        {
            ParameterCheckHelper.CheckIsValidId(emailAddressId, "emailAddressId");

            if (!EmailAddressExists(emailAddressId))
            {
                throw new ArgumentException(string.Format("emailAddressId: {0} does not exist", emailAddressId));
            }

            DbParameter[] spParams =
            {
                new DbParameter("@EmailAddressId", DbType.Int32, emailAddressId),
                new DbParameter("@AssemblyVersion", DbType.String, 10, Abstraction.Version)
            };

            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.EmailAddress_GetAddresses", spParams);
        }
    }
}

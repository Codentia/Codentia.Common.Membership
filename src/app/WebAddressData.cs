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
    /// This class exposes methods to create, retrieve update Web Address and WebAddressType Data
    /// </summary>
    public class WebAddressData
    {
        /// <summary>
        /// Retrieve all available Web Address Types
        /// </summary>
        /// <returns>DataTable - WebAddress</returns>
        public static DataTable GetWebAddressTypes()
        {
            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.WebAddressType_GetAll");
        }

        /// <summary>
        /// Check if a specified URL exists or not
        /// </summary>
        /// <param name="url">URL to check</param>       
        /// <returns>true if the WebAddress exists</returns>
        public static bool WebAddressExists(string url)
        {
            return WebAddressExists(Guid.Empty, url);
        }

        /// <summary>
        /// Check if a specified URL exists or not
        /// </summary>
        /// <param name="txnId">transaction (if applicable)</param>
        /// <param name="url">URL to check</param>       
        /// <returns>true if the WebAddress exists</returns>
        public static bool WebAddressExists(Guid txnId, string url)
        {
            ParameterCheckHelper.CheckIsValidString(url, "url", 300, false);

            DbParameter[] spParams =
                {
                    new DbParameter("@URL", DbType.StringFixedLength, 300, url),                   
                    new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.WebAddress_ExistsByURL", spParams, txnId);

            return Convert.ToBoolean(spParams[1].Value);
        }

        /// <summary>
        /// Check if a specified web address Id exists or not
        /// </summary>
        /// <param name="webAddressId">webAddressId to check</param>       
        /// <returns>true if the WebAddress exists</returns>
        public static bool WebAddressExists(int webAddressId)
        {
            return WebAddressData.WebAddressExists(Guid.Empty, webAddressId);
        }

        /// <summary>
        /// Check if a specified web address Id exists or not
        /// </summary>
        /// <param name="txnId">transaction (if applicable)</param>
        /// <param name="webAddressId">webAddressId to check</param>       
        /// <returns>true if the WebAddress exists</returns>
        public static bool WebAddressExists(Guid txnId, int webAddressId)
        {
            ParameterCheckHelper.CheckIsValidId(webAddressId, "webAddressId");

            DbParameter[] spParams =
                {
                    new DbParameter("@WebAddressId", DbType.Int32, webAddressId),                   
                    new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.WebAddress_ExistsById", spParams, txnId);

            return Convert.ToBoolean(spParams[1].Value);
        }

        /// <summary>
        /// Retrieve web address data (by Id)
        /// </summary>
        /// <param name="webAddressId">webAddressId to check</param>       
        /// <returns>DataTable - WebAddress</returns>
        public static DataTable GetWebAddressData(int webAddressId)
        {
            return WebAddressData.GetWebAddressData(Guid.Empty, webAddressId);
        }
        
        /// <summary>
        /// Retrieve web address data (by Id)
        /// </summary>
        /// <param name="txnId">transaction (if applicable)</param>
        /// <param name="webAddressId">webAddressId to check</param>       
        /// <returns>DataTable - WebAddress</returns>
        public static DataTable GetWebAddressData(Guid txnId, int webAddressId)
        {
            if (!WebAddressExists(txnId, webAddressId))
            {
                throw new ArgumentException(string.Format("webAddressId: {0} does not exist", webAddressId));
            }

            DbParameter[] spParams =
            {
                new DbParameter("@WebAddressId", DbType.Int32, webAddressId)                   
            };

            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.WebAddress_GetById", spParams, txnId);
        }

        /// <summary>
        /// Retrieve web address data (by URL)
        /// </summary>
        /// <param name="url">url to check</param>       
        /// <returns>DataTable - WebAddress</returns>
        public static DataTable GetWebAddressData(string url)
        {
            return WebAddressData.GetWebAddressData(Guid.Empty, url);
        }
       
        /// <summary>
        /// Retrieve web address data (by URL)
        /// </summary>
        /// <param name="txnId">transaction (if applicable)</param>
        /// <param name="url">url to check</param>       
        /// <returns>DataTable - WebAddress</returns>
        public static DataTable GetWebAddressData(Guid txnId, string url)
        {
            if (!WebAddressExists(url))
            {
                throw new ArgumentException(string.Format("url: {0} does not exist", url));
            }

            DbParameter[] spParams =
            {
                new DbParameter("@URL", DbType.StringFixedLength, 300, url)                   
            };

            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.WebAddress_GetByURL", spParams, txnId);
        }

        /// <summary>
        /// Update URL as a dead link
        /// </summary>
        /// <param name="webAddressId">webAddressId to check</param>       
        public static void UpdateWebAddressAsDead(int webAddressId)
        {
            if (!WebAddressExists(webAddressId))
            {
                throw new ArgumentException(string.Format("webAddressId: {0} does not exist", webAddressId));
            }

            DbParameter[] spParams =
                {
                    new DbParameter("@WebAddressId", DbType.Int32, webAddressId)                   
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.WebAddress_UpdateAsDead", spParams);

            LogManager.Instance.AddToLog(LogMessageType.Information, "WebAddressData", string.Format("UpdateWebAddressAsDead:  webAddressId - {0}", webAddressId));
        }

        /// <summary>
        /// Creates the web address.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The webAddressId</returns>
        public static int CreateWebAddress(string url)
        {
            return WebAddressData.CreateWebAddress(Guid.Empty, url);
        }

        /// <summary>
        /// Creates the web address.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="url">The URL.</param>
        /// <returns>The webAddressId</returns>
        public static int CreateWebAddress(Guid txnId, string url)
        {
            if (WebAddressExists(url))
            {
                throw new ArgumentException(string.Format("url: {0} already exists", url));
            }

            DbParameter[] spParams =
                {
                    new DbParameter("@URL", DbType.StringFixedLength, 300, url),
                    new DbParameter("@WebAddressId", DbType.Int32, ParameterDirection.Output, 0)          
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.WebAddress_Create", spParams, txnId);

            int webAddressId = Convert.ToInt32(spParams[1].Value);

            LogManager.Instance.AddToLog(LogMessageType.Information, "WebAddressData", string.Format("CreateWebAddress: url={0} - new webAddressId - {1}", url, webAddressId));

            return webAddressId;
        }    
    }    
}

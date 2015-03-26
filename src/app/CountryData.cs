using System;
using System.Data;
using Codentia.Common.Data;
using Codentia.Common.Helper;
using Codentia.Common.Membership.Providers;

namespace Codentia.Common.Membership
{
    /// <summary>
    /// This class exposes methods to create, retrieve update and delete Country Data
    /// </summary>
    public static class CountryData
    {
        /// <summary>
        /// Gets the country by id.
        /// </summary>
        /// <param name="countryId">The country id.</param>
        /// <returns>DataTable - Country</returns>
        public static DataTable GetCountryById(int countryId)
        {
            ParameterCheckHelper.CheckIsValidId(countryId, "countryId");

            if (!CountryExists(countryId))
            {
                throw new ArgumentException(string.Format("countryId: {0} does not exist", countryId));
            }

            DbParameter[] spParams =
                {
                    new DbParameter("@CountryId", DbType.Int32, countryId)
                };

            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Country_GetById", spParams);
        }

        /// <summary>
        /// Verify if a given country exists, based on its id
        /// </summary>
        /// <param name="countryId">Id to be checked for existence</param>
        /// <returns>Does Exist</returns>
        public static bool CountryExists(int countryId)
        {
            return CountryExists(Guid.Empty, countryId);
        }

        /// <summary>
        /// Verify if a given country exists, based on its id
        /// </summary>
        /// <param name="txnId">transaction Id</param>
        /// <param name="countryId">Id to be checked for existence</param>
        /// <returns>Does Exist</returns>
        public static bool CountryExists(Guid txnId, int countryId)
        {
            ParameterCheckHelper.CheckIsValidId(countryId, "countryId");

            DbParameter[] spParams =
                {
                    new DbParameter("@CountryId", DbType.Int32, countryId),
                    new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Country_ExistsById", spParams);

            return Convert.ToBoolean(spParams[1].Value);
        }

        /// <summary>
        /// Verify if a given country exists, based on its displayText
        /// </summary>
        /// <param name="displayText">DisplayText to be checked for existence</param>
        /// <returns>Does Exist</returns>
        public static bool CountryExists(string displayText)
        {
            return CountryExists(Guid.Empty, displayText);
        }

        /// <summary>
        /// Verify if a given country exists, based on its displayText
        /// </summary>
        /// <param name="txnId">transaction Id</param>
        /// <param name="displayText">DisplayText to be checked for existence</param>
        /// <returns>Does Exist</returns>
        public static bool CountryExists(Guid txnId, string displayText)
        {
            ParameterCheckHelper.CheckIsValidString(displayText, "displayText", false, true);

            DbParameter[] spParams =
                {
                    new DbParameter("@DisplayText", DbType.StringFixedLength, 50, displayText),
                    new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
                };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Country_ExistsByDisplayText", spParams, txnId);

            return Convert.ToBoolean(spParams[1].Value);
        }

        /// <summary>
        /// Retrieve all available countries
        /// </summary>
        /// <returns>DataTable - Countries</returns>
        public static DataTable GetCountries()
        {
            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Country_GetAll");
        }

        /// <summary>
        /// Retrieve the Id of a country by its displayText.
        /// </summary>
        /// <param name="displayText">DisplayText to search for</param>
        /// <returns>The countryId</returns>
        public static int GetCountryIdByDisplayText(string displayText)
        {
            if (!CountryExists(displayText))
            {
                throw new ArgumentException(string.Format("displayText: {0} does not exist", displayText));
            }

            int countryId = 0;

            DbParameter[] spParams =
            {
                new DbParameter("@DisplayText", DbType.StringFixedLength, 50, displayText),
                new DbParameter("@CountryId", DbType.Int32, ParameterDirection.Output, 0)
            };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Country_GetIdByDisplayText", spParams);
            countryId = Convert.ToInt32(spParams[1].Value);            

            return countryId;
        }

        /// <summary>
        /// Retrieve a specific country's displaytext by its Id
        /// </summary>
        /// <param name="countryId">The CountryId</param>
        /// <returns>string - Country Display Text</returns>
        public static string GetCountryDisplayText(int countryId)
        {
            DataTable dt = GetCountryById(countryId);

            return Convert.ToString(dt.Rows[0]["DisplayText"]);
        }
    }
}

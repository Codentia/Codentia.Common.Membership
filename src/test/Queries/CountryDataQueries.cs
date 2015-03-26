using Codentia.Test;

namespace Codentia.Common.Membership.Test.Queries
{
    /// <summary>
    /// CountryData Queries
    /// </summary>
    [CoverageExclude]
    public static class CountryDataQueries
    {
        /// <summary>
        /// SELECT CountryId, DisplayText FROM dbo.Country ORDER BY DisplayText
        /// </summary>
        public const string Country_Get_All = "SELECT CountryId, DisplayText FROM dbo.Country ORDER BY DisplayText";

        /// <summary>
        /// INSERT INTO dbo.Country (CountryId, DisplayText) VALUES ({0}, '{1}')
        /// </summary>
        public const string Country_Insert = "INSERT INTO dbo.Country (CountryId, DisplayText) VALUES ({0}, '{1}')";

        /// <summary>
        /// SELECT TOP 1 DisplayText FROM Country ORDER BY NEWID()
        /// </summary>
        public const string CountryDisplayText_Get_Random = "SELECT TOP 1 DisplayText FROM Country ORDER BY NEWID()";

        /// <summary>
        /// SELECT DisplayText FROM Country WHERE CountryId={0}
        /// </summary>
        public const string CountryDisplayText_Select = "SELECT DisplayText FROM Country WHERE CountryId={0}";

        /// <summary>
        /// SELECT CountryGroupId FROM dbo.CountryGroup WHERE CountryGroupCode = '{0}'
        /// </summary>
        public const string CountryGroupId_GetByCode = "SELECT CountryGroupId FROM dbo.CountryGroup WHERE CountryGroupCode = '{0}'";

        /// <summary>
        /// SELECT TOP 1 CountryId FROM dbo.Country WHERE CountryId!={0}  ORDER BY NEWID()
        /// </summary>
        public const string CountryId_Get_RandomExceptQuotedId = "SELECT TOP 1 CountryId FROM dbo.Country WHERE CountryId!={0}  ORDER BY NEWID()";

        /// <summary>
        /// SELECT CountryId FROM Address ORDER BY NEWID()
        /// </summary>
        public const string CountryId_Get_RandomFromAddress = "SELECT CountryId FROM Address ORDER BY NEWID()";

        /// <summary>
        /// SELECT CountryId FROM Country WHERE DisplayText = '{0}'
        /// </summary>
        public const string CountryId_SelectByDisplayText = "SELECT CountryId FROM Country WHERE DisplayText = '{0}'";
    }
}

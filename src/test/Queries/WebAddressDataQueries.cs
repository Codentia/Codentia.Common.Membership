using Codentia.Test;

namespace Codentia.Common.Membership.Test.Queries
{
    /// <summary>
    /// WebAddressData Queries
    /// </summary>
    [CoverageExclude]
    public static class WebAddressDataQueries
    {
        /// <summary>
        /// SELECT TOP 1 URL FROM WebAddress ORDER BY NEWID()
        /// </summary>
        public const string URL_Get_Random = "SELECT TOP 1 URL FROM WebAddress ORDER BY NEWID()";

        /// <summary>
        /// INSERT INTO WebAddress (URL) VALUES ('{0}')
        /// </summary>
        public const string WebAddress_Insert = "INSERT INTO WebAddress (URL) VALUES ('{0}')";

        /// <summary>
        /// SELECT WebAddressId, URL, IsDead FROM WebAddress WHERE WebAddressId={0}
        /// </summary>
        public const string WebAddress_SelectById = "SELECT WebAddressId, URL, IsDead FROM WebAddress WHERE WebAddressId={0}";

        /// <summary>
        /// SELECT WebAddressId, URL, IsDead FROM WebAddress WHERE URL='{0}'
        /// </summary>
        public const string WebAddress_SelectByURL = "SELECT WebAddressId, URL, IsDead FROM WebAddress WHERE URL='{0}'";

        /// <summary>
        /// SELECT URL FROM WebAddress WHERE WebAddressId={0}
        /// </summary>
        public const string WebAddress_SelectURLById = "SELECT URL FROM WebAddress WHERE WebAddressId={0}";

        /// <summary>
        /// UPDATE WebAddress SET  IsDead=0 WHERE WebAddressId={0}
        /// </summary>
        public const string WebAddress_UpdateAsAlive = "UPDATE WebAddress SET  IsDead=0 WHERE WebAddressId={0}";

        /// <summary>
        /// SELECT TOP 1 WebAddressId FROM WebAddress WHERE IsDead=0 ORDER BY NEWID()
        /// </summary>
        public const string WebAddressId_Get_Random_Alive = "SELECT TOP 1 WebAddressId FROM WebAddress WHERE IsDead=0 ORDER BY NEWID()";

        /// <summary>
        /// SELECT WebAddressId FROM WebAddress WHERE URL='{0}'
        /// </summary>
        public const string WebAddressId_SelectByURL = "SELECT WebAddressId FROM WebAddress WHERE URL='{0}'";

        /// <summary>
        /// SELECT TOP 1 WebAddressId, URL FROM WebAddress ORDER BY NEWID()
        /// </summary>
        public const string WebAddressIdAndURL_Get_Random = "SELECT TOP 1 WebAddressId, URL FROM WebAddress ORDER BY NEWID()";

        /// <summary>
        /// SELECT WebAddressTypeId, DisplayText FROM dbo.WebAddressType ORDER BY WebAddressTypeId
        /// </summary>
        public const string WebAddressType_Get_All = "SELECT WebAddressTypeId, DisplayText FROM dbo.WebAddressType ORDER BY WebAddressTypeId";
    }
}

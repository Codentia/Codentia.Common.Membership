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
    /// This class exposes methods to create, read, update and delete data related to Addresses.    
    /// </summary>
    public class AddressData
    {
        /// <summary>
        /// Check if a specified addressId exists or not - if local user provided check it exists for user as well
        /// </summary>
        /// <param name="addressId">addressId to check</param>   
        /// <returns>Does Exist</returns>
        public static bool AddressExists(int addressId)
        {
            return AddressExists(Guid.Empty, addressId);
        }

        /// <summary>
        /// Check if a specified addressId exists or not - if local user provided check it exists for user as well
        /// </summary>
        /// <param name="txnId">Transaction Id of ADO.Net Transaction</param>
        /// <param name="addressId">addressId to check</param>   
        /// <returns>Does Exist</returns>
        public static bool AddressExists(Guid txnId, int addressId)
        {
            ParameterCheckHelper.CheckIsValidId(addressId, "addressId");

            DbParameter[] spParams =
            {
                new DbParameter("@AddressId", DbType.Int32, addressId),         
                new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
            };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Address_ExistsById", spParams, txnId);

            return Convert.ToBoolean(spParams[1].Value);
        }

        /// <summary>
        /// Check if a specified addressId exists or not and that a specified emailAddressId is the Id for that address
        /// </summary>
        /// <param name="addressCookie">The address cookie.</param>
        /// <param name="emailAddressCookie">The email address cookie.</param>
        /// <returns>Does Exist</returns>
        public static bool AddressExists(Guid addressCookie, Guid emailAddressCookie)
        {
            ParameterCheckHelper.CheckIsValidGuid(addressCookie, "addressCookie");
            ParameterCheckHelper.CheckIsValidGuid(emailAddressCookie, "emailAddressCookie");

            DbParameter[] spParams =
            {
                new DbParameter("@AddressCookie", DbType.Guid, addressCookie),
                new DbParameter("@EmailAddressCookie", DbType.Guid, emailAddressCookie), 
                new DbParameter("@Exists", DbType.Boolean, ParameterDirection.Output, false)
            };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Address_ExistsByEmailAndAddressIds", spParams);

            return Convert.ToBoolean(spParams[2].Value);
        }

        /// <summary>
        /// Gets the address by id.
        /// </summary>
        /// <param name="addressId">The address id.</param>
        /// <returns>DataTable - Address</returns>
        public static DataTable GetAddressById(int addressId)
        {
            return AddressData.GetAddressById(Guid.Empty, addressId);
        }

        /// <summary>
        /// Gets the address by id.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="addressId">The address id.</param>
        /// <returns>DataTable - Address</returns>
        public static DataTable GetAddressById(Guid txnId, int addressId)
        {
            if (!AddressExists(txnId, addressId))
            {
                throw new ArgumentException(string.Format("addressId: {0} does not exist", addressId));
            }

            DbParameter[] spParams =
            {
                new DbParameter("@AddressId", DbType.Int32, addressId),
                new DbParameter("@AssemblyVersion", DbType.String, 10, Abstraction.Version)
            };

            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Address_GetById", spParams, txnId);
        }

        /// <summary>
        /// Retrieve a specific address by Cookie
        /// </summary>
        /// <param name="addressCookie">Address Cookie</param>
        /// <returns>
        /// DataTable - The Address
        /// </returns>
        public static DataTable GetAddressByCookie(Guid addressCookie)
        {
            // parameter checks

            // valid ids
            ParameterCheckHelper.CheckIsValidGuid(addressCookie, "addressCookie");

            DbParameter[] spParams =
            {
                new DbParameter("@Cookie", DbType.Guid, addressCookie),
                new DbParameter("@AssemblyVersion", DbType.String, 10, Abstraction.Version)
            };

            return DbInterface.ExecuteProcedureDataTable(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Address_GetByCookie", spParams);
        }

        /// <summary>
        /// Create a Country Only Address (other fields are empty strings)
        /// </summary>
        /// <param name="countryId">The country id.</param>
        /// <param name="emailAddressId">All addresses are associated to an emailAddressId</param>
        /// <returns>
        /// The addressId
        /// </returns>
        public static int CreateCountryOnlyAddress(int countryId, int emailAddressId)
        {
            return AddressData.CreateCountryOnlyAddress(Guid.Empty, countryId, emailAddressId);
        }

        /// <summary>
        /// Create a Country Only Address (other fields are empty strings)
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="countryId">The country id.</param>
        /// <param name="emailAddressId">All addresses are associated to an emailAddressId</param>
        /// <returns>
        /// The addressId
        /// </returns>
        public static int CreateCountryOnlyAddress(Guid txnId, int countryId, int emailAddressId)
        {
            ParameterCheckHelper.CheckIsValidId(countryId, "countryId");

            if (!CountryData.CountryExists(countryId))
            {
                throw new ArgumentException(string.Format("countryId: {0} does not exist", countryId));
            }

            return AddressCreate(txnId, true, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, countryId, emailAddressId);
        }

        /// <summary>
        /// Creates the address.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="houseName">Name of the house.</param>
        /// <param name="street">The street.</param>
        /// <param name="town">The town.</param>
        /// <param name="city">The city.</param>
        /// <param name="county">The county.</param>
        /// <param name="postcode">The postcode.</param>
        /// <param name="countryId">The country id.</param>
        /// <param name="emailAddressId">The email address id.</param>
        /// <returns>
        /// The addressId
        /// </returns>
        public static int CreateAddress(string title, string firstName, string lastName, string houseName, string street, string town, string city, string county, string postcode, int countryId, int emailAddressId)
        {
            return AddressCreate(Guid.Empty, false, title, firstName, lastName, houseName, street, town, city, county, postcode, countryId, emailAddressId);
        }

        /// <summary>
        /// Creates the address.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="title">The title.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="houseName">Name of the house.</param>
        /// <param name="street">The street.</param>
        /// <param name="town">The town.</param>
        /// <param name="city">The city.</param>
        /// <param name="county">The county.</param>
        /// <param name="postcode">The postcode.</param>
        /// <param name="countryId">The country id.</param>
        /// <param name="emailAddressId">The email address id.</param>
        /// <returns>
        /// The addressId
        /// </returns>
        public static int CreateAddress(Guid txnId, string title, string firstName, string lastName, string houseName, string street, string town, string city, string county, string postcode, int countryId, int emailAddressId)
        {
            return AddressCreate(txnId, false, title, firstName, lastName, houseName, street, town, city, county, postcode, countryId, emailAddressId);
        }

        /// <summary>
        /// Update an address entry with blank data and just a countryId
        /// </summary>
        /// <param name="addressId">Address Id</param>
        /// <param name="countryId">id of country to which address belongs</param>
        public static void UpdateCountryOnlyAddress(int addressId, int countryId)
        {
            ParameterCheckHelper.CheckIsValidId(addressId, "addressId");
            ParameterCheckHelper.CheckIsValidId(countryId, "countryId");

            if (!AddressExists(addressId))
            {
                throw new ArgumentException(string.Format("addressId: {0} does not exist", addressId));
            }

            if (!CountryData.CountryExists(countryId))
            {
                throw new ArgumentException(string.Format("countryId: {0} does not exist", countryId));
            }

            DataTable dt = GetAddressById(addressId);
            DataRow dr = dt.Rows[0];
            string addressCheck = string.Format("{0}{1}{2}", dr["FirstName"], dr["HouseName"], dr["Street"]);
            if (addressCheck.Length > 0)
            {
                throw new ArgumentException(string.Format("addressId: {0} is not a country only address", addressId));
            }

            AddressUpdate(true, addressId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, countryId);
        }

        /// <summary>
        /// Updates the address.
        /// </summary>
        /// <param name="addressId">The address id.</param>
        /// <param name="title">The title.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="houseName">Name of the house.</param>
        /// <param name="street">The street.</param>
        /// <param name="town">The town.</param>
        /// <param name="city">The city.</param>
        /// <param name="county">The county.</param>
        /// <param name="postcode">The postcode.</param>
        /// <param name="countryId">The country id.</param>
        public static void UpdateAddress(int addressId, string title, string firstName, string lastName, string houseName, string street, string town, string city, string county, string postcode, int countryId)
        {
            AddressUpdate(false, addressId, title, firstName, lastName, houseName, street, town, city, county, postcode, countryId);
        }

        private static int AddressCreate(Guid txnId, bool blanksAllowed, string title, string firstName, string lastName, string houseName, string street, string town, string city, string county, string postcode, int countryId, int emailAddressId)
        {
            int addressId;

            if (!ContactData.EmailAddressExists(txnId, emailAddressId))
            {
                throw new ArgumentException(string.Format("emailAddressId: {0} does not exist", emailAddressId));
            }

            if (!CountryData.CountryExists(txnId, countryId))
            {
                throw new ArgumentException(string.Format("countryId: {0} does not exist", countryId));
            }

            // valid ids
            if (!blanksAllowed)
            {
                ParameterCheckHelper.CheckIsValidString(houseName, "houseName", 100, false);
                ParameterCheckHelper.CheckIsValidString(street, "street", 100, false);
                ParameterCheckHelper.CheckIsValidString(city, "city", 100, false);
                ParameterCheckHelper.CheckIsValidString(county, "county", 100, false);
                ParameterCheckHelper.CheckIsValidString(postcode, "postcode", 15, false);
            }

            // optional parameters
            title = string.IsNullOrEmpty(title) ? string.Empty : title;
            firstName = string.IsNullOrEmpty(firstName) ? string.Empty : firstName;
            lastName = string.IsNullOrEmpty(lastName) ? string.Empty : lastName;
            town = string.IsNullOrEmpty(town) ? string.Empty : town;

            if (title.Length > 0)
            {
                ParameterCheckHelper.CheckStringLengthOnly(title, "title", 100);
            }

            if (firstName.Length > 0)
            {
                ParameterCheckHelper.CheckStringLengthOnly(firstName, "firstName", 150);
            }

            if (lastName.Length > 0)
            {
                ParameterCheckHelper.CheckStringLengthOnly(lastName, "lastName", 150);
            }

            if (town.Length > 0)
            {
                ParameterCheckHelper.CheckStringLengthOnly(town, "town", 100);
            }

            // prepare query and execute
            DbParameter[] spParams =
            {
                new DbParameter("@Title", DbType.StringFixedLength, 25, title),
                new DbParameter("@FirstName", DbType.StringFixedLength, 150, firstName),
                new DbParameter("@LastName", DbType.StringFixedLength, 150, lastName),
                new DbParameter("@HouseName", DbType.StringFixedLength, 100, houseName),
                new DbParameter("@Street", DbType.StringFixedLength, 100, street),
                new DbParameter("@Town", DbType.StringFixedLength, 100, town),
                new DbParameter("@City", DbType.StringFixedLength, 100, city),
                new DbParameter("@County", DbType.StringFixedLength, 100, county),
                new DbParameter("@Postcode", DbType.StringFixedLength, 15, postcode),
                new DbParameter("@CountryId", DbType.Int32, countryId),
                new DbParameter("@EmailAddressId", DbType.Int32, emailAddressId),
                new DbParameter("@AssemblyVersion", DbType.String, 10, Abstraction.Version),
                new DbParameter("@AddressId", DbType.Int32, ParameterDirection.Output, 0)
            };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Address_Create", spParams, txnId);

            addressId = Convert.ToInt32(spParams[spParams.Length - 1].Value);

            LogManager.Instance.AddToLog(LogMessageType.Information, "AddressData", string.Format("CreateAddress: addressId={0}, title={8}, firstName={9}, lastName={10}, houseName={1}, street={2}, town={3}, city={4}, county={5}, postcode={6}, countryId={7}, emailAddressId={11}", addressId, houseName, street, town, city, county, postcode, countryId, title, firstName, lastName, emailAddressId));

            return addressId;
        }

        private static void AddressUpdate(bool blanksAllowed, int addressId, string title, string firstName, string lastName, string houseName, string street, string town, string city, string county, string postcode, int countryId)
        {
            ParameterCheckHelper.CheckIsValidId(addressId, "addressId");
            if (!AddressExists(addressId))
            {
                throw new ArgumentException(string.Format("addressId: {0} does not exist", addressId));
            }

            if (!CountryData.CountryExists(countryId))
            {
                throw new ArgumentException(string.Format("countryId: {0} does not exist", countryId));
            }

            if (!blanksAllowed)
            {
                ParameterCheckHelper.CheckIsValidId(addressId, "addressId");
                ParameterCheckHelper.CheckIsValidString(houseName, "houseName", false);
                ParameterCheckHelper.CheckIsValidString(street, "street", false);
                ParameterCheckHelper.CheckIsValidString(city, "city", false);
                ParameterCheckHelper.CheckIsValidString(county, "county", false);
                ParameterCheckHelper.CheckIsValidString(postcode, "postcode", false);
                ParameterCheckHelper.CheckIsValidId(countryId, "countryId");
            }

            title = string.IsNullOrEmpty(title) ? string.Empty : title;
            firstName = string.IsNullOrEmpty(firstName) ? string.Empty : firstName;
            lastName = string.IsNullOrEmpty(lastName) ? string.Empty : lastName;

            if (town == null)
            {
                town = string.Empty;
            }

            DbParameter[] spParams =
            {
                new DbParameter("@Title", DbType.StringFixedLength, 25, title),
                new DbParameter("@FirstName", DbType.StringFixedLength, 150, firstName),
                new DbParameter("@LastName", DbType.StringFixedLength, 150, lastName),
                new DbParameter("@HouseName", DbType.StringFixedLength, 100, houseName),
                new DbParameter("@Street", DbType.StringFixedLength, 100, street),
                new DbParameter("@Town", DbType.StringFixedLength, 100, town),
                new DbParameter("@City", DbType.StringFixedLength, 100, city),
                new DbParameter("@County", DbType.StringFixedLength, 100, county),
                new DbParameter("@Postcode", DbType.StringFixedLength, 15, postcode),
                new DbParameter("@CountryId", DbType.Int32, countryId),
                new DbParameter("@AddressId", DbType.Int32, addressId),
                new DbParameter("@AssemblyVersion", DbType.String, 10, Abstraction.Version)
            };

            DbInterface.ExecuteProcedureNoReturn(CESqlMembershipProvider.ProviderDbDataSource, "dbo.Address_Update", spParams);

            LogManager.Instance.AddToLog(LogMessageType.Information, "AddressData", string.Format("UpdateAddress: addressId={0}, title={8}, firstName={9}, lastName={10}, houseName={1}, street={2}, town={3}, city={4}, county={5}, postcode={6}, countryId={7}", addressId, houseName, street, town, city, county, postcode, countryId, title, firstName, lastName));
        }
    }
}

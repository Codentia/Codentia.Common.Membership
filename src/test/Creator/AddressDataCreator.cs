using System.Collections.Generic;
using Codentia.Common.Data;
using Codentia.Common.Membership.Test.Queries;
using Codentia.Test;
using Codentia.Test.Generator;
using Codentia.Test.Helper;

namespace Codentia.Common.Membership.Test.Creator
{
    /// <summary>
    /// Common methods for creating Address related data
    /// </summary>
    [CoverageExclude]
    public class AddressDataCreator
    {
        private static string[] _titles = new string[] { "Mr", "Ms", "Mrs", "Dr" };

        /// <summary>
        /// Creates the random address.
        /// </summary>
        /// <param name="database">The database</param>
        /// <param name="nullName">if set to <c>true</c> [null name].</param>
        /// <param name="nullTown">if set to <c>true</c> [null town].</param>
        /// <returns>
        /// int of id
        /// </returns>
        public static int CreateRandomAddress(string database, bool nullName, bool nullTown)
        {
            string title = null;
            string firstName = null;
            string lastName = null;

            string town = null;
            if (!nullName)
            {
                title = _titles[DataGenerator.GenerateRandomInteger(0, _titles.Length - 1)];
                firstName = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), false, false);
                lastName = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), false, false);
            }

            string houseName = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), false, false);
            string street = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            if (!nullTown)
            {
                town = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            }

            string city = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            string county = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            string postcode = DataGenerator.GenerateRandomText(8, false, false);
            int countryId = SqlHelper.GetRandomIdFromTable(database, "Country");
            Dictionary<int, string> dict = InternetDataCreator.CreateRandomEmailAddress(database, 15, "mattchedit.com");
            IEnumerator<int> ie = dict.Keys.GetEnumerator();
            ie.MoveNext();
            int emailAddressId = ie.Current;
            string emailAddress = dict[ie.Current];

            DbInterface.ExecuteQueryNoReturn(database, string.Format(AddressDataQueries.Address_Insert, title, firstName, lastName, houseName, street, town, city, county, postcode, countryId, emailAddressId));

            return SqlHelper.GetMaxIdFromTable(database, "Address");
        }

        /// <summary>
        /// Creates the random address.
        /// </summary>
        /// <param name="nullRecipient">if set to <c>true</c> [null recipient].</param>
        /// <param name="nullTown">if set to <c>true</c> [null town].</param>
        /// <param name="countryId">The country id.</param>
        /// <param name="emailAddressId">The email address id.</param>
        /// <returns>int of id</returns>
        public static int CreateRandomAddress(bool nullRecipient, bool nullTown, int countryId, int emailAddressId)
        {
            string recipient = null;
            string town = null;
            if (!nullRecipient)
            {
                recipient = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), false, false);
            }

            string houseName = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), false, false);
            string street = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            if (!nullTown)
            {
                town = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            }

            string city = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            string county = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            string postcode = DataGenerator.GenerateRandomText(8, false, false);

            DbInterface.ExecuteQueryNoReturn("ecom", string.Format(AddressDataQueries.Address_Insert, recipient, houseName, street, town, city, county, postcode, countryId, emailAddressId));

            return SqlHelper.GetMaxIdFromTable("ecom", "Address");
        }

        /// <summary>
        /// Creates the random application address.
        /// </summary>
        /// <param name="nullRecipient">if set to <c>true</c> [null recipient].</param>
        /// <param name="nullTown">if set to <c>true</c> [null town].</param>
        /// <param name="countryId">The country id.</param>
        /// <param name="emailAddressId">The email address id.</param>
        /// <returns>int of id</returns>
        public static int CreateRandomApplicationAddress(bool nullRecipient, bool nullTown, int countryId, int emailAddressId)
        {
            string recipient = null;
            string town = null;
            if (!nullRecipient)
            {
                recipient = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), false, false);
            }

            string houseName = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), false, false);
            string street = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            if (!nullTown)
            {
                town = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            }

            string city = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            string county = DataGenerator.GenerateRandomText(DataGenerator.GenerateRandomInteger(10, 25), true, false);
            string postcode = DataGenerator.GenerateRandomText(8, false, false);

            DbInterface.ExecuteQueryNoReturn("ecom", string.Format(AddressDataQueries.Address_Insert, recipient, houseName, street, town, city, county, postcode, countryId, emailAddressId));

            return SqlHelper.GetMaxIdFromTable("ecom", "Address");
        }

        /// <summary>
        /// Create CountryOnly Address
        /// </summary>
        /// <param name="database">The database</param>
        /// <param name="countryId">The country id.</param>
        /// <returns>int of id</returns>
        public static int CreateCountryOnlyAddress(string database, int countryId)
        {
            Dictionary<int, string> dict = InternetDataCreator.CreateRandomEmailAddress(database, 15, "mattchedit.com");
            IEnumerator<int> ie = dict.Keys.GetEnumerator();
            ie.MoveNext();
            int emailAddressId = ie.Current;
            string emailAddress = dict[ie.Current];

            DbInterface.ExecuteQueryNoReturn(database, string.Format(AddressDataQueries.Address_CountryOnly_Insert, countryId, emailAddressId));

            return SqlHelper.GetMaxIdFromTable(database, "Address");
        }

        /// <summary>
        /// Creates the country only address.
        /// </summary>
        /// <param name="database">The database</param>
        /// <param name="countryId">The country id.</param>
        /// <param name="emailAddressId">The email address id.</param>
        /// <returns>int of id</returns>
        public static int CreateCountryOnlyAddress(string database, int countryId, int emailAddressId)
        {
            DbInterface.ExecuteQueryNoReturn(database, string.Format(AddressDataQueries.Address_CountryOnly_Insert, countryId, emailAddressId));
            return SqlHelper.GetMaxIdFromTable(database, "Address");
        }

        /// <summary>
        /// Create a set of Addresses
        /// </summary>
        /// <param name="database">The database</param>
        /// <param name="noOfAddresses">The no of addresses.</param>
        public static void CreateRandomAddresses(string database, int noOfAddresses)
        {
            if (DbInterface.ExecuteQueryScalar<int>(database, AddressDataQueries.Address_Count) < noOfAddresses)
            {
                for (int i = 1; i < noOfAddresses; i++)
                {
                    CreateRandomAddress(database, false, false);
                    CreateRandomAddress(database, true, false);
                    CreateRandomAddress(database, false, true);
                    CreateRandomAddress(database, true, true);
                }
            }
        }
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Web.Configuration;
using Codentia.Common.Data;
using Codentia.Common.Logging.BL;
using Codentia.Common.Membership.Test.Creator;
using Codentia.Common.Membership.Test.Queries;
using Codentia.Test.Generator;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Common.Membership.Test
{
    /// <summary>
    /// TestFixture for AddressData
    /// <seealso cref="AddressData"/>
    /// </summary>
    [TestFixture]
    public class AddressDataTest
    {
        /// <summary>
        /// Ensure all set-up required for testing has been completed
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            RoleManagerSection configSection = (RoleManagerSection)config.GetSection("system.web/roleManager");
            ProviderSettings ps = configSection.Providers[0];
            ps.Type = "Codentia.Common.Membership.Providers.CESqlRoleProvider, Codentia.Common.Membership";

            MembershipSection memConfigSection = (MembershipSection)config.GetSection("system.web/membership");
            ps = memConfigSection.Providers[0];
            ps.Type = "Codentia.Common.Membership.Providers.CESqlMembershipProvider, Codentia.Common.Membership";
            config.Save();

            ConfigurationManager.RefreshSection("system.web/roleManager");
            ConfigurationManager.RefreshSection("system.web/membership");

            if (!DbSystem.DoesUserTableHaveData("membership", "Country"))
            {
                DbInterface.ExecuteQueryNoReturn("membership", string.Format(CountryDataQueries.Country_Insert, 1, "England"));
                DbInterface.ExecuteQueryNoReturn("membership", string.Format(CountryDataQueries.Country_Insert, 2, "Eire"));
                DbInterface.ExecuteQueryNoReturn("membership", string.Format(CountryDataQueries.Country_Insert, 3, "Scotland"));
                DbInterface.ExecuteQueryNoReturn("membership", string.Format(CountryDataQueries.Country_Insert, 4, "N Ireland"));
                DbInterface.ExecuteQueryNoReturn("membership", string.Format(CountryDataQueries.Country_Insert, 5, "Wales"));
                DbInterface.ExecuteQueryNoReturn("membership", string.Format(CountryDataQueries.Country_Insert, 6, "Jersey CI"));
                DbInterface.ExecuteQueryNoReturn("membership", string.Format(CountryDataQueries.Country_Insert, 7, "Guernsey CI"));
            }

            SystemUserDataCreator.CreateOnlySystemUsers("membership", 30, 100);
            AddressDataCreator.CreateRandomAddresses("membership", 30);
        }

        /// <summary>
        /// Perform test clean-up activities
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogManager.Instance.Dispose();
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with an Invalid HouseName
        /// Expected: houseName not specified Exception raised
        /// </summary>
        [Test]
        public void _001_CreateAddress_InvalidHouseName()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // null
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", null, "mystreet", "mytown", "mycity", "mycounty", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("houseName is not specified"));

            // empty 
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", string.Empty, "mystreet", "mytown", "mycity", "mycounty", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("houseName is not specified"));

            // exceeds max length
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", DataGenerator.GenerateRandomString(150, null), "mystreet", "mytown", "mycity", "mycounty", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("houseName exceeds maxLength 100 as it has 150 chars"));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with an Invalid street
        /// Expected: street not specified Exception raised
        /// </summary>
        [Test]
        public void _002_CreateAddress_InvalidStreet()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // null
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", null, "mytown", "mycity", "mycountry", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("street is not specified"));

            // empty 
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", string.Empty, "mytown", "mycity", "mycountry", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("street is not specified"));

            // exceeds max length
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", DataGenerator.GenerateRandomString(150, null), "mytown", "mycity", "mycountry", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("street exceeds maxLength 100 as it has 150 chars"));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with an Invalid town
        /// Expected: town not specified Exception raised
        /// </summary>
        [Test]
        public void _003_CreateAddress_EmptyTown()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            int id = AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", null, "mycity", "mycountry", "mypostcode", 1, emailAddressId);
            Assert.That(id, Is.GreaterThan(0), "id must be greater than 0");

            emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            id = AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", null, "mycity", "mycountry", "mypostcode", 1, emailAddressId);
            Assert.That(id, Is.GreaterThan(0), "id must be greater than 0");
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with an Invalid city
        /// Expected: city not specified Exception raised
        /// </summary>
        [Test]
        public void _004_CreateAddress_InvalidCity()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // null
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", null, "mycountry", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("city is not specified"));

            // empty 
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", string.Empty, "mycountry", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("city is not specified"));

            // exceeds max length
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", DataGenerator.GenerateRandomString(150, null), "mycounty", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("city exceeds maxLength 100 as it has 150 chars"));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with an Invalid county
        /// Expected: county not specified Exception raised
        /// </summary>
        [Test]
        public void _005_CreateAddress_InvalidCounty()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // null
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", null, "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("county is not specified"));

            // empty 
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", string.Empty, "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("county is not specified"));

            // exceeds max length
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", DataGenerator.GenerateRandomString(150, null), "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("county exceeds maxLength 100 as it has 150 chars"));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with an Invalid postcode
        /// Expected: postcode not specified Exception raised
        /// </summary>
        [Test]
        public void _006_CreateAddress_InvalidPostcode()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // null
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", null, 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("postcode is not specified"));

            // empty 
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", string.Empty, 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("postcode is not specified"));

            // exceeds max length
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", DataGenerator.GenerateRandomString(20, null), 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("postcode exceeds maxLength 15 as it has 20 chars"));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with an Invalid countryId
        /// Expected: Invalid countryId Exception raised
        /// </summary>
        [Test]
        public void _007_CreateAddress_InvalidCountryId()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // 0
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", 0, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: 0 is not valid"));

            // -1 
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", -1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: -1 is not valid"));

            // non-existant
            int countryIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Country");
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryIdNonExistant, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("countryId: {0} does not exist", countryIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with valid params
        /// Expected: Address created
        /// </summary>
        [Test]
        public void _008_CreateAddress_ValidParams()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            int newAddressId = AddressData.CreateAddress(null, null, null, "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryId, emailAddressId);
            Assert.That(newAddressId, Is.GreaterThan(0), "_newAddressId must be greater than 0");

            countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            newAddressId = AddressData.CreateAddress(string.Empty, string.Empty, string.Empty, "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryId, emailAddressId);
            Assert.That(newAddressId, Is.GreaterThan(0), "_newAddressId must be greater than 0");

            countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            newAddressId = AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryId, emailAddressId);
            Assert.That(newAddressId, Is.GreaterThan(0), "_newAddressId must be greater than 0");

            // txn
            Guid txnId = Guid.NewGuid();

            countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            try
            {
                newAddressId = AddressData.CreateAddress(txnId, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryId, emailAddressId);
                DbInterface.CommitTransaction(txnId);
            }
            catch (Exception ex)
            {
                DbInterface.RollbackTransaction(txnId);
                Assert.Fail(ex.Message);
            }

            Assert.That(newAddressId, Is.GreaterThan(0), "_newAddressId must be greater than 0");
        }

        /// <summary>
        /// Scenario: Attempt to call AddressExists with an Invalid AddressId
        /// Expected: Invalid AddressId Exception raised
        /// </summary>
        [Test]
        public void _009_AddressExists_Id_InvalidId()
        {
            // 0
            Assert.That(delegate { bool retval = AddressData.AddressExists(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { bool retval = AddressData.AddressExists(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressId: -1 is not valid"));
        }

        /// <summary>
        /// Scenario: Attempt to call AddressExists with an existing Address
        /// Expected: Table row count of 1
        /// </summary>
        [Test]
        public void _010_AddressExists_Id_ExistingAddress()
        {
            Assert.That(AddressData.AddressExists(SqlHelper.GetRandomIdFromTable("membership", "Address")), Is.True);

            // txnl
            Guid txnId = Guid.NewGuid();
            try
            {
                Assert.That(AddressData.AddressExists(txnId, SqlHelper.GetRandomIdFromTable("membership", "Address")), Is.True);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }
        }

        /// <summary>
        /// Scenario: Attempt to call GetAddressById with an Invalid AddressId
        /// Expected: Invalid AddressId Exception raised
        /// </summary>
        [Test]
        public void _011_AddressExists_Id_NonExistantAddress()
        {
            int systemUserId = SystemUserDataCreator.CreateSystemUser("membership", 10);

            Assert.That(AddressData.AddressExists(SqlHelper.GetUnusedIdFromTable("membership", "Address")), Is.False);

            // txnl
            Guid txnId = Guid.NewGuid();
            try
            {
                Assert.That(AddressData.AddressExists(txnId, SqlHelper.GetUnusedIdFromTable("membership", "Address")), Is.False);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }
        }

        /// <summary>
        /// Scenario: Attempt to call GetAddressByCookie with an empty Guid
        /// Expected: Empty Guid Exception raised
        /// </summary>
        [Test]
        public void _012_GetAddressByCookie_EmptyGuid()
        {
            Assert.That(delegate { AddressData.GetAddressByCookie(Guid.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressCookie is an empty Guid"));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with an Invalid HouseName
        /// Expected: houseName not specified Exception raised
        /// </summary>
        [Test]
        public void _014_UpdateAddress_InvalidHouseName()
        {
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");

            // null
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", null, "mystreet", "mytown", "mycity", "mycountry", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("houseName is not specified"));

            // empty 
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", null, "mystreet", "mytown", "mycity", "mycountry", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("houseName is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with an Invalid street
        /// Expected: street not specified Exception raised
        /// </summary>
        [Test]
        public void _015_UpdateAddress_InvalidStreet()
        {
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");

            // null
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", null, "mytown", "mycity", "mycountry", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("street is not specified"));

            // empty 
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", string.Empty, "mytown", "mycity", "mycountry", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("street is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with an Invalid town
        /// Expected: town not specified Exception raised
        /// </summary>
        [Test]
        public void _016_UpdateAddress_EmptyTown()
        {
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");

            AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", null, "mycity", "mycountry", "mypostcode", 1);
            AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", string.Empty, "mycity", "mycountry", "mypostcode", 1);
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with an Invalid city
        /// Expected: city not specified Exception raised
        /// </summary>
        [Test]
        public void _017_UpdateAddress_InvalidCity()
        {
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");

            // null
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", "mytown", null, "mycountry", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("city is not specified"));

            // empty 
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", "mytown", string.Empty, "mycountry", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("city is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with an Invalid county
        /// Expected: county not specified Exception raised
        /// </summary>
        [Test]
        public void _018_UpdateAddress_InvalidCounty()
        {
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");

            // null
            Assert.That(delegate { AddressData.UpdateAddress(addressId, null, null, null, "myhouse", "mystreet", "mytown", "mycity", null, "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("county is not specified"));

            // empty 
            Assert.That(delegate { AddressData.UpdateAddress(addressId, null, null, null, "myhouse", "mystreet", "mytown", "mycity", string.Empty, "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("county is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with an Invalid postcode
        /// Expected: postcode not specified Exception raised
        /// </summary>
        [Test]
        public void _019_UpdateAddress_InvalidPostcode()
        {
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");

            // null
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", null, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("postcode is not specified"));

            // empty 
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", string.Empty, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("postcode is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with an Invalid countryId
        /// Expected: Invalid countryId Exception raised
        /// </summary>
        [Test]
        public void _020_UpdateAddress_InvalidCountryId()
        {
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");

            // 0
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", 0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: 0 is not valid"));

            // -1
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", -1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: -1 is not valid"));

            // non-existant
            int countryIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Country");
            Assert.That(delegate { AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("countryId: {0} does not exist", countryIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with an Invalid addressId
        /// Expected: Invalid countryId Exception raised
        /// </summary>
        [Test]
        public void _021_UpdateAddress_InvalidAddressId()
        {
            // 0
            Assert.That(delegate { AddressData.UpdateAddress(0, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressId: 0 is not valid"));

            // -1
            Assert.That(delegate { AddressData.UpdateAddress(-1, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressId: -1 is not valid"));

            // non-existant
            int addressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Address");
            Assert.That(delegate { AddressData.UpdateAddress(addressIdNonExistant, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("addressId: {0} does not exist", addressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateAddress with valid params
        /// Expected: Address created
        /// </summary>
        [Test]
        public void _022_UpdateAddress_ValidParams()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");

            AddressData.UpdateAddress(addressId, null, null, null, "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryId);
            AddressData.UpdateAddress(addressId, string.Empty, string.Empty, string.Empty, "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryId);
            AddressData.UpdateAddress(addressId, "mr", "test", "name", "myhouse", "mystreet", "mytown", "mycity", "mycounty", "mypostcode", countryId);

            // *** needs fixing
            DataTable dt = AddressData.GetAddressById(addressId);

            Assert.That(Convert.ToString(dt.Rows[0]["HouseName"]), Is.EqualTo("myhouse"), "Update did not work");
        }

        /// <summary>
        /// Scenario: Attempt to call CreateCountryOnlyAddress with an invalid countryId
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _023_CreateCountryOnlyAddress_InvalidCountryId()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // 0
            Assert.That(delegate { AddressData.CreateCountryOnlyAddress(0, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: 0 is not valid"));

            // -1
            Assert.That(delegate { AddressData.CreateCountryOnlyAddress(-1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: -1 is not valid"));

            // non-existant
            int countryIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Country");
            Assert.That(delegate { AddressData.CreateCountryOnlyAddress(countryIdNonExistant, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("countryId: {0} does not exist", countryIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateCountryOnlyAddress with an invalid countryId
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _024_CreateCountryOnlyAddress_InvalidEmailAddressId()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");

            // 0
            Assert.That(delegate { AddressData.CreateCountryOnlyAddress(countryId, 0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: 0 is not valid"));

            // -1
            Assert.That(delegate { AddressData.CreateCountryOnlyAddress(countryId, -1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: -1 is not valid"));

            // non-existant
            int emailAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
            Assert.That(delegate { AddressData.CreateCountryOnlyAddress(countryId, emailAddressIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("emailAddressId: {0} does not exist", emailAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateCountryOnlyAddress with valid countryId
        /// Expected: Address created
        /// </summary>
        [Test]
        public void _025_CreateCountryOnlyAddress_ValidParams()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            int addressId = AddressData.CreateCountryOnlyAddress(countryId, emailAddressId);

            Assert.That(addressId, Is.GreaterThan(0));
            Assert.That(addressId, Is.EqualTo(SqlHelper.GetMaxIdFromTable("membership", "Address")));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateCountryOnlyAddress with invalid addressId
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _026_UpdateCountryOnlyAddress_InvalidAddressId()
        {
            // 0
            Assert.That(delegate { AddressData.UpdateCountryOnlyAddress(0, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressId: 0 is not valid"));

            // -1
            Assert.That(delegate { AddressData.UpdateCountryOnlyAddress(-1, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressId: -1 is not valid"));

            // non-existant
            int addressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Address");
            Assert.That(delegate { AddressData.UpdateCountryOnlyAddress(addressIdNonExistant, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("addressId: {0} does not exist", addressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateCountryOnlyAddress with non country only addressId
        /// Expected: Exception raised
        /// </summary>
        [Test]
        public void _027_UpdateCountryOnlyAddress_NonCountryOnlyAddress()
        {
            int nonCountryOnlyAddressId = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.AddressId_Get_NonCountryOnlyAddress);
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            Assert.That(delegate { AddressData.UpdateCountryOnlyAddress(nonCountryOnlyAddressId, countryId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("addressId: {0} is not a country only address", nonCountryOnlyAddressId)));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateCountryOnlyAddress with invalid countryId
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _028_UpdateCountryOnlyAddress_InvalidCountryId()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            DbInterface.ExecuteQueryNoReturn("membership", string.Format(AddressDataQueries.Address_CountryOnly_Insert, countryId, emailAddressId));
            int addressId = SqlHelper.GetMaxIdFromTable("membership", "Address");

            // 0
            Assert.That(delegate { AddressData.UpdateCountryOnlyAddress(addressId, 0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: 0 is not valid"));

            // -1
            Assert.That(delegate { AddressData.UpdateCountryOnlyAddress(addressId, -1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: -1 is not valid"));

            // non-existant
            int nonExistantCountryId = SqlHelper.GetUnusedIdFromTable("membership", "Country");
            Assert.That(delegate { AddressData.UpdateCountryOnlyAddress(addressId, nonExistantCountryId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("countryId: {0} does not exist", nonExistantCountryId)));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateCountryOnlyAddress with valid countryId
        /// Expected: Address created
        /// </summary>
        [Test]
        public void _029_UpdateCountryOnlyAddress_ValidParams()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.AddressId_Get_CountryOnlyAddress);
            int existingCountryId = Convert.ToInt32(dt.Rows[0]["CountryId"]);
            int addressId = Convert.ToInt32(dt.Rows[0]["AddressId"]);

            int newCountryId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(CountryDataQueries.CountryId_Get_RandomExceptQuotedId, existingCountryId));

            AddressData.UpdateCountryOnlyAddress(addressId, newCountryId);

            DataTable dt2 = AddressData.GetAddressById(addressId);
            Assert.That(newCountryId, Is.EqualTo(Convert.ToInt32(dt2.Rows[0]["CountryId"])));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateAddress with an Invalid town
        /// Expected: town not specified Exception raised
        /// </summary>
        [Test]
        public void _030_CreateAddress_TownExceedsMaxLength()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // exceeds max length
            Assert.That(delegate { AddressData.CreateAddress("mr", "test", "name", "myhouse", "mystreet", DataGenerator.GenerateRandomString(150, null), "mycity", "mycountry", "mypostcode", 1, emailAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("town exceeds maxLength 100 as it has 150 chars"));
        }

        /// <summary>
        /// Scenario: Attempt to call GetAddressById with invalid addressId
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _031_GetAddressById_InvalidAddressId()
        {
            // 0
            Assert.That(delegate { AddressData.GetAddressById(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressId: 0 is not valid"));

            // -1
            Assert.That(delegate { AddressData.GetAddressById(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressId: -1 is not valid"));

            // non-existant
            int nonExistantAddressId = SqlHelper.GetUnusedIdFromTable("membership", "Address");
            Assert.That(delegate { AddressData.GetAddressById(nonExistantAddressId); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("addressId: {0} does not exist", nonExistantAddressId)));
        }

        /// <summary>
        /// Scenario: Attempt to call AddressExists with an Invalid AddressId
        /// Expected: Invalid AddressId Exception raised
        /// </summary>
        [Test]
        public void _032_AddressExists_MultipleIds_InvalidAddressCookie()
        {
            Assert.That(delegate { bool retval = AddressData.AddressExists(Guid.Empty, Guid.NewGuid()); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("addressCookie is an empty Guid"));
        }

        /// <summary>
        /// Scenario: Attempt to call AddressExists with an Invalid AddressId
        /// Expected: Invalid AddressId Exception raised
        /// </summary>
        [Test]
        public void _033_AddressExists_MultipleIds_InvalidEmailAddressCookie()
        {
            Assert.That(delegate { bool retval = AddressData.AddressExists(Guid.NewGuid(), Guid.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressCookie is an empty Guid"));
        }

        /// <summary>
        /// Scenario: Attempt to call AddressExists with an existing Address
        /// Expected: Table row count of 1
        /// </summary>
        [Test]
        public void _034_AddressExists_Id_CombinationExists()
        {
            Guid addressCookie = DbInterface.ExecuteQueryScalar<Guid>("membership", AddressDataQueries.Cookie_Get_Random);
            Guid emailAddressCookie = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(AddressDataQueries.ConfirmGuid_Get_ForAddressCookie, addressCookie));

            Assert.That(AddressData.AddressExists(addressCookie, emailAddressCookie), Is.True);
        }

        /// <summary>
        /// Scenario: Attempt to call GetAddressById with an Invalid AddressId
        /// Expected: Invalid AddressId Exception raised
        /// </summary>
        [Test]
        public void _035_AddressExists_Id_CombinationDoesNotExist()
        {
            Guid addressCookie = DbInterface.ExecuteQueryScalar<Guid>("membership", AddressDataQueries.Cookie_Get_Random);
            Assert.That(AddressData.AddressExists(addressCookie, Guid.NewGuid()), Is.False);
        }
    }
}

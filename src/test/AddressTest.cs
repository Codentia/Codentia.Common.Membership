using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
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
    /// This class is the unit testing fixture for the Address business entity.
    /// <seealso cref="AddressData"/>
    /// </summary>
    [TestFixture]
    public class AddressTest
    {
        private int _addressIdNonExistant;

        /// <summary>
        /// Ensure all data required during test is set up
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            SystemUserDataCreator.CreateOnlySystemUsers("membership", 30, 300);
            InternetDataCreator.CreateOnlyEmailAddresses("membership", 20);
            AddressDataCreator.CreateRandomAddresses("membership", 30);

            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            _addressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Address");
        }

        /// <summary>
        /// Ensure all data entered during test is cleared
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            // validate data
            Assert.That(DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.Address_Get_AddressWithoutExistingCountries), Is.EqualTo(0), "Found addresses with countries that are not in Country");
            LogManager.Instance.Dispose();
        }

        /// <summary>
        /// Scenario: Construct object by valid Id (retrieve only)
        /// Expected: Completes without error
        /// </summary>
        [Test]
        public void _001_Constructor_ById_ValidId()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No address data exists to test with");
            int addressId = SqlHelper.GetRandomIdFromTable("membership", "Address");
            Address a = new Address(addressId);
            Assert.That(a.AddressId, Is.EqualTo(addressId), "AddressId is incorrect");
        }

        /// <summary>
        /// Scenario: Construct object by valid Id (retrieve only)
        /// Expected: Completes without error
        /// </summary>
        [Test]
        public void _002_CreateAddress_InvalidCountryId()
        {
            // null  
            Assert.That(delegate { Address a = Address.CreateAddress("mr", "bl", "tester", "validhousename", "validstreet", "validtown", "validcity", "validcounty", "PO7 8RU", null, "test009@mattchedit.com"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("country: was not specified"));
        }

        /// <summary>
        /// Scenario: Construct object with all address strings
        /// Expected: Completes without error
        /// </summary>
        [Test]
        public void _003_CreateAddress_ValidParams()
        {
            string houseName = string.Format("myhouse{0}", Guid.NewGuid());
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");

            Address a = Address.CreateAddress("mr", "bl", "tester", houseName, "validstreet", "validtown", "validcity", "validcounty", "PO7 8RU", new Country(countryId), "test010@mattchedit.com");
            Assert.That(a, Is.Not.Null, "Address should not be null");
            Assert.That(a.IsCountryOnlyAddress, Is.False);
            Assert.That(a.AddressId, Is.GreaterThan(0), "Addressid should be a valid Id");

            countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            Address a2 = Address.CreateAddress(null, null, null, houseName, "validstreet", "validtown", "validcity", "validcounty", "PO7 8RU", new Country(countryId), "test010@mattchedit.com");
            Assert.That(a2, Is.Not.Null, "Address should not be null");
            Assert.That(a2.IsCountryOnlyAddress, Is.False);
            Assert.That(a2.AddressId, Is.GreaterThan(0), "Addressid should be a valid Id");

            // txn
            Guid txnId = Guid.NewGuid();

            countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            Address a3 = null;
            try
            {
                a3 = Address.CreateAddress(txnId, null, null, null, houseName, "validstreet", "validtown", "validcity", "validcounty", "PO7 8RU", new Country(countryId), "test010@mattchedit.com");
                DbInterface.CommitTransaction(txnId);
            }
            catch (Exception ex)
            {
                DbInterface.RollbackTransaction(txnId);
                Assert.Fail(ex.Message);
            }

            Assert.That(a3, Is.Not.Null, "Address should not be null");
            Assert.That(a3.IsCountryOnlyAddress, Is.False);
            Assert.That(a3.AddressId, Is.GreaterThan(0), "Addressid should be a valid Id");
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _004_CountryId()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.Country.CountryId, Is.EqualTo(Convert.ToInt32(dt.Rows[i]["CountryId"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _005_Name()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.Title, Is.EqualTo(Convert.ToString(dt.Rows[i]["Title"])), string.Format("Property incorrect for row {0}", i));
                Assert.That(a.FirstName, Is.EqualTo(Convert.ToString(dt.Rows[i]["FirstName"])), string.Format("Property incorrect for row {0}", i));
                Assert.That(a.LastName, Is.EqualTo(Convert.ToString(dt.Rows[i]["LastName"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _006_HouseName()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.HouseName, Is.EqualTo(Convert.ToString(dt.Rows[i]["HouseName"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _007_Street()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.Street, Is.EqualTo(Convert.ToString(dt.Rows[i]["Street"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _008_Town()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.Town, Is.EqualTo(Convert.ToString(dt.Rows[i]["Town"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _009_City()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.City, Is.EqualTo(Convert.ToString(dt.Rows[i]["City"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _010_County()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.County, Is.EqualTo(Convert.ToString(dt.Rows[i]["County"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _011_Postcode()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.Postcode, Is.EqualTo(Convert.ToString(dt.Rows[i]["PostCode"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _012_Cookie()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.Cookie, Is.EqualTo(new Guid(Convert.ToString(dt.Rows[i]["Cookie"]))), string.Format("Property incorrect for row {0}", i));
                Address a2 = new Address(new Guid(Convert.ToString(dt.Rows[i]["Cookie"])));
                Assert.That(a2.AddressId, Is.EqualTo(a.AddressId), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _013_EmailAddressId()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Address"), Is.True, "No Address data exists to test with");
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random10);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Address a = new Address(Convert.ToInt32(dt.Rows[i]["AddressId"]));
                Assert.That(a.Contact.EmailAddressId, Is.EqualTo(Convert.ToInt32(dt.Rows[i]["EmailAddressId"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Get an address using email and address cookies
        /// Expected: address returned correctly
        /// </summary>
        [Test]
        public void _014_Constructor_Context_EmailAddressCookieBlank()
        {
            Guid addressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(AddressDataQueries.Cookie_Get_Random));
            Address addToCheck = new Address(addressGuid);

            HttpContext hc = HttpHelper.CreateHttpContext(string.Empty);

            // set emailaddress cookie
            string emailAddressCookieName = "TestEmailCookie";
            HttpCookie cookieEmail = new HttpCookie(emailAddressCookieName);
            cookieEmail.Value = string.Empty;
            hc.Request.Cookies.Add(cookieEmail);

            // set address cookie
            string addressCookieName = "TestAddrCookie";
            HttpCookie cookieAddress = new HttpCookie(addressCookieName);
            cookieAddress.Value = addressGuid.ToString();
            hc.Request.Cookies.Add(cookieAddress);

            Assert.That(delegate { Address a = new Address(hc, addressCookieName, emailAddressCookieName); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("A valid emailaddress cannot be found, empty or missing Guid"));
        }

        /// <summary>
        /// Scenario: Get an address using email and address cookies
        /// Expected: address returned correctly
        /// </summary>
        [Test]
        public void _015_Constructor_Context_AddressCookieBlank()
        {
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_NotInSystemUser_EmailAddress_Get_Random);

            Guid addressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(AddressDataQueries.Cookie_Get_Random));
            Address addToCheck = new Address(addressGuid);

            HttpContext hc = HttpHelper.CreateHttpContext(string.Empty);

            // set emailaddress cookie
            string emailAddressCookieName = "TestEmailCookie";

            HttpCookie cookieEmail = new HttpCookie(emailAddressCookieName);
            cookieEmail.Value = emailAddressGuid.ToString();
            hc.Request.Cookies.Add(cookieEmail);

            // set address cookie
            string addressCookieName = "TestAddrCookie";
            HttpCookie cookieAddress = new HttpCookie(addressCookieName);
            cookieAddress.Value = string.Empty;
            hc.Request.Cookies.Add(cookieAddress);

            Assert.That(delegate { Address a = new Address(hc, addressCookieName, emailAddressCookieName); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("A valid address cannot be found, empty or missing Guid"));
        }

        /// <summary>
        /// Scenario: Get an address using email and address cookies
        /// Expected: address returned correctly
        /// </summary>
        [Test]
        public void _016_Constructor_Context_ValidGuidsButMismatched()
        {
            Dictionary<int, string> dict = InternetDataCreator.CreateRandomEmailAddress("membership", 15, "mattchedit.com");

            Guid addressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", AddressDataQueries.Cookie_Get_Random);
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(AddressDataQueries.ConfirmGuid_Get_NotForAddressCookie, addressGuid));

            HttpContext hc = HttpHelper.CreateHttpContext(string.Empty);

            // set emailaddress cookie
            string emailAddressCookieName = "TestEmailCookie";
            HttpCookie cookieEmail = new HttpCookie(emailAddressCookieName);
            cookieEmail.Value = emailAddressGuid.ToString();
            hc.Request.Cookies.Add(cookieEmail);

            // set address cookie
            string addressCookieName = "TestAddrCookie";
            HttpCookie cookieAddress = new HttpCookie(addressCookieName);
            cookieAddress.Value = addressGuid.ToString();
            hc.Request.Cookies.Add(cookieAddress);

            Assert.That(delegate { Address a = new Address(hc, addressCookieName, emailAddressCookieName); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("Cookie mismatch: address record for addressGuid={0} does not match email address record for emailAddressGuid={1}", addressGuid.ToString(), emailAddressGuid.ToString())));
        }

        /// <summary>
        /// Scenario: Get an address using email and address cookies
        /// Expected: address returned correctly
        /// </summary>
        [Test]
        public void _017_Constructor_Context_ValidGuidsMatched()
        {
            Dictionary<int, string> dict = InternetDataCreator.CreateRandomEmailAddress("membership", 15, "mattchedit.com");
            Guid addressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", AddressDataQueries.Cookie_Get_Random);
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(AddressDataQueries.ConfirmGuid_Get_ForAddressCookie, addressGuid));

            Address addToCheck = new Address(addressGuid);

            HttpContext hc = HttpHelper.CreateHttpContext(string.Empty);

            // set emailaddress cookie
            string emailAddressCookieName = "TestEmailCookie";
            HttpCookie cookieEmail = new HttpCookie(emailAddressCookieName);
            cookieEmail.Value = emailAddressGuid.ToString();
            hc.Request.Cookies.Add(cookieEmail);

            // set address cookie
            string addressCookieName = "TestAddrCookie";
            HttpCookie cookieAddress = new HttpCookie(addressCookieName);
            cookieAddress.Value = addressGuid.ToString();
            hc.Request.Cookies.Add(cookieAddress);

            Address a = new Address(hc, addressCookieName, emailAddressCookieName);

            Assert.That(addToCheck.AddressId, Is.EqualTo(a.AddressId), "Address retrieval not correct (AddressId)");
            Assert.That(addToCheck.HouseName, Is.EqualTo(a.HouseName), "Address retrieval not correct (HouseName)");
            Assert.That(addToCheck.Street, Is.EqualTo(a.Street), "Address retrieval not correct (Street)");
            Assert.That(addToCheck.Town, Is.EqualTo(a.Town), "Address retrieval not correct (Town)");
            Assert.That(addToCheck.City, Is.EqualTo(a.City), "Address retrieval not correct (City)");
            Assert.That(addToCheck.County, Is.EqualTo(a.County), "Address retrieval not correct (County)");
            Assert.That(addToCheck.Postcode, Is.EqualTo(a.Postcode), "Address retrieval not correct (Postcode)");
            Assert.That(addToCheck.Country.CountryId, Is.EqualTo(a.Country.CountryId), "Address retrieval not correct (CountryId)");
            Assert.That(addToCheck.Cookie, Is.EqualTo(a.Cookie), "Address retrieval not correct (Cookie)");
            Assert.That(addToCheck.Cookie, Is.EqualTo(addressGuid), "Address retrieval not correct (Cookie)");

            Assert.That(a, Is.Not.Null);
        }

        /// <summary>
        /// Scenario: Get an address using email that is associated to a system user
        /// Expected: system user exception raised
        /// </summary>
        [Test]
        public void _018_Constructor_DataRow()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", AddressDataQueries.Address_Get_Random_NotNullCookie);
            int addressId = Convert.ToInt32(dt.Rows[0]["AddressId"]);

            Address add = new Address(dt.Rows[0]);

            Assert.That(add.City, Is.EqualTo(Convert.ToString(dt.Rows[0]["City"])));
            Assert.That(add.Postcode, Is.EqualTo(Convert.ToString(dt.Rows[0]["PostCode"])));
        }

        /// <summary>
        /// Scenario: Get an address using email that is associated to a system user
        /// Expected: system user exception raised
        /// </summary>
        [Test]
        public void _019_ConcatenateAddress_PostCode()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            string country = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(CountryDataQueries.CountryDisplayText_Select, countryId));
            string email = string.Format("{0}@mattchedit.com", Guid.NewGuid().ToString());

            // full address
            Address full = Address.CreateAddress("Mr", "Bob", "McTest", "House", "Street", "Town", "City", "County", "Postcode", new Country(country), email);
            Assert.That(full.ConcatenateAddress(",", true), Is.EqualTo(string.Format("Mr Bob McTest,House,Street,Town,City,County,Postcode,{0}", country)));

            // country only address
            Contact contact = Contact.CreateContact(email);
            Address countryOnly = Address.CreateAddress(Guid.Empty, new Country(country).CountryId, contact.EmailAddressId);
            Assert.That(countryOnly.ConcatenateAddress(",", true), Is.EqualTo(country));
        }

        /// <summary>
        /// Scenario: Get an address with no name and concatenate it
        /// Expected: Correctly concatenated address
        /// </summary>
        [Test]
        public void _020_ConcatenateAddress_NoName()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            string country = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(CountryDataQueries.CountryDisplayText_Select, countryId));

            Address full = Address.CreateAddress(string.Empty, string.Empty, string.Empty, "House", "Street", "Town", "City", "County", "Postcode", new Country(country), "test019@mattchedit.com");
            Assert.That(full.ConcatenateAddress(",", true), Is.EqualTo(string.Format("House,Street,Town,City,County,Postcode,{0}", country)));
        }

        /// <summary>
        /// Scenario: Get an address with no town and concatenate it
        /// Expected: Correctly concatenated address (with no extra delimiter)
        /// </summary>
        [Test]
        public void _022_ConcatenateAddress_NoTown()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            string country = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(CountryDataQueries.CountryDisplayText_Select, countryId));

            // full address
            Address full = Address.CreateAddress("Mr", "Bob", "McTest", "House", "Street", string.Empty, "City", "County", "Postcode", new Country(country), "test019@mattchedit.com");
            Assert.That(full.ConcatenateAddress(",", true), Is.EqualTo(string.Format("Mr Bob McTest,House,Street,City,County,Postcode,{0}", country)));
        }

        /// <summary>
        /// Scenario: An address is updated. IncludeName is set to false (optional) and name is not specified.
        /// Expected: Completes without error
        /// </summary>
        [Test]
        public void _023_UpdatefromAnotherAddress_User_NoName()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            string country = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(CountryDataQueries.CountryDisplayText_Select, countryId));

            Address source = Address.CreateAddress(string.Empty, string.Empty, string.Empty, "House", "Street", string.Empty, "City", "County", "Postcode", new Country(country), "test019@mattchedit.com");
            Address target = Address.CreateAddress("Mr", "Bob", "McTest", "House", "Street", string.Empty, "City", "County", "Postcode", new Country(country), "test019@mattchedit.com");

            target.UpdateFromAnotherAddress(source);
            Assert.That(target.Title, Is.EqualTo(string.Empty));
            Assert.That(target.FirstName, Is.EqualTo(string.Empty));
            Assert.That(target.LastName, Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// Scenario: An address is updated. IncludeName is set to false (optional) and town is not specified.
        /// Expected: Completes without error
        /// </summary>
        [Test]
        public void _024_UpdatefromAnotherAddress_User_NoTown()
        {
            int addressId = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.AddressId_Get_NonCountryOnlyAddress);
            int addressId2 = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.AddressId_Get_NonCountryOnlyAddress);
            Address x = new Address(addressId);
            x.Town = null;
            Address x2 = new Address(addressId2);
            x2.UpdateFromAnotherAddress(x);
        }

        /// <summary>
        /// Scenario: Attempt to call Address(region, countryid) with valid params
        /// Expected: Raises Exceptions
        /// </summary>
        [Test]
        public void _025_CreateAddress_CountryOnly_ValidParams()
        {
            int countryIdExistant = SqlHelper.GetRandomIdFromTable("membership", "Country");
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            Address a1 = Address.CreateAddress(Guid.Empty, countryIdExistant, emailAddressId);
            Assert.That(a1, Is.Not.Null, "Address should not be null");
            Assert.That(a1.IsCountryOnlyAddress, Is.True);

            Assert.That(a1.AddressId, Is.GreaterThan(0));
            DataTable dt1 = DbInterface.ExecuteQueryDataTable("membership", string.Format(AddressDataQueries.Address_Select_ByAddressId, a1.AddressId));
            Assert.That(dt1.Rows.Count, Is.EqualTo(1));

            Assert.That(Convert.ToString(dt1.Rows[0]["Title"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["FirstName"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["LastName"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["HouseName"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["Street"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["Town"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["City"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["County"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["Postcode"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToUInt32(dt1.Rows[0]["CountryId"]), Is.EqualTo(countryIdExistant));

            countryIdExistant = SqlHelper.GetRandomIdFromTable("membership", "Country");
            emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            Address a2 = Address.CreateAddress(Guid.Empty, countryIdExistant, emailAddressId);
            Assert.That(a2, Is.Not.Null, "Address should not be null");
            Assert.That(a2.IsCountryOnlyAddress, Is.True);
            Assert.That(a2.AddressId, Is.GreaterThan(0));
            DataTable dt2 = DbInterface.ExecuteQueryDataTable("membership", string.Format(AddressDataQueries.Address_Select_ByAddressId, a2.AddressId));
            Assert.That(dt2.Rows.Count, Is.EqualTo(1));

            Assert.That(Convert.ToString(dt1.Rows[0]["Title"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["FirstName"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt1.Rows[0]["LastName"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt2.Rows[0]["HouseName"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt2.Rows[0]["Street"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt2.Rows[0]["Town"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt2.Rows[0]["City"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt2.Rows[0]["County"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToString(dt2.Rows[0]["Postcode"]), Is.EqualTo(string.Empty));
            Assert.That(Convert.ToUInt32(dt2.Rows[0]["CountryId"]), Is.EqualTo(countryIdExistant));
        }

        /// <summary>
        /// Scenario: Attempt to call Address Update(region, countryid) with valid params
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _026_Update_CountryOnlyAddress()
        {
            int addressId = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.AddressId_Get_CountryOnlyAddress);
            Address a = new Address(addressId);
            int oldCountryId = a.Country.CountryId;

            int newCountryId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(CountryDataQueries.CountryId_Get_RandomExceptQuotedId, oldCountryId));

            a.Country = new Country(newCountryId);

            Address a1 = new Address(addressId);

            Assert.That(a1.Title, Is.EqualTo(string.Empty));
            Assert.That(a1.FirstName, Is.EqualTo(string.Empty));
            Assert.That(a1.LastName, Is.EqualTo(string.Empty));
            Assert.That(a1.HouseName, Is.EqualTo(string.Empty));
            Assert.That(a1.Street, Is.EqualTo(string.Empty));
            Assert.That(a1.Town, Is.EqualTo(string.Empty));
            Assert.That(a1.City, Is.EqualTo(string.Empty));
            Assert.That(a1.County, Is.EqualTo(string.Empty));
            Assert.That(a1.Postcode, Is.EqualTo(string.Empty));
            Assert.That(a1.Country.CountryId, Is.Not.EqualTo(oldCountryId));
            Assert.That(a1.Country.CountryId, Is.EqualTo(newCountryId));
        }

        /// <summary>
        /// Scenario: Attempt to call implicit save with a null name
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _027_Update_NullName()
        {
            int addressId = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.AddressId_Get_NonCountryOnlyAddress);
            Address a = new Address(addressId);

            a.Title = null;
            a.FirstName = null;
            a.LastName = null;

            Address a1 = new Address(addressId);
            Assert.That(a1.Title, Is.EqualTo(string.Empty));
            Assert.That(a1.FirstName, Is.EqualTo(string.Empty));
            Assert.That(a1.LastName, Is.EqualTo(string.Empty));
            Assert.That(a1.HouseName, Is.EqualTo(a.HouseName));
            Assert.That(a1.Street, Is.EqualTo(a.Street));
            Assert.That(a1.Town, Is.EqualTo(a.Town));
            Assert.That(a1.City, Is.EqualTo(a.City));
            Assert.That(a1.County, Is.EqualTo(a.County));
            Assert.That(a1.Postcode, Is.EqualTo(a.Postcode));
            Assert.That(a1.Country.CountryId, Is.EqualTo(a.Country.CountryId));
        }

        /// <summary>
        /// Scenario: Attempt to call implicit save with a null town
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _028_Update_NullTown()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            AddressDataCreator.CreateCountryOnlyAddress("membership", countryId);

            int addressId = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.AddressId_Get_NonCountryOnlyAddress);
            Assert.That(addressId, Is.GreaterThan(0));
            Address a = new Address(addressId);

            a.Town = null;

            Address a1 = new Address(addressId);

            Assert.That(a1.Title, Is.EqualTo(a.Title));
            Assert.That(a1.FirstName, Is.EqualTo(a.FirstName));
            Assert.That(a1.LastName, Is.EqualTo(a.LastName));
            Assert.That(a1.HouseName, Is.EqualTo(a.HouseName));
            Assert.That(a1.Street, Is.EqualTo(a.Street));
            Assert.That(a1.Town, Is.EqualTo(string.Empty));
            Assert.That(a1.City, Is.EqualTo(a.City));
            Assert.That(a1.County, Is.EqualTo(a.County));
            Assert.That(a1.Postcode, Is.EqualTo(a.Postcode));
            Assert.That(a1.Country.CountryId, Is.EqualTo(a.Country.CountryId));
        }

        /// <summary>
        /// Scenario: Attempt to call Update Address with valid params
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _029_Update_NonCountryOnlyAddress_AllParams()
        {
            int addressId = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.AddressId_Get_NonCountryOnlyAddress);
            Assert.That(addressId, Is.GreaterThan(0));
            Address a = new Address(addressId);
            int newCountryId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(CountryDataQueries.CountryId_Get_RandomExceptQuotedId, a.Country.CountryId));
            Assert.That(newCountryId, Is.GreaterThan(0));

            string newName = Guid.NewGuid().ToString();
            string newHouse = Guid.NewGuid().ToString();
            string newStreet = Guid.NewGuid().ToString();
            string newTown = Guid.NewGuid().ToString();
            string newCity = Guid.NewGuid().ToString();
            string newCounty = Guid.NewGuid().ToString();
            string newPostCode = DataGenerator.GenerateRandomText(8, false, false);

            a.Title = "Mrs";
            a.FirstName = newName;
            a.LastName = newName;
            a.HouseName = newHouse;
            a.Street = newStreet;
            a.Town = newTown;
            a.City = newCity;
            a.County = newCounty;
            a.Postcode = newPostCode;
            a.Country = new Country(newCountryId);

            Address a1 = new Address(addressId);
            Assert.That(a1.Title, Is.EqualTo("Mrs"));
            Assert.That(a1.FirstName, Is.EqualTo(newName));
            Assert.That(a1.LastName, Is.EqualTo(newName));
            Assert.That(a1.HouseName, Is.EqualTo(newHouse));
            Assert.That(a1.Street, Is.EqualTo(newStreet));
            Assert.That(a1.Town, Is.EqualTo(newTown));
            Assert.That(a1.City, Is.EqualTo(newCity));
            Assert.That(a1.County, Is.EqualTo(newCounty));
            Assert.That(a1.Postcode, Is.EqualTo(newPostCode));
            Assert.That(a1.Country.CountryId, Is.EqualTo(newCountryId));
        }

        /// <summary>
        /// Scenario: Compare results of method to test data - as it is a lookup table fetch they should be identical
        /// Expected: Lists of countries/ids match
        /// </summary>
        [Test]
        public void _030_GetCountryList()
        {
            DataTable dtCountries = DbInterface.ExecuteQueryDataTable("membership", CountryDataQueries.Country_Get_All);
            LookupPair[] countryList = Address.GetCountryList();

            // ensure there is data to test with
            Assert.That(dtCountries.Rows.Count, Is.GreaterThan(0));

            // ensure the counts match
            Assert.That(countryList.Length, Is.EqualTo(dtCountries.Rows.Count));

            // check each item individuallys
            for (int i = 0; i < dtCountries.Rows.Count; i++)
            {
                Assert.That(countryList[i], Is.Not.Null);
                Assert.That(countryList[i].Key, Is.EqualTo(Convert.ToString(dtCountries.Rows[i]["CountryId"])));
                Assert.That(countryList[i].Value, Is.EqualTo(Convert.ToString(dtCountries.Rows[i]["DisplayText"])));
            }
        }

        /// <summary>
        /// Scenario: Evaluate the first/last name properties (IAddress)
        /// Expected: Appropriate values - calculated from recipient
        /// </summary>
        [Test]
        public void _031_FirstName_LastName()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            Country country = new Country(countryId);

            // with a space
            Address a1 = Address.CreateAddress(string.Empty, "Bob", "McTesty", "test", "test", "test", "test", "test", "test", country, "test031a@mattchedit.com");
            Assert.That(a1.FirstName, Is.EqualTo("Bob"));
            Assert.That(a1.LastName, Is.EqualTo("McTesty"));

            // without a space
            Address a2 = Address.CreateAddress(string.Empty, string.Empty, "McTesty", "test", "test", "test", "test", "test", "test", country, "test031b@mattchedit.com");
            Assert.That(a2.FirstName, Is.EqualTo(string.Empty));
            Assert.That(a2.LastName, Is.EqualTo("McTesty"));
        }

        /// <summary>
        /// Scenario: Evaluate the IAddress compatible Country property
        /// Expected: Correct (string) values
        /// </summary>
        [Test]
        public void _032_IAddress_Country()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            Country country = new Country(countryId);

            Address a1 = Address.CreateAddress(string.Empty, "Bob", "McTesty", "test", "test", "test", "test", "test", "test", country, "test032@mattchedit.com");
            Assert.That(((IAddress)a1).Country, Is.EqualTo(country.DisplayText));
        }
    }
}
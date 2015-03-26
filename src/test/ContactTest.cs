using System;
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
    /// This class is the unit testing fixture for the Contact business entity.
    /// </summary>
    /// <seealso cref="Contact"/>
    [TestFixture]
    public class ContactTest
    {
        /// <summary>
        /// Ensure all data required during test is set up
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            SystemUserDataCreator.CreateOnlySystemUsers("membership", 60, 300);
        }

        /// <summary>
        /// Ensure all data entered during test is cleared
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogManager.Instance.Dispose();
        }

        /// <summary>
        /// Scenario: Object created with a valid Guid
        /// Expected: Email Address created
        /// </summary>
        [Test]
        public void _001_Constructor_ByCookie_ValidGuid()
        {
            Guid confirmGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);
            Contact ea = new Contact(confirmGuid);
            Assert.That(ea.ConfirmGuid, Is.EqualTo(confirmGuid));
            int emailAddressId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.EmailAddressId_Select, ea.EmailAddress));
            Contact ea2 = new Contact(emailAddressId);
            Assert.That(emailAddressId, Is.EqualTo(ea2.EmailAddressId));
            Assert.That(ea.EmailAddress, Is.EqualTo(ea2.EmailAddress));
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _002_IsConfirmed()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", ContactDataQueries.EmailAddress_All_Get_Random);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Contact ea = new Contact(new Guid(Convert.ToString(dt.Rows[i]["ConfirmGuid"])));
                Assert.That(ea.IsConfirmed, Is.EqualTo(Convert.ToBoolean(dt.Rows[i]["IsConfirmed"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Create an object for a known set of data, test the property
        /// Expected: value returned as per database
        /// </summary>
        [Test]
        public void _003_EmailAddressOrder()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_EmailAddress_All_Get_Random);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Contact ea = new Contact(new Guid(Convert.ToString(dt.Rows[i]["ConfirmGuid"])));
                Assert.That(ea.EmailAddressOrder, Is.EqualTo(Convert.ToInt32(dt.Rows[i]["EmailAddressOrder"])), string.Format("Property incorrect for row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Call ConfirmEmailAddress with valid params
        /// Expected: Confirm done
        /// </summary>
        [Test]
        public void _004_ConfirmEmailAddress()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", ContactDataQueries.EmailAddress_Unconfirmed_Get_Random);
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
            string email = Convert.ToString(dt.Rows[0]["EmailAddress"]);
            Guid confirmGuid = new Guid(Convert.ToString(dt.Rows[0]["ConfirmGuid"]));

            Contact.ConfirmEmailAddress(email, confirmGuid);
            Contact ea = new Contact(confirmGuid);
            Assert.That(ea.IsConfirmed, Is.True, "Confirm did not work");
        }

        /// <summary>
        /// Scenario: Call GetSystemUserIdForEmailAddress with valid params
        /// Expected: GetSystemUserIdForEmailAddress done
        /// </summary>
        [Test]
        public void _005_GetSystemUserIdForEmailAddress()
        {
            // null user
            int emailAddressId = DbInterface.ExecuteQueryScalar<int>("membership", ContactDataQueries.EmailAddressId_Select_NotAssociatedToSystemUser);
            Guid confirmId = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(ContactDataQueries.EmailAddress_SelectConfirmGuid_ById, emailAddressId));
            Contact ea = new Contact(confirmId);
            Assert.That(ea.GetSystemUserForEmailAddress(), Is.Null, "Null expected");

            // not null user
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_EmailAddress_All_Get_Random);
            emailAddressId = Convert.ToInt32(dt.Rows[0]["EmailAddressId"]);
            confirmId = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(ContactDataQueries.EmailAddress_SelectConfirmGuid_ById, emailAddressId));

            Contact ea2 = new Contact(confirmId);
            Assert.That(ea2.GetSystemUserForEmailAddress(), Is.Not.Null, "SystemUser expected to be not null");
        }

        /// <summary>
        /// Scenario: Get an emailaddress using email cookie for an email address not asssociated with a systemuser
        /// Expected: emailaddress object returned correctly
        /// </summary>
        [Test]
        public void _006_Constructor_Context_NotSystemUser()
        {
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_NotInSystemUser_EmailAddress_Get_Random);
            Contact contactToCheck = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(string.Empty);

            // set emailaddress cookie
            string emailAddressCookieName = "TestEmailCookie";
            HttpCookie cookieEmail = new HttpCookie(emailAddressCookieName);
            cookieEmail.Value = emailAddressGuid.ToString();
            hc.Request.Cookies.Add(cookieEmail);

            Contact ea = new Contact(hc, emailAddressCookieName);

            Assert.That(ea, Is.Not.Null, "Object expected");
            Assert.That(ea.EmailAddress, Is.EqualTo(contactToCheck.EmailAddress), "EmailAddress retrieval not correct (Value)");
        }

        /// <summary>
        /// Scenario: Get an emailaddress using email cookie for an email address asssociated with a systemuser
        /// Expected: emailaddress object returned correctly
        /// </summary>
        [Test]
        public void _007_Constructor_Context_SystemUser()
        {
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_NotInSystemUser_EmailAddress_Get_Random);
            Contact contactToCheck = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(contactToCheck.EmailAddress);

            // set emailaddress cookie
            string emailAddressCookieName = "TestEmailCookie";
            HttpCookie cookieEmail = new HttpCookie(emailAddressCookieName);
            cookieEmail.Value = emailAddressGuid.ToString();
            hc.Request.Cookies.Add(cookieEmail);

            Contact ea = new Contact(hc, emailAddressCookieName);

            Assert.That(ea, Is.Not.Null, "Object expected");
            Assert.That(ea.EmailAddress, Is.EqualTo(contactToCheck.EmailAddress), "EmailAddress retrieval not correct (Value)");
        }

        /// <summary>
        /// Scenario: Get an emailaddress using email cookie
        /// Expected: emailaddress object returned correctly
        /// </summary>
        [Test]
        public void _008_Constructor_Context_NonExistantCookie()
        {
            HttpContext hc = HttpHelper.CreateHttpContext(string.Empty);

            // set emailaddress cookie
            string emailAddressCookieName = "TestEmailCookie";
            HttpCookie cookieEmail = new HttpCookie(emailAddressCookieName);
            cookieEmail.Value = Guid.NewGuid().ToString();
            hc.Request.Cookies.Add(cookieEmail);

            Assert.That(delegate { Contact ea = new Contact(hc, emailAddressCookieName); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("A valid emailaddress cannot be found, emailAddressGuid={0}", cookieEmail.Value)));
        }

        /// <summary>
        /// Scenario: Get an emailaddress using email cookie
        /// Expected: emailaddress object returned correctly
        /// </summary>
        [Test]
        public void _009_Constructor_Context_CookieWithInvalidGuid()
        {
            HttpContext hc = HttpHelper.CreateHttpContext(string.Empty);
            Assert.That(delegate { Contact ea = new Contact(hc, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("A valid emailaddress cannot be found, empty or missing Guid"));
        }

        /// <summary>
        /// Scenario: Create an emailaddress not in a transaction
        /// Expected: emailaddress object returned correctly
        /// </summary>
        [Test]
        public void _010_CreateContact_NoTransaction()
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(25, "mattchedit.com");
            Contact cn = Contact.CreateContact(emailAddress);
            Assert.That(cn.EmailAddress, Is.EqualTo(emailAddress));
        }

        /// <summary>
        /// Scenario: Create a contact twice
        /// Expected: Rows inserted once, object with same ID(s) returned
        /// </summary>
        [Test]
        public void _011_CreateContact_AlreadyExists()
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(25, "mattchedit.com");
            Contact cn1 = Contact.CreateContact(emailAddress);
            Contact cn2 = Contact.CreateContact(emailAddress);
            Assert.That(cn1.EmailAddressId, Is.EqualTo(cn2.EmailAddressId));
        }

        /// <summary>
        /// Scenario: Get Addresses array for a Contact
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _012_Contact_AddressesArray()
        {
            int emailAddressId = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.EmailAddressId_Get_RandomFromAddress);
            Contact cn = new Contact(emailAddressId);

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", string.Format(AddressDataQueries.Addresses_GetForEmailAddress, emailAddressId));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int addressId = Convert.ToInt32(dt.Rows[i]["AddressId"]);
                bool found = false;
                
                for (int j = 0; j < cn.Addresses.Length; j++)
                {
                    if (cn.Addresses[j].AddressId == addressId)
                    {
                        found = true;
                    }
                }

                Assert.That(found, string.Format("addressId: {0} not found", addressId), Is.True);
            }
        }
    }
}
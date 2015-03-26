using System;
using System.Data;
using Codentia.Common.Data;
using Codentia.Common.Membership.Test.Queries;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Common.Membership.Test
{
    /// <summary>
    /// TestFixture for ContactData
    /// <seealso cref="ContactData"/>
    /// </summary>
    [TestFixture]
    public class ContactDataTest
    {
        /// <summary>
        /// Ensure all set-up required for testing has been completed
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        /// <summary>
        /// Clean up
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        /// <summary>
        /// Scenario: Attempt to call EmailAddressExists with an empty email address
        /// Expected: Email Address not specified Exception raised for null and empty strings
        ///           Invalid email address Exception raised for invalid email address
        /// </summary>
        [Test]
        public void _001_EmailAddressExists_Address_EmptyAddress()
        {
            // null
            Assert.That(delegate { ContactData.EmailAddressExists(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // empty 
            Assert.That(delegate { ContactData.EmailAddressExists(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to call EmailAddressExists with an existing address
        /// Expected: EmailAddressExists returns true for check id 0, EmailAddressExists returns false for check id _SystemUserIdForExistantEmailAddress
        /// </summary>
        [Test]
        public void _002_EmailAddressExists_Address_ExistingAddress()
        {
            string emailAddressExistant = DbInterface.ExecuteQueryScalar<string>("membership", ContactDataQueries.EmailAddress_Get_Random);
            Assert.That(ContactData.EmailAddressExists(emailAddressExistant), Is.True, "Result should be true");

            // txnl
            emailAddressExistant = DbInterface.ExecuteQueryScalar<string>("membership", ContactDataQueries.EmailAddress_Get_Random);
            Guid txnId = Guid.NewGuid();
            try
            {
                Assert.That(ContactData.EmailAddressExists(txnId, emailAddressExistant), Is.True, "Result should be true");
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }
        }

        /// <summary>
        /// Scenario: Attempt to call EmailAddressExists with an non-existant email address
        /// Expected: EmailAddressExists returns false
        /// </summary>
        [Test]
        public void _003_EmailAddressExists_Address_NonExistantAddress()
        {
            Assert.That(ContactData.EmailAddressExists("THISISNOTANEMAILADDRESSTHATEXISTS@MYINVALIDDOMAIN.COM"), Is.False, "Result should be false");

            // txnl
            Guid txnId = Guid.NewGuid();
            try
            {
                Assert.That(ContactData.EmailAddressExists(txnId, "THISISNOTANEMAILADDRESSTHATEXISTS@MYINVALIDDOMAIN.COM"), Is.False, "Result should be false");
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }
        }

        /// <summary>
        /// Scenario: Attempt to call EmailAddressExists with an invalid Id
        /// Expected: Invalid Email Address Exception raised 
        /// </summary>
        [Test]
        public void _004_EmailAddressExists_Id_InvalidId()
        {
            // 0
            Assert.That(delegate { ContactData.EmailAddressExists(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { ContactData.EmailAddressExists(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: -1 is not valid"));
        }

        /// <summary>
        /// Scenario: Attempt to call EmailAddressExists with an existing address
        /// Expected: EmailAddressExists returns true for check id 0, EmailAddressExists returns false for check id _SystemUserIdForExistantEmailAddress
        /// </summary>
        [Test]
        public void _005_EmailAddressExists_Id_ExistingAddress()
        {
            int emailAddressExistant = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            Assert.That(ContactData.EmailAddressExists(emailAddressExistant), Is.True, "Result should be true");

            // txnl
            Guid txnId = Guid.NewGuid();
            try
            {
                Assert.That(ContactData.EmailAddressExists(txnId, emailAddressExistant), Is.True, "Result should be true");
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }
        }

        /// <summary>
        /// Scenario: Attempt to call EmailAddressExists with an non-existant email address
        /// Expected: EmailAddressExists returns false
        /// </summary>
        [Test]
        public void _006_EmailAddressExists_Id_NonExistantAddress()
        {
            int emailAddressNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
            Assert.That(ContactData.EmailAddressExists(emailAddressNonExistant), Is.False, "Result should be false");

            // txnl
            Guid txnId = Guid.NewGuid();
            try
            {
                Assert.That(ContactData.EmailAddressExists(txnId, emailAddressNonExistant), Is.False, "Result should be false");
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }
        }

        /// <summary>
        /// Scenario: Attempt to call CreateEmailAddress with a null or empty string
        /// Expected: Not specified Email Address Exception raised 
        /// </summary>
        [Test]
        public void _007_CreateEmailAddress_NullOrEmptyAddress()
        {
            // null
            Assert.That(delegate { ContactData.CreateEmailAddress(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // empty 
            Assert.That(delegate { ContactData.CreateEmailAddress(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateEmailAddress with an email address that already exists
        /// Expected: Email Address already exists Exception raised 
        /// </summary>
        [Test]
        public void _008_CreateEmailAddress_ExistantEmailAddress()
        {
            string emailAddress = DbInterface.ExecuteQueryScalar<string>("membership", ContactDataQueries.EmailAddress_Get_Random);

            // already exists
            Assert.That(delegate { ContactData.CreateEmailAddress(emailAddress); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("emailAddress: {0} already exists", emailAddress)));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateEmailAddress with a valid new email address
        /// Expected: Email Address created
        /// </summary>
        [Test]
        public void _009_CreateEmailAddress_ValidNewAddress()
        {
            string newEmail = string.Format("BLAH{0}@blahblah.com", Guid.NewGuid());
            int newEmailId = ContactData.CreateEmailAddress(newEmail);
            Assert.That(newEmailId, Is.GreaterThan(0), "Email address not created");
            string newEmailCheck = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(ContactDataQueries.EmailAddress_Select, newEmailId));
            Assert.That(newEmail, Is.EqualTo(newEmailCheck), "Email Addresses do not match");

            // txnl
            newEmail = string.Format("BLAH{0}@blahblah.com", Guid.NewGuid());

            Guid txnId = Guid.NewGuid();
            try
            {
                newEmailId = ContactData.CreateEmailAddress(txnId, newEmail);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(newEmailId, Is.GreaterThan(0), "Email address not created");
            newEmailCheck = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(ContactDataQueries.EmailAddress_Select, newEmailId));
            Assert.That(newEmail, Is.EqualTo(newEmailCheck), "Email Addresses do not match");
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an empty Guid
        /// Expected: Empty Guid Exception raised
        /// </summary>
        [Test]
        public void _010_GetEmailAddressData_Guid_EmptyGuid()
        {
            Assert.That(delegate { ContactData.GetEmailAddressData(Guid.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressCookie is an empty Guid"));
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with valid Guid
        /// Expected: EmailAddress data returned
        /// </summary>
        [Test]
        public void _011_GetEmailAddressData_Guid_ValidGuid()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", ContactDataQueries.EmailAddressIdAndConfirmGuid_Get_Random);
            int emailAddressId = Convert.ToInt32(dt.Rows[0]["EmailAddressId"]);
            Guid emailAddressGuid = new Guid(Convert.ToString(dt.Rows[0]["ConfirmGuid"]));

            DataTable dt2 = ContactData.GetEmailAddressData(emailAddressGuid);
            int emailAddressCheckId = Convert.ToInt32(dt2.Rows[0]["EmailAddressId"]);

            Assert.That(emailAddressId, Is.EqualTo(emailAddressCheckId), "EmailAddressIds do not match");
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an invalid Id
        /// Expected: Invalid Id Exception raised
        /// </summary>
        [Test]
        public void _012_GetEmailAddressData_Id_InvalidId()
        {
            // 0
            Assert.That(delegate { ContactData.GetEmailAddressData(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { ContactData.GetEmailAddressData(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: -1 is not valid"));

            // non-existant
            int emailAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
            Assert.That(delegate { ContactData.GetEmailAddressData(emailAddressIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("emailAddressId: {0} does not exist", emailAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an existing Id
        /// Expected: EmailAddress data returned
        /// </summary>
        [Test]
        public void _013_GetEmailAddressData_Id_ExistingId()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", ContactDataQueries.EmailAddressIdAndConfirmGuid_Get_Random);
            int emailAddressId = Convert.ToInt32(dt.Rows[0]["EmailAddressId"]);
            Guid emailAddressGuid = new Guid(Convert.ToString(dt.Rows[0]["ConfirmGuid"]));

            DataTable dt2 = ContactData.GetEmailAddressData(emailAddressId);
            Guid emailAddressCheckId = new Guid(Convert.ToString(dt2.Rows[0]["ConfirmGuid"]));

            Assert.That(emailAddressGuid, Is.EqualTo(emailAddressCheckId), "EmailAddressGuids do not match");

            // txnl
            dt = DbInterface.ExecuteQueryDataTable("membership", ContactDataQueries.EmailAddressIdAndConfirmGuid_Get_Random);
            emailAddressId = Convert.ToInt32(dt.Rows[0]["EmailAddressId"]);
            emailAddressGuid = new Guid(Convert.ToString(dt.Rows[0]["ConfirmGuid"]));

            Guid txnId = Guid.NewGuid();
            try
            {
                dt2 = ContactData.GetEmailAddressData(txnId, emailAddressId);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            emailAddressCheckId = new Guid(Convert.ToString(dt2.Rows[0]["ConfirmGuid"]));

            Assert.That(emailAddressGuid, Is.EqualTo(emailAddressCheckId), "EmailAddressGuids do not match");
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with a null or empty string
        /// Expected: Not specified Exception raised
        /// </summary>
        [Test]
        public void _014_GetEmailAddressData_String_InvalidEmailAddress()
        {
            // null
            Assert.That(delegate { ContactData.GetEmailAddressData(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // empty 
            Assert.That(delegate { ContactData.GetEmailAddressData(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // non-existant            
            Assert.That(delegate { ContactData.GetEmailAddressData("THISWILLDEFINITELYNOTEXISTEVERIDONTTHINK@BLAHBLAH.COM"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress: THISWILLDEFINITELYNOTEXISTEVERIDONTTHINK@BLAHBLAH.COM does not exist"));
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an existing Email
        /// Expected: EmailAddress data returned
        /// </summary>
        [Test]
        public void _015_GetEmailAddressData_String_ExistingEmail()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", ContactDataQueries.EmailAddressAndConfirmGuid_Get_Random);
            string emailAddress = Convert.ToString(dt.Rows[0]["EmailAddress"]);
            Guid emailAddressGuid = new Guid(Convert.ToString(dt.Rows[0]["ConfirmGuid"]));

            DataTable dt2 = ContactData.GetEmailAddressData(emailAddress);
            Guid emailAddressCheckId = new Guid(Convert.ToString(dt2.Rows[0]["ConfirmGuid"]));

            Assert.That(emailAddressGuid, Is.EqualTo(emailAddressCheckId), "EmailAddressGuids do not match");

            // txnl
            dt = DbInterface.ExecuteQueryDataTable("membership", ContactDataQueries.EmailAddressAndConfirmGuid_Get_Random);
            emailAddress = Convert.ToString(dt.Rows[0]["EmailAddress"]);
            emailAddressGuid = new Guid(Convert.ToString(dt.Rows[0]["ConfirmGuid"]));

            Guid txnId = Guid.NewGuid();
            try
            {
                dt2 = ContactData.GetEmailAddressData(txnId, emailAddress);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            emailAddressCheckId = new Guid(Convert.ToString(dt2.Rows[0]["ConfirmGuid"]));

            Assert.That(emailAddressGuid, Is.EqualTo(emailAddressCheckId), "EmailAddressGuids do not match");
        }

        /// <summary>
        /// Scenario: Attempt to call Confirm with null or empty EmailAddress
        /// Expected: Not Specified EmailAddress exception raised
        /// </summary>
        [Test]
        public void _016_Confirm_NullOrEmptyEmailAddress()
        {
            // null
            Assert.That(delegate { ContactData.Confirm(null, Guid.NewGuid()); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // empty 
            Assert.That(delegate { ContactData.Confirm(string.Empty, Guid.NewGuid()); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // non-existant            
            Assert.That(delegate { ContactData.Confirm("THISWILLDEFINITELYNOTEXISTEVERIDONTTHINK@BLAHBLAH.COM", Guid.NewGuid()); }, Throws.InstanceOf<ArgumentException>());
        }

        /// <summary>
        /// Scenario: Attempt to call Confirm with an empty Guid
        /// Expected: Empty Guid exception raised
        /// </summary>
        [Test]
        public void _017_Confirm_EmptyGuid()
        {
            string existingEmail = DbInterface.ExecuteQueryScalar<string>("membership", ContactDataQueries.EmailAddress_Get_Random);
            Assert.That(delegate { ContactData.Confirm(existingEmail, Guid.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("confirmGuid is an empty Guid"));
        }

        /// <summary>
        /// Scenario: Attempt to call Confirm with a non-existant Guid
        /// Expected: Non-Existant Guid exception raised
        /// </summary>
        [Test]
        public void _018_Confirm_NonExistantGuid()
        {
            string existingEmail = DbInterface.ExecuteQueryScalar<string>("membership", ContactDataQueries.EmailAddress_Get_Random);
            Guid nonExistantGuid = Guid.NewGuid();
            Assert.That(delegate { ContactData.Confirm(existingEmail, nonExistantGuid); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("confirmGuid: {0} does not exist", nonExistantGuid)));
        }

        /// <summary>
        /// Scenario: Attempt to call Confirm with a non-existant EmailAddress
        /// Expected: Non-Existant EmailAddress exception raised
        /// </summary>
        [Test]
        public void _019_Confirm_GuidForAnotherUser()
        {
            string existingEmail = DbInterface.ExecuteQueryScalar<string>("membership", ContactDataQueries.EmailAddress_Get_Random);
            Guid anotherUserGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(ContactDataQueries.ConfirmGuid_Select, existingEmail));
            Assert.That(delegate { ContactData.Confirm(existingEmail, anotherUserGuid); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("confirmGuid: {0} exists for another emailaddress", anotherUserGuid)));
        }

        /// <summary>
        /// Scenario: Attempt to call Confirm with a non-existant EmailAddress
        /// Expected: Non-Existant EmailAddress exception raised
        /// </summary>
        [Test]
        public void _020_Confirm_ValidParams()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", ContactDataQueries.EmailAddressAndConfirmGuid_Get_Random);
            string emailAddress = Convert.ToString(dt.Rows[0]["EmailAddress"]);
            Guid confirmGuid = new Guid(Convert.ToString(dt.Rows[0]["ConfirmGuid"]));
            DbInterface.ExecuteQueryNoReturn("membership", string.Format(ContactDataQueries.IsConfirmed_Update, emailAddress));
            ContactData.Confirm(emailAddress, confirmGuid);
            bool isConfirmed = DbInterface.ExecuteQueryScalar<bool>("membership", string.Format(ContactDataQueries.IsConfirmed_Select, emailAddress));
            Assert.That(isConfirmed, Is.True, "True expected");
        }

        /// <summary>
        /// Scenario: Method called with a null/empty phonenumber string
        /// Expected: Exception
        /// </summary>
        [Test]
        public void _021_CreatePhoneNumber_Invalid()
        {
            // null
            Assert.That(delegate { ContactData.CreatePhoneNumber(Guid.NewGuid(), null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumber is not specified"));

            // empty
            Assert.That(delegate { ContactData.CreatePhoneNumber(Guid.NewGuid(), string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumber is not specified"));

            // invalid 1
            Assert.That(delegate { ContactData.CreatePhoneNumber(Guid.NewGuid(), "invalid"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumber: invalid is invalid"));

            // invalid 2
            Assert.That(delegate { ContactData.CreatePhoneNumber(Guid.NewGuid(), "123"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumber: 123 is invalid"));

            // exceed max length
            Assert.That(delegate { ContactData.CreatePhoneNumber(Guid.NewGuid(), "12345678901234567890"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumber exceeds maxLength 13 as it has 20 chars"));
        }

        /// <summary>
        /// Scenario: Method called with valid arguments (number not already in use)
        /// Expected: Record created
        /// </summary>
        [Test]
        public void _022_CreatePhoneNumber_Valid()
        {
            // get a random, non-existant value
            Random r = new Random();
            int phoneNumber = r.Next(111111111, 999999999);

            while (DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.PhoneNumberId_Get_ByPhoneNumber, "01" + phoneNumber)) > 0)
            {
                phoneNumber = r.Next(111111111, 999999999);
            }

            int id = 0;
            Guid txnId = Guid.NewGuid();
            try
            {
                id = ContactData.CreatePhoneNumber(txnId, "01" + phoneNumber);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(id, Is.GreaterThan(0));

            int checkId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.PhoneNumberId_Get_ByPhoneNumber, "01" + phoneNumber));
            Assert.That(checkId, Is.EqualTo(id));

            Assert.That(DbInterface.ExecuteQueryScalar<string>("membership", string.Format(ContactDataQueries.PhoneNumber_Get_ByPhoneNumberId, id)), Is.EqualTo("01" + phoneNumber));
        }

        /// <summary>
        /// Scenario: Method called with valid arguments (number already in use)
        /// Expected: Record Id returned
        /// </summary>
        [Test]
        public void _023_CreatePhoneNumber_Valid_Exists()
        {
            // get an extant phoneNumber
            string phoneNumber = DbInterface.ExecuteQueryScalar<string>("membership", ContactDataQueries.PhoneNumber_Get_Random);
            int id = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.PhoneNumberId_Get_ByPhoneNumber, phoneNumber));
            Assert.That(id, Is.GreaterThan(0));

            int newId = 0;
            Guid txnId = Guid.NewGuid();
            try
            {
                newId = ContactData.CreatePhoneNumber(txnId, phoneNumber);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(newId, Is.EqualTo(id));
        }

        /// <summary>
        /// Scenario: Method called with invalid or non-existant id
        /// Expected: Exception
        /// </summary>
        [Test]
        public void _024_GetPhoneNumber_InvalidPhoneNumberId()
        {
            // 0
            Assert.That(delegate { ContactData.GetPhoneNumber(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumberId: 0 is not valid"));

            // -1 
            Assert.That(delegate { ContactData.GetPhoneNumber(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumberId: -1 is not valid"));

            // non-existant
            int phoneNumberIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "PhoneNumber");
            Assert.That(delegate { ContactData.GetPhoneNumber(phoneNumberIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("phoneNumberId: {0} does not exist", phoneNumberIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Method called with valid Id
        /// Expected: Corresponding phone number returned
        /// </summary>
        [Test]
        public void _025_GetPhoneNumber_ValidPhoneNumberId()
        {
            int id = SqlHelper.GetRandomIdFromTable("membership", "PhoneNumber");
            Assert.That(id, Is.GreaterThan(0));

            string phoneNumber = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(ContactDataQueries.PhoneNumber_Get_ByPhoneNumberId, id));
            string phoneNumberMethod = ContactData.GetPhoneNumber(id);

            Assert.That(phoneNumberMethod, Is.EqualTo(phoneNumber));
        }

        /// <summary>
        /// Scenario: Attempt to call GetSystemUserIdForEmailAddress with an invalid Id
        /// Expected: Raises exceptions
        /// </summary>
        [Test]
        public void _026_GetSystemUserIdForEmailAddress_InvalidId()
        {
            // 0
            Assert.That(delegate { ContactData.GetSystemUserIdForEmailAddress(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { ContactData.GetSystemUserIdForEmailAddress(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: -1 is not valid"));

            // non-existant
            int emailAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
            Assert.That(delegate { ContactData.GetSystemUserIdForEmailAddress(emailAddressIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("emailAddressId: {0} does not exist", emailAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetSystemUserIdForEmailAddress with valid param
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _027_GetSystemUserIdForEmailAddress_ValidParam()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_EmailAddress_All_Get_Random);
            int emailAddressId = Convert.ToInt32(dt.Rows[0]["EmailAddressId"]);
            int systemUserIdExpected = Convert.ToInt32(dt.Rows[0]["SystemUserId"]);

            int systemUserIdActual = ContactData.GetSystemUserIdForEmailAddress(emailAddressId);

            Assert.That(systemUserIdActual, Is.EqualTo(systemUserIdExpected));

            // txnl
            dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_EmailAddress_All_Get_Random);
            emailAddressId = Convert.ToInt32(dt.Rows[0]["EmailAddressId"]);
            systemUserIdExpected = Convert.ToInt32(dt.Rows[0]["SystemUserId"]);

            Guid txnId = Guid.NewGuid();
            try
            {
                systemUserIdActual = ContactData.GetSystemUserIdForEmailAddress(txnId, emailAddressId);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(systemUserIdActual, Is.EqualTo(systemUserIdExpected));
        }

        /// <summary>
        /// Scenario: Attempt to call GetAddressesForEmailAddress with invalid param
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _028_GetAddressesForEmailAddress_ValidId()
        {
            // 0
            Assert.That(delegate { ContactData.GetAddressesForEmailAddress(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { ContactData.GetAddressesForEmailAddress(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: -1 is not valid"));

            // non-existant
            int emailAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
            Assert.That(delegate { ContactData.GetAddressesForEmailAddress(emailAddressIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("emailAddressId: {0} does not exist", emailAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetAddressesForEmailAddress with invalid param
        /// Expected: Raises exceptions
        /// </summary>
        [Test]
        public void _029_GetAddressesForEmailAddress_InvalidId()
        {
            int emailAddressId = DbInterface.ExecuteQueryScalar<int>("membership", AddressDataQueries.EmailAddressId_Get_RandomFromAddress);

            DataTable dt1 = DbInterface.ExecuteQueryDataTable("membership", string.Format(AddressDataQueries.Addresses_GetForEmailAddress, emailAddressId));
            Assert.That(dt1.Rows.Count, Is.GreaterThan(0));

            DataTable dt2 = ContactData.GetAddressesForEmailAddress(emailAddressId);

            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.True);
        }

        /// <summary>
        /// Scenario: Method called with valid arguments (number not already in use) but containing brackets and spaces
        /// Expected: Record created with brackets/spaces stripped
        /// </summary>
        [Test]
        public void _030_CreatePhoneNumber_Valid_WithRemovedChars()
        {
            // get a random, non-existant value
            Random r = new Random();
            int phoneNumber = r.Next(111111, 999999);

            while (DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.PhoneNumberId_Get_ByPhoneNumber, "01534" + phoneNumber)) > 0)
            {
                phoneNumber = r.Next(111111, 999999);
            }

            int id = 0;
            Guid txnId = Guid.NewGuid();
            try
            {
                id = ContactData.CreatePhoneNumber(txnId, "(01534) " + phoneNumber);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(id, Is.GreaterThan(0));

            int checkId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.PhoneNumberId_Get_ByPhoneNumber, "01534" + phoneNumber));
            Assert.That(checkId, Is.EqualTo(id));

            Assert.That(DbInterface.ExecuteQueryScalar<string>("membership", string.Format(ContactDataQueries.PhoneNumber_Get_ByPhoneNumberId, id)), Is.EqualTo("01534" + phoneNumber));
        }
    }
}
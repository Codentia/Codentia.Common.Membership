using System;
using System.Data;
using System.Web;
using System.Web.Security;
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
    /// This class serves as the unit testing fixture for the SystemUser business entity
    /// </summary>
    /// <seealso cref="SystemUser"/>
    [TestFixture]
    public class SystemUserTest
    {
        private int _emailAddressExistant;
        private int _emailAddressNonExistant;

        /// <summary>
        /// Ensure all data required during test is set up
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            SystemUserDataCreator.CreateOnlySystemUsers("membership", 90, 300);
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "SystemUser"), Is.True, "No system user data exists to test with");

            _emailAddressExistant = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            _emailAddressNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
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
        /// Scenario: New user created, email addresses are already in use (duplicate user)
        /// Expected: Exception(A user with these details already exists)
        /// </summary>
        [Test]
        public void _001_CreateSystemUser_Duplicate()
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(20, "mattchedit.com");

            Guid txnId = Guid.NewGuid();
            try
            {
                SystemUser su = SystemUser.CreateSystemUser(txnId, emailAddress, "Password123", "Firstname", "Surname", true, null, "User");
                DbInterface.CommitTransaction(txnId);
            }
            catch (Exception ex)
            {
                DbInterface.RollbackTransaction(txnId);
                throw ex;
            }

            SystemUser su2 = null;

            txnId = Guid.NewGuid();
            Assert.That(delegate { su2 = SystemUser.CreateSystemUser(txnId, emailAddress, "Password123", "Firstname", "Surname", false, null, "User"); }, Throws.InstanceOf<DuplicateEmailAddressException>().With.Message.EqualTo("An account already exists for that email address. If you have forgotten your password, please use the links at the top of the page to receive a new one."));
            DbInterface.RollbackTransaction(txnId);
            Assert.That(su2, Is.Null, "User was created but should not have been");
        }

        /// <summary>
        /// Scenario: New user created with valid parameters
        /// Expected: User is registered in database, object created without error
        /// </summary>
        [Test]
        public void _004_CreateSystemUser_NewUser()
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(15, "mattchedit.com");

            SystemUser su = null;
            Guid txnId = Guid.NewGuid();
            try
            {
                su = SystemUser.CreateSystemUser(txnId, emailAddress, "Password123", "Firstname2", "Surname2", false, null, "User");
                DbInterface.CommitTransaction(txnId);
            }
            catch (Exception ex)
            {
                DbInterface.RollbackTransaction(txnId);
                throw ex;
            }

            int systemUserId = SqlHelper.GetMaxIdFromTable("membership", "SystemUser");

            Assert.That(su.PhoneNumber, Is.EqualTo(string.Empty));
            Assert.That(su.HasNewsLetter, Is.False);
            Assert.That(su.SystemUserId, Is.EqualTo(systemUserId));
        }

        /// <summary>
        /// Scenario: New user created with valid parameters
        /// Expected: User is registered in database, object created without error
        /// </summary>
        [Test]
        public void _004a_Create_NewUser()
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(15, "mattchedit.com");

            int systemUserId = 0;
            Guid txnId = Guid.NewGuid();
            try
            {
                systemUserId = SystemUser.Create(txnId, emailAddress, "Password123", "Firstname2", "Surname2", false, null, "User");
                DbInterface.CommitTransaction(txnId);
            }
            catch (Exception ex)
            {
                DbInterface.RollbackTransaction(txnId);
                throw ex;
            }

            int systemUserIdCheck = SqlHelper.GetMaxIdFromTable("membership", "SystemUser");
            Assert.That(systemUserId, Is.EqualTo(systemUserIdCheck));
        }

        /// <summary>
        /// Scenario: Attempt made to construct a new system user with an invalid (null, empty) password)
        /// Expected: Exception()
        /// </summary>
        [Test]
        public void _005_CreateSystemUser_InvalidPassword()
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(15, "mattchedit.com");

            Guid txnId = Guid.NewGuid();

            Assert.That(delegate { SystemUser su = SystemUser.CreateSystemUser(txnId, emailAddress, null, "Test", "User", true, null, "User"); }, Throws.InstanceOf<InvalidPasswordException>().With.Message.EqualTo("We were unable to create your account. Please ensure that you are using a 'strong' password containing several letters (both upper and lower case), and at least one number."));
            DbInterface.RollbackTransaction(txnId);

            txnId = Guid.NewGuid();
            Assert.That(delegate { SystemUser su = SystemUser.CreateSystemUser(txnId, emailAddress, string.Empty, "Test", "User", true, null, "User"); }, Throws.InstanceOf<InvalidPasswordException>().With.Message.EqualTo("We were unable to create your account. Please ensure that you are using a 'strong' password containing several letters (both upper and lower case), and at least one number."));
            DbInterface.RollbackTransaction(txnId);
        }

        /// <summary>
        /// Scenario: Create a new user, with a phone number specified
        /// Expected: Object created, properties match input data
        /// </summary>
        [Test]
        public void _007_CreateSystemUser_WithPhoneNumber()
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(15, "mattchedit.com");

            SystemUser su = null;
            Guid txnId = Guid.NewGuid();
            try
            {
                su = SystemUser.CreateSystemUser(txnId, emailAddress, "test1234", "Test", "User", false, "01534123456", "User");
                DbInterface.CommitTransaction(txnId);
            }
            catch (Exception ex)
            {
                DbInterface.RollbackTransaction(txnId);
                throw ex;
            }

            Assert.That(su.PhoneNumber, Is.EqualTo("01534123456"));
        }

        /// <summary>
        /// Scenario: Create a new user. Change it's password and then authenticate against the new password
        /// Expected: ChangePassword and Authenticate both return true.
        /// </summary>
        [Test]
        public void _008_ChangePassword()
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(17, "mattchedit.com");

            SystemUser su = null;
            Guid txnId = Guid.NewGuid();
            try
            {
                su = SystemUser.CreateSystemUser(txnId, emailAddress, "test123", "test", "test", false, null, "User");
                DbInterface.CommitTransaction(txnId);
            }
            catch (Exception ex)
            {
                DbInterface.RollbackTransaction(txnId);
                Assert.Fail(string.Format("Rolled back: {0}", ex.Message));
            }

            HttpContext x = HttpHelper.CreateHttpContext(string.Empty);

            Assert.That(SystemUser.AuthenticateUser(x.Response, su.EmailAddresses[0].EmailAddress, "test123"), Is.True);

            Assert.That(su.ChangePassword("test123", "321test"), Is.True);

            Assert.That(SystemUser.AuthenticateUser(x.Response, su.EmailAddresses[0].EmailAddress, "test123"), Is.False);
            Assert.That(SystemUser.AuthenticateUser(x.Response, su.EmailAddresses[0].EmailAddress, "321test"), Is.True);
        }

        /// <summary>
        /// Scenario: Set Role for a User
        /// Expected: no exception
        /// </summary>
        [Test]
        public void _009_SetRole_ValidRole()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_Get_NotInAspNetUsers_Random);
            Assert.That(dt, Is.Not.Null, "No Valid SystemUserId to test with");
            int systemUserId = Convert.ToInt32(dt.Rows[0]["SystemUserId"]);
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);

            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);

            SystemUser su = new SystemUser(hc);

            su.SetRole("User");
        }

        /// <summary>
        /// Scenario: Set Invalid Role for a User, null, empty and invalid role
        /// Expected: exception
        /// </summary>
        [Test]
        public void _010_SetRole_InvalidRole()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_EmailAddress_SystemUser_Get_Random);
            Assert.That(dt, Is.Not.Null, "No Valid SystemUserId to test with");
            int systemUserId = Convert.ToInt32(dt.Rows[0]["SystemUserId"]);
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(ContactDataQueries.ConfirmGuid_SelectBySystemUserId, systemUserId));

            Contact ea = new Contact(emailAddressGuid);

            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);

            SystemUser su = new SystemUser(hc);

            // null
            Assert.That(delegate { su.SetRole(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName is not specified"));

            // empty 
            Assert.That(delegate { su.SetRole(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName is not specified"));

            // existing            
            Assert.That(delegate { su.SetRole("THISROLEDEFINITELYDOESNOTEXIST"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName: THISROLEDEFINITELYDOESNOTEXIST does not exist"));
        }

        /// <summary>
        /// Scenario: Property tested against known user in database
        /// Expected: Value returned matches database
        /// </summary>
        [Test]
        public void _011_HasNewsLetter()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_EmailAddress_All_Get_Random);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int systemUserId = Convert.ToInt32(dt.Rows[i]["SystemUserId"]);
                DataTable dt2 = DbInterface.ExecuteQueryDataTable("membership", string.Format(SystemUserDataQueries.SystemUser_Get_ById, systemUserId));
                Assert.That(dt2.Rows.Count, Is.EqualTo(1));
                Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(ContactDataQueries.ConfirmGuid_SelectBySystemUserId, systemUserId));
                Contact ea = new Contact(emailAddressGuid);
                HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
                SystemUser su = new SystemUser(hc);
                Assert.That(su.HasNewsLetter, Is.EqualTo(Convert.ToBoolean(dt2.Rows[0]["HasNewsLetter"])), string.Format("Mismatch at row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: New user created by Id where UserId (GUID) is not null
        /// Expected: User should populate without error
        /// </summary>
        [Test]
        public void _012_Constructor_UserIdNotNull()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUserId_Get_HasUserId);
            int systemUserId = Convert.ToInt32(dt.Rows[0]["SystemUserId"]);
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);
            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);
            Assert.That(su.UserId, Is.Not.Null, "UserId should not be null");
        }

        /// <summary>
        /// Scenario: Ensure a Membership User is created without associating to SystemUser
        /// Expected: Create occurs without error
        /// </summary>
        [Test]
        public void _013_UserId_CreateNewMembershipUser()
        {
            System.Web.Security.Membership.DeleteUser("test@test.com");
            MembershipUser mu = System.Web.Security.Membership.CreateUser("test@test.com", "mytest", "test@test.com");
            Assert.That(mu, Is.Not.Null, "Membership User cannot be null");
            System.Web.Security.Membership.DeleteUser("test@test.com");
        }

        /// <summary>
        /// Scenario: Get valid MembershipUser associated with a System User 
        /// Expected: Membership object is not null
        /// </summary>
        [Test]
        public void _014_UserId_GetMembershipUser_Valid()
        {
            int systemUserId = DbInterface.ExecuteQueryScalar<int>("membership", SystemUserDataQueries.SystemUserId_InAspNetUsers_Get_Random);
            Assert.That(systemUserId, Is.GreaterThan(0), "No Valid SystemUserId to test with");
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);

            Assert.That(su.MembershipUser, Is.Not.Null, "MembershipUser should not be null");
        }

        /// <summary>
        /// Scenario: New user created, update first name as null or empty string implicitly
        /// Expected: Exception(Invalid first name specified)
        /// </summary>
        [Test]
        public void _015_UpdateUser_Params_NullOrEmptyFirstName()
        {
            int systemUserId = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);

            // null
            Assert.That(delegate { su.FirstName = null; }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("firstName is not specified"));

            // empty 
            Assert.That(delegate { su.FirstName = string.Empty; }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("firstName is not specified"));
        }

        /// <summary>
        /// Scenario: New user created, update surname as null or empty string
        /// Expected: Exception(Invalid surname specified)
        /// </summary>
        [Test]
        public void _016_UpdateUser_Params_NullOrEmptySurname()
        {
            int systemUserId = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);

            // null
            Assert.That(delegate { su.Surname = null; }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("surname is not specified"));

            // empty 
            Assert.That(delegate { su.Surname = string.Empty; }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("surname is not specified"));
        }

        /// <summary>
        /// Scenario: Update User with Valid Details
        /// Expected: User Update successful
        /// </summary>
        [Test]
        public void _017_UpdateUser_ValidDetails()
        {
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);

            string firstName = su.FirstName;
            string surname = su.Surname;
            string guid = Guid.NewGuid().ToString().Replace("-", string.Empty);

            string firstNameU = Guid.NewGuid().ToString().Replace("-", string.Empty);
            string surnameU = Guid.NewGuid().ToString().Replace("-", string.Empty);
            bool hn = !su.HasNewsLetter;

            su.FirstName = firstNameU;
            su.Surname = surnameU;
            su.HasNewsLetter = hn;
            su.PhoneNumber = string.Empty;
            su.PhoneNumber = "02392365145";

            // check object has picked up changes
            Assert.That(su.FirstName, Is.EqualTo(firstNameU));
            Assert.That(su.Surname, Is.EqualTo(surnameU));
            Assert.That(su.PhoneNumber, Is.EqualTo("02392365145"));
            Assert.That(su.HasNewsLetter, Is.EqualTo(hn));

            // update user back
            su.FirstName = firstName;
            su.Surname = surname;
        }

        /// <summary>
        /// Scenario: Set SetForcePassword for a User
        /// Expected: runs without exception
        /// </summary>
        [Test]
        public void _018_SetForcePassword()
        {
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);

            su.SetForcePassword();
        }

        /* NOTE: Add this when there are multiple addresses
        /// <summary>
        /// Scenario: Set SetForcePassword for a User
        /// Expected: runs without exception
        /// </summary>
        [Test]
        public void _019_AddAddress_Duplicate()
        {
            int addressIdExistant = 0;
            SystemUser su=null;
            while (addressIdExistant==0)
            {
                Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

                Contact ea = new Contact(emailAddressGuid);
                HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
                su = new SystemUser(hc);           

                addressIdExistant = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(SystemUserDataQueries.SystemUser_Address_Select_ByUserId,  su.MembershipUser.ProviderUserKey.ToString()));
               
            }
            try
            {
                su.AddAddress(new Address(addressIdExistant));
                Assert.Fail("No exception raised");
            }
            catch (Exception ex)
            {
                Assert.That(string.Format("addressId: {0} already exists for UserId={1}", addressIdExistant, su.MembershipUser.ProviderUserKey.ToString()), ex.Message, "Incorrect exception caught");
            }            
        }
         */

        /// <summary>
        /// Scenario: Get a systemUser using context
        /// Expected: systemuser object returned correctly
        /// </summary>
        [Test]
        public void _020_Constructor_Context()
        {
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);

            Assert.That(su, Is.Not.Null, "Object expected");
            Assert.That(su.EmailAddresses[0].EmailAddress, Is.EqualTo(ea.EmailAddress), "SystemUser retrieval not correct (EmailAddress)");
        }

        /// <summary>
        /// Scenario: A valid user is authenticated with a valid password
        /// Expected: true
        /// </summary>
        [Test]
        public void _021_AuthenticateUser_ValidCredentials()
        {
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);

            MembershipUser mu = System.Web.Security.Membership.GetUser(su.EmailAddresses[0].EmailAddress);
            Assert.That(mu, Is.Not.Null, "Got Null for: " + su.EmailAddresses[0].EmailAddress);

            string newPassword = mu.ResetPassword();

            HttpContext x = HttpHelper.CreateHttpContext(string.Empty);

            bool result = SystemUser.AuthenticateUser(x.Response, su.EmailAddresses[0].EmailAddress, newPassword);
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Scenario: ResetPassword called against a null or empty email address
        /// Expected: Exception(User does not exist)
        /// </summary>
        [Test]
        public void _023_ResetPassword_InvalidEmail()
        {
            // null
            Assert.That(delegate { SystemUser.ResetPassword(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // empty 
            Assert.That(delegate { SystemUser.ResetPassword(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // non-existant 
            Assert.That(delegate { SystemUser.ResetPassword("user@does.not.exist"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("A user with emailaddress: user@does.not.exist does not exist"));
        }

        /// <summary>
        /// Scenario: ResetPassword called against a valid email address
        /// Expected: Password for user changes. Email notification sent (manual check required)
        /// </summary>
        [Test]
        public void _025_ResetPassword_ValidUser()
        {
            string validEmail = DbInterface.ExecuteQueryScalar<string>("membership", SystemUserDataQueries.UserName_AspNet_Users_Get);
            string passwordBefore = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(SystemUserDataQueries.Password_Get, validEmail));
            string returnValue = SystemUser.ResetPassword(validEmail);
            string passwordAfter = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(SystemUserDataQueries.Password_Get, validEmail));
            Assert.That(passwordAfter, Is.Not.EqualTo(passwordBefore));
        }

        /// <summary>
        /// Scenario: Update a user with a phone number
        /// Expected: Object update, properties match input data (other properties not changed)
        /// </summary>
        [Test]
        public void _026_UpdateUser_WithPhoneNumber()
        {
            Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", ContactDataQueries.ConfirmGuid_InSystemUser_EmailAddress_Get_Random);

            Contact ea = new Contact(emailAddressGuid);
            HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
            SystemUser su = new SystemUser(hc);
            su.PhoneNumber = "01234567891";
            SystemUser su2 = new SystemUser(hc);
            Assert.That(su2.PhoneNumber, Is.EqualTo("01234567891"));
        }

        /// <summary>
        /// Scenario: Property tested against known user in database
        /// Expected: Value returned matches database
        /// </summary>
        [Test]
        public void _027_FirstName()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_EmailAddress_All_Get_Random);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int systemUserId = Convert.ToInt32(dt.Rows[0]["SystemUserId"]);
                DataTable dt2 = DbInterface.ExecuteQueryDataTable("membership", string.Format(SystemUserDataQueries.SystemUser_Get_ById, systemUserId));
                Assert.That(dt2.Rows.Count, Is.EqualTo(1));
                Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(ContactDataQueries.ConfirmGuid_SelectBySystemUserId, systemUserId));
                Contact ea = new Contact(emailAddressGuid);
                HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
                SystemUser su = new SystemUser(hc);
                Assert.That(su.FirstName, Is.EqualTo(Convert.ToString(dt2.Rows[0]["FirstName"])), string.Format("Mismatch at row {0}", i));
            }
        }

        /// <summary>
        /// Scenario: Property tested against known user in database
        /// Expected: Value returned matches database
        /// </summary>
        [Test]
        public void _028_Surname()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_EmailAddress_All_Get_Random);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int systemUserId = Convert.ToInt32(dt.Rows[0]["SystemUserId"]);
                DataTable dt2 = DbInterface.ExecuteQueryDataTable("membership", string.Format(SystemUserDataQueries.SystemUser_Get_ById, systemUserId));
                Assert.That(dt2.Rows.Count, Is.EqualTo(1));
                Guid emailAddressGuid = DbInterface.ExecuteQueryScalar<Guid>("membership", string.Format(ContactDataQueries.ConfirmGuid_SelectBySystemUserId, systemUserId));
                Contact ea = new Contact(emailAddressGuid);
                HttpContext hc = HttpHelper.CreateHttpContext(ea.EmailAddress);
                SystemUser su = new SystemUser(hc);
                Assert.That(su.Surname, Is.EqualTo(Convert.ToString(dt2.Rows[0]["Surname"])), string.Format("Mismatch at row {0}", i));
            }
        }

        /// <summary>
        /// Create a System User. Add an email address, prove that it is successfully added (and still there after reloading). Then remove it and prove it is correctly removed.
        /// </summary>
        [Test]
        public void _029_AddAndRemoveEmailAddress()
        {
            string emailAddress1 = InternetDataGenerator.GenerateEmailAddress(19, "mattchedit.com");
            string emailAddress2 = InternetDataGenerator.GenerateEmailAddress(19, "mattchedit.com");
            int systemUserId = SystemUser.Create(Guid.Empty, emailAddress1, "password", "first", "surname", false, "02392987801", string.Empty);

            // get user
            SystemUser user1 = new SystemUser(systemUserId);

            // add new address
            user1.AddEmailAddress(emailAddress2, 2);
            Assert.That(user1.EmailAddresses.Length, Is.EqualTo(2));
            Assert.That(user1.EmailAddresses[0].EmailAddress, Is.EqualTo(emailAddress1));
            Assert.That(user1.EmailAddresses[1].EmailAddress, Is.EqualTo(emailAddress2));

            // reload and re-test
            SystemUser user2 = new SystemUser(systemUserId);
            Assert.That(user2.EmailAddresses.Length, Is.EqualTo(2));
            Assert.That(user2.EmailAddresses[0].EmailAddress, Is.EqualTo(emailAddress1));
            Assert.That(user2.EmailAddresses[1].EmailAddress, Is.EqualTo(emailAddress2));

            // remove an address
            user1.RemoveEmailAddress(emailAddress1);
            Assert.That(user1.EmailAddresses.Length, Is.EqualTo(1));
            Assert.That(user1.EmailAddresses[0].EmailAddress, Is.EqualTo(emailAddress2));

            // reload and re-test
            SystemUser user3 = new SystemUser(systemUserId);
            Assert.That(user3.EmailAddresses.Length, Is.EqualTo(1));
            Assert.That(user3.EmailAddresses[0].EmailAddress, Is.EqualTo(emailAddress2));
        }
    }
}
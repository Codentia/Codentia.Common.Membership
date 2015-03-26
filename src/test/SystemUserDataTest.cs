using System;
using System.Data;
using System.Web.Security;
using Codentia.Common.Data;
using Codentia.Common.Data.Caching;
using Codentia.Common.Logging.BL;
using Codentia.Common.Membership.Test.Creator;
using Codentia.Common.Membership.Test.Queries;
using Codentia.Test.Generator;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Common.Membership.Test
{
    /// <summary>
    /// TestFixture for SystemUserData
    /// <seealso cref="SystemUserData"/>
    /// </summary>
    [TestFixture]
    public class SystemUserDataTest
    {
        /// <summary>
        /// Ensure all set-up required for testing has been completed
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // enable diagnostic output from DataCache
            DataCache.ConsoleOutputEnabled = true;

            SystemUserDataCreator.CreateOnlySystemUsers("membership", 30, 120);
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "SystemUser"), Is.True, "No SystemUser data exists to test with");
        }

        /// <summary>
        /// Clear down 
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogManager.Instance.Dispose();
        }

        /// <summary>
        /// Scenario: Attempt to create a SystemUser row with UserId=Guid.Empty
        /// Expected: Exception(UserId is an empty Guid)
        /// </summary>
        [Test]
        public void _001_CreateSystemUser_InvalidMembershipUserId()
        {
            // empty
            Assert.That(delegate { SystemUserData.CreateSystemUser(Guid.NewGuid(), Guid.Empty, string.Empty, string.Empty, true, 0, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("userId is an empty Guid"));

            // non-existant
            Guid userId = Guid.NewGuid();
            Assert.That(delegate { SystemUserData.CreateSystemUser(Guid.NewGuid(), userId, string.Empty, string.Empty, true, 0, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("MembershipUser {0} does not exist", userId)));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateSystemUser with an invalid primary email address
        /// Expected: Invalid primary email address Exception raised for null and empty strings
        /// </summary>
        [Test]
        public void _002_CreateSystemUser_InvalidEmailAddressId()
        {
            string email = string.Format("{0}@mattchedit.com", Guid.NewGuid().ToString());
            MembershipUser mu = System.Web.Security.Membership.CreateUser(email, "Abcdefg1234567", email);

            // 0
            Assert.That(delegate { SystemUserData.CreateSystemUser(Guid.NewGuid(), new Guid(mu.ProviderUserKey.ToString()), "blah", "blah blah", true, 0, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("primaryEmailAddressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.CreateSystemUser(Guid.NewGuid(), new Guid(mu.ProviderUserKey.ToString()), "blah", "blah blah", true, -1, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("primaryEmailAddressId: -1 is not valid"));

            // non-existant
            email = string.Format("{0}@mattchedit.com", Guid.NewGuid().ToString());
            mu = System.Web.Security.Membership.CreateUser(email, "Abcdefg1234567", email);

            int emailAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
            Assert.That(delegate { SystemUserData.CreateSystemUser(Guid.NewGuid(), new Guid(mu.ProviderUserKey.ToString()), "blah", "blah blah", true, emailAddressIdNonExistant, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("primaryEmailAddressId: {0} does not exist", emailAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateSystemUser with an email address that has already been associated to a SystemUser
        /// Expected: NonExistant primary email address Exception raised for null and empty strings
        /// </summary>
        [Test]
        public void _003_CreateSystemUser_EmailAddressAlreadyRegistered()
        {
            string email = string.Format("{0}@mattchedit.com", Guid.NewGuid().ToString());
            MembershipUser mu = System.Web.Security.Membership.CreateUser(email, "Abcdefg1234567", email);
            int emailAddressExistantAssociated = DbInterface.ExecuteQueryScalar<int>("membership", ContactDataQueries.EmailAddressId_Select_AssociatedToSystemUser);
            Assert.That(delegate { SystemUserData.CreateSystemUser(Guid.NewGuid(), new Guid(mu.ProviderUserKey.ToString()), "blah", "blah blah", true, emailAddressExistantAssociated, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Email address has already been registered"));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateSystemUser with valid params
        /// Expected: Local User created
        /// </summary>
        [Test]
        public void _005_CreateSystemUser_ValidParams()
        {
            string email = string.Format("{0}@mattchedit.com", Guid.NewGuid().ToString());
            MembershipUser mu = System.Web.Security.Membership.CreateUser(email, "Abcdefg1234567", email);

            int emailAddressExistantNotAssociated = DbInterface.ExecuteQueryScalar<int>("membership", ContactDataQueries.EmailAddressId_Select_NotAssociatedToSystemUser);

            int newsystemUserId = 0;
            Guid txnId = Guid.NewGuid();
            try
            {
                newsystemUserId = SystemUserData.CreateSystemUser(txnId, new Guid(mu.ProviderUserKey.ToString()), "blah", "mysurname", true, emailAddressExistantNotAssociated, null);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(newsystemUserId, Is.GreaterThan(0), "_newsystemUserId must be greater than 0");

            int expectedId = SqlHelper.GetMaxIdFromTable("membership", "SystemUser");
            Assert.That(newsystemUserId, Is.EqualTo(expectedId));
        }

        /// <summary>
        /// Scenario: Attempt to call SystemUserExists with an existing valid id
        /// Expected: SystemUserExists returns true
        /// </summary>
        [Test]
        public void _006_SystemUserExists_Existant()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");

            Assert.That(SystemUserData.SystemUserExists(systemUserIdExistant), Is.True, "systemUserId does not exist");

            // txnl
            Guid txnId = Guid.NewGuid();
            try
            {
                Assert.That(SystemUserData.SystemUserExists(txnId, systemUserIdExistant), Is.True, "systemUserId does not exist");
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }
        }

        /// <summary>
        /// Scenario: Attempt to call SystemUserExists with an invalid id
        /// Expected: SystemUserExists returns false for -1 and non existant id
        /// </summary>
        [Test]
        public void _007_SystemUserExists_NonExistant()
        {
            int systemUserIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "SystemUser");
            Assert.That(SystemUserData.SystemUserExists(systemUserIdNonExistant), Is.False, "systemUserId exists");

            // txnl
            Guid txnId = Guid.NewGuid();
            try
            {
                Assert.That(SystemUserData.SystemUserExists(txnId, systemUserIdNonExistant), Is.False, "systemUserId exists");
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }
        }

        /// <summary>
        /// Scenario: Attempt to call GetSystemUser with an invalid id
        /// Expected: Invalid systemUserId Exception raised
        /// </summary>
        [Test]
        public void _008_GetSystemUser_ById_InvalidId()
        {
            // 0
            Assert.That(delegate { SystemUserData.GetSystemUser(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.GetSystemUser(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: -1 is not valid"));

            // non-existant
            int systemUserIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "SystemUser");
            Assert.That(delegate { SystemUserData.GetSystemUser(systemUserIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("systemUserId: {0} does not exist", systemUserIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetSystemUser with a valid existing id
        /// Expected: DataTable with 1 row returned
        /// </summary>
        [Test]
        public void _009_GetSystemUser_ById_ValidId_Existant()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");

            DataSet ds = SystemUserData.GetSystemUser(systemUserIdExistant);
            Assert.That(ds.Tables.Count, Is.EqualTo(2));

            DataTable dt = ds.Tables[0];
            Assert.That(dt.Rows.Count, Is.EqualTo(1), "Row count of 1 expected");

            // txnl
            Guid txnId = Guid.NewGuid();
            try
            {
                ds = SystemUserData.GetSystemUser(txnId, systemUserIdExistant);
                Assert.That(ds.Tables.Count, Is.EqualTo(2));

                dt = ds.Tables[0];
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(dt.Rows.Count, Is.EqualTo(1), "Row count of 1 expected");
        }

        /// <summary>
        /// Scenario: Attempt to call GetSystemUser with a invalid email address
        /// Expected: Invalid EmailAddress exception raised
        /// </summary>
        [Test]
        public void _010_GetSystemUser_ByEmail_InvalidEmailAddress()
        {
            // null
            Assert.That(delegate { SystemUserData.GetSystemUser(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // empty 
            Assert.That(delegate { SystemUserData.GetSystemUser(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress is not specified"));

            // invalid
            Assert.That(delegate { SystemUserData.GetSystemUser("blah"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress: blah is not a valid email address"));

            // non-existant
            Assert.That(delegate { SystemUserData.GetSystemUser("THISDOESNOTEXIST@Blah.com"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddress: THISDOESNOTEXIST@Blah.com does not exist"));
        }

        /// <summary>
        /// Scenario: Attempt to call GetSystemUser with a valid email address
        /// Expected: DataTable with 1 row returned
        /// </summary>
        [Test]
        public void _011_GetSystemUser_ByEmail_ValidEmailAddress()
        {
            string email = DbInterface.ExecuteQueryScalar<string>("membership", SystemUserDataQueries.SystemUser_EmailAddress_ExistingEmailAddress);
            DataSet ds = SystemUserData.GetSystemUser(email);
            Assert.That(ds.Tables.Count, Is.EqualTo(2));

            DataTable dt = ds.Tables[0];
            Assert.That(dt.Rows.Count, Is.EqualTo(1), "Row Count of 1 expected");

            // txnl
            email = DbInterface.ExecuteQueryScalar<string>("membership", SystemUserDataQueries.SystemUser_EmailAddress_ExistingEmailAddress);

            Guid txnId = Guid.NewGuid();
            try
            {
                ds = SystemUserData.GetSystemUser(txnId, email);
                dt = ds.Tables[0];
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(dt.Rows.Count, Is.EqualTo(1), "Row Count of 1 expected");
        }

        /// <summary>
        /// Scenario: Attempt to update SystemUser with an invalid systemUserId
        /// Expected: Invalid systemUserId Exception raised
        /// </summary>
        [Test]
        public void _012_UpdateSystemUser_InvalidSystemUserId()
        {
            // 0
            Assert.That(delegate { SystemUserData.UpdateSystemUser(0, "blah", "blah blah", true, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.UpdateSystemUser(-1, "blah", "blah blah", true, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: -1 is not valid"));
        }

        /// <summary>
        /// Scenario: Attempt to update SystemUser with an empty first name
        /// Expected: first name not specified Exceptions raised for null and empty strings
        /// </summary>
        [Test]
        public void _013_UpdateSystemUser_InvalidFirstName()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");

            // null
            Assert.That(delegate { SystemUserData.UpdateSystemUser(systemUserIdExistant, null, "blah blah", true, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("first name is not specified"));

            // empty 
            Assert.That(delegate { SystemUserData.UpdateSystemUser(systemUserIdExistant, string.Empty, "blah blah", true, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("first name is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to update SystemUser with an empty surname
        /// Expected: surname not specified Exceptions raised for null and empty strings
        /// </summary>
        [Test]
        public void _014_UpdateSystemUser_InvalidSurname()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");

            // null
            Assert.That(delegate { SystemUserData.UpdateSystemUser(systemUserIdExistant, "blah", null, true, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("surname is not specified"));

            // empty 
            Assert.That(delegate { SystemUserData.UpdateSystemUser(systemUserIdExistant, "blah", string.Empty, true, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("surname is not specified"));
        }

        /// <summary>
        /// Scenario: Attempt to update SystemUser with an invalid phone Number
        /// Expected: Raises Execption
        /// </summary>
        [Test]
        public void _015_UpdateSystemUser_InvalidPhoneNumber()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            Assert.That(delegate { SystemUserData.UpdateSystemUser(systemUserIdExistant, "blah", "blah blah", true, "Abcdefg1234567"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumber exceeds maxLength 13 as it has 14 chars"));
        }

        /// <summary>
        /// Scenario: Attempt to update SystemUser
        /// Expected: No Exception is raised
        /// </summary>
        [Test]
        public void _016_UpdateSystemUser_Valid()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", SystemUserDataQueries.SystemUser_Get_Random_10);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int systemUserIdExistant = Convert.ToInt32(dt.Rows[i]["systemUserId"]);
                DataTable dt2 = DbInterface.ExecuteQueryDataTable("membership", string.Format(SystemUserDataQueries.SystemUser_FirstAndLastNames, systemUserIdExistant));
                string firstname = Convert.ToString(dt2.Rows[0]["FirstName"]);
                string surname = Convert.ToString(dt2.Rows[0]["Surname"]);

                SystemUserData.UpdateSystemUser(systemUserIdExistant, "blah", "blah blah", true, null);
                SystemUserData.UpdateSystemUser(systemUserIdExistant, firstname, surname, true, null);

                DataTable dt3 = DbInterface.ExecuteQueryDataTable("membership", string.Format(SystemUserDataQueries.SystemUser_FirstAndLastNames, systemUserIdExistant));
                string firstname2 = Convert.ToString(dt2.Rows[0]["FirstName"]);
                string surname2 = Convert.ToString(dt2.Rows[0]["Surname"]);

                Assert.That(firstname, Is.EqualTo(firstname2));
                Assert.That(surname, Is.EqualTo(surname2));
            }
        }

        /// <summary>
        /// Scenario: Attempt to call SetForcePassword with an invalid id
        /// Expected: Invalid systemUserId Exception raised
        /// </summary>
        [Test]
        public void _017_SetForcePassword_InvalidId()
        {
            // 0
            Assert.That(delegate { SystemUserData.SetForcePassword(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.SetForcePassword(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: -1 is not valid"));
        }

        /// <summary>
        /// Scenario: Attempt to call SetForcePassword with an non-existant id
        /// Expected: Non-Existant systemUserId Exception raised
        /// </summary>
        [Test]
        public void _018_SetForcePassword_ValidId()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            SystemUserData.SetForcePassword(systemUserIdExistant);
            bool forcePass = DbInterface.ExecuteQueryScalar<bool>("membership", string.Format(SystemUserDataQueries.SystemUser_ForcePassword, systemUserIdExistant));
            Assert.That(forcePass, Is.True, "ForcePassword expected to be true");
        }

        /// <summary>
        /// Scenario: Attempt to call GetSystemUser with an invalid userid
        /// Expected: Invalid UserId Exception raised
        /// </summary>
        [Test]
        public void _019_GetSystemUser_ByUserId_InvalidUserId()
        {
            Assert.That(delegate { SystemUserData.GetSystemUser(Guid.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("userId is an empty Guid"));
        }

        /// <summary>
        /// Scenario: Attempt to call GetSystemUser with a valid existing id
        /// Expected: DataTable with 1 row returned
        /// </summary>
        [Test]
        public void _020_GetSystemUser_ByUserId_ValidUserId_Existant()
        {
            Guid guid = DbInterface.ExecuteQueryScalar<Guid>("membership", SystemUserDataQueries.SystemUserId_Get_RandomUserId);
            DataTable dt = SystemUserData.GetSystemUser(guid);
            Assert.That(dt.Rows.Count, Is.EqualTo(1), "Row count of 1 expected");
        }

        /// <summary>
        /// Scenario: Attempt to call SetRole with an Invalid SystemUserId
        /// Expected: Invalid systemUserId Exception raised
        /// </summary>
        [Test]
        public void _021_SetRole_Txl_InvalidSystemUser()
        {
            string roleNameExistant = DbInterface.ExecuteQueryScalar<string>("membership", SystemUserDataQueries.GetRandomRoleName);

            // 0
            Assert.That(delegate { SystemUserData.SetRole(Guid.NewGuid(), 0, roleNameExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.SetRole(Guid.NewGuid(), -1, roleNameExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: -1 is not valid"));

            // non-existant
            int systemUserIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "SystemUser");
            Assert.That(delegate { SystemUserData.SetRole(Guid.NewGuid(), systemUserIdNonExistant, roleNameExistant); }, Throws.InstanceOf<System.Data.SqlClient.SqlException>());
        }

        /// <summary>
        /// Scenario: Attempt to call SetRole with an Invalid roleName
        /// Expected: Invalid roleName Exception raised for null and empty strings
        /// </summary>
        [Test]
        public void _022_SetRole_Txl_InvalidRole()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");

            // null
            Assert.That(delegate { SystemUserData.SetRole(Guid.NewGuid(), systemUserIdExistant, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName is not specified"));

            // empty 
            Assert.That(delegate { SystemUserData.SetRole(Guid.NewGuid(), systemUserIdExistant, string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName is not specified"));

            // non-existant 
            Assert.That(delegate { SystemUserData.SetRole(Guid.NewGuid(), systemUserIdExistant, "NONEXISTRANTROLESHOULDNOTEXIST"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName: NONEXISTRANTROLESHOULDNOTEXIST does not exist"));
        }

        /// <summary>
        /// Scenario: Attempt to call SetRole with valid params
        /// Expected: Role Set
        /// </summary>
        [Test]
        public void _023_SetRole_Txl_ValidParams()
        {
            int systemUserIdWithUserId = DbInterface.ExecuteQueryScalar<int>("membership", SystemUserDataQueries.SystemUserId_Get_HasUserId);
            Assert.That(systemUserIdWithUserId, Is.GreaterThan(0), "systemUserIdWithUserId must be greater than 0");

            Guid txnId = Guid.NewGuid();
            string roleNameExistant = DbInterface.ExecuteQueryScalar<string>("membership", SystemUserDataQueries.GetRandomRoleName);

            try
            {
                SystemUserData.SetRole(txnId, systemUserIdWithUserId, roleNameExistant);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            string checkString = string.Format(SystemUserDataQueries.RoleCheck, systemUserIdWithUserId, roleNameExistant);

            Assert.That(DbInterface.ExecuteQueryScalar<bool>("membership", checkString), Is.True, "true expected");
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailsAddressForSystemUser with an invalid local user
        /// Expected: Invalid systemUserId Exception raised
        /// </summary>
        [Test]
        public void _024_GetEmailAddressForSystemUser_InvalidSystemUserId()
        {
            // 0
            Assert.That(delegate { SystemUserData.GetEmailAddressesForSystemUser(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.GetEmailAddressesForSystemUser(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: -1 is not valid"));

            // non-existant
            int systemUserIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "SystemUser");
            Assert.That(delegate { SystemUserData.GetEmailAddressesForSystemUser(systemUserIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("systemUserId: {0} does not exist", systemUserIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailsAddressForSystemUser with valid local user
        /// Expected: Correct number of emails returned
        /// </summary>
        [Test]
        public void _025_GetEmailAddressForSystemUser_ValidSystemUserId()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            DataTable dt1 = DbInterface.ExecuteQueryDataTable("membership", string.Format(ContactDataQueries.EmailAddress_Select_AssociatedToSystemUser, systemUserIdExistant));
            DataTable dt2 = SystemUserData.GetEmailAddressesForSystemUser(systemUserIdExistant);
            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.True, "Data Tables do not match");

            // txnl
            dt1 = DbInterface.ExecuteQueryDataTable("membership", string.Format(ContactDataQueries.EmailAddress_Select_AssociatedToSystemUser, systemUserIdExistant));

            Guid txnId = Guid.NewGuid();
            try
            {
                dt2 = SystemUserData.GetEmailAddressesForSystemUser(txnId, systemUserIdExistant);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.True, "Data Tables do not match");
        }

        /// <summary>
        /// Scenario: Attempt to call CreateSystemUser with invalid phone number
        /// Expected: Raises Exception
        /// </summary>
        [Test]
        public void _026_CreateSystemUser_PhoneNumber_InvalidValidParams()
        {
            string email = DataGenerator.GenerateEmail(8, "mattchedit.com");
            MembershipUser mu = System.Web.Security.Membership.CreateUser(email, "Abcdefg1234567", email);
            string phoneNumber = "015347386541575165776";

            int emailId = ContactData.CreateEmailAddress(email);

            int systemUserId = 0;
            Guid txnId = Guid.NewGuid();

            Assert.That(delegate { systemUserId = SystemUserData.CreateSystemUser(txnId, new Guid(mu.ProviderUserKey.ToString()), "blah", "mysurname", true, emailId, phoneNumber); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("phoneNumber exceeds maxLength 13 as it has 21 chars"));
            DbInterface.RollbackTransaction(txnId);
        }

        /// <summary>
        /// Scenario: Attempt to call CreateSystemUser with valid params
        /// Expected: System User created
        /// </summary>
        [Test]
        public void _027_CreateSystemUser_PhoneNumber_ValidParams()
        {
            string email = DataGenerator.GenerateEmail(8, "mattchedit.com");
            MembershipUser mu = System.Web.Security.Membership.CreateUser(email, "Abcdefg1234567", email);
            string phoneNumber = "01534738654";

            int emailId = ContactData.CreateEmailAddress(email);

            int systemUserId = 0;
            Guid txnId = Guid.NewGuid();

            try
            {
                systemUserId = SystemUserData.CreateSystemUser(txnId, new Guid(mu.ProviderUserKey.ToString()), "blah", "mysurname", true, emailId, phoneNumber);
                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
            }

            Assert.That(systemUserId, Is.GreaterThan(0));

            int expectedId = SqlHelper.GetMaxIdFromTable("membership", "SystemUser");
            Assert.That(systemUserId, Is.EqualTo(expectedId));

            int phoneNumberId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.PhoneNumberId_Get_ByPhoneNumber, phoneNumber));
            Assert.That(phoneNumberId, Is.GreaterThan(0));

            int userPhoneNumberId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.PhoneNumberId_Get_ForSystemUser, systemUserId));
            Assert.That(userPhoneNumberId, Is.EqualTo(phoneNumberId));
        }

        /// <summary>
        /// Scenario: Attempt to update SystemUser with PhoneNumber
        /// Expected: No Exception is raised
        /// </summary>
        [Test]
        public void _028_UpdateSystemUser_Valid()
        {
            int systemUserId = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            Assert.That(systemUserId, Is.GreaterThan(0));

            SystemUserData.UpdateSystemUser(systemUserId, "blah", "blah blah", true, "01534738653");

            int phoneNumberId = DbInterface.ExecuteQueryScalar<int>("membership", string.Format(ContactDataQueries.PhoneNumberId_Get_ByPhoneNumber, "01534738653"));

            DataTable dtCheck = DbInterface.ExecuteQueryDataTable("membership", string.Format(SystemUserDataQueries.SystemUser_Get_ById, systemUserId));
            Assert.That(dtCheck.Rows[0]["FirstName"], Is.EqualTo("blah"));
            Assert.That(dtCheck.Rows[0]["Surname"], Is.EqualTo("blah blah"));
            Assert.That(Convert.ToBoolean(dtCheck.Rows[0]["HasNewsLetter"]), Is.True);
            Assert.That(dtCheck.Rows[0]["PhoneNumberId"], Is.EqualTo(phoneNumberId));
        }

        /// <summary>
        /// Scenario: Attempt to call AssociateSystemUserToEmailAddress with an invalid EmailAddressId
        /// Expected: Invalid EmailAddressId exception raised
        /// </summary>
        [Test]
        public void _029_AssociateSystemUserToEmailAddress_InvalidEmailAddressId()
        {
            // 0
            Assert.That(delegate { SystemUserData.AssociateSystemUserToEmailAddress(0, 1, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.AssociateSystemUserToEmailAddress(-1, 1, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: -1 is not valid"));

            // non-existant
            int emailAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
            Assert.That(delegate { SystemUserData.AssociateSystemUserToEmailAddress(emailAddressIdNonExistant, 1, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("emailAddressId: {0} does not exist", emailAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an existing Email
        /// Expected: EmailAddress data returned
        /// </summary>
        [Test]
        public void _030_AssociateSystemUserToEmailAddress_InvalidSystemUserId()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // 0
            Assert.That(delegate { SystemUserData.AssociateSystemUserToEmailAddress(emailAddressId, 0, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.AssociateSystemUserToEmailAddress(emailAddressId, -1, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: -1 is not valid"));

            // non-existant
            int systemUserIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "SystemUser");
            Assert.That(delegate { SystemUserData.AssociateSystemUserToEmailAddress(emailAddressId, systemUserIdNonExistant, 1); }, Throws.InstanceOf<System.Data.SqlClient.SqlException>());
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an existing Email
        /// Expected: EmailAddress data returned
        /// </summary>
        [Test]
        public void _031_AssociateSystemUserToEmailAddress_InvalidEmailAddressOrder()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            int systemUserId = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            DbInterface.ExecuteQueryNoReturn("membership", string.Format(SystemUserDataQueries.SystemUser_EmailAddress_DeleteBySystemUser, systemUserId));

            // 0
            Assert.That(delegate { SystemUserData.AssociateSystemUserToEmailAddress(emailAddressId, systemUserId, 0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressOrder: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.AssociateSystemUserToEmailAddress(emailAddressId, systemUserId, -1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressOrder: -1 is not valid"));
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an existing Email
        /// Expected: EmailAddress data returned
        /// </summary>
        [Test]
        public void _032_AssociateSystemUserToEmailAddress_ValidParams()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            int systemUserId = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            DbInterface.ExecuteQueryNoReturn("membership", string.Format(SystemUserDataQueries.SystemUser_EmailAddress_DeleteBySystemUser, systemUserId));
            DbInterface.ExecuteQueryNoReturn("membership", string.Format(SystemUserDataQueries.SystemUser_EmailAddress_DeleteByEmailAddress, emailAddressId));
            SystemUserData.AssociateSystemUserToEmailAddress(emailAddressId, systemUserId, 1);
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", string.Format(SystemUserDataQueries.EmailAddressOrder_Select_BySystemUserAndEmailAddress, systemUserId, emailAddressId));
            Assert.That(dt.Rows.Count, Is.GreaterThan(0), "SystemUser and EmailAddress not associated correctly (row returned)");
            Assert.That(Convert.ToInt32(dt.Rows[0]["EmailAddressOrder"]), Is.EqualTo(1), "SystemUser and EmailAddress not associated correctly (emailaddressorder)");
        }

        /// <summary>
        /// Scenario: Attempt to call SetRole with an Invalid SystemUserId
        /// Expected: Invalid systemUserId Exception raised
        /// </summary>
        [Test]
        public void _033_SetRole_NonTxl_InvalidSystemUser()
        {
            string roleNameExistant = DbInterface.ExecuteQueryScalar<string>("membership", SystemUserDataQueries.GetRandomRoleName);

            // 0
            Assert.That(delegate { SystemUserData.SetRole(0, roleNameExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.SetRole(-1, roleNameExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: -1 is not valid"));
        }

        /// <summary>
        /// Scenario: Attempt to call SetRole with an Invalid roleName
        /// Expected: Invalid roleName Exception raised for null and empty strings
        /// </summary>
        [Test]
        public void _034_SetRole_NonTxl_InvalidRole()
        {
            int systemUserIdExistant = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");

            // null
            Assert.That(delegate { SystemUserData.SetRole(systemUserIdExistant, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName is not specified"));

            // empty
            Assert.That(delegate { SystemUserData.SetRole(systemUserIdExistant, string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName is not specified"));

            // non-existant
            Assert.That(delegate { SystemUserData.SetRole(systemUserIdExistant, "NONEXISTRANTROLESHOULDNOTEXIST"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("roleName: NONEXISTRANTROLESHOULDNOTEXIST does not exist"));
        }

        /// <summary>
        /// Scenario: Attempt to call SetRole with valid params
        /// Expected: Role Set
        /// </summary>
        [Test]
        public void _035_SetRole_NonTxl_ValidParams()
        {
            int systemUserIdWithUserId = DbInterface.ExecuteQueryScalar<int>("membership", SystemUserDataQueries.SystemUserId_Get_HasUserId);
            Assert.That(systemUserIdWithUserId, Is.GreaterThan(0), "systemUserIdWithUserId must be greater than 0");
            string roleNameExistant = DbInterface.ExecuteQueryScalar<string>("membership", SystemUserDataQueries.GetRandomRoleName);

            SystemUserData.SetRole(systemUserIdWithUserId, roleNameExistant);

            string checkString = string.Format(SystemUserDataQueries.RoleCheck, systemUserIdWithUserId, roleNameExistant);

            Assert.That(DbInterface.ExecuteQueryScalar<bool>("membership", checkString), Is.True, "true expected");
        }

        /// <summary>
        /// Scenario: Attempt to call AssociateSystemUserToEmailAddress with an invalid EmailAddressId
        /// Expected: Invalid EmailAddressId exception raised
        /// </summary>
        [Test]
        public void _035_DissociateSystemUserFromEmailAddress_InvalidEmailAddressId()
        {
            // 0
            Assert.That(delegate { SystemUserData.DissociateSystemUserFromEmailAddress(0, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.DissociateSystemUserFromEmailAddress(-1, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailAddressId: -1 is not valid"));

            // non-existant
            int emailAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "EmailAddress");
            Assert.That(delegate { SystemUserData.DissociateSystemUserFromEmailAddress(emailAddressIdNonExistant, 1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("emailAddressId: {0} does not exist", emailAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an existing Email
        /// Expected: EmailAddress data returned
        /// </summary>
        [Test]
        public void _036_DissociateSystemUserFromEmailAddress_InvalidSystemUserId()
        {
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");

            // 0
            Assert.That(delegate { SystemUserData.DissociateSystemUserFromEmailAddress(emailAddressId, 0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: 0 is not valid"));

            // -1 
            Assert.That(delegate { SystemUserData.DissociateSystemUserFromEmailAddress(emailAddressId, -1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("systemUserId: -1 is not valid"));
        }

        /// <summary>
        /// Scenario: Attempt to call GetEmailAddressData with an existing Email
        /// Expected: EmailAddress data returned
        /// </summary>
        [Test]
        public void _037_DissociateSystemUserFromEmailAddress_ValidParams()
        {
            // associate an address
            int emailAddressId = SqlHelper.GetRandomIdFromTable("membership", "EmailAddress");
            int systemUserId = SqlHelper.GetRandomIdFromTable("membership", "SystemUser");
            DbInterface.ExecuteQueryNoReturn("membership", string.Format(SystemUserDataQueries.SystemUser_EmailAddress_DeleteBySystemUser, systemUserId));
            DbInterface.ExecuteQueryNoReturn("membership", string.Format(SystemUserDataQueries.SystemUser_EmailAddress_DeleteByEmailAddress, emailAddressId));
            SystemUserData.AssociateSystemUserToEmailAddress(emailAddressId, systemUserId, 1);

            // now dissociate it
            SystemUserData.DissociateSystemUserFromEmailAddress(emailAddressId, systemUserId);
            
            // prove it's gone!
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", string.Format(SystemUserDataQueries.EmailAddressOrder_Select_BySystemUserAndEmailAddress, systemUserId, emailAddressId));                      
            Assert.That(dt.Rows.Count, Is.EqualTo(0), "SystemUser and EmailAddress not associated correctly (row returned)");
        }
    }
}
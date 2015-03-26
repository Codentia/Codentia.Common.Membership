using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using Codentia.Common.Helper;
using Codentia.Common.Logging;
using Codentia.Common.Logging.BL;

namespace Codentia.Common.Membership
{
    /// <summary>
    /// This class represents a user within the system, e.g. a registered user whose details have been stored and are
    /// available to ease the ordering process via login.
    /// </summary>
    public class SystemUser
    {
        private int _systemUserId;
        private string _firstName;
        private string _surname;
        private string _phoneNumber = string.Empty;
        private Guid _userId;
        private bool _forcePassword;
        private bool _hasNewsLetter;
        private Contact[] _emailAddresses;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemUser"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SystemUser(HttpContext context)
        {
            // get email address (username) from context
            DataSet ds = SystemUserData.GetSystemUser(context.User.Identity.Name);
            this.PopulateByDataRow(Guid.Empty, ds.Tables[0].Rows[0], ds.Tables[1]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemUser"/> class.
        /// </summary>
        /// <param name="systemUserId">The system user id.</param>
        public SystemUser(int systemUserId)
            : this(Guid.Empty, systemUserId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemUser"/> class.
        /// </summary>
        /// <param name="systemUser">The system user.</param>
        /// <param name="emailAddressData">The email address data.</param>
        internal SystemUser(DataRow systemUser, DataTable emailAddressData)
        {
            this.PopulateByDataRow(Guid.Empty, systemUser, emailAddressData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemUser"/> class.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="systemUserId">The system user id.</param>
        internal SystemUser(Guid txnId, int systemUserId)
        {
            DataSet ds = SystemUserData.GetSystemUser(txnId, systemUserId);
            this.PopulateByDataRow(txnId, ds.Tables[0].Rows[0], ds.Tables[1]);
        }

        /// <summary>
        /// Gets the system user id.
        /// </summary>
        public int SystemUserId
        {
            get
            {
                return _systemUserId;
            }
        }

        /// <summary>
        /// Gets the email addresses.
        /// </summary>
        public Contact[] EmailAddresses
        {
            get
            {
                return _emailAddresses;
            }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("firstName is not specified");
                }

                if (value != _firstName)
                {
                    _firstName = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the surname.
        /// </summary>
        /// <value>
        /// The surname.
        /// </value>
        public string Surname
        {
            get
            {
                return _surname;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("surname is not specified");
                }

                if (value != _surname)
                {
                    _surname = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        public Guid UserId
        {
            get
            {
                return _userId;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has news letter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has news letter; otherwise, <c>false</c>.
        /// </value>
        public bool HasNewsLetter
        {
            get
            {
                return _hasNewsLetter;
            }

            set
            {
                if (value != _hasNewsLetter)
                {
                    _hasNewsLetter = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets the membership user.
        /// </summary>
        public MembershipUser MembershipUser
        {
            get
            {
                return System.Web.Security.Membership.GetUser(_userId);
            }
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }

            set
            {
                if (value != _phoneNumber)
                {
                    _phoneNumber = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Create a brand new user (new registration) - returning the Id
        /// </summary>
        /// <param name="txnId">Id of ADO.NET transaction</param>
        /// <param name="emailAddress">Email Address for the user (Primary)</param>
        /// <param name="password">Password supplied by the user</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="surname">User's surname</param>
        /// <param name="hasNewsLetter">Has Newsletter</param>
        /// <param name="phoneNumber">User's contact phone number (optional)</param>
        /// <param name="defaultRole">User's default role (optional)</param>
        /// <returns>int - id</returns>
        public static int Create(Guid txnId, string emailAddress, string password, string firstName, string surname, bool hasNewsLetter, string phoneNumber, string defaultRole)
        {
            SystemUser su = CreateSystemUser(txnId, emailAddress, password, firstName, surname, hasNewsLetter, phoneNumber, defaultRole);
            return su.SystemUserId;
        }

        /// <summary>
        /// Create a brand new user (new registration)- returning the SystemUser object
        /// </summary>
        /// <param name="txnId">Id of ADO.NET transaction</param>
        /// <param name="emailAddress">Email Address for the user (Primary)</param>
        /// <param name="password">Password supplied by the user</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="surname">User's surname</param>
        /// <param name="hasNewsLetter">Has Newsletter</param>
        /// <param name="phoneNumber">User's contact phone number (optional)</param>
        /// <param name="defaultRole">User's default role (optional)</param>
        /// <returns>SystemUser object</returns>
        public static SystemUser CreateSystemUser(Guid txnId, string emailAddress, string password, string firstName, string surname, bool hasNewsLetter, string phoneNumber, string defaultRole)
        {
            ParameterCheckHelper.CheckIsValidString(emailAddress, "emailAddress", false);

            LogManager.Instance.AddToLog(LogMessageType.Information, "SystemUser", string.Format("SystemUser.Constructor emailAddress={0}, firstName={1}, surname={2}, hasNewsLetter={3}", emailAddress, firstName, surname, hasNewsLetter));
            int systemUserId = 0;

            try
            {
                MembershipUser mu = System.Web.Security.Membership.CreateUser(emailAddress, password, emailAddress);
                mu.IsApproved = true;
                System.Web.Security.Membership.UpdateUser(mu);

                Contact cn = ContactData.EmailAddressExists(txnId, emailAddress) ? new Contact(txnId, emailAddress) : Contact.CreateContact(txnId, emailAddress);

                DataTable dt = ContactData.GetEmailAddressData(txnId, emailAddress);
                int emailAddressId = Convert.ToInt32(dt.Rows[0]["EmailAddressId"]);

                systemUserId = SystemUserData.CreateSystemUser(txnId, new Guid(mu.ProviderUserKey.ToString()), firstName, surname, hasNewsLetter, emailAddressId, phoneNumber);

                if (!string.IsNullOrEmpty(defaultRole))
                {
                    SetRoleForSystemUser(txnId, systemUserId, defaultRole);
                }
            }
            catch (MembershipCreateUserException ex)
            {
                if (ex.Message.StartsWith("The password supplied is invalid"))
                {
                    throw new InvalidPasswordException("We were unable to create your account. Please ensure that you are using a 'strong' password containing several letters (both upper and lower case), and at least one number.", ex);
                }

                if (ex.Message.StartsWith("The username is already in use"))
                {
                    throw new DuplicateEmailAddressException("An account already exists for that email address. If you have forgotten your password, please use the links at the top of the page to receive a new one.", ex);
                }
            }

            return new SystemUser(txnId, systemUserId);
        }

        /// <summary>
        /// Perform authentication for a given username and password combination.
        /// If successful, set the appropriate cookies.
        /// </summary>
        /// <param name="response">Server Response object for the connection making the authentication request</param>
        /// <param name="emailAddress">EmailAddress of the user</param>
        /// <param name="password">Password supplied by the user</param>
        /// <returns>bool - authentication success</returns>
        public static bool AuthenticateUser(HttpResponse response, string emailAddress, string password)
        {
            bool authenticated = false;

            if (!string.IsNullOrEmpty(emailAddress) && !string.IsNullOrEmpty(password))
            {
                if (System.Web.Security.Membership.ValidateUser(emailAddress, password))
                {
                    HttpCookie authCookie = FormsAuthentication.GetAuthCookie(emailAddress, false);
                    authCookie.Domain = ConfigurationManager.AppSettings["WebSiteCookieDomain"];
                    response.Cookies.Add(authCookie);

                    Contact address = new Contact(emailAddress);

                    HttpCookie emailCookie = new HttpCookie(
                                                            string.Format(
                                                            "{0}_{1}",
                                                            ConfigurationManager.AppSettings["WebSiteCookieName"],
                                                            ConfigurationManager.AppSettings["WebSiteEmailAddressCookieName"]),
                                                            address.ConfirmGuid.ToString());

                    emailCookie.Expires = DateTime.Now.AddYears(1);
                    emailCookie.Domain = ConfigurationManager.AppSettings["WebSiteCookieDomain"];

                    response.Cookies.Add(emailCookie);

                    authenticated = true;
                }
            }

            return authenticated;
        }

        /// <summary>
        /// Reset the password for a given systemuser to a randomly generated value.
        /// </summary>
        /// <param name="emailAddress">Email Address identifying the user</param>
        /// <returns>new password</returns>
        public static string ResetPassword(string emailAddress)
        {
            ParameterCheckHelper.CheckIsValidString(emailAddress, "emailAddress", false);

            MembershipUser mu = null;

            if (!string.IsNullOrEmpty(emailAddress))
            {
                mu = System.Web.Security.Membership.GetUser(emailAddress);
            }

            if (mu == null)
            {
                throw new ArgumentException(string.Format("A user with emailaddress: {0} does not exist", emailAddress));
            }

            return mu.ResetPassword();
        }

        /// <summary>
        /// Sets the role.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        public void SetRole(string roleName)
        {
            SetRoleForSystemUser(Guid.Empty, _systemUserId, roleName);
        }

        /// <summary>
        /// Sets the force password.
        /// </summary>
        public void SetForcePassword()
        {
            SystemUserData.SetForcePassword(_systemUserId);
        }

        /// <summary>
        /// Change the user's password
        /// </summary>
        /// <param name="oldPassword">Old password (for confirmation)</param>
        /// <param name="newPassword">New password to use</param>
        /// <returns>true if password was changed</returns>
        public bool ChangePassword(string oldPassword, string newPassword)
        {
            MembershipUser mu = System.Web.Security.Membership.GetUser(_userId);

            return mu.ChangePassword(oldPassword, newPassword);
        }

        /// <summary>
        /// Adds the email address.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="emailAddressOrder">The email address order.</param>
        public void AddEmailAddress(string emailAddress, int emailAddressOrder)
        {
            Contact cn = ContactData.EmailAddressExists(emailAddress) ? new Contact(emailAddress) : Contact.CreateContact(emailAddress);
            DataTable dt = ContactData.GetEmailAddressData(emailAddress);
            int emailAddressId = Convert.ToInt32(dt.Rows[0]["EmailAddressId"]);

            SystemUserData.AssociateSystemUserToEmailAddress(emailAddressId, _systemUserId, emailAddressOrder);

            // reload data
            DataTable data = SystemUserData.GetEmailAddressesForSystemUser(_systemUserId);
            this.PopulateEmailAddresses(Guid.Empty, data);
        }

        /// <summary>
        /// Removes the email address.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        public void RemoveEmailAddress(string emailAddress)
        {
            for (int i = 0; i < _emailAddresses.Length; i++)
            {
                if (_emailAddresses[i].EmailAddress == emailAddress)
                {
                    SystemUserData.DissociateSystemUserFromEmailAddress(_emailAddresses[i].EmailAddressId, _systemUserId);
                }
            }

            // reload data
            DataTable data = SystemUserData.GetEmailAddressesForSystemUser(_systemUserId);
            this.PopulateEmailAddresses(Guid.Empty, data);
        }

        private static void SetRoleForSystemUser(Guid txnId, int systemUserId, string roleName)
        {
            ParameterCheckHelper.CheckIsValidString(roleName, "roleName", false);

            if (!Roles.RoleExists(roleName))
            {
                throw new ArgumentException(string.Format("roleName: {0} does not exist", roleName));
            }

            LogManager.Instance.AddToLog(LogMessageType.Information, "SystemUser", string.Format("SystemUser.SetRole _systemUserId={0}, roleName={1}", systemUserId, roleName));

            if (txnId == Guid.Empty)
            {
                SystemUserData.SetRole(systemUserId, roleName);
            }
            else
            {
                SystemUserData.SetRole(txnId, systemUserId, roleName);
            }
        }

        private void PopulateByDataRow(Guid txnId, DataRow systemUser, DataTable emailAddressData)
        {
            _systemUserId = Convert.ToInt32(systemUser["SystemUserId"]);
            _firstName = Convert.ToString(systemUser["FirstName"]);
            _surname = Convert.ToString(systemUser["Surname"]);
            _forcePassword = Convert.ToBoolean(systemUser["ForcePassword"]);
            _hasNewsLetter = Convert.ToBoolean(systemUser["HasNewsLetter"]);
            _phoneNumber = Convert.ToString(systemUser["PhoneNumber"]);
            _userId = new Guid(systemUser["UserId"].ToString());

            PopulateEmailAddresses(txnId, emailAddressData);
        }

        private void PopulateEmailAddresses(Guid txnId, DataTable emailAddressData)
        {
            List<Contact> emailAddresses = new List<Contact>();

            for (int i = 0; i < emailAddressData.Rows.Count; i++)
            {
                Contact cn = new Contact(txnId, emailAddressData.Rows[i]);
                emailAddresses.Add(cn);
            }

            _emailAddresses = emailAddresses.ToArray();
        }

        private void Save()
        {
            SystemUserData.UpdateSystemUser(_systemUserId, _firstName, _surname, _hasNewsLetter, _phoneNumber);
        }
    }
}

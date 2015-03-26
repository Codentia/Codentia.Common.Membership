using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Codentia.Common.Membership
{
    /// <summary>
    /// Contact class
    /// </summary>
    public class Contact : IContactDetails
    {
        private int _emailAddressId;
        private string _emailAddressText;
        private bool _isConfirmed;
        private int _emailAddressOrder;
        private Guid _confirmGuid;
        private Address[] _addresses;

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="emailAddressText">The email address text.</param>
        public Contact(string emailAddressText)
            : this(Guid.Empty, emailAddressText)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="emailAddressText">The email address text.</param>
        public Contact(Guid txnId, string emailAddressText)
        {
            _emailAddressText = emailAddressText;
            PopulateByEmailAddress(txnId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        public Contact(Guid cookie)
        {
            PopulateViaCookie(cookie);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contactCookieName">Name of the contact cookie.</param>
        public Contact(HttpContext context, string contactCookieName)
        {
            Guid emailAddressGuid = Guid.Empty;
            if (context.User.Identity.IsAuthenticated)
            {
                _emailAddressText = context.User.Identity.Name;
                PopulateByEmailAddress(Guid.Empty);
            }
            else
            {
                HttpCookie emailAddressCookie = context.Request.Cookies[contactCookieName];
                if (emailAddressCookie != null)
                {
                    if (!string.IsNullOrEmpty(emailAddressCookie.Value))
                    {
                        emailAddressGuid = new Guid(emailAddressCookie.Value);
                    }
                }

                if (emailAddressGuid == Guid.Empty)
                {
                    throw new ArgumentException("A valid emailaddress cannot be found, empty or missing Guid");
                }

                PopulateViaCookie(emailAddressGuid);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="emailAddressId">The email address id.</param>
        internal Contact(int emailAddressId)
            : this(Guid.Empty, emailAddressId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="emailAddressId">The email address id.</param>
        internal Contact(Guid txnId, int emailAddressId)
        {
            DataTable dt = ContactData.GetEmailAddressData(txnId, emailAddressId);
            PopulateByDataRow(dt.Rows[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="emailAddressData">The email address data.</param>
        internal Contact(Guid txnId, DataRow emailAddressData)
        {
            this.PopulateByDataRow(emailAddressData);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is confirmed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is confirmed; otherwise, <c>false</c>.
        /// </value>
        public bool IsConfirmed
        {
            get
            {
                return _isConfirmed;
            }
        }

        /// <summary>
        /// Gets the email address order.
        /// </summary>
        public int EmailAddressOrder
        {
            get
            {
                return _emailAddressOrder;
            }
        }

        /// <summary>
        /// Gets the confirm GUID.
        /// </summary>
        public Guid ConfirmGuid
        {
            get
            {
                return _confirmGuid;
            }
        }

        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <value>The addresses.</value>
        public Address[] Addresses
        {
            get
            {
                if (_addresses == null)
                {
                    _addresses = GetAddressesForContact();
                }

                return _addresses;
            }
        }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        /// <value>The email address.</value>
        public string EmailAddress
        {
            get
            {
                return _emailAddressText;
            }
        }

        /// <summary>
        /// Gets the email address id.
        /// </summary>
        internal int EmailAddressId
        {
            get
            {
                return _emailAddressId;
            }
        }

        /// <summary>
        /// Constructor (By Cookie) - get data
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>Contact object</returns>
        public static Contact CreateContact(string emailAddress)
        {
            return CreateContact(Guid.Empty, emailAddress);
        }

        /// <summary>
        /// Constructor (By Cookie) - get data
        /// </summary>
        /// <param name="txnId">transaction Id</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>Contact object</returns>
        public static Contact CreateContact(Guid txnId, string emailAddress)
        {
            if (!ContactData.EmailAddressExists(txnId, emailAddress))
            {
                int emailAddressId = ContactData.CreateEmailAddress(txnId, emailAddress);
                return new Contact(txnId, emailAddressId);
            }
            else
            {
                return new Contact(txnId, emailAddress);
            }
        }

        /// <summary>
        /// Confirms the email address.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="confirmGuid">The confirm GUID.</param>
        /// <returns>Is Confirmed</returns>
        public static bool ConfirmEmailAddress(string emailAddress, Guid confirmGuid)
        {
            return ContactData.Confirm(emailAddress, confirmGuid);
        }

        /// <summary>
        /// Gets the system user for email address.
        /// </summary>
        /// <returns>SystemUser object</returns>
        public SystemUser GetSystemUserForEmailAddress()
        {
            int systemUserId = ContactData.GetSystemUserIdForEmailAddress(_emailAddressId);

            if (systemUserId > 0)
            {
                return new SystemUser(Convert.ToInt32(systemUserId));
            }
            else
            {
                return null;
            }
        }

        private void PopulateViaCookie(Guid cookie)
        {
            DataTable dt = ContactData.GetEmailAddressData(cookie);
            if (dt.Rows.Count == 0)
            {
                throw new ArgumentException(string.Format("A valid emailaddress cannot be found, emailAddressGuid={0}", cookie));
            }

            PopulateByDataRow(dt.Rows[0]);
        }

        private void PopulateByEmailAddress(Guid txnId)
        {
            DataTable dt = ContactData.GetEmailAddressData(txnId, _emailAddressText);
            PopulateByDataRow(dt.Rows[0]);
        }

        private void PopulateByDataRow(DataRow dr)
        {
            _emailAddressId = Convert.ToInt32(dr["EmailAddressId"]);
            _emailAddressText = Convert.ToString(dr["EmailAddress"]);
            _emailAddressOrder = 0;

            if (dr["EmailAddressOrder"] != DBNull.Value)
            {
                _emailAddressOrder = Convert.ToInt32(dr["EmailAddressOrder"]);
            }

            _isConfirmed = Convert.ToBoolean(dr["IsConfirmed"]);
            _confirmGuid = new Guid(Convert.ToString(dr["ConfirmGuid"]));
        }

        private Address[] GetAddressesForContact()
        {
            DataTable dt = ContactData.GetAddressesForEmailAddress(_emailAddressId);
            List<Address> list = new List<Address>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new Address(dt.Rows[i]));
            }

            return list.ToArray();
        }
    }
}

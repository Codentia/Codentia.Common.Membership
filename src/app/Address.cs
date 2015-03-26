using System;
using System.Data;
using System.Web;

namespace Codentia.Common.Membership
{
    /// <summary>
    /// This class represents an Address within the system
    /// </summary>
    public class Address : IAddress
    {
        private int _addressId;
        private Country _country;
        private string _title = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _houseName;
        private string _street;
        private string _town;
        private string _city;
        private string _county;
        private string _postcode;
        private Guid _cookie;
        private bool _isCountryOnlyAddress;
        private int _emailAddressId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="addressId">The address id.</param>
        public Address(int addressId)
        {
            PopulateById(Guid.Empty, addressId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="dr">The dr.</param>
        public Address(DataRow dr)
        {
            PopulateByDataRow(dr);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        public Address(Guid cookie)
        {
            PopulateViaCookie(cookie);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="addressCookieName">Name of the address cookie.</param>
        /// <param name="emailCookieName">Name of the email cookie.</param>
        public Address(HttpContext context, string addressCookieName, string emailCookieName)
        {
            Contact cn = new Contact(context, emailCookieName);
            Guid addressGuid = Guid.Empty;

            HttpCookie addressCookie = context.Request.Cookies[addressCookieName];
            if (addressCookie != null)
            {
                if (!string.IsNullOrEmpty(addressCookie.Value))
                {
                    addressGuid = new Guid(addressCookie.Value);
                }
            }

            if (addressGuid == Guid.Empty)
            {
                throw new ArgumentException("A valid address cannot be found, empty or missing Guid");
            }

            if (!AddressData.AddressExists(addressGuid, cn.ConfirmGuid))
            {
                throw new ArgumentException(string.Format("Cookie mismatch: address record for addressGuid={0} does not match email address record for emailAddressGuid={1}", addressGuid.ToString(), cn.ConfirmGuid.ToString()));
            }

            PopulateViaCookie(addressGuid);
        }

        private Address(Guid txnId, int addressId)
        {
            PopulateById(txnId, addressId);
        }

        /// <summary>
        /// Gets the address id.
        /// </summary>
        public int AddressId
        {
            get
            {
                return _addressId;
            }
        }

        /// <summary>
        /// Gets the contact.
        /// </summary>
        public Contact Contact
        {
            get
            {
                return new Contact(_emailAddressId);
            }
        }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        public Country Country
        {
            get
            {
                return _country;
            }

            set
            {
                if (value == null || _country == null || _country.CountryId != value.CountryId)
                {
                    _country = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets the country.
        /// </summary>
        string IAddress.Country
        {
            get
            {
                return _country == null ? string.Empty : _country.DisplayText;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is country only address.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is country only address; otherwise, <c>false</c>.
        /// </value>
        public bool IsCountryOnlyAddress
        {
            get
            {
                return _isCountryOnlyAddress;
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title != value)
                {
                    _title = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the HouseName
        /// </summary>
        public string HouseName
        {
            get
            {
                return _houseName;
            }

            set
            {
                if (_houseName != value)
                {
                    _houseName = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Street
        /// </summary>
        public string Street
        {
            get
            {
                return _street;
            }

            set
            {
                if (_street != value)
                {
                    _street = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets Town
        /// </summary>
        public string Town
        {
            get
            {
                if (_town == null)
                {
                    _town = string.Empty;
                }

                return _town;
            }

            set
            {
                if (_town != value)
                {
                    _town = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the City
        /// </summary>
        public string City
        {
            get
            {
                return _city;
            }

            set
            {
                if (_city != value)
                {
                    _city = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the County
        /// </summary>
        public string County
        {
            get
            {
                return _county;
            }

            set
            {
                if (_county != value)
                {
                    _county = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Postcode
        /// </summary>
        public string Postcode
        {
            get
            {
                return _postcode;
            }

            set
            {
                if (_postcode != value)
                {
                    _postcode = value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Gets the cookie.
        /// </summary>
        public Guid Cookie
        {
            get
            {
                return _cookie;
            }
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
        /// <param name="postCode">The post code.</param>
        /// <param name="country">The country.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>
        /// Address object
        /// </returns>
        public static Address CreateAddress(string title, string firstName, string lastName, string houseName, string street, string town, string city, string county, string postCode, Country country, string emailAddress)
        {
            return CreateAddress(Guid.Empty, title, firstName, lastName, houseName, street, town, city, county, postCode, country, emailAddress);
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
        /// <param name="postCode">The post code.</param>
        /// <param name="country">The country.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>
        /// Address object
        /// </returns>
        public static Address CreateAddress(Guid txnId, string title, string firstName, string lastName, string houseName, string street, string town, string city, string county, string postCode, Country country, string emailAddress)
        {
            if (country == null)
            {
                throw new ArgumentException("country: was not specified");
            }

            Contact contact = null;

            if (ContactData.EmailAddressExists(emailAddress))
            {
                contact = new Contact(emailAddress);
            }
            else
            {
                contact = Contact.CreateContact(emailAddress);
            }

            int addressId = AddressData.CreateAddress(txnId, title, firstName, lastName, houseName, street, town, city, county, postCode, country.CountryId, contact.EmailAddressId);

            return new Address(txnId, addressId);
        }

        /// <summary>
        /// Create a Country Only address
        /// </summary>
        /// <param name="txnId">transaction Id</param>        
        /// <param name="countryId">Database Id of Country</param>
        /// <param name="emailAddressId">All addresses are associated to an emailAddressId</param>
        /// <returns>The Address</returns>
        public static Address CreateAddress(Guid txnId, int countryId, int emailAddressId)
        {
            int addressId = AddressData.CreateCountryOnlyAddress(txnId, countryId, emailAddressId);
            return new Address(txnId, addressId);
        }

        /// <summary>
        /// Return a list of countries in a format suitable for front-end binding and/or use
        /// </summary>
        /// <returns>LookupPair array</returns>
        public static LookupPair[] GetCountryList()
        {
            DataTable countries = CountryData.GetCountries();
            LookupPair[] results = new LookupPair[countries.Rows.Count];

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new LookupPair(Convert.ToString(countries.Rows[i]["CountryId"]), Convert.ToString(countries.Rows[i]["DisplayText"]));
            }

            return results;
        }

        /// <summary>
        /// ConcatenateAddress - provide address in delimited format
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="isPostCodeRequired">if set to <c>true</c> [is post code required].</param>
        /// <returns>
        /// string of concatenated address
        /// </returns>
        public string ConcatenateAddress(string delimiter, bool isPostCodeRequired)
        {
            string returnVal = string.Empty;
            string townString = string.Empty;

            if (!string.IsNullOrEmpty(_town))
            {
                townString = string.Format("{0}{1}", _town, delimiter);
            }

            if (_isCountryOnlyAddress)
            {
                returnVal = _country.DisplayText;
            }
            else
            {
                string name = string.Format("{0}{1}{2}{3}{4}", _title, string.IsNullOrEmpty(_title) ? string.Empty : " ", _firstName, string.IsNullOrEmpty(_firstName) ? string.Empty : " ", _lastName);

                if (!string.IsNullOrEmpty(name))
                {
                    returnVal = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", name, delimiter, _houseName, delimiter, _street, delimiter, townString, _city, delimiter, _county);
                }
                else
                {
                    returnVal = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", _houseName, delimiter, _street, delimiter, townString, _city, delimiter, _county);
                }

                if (isPostCodeRequired)
                {
                    returnVal = string.Format("{0}{1}{2}", returnVal, delimiter, _postcode);
                }

                returnVal = string.Format("{0}{1}{2}", returnVal, delimiter, _country.DisplayText);
            }

            return returnVal;
        }

        /// <summary>
        /// Updates from another address.
        /// </summary>
        /// <param name="add">The add.</param>
        public void UpdateFromAnotherAddress(Address add)
        {
            _title = add.Title;
            _firstName = add.FirstName;
            _lastName = add.LastName;
            _houseName = add.HouseName;
            _street = add.Street;
            _town = add.Town;
            _city = add.City;
            _county = add.County;
            _postcode = add.Postcode;
            _country = add.Country;

            Save();
        }

        private void PopulateViaCookie(Guid cookie)
        {
            DataTable dt = AddressData.GetAddressByCookie(cookie);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    int addressId = Convert.ToInt32(dt.Rows[0]["Addressid"]);
                    PopulateById(Guid.Empty, addressId);
                }
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        private void Save()
        {
            if (_isCountryOnlyAddress)
            {
                AddressData.UpdateCountryOnlyAddress(_addressId, _country.CountryId);
            }
            else
            {
                AddressData.UpdateAddress(_addressId, _title, _firstName, _lastName, _houseName, _street, _town, _city, _county, _postcode, _country.CountryId);
            }
        }

        /// <summary>
        /// PopulateById
        /// Populate the address object using the addressId 
        /// </summary>
        /// <param name="txnId">transaction Id</param>
        /// <param name="addressId">Database Id of Address</param>
        private void PopulateById(Guid txnId, int addressId)
        {
            DataTable dt = AddressData.GetAddressById(txnId, addressId);
            PopulateByDataRow(dt.Rows[0]);
        }

        private void PopulateByDataRow(DataRow dr)
        {
            // addressId is split so that an application can implement a different table for addresses other than Address
            // update optional members
            _title = Convert.ToString(dr["Title"]);
            _firstName = Convert.ToString(dr["FirstName"]);
            _lastName = Convert.ToString(dr["LastName"]);
            _town = Convert.ToString(dr["Town"]);

            // update non optional members
            _country = new Country(Convert.ToInt32(dr["CountryId"]));
            _houseName = Convert.ToString(dr["HouseName"]);
            _street = Convert.ToString(dr["Street"]);
            _city = Convert.ToString(dr["City"]);
            _county = Convert.ToString(dr["County"]);
            _postcode = Convert.ToString(dr["PostCode"]);
            _cookie = new Guid(Convert.ToString(dr["Cookie"]));
            _addressId = Convert.ToInt32(dr["AddressId"]);
            _emailAddressId = Convert.ToInt32(dr["EmailAddressId"]);

            if (_firstName == string.Empty && _houseName == string.Empty && _street == string.Empty)
            {
                _isCountryOnlyAddress = true;
            }
        }
    }
}
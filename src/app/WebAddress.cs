using System;
using System.Data;

namespace Codentia.Common.Membership
{
    /// <summary>
    /// This class represents a webaddress within the system, 
    /// </summary>
    public class WebAddress
    {
        private int _webAddressId;
        private string _url;
        private bool _isDead;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAddress"/> class.
        /// </summary>
        /// <param name="webAddressId">The web address id.</param>
        public WebAddress(int webAddressId)
            : this(Guid.Empty, webAddressId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAddress"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public WebAddress(string url)
            : this(Guid.Empty, url)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAddress"/> class.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="url">The URL.</param>
        public WebAddress(Guid txnId, string url)
        {
            DataTable dt = WebAddressData.GetWebAddressData(txnId, url);
            PopulateByDataRow(dt.Rows[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAddress"/> class.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="webAddressId">The web address id.</param>
        internal WebAddress(Guid txnId, int webAddressId)
        {
            _webAddressId = webAddressId;
            PopulateById(txnId);
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        public string URL
        {
            get
            {
                return _url;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dead.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dead; otherwise, <c>false</c>.
        /// </value>
        public bool IsDead
        {
            get
            {
                return _isDead;
            }
        }

        /// <summary>
        /// Gets the web address id.
        /// </summary>
        public int WebAddressId
        {
            get
            {
                return _webAddressId;
            }
        }

        /// <summary>
        /// Creates the web address.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="url">The URL.</param>
        /// <returns>WebAddress object</returns>
        public static WebAddress CreateWebAddress(Guid txnId, string url)
        {
            if (WebAddressData.WebAddressExists(txnId, url))
            {
                throw new ArgumentException(string.Format("url: {0} already exists", url));
            }

            int webAddressId = WebAddressData.CreateWebAddress(txnId, url);

            return new WebAddress(txnId, webAddressId);
        }

        private void PopulateById(Guid txnId)
        {
            DataTable dt = WebAddressData.GetWebAddressData(txnId, _webAddressId);
            PopulateByDataRow(dt.Rows[0]);
        }

        private void PopulateByDataRow(DataRow dr)
        {
            _webAddressId = Convert.ToInt32(dr["WebAddressId"]);
            _url = Convert.ToString(dr["URL"]);
            _isDead = Convert.ToBoolean(dr["IsDead"]);
        }
    }
}

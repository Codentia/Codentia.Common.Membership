namespace Codentia.Common.Membership
{
    /// <summary>
    /// Country class
    /// </summary>
    public class Country
    {
        private int _countryId;
        private string _displayText;

        /// <summary>
        /// Initializes a new instance of the <see cref="Country"/> class.
        /// </summary>
        /// <param name="displayText">The display text.</param>
        public Country(string displayText)
        {
            _displayText = displayText;
            _countryId = CountryData.GetCountryIdByDisplayText(displayText);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Country"/> class.
        /// </summary>
        /// <param name="countryId">The country id.</param>
        internal Country(int countryId)
        {
            _countryId = countryId;
            _displayText = CountryData.GetCountryDisplayText(countryId);
        }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        public string DisplayText
        {
            get
            {
                return _displayText;
            }
        }

        /// <summary>
        /// Gets the country id.
        /// </summary>
        public int CountryId
        {
            get
            {
                return _countryId;
            }
        }
    }
}

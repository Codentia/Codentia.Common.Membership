using System;
using System.Data;
using Codentia.Common.Data;
using Codentia.Common.Logging.BL;
using Codentia.Common.Membership.Test.Queries;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Common.Membership.Test
{
    /// <summary>
    /// TestFixture for CountryData
    /// <seealso cref="CountryData"/>
    /// </summary>
    [TestFixture]
    public class CountryDataTest
    {
        private int _countryIdNonExistant;
        private int _countryIdExistant;
        private string _countryDisplayTextExistant;

        /// <summary>
        /// Ensure all set-up required for testing has been completed
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("membership", "Country"), Is.True, "No Country data exists to test with");

            _countryIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Country");
            _countryIdExistant = SqlHelper.GetRandomIdFromTable("membership", "Country");
            Assert.That(_countryIdExistant, Is.GreaterThan(0), "_countryIdExistant expected to be greater than 0");

            _countryDisplayTextExistant = DbInterface.ExecuteQueryScalar<string>("membership", CountryDataQueries.CountryDisplayText_Get_Random);

            LogManager.Instance.Dispose();
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
        /// Scenario: Attempt to call GetCountryById with an invalid Id
        /// Expected: Invalid countryId Exception raised
        /// </summary>
        [Test]
        public void _001_GetCountryById_InvalidId()
        {
            // 0
            Assert.That(delegate { CountryData.GetCountryById(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: 0 is not valid"));

            // -1 
            Assert.That(delegate { CountryData.GetCountryById(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: -1 is not valid"));          
            
            // non-existant
            int countryIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Country");
            Assert.That(delegate { CountryData.GetCountryById(countryIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("countryId: {0} does not exist", countryIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetCountryById with an existing Id
        /// Expected: Row count of 1 expected
        /// </summary>
        [Test]
        public void _002_GetCountryById_ValidId_Existant()
        {
            DataTable dt = CountryData.GetCountryById(_countryIdExistant);
            Assert.That(dt.Rows.Count, Is.EqualTo(1), "Row count of 1 expected");
        }

        /// <summary>
        /// Scenario: Attempt to call GetCountryIdByDisplayText with an invalid countryDisplayText
        /// Expected: Invalid countryDisplayText Exceptions raised for null and empty strings
        /// </summary>
        [Test]
        public void _003_GetCountryIdByDisplayText_InvalidDisplayText()
        {
            // null
            Assert.That(delegate { CountryData.GetCountryIdByDisplayText(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("displayText is not specified"));

            // empty 
            Assert.That(delegate { CountryData.GetCountryIdByDisplayText(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("displayText is not specified"));            
   
            // non-existant
            Assert.That(delegate { CountryData.GetCountryIdByDisplayText("THISCOUNTRYCODEDOESNOTEXIST"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("displayText: THISCOUNTRYCODEDOESNOTEXIST does not exist"));            
        }

        /// <summary>
        /// Scenario: Attempt to call GetCountryIdByDisplayText with an existing countryDisplayText
        /// Expected: An int greater than 0 expected
        /// </summary>
        [Test]
        public void _004_GetCountryIdByDisplayText_ValidDisplayText()
        {
            int countryId = CountryData.GetCountryIdByDisplayText(_countryDisplayTextExistant);
            Assert.That(countryId, Is.GreaterThan(0), "countryId must be greater than 0");
        }

        /// <summary>
        /// Scenario: Attempt to call GetCountries
        /// Expected: Table row count is greater than 0
        /// </summary>
        [Test]
        public void _005_GetCountries()
        {
            DataTable dt = CountryData.GetCountries();
            Assert.That(dt.Rows.Count, Is.GreaterThan(0), "DataTable should have 1 or more rows");
            DataTable dt2 = DbInterface.ExecuteQueryDataTable("membership", CountryDataQueries.Country_Get_All);
            Assert.That(SqlHelper.CompareDataTables(dt, dt2), Is.True);
        }

        /// <summary>
        /// Scenario: Attempt to call CountryExists with an existing valid id
        /// Expected: CountryExists returns true
        /// </summary>
        [Test]
        public void _006_CountryExists_Existant()
        {
            Assert.That(CountryData.CountryExists(_countryIdExistant), Is.True, "countryId does not exist");
        }

        /// <summary>
        /// Scenario: Attempt to call CountryExists with an invalid id
        /// Expected: CountryExists returns false for -1 and non existant id
        /// </summary>
        [Test]
        public void _007_CountryExists_NonExistant()
        {
            Assert.That(CountryData.CountryExists(_countryIdNonExistant), Is.False, "countryId exists");
        }

        /// <summary>
        /// Scenario: Attempt to call GetCountryDisplayText with an invalid countryId
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _008_GetCountryDisplayText_InvalidId()
        {
            // 0
            Assert.That(delegate { CountryData.GetCountryDisplayText(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: 0 is not valid"));

            // -1 
            Assert.That(delegate { CountryData.GetCountryDisplayText(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("countryId: -1 is not valid"));

            // non-existant
            int countryIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "Country");
            Assert.That(delegate { CountryData.GetCountryDisplayText(countryIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("countryId: {0} does not exist", countryIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetCountryDisplayText with a valid countryId
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _009_GetCountryDisplayText_ValidParam()
        {
            int countryId = SqlHelper.GetRandomIdFromTable("membership", "Country");
            string expectedDesc = DbInterface.ExecuteQueryScalar<string>("membership", string.Format(CountryDataQueries.CountryDisplayText_Select, countryId));
            Assert.That(CountryData.GetCountryDisplayText(countryId), Is.EqualTo(expectedDesc));
        }
    }
}
using System;
using System.Data;
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
    /// TestFixture for WebAddressData
    /// <seealso cref="WebAddressData"/>
    /// </summary>
    [TestFixture]
    public class WebAddressDataTest
    {
        private int _webAddressIdExistant;
        private int _webAddressIdNonExistant;

        /// <summary>
        /// Ensure all set-up required for testing has been completed
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            InternetDataCreator.CreateOnlyWebAddresses("membership", 30);
            _webAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "WebAddress");
            _webAddressIdExistant = SqlHelper.GetRandomIdFromTable("membership", "WebAddress");
        }

        /// <summary>
        /// Perform test clean-up activities
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogManager.Instance.Dispose();
        }

        /// <summary>
        /// Scenario: Attempt to call GetWebAddressTypes
        /// Expected: Table row count is greater than 0
        /// </summary>
        [Test]
        public void _001_GetWebAddressTypes()
        {
            DataTable dt = WebAddressData.GetWebAddressTypes();
            Assert.That(dt.Rows.Count, Is.GreaterThan(0), "DataTable should have 1 or more rows");

            DataTable dt2 = DbInterface.ExecuteQueryDataTable("membership", WebAddressDataQueries.WebAddressType_Get_All);

            Assert.That(SqlHelper.CompareDataTables(dt, dt2), Is.True);
        }

        /// <summary>
        /// Scenario: Attempt to call WebAddressExists with an empty Web address
        /// Expected: Web Address not specified Exception raised for null and empty strings
        ///           Invalid Web address Exception raised for invalid Web address
        /// </summary>
        [Test]
        public void _002_WebAddressExists_Address_EmptyAddress()
        {
            // null
            Assert.That(delegate { WebAddressData.WebAddressExists(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("url is not specified"));

            // empty 
            Assert.That(delegate { WebAddressData.WebAddressExists(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("url is not specified"));            
        }

        /// <summary>
        /// Scenario: Attempt to call WebAddressExists with an existing address
        /// Expected: WebAddressExists returns true for check id 0, WebAddressExists returns false for check id _SystemUserIdForExistantWebAddress
        /// </summary>
        [Test]
        public void _003_WebAddressExists_Address_ExistingAddress()
        {
            string urlExistant = DbInterface.ExecuteQueryScalar<string>("membership", WebAddressDataQueries.URL_Get_Random);
            Assert.That(WebAddressData.WebAddressExists(urlExistant), Is.True, "Result should be true");
        }

        /// <summary>
        /// Scenario: Attempt to call WebAddressExists with an non-existant Web address
        /// Expected: WebAddressExists returns false
        /// </summary>
        [Test]
        public void _004_WebAddressExists_Address_NonExistantAddress()
        {
            Assert.That(WebAddressData.WebAddressExists("http:// www.THISISNOTANEXISTINGURL.COM"), Is.False, "Result should be false");
        }

        /// <summary>
        /// Scenario: Attempt to call WebAddressExists with an invalid Id
        /// Expected: Invalid Web Address Exception raised 
        /// </summary>
        [Test]
        public void _005_WebAddressExists_Id_InvalidId()
        {
            // 0
            Assert.That(delegate { WebAddressData.WebAddressExists(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("webAddressId: 0 is not valid"));

            // -1 
            Assert.That(delegate { WebAddressData.WebAddressExists(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("webAddressId: -1 is not valid"));          
        }

        /// <summary>
        /// Scenario: Attempt to call WebAddressExists with an existing address
        /// Expected: WebAddressExists returns true for check id 0, WebAddressExists returns false for check id _SystemUserIdForExistantWebAddress
        /// </summary>
        [Test]
        public void _006_WebAddressExists_Id_ExistingAddress()
        {
            int webAddressExistant = SqlHelper.GetRandomIdFromTable("membership", "WebAddress");
            Assert.That(WebAddressData.WebAddressExists(webAddressExistant), Is.True, "Result should be true");
        }

        /// <summary>
        /// Scenario: Attempt to call WebAddressExists with an non-existant web address
        /// Expected: WebAddressExists returns false
        /// </summary>
        [Test]
        public void _007_WebAddressExists_Id_NonExistantAddress()
        {
            int webAddressNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "WebAddress");
            Assert.That(WebAddressData.WebAddressExists(webAddressNonExistant), Is.False, "Result should be false");
        }

        /// <summary>
        /// Scenario: Attempt to call GetWebAddressData with an invalid Id
        /// Expected: Invalid Id Exception raised
        /// </summary>
        [Test]
        public void _008_GetWebAddressData_Id_InvalidId()
        {
            // 0
            Assert.That(delegate { WebAddressData.GetWebAddressData(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("webAddressId: 0 is not valid"));

            // -1
            Assert.That(delegate { WebAddressData.GetWebAddressData(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("webAddressId: -1 is not valid"));

            // non-existant
            int webAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "WebAddress");
            Assert.That(delegate { WebAddressData.GetWebAddressData(webAddressIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("webAddressId: {0} does not exist", webAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call GetWebAddressData with an existing Id
        /// Expected: WebAddress data returned
        /// </summary>
        [Test]
        public void _009_GetWebAddressData_Id_ExistingId()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", WebAddressDataQueries.WebAddressIdAndURL_Get_Random);
            int webAddressId = Convert.ToInt32(dt.Rows[0]["WebAddressId"]);
            string url = Convert.ToString(dt.Rows[0]["URL"]);

            DataTable dt2 = WebAddressData.GetWebAddressData(webAddressId);
            string urlCheck = Convert.ToString(dt2.Rows[0]["URL"]);

            Assert.That(url, Is.EqualTo(urlCheck), "URLs do not match");
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateWebAddressAsDead with an invalid id
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _010_UpdateWebAddressAsDead_InvalidId()
        {
            // 0
            Assert.That(delegate { WebAddressData.UpdateWebAddressAsDead(0); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("webAddressId: 0 is not valid"));

            // -1
            Assert.That(delegate { WebAddressData.UpdateWebAddressAsDead(-1); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("webAddressId: -1 is not valid"));

            // non-existant
            int webAddressIdNonExistant = SqlHelper.GetUnusedIdFromTable("membership", "WebAddress");
            Assert.That(delegate { WebAddressData.UpdateWebAddressAsDead(webAddressIdNonExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("webAddressId: {0} does not exist", webAddressIdNonExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call UpdateWebAddressAsDead with an valid existing id
        /// Expected: Runs Successfully
        /// </summary>
        [Test]
        public void _011_UpdateWebAddressAsDead_ValidParam()
        {
            int webAddressId = DbInterface.ExecuteQueryScalar<int>("membership", WebAddressDataQueries.WebAddressId_Get_Random_Alive);

            WebAddressData.UpdateWebAddressAsDead(webAddressId);

            DataTable dt = WebAddressData.GetWebAddressData(webAddressId);
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
            bool isDead = Convert.ToBoolean(dt.Rows[0]["IsDead"]);

            // reset first
            DbInterface.ExecuteQueryNoReturn("membership", string.Format(WebAddressDataQueries.WebAddress_UpdateAsAlive, webAddressId));

            Assert.That(isDead, Is.True);
        }

        /// <summary>
        /// Scenario: Attempt to call CreateWebAddress with an invalid url
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _012_CreateWebAddress_InvalidURL()
        {
            // null
            Assert.That(delegate { WebAddressData.CreateWebAddress(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("url is not specified"));

            // empty 
            Assert.That(delegate { WebAddressData.CreateWebAddress(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("url is not specified"));

            // existing
            string urlExistant = DbInterface.ExecuteQueryScalar<string>("membership", WebAddressDataQueries.URL_Get_Random);
            Assert.That(delegate { WebAddressData.CreateWebAddress(urlExistant); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(string.Format("url: {0} already exists", urlExistant)));
        }

        /// <summary>
        /// Scenario: Attempt to call CreateWebAddress with a valid url
        /// Expected: Runs Successfully
        /// </summary>
        [Test]
        public void _013_CreateWebAddress_ValidURL()
        {
            string domainType = DataGenerator.RetrieveRandomStringFromXmlDoc(InternetDataCreator.GetDomainType(), "domaintype");
            string url = string.Format("http:// www.{0}.{1}", Guid.NewGuid().ToString().Replace("-", string.Empty), domainType);

            int webAddressId = WebAddressData.CreateWebAddress(url);

            DataTable dt = WebAddressData.GetWebAddressData(webAddressId);
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
            Assert.That(Convert.ToString(dt.Rows[0]["URL"]), Is.EqualTo(url));
            Assert.That(Convert.ToBoolean(dt.Rows[0]["IsDead"]), Is.False);
        }

        /// <summary>
        /// Scenario: Check WebAddressType Enums both in class and in database
        /// Expected: Runs Successfully
        /// </summary>
        [Test]
        public void _014_WebAddressTypesCheck()
        {
            Assert.That((int)WebAddressType.None == 0, Is.True);
            Assert.That((int)WebAddressType.Blog == 1, Is.True);
            Assert.That((int)WebAddressType.Shop == 2, Is.True);

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", WebAddressDataQueries.WebAddressType_Get_All);

            string[] enums = Enum.GetNames(typeof(WebAddressType));

            // take off the None option from expected count
            Assert.That(dt.Rows.Count, Is.EqualTo(enums.Length - 1), "Total count of enums does not match database");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string code = Convert.ToString(dr["DisplayText"]);
                int id = Convert.ToInt32(dr["WebAddressTypeId"]);

                Assert.That(Enum.Format(typeof(WebAddressType), id, "F"), Is.EqualTo(code), string.Format("id for {0} incorrect", code));
            }
        }

        /// <summary>
        /// Scenario: Attempt to call GetWebAddressData with an invalid url
        /// Expected: Raises exceptions
        /// </summary>
        [Test]
        public void _015_GetWebAddressData_String_InvalidURL()
        {
            // null
            Assert.That(delegate { WebAddressData.GetWebAddressData(null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("url is not specified"));

            // empty 
            Assert.That(delegate { WebAddressData.GetWebAddressData(string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("url is not specified"));

            // exceeds max length            
            Assert.That(delegate { WebAddressData.GetWebAddressData("http:// THISADDRESSWILLNOTEXIST.Com"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("url: http:// THISADDRESSWILLNOTEXIST.Com does not exist"));  
        }

        /// <summary>
        /// Scenario: Attempt to call GetWebAddressData with an existing Id
        /// Expected: WebAddress data returned
        /// </summary>
        [Test]
        public void _016_GetWebAddressData_String_ExistingId()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", WebAddressDataQueries.WebAddressIdAndURL_Get_Random);
            int webAddressId = Convert.ToInt32(dt.Rows[0]["WebAddressId"]);
            string url = Convert.ToString(dt.Rows[0]["URL"]);

            DataTable dt2 = WebAddressData.GetWebAddressData(url);
            string urlActual = Convert.ToString(dt2.Rows[0]["URL"]);
            int webAddressIdActual = Convert.ToInt32(dt.Rows[0]["WebAddressId"]);

            Assert.That(urlActual, Is.EqualTo(url), "URLs do not match");
            Assert.That(webAddressIdActual, Is.EqualTo(webAddressId), "URLs do not match");
        }
    }
}
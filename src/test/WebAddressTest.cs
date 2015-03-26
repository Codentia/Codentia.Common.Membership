using System;
using System.Data;
using Codentia.Common.Data;
using Codentia.Common.Logging.BL;
using Codentia.Common.Membership.Test.Creator;
using Codentia.Common.Membership.Test.Queries;
using Codentia.Test.Generator;
using NUnit.Framework;

namespace Codentia.Common.Membership.Test
{
    /// <summary>
    /// This class serves as the unit testing fixture for the InterviewPage business entity
    /// <seealso cref="WebAddress"/>
    /// </summary>
    [TestFixture]
    public class WebAddressTest
    {
        /// <summary>
        /// Ensure all data required during test is set up
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            SystemUserDataCreator.CreateOnlySystemUsers("membership", 200, 500);
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
        /// Scenario: AllParams constructor with valid params
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _001_CreateWebAddress_ValidParam()
        {
            string url = InternetDataGenerator.GenerateRandomWebAddress();

            WebAddress wa = WebAddress.CreateWebAddress(Guid.Empty, url);
            Assert.That(wa.URL, Is.EqualTo(url));
            Assert.That(wa.IsDead, Is.False);

            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", string.Format(WebAddressDataQueries.WebAddress_SelectByURL, url));
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
            Assert.That(wa.WebAddressId, Is.EqualTo(Convert.ToInt32(dt.Rows[0]["WebAddressId"])));

            // check "internal" constructor - not main constructor - is only public as its used in outside libraries
            WebAddress wa2 = new WebAddress(wa.WebAddressId);
            Assert.That(wa2.URL, Is.EqualTo(wa.URL));
        }

        /// <summary>
        /// Scenario: AllParams constructor with valid params
        /// Expected: Runs succesfully
        /// </summary>
        [Test]
        public void _002_Constructor_URL_ValidParam()
        {
            int webAddressId = DbInterface.ExecuteQueryScalar<int>("membership", WebAddressDataQueries.WebAddressId_Get_Random_Alive);
            Assert.That(webAddressId, Is.GreaterThan(0));
            DataTable dt = DbInterface.ExecuteQueryDataTable("membership", string.Format(WebAddressDataQueries.WebAddress_SelectById, webAddressId));
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
            string url = Convert.ToString(dt.Rows[0]["URL"]);
            WebAddress wa = new WebAddress(url);
            Assert.That(wa.URL, Is.EqualTo(url));
            Assert.That(wa.IsDead, Is.False);
        }
    }
}
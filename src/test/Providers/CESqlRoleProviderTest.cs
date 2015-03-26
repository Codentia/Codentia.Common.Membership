using System.Collections.Specialized;
using System.Configuration;
using NUnit.Framework;

namespace Codentia.Common.Membership.Providers.Test
{
    /// <summary>
    /// TestFixture for CESqlRoleProvider
    /// <seealso cref="CESqlRoleProvider"/>
    /// </summary>
    [TestFixture]
    public class CESqlRoleProviderTest
    {
        /// <summary>
        /// Ensure all set-up required for testing has been completed
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // ConfigurationSectionGroup systemWeb = appConfig.GetSectionGroup("system.web");
            // appConfig.Sections.Remove("connectionStrings");
            // appConfig.Save();
            // ConfigurationManager.RefreshSection("connectionStrings");
        }

        /// <summary>
        /// Clear down 
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        /// <summary>
        /// Scenario: Just run the code for coverage
        /// Expected: 
        /// </summary>
        [Test]
        public void _001_Initialize()
        {
            CESqlRoleProvider p = new CESqlRoleProvider();
            NameValueCollection coll = new NameValueCollection();
            coll.Add("connectionStringName", "membership");
            p.Initialize("blah", coll);

            switch (System.Environment.MachineName)
            {
                case "MIDEV02":
                    Assert.That(CESqlMembershipProvider.ProviderDbDataSource, Is.EqualTo("membership"));
                    break;

                case "MIDEV01":
                    Assert.That(CESqlMembershipProvider.ProviderDbDataSource, Is.EqualTo("membership"));
                    break;

                case "SRV02":
                    Assert.That(CESqlMembershipProvider.ProviderDbDataSource, Is.EqualTo("membership"));
                    break;
            }
        }
    }
}
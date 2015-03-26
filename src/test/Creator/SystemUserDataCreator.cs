using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Xml;
using Codentia.Common.Data;
using Codentia.Common.Helper;
using Codentia.Common.Membership.Test.Queries;
using Codentia.Test;
using Codentia.Test.Generator;
using Codentia.Test.Helper;

namespace Codentia.Common.Membership.Test.Creator
{
    /// <summary>
    /// Common methods to create System Users
    /// </summary>
    [CoverageExclude]
    public class SystemUserDataCreator
    {
        /// <summary>
        /// Create a set of System Users
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="noOfUsers">no of Users</param>
        /// <param name="maxNoOfUsers">Max no of Users</param>
        public static void CreateOnlySystemUsers(string database, int noOfUsers, int maxNoOfUsers)
        {
            if (SqlHelper.GetRowCountFromTable(database, "EmailAddress") < maxNoOfUsers)
            {
                for (int i = 1; i < noOfUsers; i++)
                {
                    CreateSystemUser(database, i);
                }
            }
        }

        /// <summary>
        /// Create a set of System Users
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="noOfUsers">no of users</param>
        public static void CreateOnlySystemUsers(string database, int noOfUsers)
        {        
            for (int i = 1; i < noOfUsers; i++)
            {
                CreateSystemUser(database, i);
            }        
        }

        /// <summary>
        /// Creates the system user.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="emailAddressId">The email address id.</param>
        /// <param name="password">The password.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <returns>int of id</returns>
        public static int CreateSystemUser(string database, string emailAddress, int emailAddressId, string password, string firstName, string lastName)
        {
            MembershipUser mu = System.Web.Security.Membership.CreateUser(emailAddress, password, emailAddress);
            int systemUserId = 0;

            DbInterface.ExecuteQueryNoReturn(database, string.Format(SystemUserDataQueries.SystemUser_InsertNoPhoneNumber, new Guid(mu.ProviderUserKey.ToString()), firstName, lastName, 1));

            systemUserId = DbInterface.ExecuteQueryScalar<int>(database, string.Format(SystemUserDataQueries.SystemUserId_Get_ByUserId, new Guid(mu.ProviderUserKey.ToString())));

            DbInterface.ExecuteQueryNoReturn(database, string.Format(SystemUserDataQueries.SystemUser_EmailAddress_Insert, systemUserId, emailAddressId, 1));

            return systemUserId;
        }

        /// <summary>
        /// Create a system User for test data
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="i">seed for generating randomness</param>
        /// <returns>int of id</returns>
        public static int CreateSystemUser(string database, int i)
        {
            Dictionary<int, string> dict = InternetDataCreator.CreateRandomEmailAddress(database, 15, "mattchedit.com");

            Dictionary<int, string>.Enumerator ieDict = dict.GetEnumerator();
            ieDict.MoveNext();
            int emailAddressId = ieDict.Current.Key;
            string emailAddress = ieDict.Current.Value;

            string firstName = DataGenerator.RetrieveRandomStringFromXmlDoc(GetFirstName(), "firstname");
            string lastName = DataGenerator.RetrieveRandomStringFromXmlDoc(GetLastName(), "lastname");

            string password = DataGenerator.GeneratePassword(8);

            string p = System.Web.Security.Membership.Provider.Name;

            return CreateSystemUser(database, emailAddress, emailAddressId, password, firstName, lastName);
        }

        /// <summary>
        /// Get FirstName list and return as Xml Document
        /// </summary>
        /// <returns>XmlDocument object</returns>
        public static XmlDocument GetFirstName()
        {
            string list = "Ricky,Bianca,Karl,Victor,Margaret,Peggy,Magnus,James,Rose,Hannah,Hilary,Bill,William,Billy,Phoebe,Monica,Marlene,Chandler,Cassandra,Raquel,Damian,Ray,Roy,Gavin,Derek,Rodney,Albert,Alf,Fred,Brian,Roger,Tamsin,Taryn,Harry,Jamie,Saddam,Barry,Nathan,Brynn,Clive,Frank,Vanessa,Wayne,Bobby,Ross,Darren,Terry,Oliver,Thomas,Joseph,Cheryl,Melanie,Emma,Lawrence,Sam,Helen,Claire,Victoria,George,Ringo,Jeff,Geoff,Randy,Mia,Joel,Glenn,Howard,Michael,Paul,Peter,Rachel,Kerry,Adrian,Gareth,Danny,Fabio,Jermaine,Joe,Parys,Robert,John,Steve,Matt,Fred,Arthur,Darren,Tom,Rob,Paul,Dave,Phil,Chris,Colin,Graham,Betty,Jennifer,Alison,Philippa,Sue,Deborah,Annabel,Haley,Rosa";
            return XMLHelper.ConvertCSVStringToXmlDoc(list, "firstname");
        }

        /// <summary>
        /// Get LastName list and return as Xml Document
        /// </summary>
        /// <returns>XmlDocument object</returns>
        public static XmlDocument GetLastName()
        {
            string list = "Gervais,Merchant,Pilkington,Mitchell,Epstein,Meldrew,Clarke,Lowe,White,Black,Pyke,Milner,Clinton,Lewinsky,Boyce,Bing,Geller,Green,Buffay,Garnet,Trotter,Moore,Sayers,Hussein,Lerner,Redknapp,Borton,James,Hart,Carson,Rooney,Johnson,Woodcock,Southgate,Beckham,Edwards,Defoe,Kurn,Gerrard,Cole,Chiles,Smith,Jones,Hatchard,Chatterley,Thompson,Stephenson,Howe,Bedford,Leach,Bull,McCartney,Lennon,Harrison,Starkey,Clarke,Rogers,Webb,Ormrod,Harris,Taylor,Hunter,Stephens,Chisholm,Walcott,Barry";
            return XMLHelper.ConvertCSVStringToXmlDoc(list, "lastname");
        }
    }
}

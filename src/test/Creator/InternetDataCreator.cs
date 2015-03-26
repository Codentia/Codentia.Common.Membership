using System.Collections.Generic;
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
    /// Common methods for creating Internet related data    
    /// </summary>
    [CoverageExclude]
    public class InternetDataCreator
    {
        /// <summary>
        /// CreateRandom EmailAddress
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="seed">seed for generating randomness</param>
        /// <param name="domainName">email domain name</param>
        /// <returns>Dictionary - int, string</returns>
        public static Dictionary<int, string> CreateRandomEmailAddress(string database, int seed, string domainName)
        {
            string emailAddress = InternetDataGenerator.GenerateEmailAddress(seed, domainName);
                        
            return CreateSpecificEmailAddress(database, emailAddress);
        }

        /// <summary>
        /// CreateSpecificEmailAddress - create email if does not exist then return emailaddressId and emailaddress in dictionary
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="emailAddress">email Address</param>
        /// <returns>Dictionary - int, string</returns>
        public static Dictionary<int, string> CreateSpecificEmailAddress(string database, string emailAddress)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            int id = DbInterface.ExecuteQueryScalar<int>(database, string.Format(ContactDataQueries.EmailAddressId_Select, emailAddress));

            if (id > 0)
            {
               dict.Add(id, emailAddress);               
            }
            else
            {
                DbInterface.ExecuteQueryNoReturn(database, string.Format(ContactDataQueries.EmailAddress_Insert, emailAddress));
                int emailAddressId = SqlHelper.GetMaxIdFromTable(database, "EmailAddress");
                dict.Add(emailAddressId, emailAddress);                   
            }

            return dict;
        }
        
        /// <summary>
        /// Create Only EmailAddresses
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="noOfAddresses">No of Addresses</param>
        public static void CreateOnlyEmailAddresses(string database, int noOfAddresses)
        {
            for (int i = 0; i < noOfAddresses; i++)
                {
                    string emailAddress = InternetDataGenerator.GenerateEmailAddress(i + 3, "mattchedit.com");
                    DbInterface.ExecuteQueryNoReturn(database, string.Format(ContactDataQueries.EmailAddress_Insert, emailAddress));                   
                }
        }

        /// <summary>
        /// Create a random Web Address
        /// </summary>
        /// <param name="database">the database</param>
        /// <returns>int of id</returns>
        public static int CreateRandomWebAddress(string database)
        {
            string url = InternetDataGenerator.GenerateRandomWebAddress();

            DbInterface.ExecuteQueryNoReturn(database, string.Format(WebAddressDataQueries.WebAddress_Insert, url));
            return SqlHelper.GetMaxIdFromTable(database, "WebAddress");
        }      

        /// <summary>
        /// Create Only WebAddresses
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="noOfAddresses">No of Addresses</param>
        public static void CreateOnlyWebAddresses(string database, int noOfAddresses)
        {
            for (int i = 0; i < noOfAddresses; i++)
            {
                InternetDataCreator.CreateRandomWebAddress(database);
            }
        }

        /// <summary>
        /// Get WebAddressDomain list and return as Xml Document
        /// </summary>
        /// <returns>XmlDocument object</returns>
        public static XmlDocument GetWebAddressDomain()
        {
            string list = "mattchedit.com,mattchedit.co.uk,jerseycraftshop.com,jerseycraftshop.co.uk,shakeonit.org,gift-ted.com,gift-ted.co.uk,squidgyware.com,google.co.uk,google.com,microsoft.com,microsoft.co.uk,yahoo.com,yahoo.co.uk,bbc.co.uk";

            return XMLHelper.ConvertCSVStringToXmlDoc(list, "webaddress");
        }

        /// <summary>
        /// Get WebAddressDomain list and return as Xml Document
        /// </summary>
        /// <returns>XmlDocument object</returns>
        public static XmlDocument GetDomainType()
        {
            string list = "com,co.uk,org,biz,tv,au,uk.com";
            return XMLHelper.ConvertCSVStringToXmlDoc(list, "domaintype");
        }
    }
}

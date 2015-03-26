using System;

namespace Codentia.Common.Membership.Providers
{
    /// <summary>
    /// ProviderStrings class
    /// </summary>
    internal static class ProviderStrings
    {
        /// <summary>
        /// Gets the auth_rule_names_cant_contain_char.
        /// </summary>
        internal static string Auth_rule_names_cant_contain_char
        {
            get { return "Authorization rule names cannot contain the '{0}' character."; }
        }

        /// <summary>
        /// Gets the connection_name_not_specified.
        /// </summary>
        internal static string Connection_name_not_specified
        {
            get { return "The attribute 'connectionStringName' is missing or empty."; }
        }

        /// <summary>
        /// Gets the connection_string_not_found.
        /// </summary>
        internal static string Connection_string_not_found
        {
            get { return "The connection name '{0}' was not found in the applications configuration or the connection string is empty."; }
        }

        /// <summary>
        /// Gets the membership_ account lock out.
        /// </summary>
        internal static string Membership_AccountLockOut
        {
            get { return "The user account has been locked out."; }
        }

        /// <summary>
        /// Gets the membership_ custom_ password_ validation_ failure.
        /// </summary>
        internal static string Membership_Custom_Password_Validation_Failure
        {
            get { return "The custom password validation failed."; }
        }

        /// <summary>
        /// Gets the membership_ invalid answer.
        /// </summary>
        internal static string Membership_InvalidAnswer
        {
            get { return "The password-answer supplied is invalid."; }
        }

        /// <summary>
        /// Gets the membership_ invalid email.
        /// </summary>
        internal static string Membership_InvalidEmail
        {
            get { return "The E-mail supplied is invalid."; }
        }

        /// <summary>
        /// Gets the membership_ invalid password.
        /// </summary>
        internal static string Membership_InvalidPassword
        {
            get { return "The password supplied is invalid.  Passwords must conform to the password strength requirements configured for the default provider."; }
        }

        /// <summary>
        /// Gets the membership_ invalid provider user key.
        /// </summary>
        internal static string Membership_InvalidProviderUserKey
        {
            get { return "The provider user key supplied is invalid.  It must be of type System.Guid."; }
        }

        /// <summary>
        /// Gets the membership_ invalid question.
        /// </summary>
        internal static string Membership_InvalidQuestion
        {
            get { return "The password-question supplied is invalid.  Note that the current provider configuration requires a valid password question and answer.  As a result, a CreateUser overload that accepts question and answer parameters must also be used."; }
        }

        /// <summary>
        /// Gets the membership_more_than_one_user_with_email.
        /// </summary>
        internal static string Membership_more_than_one_user_with_email
        {
            get { return "More than one user has the specified e-mail address."; }
        }

        /// <summary>
        /// Gets the membership_password_too_long.
        /// </summary>
        internal static string Membership_password_too_long
        {
            get { return "The password is too long: it must not exceed 128 chars after encrypting."; }
        }

        /// <summary>
        /// Gets the membership_ password retrieval_not_supported.
        /// </summary>
        internal static string Membership_PasswordRetrieval_not_supported
        {
            get { return "This Membership Provider has not been configured to support password retrieval."; }
        }

        /// <summary>
        /// Gets the membership_ user not found.
        /// </summary>
        internal static string Membership_UserNotFound
        {
            get { return "The user was not found."; }
        }

        /// <summary>
        /// Gets the membership_ wrong answer.
        /// </summary>
        internal static string Membership_WrongAnswer
        {
            get { return "The password-answer supplied is wrong."; }
        }

        /// <summary>
        /// Gets the membership_ wrong password.
        /// </summary>
        internal static string Membership_WrongPassword
        {
            get { return "The password supplied is wrong."; }
        }

        /// <summary>
        /// Gets the page index_bad.
        /// </summary>
        internal static string PageIndex_bad
        {
            get { return "The pageIndex must be greater than or equal to zero."; }
        }

        /// <summary>
        /// Gets the page index_ page size_bad.
        /// </summary>
        internal static string PageIndex_PageSize_bad
        {
            get { return "The combination of pageIndex and pageSize cannot exceed the maximum value of System.Int32."; }
        }

        /// <summary>
        /// Gets the page size_bad.
        /// </summary>
        internal static string PageSize_bad
        {
            get { return "The pageSize must be greater than zero."; }
        }

        /// <summary>
        /// Gets the parameter_array_empty.
        /// </summary>
        internal static string Parameter_array_empty
        {
            get { return "The array parameter '{0}' should not be empty."; }
        }

        /// <summary>
        /// Gets the parameter_can_not_be_empty.
        /// </summary>
        internal static string Parameter_can_not_be_empty
        {
            get { return "The parameter '{0}' must not be empty."; }
        }

        /// <summary>
        /// Gets the parameter_can_not_contain_comma.
        /// </summary>
        internal static string Parameter_can_not_contain_comma
        {
            get { return "The parameter '{0}' must not contain commas."; }
        }

        /// <summary>
        /// Gets the parameter_duplicate_array_element.
        /// </summary>
        internal static string Parameter_duplicate_array_element
        {
            get { return "The array '{0}' should not contain duplicate values."; }
        }

        /// <summary>
        /// Gets the parameter_too_long.
        /// </summary>
        internal static string Parameter_too_long
        {
            get { return "The parameter '{0}' is too long: it must not exceed {1} chars in length."; }
        }

        /// <summary>
        /// Gets the password_does_not_match_regular_expression.
        /// </summary>
        internal static string Password_does_not_match_regular_expression
        {
            get { return "The parameter '{0}' does not match the regular expression specified in config file."; }
        }

        /// <summary>
        /// Gets the password_need_more_non_alpha_numeric_chars.
        /// </summary>
        internal static string Password_need_more_non_alpha_numeric_chars
        {
            get { return "Non alpha numeric characters in '{0}' needs to be greater than or equal to '{1}'."; }
        }

        /// <summary>
        /// Gets the password_too_short.
        /// </summary>
        internal static string Password_too_short
        {
            get { return "The length of parameter '{0}' needs to be greater or equal to '{1}'."; }
        }

        /// <summary>
        /// Gets the length of the personalization provider_ application name exceed max.
        /// </summary>
        /// <value>
        /// The length of the personalization provider_ application name exceed max.
        /// </value>
        internal static string PersonalizationProvider_ApplicationNameExceedMaxLength
        {
            get { return "The ApplicationName cannot exceed character length {0}."; }
        }

        /// <summary>
        /// Gets the personalization provider_ bad connection.
        /// </summary>
        internal static string PersonalizationProvider_BadConnection
        {
            get { return "The specified connectionStringName, '{0}', was not registered."; }
        }

        /// <summary>
        /// Gets the personalization provider_ cant access.
        /// </summary>
        internal static string PersonalizationProvider_CantAccess
        {
            get { return "A connection could not be made by the {0} personalization provider using the specified registration."; }
        }

        /// <summary>
        /// Gets the personalization provider_ no connection.
        /// </summary>
        internal static string PersonalizationProvider_NoConnection
        {
            get { return "The connectionStringName attribute must be specified when registering a personalization provider."; }
        }

        /// <summary>
        /// Gets the personalization provider_ unknown prop.
        /// </summary>
        internal static string PersonalizationProvider_UnknownProp
        {
            get { return "Invalid attribute '{0}', specified in the '{1}' personalization provider registration."; }
        }

        /// <summary>
        /// Gets the profile SQL provider_description.
        /// </summary>
        internal static string ProfileSqlProvider_description
        {
            get { return "SQL profile provider."; }
        }

        /// <summary>
        /// Gets the property_ had_ malformed_ URL.
        /// </summary>
        internal static string Property_Had_Malformed_Url
        {
            get { return "The '{0}' property had a malformed URL: {1}."; }
        }

        /// <summary>
        /// Gets the provider_application_name_too_long.
        /// </summary>
        internal static string Provider_application_name_too_long
        {
            get { return "The application name is too long."; }
        }

        /// <summary>
        /// Gets the provider_bad_password_format.
        /// </summary>
        internal static string Provider_bad_password_format
        {
            get { return "Password format specified is invalid."; }
        }

        /// <summary>
        /// Gets the provider_can_not_retrieve_hashed_password.
        /// </summary>
        internal static string Provider_can_not_retrieve_hashed_password
        {
            get { return "Configured settings are invalid: Hashed passwords cannot be retrieved. Either set the password format to different type, or set supportsPasswordRetrieval to false."; }
        }

        /// <summary>
        /// Gets the provider_ error.
        /// </summary>
        internal static string Provider_Error
        {
            get { return "The Provider encountered an unknown error."; }
        }

        /// <summary>
        /// Gets the provider_ not_ found.
        /// </summary>
        internal static string Provider_Not_Found
        {
            get { return "Provider '{0}' was not found."; }
        }

        /// <summary>
        /// Gets the provider_role_already_exists.
        /// </summary>
        internal static string Provider_role_already_exists
        {
            get { return "The role '{0}' already exists."; }
        }

        /// <summary>
        /// Gets the provider_role_not_found.
        /// </summary>
        internal static string Provider_role_not_found
        {
            get { return "The role '{0}' was not found."; }
        }

        /// <summary>
        /// Gets the provider_ schema_ version_ not_ match.
        /// </summary>
        internal static string Provider_Schema_Version_Not_Match
        {
            get { return "The '{0}' requires a database schema compatible with schema version '{1}'.  However, the current database schema is not compatible with this version.  You may need to either install a compatible schema with aspnet_regsql.exe (available in the framework installation directory), or upgrade the provider to a newer version."; }
        }

        /// <summary>
        /// Gets the provider_this_user_already_in_role.
        /// </summary>
        internal static string Provider_this_user_already_in_role
        {
            get { return "The user '{0}' is already in role '{1}'."; }
        }

        /// <summary>
        /// Gets the provider_this_user_not_found.
        /// </summary>
        internal static string Provider_this_user_not_found
        {
            get { return "The user '{0}' was not found."; }
        }

        /// <summary>
        /// Gets the provider_unknown_failure.
        /// </summary>
        internal static string Provider_unknown_failure
        {
            get { return "Stored procedure call failed."; }
        }

        /// <summary>
        /// Gets the provider_unrecognized_attribute.
        /// </summary>
        internal static string Provider_unrecognized_attribute
        {
            get { return "Attribute not recognized '{0}'"; }
        }

        /// <summary>
        /// Gets the provider_user_not_found.
        /// </summary>
        internal static string Provider_user_not_found
        {
            get { return "The user was not found in the database."; }
        }

        /// <summary>
        /// Gets the role_is_not_empty.
        /// </summary>
        internal static string Role_is_not_empty
        {
            get { return "This role cannot be deleted because there are users present in it."; }
        }

        /// <summary>
        /// Gets the role SQL provider_description.
        /// </summary>
        internal static string RoleSqlProvider_description
        {
            get { return "SQL role provider."; }
        }

        /// <summary>
        /// Gets the site map provider_cannot_remove_root_node.
        /// </summary>
        internal static string SiteMapProvider_cannot_remove_root_node
        {
            get { return "Root node cannot be removed from the providers, use RemoveProvider(string providerName) instead."; }
        }

        /// <summary>
        /// Gets the SQL error_ connection_ string.
        /// </summary>
        internal static string SqlError_Connection_String
        {
            get { return "An error occurred while attempting to initialize a System.Data.SqlClient.SqlConnection object. The value that was provided for the connection string may be wrong, or it may contain an invalid syntax."; }
        }

        /// <summary>
        /// Gets the SQL express_file_not_found_in_connection_string.
        /// </summary>
        internal static string SqlExpress_file_not_found_in_connection_string
        {
            get { return "SQL Express filename was not found in the connection string."; }
        }

        /// <summary>
        /// Gets the SQL personalization provider_ description.
        /// </summary>
        internal static string SqlPersonalizationProvider_Description
        {
            get { return "Personalization provider that stores data in a SQL Server database."; }
        }

        /// <summary>
        /// Gets the value_must_be_boolean.
        /// </summary>
        internal static string Value_must_be_boolean
        {
            get { return "The value must be boolean (true or false) for property '{0}'."; }
        }

        /// <summary>
        /// Gets the value_must_be_non_negative_integer.
        /// </summary>
        internal static string Value_must_be_non_negative_integer
        {
            get { return "The value must be a non-negative 32-bit integer for property '{0}'."; }
        }

        /// <summary>
        /// Gets the value_must_be_positive_integer.
        /// </summary>
        internal static string Value_must_be_positive_integer
        {
            get { return "The value must be a positive 32-bit integer for property '{0}'."; }
        }

        /// <summary>
        /// Gets the value_too_big.
        /// </summary>
        internal static string Value_too_big
        {
            get { return "The value '{0}' can not be greater than '{1}'."; }
        }

        /// <summary>
        /// Gets the XML site map provider_cannot_add_node.
        /// </summary>
        internal static string XmlSiteMapProvider_cannot_add_node
        {
            get { return "SiteMapNode {0} cannot be found in current provider, only nodes in the same provider can be added."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ cannot_ be_ inited_ twice.
        /// </summary>
        internal static string XmlSiteMapProvider_Cannot_Be_Inited_Twice
        {
            get { return "XmlSiteMapProvider cannot be initialized twice."; }
        }

        /// <summary>
        /// Gets the XML site map provider_cannot_find_provider.
        /// </summary>
        internal static string XmlSiteMapProvider_cannot_find_provider
        {
            get { return "Provider {0} cannot be found inside XmlSiteMapProvider {1}."; }
        }

        /// <summary>
        /// Gets the XML site map provider_cannot_remove_node.
        /// </summary>
        internal static string XmlSiteMapProvider_cannot_remove_node
        {
            get { return "SiteMapNode {0} does not exist in provider {1}, it must be removed from provider {2}."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ description.
        /// </summary>
        internal static string XmlSiteMapProvider_Description
        {
            get { return "SiteMap provider which reads in .sitemap XML files."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ error_loading_ config_file.
        /// </summary>
        internal static string XmlSiteMapProvider_Error_loading_Config_file
        {
            get { return "The XML sitemap config file {0} could not be loaded.  {1}"; }
        }

        /// <summary>
        /// Gets the XML site map provider_ file name_already_in_use.
        /// </summary>
        internal static string XmlSiteMapProvider_FileName_already_in_use
        {
            get { return "The sitemap config file {0} is already used by other nodes or providers."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ file name_does_not_exist.
        /// </summary>
        internal static string XmlSiteMapProvider_FileName_does_not_exist
        {
            get { return "The file {0} required by XmlSiteMapProvider does not exist."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ invalid_ extension.
        /// </summary>
        internal static string XmlSiteMapProvider_Invalid_Extension
        {
            get { return "The file {0} has an invalid extension, only .sitemap files are allowed in XmlSiteMapProvider."; }
        }

        /// <summary>
        /// Gets the XML site map provider_invalid_ get root node core.
        /// </summary>
        internal static string XmlSiteMapProvider_invalid_GetRootNodeCore
        {
            get { return "GetRootNode is returning null from Provider {0}, this method must return a non-empty sitemap node."; }
        }

        /// <summary>
        /// Gets the XML site map provider_invalid_resource_key.
        /// </summary>
        internal static string XmlSiteMapProvider_invalid_resource_key
        {
            get { return "Resource key {0} is not valid, it must contain a valid class name and key pair. For example, $resources:'className','key'"; }
        }

        /// <summary>
        /// Gets the XML site map provider_invalid_sitemapnode_returned.
        /// </summary>
        internal static string XmlSiteMapProvider_invalid_sitemapnode_returned
        {
            get { return "Provider {0} must return a valid sitemap node."; }
        }

        /// <summary>
        /// Gets the XML site map provider_missing_site map file.
        /// </summary>
        internal static string XmlSiteMapProvider_missing_siteMapFile
        {
            get { return "The {0} attribute must be specified on the XmlSiteMapProvider."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ multiple_ nodes_ with_ identical_ key.
        /// </summary>
        internal static string XmlSiteMapProvider_Multiple_Nodes_With_Identical_Key
        {
            get { return "Multiple nodes with the same key '{0}' were found. XmlSiteMapProvider requires that sitemap nodes have unique keys."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ multiple_ nodes_ with_ identical_ URL.
        /// </summary>
        internal static string XmlSiteMapProvider_Multiple_Nodes_With_Identical_Url
        {
            get { return "Multiple nodes with the same URL '{0}' were found. XmlSiteMapProvider requires that sitemap nodes have unique URLs."; }
        }

        /// <summary>
        /// Gets the XML site map provider_multiple_resource_definition.
        /// </summary>
        internal static string XmlSiteMapProvider_multiple_resource_definition
        {
            get { return "Cannot have more than one resource binding on attribute '{0}'. Ensure that this attribute is not bound through an implicit expression, for example, {0}=\"$resources:key\"."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ not_ initialized.
        /// </summary>
        internal static string XmlSiteMapProvider_Not_Initialized
        {
            get { return "XmlSiteMapProvider is not initialized. Call Initialize() method first."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ only_ one_ site map node_ required_ at_ top.
        /// </summary>
        internal static string XmlSiteMapProvider_Only_One_SiteMapNode_Required_At_Top
        {
            get { return "Exactly one <siteMapNode> element is required directly inside the <siteMap> element."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ only_ site map node_ allowed.
        /// </summary>
        internal static string XmlSiteMapProvider_Only_SiteMapNode_Allowed
        {
            get { return "Only <siteMapNode> elements are allowed at this location."; }
        }

        /// <summary>
        /// Gets the XML site map provider_resource key_cannot_be_empty.
        /// </summary>
        internal static string XmlSiteMapProvider_resourceKey_cannot_be_empty
        {
            get { return "Resource key cannot be empty."; }
        }

        /// <summary>
        /// Gets the XML site map provider_ top_ element_ must_ be_ site map.
        /// </summary>
        internal static string XmlSiteMapProvider_Top_Element_Must_Be_SiteMap
        {
            get { return "Top element must be siteMap."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ trimmed empty string.
        /// </summary>
        internal static string PersonalizationProviderHelper_TrimmedEmptyString
        {
            get { return "Input parameter '{0}' cannot be an empty string."; }
        }

        /// <summary>
        /// Gets the length of the string util_ trimmed_ string_ exceed_ maximum_.
        /// </summary>
        /// <value>
        /// The length of the string util_ trimmed_ string_ exceed_ maximum_.
        /// </value>
        internal static string StringUtil_Trimmed_String_Exceed_Maximum_Length
        {
            get { return "Trimmed string value '{0}' of input parameter '{1}' cannot exceed character length {2}."; }
        }

        /// <summary>
        /// Gets the membership SQL provider_description.
        /// </summary>
        internal static string MembershipSqlProvider_description
        {
            get { return "SQL membership provider."; }
        }

        /// <summary>
        /// Gets the length of the min required nonalphanumeric characters_can_not_be_more_than_ min required password.
        /// </summary>
        /// <value>
        /// The length of the min required nonalphanumeric characters_can_not_be_more_than_ min required password.
        /// </value>
        internal static string MinRequiredNonalphanumericCharacters_can_not_be_more_than_MinRequiredPasswordLength
        {
            get { return "The minRequiredNonalphanumericCharacters can not be greater than minRequiredPasswordLength."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ empty_ collection.
        /// </summary>
        internal static string PersonalizationProviderHelper_Empty_Collection
        {
            get { return "Input parameter '{0}' cannot be an empty collection."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ null_ or_ empty_ string_ entries.
        /// </summary>
        internal static string PersonalizationProviderHelper_Null_Or_Empty_String_Entries
        {
            get { return "Input parameter '{0}' cannot contain null or empty string entries."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ cannot have comma in string.
        /// </summary>
        internal static string PersonalizationProviderHelper_CannotHaveCommaInString
        {
            get { return "Input parameter '{0}' cannot have comma in string value '{1}'."; }
        }

        /// <summary>
        /// Gets the length of the personalization provider helper_ trimmed_ entry_ value_ exceed_ maximum_.
        /// </summary>
        /// <value>
        /// The length of the personalization provider helper_ trimmed_ entry_ value_ exceed_ maximum_.
        /// </value>
        internal static string PersonalizationProviderHelper_Trimmed_Entry_Value_Exceed_Maximum_Length
        {
            get { return "Trimmed entry value '{0}' of input parameter '{1}' cannot exceed character length {2}."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ more_ than_ one_ path.
        /// </summary>
        internal static string PersonalizationProviderHelper_More_Than_One_Path
        {
            get { return "Input parameter '{0}' cannot contain more than one entry when '{1}' contains some entries."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ negative_ integer.
        /// </summary>
        internal static string PersonalizationProviderHelper_Negative_Integer
        {
            get { return "The input parameter cannot be negative."; }
        }

        /// <summary>
        /// Gets the personalization admin_ unexpected personalization provider return value.
        /// </summary>
        internal static string PersonalizationAdmin_UnexpectedPersonalizationProviderReturnValue
        {
            get { return "The negative value '{0}' is returned when calling provider's '{1}' method.  The method should return non-negative integer."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ null_ entries.
        /// </summary>
        internal static string PersonalizationProviderHelper_Null_Entries
        {
            get { return "Input parameter '{0}' cannot contain null entries."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ invalid_ less_ than_ parameter.
        /// </summary>
        internal static string PersonalizationProviderHelper_Invalid_Less_Than_Parameter
        {
            get { return "Input parameter '{0}' must be greater than or equal to {1}."; }
        }

        /// <summary>
        /// Gets the personalization provider helper_ no_ usernames_ set_ in_ shared_ scope.
        /// </summary>
        internal static string PersonalizationProviderHelper_No_Usernames_Set_In_Shared_Scope
        {
            get { return "Input parameter '{0}' cannot be provided when '{1}' is set to '{2}'."; }
        }

        /// <summary>
        /// Gets the provider_this_user_already_not_in_role.
        /// </summary>
        internal static string Provider_this_user_already_not_in_role
        {
            get { return "The user '{0}' is already not in role '{1}'."; }
        }

        /// <summary>
        /// Gets the not_configured_to_support_password_resets.
        /// </summary>
        internal static string Not_configured_to_support_password_resets
        {
            get { return "This provider is not configured to allow password resets. To enable password reset, set enablePasswordReset to \"true\" in the configuration file."; }
        }

        /// <summary>
        /// Gets the parameter_collection_empty.
        /// </summary>
        internal static string Parameter_collection_empty
        {
            get { return "The collection parameter '{0}' should not be empty."; }
        }

        /// <summary>
        /// Gets the provider_can_not_decode_hashed_password.
        /// </summary>
        internal static string Provider_can_not_decode_hashed_password
        {
            get { return "Hashed passwords cannot be decoded."; }
        }

        /// <summary>
        /// Gets the db file name_can_not_contain_invalid_chars.
        /// </summary>
        internal static string DbFileName_can_not_contain_invalid_chars
        {
            get { return "The database filename can not contain the following 3 characters: [ (open square brace), ] (close square brace) and ' (single quote)"; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ error_ deleting_ session_ job.
        /// </summary>
        internal static string SQL_Services_Error_Deleting_Session_Job
        {
            get { return "The attempt to remove the Session State expired sessions job from msdb did not succeed.  This can occur either because the job no longer exists, or because the job was originally created with a different user account than the account that is currently performing the uninstall.  You will need to manually delete the Session State expired sessions job if it still exists."; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ error_ executing_ command.
        /// </summary>
        internal static string SQL_Services_Error_Executing_Command
        {
            get { return "An error occurred during the execution of the SQL file '{0}'. The SQL error number is {1} and the SqlException message is: {2}"; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ invalid_ feature.
        /// </summary>
        internal static string SQL_Services_Invalid_Feature
        {
            get { return "An invalid feature is requested."; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ database_ empty_ or_ space_ only_ arg.
        /// </summary>
        internal static string SQL_Services_Database_Empty_Or_Space_Only_Arg
        {
            get { return "The database name cannot be empty or contain only white space characters."; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ database_contains_invalid_chars.
        /// </summary>
        internal static string SQL_Services_Database_contains_invalid_chars
        {
            get { return "The custom database name cannot contain the following three characters: single quotation mark ('), left bracket ([) or right bracket (])."; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ error_ cant_ uninstall_ nonexisting_ database.
        /// </summary>
        internal static string SQL_Services_Error_Cant_Uninstall_Nonexisting_Database
        {
            get { return "Cannot uninstall the specified feature(s) because the SQL database '{0}' does not exist."; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ error_ cant_ uninstall_ nonempty_ table.
        /// </summary>
        internal static string SQL_Services_Error_Cant_Uninstall_Nonempty_Table
        {
            get { return "Cannot uninstall the specified feature(s) because the SQL table '{0}' in the database '{1}' is not empty. You must first remove all rows from the table."; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ error_missing_custom_database.
        /// </summary>
        internal static string SQL_Services_Error_missing_custom_database
        {
            get { return "The database name cannot be null or empty if the session state type is SessionStateType.Custom."; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ error_ cant_use_custom_database.
        /// </summary>
        internal static string SQL_Services_Error_Cant_use_custom_database
        {
            get { return "You cannot specify the database name because it is allowed only if the session state type is SessionStateType.Custom."; }
        }

        /// <summary>
        /// Gets the SQ l_ services_ cant_connect_sql_database.
        /// </summary>
        internal static string SQL_Services_Cant_connect_sql_database
        {
            get { return "Unable to connect to SQL Server database."; }
        }

        /// <summary>
        /// Gets the error_parsing_sql_partition_resolver_string.
        /// </summary>
        internal static string Error_parsing_sql_partition_resolver_string
        {
            get { return "Error parsing the SQL connection string returned by an instance of the IPartitionResolver type '{0}': {1}"; }
        }

        /// <summary>
        /// Gets the error_parsing_session_sql connection string.
        /// </summary>
        internal static string Error_parsing_session_sqlConnectionString
        {
            get { return "Error parsing <sessionState> sqlConnectionString attribute: {0}"; }
        }

        /// <summary>
        /// Gets the no_database_allowed_in_sql connection string.
        /// </summary>
        internal static string No_database_allowed_in_sqlConnectionString
        {
            get { return "The sqlConnectionString attribute or the connection string it refers to cannot contain the connection options 'Database', 'Initial Catalog' or 'AttachDbFileName'. In order to allow this, allowCustomSqlDatabase attribute must be set to true and the application needs to be granted unrestricted SqlClientPermission. Please check with your administrator if the application does not have this permission."; }
        }

        /// <summary>
        /// Gets the no_database_allowed_in_sql_partition_resolver_string.
        /// </summary>
        internal static string No_database_allowed_in_sql_partition_resolver_string
        {
            get { return "The SQL connection string (server='{1}', database='{2}') returned by an instance of the IPartitionResolver type '{0}' cannot contain the connection options 'Database', 'Initial Catalog' or 'AttachDbFileName'. In order to allow this, allowCustomSqlDatabase attribute must be set to true and the application needs to be granted unrestricted SqlClientPermission. Please check with your administrator if the application does not have this permission."; }
        }

        /// <summary>
        /// Gets the cant_connect_sql_session_database.
        /// </summary>
        internal static string Cant_connect_sql_session_database
        {
            get { return "Unable to connect to SQL Server session database."; }
        }

        /// <summary>
        /// Gets the cant_connect_sql_session_database_partition_resolver.
        /// </summary>
        internal static string Cant_connect_sql_session_database_partition_resolver
        {
            get { return "Unable to connect to SQL Server session database. The connection string (server='{1}', database='{2}') was returned by an instance of the IPartitionResolver type '{0}'."; }
        }

        /// <summary>
        /// Gets the login_failed_sql_session_database.
        /// </summary>
        internal static string Login_failed_sql_session_database
        {
            get { return "Failed to login to session state SQL server for user '{0}'."; }
        }

        /// <summary>
        /// Gets the need_v2_ SQ l_ server.
        /// </summary>
        internal static string Need_v2_SQL_Server
        {
            get { return "Unable to use SQL Server because ASP.NET version 2.0 Session State is not installed on the SQL server. Please install ASP.NET Session State SQL Server version 2.0 or above."; }
        }

        /// <summary>
        /// Gets the need_v2_ SQ l_ server_partition_resolver.
        /// </summary>
        internal static string Need_v2_SQL_Server_partition_resolver
        {
            get { return "Unable to use SQL Server because ASP.NET version 2.0 Session State is not installed on the SQL server. Please install ASP.NET Session State SQL Server version 2.0 or above. The connection string (server='{1}', database='{2}') was returned by an instance of the IPartitionResolver type '{0}'."; }
        }

        /// <summary>
        /// Gets the invalid_session_state.
        /// </summary>
        internal static string Invalid_session_state
        {
            get { return "The session state information is invalid and might be corrupted."; }
        }

        /// <summary>
        /// Gets the missing_required_attribute.
        /// </summary>
        internal static string Missing_required_attribute
        {
            get { return "The '{0}' attribute must be specified on the '{1}' tag."; }
        }

        /// <summary>
        /// Gets the invalid_boolean_attribute.
        /// </summary>
        internal static string Invalid_boolean_attribute
        {
            get { return "The '{0}' attribute must be set to 'true' or 'false'."; }
        }

        /// <summary>
        /// Gets the empty_attribute.
        /// </summary>
        internal static string Empty_attribute
        {
            get { return "The '{0}' attribute cannot be an empty string."; }
        }

        /// <summary>
        /// Gets the config_base_unrecognized_attribute.
        /// </summary>
        internal static string Config_base_unrecognized_attribute
        {
            get { return "Unrecognized attribute '{0}'. Note that attribute names are case-sensitive."; }
        }

        /// <summary>
        /// Gets the config_base_no_child_nodes.
        /// </summary>
        internal static string Config_base_no_child_nodes
        {
            get { return "Child nodes are not allowed."; }
        }

        /// <summary>
        /// Gets the unexpected_provider_attribute.
        /// </summary>
        internal static string Unexpected_provider_attribute
        {
            get { return "The attribute '{0}' is unexpected in the configuration of the '{1}' provider."; }
        }

        /// <summary>
        /// Gets the only_one_connection_string_allowed.
        /// </summary>
        internal static string Only_one_connection_string_allowed
        {
            get { return "SqlWebEventProvider: Specify either a connectionString or connectionStringName, not both."; }
        }

        /// <summary>
        /// Gets the cannot_use_integrated_security.
        /// </summary>
        internal static string Cannot_use_integrated_security
        {
            get { return "SqlWebEventProvider: connectionString can only contain connection strings that use Sql Server authentication.  Trusted Connection security is not supported."; }
        }

        /// <summary>
        /// Gets the must_specify_connection_string_or_name.
        /// </summary>
        internal static string Must_specify_connection_string_or_name
        {
            get { return "SqlWebEventProvider: Either a connectionString or connectionStringName must be specified."; }
        }

        /// <summary>
        /// Gets the invalid_max_event_details_length.
        /// </summary>
        internal static string Invalid_max_event_details_length
        {
            get { return "The value '{1}' specified for the maxEventDetailsLength attribute of the '{0}' provider is invalid. It should be between 0 and 1073741823."; }
        }

        /// <summary>
        /// Gets the sql_webevent_provider_events_dropped.
        /// </summary>
        internal static string Sql_webevent_provider_events_dropped
        {
            get { return "{0} events were discarded since last notification was made at {1} because the event buffer capacity was exceeded."; }
        }

        /// <summary>
        /// Gets the invalid_provider_positive_attributes.
        /// </summary>
        internal static string Invalid_provider_positive_attributes
        {
            get { return "The attribute '{0}' is invalid in the configuration of the '{1}' provider. The attribute must be set to a non-negative integer."; }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="strString">The STR string.</param>
        /// <returns>The String</returns>
        internal static string GetString(string strString)
        {
            return strString;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="strString">The STR string.</param>
        /// <param name="param1">The param1.</param>
        /// <returns>The String</returns>
        internal static string GetString(string strString, string param1)
        {
            return string.Format(strString, param1);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="strString">The STR string.</param>
        /// <param name="param1">The param1.</param>
        /// <param name="param2">The param2.</param>
        /// <returns>The String</returns>
        internal static string GetString(string strString, string param1, string param2)
        {
            return string.Format(strString, param1, param2);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="strString">The STR string.</param>
        /// <param name="param1">The param1.</param>
        /// <param name="param2">The param2.</param>
        /// <param name="param3">The param3.</param>
        /// <returns>The String</returns>
        internal static string GetString(string strString, string param1, string param2, string param3)
        {
            return string.Format(strString, param1, param2, param3);
        }
    }
}
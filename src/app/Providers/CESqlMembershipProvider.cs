using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using Codentia.Common.Config;
using Codentia.Common.Data;
using Codentia.Common.Helper;

namespace Codentia.Common.Membership.Providers
{
    /// <summary>
    /// CESqlMembershipProvider - override standard SqlMembershipProvider to have dynamic connection strings
    /// </summary>
    public class CESqlMembershipProvider : MembershipProvider
    {
        private const int PASSWORDSIZE = 14;
        private static string _providerDbDataSource = null;
        private bool _enablePasswordRetrieval;
        private bool _enablePasswordReset;
        private bool _requiresQuestionAndAnswer;
        private string _appName;
        private bool _requiresUniqueEmail;
        private int _maxInvalidPasswordAttempts;
        private int _commandTimeout;
        private int _passwordAttemptWindow;
        private int _minRequiredPasswordLength;
        private int _minRequiredNonalphanumericCharacters;
        private string _passwordStrengthRegularExpression;
        private int _schemaVersionCheck;
        private MembershipPasswordFormat _passwordFormat;

        /// <summary>
        /// Gets or sets the provider data source.
        /// </summary>
        /// <value>The provider data source.</value>
        public static string ProviderDbDataSource
        {
            get
            {
                return string.IsNullOrEmpty(_providerDbDataSource) ? CESqlMembershipProvider.ConnectionStringName : _providerDbDataSource;
            }

            set
            {
                _providerDbDataSource = value;
            }
        }

        /// <summary>
        /// Gets ConnectionStringName
        /// </summary>
        public static string ConnectionStringName
        {
            get
            {
                // load the config from system.web and check the connectionstring name given
                Configuration appConfig = ConfigManager.GetAppConfig();
                ConfigurationSectionGroup systemWeb = appConfig.GetSectionGroup("system.web");
                MembershipSection membership = (MembershipSection)systemWeb.Sections.Get("membership");

                ProviderSettings providerSettings = null;

                for (int i = 0; i < membership.Providers.Count; i++)
                {
                    if (membership.Providers[i].Type.ToLower().StartsWith("mattchedit"))
                    {
                        providerSettings = membership.Providers[i];
                    }
                }

                return providerSettings.Parameters["connectionStringName"];
            }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.
        /// </returns>
        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider supports password reset; otherwise, false. The default is true.
        /// </returns>
        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <value></value>
        /// <returns>true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.
        /// </returns>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </returns>
        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </returns>
        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The minimum length required for a password.
        /// </returns>
        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonalphanumericCharacters; }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        /// <returns>string</returns>
        public override string ApplicationName
        {
            get
            {
                return _appName;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Length > 256)
                {
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_application_name_too_long));
                }

                _appName = value;
            }
        }

        private int CommandTimeout
        {
            get { return _commandTimeout; }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "CESqlMembershipProvider";
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", ProviderStrings.GetString(ProviderStrings.MembershipSqlProvider_description));
            }

            base.Initialize(name, config);
            _providerDbDataSource = config["connectionStringName"];

            _schemaVersionCheck = 0;

            _enablePasswordRetrieval = SecUtility.GetBooleanValue(config, "enablePasswordRetrieval", false);
            _enablePasswordReset = SecUtility.GetBooleanValue(config, "enablePasswordReset", true);
            _requiresQuestionAndAnswer = SecUtility.GetBooleanValue(config, "requiresQuestionAndAnswer", true);
            _requiresUniqueEmail = SecUtility.GetBooleanValue(config, "requiresUniqueEmail", true);
            _maxInvalidPasswordAttempts = SecUtility.GetIntValue(config, "maxInvalidPasswordAttempts", 5, false, 0);
            _passwordAttemptWindow = SecUtility.GetIntValue(config, "passwordAttemptWindow", 10, false, 0);
            _minRequiredPasswordLength = SecUtility.GetIntValue(config, "minRequiredPasswordLength", 7, false, 128);
            _minRequiredNonalphanumericCharacters = SecUtility.GetIntValue(config, "minRequiredNonalphanumericCharacters", 1, true, 128);

            _passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
            
            if (_passwordStrengthRegularExpression != null)
            {
                _passwordStrengthRegularExpression = _passwordStrengthRegularExpression.Trim();
                if (_passwordStrengthRegularExpression.Length != 0)
                {
                    try
                    {
                        Regex regex = new Regex(_passwordStrengthRegularExpression);
                    }
                    catch (ArgumentException e)
                    {
                        throw new ProviderException(e.Message, e);
                    }
                }
            }
            else
            {
                _passwordStrengthRegularExpression = string.Empty;
            }

            if (_minRequiredNonalphanumericCharacters > _minRequiredPasswordLength)
            {
                throw new HttpException(ProviderStrings.GetString(ProviderStrings.MinRequiredNonalphanumericCharacters_can_not_be_more_than_MinRequiredPasswordLength));
            }

            _commandTimeout = SecUtility.GetIntValue(config, "commandTimeout", 30, true, 0);
            _appName = config["applicationName"];

            if (string.IsNullOrEmpty(_appName))
            {
                _appName = SecUtility.GetDefaultAppName();
            }

            if (_appName.Length > 256)
            {
                throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_application_name_too_long));
            }

            string strTemp = config["passwordFormat"];
            if (strTemp == null)
            {
                strTemp = "Hashed";
            }

            switch (strTemp)
            {
                case "Clear":
                    _passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                case "Encrypted":
                    _passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Hashed":
                    _passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                default:
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_bad_password_format));
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed && EnablePasswordRetrieval)
            {
                throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_can_not_retrieve_hashed_password));
            }

            string temp = config["connectionStringName"];
            if (temp == null || temp.Length < 1)
            {
                throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Connection_name_not_specified));
            }

            config.Remove("connectionStringName");
            config.Remove("enablePasswordRetrieval");
            config.Remove("enablePasswordReset");
            config.Remove("requiresQuestionAndAnswer");
            config.Remove("applicationName");
            config.Remove("requiresUniqueEmail");
            config.Remove("maxInvalidPasswordAttempts");
            config.Remove("passwordAttemptWindow");
            config.Remove("commandTimeout");
            config.Remove("passwordFormat");
            config.Remove("name");
            config.Remove("minRequiredPasswordLength");
            config.Remove("minRequiredNonalphanumericCharacters");
            config.Remove("passwordStrengthRegularExpression");
            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!string.IsNullOrEmpty(attribUnrecognized))
                {
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unrecognized_attribute, attribUnrecognized));
                }
            }
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        public override MembershipUser CreateUser(
                                                    string username,
                                                   string password,
                                                   string email,
                                                   string passwordQuestion,
                                                   string passwordAnswer,
                                                   bool isApproved,
                                                   object providerUserKey,
                                                   out MembershipCreateStatus status)
        {
            if (!SecUtility.ValidateParameter(ref password, true, true, false, 128))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            string salt = GenerateSalt();
            string pass = EncodePassword(password, (int)_passwordFormat, salt);
            if (pass.Length > 128)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            string encodedPasswordAnswer;
            if (passwordAnswer != null)
            {
                passwordAnswer = passwordAnswer.Trim();
            }

            if (!string.IsNullOrEmpty(passwordAnswer))
            {
                if (passwordAnswer.Length > 128)
                {
                    status = MembershipCreateStatus.InvalidAnswer;
                    return null;
                }

                encodedPasswordAnswer = EncodePassword(passwordAnswer.ToLower(CultureInfo.InvariantCulture), (int)_passwordFormat, salt);
            }
            else
            {
                encodedPasswordAnswer = passwordAnswer;
            }

            if (!SecUtility.ValidateParameter(ref encodedPasswordAnswer, RequiresQuestionAndAnswer, true, false, 128))
            {
                status = MembershipCreateStatus.InvalidAnswer;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref username, true, true, true, 256))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }

            if (!SecUtility.ValidateParameter(
                                                ref email,
                                               RequiresUniqueEmail,
                                               RequiresUniqueEmail,
                                               false,
                                               256))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref passwordQuestion, RequiresQuestionAndAnswer, true, false, 256))
            {
                status = MembershipCreateStatus.InvalidQuestion;
                return null;
            }

            if (providerUserKey != null)
            {
                if (!(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }
            }

            if (password.Length < MinRequiredPasswordLength)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            int count = 0;

            for (int i = 0; i < password.Length; i++)
            {
                if (!char.IsLetterOrDigit(password, i))
                {
                    count++;
                }
            }

            if (count < MinRequiredNonAlphanumericCharacters)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(password, PasswordStrengthRegularExpression))
                {
                    status = MembershipCreateStatus.InvalidPassword;
                    return null;
                }
            }

            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(e);

            if (e.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DateTime dt = RoundToSeconds(DateTime.UtcNow);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username),
                        new DbParameter("@Password", DbType.String, pass),
                        new DbParameter("@PasswordSalt", DbType.String, salt),
                        new DbParameter("@Email", DbType.String, email),
                        new DbParameter("@PasswordQuestion", DbType.String, passwordQuestion),
                        new DbParameter("@PasswordAnswer", DbType.String, encodedPasswordAnswer),
                        new DbParameter("@IsApproved", DbType.Boolean, isApproved),
                        new DbParameter("@UniqueEmail", DbType.Int32, RequiresUniqueEmail ? 1 : 0),
                        new DbParameter("@PasswordFormat", DbType.Int32, (int)PasswordFormat),
                        new DbParameter("@CurrentTimeUtc", DbType.DateTime, dt),
                        new DbParameter("@UserId", DbType.Guid, ParameterDirection.InputOutput, providerUserKey)
                    };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Membership_CreateUser", spParams);

                int returnValue = (p.Value != null) ? ((int)p.Value) : -1;
                if (returnValue < 0 || returnValue > (int)MembershipCreateStatus.ProviderError)
                {
                    returnValue = (int)MembershipCreateStatus.ProviderError;
                }

                status = (MembershipCreateStatus)returnValue;
                if (status != 0) 
                {
                    // !success
                    return null;
                }

                providerUserKey = new Guid(spParams[11].Value.ToString());
                dt = dt.ToLocalTime();
                return new MembershipUser(
                                            this.Name,
                                            username,
                                            providerUserKey,
                                            email,
                                            passwordQuestion,
                                            null,
                                            isApproved,
                                            false,
                                            dt,
                                            dt,
                                            dt,
                                            dt,
                                            new DateTime(1754, 1, 1));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            SecUtility.CheckParameter(ref username, true, true, true, 256, "username");
            SecUtility.CheckParameter(ref password, true, true, false, 128, "password");

            string salt;
            int passwordFormat;
            if (!CheckPassword(username, password, false, false, out salt, out passwordFormat))
            {
                return false;
            }

            SecUtility.CheckParameter(ref newPasswordQuestion, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, false, 256, "newPasswordQuestion");
            string encodedPasswordAnswer;
            if (newPasswordAnswer != null)
            {
                newPasswordAnswer = newPasswordAnswer.Trim();
            }

            SecUtility.CheckParameter(ref newPasswordAnswer, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, false, 128, "newPasswordAnswer");
            if (!string.IsNullOrEmpty(newPasswordAnswer))
            {
                encodedPasswordAnswer = EncodePassword(newPasswordAnswer.ToLower(CultureInfo.InvariantCulture), (int)passwordFormat, salt);
            }
            else
            {
                encodedPasswordAnswer = newPasswordAnswer;
            }

            SecUtility.CheckParameter(ref encodedPasswordAnswer, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, false, 128, "newPasswordAnswer");

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                        {
                            new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                            new DbParameter("@UserName", DbType.String, username),
                            new DbParameter("@NewPasswordQuestion", DbType.String, newPasswordQuestion),
                            new DbParameter("@NewPasswordAnswer", DbType.String, encodedPasswordAnswer)                            
                        };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Membership_ChangePasswordQuestionAndAnswer", spParams);

                int status = (p.Value != null) ? ((int)p.Value) : -1;
                if (status != 0)
                {
                    throw new ProviderException(GetExceptionText(status));
                }

                return status == 0;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="passwordAnswer">The password answer.</param>
        /// <returns>The Password</returns>
        public override string GetPassword(string username, string passwordAnswer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new NotSupportedException(ProviderStrings.GetString(ProviderStrings.Membership_PasswordRetrieval_not_supported));
            }

            SecUtility.CheckParameter(ref username, true, true, true, 256, "username");

            string encodedPasswordAnswer = GetEncodedPasswordAnswer(username, passwordAnswer);
            SecUtility.CheckParameter(ref encodedPasswordAnswer, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, false, 128, "passwordAnswer");

            string errText;
            int passwordFormat = 0;
            int status = 0;

            string pass = GetPasswordFromDB(username, encodedPasswordAnswer, RequiresQuestionAndAnswer, out passwordFormat, out status);

            if (pass == null)
            {
                errText = GetExceptionText(status);
                if (IsStatusDueToBadPassword(status))
                {
                    throw new MembershipPasswordException(errText);
                }
                else
                {
                    throw new ProviderException(errText);
                }
            }

            return UnEncodePassword(pass, passwordFormat);
        }

        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            SecUtility.CheckParameter(ref username, true, true, true, 256, "username");
            SecUtility.CheckParameter(ref oldPassword, true, true, false, 128, "oldPassword");
            SecUtility.CheckParameter(ref newPassword, true, true, false, 128, "newPassword");

            string salt = null;
            int passwordFormat;
            int status;

            if (!CheckPassword(username, oldPassword, false, false, out salt, out passwordFormat))
            {
                return false;
            }

            if (newPassword.Length < MinRequiredPasswordLength)
            {
                throw new ArgumentException(ProviderStrings.GetString(
                              ProviderStrings.Password_too_short,
                              "newPassword",
                              MinRequiredPasswordLength.ToString(CultureInfo.InvariantCulture)));
            }

            int count = 0;

            for (int i = 0; i < newPassword.Length; i++)
            {
                if (!char.IsLetterOrDigit(newPassword, i))
                {
                    count++;
                }
            }

            if (count < MinRequiredNonAlphanumericCharacters)
            {
                throw new ArgumentException(ProviderStrings.GetString(
                              ProviderStrings.Password_need_more_non_alpha_numeric_chars,
                              "newPassword",
                              MinRequiredNonAlphanumericCharacters.ToString(CultureInfo.InvariantCulture)));
            }

            if (PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(newPassword, PasswordStrengthRegularExpression))
                {
                    throw new ArgumentException(ProviderStrings.GetString(ProviderStrings.Password_does_not_match_regular_expression, "newPassword"));
                }
            }

            string pass = EncodePassword(newPassword, (int)passwordFormat, salt);
            if (pass.Length > 128)
            {
                throw new ArgumentException(ProviderStrings.GetString(ProviderStrings.Membership_password_too_long), "newPassword");
            }

            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, newPassword, false);
            OnValidatingPassword(e);

            if (e.Cancel)
            {
                if (e.FailureInformation != null)
                {
                    throw e.FailureInformation;
                }
                else
                {
                    throw new ArgumentException(ProviderStrings.GetString(ProviderStrings.Membership_Custom_Password_Validation_Failure), "newPassword");
                }
            }

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                {
                    new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                    new DbParameter("@UserName", DbType.String, username),
                    new DbParameter("@NewPassword", DbType.String, pass),
                    new DbParameter("@PasswordSalt", DbType.String, salt),
                    new DbParameter("@PasswordFormat", DbType.Int32, passwordFormat),
                    new DbParameter("@CurrentTimeUtc", DbType.DateTime,  DateTime.UtcNow)                    
                };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Membership_SetPassword", spParams);

                status = (p.Value != null) ? ((int)p.Value) : -1;

                if (status != 0)
                {
                    string errText = GetExceptionText(status);

                    if (IsStatusDueToBadPassword(status))
                    {
                        throw new MembershipPasswordException(errText);
                    }
                    else
                    {
                        throw new ProviderException(errText);
                    }
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="passwordAnswer">The password answer.</param>
        /// <returns>The Password</returns>
        public override string ResetPassword(string username, string passwordAnswer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException(ProviderStrings.GetString(ProviderStrings.Not_configured_to_support_password_resets));
            }

            SecUtility.CheckParameter(ref username, true, true, true, 256, "username");

            string salt;
            int passwordFormat;
            string passwdFromDB;
            int status;
            int failedPasswordAttemptCount;
            int failedPasswordAnswerAttemptCount;
            bool isApproved;
            DateTime lastLoginDate, lastActivityDate;

            GetPasswordWithFormat(username, false, out status, out passwdFromDB, out passwordFormat, out salt, out failedPasswordAttemptCount, out failedPasswordAnswerAttemptCount, out isApproved, out lastLoginDate, out lastActivityDate);
            if (status != 0)
            {
                if (IsStatusDueToBadPassword(status))
                {
                    throw new MembershipPasswordException(GetExceptionText(status));
                }
                else
                {
                    throw new ProviderException(GetExceptionText(status));
                }
            }

            string encodedPasswordAnswer;
            if (passwordAnswer != null)
            {
                passwordAnswer = passwordAnswer.Trim();
            }
            
            if (!string.IsNullOrEmpty(passwordAnswer))
            {
                encodedPasswordAnswer = EncodePassword(passwordAnswer.ToLower(CultureInfo.InvariantCulture), passwordFormat, salt);
            }
            else
            {
                encodedPasswordAnswer = passwordAnswer;
            }

            SecUtility.CheckParameter(ref encodedPasswordAnswer, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, false, 128, "passwordAnswer");
            string newPassword = GeneratePassword();

            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, newPassword, false);
            OnValidatingPassword(e);

            if (e.Cancel)
            {
                if (e.FailureInformation != null)
                {
                    throw e.FailureInformation;
                }
                else
                {
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Membership_Custom_Password_Validation_Failure));
                }
            }

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                string useEncodedPasswordAnswer = string.Empty;

                if (RequiresQuestionAndAnswer)
                {
                    useEncodedPasswordAnswer = encodedPasswordAnswer;
                }

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username),
                        new DbParameter("@NewPassword", DbType.String, EncodePassword(newPassword, (int)passwordFormat, salt)),
                        new DbParameter("@MaxInvalidPasswordAttempts", DbType.Int32, MaxInvalidPasswordAttempts),
                        new DbParameter("@PasswordAttemptWindow", DbType.Int32, PasswordAttemptWindow),
                        new DbParameter("@PasswordSalt", DbType.String, salt),
                        new DbParameter("@PasswordFormat", DbType.Int32, (int)passwordFormat),
                        new DbParameter("@CurrentTimeUtc", DbType.DateTime, DateTime.UtcNow), 
                        new DbParameter("@PasswordAnswer", DbType.String, encodedPasswordAnswer)                        
                    };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Membership_ResetPassword", spParams);

                string errText;

                status = (p.Value != null) ? ((int)p.Value) : -1;

                if (status != 0)
                {
                    errText = GetExceptionText(status);

                    if (IsStatusDueToBadPassword(status))
                    {
                        throw new MembershipPasswordException(errText);
                    }
                    else
                    {
                        throw new ProviderException(errText);
                    }
                }

                return newPassword;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            string temp = user.UserName;
            SecUtility.CheckParameter(ref temp, true, true, true, 256, "UserName");
            temp = user.Email;
            SecUtility.CheckParameter(
                                        ref temp,
                                       RequiresUniqueEmail,
                                       RequiresUniqueEmail,
                                       false,
                                       256,
                                       "Email");
            user.Email = temp;
            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, user.UserName),
                        new DbParameter("@Email", DbType.String, user.Email),
                        new DbParameter("@Comment", DbType.String, user.Comment),                        
                        new DbParameter("@IsApproved", DbType.Boolean, user.IsApproved ? 1 : 0),
                        new DbParameter("@LastLoginDate", DbType.DateTime, user.LastLoginDate.ToUniversalTime()),
                        new DbParameter("@LastActivityDate", DbType.DateTime, user.LastActivityDate.ToUniversalTime()),
                        new DbParameter("@UniqueEmail", DbType.Int32, RequiresUniqueEmail ? 1 : 0),                        
                        new DbParameter("@CurrentTimeUtc", DbType.DateTime, DateTime.UtcNow)
                    };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Membership_UpdateUser", spParams);

                int status = (p.Value != null) ? ((int)p.Value) : -1;
                if (status != 0)
                {
                    throw new ProviderException(GetExceptionText(status));
                }

                return;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        public override bool ValidateUser(string username, string password)
        {
            if (SecUtility.ValidateParameter(ref username, true, true, true, 256) &&
                    SecUtility.ValidateParameter(ref password, true, true, false, 128) &&
                    CheckPassword(username, password, true, true))
            {
                // Comment out perf counters in sample: PerfCounters.IncrementCounter(AppPerfCounter.MEMBER_SUCCESS);
                // Comment out events in sample: WebBaseEvent.RaiseSystemEvent(null, WebEventCodes.AuditMembershipAuthenticationSuccess, username);
                return true;
            }
            else
            {
                // Comment out perf counters in sample: PerfCounters.IncrementCounter(AppPerfCounter.MEMBER_FAIL);
                // Comment out events in sample: WebBaseEvent.RaiseSystemEvent(null, WebEventCodes.AuditMembershipAuthenticationFailure, username);
                return false;
            }
        }

        /// <summary>
        /// Unlocks the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>true if user was unlocked</returns>
        public override bool UnlockUser(string username)
        {
            SecUtility.CheckParameter(ref username, true, true, true, 256, "username");
            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username)
                    };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Membership_UnlockUser", spParams);

                int status = (p.Value != null) ? (int)p.Value : -1;
                if (status == 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
            {
                throw new ArgumentNullException("providerUserKey");
            }

            if (!(providerUserKey is Guid))
            {
                throw new ArgumentException(ProviderStrings.GetString(ProviderStrings.Membership_InvalidProviderUserKey), "providerUserKey");
            }

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@UserId", DbType.Guid, providerUserKey),
                        new DbParameter("@UpdateLastActivity", DbType.Boolean, userIsOnline),                                           
                        new DbParameter("@CurrentTimeUtc", DbType.DateTime, DateTime.UtcNow)
                    };

                DataTable dt = DbInterface.ExecuteProcedureDataTable(_providerDbDataSource, "dbo.aspnet_Membership_GetUserByUserId", spParams);

                if (dt != null)
                {
                    if (dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];

                        string email = GetNullableString(dr, 0);
                        string passwordQuestion = GetNullableString(dr, 1);
                        string comment = GetNullableString(dr, 2);
                        bool isApproved = Convert.ToBoolean(dr[3]);
                        DateTime dtCreate = Convert.ToDateTime(dr[4]).ToLocalTime();
                        DateTime dtLastLogin = Convert.ToDateTime(dr[5]).ToLocalTime();
                        DateTime dtLastActivity = Convert.ToDateTime(dr[6]).ToLocalTime();
                        DateTime dtLastPassChange = Convert.ToDateTime(dr[7]).ToLocalTime();
                        string userName = GetNullableString(dr, 8);
                        bool isLockedOut = Convert.ToBoolean(dr[9]);
                        DateTime dtLastLockoutDate = Convert.ToDateTime(dr[10]).ToLocalTime();

                        //// //// //// //// //// //// //// //// //// //// //// //// //// //// //// 
                        // Step 4 : Return the result
                        return new MembershipUser(
                                                    this.Name,
                                                    userName,
                                                    providerUserKey,
                                                    email,
                                                    passwordQuestion,
                                                    comment,
                                                    isApproved,
                                                    isLockedOut,
                                                    dtCreate,
                                                    dtLastLogin,
                                                    dtLastActivity,
                                                    dtLastPassChange,
                                                    dtLastLockoutDate);
                    }
                }

                return null;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            SecUtility.CheckParameter(
                            ref username,
                            true,
                            false,
                            true,
                            256,
                            "username");

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username),                           
                        new DbParameter("@UpdateLastActivity", DbType.Boolean, userIsOnline),                                           
                        new DbParameter("@CurrentTimeUtc", DbType.DateTime, DateTime.UtcNow)
                    };

                DataTable dt = DbInterface.ExecuteProcedureDataTable(_providerDbDataSource, "dbo.aspnet_Membership_GetUserByName", spParams);

                if (dt != null)
                {
                    if (dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];

                        string email = GetNullableString(dr, 0);
                        string passwordQuestion = GetNullableString(dr, 1);
                        string comment = GetNullableString(dr, 2);
                        bool isApproved = Convert.ToBoolean(dr[3]);
                        DateTime dtCreate = Convert.ToDateTime(dr[4]).ToLocalTime();
                        DateTime dtLastLogin = Convert.ToDateTime(dr[5]).ToLocalTime();
                        DateTime dtLastActivity = Convert.ToDateTime(dr[6]).ToLocalTime();
                        DateTime dtLastPassChange = Convert.ToDateTime(dr[7]).ToLocalTime();
                        Guid userId = new Guid(Convert.ToString(dr[8]));
                        bool isLockedOut = Convert.ToBoolean(dr[9]);
                        DateTime dtLastLockoutDate = Convert.ToDateTime(dr[10]).ToLocalTime();

                        //// //// //// //// //// //// //// //// //// //// //// //// //// //// //// 
                        // Step 4 : Return the result
                        return new MembershipUser(
                                                    this.Name,
                                                   username,
                                                   userId,
                                                   email,
                                                   passwordQuestion,
                                                   comment,
                                                   isApproved,
                                                   isLockedOut,
                                                   dtCreate,
                                                   dtLastLogin,
                                                   dtLastActivity,
                                                   dtLastPassChange,
                                                   dtLastLockoutDate);
                    }
                }

                return null;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        public override string GetUserNameByEmail(string email)
        {
            SecUtility.CheckParameter(
                            ref email,
                            false,
                            false,
                            false,
                            256,
                            "email");

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                string username = null;
                StringBuilder users = new StringBuilder();

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@Email", DbType.String, email)
                    };

                DataTable dt = DbInterface.ExecuteQueryDataTable(_providerDbDataSource, "dbo.aspnet_Membership_GetUserByEmail", spParams);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    string currentUsername = GetNullableString(dr, 0);

                    if (users.Length > 0 && RequiresUniqueEmail)
                    {
                        throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Membership_more_than_one_user_with_email));
                    }

                    users.Append(currentUsername);
                    username = currentUsername;
                }

                return username;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            SecUtility.CheckParameter(ref username, true, true, true, 256, "username");

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                object tabsToDeleteFrom = 1;

                if (deleteAllRelatedData)
                {
                    tabsToDeleteFrom = 0xF;
                }

                DbParameter p = new DbParameter("@NumTablesDeletedFrom", DbType.Int32, ParameterDirection.Output);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username),    
                        new DbParameter("@TablesToDeleteFrom", DbType.Int32, tabsToDeleteFrom),
                        p                                              
                    };

                DbInterface.ExecuteProcedureNoReturn(_providerDbDataSource, "dbo.aspnet_Users_DeleteUser", spParams);

                int status = (p.Value != null) ? (int)p.Value : -1;

                return status > 0;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException(ProviderStrings.GetString(ProviderStrings.PageIndex_bad), "pageIndex");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException(ProviderStrings.GetString(ProviderStrings.PageSize_bad), "pageSize");
            }

            long upperBound = (((long)pageIndex * pageSize) + pageSize) - 1;
            if (upperBound > int.MaxValue)
            {
                throw new ArgumentException(ProviderStrings.GetString(ProviderStrings.PageIndex_PageSize_bad), "pageIndex and pageSize");
            }

            MembershipUserCollection users = new MembershipUserCollection();
            totalRecords = 0;
            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@PageIndex", DbType.Int32, pageIndex),    
                        new DbParameter("@PageSize", DbType.Int32, pageSize)
                    };

                DataTable dt = DbInterface.ExecuteProcedureDataTable(_providerDbDataSource, "dbo.aspnet_Membership_GetAllUsers", spParams);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];

                        string username, email, passwordQuestion, comment;
                        bool isApproved;
                        DateTime dtCreate, dtLastLogin, dtLastActivity, dtLastPassChange;
                        Guid userId;
                        bool isLockedOut;
                        DateTime dtLastLockoutDate;

                        username = GetNullableString(dr, 0);
                        email = GetNullableString(dr, 1);
                        passwordQuestion = GetNullableString(dr, 2);
                        comment = GetNullableString(dr, 3);
                        isApproved = Convert.ToBoolean(dr[4]);
                        dtCreate = Convert.ToDateTime(dr[5]).ToLocalTime();
                        dtLastLogin = Convert.ToDateTime(dr[6]).ToLocalTime();
                        dtLastActivity = Convert.ToDateTime(dr[7]).ToLocalTime();
                        dtLastPassChange = Convert.ToDateTime(dr[8]).ToLocalTime();
                        userId = new Guid(Convert.ToString(dr[9]));
                        isLockedOut = Convert.ToBoolean(dr[10]);
                        dtLastLockoutDate = Convert.ToDateTime(dr[11]).ToLocalTime();

                        users.Add(new MembershipUser(
                                                        this.Name,
                                                       username,
                                                       userId,
                                                       email,
                                                       passwordQuestion,
                                                       comment,
                                                       isApproved,
                                                       isLockedOut,
                                                       dtCreate,
                                                       dtLastLogin,
                                                       dtLastActivity,
                                                       dtLastPassChange,
                                                       dtLastLockoutDate));
                    }
                }
            }
            catch
            {
                throw;
            }

            return users;
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>
        /// The number of users currently accessing the application.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Generates the password.
        /// </summary>
        /// <returns>The password</returns>
        public virtual string GeneratePassword()
        {
            return StringHelper.GenerateFriendlyPassword(MinRequiredPasswordLength < PASSWORDSIZE ? PASSWORDSIZE : MinRequiredPasswordLength);
        }

        /// <summary>
        /// Generates the salt.
        /// </summary>
        /// <returns>The salt</returns>
        internal string GenerateSalt()
        {
            byte[] buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        /// Encodes the password.
        /// </summary>
        /// <param name="pass">The pass.</param>
        /// <param name="passwordFormat">The password format.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>The encoded password</returns>
        internal string EncodePassword(string pass, int passwordFormat, string salt)
        {
            if (passwordFormat == 0) 
            {
                // MembershipPasswordFormat.Clear
                return pass;
            }

            byte[] bytesIn = Encoding.Unicode.GetBytes(pass);
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] allBytes = new byte[saltBytes.Length + bytesIn.Length];
            byte[] returnBytes = null;

            Buffer.BlockCopy(saltBytes, 0, allBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(bytesIn, 0, allBytes, saltBytes.Length, bytesIn.Length);
            if (passwordFormat == 1)
            { // MembershipPasswordFormat.Hashed
                HashAlgorithm s = HashAlgorithm.Create(System.Web.Security.Membership.HashAlgorithmType);
                returnBytes = s.ComputeHash(allBytes);
                bool a = s.CanReuseTransform;
            }
            else
            {
                returnBytes = EncryptPassword(allBytes);
            }

            return Convert.ToBase64String(returnBytes);
        }

        /// <summary>
        /// Uns the encode password.
        /// </summary>
        /// <param name="pass">The pass.</param>
        /// <param name="passwordFormat">The password format.</param>
        /// <returns>The unencoded password</returns>
        internal string UnEncodePassword(string pass, int passwordFormat)
        {
            switch (passwordFormat)
            {
                case 0: // MembershipPasswordFormat.Clear:
                    return pass;
                case 1: // MembershipPasswordFormat.Hashed:
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_can_not_decode_hashed_password));
                default:
                    byte[] bytesIn = Convert.FromBase64String(pass);
                    byte[] returnBytes = DecryptPassword(bytesIn);
                    if (returnBytes == null)
                    {
                        return null;
                    }

                    return Encoding.Unicode.GetString(returnBytes, 16, returnBytes.Length - 16);
            }
        }

        private string GetNullableString(DataRow dr, int col)
        {
            if (dr[col] != DBNull.Value)
            {
                return Convert.ToString(dr[col]);
            }

            return null;
        }

        private string GetExceptionText(int status)
        {
            string key;
            switch (status)
            {
                case 0:
                    return string.Empty;
                case 1:
                    key = ProviderStrings.Membership_UserNotFound;
                    break;
                case 2:
                    key = ProviderStrings.Membership_WrongPassword;
                    break;
                case 3:
                    key = ProviderStrings.Membership_WrongAnswer;
                    break;
                case 4:
                    key = ProviderStrings.Membership_InvalidPassword;
                    break;
                case 5:
                    key = ProviderStrings.Membership_InvalidQuestion;
                    break;
                case 6:
                    key = ProviderStrings.Membership_InvalidAnswer;
                    break;
                case 7:
                    key = ProviderStrings.Membership_InvalidEmail;
                    break;
                case 99:
                    key = ProviderStrings.Membership_AccountLockOut;
                    break;
                default:
                    key = ProviderStrings.Provider_Error;
                    break;
            }

            return ProviderStrings.GetString(key);
        }

        private bool IsStatusDueToBadPassword(int status)
        {
            return (status >= 2 && status <= 6) || status == 99;
        }

        private DateTime RoundToSeconds(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
        
        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption
        /// </summary>
        /// <param name="hexString">The hex string.</param>
        /// <returns>byte array</returns>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }

        private bool CheckPassword(string username, string password, bool updateLastLoginActivityDate, bool failIfNotApproved)
        {
            string salt;
            int passwordFormat;
            return CheckPassword(username, password, updateLastLoginActivityDate, failIfNotApproved, out salt, out passwordFormat);
        }

        private bool CheckPassword(string username, string password, bool updateLastLoginActivityDate, bool failIfNotApproved, out string salt, out int passwordFormat)
        {
            string passwdFromDB;
            int status;
            int failedPasswordAttemptCount;
            int failedPasswordAnswerAttemptCount;
            bool isPasswordCorrect;
            bool isApproved;
            DateTime lastLoginDate, lastActivityDate;

            GetPasswordWithFormat(username, updateLastLoginActivityDate, out status, out passwdFromDB, out passwordFormat, out salt, out failedPasswordAttemptCount, out failedPasswordAnswerAttemptCount, out isApproved, out lastLoginDate, out lastActivityDate);
            if (status != 0)
            {
                return false;
            }

            if (!isApproved && failIfNotApproved)
            {
                return false;
            }

            string encodedPasswd = EncodePassword(password, passwordFormat, salt);

            isPasswordCorrect = passwdFromDB.Equals(encodedPasswd);

            if (isPasswordCorrect && failedPasswordAttemptCount == 0 && failedPasswordAnswerAttemptCount == 0)
            {
                return true;
            }

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DateTime dtNow = DateTime.UtcNow;

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username),    
                        new DbParameter("@IsPasswordCorrect", DbType.Boolean, isPasswordCorrect),    
                        new DbParameter("@UpdateLastLoginActivityDate", DbType.Boolean, updateLastLoginActivityDate),
                        new DbParameter("@MaxInvalidPasswordAttempts", DbType.Int32, MaxInvalidPasswordAttempts),    
                        new DbParameter("@PasswordAttemptWindow", DbType.Int32, PasswordAttemptWindow),    
                        new DbParameter("@CurrentTimeUtc", DbType.DateTime, dtNow),
                        new DbParameter("@LastLoginDate", DbType.DateTime,  isPasswordCorrect ? dtNow : lastLoginDate),
                        new DbParameter("@LastActivityDate", DbType.DateTime, isPasswordCorrect ? dtNow : lastActivityDate)
                    };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Membership_UpdateUserInfo", spParams);

                status = (p.Value != null) ? (int)p.Value : -1;
            }
            catch
            {
                throw;
            }

            return isPasswordCorrect;
        }

        private void GetPasswordWithFormat(
                                            string username,
                                            bool updateLastLoginActivityDate,
                                            out int status,
                                            out string password,
                                            out int passwordFormat,
                                            out string passwordSalt,
                                            out int failedPasswordAttemptCount,
                                            out int failedPasswordAnswerAttemptCount,
                                            out bool isApproved,
                                            out DateTime lastLoginDate,
                                            out DateTime lastActivityDate)
        {
            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DateTime dtNow = DateTime.UtcNow;

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username),                            
                        new DbParameter("@UpdateLastLoginActivityDate", DbType.Boolean, updateLastLoginActivityDate), 
                        new DbParameter("@CurrentTimeUtc", DbType.DateTime, DateTime.UtcNow)
                    };

                Dictionary<DataTable, SqlParameter> retVars = DbInterface.ExecuteProcedureDataTableWithReturn(_providerDbDataSource, "dbo.aspnet_Membership_GetPasswordWithFormat", spParams);

                IEnumerator<DataTable> ie = retVars.Keys.GetEnumerator();
                ie.MoveNext();
                DataTable dt = ie.Current;
                SqlParameter p = retVars[ie.Current];

                status = -1;

                password = null;
                passwordFormat = 0;
                passwordSalt = null;
                failedPasswordAttemptCount = 0;
                failedPasswordAnswerAttemptCount = 0;
                isApproved = false;
                lastLoginDate = DateTime.UtcNow;
                lastActivityDate = DateTime.UtcNow;

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    password = Convert.ToString(dr[0]);
                    passwordFormat = Convert.ToInt32(dr[1]);
                    passwordSalt = Convert.ToString(dr[2]);
                    failedPasswordAttemptCount = Convert.ToInt32(dr[3]);
                    failedPasswordAnswerAttemptCount = Convert.ToInt32(dr[4]);
                    isApproved = Convert.ToBoolean(dr[5]);
                    lastLoginDate = Convert.ToDateTime(dr[6]);
                    lastActivityDate = Convert.ToDateTime(dr[7]);
                }

                status = (p.Value != null) ? (int)p.Value : -1;
            }
            catch
            {
                throw;
            }
        }

        private string GetPasswordFromDB(
                                          string username,
                                          string passwordAnswer,
                                          bool requiresQuestionAndAnswer,
                                          out int passwordFormat,
                                          out int status)
        {
            throw new NotSupportedException("GetPasswordFromDB is not currently supported");
        }

        private string GetEncodedPasswordAnswer(string username, string passwordAnswer)
        {
            if (passwordAnswer != null)
            {
                passwordAnswer = passwordAnswer.Trim();
            }

            if (string.IsNullOrEmpty(passwordAnswer))
            {
                return passwordAnswer;
            }

            int status, passwordFormat, failedPasswordAttemptCount, failedPasswordAnswerAttemptCount;
            string password, passwordSalt;
            bool isApproved;
            DateTime lastLoginDate, lastActivityDate;
            
            GetPasswordWithFormat(username, false, out status, out password, out passwordFormat, out passwordSalt, out failedPasswordAttemptCount, out failedPasswordAnswerAttemptCount, out isApproved, out lastLoginDate, out lastActivityDate);
            
            if (status == 0)
            {
                return EncodePassword(passwordAnswer.ToLower(CultureInfo.InvariantCulture), passwordFormat, passwordSalt);
            }
            else
            {
                throw new ProviderException(GetExceptionText(status));
            }
        }

        private void CheckSchemaVersion(string dataSource)
        {
            string[] features = { "Common", "Membership" };
            string version = "1";

            SecUtility.CheckSchemaVersion(
                                            this,
                                           dataSource,
                                           features,
                                           version,
                                           ref _schemaVersionCheck);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Security;
using Codentia.Common.Config;
using Codentia.Common.Data;

namespace Codentia.Common.Membership.Providers
{
    /// <summary>
    /// CESqlRoleProvider - override standard SqlRoleProvider to have dynamic connection strings
    /// </summary>
    public sealed class CESqlRoleProvider : RoleProvider
    {
        private static string _providerDbDataSource = null;
        private string _appName;
        private int _schemaVersionCheck;
        private int _commandTimeout;

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
                    if (membership.Providers[i].Type.ToLower().StartsWith("codentia"))
                    {
                        providerSettings = membership.Providers[i];
                    }
                }

                return providerSettings.Parameters["connectionStringName"];
            }
        }

        /// <summary>
        /// Gets/Sets ApplicationName
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return _appName;
            }

            set
            {
                _appName = value;

                if (_appName.Length > 256)
                {
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_application_name_too_long));
                }
            }
        }
        
        private int CommandTimeout
        {
            get { return _commandTimeout; }
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>
        /// true if the specified user is in the specified role for the configured applicationName; otherwise, false.
        /// </returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            SecUtility.CheckParameter(ref username, true, false, true, 256, "username");
            if (username.Length < 1)
            {
                return false;
            }

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username),
                        new DbParameter("@RoleName", DbType.String, roleName),                       
                    };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_UsersInRoles_IsUserInRole", spParams);

                switch ((int)p.Value)
                {
                    case 0:
                        return false;
                    case 1:
                        return true;
                    case 2:
                        return false;
                    case 3:
                        return false;
                }

                throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unknown_failure));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>
        /// A string array containing the names of all the roles that the specified user is in for the configured applicationName.
        /// </returns>
        public override string[] GetRolesForUser(string username)
        {
            SecUtility.CheckParameter(ref username, true, false, true, 256, "username");
            if (username.Length < 1)
            {
                return new string[0];
            }

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@UserName", DbType.String, username)                                         
                    };

                Dictionary<DataTable, SqlParameter> retVars = DbInterface.ExecuteProcedureDataTableWithReturn(_providerDbDataSource, "dbo.aspnet_UsersInRoles_GetRolesForUser", spParams);
                IEnumerator<DataTable> ie = retVars.Keys.GetEnumerator();
                ie.MoveNext();
                DataTable dt = ie.Current;
                SqlParameter p = retVars[dt];

                StringCollection sc = new StringCollection();

                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        sc.Add(Convert.ToString(dr[0]));
                    }
                }

                if (sc.Count > 0)
                {
                    string[] strReturn = new string[sc.Count];
                    sc.CopyTo(strReturn, 0);
                    return strReturn;
                }

                switch ((int)p.Value)
                {
                    case 0:
                        return new string[0];
                    case 1:
                        return new string[0];
                    default:
                        throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unknown_failure));
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public override void CreateRole(string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),                        
                        new DbParameter("@RoleName", DbType.String, roleName),                       
                    };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Roles_CreateRole", spParams);

                switch ((int)p.Value)
                {
                    case 0:
                        return;

                    case 1:
                        throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_role_already_exists, roleName));

                    default:
                        throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unknown_failure));
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if <paramref name="roleName"/> has one or more members and do not delete <paramref name="roleName"/>.</param>
        /// <returns>
        /// true if the role was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>
        /// true if the role name already exists in the data source for the configured applicationName; otherwise, false.
        /// </returns>
        public override bool RoleExists(string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),                        
                        new DbParameter("@RoleName", DbType.String, roleName),                       
                    };

                SqlParameter p = DbInterface.ExecuteProcedureWithReturn(_providerDbDataSource, "dbo.aspnet_Roles_RoleExists", spParams);

                switch ((int)p.Value)
                {
                    case 0:
                        return false;
                    case 1:
                        return true;
                }

                throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unknown_failure));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            SecUtility.CheckArrayParameter(ref roleNames, true, true, true, 256, "roleNames");
            SecUtility.CheckArrayParameter(ref usernames, true, true, true, 256, "usernames");

            Guid txnId = Guid.NewGuid();

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                int numUsersRemaing = usernames.Length;
                while (numUsersRemaing > 0)
                {
                    int iter;
                    string allUsers = usernames[usernames.Length - numUsersRemaing];
                    numUsersRemaing--;
                    for (iter = usernames.Length - numUsersRemaing; iter < usernames.Length; iter++)
                    {
                        if (allUsers.Length + usernames[iter].Length + 1 >= 4000)
                        {
                            break;
                        }

                        allUsers += "," + usernames[iter];
                        numUsersRemaing--;
                    }

                    int numRolesRemaining = roleNames.Length;
                    while (numRolesRemaining > 0)
                    {
                        string allRoles = roleNames[roleNames.Length - numRolesRemaining];
                        numRolesRemaining--;
                        for (iter = roleNames.Length - numRolesRemaining; iter < roleNames.Length; iter++)
                        {
                            if (allRoles.Length + roleNames[iter].Length + 1 >= 4000)
                            {
                                break;
                            }

                            allRoles += "," + roleNames[iter];
                            numRolesRemaining--;
                        }

                        AddUsersToRolesCore(allUsers, allRoles, txnId);
                    }
                }

                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);

                throw;
            }
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            SecUtility.CheckArrayParameter(ref roleNames, true, true, true, 256, "roleNames");
            SecUtility.CheckArrayParameter(ref usernames, true, true, true, 256, "usernames");

            Guid txnId = Guid.NewGuid();
            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                int numUsersRemaing = usernames.Length;
                while (numUsersRemaing > 0)
                {
                    int iter;
                    string allUsers = usernames[usernames.Length - numUsersRemaing];
                    numUsersRemaing--;
                    for (iter = usernames.Length - numUsersRemaing; iter < usernames.Length; iter++)
                    {
                        if (allUsers.Length + usernames[iter].Length + 1 >= 4000)
                        {
                            break;
                        }

                        allUsers += "," + usernames[iter];
                        numUsersRemaing--;
                    }

                    int numRolesRemaining = roleNames.Length;
                    while (numRolesRemaining > 0)
                    {
                        string allRoles = roleNames[roleNames.Length - numRolesRemaining];
                        numRolesRemaining--;
                        for (iter = roleNames.Length - numRolesRemaining; iter < roleNames.Length; iter++)
                        {
                            if (allRoles.Length + roleNames[iter].Length + 1 >= 4000)
                            {
                                break;
                            }

                            allRoles += "," + roleNames[iter];
                            numRolesRemaining--;
                        }

                        RemoveUsersFromRolesCore(allUsers, allRoles, txnId);
                    }
                }

                DbInterface.CommitTransaction(txnId);
            }
            catch
            {
                DbInterface.RollbackTransaction(txnId);
                throw;
            }
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>
        /// A string array containing the names of all the users who are members of the specified role for the configured applicationName.
        /// </returns>
        public override string[] GetUsersInRole(string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),                        
                        new DbParameter("@RoleName", DbType.String, roleName)                                              
                    };

                Dictionary<DataTable, SqlParameter> retVars = DbInterface.ExecuteProcedureDataTableWithReturn(_providerDbDataSource, "dbo.aspnet_UsersInRoles_GetUsersInRoles", spParams);
                IEnumerator<DataTable> ie = retVars.Keys.GetEnumerator();
                ie.MoveNext();
                DataTable dt = ie.Current;
                SqlParameter p = retVars[dt];

                StringCollection sc = new StringCollection();

                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        sc.Add(Convert.ToString(dr[0]));
                    }
                }

                if (sc.Count < 1)
                {
                    switch ((int)p.Value)
                    {
                        case 0:
                            return new string[0];
                        case 1:
                            throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_role_not_found, roleName));
                    }

                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unknown_failure));
                }

                string[] strReturn = new string[sc.Count];
                sc.CopyTo(strReturn, 0);
                return strReturn;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured applicationName.
        /// </returns>
        public override string[] GetAllRoles()
        {
            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName)                                                                  
                    };

                Dictionary<DataTable, SqlParameter> retVars = DbInterface.ExecuteProcedureDataTableWithReturn(_providerDbDataSource, "dbo.aspnet_Roles_GetAllRoles", spParams);
                IEnumerator<DataTable> ie = retVars.Keys.GetEnumerator();
                ie.MoveNext();
                DataTable dt = ie.Current;
                SqlParameter p = retVars[dt];

                StringCollection sc = new StringCollection();

                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        sc.Add(Convert.ToString(dr[0]));
                    }
                }

                string[] strReturn = new string[sc.Count];
                sc.CopyTo(strReturn, 0);
                return strReturn;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>
        /// A string array containing the names of all the users where the user name matches <paramref name="usernameToMatch"/> and the user is a member of the specified role.
        /// </returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 256, "usernameToMatch");

            try
            {
                CheckSchemaVersion(_providerDbDataSource);

                DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),
                        new DbParameter("@RoleName", DbType.String, roleName),
                        new DbParameter("@UserNameToMatch", DbType.String, usernameToMatch)    
                    };

                Dictionary<DataTable, SqlParameter> retVars = DbInterface.ExecuteProcedureDataTableWithReturn(_providerDbDataSource, "dbo.aspnet_Roles_GetAllRoles", spParams);
                IEnumerator<DataTable> ie = retVars.Keys.GetEnumerator();
                ie.MoveNext();
                DataTable dt = ie.Current;
                SqlParameter p = retVars[dt];

                StringCollection sc = new StringCollection();

                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        sc.Add(Convert.ToString(dr[0]));
                    }
                }

                if (sc.Count < 1)
                {
                    switch ((int)p.Value)
                    {
                        case 0:
                            return new string[0];

                        case 1:
                            throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_role_not_found, roleName));

                        default:
                            throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unknown_failure));
                    }
                }

                string[] strReturn = new string[sc.Count];
                sc.CopyTo(strReturn, 0);
                return strReturn;
            }
            catch
            {
                throw;
            }
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
                name = "CESqlRoleProvider";
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", ProviderStrings.GetString(ProviderStrings.RoleSqlProvider_description));
            }
            
            base.Initialize(name, config);
            _providerDbDataSource = config["connectionStringName"];

            _schemaVersionCheck = 0;

            _commandTimeout = SecUtility.GetIntValue(config, "commandTimeout", 30, true, 0);

            string temp = config["connectionStringName"];
            if (temp == null || temp.Length < 1)
            {
                throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Connection_name_not_specified));
            }

            _appName = config["applicationName"];
            if (string.IsNullOrEmpty(_appName))
            {
                _appName = SecUtility.GetDefaultAppName();
            }

            if (_appName.Length > 256)
            {
                throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_application_name_too_long));
            }

            config.Remove("connectionStringName");
            config.Remove("applicationName");
            config.Remove("commandTimeout");
            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!string.IsNullOrEmpty(attribUnrecognized))
                {
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unrecognized_attribute, attribUnrecognized));
                }
            }
        }
        
        private void CheckSchemaVersion(string dataSource)
        {
            string[] features = { "Role Manager" };
            string version = "1";

            SecUtility.CheckSchemaVersion(
                                           this,
                                           dataSource,
                                           features,
                                           version,
                                           ref _schemaVersionCheck);
        }

        private void AddUsersToRolesCore(string usernames, string roleNames, Guid txnId)
        {
            DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),                        
                        new DbParameter("@RoleNames", DbType.String, roleNames),                       
                        new DbParameter("@UserNames", DbType.String, usernames),    
                        new DbParameter("@CurrentTimeUtc", DbType.DateTime, DateTime.UtcNow)
                    };

            Dictionary<DataTable, SqlParameter> retVars = DbInterface.ExecuteProcedureDataTableWithReturn(_providerDbDataSource, "dbo.aspnet_UsersInRoles_AddUsersToRoles", spParams);
            IEnumerator<DataTable> ie = retVars.Keys.GetEnumerator();
            ie.MoveNext();
            DataTable dt = ie.Current;
            SqlParameter p = retVars[dt];

            string s1 = string.Empty, s2 = string.Empty;

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    if (dt.Columns.Count > 0)
                    {
                        s1 = Convert.ToString(dr[0]);
                    }

                    if (dt.Columns.Count > 1)
                    {
                        s1 = Convert.ToString(dr[1]);
                    }
                }
            }

            switch ((int)p.Value)
            {
                case 0:
                    return;
                case 1:
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_this_user_not_found, s1));
                case 2:
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_role_not_found, s1));
                case 3:
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_this_user_already_in_role, s1, s2));
            }

            throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unknown_failure));
        }

        private void RemoveUsersFromRolesCore(string usernames, string roleNames, Guid txnId)
        {
            DbParameter[] spParams =
                    {
                        new DbParameter("@ApplicationName", DbType.String, ApplicationName),                        
                        new DbParameter("@UserNames", DbType.String, usernames),
                        new DbParameter("@RoleNames", DbType.String, roleNames)                                               
                    };

            Dictionary<DataTable, SqlParameter> retVars = DbInterface.ExecuteProcedureDataTableWithReturn(_providerDbDataSource, "dbo.aspnet_UsersInRoles_RemoveUsersFromRoles", spParams);
            IEnumerator<DataTable> ie = retVars.Keys.GetEnumerator();
            ie.MoveNext();
            DataTable dt = ie.Current;
            SqlParameter p = retVars[dt];

            string s1 = string.Empty, s2 = string.Empty;

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    if (dt.Columns.Count > 0)
                    {
                        s1 = Convert.ToString(dr[0]);
                    }

                    if (dt.Columns.Count > 1)
                    {
                        s1 = Convert.ToString(dr[1]);
                    }
                }
            }

            switch ((int)p.Value)
            {
                case 0:
                    return;
                case 1:
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_this_user_not_found, s1));
                case 2:
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_role_not_found, s2));
                case 3:
                    throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_this_user_already_not_in_role, s1, s2));
            }

            throw new ProviderException(ProviderStrings.GetString(ProviderStrings.Provider_unknown_failure));
        }
    }
}
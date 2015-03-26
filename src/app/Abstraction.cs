using System;

namespace Codentia.Common.Membership
{
    /// <summary>
    /// Class responsible for providing DataLayer abstraction. Used as part of strategy to decrease coupling between 
    /// BL and DL - because of the need to run N versions of DLL (potentially different) against the same Database.
    /// </summary>
    internal static class Abstraction
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public static string Version
        {
            get
            {
                Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}.{1}.{2}", currentVersion.Major, currentVersion.Minor, currentVersion.Build);
            }
        }
    }
}

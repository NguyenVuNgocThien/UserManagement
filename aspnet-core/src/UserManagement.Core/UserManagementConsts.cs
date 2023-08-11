using UserManagement.Debugging;

namespace UserManagement
{
    public class UserManagementConsts
    {
        public const string LocalizationSourceName = "UserManagement";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "d13a32b94b664faca57f022bf03a5a4a";
    }
}

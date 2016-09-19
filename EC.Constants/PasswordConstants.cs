using System;
using System.Runtime.Serialization;


namespace EC.Constants
{
    public static class SecurityConstants
    {
        public const string PasswordComplexityPrefix = "System.Security.PasswordComplexity.";
        public const string PasswordHistoryPrefix = "System.Security.PasswordHistory.";
        public const string PasswordLockoutPrefix = "System.Security.PasswordLockout.";
        public const string PasswordExpiryPrefix = "System.Security.PasswordExpiry.";
        public const string LoginPrefix = "System.Security.Login.";
    }

    public static class PasswordConstants
    {
        public const int ENCRYPTION_WORKLOAD = 5;

        /// <summary>
        /// System.Security.PasswordComplexity.MinLength: Minimum length required for the password
        /// </summary>
        public const string PASSWORD_MINIMUM_LENGTH = SecurityConstants.PasswordComplexityPrefix + "MinLength";

        /// <summary>
        /// System.Security.PasswordComplexity.MinUpperCaseChars: Minimum number of uppercase characters [A-Z] required
        /// </summary>
        public const string PASSWORD_MINIMUM_UPPERCASE_CHARS = SecurityConstants.PasswordComplexityPrefix + "MinUpperCaseChars";

        /// <summary>
        /// System.Security.PasswordComplexity.MinLowerCaseChars: Minimum number of lowercase characters [a-z] required
        /// </summary>
        public const string PASSWORD_MINIMUM_LOWERCASE_CHARS = SecurityConstants.PasswordComplexityPrefix + "MinLowerCaseChars";

        /// <summary>
        /// System.Security.PasswordComplexity.MinNumericChars: Minimum number of numeric characters [0-9] required
        /// </summary>
        public const string PASSWORD_MINIMUM_NUMERIC_CHARS = SecurityConstants.PasswordComplexityPrefix + "MinNumericChars";

        /// <summary>
        /// System.Security.PasswordComplexity.MinSymbolChars: Minimum number of symbol characters [`~!@#$%^&*()_-+={}[]\|:;"'<>,.?/] and space required
        /// </summary>
        public const string PASSWORD_MINIMUM_SYMBOL_CHARS = SecurityConstants.PasswordComplexityPrefix + "MinSymbolChars";
    }

    public static class PasswordHistoryConstants
    {
        /// <summary>
        /// System.Security.PasswordHistory.PasswordHistoryInterval: Number of previous passwords remembered
        /// The user is not allowed to reuse any passwords that are in their password history.
        /// </summary>
        public const string PASSWORD_HISTORY_INTERVALS = SecurityConstants.PasswordHistoryPrefix + "PasswordHistoryInterval";
    }

    public static class PasswordExpiryConstants
    {
        /// <summary>
        /// System.Security.PasswordExpiry.ExpiryDuration: The password expiry duration in days
        /// </summary>
        public const string PASSWORD_EXPIRY_DURATION = SecurityConstants.PasswordExpiryPrefix + "ExpiryDuration";

        /// <summary>
        /// System.Security.PasswordExpiry.ExpiryWarningDuration: The password expiry duration in days
        /// </summary>
        public const string PASSWORD_EXPIRY_WARNING_DURATION = SecurityConstants.PasswordExpiryPrefix + "ExpiryWarningDuration";

    }

    public static class PasswordLockoutConstants
    {
        /// <summary>
        /// System.Security.PasswordLockout.LockOutCount: The password lockout count in number
        /// </summary>

        public const string PASSWORD_LOCKOUT_COUNT = SecurityConstants.PasswordLockoutPrefix + "LockoutCount";

        /// <summary>
        /// System.Security.PasswordLockout.LockOutWindow: The password lockout window in number
        /// </summary>

        public const string PASSWORD_LOCKOUT_WINDOW = SecurityConstants.PasswordLockoutPrefix + "LockoutWindow";
    }

}

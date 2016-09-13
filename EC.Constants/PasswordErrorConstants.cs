using System;
using System.Runtime.Serialization;

namespace EC.Constants
{
    public enum PasswordErrors
    {
	   PASSWORD_MINIMUM_LENGTH,
       PASSWORD_MINIMUM_LOWERCASE_CHARS ,
       PASSWORD_MINIMUM_UPPERCASE_CHARS ,
       PASSWORD_MINIMUM_NUMERIC_CHARS ,
       PASSWORD_MINIMUM_SYMBOL_CHARS,
       PASSWORD_HISTORY_ENTRIES 
    };

}

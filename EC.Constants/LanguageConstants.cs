using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    public class LanguageConstants
    {
        public const string Default_Culture_String = "en-US";


        public const string English = "English";
        public const string English_Native = "English";
        public const string English_Code = "en";
        public const string English_Culture_String = "en-US";

        public const string French = "French";
        public const string French_Native = "Français";
        public const string French_Code = "fr";

        public const string Spanish = "Spanish";
        public const string Spanish_Native = "Español";
        public const string Spanish_Code = "es";

        public const string Russian = "Russian";
        public const string Russian_Native = "Русский";
        public const string Russian_Code = "ru";

        public const string Arabic = "Arabic";
        public const string Arabic_Native = "العربية";
        public const string Arabic_Code = "ar";

        public const string German = "German";
        public const string German_Native = "Deutch";
        public const string German_Code = "ge";

        public enum Language_Values
        {
            English = 1,
            French = 2,
            Spanish = 3,
            Russian = 4,
            Arabic = 5,
            German = 6,
        }
    }
}

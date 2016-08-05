using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common
{
    class Language
    {
        public readonly static string English = "English";
        public readonly static string English_Native = "English";
        public readonly static string English_Code = "en";

        public readonly static string French = "French";
        public readonly static string French_Native = "Français";
        public readonly static string French_Code = "fr";

        public readonly static string Spanish = "Spanish";
        public readonly static string Spanish_Native = "Español";
        public readonly static string Spanish_Code = "es";

        public readonly static string Russian = "Russian";
        public readonly static string Russian_Native = "Русский";
        public readonly static string Russian_Code = "ru";

        public readonly static string Arabic = "Arabic";
        public readonly static string Arabic_Native = "العربية";
        public readonly static string Arabic_Code = "ar";

        public readonly static string German = "German";
        public readonly static string German_Native = "Deutch";
        public readonly static string German_Code = "ge";

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

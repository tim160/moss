using System.Linq;

namespace EC.Common.Util
{
    public class CheckBrowser
    {
        public static readonly string[] OLD_BROWSER_LIST = { "internetexplorer11", "ie10", "ie9", "ie8" };
        public static bool detectOldIE(string Type)
        {
            if (OLD_BROWSER_LIST.Contains(Type.ToLower()))
            {
                return true;
            }
            return false;
        }
    }
}
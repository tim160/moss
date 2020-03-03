using EC.Localization;

namespace EC.Services
{
    public class URLService
    {
        public string generateHeader(string hrefURL)
        {
            string page_subtitle = string.Empty;
            hrefURL = hrefURL.ToLower();

            if (hrefURL.Contains("newcase/index/") || hrefURL.Contains("/newcase/"))
            {
                page_subtitle = LocalizationGetter.GetString("report");
            }
            else if (hrefURL.Contains("newcase/messages/") || hrefURL.Contains("newcase/reporter"))
            {
                page_subtitle = LocalizationGetter.GetString("Messages");
            }
            else if (hrefURL.Contains("newcase/team/"))
            {
                page_subtitle = LocalizationGetter.GetString("Team");
            }
            else if (hrefURL.Contains("newcase/investigationnotes/") || hrefURL.Contains("newcase/caseclosurereport"))
            {
                page_subtitle = LocalizationGetter.GetString("Investigation");
            }
            else if (hrefURL.Contains("newcase/attachments"))
            {
                page_subtitle = LocalizationGetter.GetString("Attachments");
            }
            else if (hrefURL.Contains("newcase/activity"))
            {
                page_subtitle = LocalizationGetter.GetString("Activity");
            }
            else if (hrefURL.Contains("newcase/tasks"))
            {
                page_subtitle = LocalizationGetter.GetString("Tasks");
            }
            else
            {
                page_subtitle = "";
            }

            return page_subtitle;
        }
    }
}
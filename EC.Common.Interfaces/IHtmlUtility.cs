using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    public interface IHtmlUtility
    {

        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="source">Html source</param>
        /// <returns>Clean output</returns>
        string SanitizeHtml(string source);

        /// <summary>
        /// Takes a raw source and removes all HTML tags
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        string StripHtml(string source);
    }
}


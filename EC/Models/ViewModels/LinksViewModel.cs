using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
    public class LinksViewModel
    {
        public LinksViewModel(string url, string linkText)
        {
            this.url = url;
            this.linkText = linkText;
        }

        public string url { get; private set; }
        public string linkText { get; private set; }

    }
}
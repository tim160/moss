using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
  public class SSOErrorViewModel
  {
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
  }
}
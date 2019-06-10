using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.COM.Data;

namespace EC.COM.Controllers
{
  public class VideoController : Controller
  {
    // GET: Video
    public ActionResult Index(string id, string invitation, string emailedcode)
    {
      
      ViewBag.preReg = false;
      if (!string.IsNullOrWhiteSpace(invitation) && !string.IsNullOrWhiteSpace(emailedcode))
      {
        using (var db = new DBContext())
        {
          var model = db.CompanyInvitations.FirstOrDefault(x => x.Invitation_code.ToLower().Trim() == invitation.ToLower().Trim());
          if (model != null && model.Employee_price_type == 3)
          {

            ViewBag.preReg = true;
            ViewBag.invitation = invitation;
            ViewBag.emailedcode = emailedcode;

          }
        }
      }

      return View();
    }
  }
}
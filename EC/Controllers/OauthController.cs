using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC.Controllers
{
    public class OauthController : Controller
    {
        // GET: Oauth
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Token(string jwt)
        {
            //{
         /*     "access_token": < Your Token >,
        "token_type": "bearer",
        "expires_in": 7200,
        "created_at": 1429931390*/
            //}
          }
    }
}
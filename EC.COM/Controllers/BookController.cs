using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC.COM.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index(string id = "")
        {
            var data = System.Text.Encoding.Default.GetString(System.Convert.FromBase64String(id)).Split('|');
            ViewBag.FirstName = data[0];
            ViewBag.LastName = data[1];
            ViewBag.CompanyName = data[2];
            ViewBag.Phone = data[3];
            ViewBag.Email = data[4];
            ViewBag.NumberOfEmployees = data[5];
            ViewBag.InvitationCode = data[6];

            return View();
        }
    }
}
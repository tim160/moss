using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using EC.SSO.Demo.Models;

namespace EC.SSO.Demo.Controllers
{
	public class HomeController : Controller
	{
		private readonly List<User> _users = new List<User>
		{
			new User
			{
				Username="kmilton",
				Name="Kate Milton",
				Password="1234567",
				CompanyName="Stark Industries"
			},
			new User
			{
				Username="4",
				Name="Alex Grace",
				Password="123456",
				CompanyName="Stark Industries"
      },
			new User
			{
				Username="6",
				Name="Jeff Wood",
				Password="123456",
				CompanyName="Stark Industries"
      },
			new User
			{
				Username="222",
				Name="John McDonald",
				Password="123456",
				CompanyName="Stark Industries"
      }
		};

		public ActionResult Index() => View();

    public ActionResult EmbeddedApp(string username) {
      ViewBag.username = username;
      var user = _users.Where(t => t.Username.Trim().ToLower() == username.Trim().ToLower()).FirstOrDefault();
      string pass = "";
      if (user != null)
        pass = user.Password;

      ViewBag.pass = pass;
      return View();

    }

		[HttpPost]
		public ActionResult Login(string username, string password)
		{
			User user = _users.FirstOrDefault(item =>
				item.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
				&& item.Password.Equals(password, StringComparison.Ordinal));
			if (user != null)
			{
				return View("BigButton", user);
			}
			else
			{
				return Content("Incorrect user name or password.");
			}
		}
	}
}
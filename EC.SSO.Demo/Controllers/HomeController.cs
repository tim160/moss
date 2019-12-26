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
				Username="Test1",
				Name="Test 1",
				Password="111",
				CompanyName="Company 1"
			},
			new User
			{
				Username="Test2",
				Name="Test 2",
				Password="222",
				CompanyName="Company 2"
			},
			new User
			{
				Username="Test3",
				Name="Test 3",
				Password="333",
				CompanyName="Company 3"
			},
			new User
			{
				Username="Test4",
				Name="Test 4",
				Password="444",
				CompanyName="Company 4"
			}
		};

		public ActionResult Index() => View();

		public ActionResult EmbeddedApp(string username) => View(nameof(EmbeddedApp), (object)username);

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
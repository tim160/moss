using System;
using System.Linq;
using System.Text.RegularExpressions;
using EC.Common.Util;
using EC.Constants;
using EC.Models.Database;

namespace EC.Models
{
	public class GenerateRecordsModel
	{
		private ECEntities db = new ECEntities();


		public bool isLoginInUse(string login)
		{
			if (db.user.Any(t => t.login_nm.Trim().ToLower() == login.Trim().ToLower()))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool isCompanyInUse(string company_nm)
		{
			if (db.company.Any(t => t.company_nm.Trim().ToLower() == company_nm.Trim().ToLower()))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool isCompanyShortInUse(string company_nm)
		{
			if (db.company.Any(t => t.company_short_name != null && t.company_short_name.Trim().ToLower() == company_nm.Trim().ToLower()))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public string GenerateLoginName(string first, string last)
		{
			Regex rgx = new Regex("[^a-zA-Z]");


			first = rgx.Replace(first, "");
			last = rgx.Replace(last, "");

			first = first.Replace(" ", "");
			last = last.Replace(" ", "");

			string _first_short = "";
			string _last_short = "";

			if (first.Length > 0)
			{
				_first_short = first.ToCharArray()[0].ToString();
			}
			if (last.Length > 3)
			{
				_last_short = last.Substring(0, 4);
			}
			else
			{
				_last_short = last;
			}
			string _login_text_part = _first_short + _last_short;

			_login_text_part = (_login_text_part + StringUtil.RandomLetter(6 - _login_text_part.Length)).ToLower();
			_login_text_part = StringUtil.ReplaceForUI(_login_text_part);

			string _login_int_part = "";

			do
			{
				var random = new Random();
				_login_int_part = random.Next(10, 99).ToString();
				_login_int_part = StringUtil.ReplaceForUI(_login_int_part);

			}
			while (isLoginInUse(_login_text_part + _login_int_part));


			return _login_text_part + _login_int_part;
		}

		public string GenerateCompanyCode(string company_nm)
		{
			Regex rgx = new Regex("[^a-zA-Z]");
			company_nm = rgx.Replace(company_nm, "");
			company_nm = company_nm.Replace(" ", "");
			string _first_short = "";
			string _last_short = "";

			if (company_nm.Length > 2)
			{
				_first_short = company_nm.Substring(0, 3);
			}
			else
			{
				_first_short = company_nm;
			}

			_first_short = (_first_short + StringUtil.RandomLetter(3 - _first_short.Length)).ToUpper().Trim();

			_first_short = StringUtil.ReplaceForUI(_first_short);


			do
			{
				var random = new Random();
				_last_short = random.Next(1001, 9999).ToString();
				_last_short = StringUtil.ReplaceForUI(_last_short);
			}
			while (isCodeInUse(_first_short + _last_short));

			return _first_short + _last_short;
		}

		public string GenerateInvoiceNumber()
		{

			string _number = "INV_";
			string _invoice_ext = "";

			do
			{
				var random = new Random();
				_invoice_ext = random.Next(10001, 99999).ToString();
			}
			while (isInvoiceInUse(_number + _invoice_ext));

			return _number + _invoice_ext;
		}

		public bool isInvoiceInUse(string invoice)
		{
			if (db.company_payments.Any(t => t.auth_code.Trim().ToLower() == invoice.Trim().ToLower()))
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		public bool isCodeInUse(string code)
		{
			if (db.company.Any(t => t.company_code.Trim().ToLower() == code.Trim().ToLower()))
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		public string GeneretedPassword()
		{
			string newPassword = System.Web.Security.Membership.GeneratePassword(PasswordConstants.PASSWORD_MIN_LENGTH, PasswordConstants.PASSWORD_EXTRA_SYMBOLS_COUNT);

			Random rnd = new Random();

			newPassword = Regex.Replace(newPassword, @"[^a-zA-Z0-9]", m => rnd.Next(0, 10).ToString());
			newPassword = StringUtil.ReplaceForUI(newPassword);



			///string password = System.Web.Security.Membership.GeneratePassword(PasswordLength, PasswordExtraSubmolsCount);
			//// password = StringUtil.ReplaceForUI(password);
			return newPassword;
		}



		public string GenerateCaseNumber(int report_id, int company_id, string company_name)
		{
			string letter = "EMP";
			if (company_name.Length > 0)
			{
				letter = company_name[0].ToString().ToUpper();
			}

			if (company_id == 1)
			{
				letter = "UNK";
			}

			if (company_id == 2)
			{
				letter = "STA";
			}

			int number = 2000 + report_id;
			string case_number = number.ToString() + "-" + letter + "-" + company_id.ToString();

			return case_number;
		}

		public string GenerateReporterLogin()
		{
			Random rd = new Random();
			string reporter_login = "";

			do
			{
				reporter_login = reporter_login + rd.Next(0, 9).ToString();
			}
			while (reporter_login.Length < 6);

			//     ?while ((reporter_login.Length + report_id.ToString().Length) < 6);

			return "RE" + reporter_login;
		}
	}
}
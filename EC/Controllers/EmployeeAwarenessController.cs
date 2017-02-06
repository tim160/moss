using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Controllers.Utils;
using EC.Models;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Constants;
using System.IO;

namespace EC.Controllers
{
    public class EmployeeAwarenessController : BaseController
    {
        // GET: EmployeeAwareness
        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;

            ViewBag.user_id = user_id;

            return View();
        }

        public ActionResult Poster(int? id)
        {
            try
            {
                if (!id.HasValue || id > 10)
                    return RedirectToAction("Index", "EmployeeAwareness");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "EmployeeAwareness");
            }

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            ViewBag.user_id = user_id;

            string file_name = "";

            return View();
        }


        private string PosterName(int poster_id)
        {
            string file_name = "";
            switch (poster_id)
            {
                case 1:
                    file_name = "";
                    break;
                case 2:
                    file_name = "";
                    break;
                case 3:
                    file_name = "";
                    break;
                case 4:
                    file_name = "";
                    break;
                case 5:
                    file_name = "";
                    break;
                case 6:
                    file_name = "";
                    break;
                case 7:
                    file_name = "";
                    break;
                case 8:
                    file_name = "";
                    break;
                case 9:
                    file_name = "";
                    break;
                case 10:
                    file_name = "";
                    break;
                case 11:
                    file_name = "";
                    break;
                case 12:
                    file_name = "";
                    break;
                case 13:
                    file_name = "";
                    break;
            }


            return file_name + ".pdf";
        }

        private List<string> EAPdfNames()
        {
            List<string> names = new List<string>();

            DirectoryInfo d = new DirectoryInfo(@"~/EAPdf");
            FileInfo[] Files = d.GetFiles("*.pdf"); //Getting Text files
            string str = "";
            foreach (FileInfo file in Files)
            {
                str = str + ", " + file.Name;
            }

            return names;
        }

        public void DownloadPDF(string path)
        {
       /*     string path = Server.MapPath("PDFs");

            Rectangle r = new Rectangle(400, 300);

            Document doc = new Document(r);

            PdfWriter.GetInstance(doc, new FileStream(path + "/Blocks.pdf", FileMode.Create));

            doc.Open();

            Chunk c1 = new Chunk("A chunk represents an isolated string. ");

            for (int i = 1; i < 4; i++)
            {

                doc.Add(c1);

            }*/
        }


    }
}
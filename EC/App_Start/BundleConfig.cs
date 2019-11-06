using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;

namespace EC
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            BundleTable.EnableOptimizations = true;
//            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
//                            "~/Scripts/WebForms/WebForms.js",
//                            "~/Scripts/WebForms/WebUIValidation.js",
//                            "~/Scripts/WebForms/MenuStandards.js",
//                            "~/Scripts/WebForms/Focus.js",
//                            "~/Scripts/WebForms/GridView.js",
//                            "~/Scripts/WebForms/DetailsView.js",
//                            "~/Scripts/WebForms/TreeView.js",
//                            "~/Scripts/WebForms/WebParts.js"));

            // Order is very important for these files to work, they have explicit dependencies
//            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
//                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
//                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
//                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
//                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // Use the Development version of Modernizr to develop with and learn from. Then, when you’re
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));



      //      bundles.Add(new ScriptBundle("~/bundles/css").Include(
      //          "~/Content/*.css"));
      //      bundles.Add(new ScriptBundle("~/bundles/css").Include(
            //        "~/Content/fonts/*.ttf"));fonts/OpenSans/OpenSans-Light.ttf

          //  bundles.Add(new StyleBundle("~/bundles/Styles")
          //     .Include("~/Content/*.css"));

            bundles.Add(new StyleBundle("~/Content/bStart").Include(
                    "~/Content/Main.css",
                    "~/Content/MainStyle.css"));


            bundles.Add(new StyleBundle("~/Content/aStart").Include(
                    "~/Content/MainStyle.css",
                    "~/Content/style.css",
                    "~/Content/jquery-ui.css"));
               bundles.Add(new StyleBundle("~/Content/caiStart").Include(
                    "~/Content/caiNew.css",
                    "~/Content/style.css",
                    "~/Content/jquery-ui.css"));



               bundles.Add(new StyleBundle("~/Content/styleAnalytics").Include(
                    "~/Content/styleAnalytics.css",
                    "~/Content/daterangepickerAnalytic.css"));

               bundles.Add(new StyleBundle("~/Content/lcnr").Include(
                    "~/Content/main.css",
                    "~/Content/styleReportDashboard.css",
                    "~/Content/styleTasks.css",
                     "~/Content/stylePageMessages.css",
                     "~/Content/jquery-ui.css"));

               bundles.Add(new StyleBundle("~/Content/hffp").Include(
                    "~/Content/style.css",
                    "~/Content/jquery-ui.css"));



            bundles.Add(new StyleBundle("~/bundles/Styles").Include(
                "~/Content/*.css"));
            bundles.Add(new Bundle("~/bundles/ScriptsJq").Include(
                "~/Scripts/lib/jquery-*"));
        //    bundles.Add(new StyleBundle("~/bundles/Fonts").Include(
         //       "~/Content/fonts/*.ttf").Include("~/Content/fonts/OpenSans/*.ttf"));

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "respond",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/respond.min.js",
                    DebugPath = "~/Scripts/respond.js",
                });

            
        }
    }
}
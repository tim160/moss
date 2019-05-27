using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using EC.Models.Database;
using EC.Constants;

namespace EC.Models.Utils
{
    public class FileUtils
    {
        public readonly static string Root = HostingEnvironment.MapPath("~/");
        public readonly static string UploadedDirectory = "Upload";
        public readonly static string UploadTarget = Root  + UploadedDirectory+@"\";


        public static attachment SaveFile(HttpFileCollectionBase files, string file, string newFielPath)
        {
            var fileItem = files[file];
            if (files.Count <= 0) return null;
            if (fileItem.ContentLength <= 0) return null;
   ///////remove to make a check         if (fileItem.ContentLength > FileConstants.maxFileAllowed) return null;

            var fileName = DateTime.Now.Ticks + Path.GetExtension(fileItem.FileName);
            fileItem.SaveAs(UploadTarget + fileName);
            string extension = Path.GetExtension(fileItem.FileName);
            string path = @"\" + UploadedDirectory + @"\" + fileName;
            attachment attach = new attachment
            {
                report_message_id = null,
                status_id = 2, //soglasno letter
                path_nm = path,
                file_nm = fileItem.FileName,
                extension_nm = extension,
                user_id = 1,
                effective_dt = System.DateTime.Now,
                expiry_dt = System.DateTime.Now,
                last_update_dt = System.DateTime.Now
            };
            //return @"\" + UploadedDirectory + @"\" + fileName;
            return attach;
        }
    }
}
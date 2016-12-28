using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
/////using ServicesExternalEmail.DBModel;

namespace ServicesExternalEmail
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            EmailProcess ep = new EmailProcess();

            if(ConfigurationSettings.AppSettings["GD"] == "1")
                ep.Initialize_GD();

            ep.CheckEmails();
        }

        private class EmailProcess
        {
            #region Properties
            private string m_Server;
            private string m_Username;
            private string m_Password;
            private int m_Port;
            private string m_FromAddress;
            private SmtpDeliveryMethod m_DeliveryMethod = SmtpDeliveryMethod.Network;

            public string Server
            {
                get
                {
                    return m_Server;
                }
                set
                {
                    m_Server = value;
                }
            }

            public string Username
            {
                get
                {
                    return m_Username;
                }
                set
                {
                    m_Username = value;
                }
            }

            public int Port
            {
                get
                {
                    return m_Port;
                }
                set
                {
                    m_Port = value;
                }
            }

            public string Password
            {
                set { m_Password = value; }
            }

            public string AddressFrom
            {
                get
                {
                    return m_FromAddress;
                }
                set
                {
                    m_FromAddress = value;
                }
            }
            #endregion

            internal void Initialize_GD()
            {

                m_FromAddress = ConfigurationSettings.AppSettings["GDfromAddress"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationSettings.AppSettings["GDsmtpClient"]);

                // smtpClient.Credentials = new System.Net.NetworkCredential("test@voteplayers.com", "123456");
                //  smtpClient.Send(mailMessage);
                m_Server = ConfigurationSettings.AppSettings["GDserver"];
                m_Username = ConfigurationSettings.AppSettings["GDusername"];
                m_Password = ConfigurationSettings.AppSettings["GDpassword"];
                m_Port = Convert.ToInt16(ConfigurationSettings.AppSettings["GDport"]);

                /*
                m_FromAddress = "employeeconfidential@employeeconfidential.com";
                SmtpClient smtpClient = new SmtpClient("employeeconfidential.com");

                // smtpClient.Credentials = new System.Net.NetworkCredential("test@voteplayers.com", "123456");
                //  smtpClient.Send(mailMessage);
                m_Server = "employeeconfidential.com";
                m_Username = "employeeconfidential@employeeconfidential.com";
                m_Password = "confidentialConfidential1$3";
                m_Port = 25;*/
            }
            public void CheckEmails()
            {
          /*      ECEntities db = new ECEntities();
                ////   using EC.Models.Database;

                ////   using EC.Models.Database;
                ////   System.Data.Entity.DbSet<company> _company1 = new System.Data.Entity.DbSet<company>();

                foreach (notification_processed _np in db.notification_processed.Where(t => t.is_processed == false))
                {
                    // need to process email
                    string body = _np.notification_text;
                    // Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NewTask, body, true);
                    //  if(_company)
                    try
                    {
                        string result = Send(_np.receivers.Split(';').ToList(), _np.cc.Split(';').ToList(), _np.subject, _np.notification_text, false);
                        if(result == "")
                        using (ECEntities adv = new ECEntities())
                        {
                            _np.is_processed = true;
                            adv.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    { }

                }*/
                // later - redo for cleaning
            /*    foreach (notification_processed _np in db.notification_processed.Where(t => t.is_processed == true && t.created_dt < DateTime.Today.AddDays(-3)))
                {
                    using (ECEntities adv = new ECEntities())
                    {
                        adv.notification_processed.Attach(_np);
                        adv.notification_processed.Remove(_np);
                        adv.SaveChanges();
                    }
                }*/

            }

            

            public string ParseEmailAddress(string inputString)
            {
                string[] distLists = inputString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> EmailDictionary = new Dictionary<string, string>();

                //          DistListManagement distListManagement = new DistListManagement();
                //         DistListAssociationManagement distListAssociationManagement = new DistListAssociationManagement();

                for (int i = 0; i < distLists.Length; i++)
                {
                    string retDistList = distLists[i].Trim();//  distListManagement.GetDistList(distLists[i].Trim());

                    if (retDistList != null)
                    {
                        //          DistListAssociation[] retDistributionAssocationList = null;
                        //             retDistributionAssocationList = distListAssociationManagement.GetDistListAssociations(retDistList.ObjectKey);
                        //         if (retDistributionAssocationList != null)
                        {
                            ///                for (int j = 0; j < retDistributionAssocationList.Length; j++)
                            {
                                //     if (!EmailDictionary.ContainsKey(retDistributionAssocationList[j].Email.ToLower()))
                                {
                                    //           EmailDictionary.Add(retDistributionAssocationList[j].Email.ToLower(), retDistributionAssocationList[j].Name);
                                }

                                if (!EmailDictionary.ContainsKey(retDistList.ToLower()))
                                {
                                    EmailDictionary.Add(retDistList.ToLower(), "");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (IsValidEmailAddress(distLists[i]))
                        {
                            if (!EmailDictionary.ContainsKey(distLists[i].ToLower()))
                            {
                                EmailDictionary.Add(distLists[i].ToLower(), string.Empty);
                            }
                        }
                    }
                }

                StringBuilder outputStringBuffer = new StringBuilder();

                foreach (KeyValuePair<string, string> pair in EmailDictionary)
                {
                    if (pair.Value != string.Empty)
                    {
                        outputStringBuffer.Append(pair.Value);
                        outputStringBuffer.Append(" <");
                        outputStringBuffer.Append(pair.Key);
                        outputStringBuffer.Append(">");
                    }
                    else
                    {
                        outputStringBuffer.Append(pair.Key);
                    }
                    outputStringBuffer.Append(';');
                }

                if (EmailDictionary.Count > 0)
                    outputStringBuffer.Remove(outputStringBuffer.Length - 1, 1);

                return outputStringBuffer.ToString();
            }

            public static bool IsValidEmailAddress(string sEmail)
            {
                if (sEmail == null)
                {
                    return false;
                }

                int nFirstAT = sEmail.IndexOf('@');
                int nLastAT = sEmail.LastIndexOf('@');

                if ((nFirstAT > 0) && (nLastAT == nFirstAT) &&
                (nFirstAT < (sEmail.Length - 1)))
                {
                    string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                    Regex re = new Regex(strRegex);
                    return re.IsMatch(sEmail);
                }
                else
                {
                    return false;
                }
            }

            public bool ValidateEmailAddress(string email)
            {
                bool isValid = true;

                if (!string.IsNullOrWhiteSpace(email) && !IsValidEmailAddress(email))
                {
                    isValid = false;
                }

                return isValid;
            }



            public void Send(List<string> to, List<string> CC, string messageSubject, string messageBody)
            {
                StringBuilder emailTo = new StringBuilder();
                StringBuilder emailCC = new StringBuilder();

                if (to != null && to.Count > 0)
                {
                    foreach (string dla in to)
                    {
                        emailTo.Append(dla);
                        emailTo.Append(";");
                    }
                }
                if (CC != null && CC.Count > 0)
                {
                    foreach (string dla in CC)
                    {
                        emailCC.Append(dla);
                        emailCC.Append(";");
                    }
                }
                Send(AddressFrom, emailTo.ToString(), emailCC.ToString(), messageSubject, messageBody, new string[0], false);
            }


            public string Send(List<string> to, List<string> CC, string messageSubject, string messageBody, bool isHTml)
            {
                StringBuilder emailTo = new StringBuilder();
                StringBuilder emailCC = new StringBuilder();

                if (to != null && to.Count > 0)
                {
                    foreach (string dla in to)
                    {
                        emailTo.Append(dla);
                        emailTo.Append(";");
                    }
                }
                if (CC != null && CC.Count > 0)
                {
                    foreach (string dla in CC)
                    {
                        emailCC.Append(dla);
                        emailCC.Append(";");
                    }
                }
                string result = Send(AddressFrom, emailTo.ToString(), emailCC.ToString(), messageSubject, messageBody, new string[0], isHTml);
                return result;
            }

            public void Send(List<string> to, List<string> CC, string messageSubject, string messageBody, bool isHTml, string from)
            {
                StringBuilder emailTo = new StringBuilder();
                StringBuilder emailCC = new StringBuilder();

                if (to != null && to.Count > 0)
                {
                    foreach (string dla in to)
                    {
                        emailTo.Append(dla);
                        emailTo.Append(";");
                    }
                }
                if (CC != null && CC.Count > 0)
                {
                    foreach (string dla in CC)
                    {
                        emailCC.Append(dla);
                        emailCC.Append(";");
                    }
                }
                Send(from, emailTo.ToString(), emailCC.ToString(), messageSubject, messageBody, new string[0], isHTml);
            }

            public void Send(string to, string messageSubject, string messageBody)
            {
                Send(AddressFrom, to, string.Empty, messageSubject, messageBody, new string[0], false);
            }

            public void Send(string to, string cc, string messageSubject, string messageBody)
            {
                Send(AddressFrom, to, cc, messageSubject, messageBody, new string[0], false);
            }

            /*    public void Send(string to, string cc, string messageSubject, string messageBody, byte[] attachmentData, String attachmentFileName, bool isBodyHtml)
               {
                   Send(to, cc, messageSubject, messageBody, new byte[][] { attachmentData }, new string[] { attachmentFileName }, isBodyHtml);
               }

              public void Send(string to, string cc, string messageSubject, string messageBody, byte[][] attachmentDatas, String[] attachmentFileNames, bool isBodyHtml)
               {
                   if (attachmentDatas == null || attachmentFileNames == null || attachmentDatas.Length != attachmentFileNames.Length)
                       attachmentFileNames = new String[0];

                   String tempPath = Path.GetTempPath();
                   for (int i = 0; i < attachmentFileNames.Length; i++)
                   {
                       try
                       {
                           if (!attachmentFileNames[i].Contains("\\"))
                               attachmentFileNames[i] = Path.Combine(tempPath, attachmentFileNames[i]);
                           using (BinaryWriter bw = new BinaryWriter(new FileStream(attachmentFileNames[i], FileMode.Create)))
                           {
                               bw.Write(attachmentDatas[i]);
                           }
                       }
                       catch (IOException ex)
                       {
                           //            m_Log.Error(String.Format("Error writing attachment to file {0}", attachmentFileNames[i]), ex);
                       }
                   }

                   ActionResult result = Send(AddressFrom, to, cc, messageSubject, messageBody, attachmentFileNames, isBodyHtml);

                   for (int i = 0; i < attachmentFileNames.Length; i++)
                       File.Delete(attachmentFileNames[i]);

                   return result;
               }

               public void Send(string to, string cc, string messageSubject, string messageBody, string[] attachments, bool isBodyHtml)
               {
                   return Send(AddressFrom, to, cc, messageSubject, messageBody, attachments, isBodyHtml);
               }
            */
            public string Send(string fromAddress, string to, string cc, string messageSubject, string messageBody, string[] attachments, bool isBodyHtml)
            {
                string toAddress = ParseEmailAddress(to);
                string ccAddress = ParseEmailAddress(cc);

                string[] toAddressArray;
                if (toAddress != string.Empty)
                    toAddressArray = toAddress.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                else
                    toAddressArray = new String[0];

                string[] ccAddressArray;
                if (ccAddress != string.Empty)
                    ccAddressArray = ccAddress.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                else
                    ccAddressArray = new String[0];

                string result = Send(fromAddress, toAddressArray, messageSubject, messageBody, ccAddressArray, attachments, isBodyHtml);
                ////       if (result.ReturnCode == ReturnCode.Success || result.ReturnCode == ReturnCode.SuccessWithErrors)
                {
                    // Log this email
                    //         NotificationLog[] notificationLogs = { new NotificationLog(toAddress, ccAddress, messageSubject, messageBody, (attachments!= null && attachments.Length > 0)) };

                    //            NotificationLogManagement notificationLogManagement = new NotificationLogManagement();
                    //           notificationLogManagement.AddNotificationLogs(notificationLogs);
                }

                 return result;
            }

            public void Send(string to, string cc, string subject, string subjectTemplate, string bodyTemplate, string[] attachments)
            {
                bool isBodyHtml = false;

                // Get message subject
                string messageSubject;
                messageSubject = subject;

                // Get message body
                // string messageBody = GetFormattedOrderText(order, bodyTemplate, out isBodyHtml);
                string messageBody = "";

     ////           Send(to, cc, messageSubject, messageBody, attachments, isBodyHtml);
            }

            public string Send(string fromAddress, string[] to, string messageSubject, string messageBody, string[] cc, string[] attachments, bool isBodyHtml)
            {
                //if (!m_TemplatesInitialized)
                //{
                //    string err = "Send() - Email templates have not been initialized";
                //    m_Log.Error(err);
                //    return new ActionResult(ReturnCode.Fail, err);
                //}

                MailMessage msg = new MailMessage();

                try
                {
                    if (string.IsNullOrEmpty(m_Server))
                        throw new Exception("Send() - Email server not configured");

                    if (string.IsNullOrEmpty(fromAddress))
                        throw new ArgumentException("Send() - From Address not specified", "fromAddress");

                    msg.From = new MailAddress(fromAddress);
                    foreach (string toAddress in to)
                    {
                        msg.To.Add(new MailAddress(toAddress));
                    }

                    msg.Subject = messageSubject;
                    msg.Body = messageBody;

                    foreach (string attachFilename in attachments)
                    {
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachFilename);
                        msg.Attachments.Add(attachment);
                    }
                    msg.IsBodyHtml = isBodyHtml;

                    if (isBodyHtml)
                    {
                        string modifiedbody;
                        List<LinkedResource> foundResources;
                        ExtractLinkedResources(messageBody, out modifiedbody, out foundResources);

                        // Write the html to a memory stream
                        MemoryStream stream = new MemoryStream();
                        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(modifiedbody);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Position = 0;

                        // Configure the mail so it contains the html page
                        msg.Body = "This is a html mail - please use an email client that can read it";
                        AlternateView altView = new AlternateView(stream, System.Net.Mime.MediaTypeNames.Text.Html);

                        // Embed the images into the mail
                        foreach (LinkedResource linkedResource in foundResources)
                        {
                            altView.LinkedResources.Add(linkedResource);
                        }
                        msg.AlternateViews.Add(altView);
                    }

                    foreach (string ccAddress in cc)
                    {
                        msg.CC.Add(new MailAddress(ccAddress));
                    }

                    SmtpClient smtpClient = new SmtpClient(m_Server);

                    //Seems to resolve the problem of some mail servers not relaying emails. D.
                    smtpClient.DeliveryMethod = m_DeliveryMethod;

                    smtpClient.Host = m_Server;
                    smtpClient.Port = m_Port;
                    if (m_Username.Length > 0)
                    {
                        try
                        {
                            NetworkCredential cred = new NetworkCredential(m_Username, m_Password);
                            smtpClient.Credentials = cred;
                        }
                        catch (Exception e1)
                        {
                            ///        m_Log.Error("Send() - Error setting mail server credentials.", e1);
                        }

                    }
                    smtpClient.Send(msg);

                    ///           m_Log.Info(
                    // //              string.Format("Send() - Email sent: recipient [{0}], subject line [{1}]", String.Join(";", to), messageSubject));

                    msg.Attachments.Dispose();

                    return "";
                }
                catch (Exception e)
                {
                    ////           m_Log.Error(
                    // ///             string.Format("Send() - Error sending email: server [{0}], recipient [{1}], message subject line [{2}]",
                    ///                       m_Server, String.Join(";", to), messageSubject),
                    //                     e);

                    msg.Attachments.Dispose();

                    return e.Message + " - " + e.InnerException;
                }
            }


            System.IO.Stream GetImageStream(Uri uri, out string contentType)
            {

                WebRequest request = (WebRequest)WebRequest.Create(uri);


                // Set credentials to use for this request.
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = (WebResponse)request.GetResponse();

                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                BinaryReader binaryReader = new BinaryReader(receiveStream);
                MemoryStream memoryStream = new MemoryStream();
                try
                {
                    while (true)
                    {
                        byte b = binaryReader.ReadByte(); // Not so efficient but it works...
                        memoryStream.WriteByte(b);
                    }
                }
                catch (EndOfStreamException)
                {
                    memoryStream.Position = 0;
                }

                contentType = response.ContentType;
                response.Close();
                return memoryStream;
            }

            public void ExtractLinkedResources(string html, out string modifiedhtml, out List<LinkedResource> linkedResources)
            {
                modifiedhtml = html;
                linkedResources = new List<LinkedResource>();

                Uri localUri = new Uri(AppDomain.CurrentDomain.RelativeSearchPath + "\\Emails\\");

                List<string> imageNames = ExtractImageNames(html);

                int imageID = 1;

                foreach (string imageName in imageNames)
                {
                    try
                    {
                        // Deal with some escape characters that can occur in dynamic image url's
                        string workaroundImageName = imageName.Replace("&amp;", "&");

                        // Generate the uri to retrieve the image - usually an image path is relative to the page uri
                        Uri imageUri = new Uri(localUri, workaroundImageName);

                        // Retrieve the image
                        string contentType;
                        Stream imageStream = GetImageStream(imageUri, out contentType);

                        // Fill the linked resource
                        LinkedResource data = new LinkedResource(imageStream);

                        // Determine a name and set the media type of the linked resource
                        string generatedName = null;
                        if (contentType.ToLower().IndexOf("image/gif") >= 0)
                        {
                            data.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Gif;
                            generatedName = "image" + imageID.ToString() + ".gif";
                        }
                        else if (contentType.ToLower().IndexOf("image/jpeg") >= 0 || workaroundImageName.ToLower().IndexOf(".jpg") >= 0)
                        {
                            data.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                            generatedName = "image" + imageID.ToString() + ".jpeg";
                        }

                        // it is something that I don't handle yet
                        if (generatedName == null)
                            continue;

                        // Generate the linked resource for the image being embedded
                        string generatedSrc = "cid:" + generatedName;
                        data.ContentType.Name = generatedName;
                        data.ContentId = generatedName;
                        data.ContentLink = new Uri(generatedSrc);
                        linkedResources.Add(data);

                        // Let the html refer to the linked resource
                        modifiedhtml = modifiedhtml.Replace(imageName, generatedSrc);
                    }
                    catch
                    {
                        modifiedhtml = modifiedhtml.Replace(imageName, "#");
                    }

                    imageID++;
                }
            }


            /// <summary>
            /// longest length first
            /// </summary>
            private class LengthComparer : IComparer<string>
            {
                public int Compare(string x, string y)
                {
                    return -x.Length.CompareTo(y.Length);
                }
            }

            /// <summary>
            /// Sorts the list of strings so the longest names are in the start of the list
            /// When there is an image named "a.aspx?id=1" and another named "a.aspx?id=12", the list should start with "a.aspx?id=12"
            /// This guarantees that string replacement will not damage the names
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            static private List<string> SortNoDuplicate(List<string> input)
            {
                List<string> result = new List<string>(input);
                IComparer<string> comparer = new LengthComparer();
                result.Sort(comparer);
                return result;
            }

            /// <summary>
            /// Optimistic search for image names in an html document
            /// The images found here will be embedded into the email
            /// </summary>
            /// <param name="html"></param>
            /// <returns></returns>
            private static List<string> ExtractImageNames(string html)
            {
                List<string> imagenames = new List<string>();
                string[] imageattributes = new string[] { "src=", "background=" };
                foreach (string imageattribute in imageattributes)
                {
                    int position = 0;
                    while (position < html.Length)
                    {
                        int foundIndex = html.ToLower().IndexOf(imageattribute, position);
                        if (foundIndex < 0)
                        {
                            position = html.Length;
                        }
                        else
                        {
                            int valueStartIndex = foundIndex + imageattribute.Length + 1;
                            int foundIndexEnd = html.IndexOfAny(new char[] { '\"', ' ', '\'', '>' }, valueStartIndex);
                            if (foundIndexEnd < 0)
                            {
                                position = html.Length;
                            }
                            else
                            {
                                string relativeimagename = html.Substring(valueStartIndex, foundIndexEnd - valueStartIndex);
                                relativeimagename = relativeimagename.Trim(new char[] { '\"', ' ', '\'', '>' });
                                if (!imagenames.Contains(relativeimagename))
                                {
                                    imagenames.Add(relativeimagename);
                                }
                                position = foundIndexEnd;
                            }
                        }
                    }
                }
                return SortNoDuplicate(imagenames);
            }


        }

    }
}


using System;
using System.Xml;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using EC.Common.Util;
using EC.Framework.Logger;
////using EC.Framework.Encryption2;
using System.Collections.Generic;
using System.IO;

namespace EC.Framework.Email
{
    //public delegate string HTMLEmailFormatter(Type[] typeList, object[] entities, string templateData, bool isHtml);

    public class EmailTemplateInfo
    {
        private string m_BodyUri;
        private string m_Subject;

        public string BodyUri
        {
            get { return m_BodyUri; }
        }

        public string Subject
        {
            get { return m_Subject; }
        }


        public EmailTemplateInfo(string bodyUri, string subject)
        {
            m_BodyUri = bodyUri;
            m_Subject = subject;
        }
    }

    // notification: id, subject, text, receiver(s), cc, bcc, is_processed, created_dt, 
    public class MailWrapper
    {
        ////       private static readonly ICustomLog m_Log =
        ///          CustomLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, EmailTemplateInfo> m_EmailTemplates = new Dictionary<string, EmailTemplateInfo>();

        private static MailWrapper m_Instance = new MailWrapper();
        public static MailWrapper Instance
        {
            get
            {
                return m_Instance;
            }
        }

        private string m_Server;
        private string m_Username;
        private string m_Password;
        private int m_Port;
        private string m_FromAddress;
        private bool m_ImpersonateDispatcher = false;
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

        public bool ImpersonateDispatcher
        {
            get { return m_ImpersonateDispatcher; }
        }

        private bool m_Inintialized = false;

        private MailWrapper()
        {
            m_Inintialized = Initialize();
        }

        public ActionResultExtended Send(string toAddress, string messageSubject, string messageBody)
        {
            string[] toAddressArray;
            if (toAddress != string.Empty)
                toAddressArray = toAddress.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
                toAddressArray = new String[0];

            return Send(m_FromAddress, toAddressArray, messageSubject, messageBody, new string[0], new string[0], false);
        }

        public ActionResultExtended Send(string toAddress, string messageSubject, string messageBody, bool isBodyHtml)
        {
            string[] toAddressArray;
            if (toAddress != string.Empty)
                toAddressArray = toAddress.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
                toAddressArray = new String[0];

            return Send(m_FromAddress, toAddressArray, messageSubject, messageBody, new string[0], new string[0], isBodyHtml);
        }

        public ActionResultExtended Send(string toAddress, string messageSubject, string messageBody, string[] cc, bool isBodyHtml)
        {
            string[] toAddressArray;
            if (toAddress != string.Empty)
                toAddressArray = toAddress.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
                toAddressArray = new String[0];

            return Send(m_FromAddress, toAddressArray, messageSubject, messageBody, cc, new string[0], isBodyHtml);
        }

        public ActionResultExtended Send(string toAddress, string messageSubject, string messageBody, string ccAddress, bool isBodyHtml)
        {
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

            return Send(m_FromAddress, toAddressArray, messageSubject, messageBody, ccAddressArray, new string[0], isBodyHtml);
        }


        public ActionResultExtended Send(string toAddress, string messageSubject, string messageBody, string ccAddress, string[] attachments, bool isBodyHtml)
        {
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

            return Send(m_FromAddress, toAddressArray, messageSubject, messageBody, ccAddressArray, attachments, isBodyHtml);
        }

        public ActionResultExtended Send(string fromAddress, string[] to, string messageSubject, string messageBody, string[] cc, string[] attachments, bool isBodyHtml)
        {
            if (!m_Inintialized)
            {
                string err = "Send() - EmailWrapper module has not been initialized";
                /////           m_Log.Error(err);
                return new ActionResultExtended(ReturnCode.Fail, err);
            }

            MailMessage msg = new MailMessage();

            try
            {
                msg.From = new MailAddress(fromAddress);
                foreach (string toAddress in to)
                {
                    msg.To.Add(new MailAddress(toAddress));
                }

                msg.Subject = messageSubject;
                msg.Body = messageBody;

                foreach (string attachFilename in attachments)
                {
                    Attachment attachment = new Attachment(attachFilename);
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
                        //CredentialCache credentials = new CredentialCache();
                        //credentials.Add(
                        //    new Uri("http://" + m_Server),
                        //    "Basic",
                        //    new NetworkCredential(m_Username, m_Password)
                        //    );
                        //credentials.Add(
                        //    new Uri("http://" + m_Server),
                        //    "Digest",
                        //    new NetworkCredential(m_Username, m_Password, m_Server)
                        //    );
                        //smtpClient.Credentials = credentials;
                        NetworkCredential cred = new NetworkCredential(m_Username, m_Password);
                        smtpClient.Credentials = cred;
                    }
                    catch (Exception e1)
                    {
                        ////        m_Log.Error("Send() - Error setting mail server credentials.", e1);
                    }

                }
                smtpClient.Send(msg);

                ////      m_Log.Info(
                ///          string.Format("Send() - Email sent: recipient [{0}], subject line [{1}]", String.Join(";", to), messageSubject));

                msg.Attachments.Dispose();

                return new ActionResultExtended(ReturnCode.Success, string.Empty);
            }
            catch (Exception e)
            {
                ////      m_Log.Error(
                ///         string.Format("Send() - Error sending email: server [{0}], recipient [{1}], message subject line [{2}]",
                ///                       m_Server, String.Join(";", to), messageSubject),
                ////                e);

                msg.Attachments.Dispose();

                return new ActionResultExtended(ReturnCode.Fail,
                                        string.Format("Error sending email: {0}", e.Message));
            }
        }
        private bool Initialize()
        {
            try
            {
                string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                XmlDocument config = new XmlDocument();

                if (System.IO.File.Exists(AppDomain.CurrentDomain.RelativeSearchPath + '\\' +
                    assemblyName.Substring(0, assemblyName.IndexOf(',')) + ".xml"))
                {

                    config.Load(AppDomain.CurrentDomain.RelativeSearchPath + '\\' +
                        assemblyName.Substring(0, assemblyName.IndexOf(',')) + ".xml");
                }
                else if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(assemblyPath) + '\\' +
                    assemblyName.Substring(0, assemblyName.IndexOf(',')) + ".xml"))
                {
                    config.Load(System.IO.Path.GetDirectoryName(assemblyPath) + '\\' +
                        assemblyName.Substring(0, assemblyName.IndexOf(',')) + ".xml");
                }




                XmlElement root = config.DocumentElement;

                m_Server = root.GetElementsByTagName("Server")[0].InnerText.Trim();
                m_Username = root.GetElementsByTagName("Username")[0].InnerText.Trim();
                try
                {
                    m_Port = Int32.Parse(root.GetElementsByTagName("Port")[0].InnerText.Trim());
                }
                catch (Exception)
                {

                }
                if (m_Username.Length > 0)
                {
                    /////                m_Username = Cryptics.Decrypt(m_Username);
                    ///                m_Password = root.GetElementsByTagName("Password")[0].InnerText.Trim();
                    ////               if (m_Password.Length > 0)
                    ////                  m_Password = Cryptics.Decrypt(m_Password);
                }
                if (root.SelectSingleNode("DeliveryMode") != null)
                {
                    string method = root.SelectSingleNode("DeliveryMode").InnerText;
                    try
                    {
                        m_DeliveryMethod = (SmtpDeliveryMethod)Enum.Parse(typeof(SmtpDeliveryMethod), method);
                    }
                    catch (ArgumentException invalidDeliveryMethod)
                    {
                        /////               m_Log.Warn("Initialize() - Invalid delivery method configured, method = " + method, invalidDeliveryMethod);
                        m_DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    }

                }

                if (root.SelectSingleNode("ImpersonateDispatcher") != null)
                {
                    m_ImpersonateDispatcher = bool.Parse(root.SelectSingleNode("ImpersonateDispatcher").InnerText);
                }

                m_FromAddress = root.GetElementsByTagName("AddressFrom")[0].InnerText.Trim();

                XmlNode templateList = root.SelectSingleNode("EmailTemplates");
                foreach (XmlNode templateNode in templateList.ChildNodes)
                {
                    string name = templateNode.Attributes["name"].InnerText;
                    string bodyUri = templateNode.SelectSingleNode("BodyUri").InnerText;
                    string subject = templateNode.SelectSingleNode("Subject").InnerText;

                    EmailTemplateInfo eti = new EmailTemplateInfo(bodyUri, subject);
                    m_EmailTemplates.Add(name, eti);
                }

                return true;
            }
            catch (Exception e)
            {
                /////            m_Log.Error("Initialize() - Error initializing Email module.", e);
                return false;
            }
        }

        public Dictionary<string, EmailTemplateInfo> GetEmailTemplates()
        {
            return m_EmailTemplates;
        }


        ///// <summary>
        ///// Retrieves an image to embed
        ///// </summary>
        ///// <param name="uri"></param>
        ///// <param name="contentType"></param>
        ///// <returns></returns>
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

        private void ExtractLinkedResources(string html, out string modifiedhtml, out List<LinkedResource> linkedResources)
        {
            modifiedhtml = html;
            linkedResources = new List<LinkedResource>();

            Uri localUri = new Uri(AppDomain.CurrentDomain.RelativeSearchPath + "\\EmailTemplates\\");

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

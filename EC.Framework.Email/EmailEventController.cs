using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
//using EC.Framework.Logger;

namespace EC.Framework.Email
{
    public class EmailEventController
    {
   //     private static readonly ICustomLog m_Log =
    //        CustomLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly EmailEventController m_Instance = new EmailEventController();
        public static EmailEventController Instance
        {
            get { return m_Instance; }
        }

        private EmailEventController()
        { }

        private IEmailEventHandler GetInstance(Assembly assembly)
        {
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetInterface("IEmailEventHandler") != null)
                    {
                        return (IEmailEventHandler)type.InvokeMember("Instance", BindingFlags.GetProperty, null, null, new Object[0]);
                    }
                }
            }
            catch (Exception ex)
            {
  ////              m_Log.Error("Could not invoke Instance member", ex);
            }
            return null;
        }

        public void Initialize()
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
            
            DirectoryInfo di = new DirectoryInfo(assemblyPath);
            if (di.Exists)
            {
          ///      m_Log.Debug("Loading email handlers from " + di.Name);
                try
                {
                    FileInfo[] files = di.GetFiles("*Email.dll");
                    foreach (FileInfo file in files)
                    {
               ///         m_Log.Debug(string.Format("Loading assembly: {0}", file.Name));
                        Assembly assembly = Assembly.LoadFrom(di.FullName + "\\" + file.Name);
                        if (assembly != null)
                        {
                            try
                            {
                                IEmailEventHandler handler = GetInstance(assembly);
                                if (handler != null)
                                    handler.Initialize();
                            }
                            catch (ReflectionTypeLoadException rtlex)
                            {
              //                  m_Log.Error(string.Format("{0} - reflection error loading ext.", file.Name), rtlex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
         ///           m_Log.Error("Unknown error occured while trying to load email handlers", ex);
                }
            }
        }


    }
}

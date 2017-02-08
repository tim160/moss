using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using EC.Framework.Logger;

namespace EC.Framework.Data
{
    public class GenDLFactory
    {
        static ICustomLog log = CustomLogManager.GetLogger(typeof(GenDLFactory));
        static bool initialized = false; 

        private static Dictionary<string, IGenDL> _GenDLInstances = new Dictionary<string, IGenDL>();

        public static void Initialize(string genDLassemblyName)
        {
            string searchPath = null; 

            //If running from a non-IIS hosted process, RelativeSearchPath may be null.
            if (String.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
            {
                searchPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, genDLassemblyName);
            }
            else
            {
                searchPath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, genDLassemblyName);
            }

            Assembly genDLAsm = Assembly.LoadFile(searchPath);
            if (genDLAsm == null)
            {
                //log.WarnFormat("GenDL {0} not found at path {1}", genDLassemblyName, searchPath); 
            }
            Type[] types = genDLAsm.GetTypes();
            int typeCount = 0;
            foreach (Type type in types)
            {
                if (type.GetInterface("IGenDL") != null)
                {
                    typeCount++;
                    //log.DebugFormat("Found IGenDL in type {0}, name: {1}, genDLassemblyName: {2}", typeCount, type.FullName, genDLassemblyName);
                    _GenDLInstances[genDLassemblyName] = (IGenDL)type.InvokeMember("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null, new object[0]);
                    initialized = true; 
                    break;
                }
            }
        }

        internal static IGenDL GetInstance(string genDLassemblyName)
        {
            //log.Debug("GenDLFactory.GetInstance entry..."); 
            if (!_GenDLInstances.ContainsKey(genDLassemblyName))
            {
                if (!initialized)
                {
                    throw new InvalidOperationException("GenDLFactory.Initialize(...) must be called first"); 
                }
                //log.WarnFormat("GenDLFactory does not contain assembly {0}, not good, returning null", genDLassemblyName);
                return null; 
            }
            else
            {
                //log.Debug("GenDLFactory.GetInstance entry..."); 
                return _GenDLInstances[genDLassemblyName];
            }
            
        }
    }
}

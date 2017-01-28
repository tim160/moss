using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using Microsoft.Win32;
using EC.Framework.Logger;

namespace EC.Framework.TimeZone
{
    /// <summary>
    /// Information about a time zone
    /// </summary>
    public class TimeZoneInfo : IComparer
    {
        #region Member(s) and Property(s)
       /// private static readonly ICustomLog Log = CustomLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static TimeZoneInfo[] m_TimeZoneInfosAll = null;
        private static TimeZoneInfo[] m_TimeZoneInfosActive = null;
        private static Dictionary<int, TimeZoneInfo> m_IndexDictionary = new Dictionary<int, TimeZoneInfo>();
        private static Dictionary<string, int> m_StandardNameDictionary = new Dictionary<string, int>();
        private static Dictionary<string, REG_TZI> m_RegistryTimeZones = null;
        private static readonly object m_LockZones = new object();
        private TZI m_TZI;
        
        /// <summary>
        /// The zone's Id.
        /// </summary>
        private string m_Id;
        public string Id
        {
            get { return m_Id; }
        }
        
        /// <summary>
        /// The zone's name.
        /// </summary>
        //private string m_Name;
        //public string Name
        //{
        //    get { return m_Name; }
        //}

        /// <summary>
        /// The zone's display name, e.g. '(GMT) Greenwich Mean Time : Dublin, Edinburgh, Lisbon, London'.
        /// </summary>
        private string m_DisplayName;
        public string DisplayName
        {
            get { return m_DisplayName; }
            set 
            { 
                if (!string.IsNullOrEmpty(value))
                    m_DisplayName = value; 
            }
        }

        /// <summary>
        /// The zone's index. No obvious pattern.
        /// </summary>
        private int m_Index;
        public int Index
        {
            get { return m_Index; }
        }

        /// <summary>
        /// The zone's name during 'standard' time (not daylight savings).
        /// </summary>
        private string m_StandardName;
        public string StandardName
        {
            get { return m_StandardName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    m_StandardName = value;
            }
        }

        /// <summary>
        /// The zone's name during daylight savings time.
        /// </summary>
        private string m_DaylightName;
        public string DaylightName
        {
            get { return m_DaylightName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    m_DaylightName = value;
            }
        }

        private string m_ShortStandardName;
        public string ShortStandardName
        {
            get { return m_ShortStandardName; }
            set { m_ShortStandardName = value; }
        }

        private string m_ShortDaylightName;
        public string ShortDaylightName
        {
            get { return m_ShortDaylightName; }
            set { m_ShortDaylightName = value; }
        }
        #endregion

        #region Constructor
        private TimeZoneInfo()
        {
        }

        private TimeZoneInfo(XmlNode tzNode)
        {
            m_TZI = new TZI(); 
            //<Tz dn="(GMT+04:30) Kabul" sn="Afghanistan Standard Time" dln="Afghanistan Daylight Time" bias="-270" sbias="0" dlbias="-60" ddate="0,0,0,0,0,0,0,0" sdate="0,0,0,0,0,0,0,0" idx="1" />
            foreach (XmlAttribute attrib in tzNode.Attributes)
            {
                if (attrib.Name.Equals("id"))
                {
                    m_Id = attrib.Value;
                }    
                if (attrib.Name.Equals("dn"))
                {
                    m_DisplayName = attrib.Value; 
                }
                else if (attrib.Name.Equals("sn"))
                {
                    m_StandardName = attrib.Value; 
                }
                else if (attrib.Name.Equals("dln"))
                {
                    m_DaylightName = attrib.Value; 
                }
                else if (attrib.Name.Equals("bias"))
                {
                    m_TZI.bias = int.Parse(attrib.Value); 
                }
                else if (attrib.Name.Equals("sbias"))
                {
                    m_TZI.standardBias = int.Parse(attrib.Value); 
                }
                else if (attrib.Name.Equals("dlbias"))
                {
                    m_TZI.daylightBias = int.Parse(attrib.Value); 
                }
                else if (attrib.Name.Equals("ddate"))
                {
                    m_TZI.daylightDate = new SYSTEMTIME(attrib.Value); 
                }
                else if (attrib.Name.Equals("sdate"))
                {
                    m_TZI.standardDate = new SYSTEMTIME(attrib.Value); 
                }
                else if (attrib.Name.Equals("idx"))
                {
                    m_Index = int.Parse(attrib.Value); 
                }
                else if (attrib.Name.Equals("sshort"))
                {
                    m_ShortStandardName = attrib.Value; 
                }
                else if (attrib.Name.Equals("dlshort"))
                {
                    m_ShortDaylightName = attrib.Value; 
                }
                if (string.IsNullOrEmpty(m_ShortDaylightName))
                {
                    m_ShortDaylightName = m_DaylightName; 
                }
                if (string.IsNullOrEmpty(m_ShortStandardName))
                {
                    m_ShortStandardName = m_StandardName; 
                }
            }
         //   Log.Debug(this.ToString()); 
        }
        #endregion

        #region Method(s)
        /// <summary>
        /// Get the currently selected time zone
        /// </summary>
        public static TimeZoneInfo CurrentTimeZone
        {
            get
            {
                // The currently selected time zone information can
                // be retrieved using the Win32 GetTimeZoneInformation call,
                // but it only gives us names, offsets and dates - crucially,
                // not the Index.

                TIME_ZONE_INFORMATION tziNative;
                TimeZoneInfo[] zones = EnumZones();

                NativeMethods.GetTimeZoneInformation(out tziNative);

                // Getting the identity is tricky; the best we can do
                // is a match on the properties.

                // If the OS 'Automatically adjust clock for daylight saving changes' checkbox
                // is unchecked, the structure returned by GetTimeZoneInformation has
                // the DaylightBias and DaylightName members set the same as the corresponding
                // Standard members. Therefore we check against both values in case this has
                // been done.

                for (int idx = 0; idx < zones.Length; ++idx)
                {
                    if (zones[idx].m_TZI.bias == tziNative.Bias &&
                         zones[idx].m_TZI.standardBias == tziNative.StandardBias &&
                         zones[idx].m_StandardName == tziNative.StandardName &&
                         (zones[idx].m_TZI.daylightBias == tziNative.DaylightBias ||
                           zones[idx].m_TZI.standardBias == tziNative.DaylightBias) &&
                         (zones[idx].m_DaylightName == tziNative.DaylightName ||
                           zones[idx].m_StandardName == tziNative.DaylightName))
                    {
                        return zones[idx];
                    }
                }
          /*      Log.WarnFormat("Timezone was not found, standard name: {0}, bias: {1}, standard bias: {2}, daylight name: {3}, daylight bias: {4}, returning UTC", 
                    tziNative.StandardName, 
                    tziNative.Bias, 
                    tziNative.StandardBias, 
                    tziNative.DaylightName, 
                    tziNative.DaylightBias);*/
                return zones[29];
            }
        }

        /// <summary>
        /// Get a TimeZoneInformation for a supplied index.
        /// </summary>
        /// <param name="index">The time zone to find.</param>
        /// <returns>The corresponding TimeZoneInformation.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the index is not found.</exception>
        public static TimeZoneInfo FromIndex(int index)
        {
            //return m_IndexDictionary[index];
            TimeZoneInfo[] zones = EnumZones();
            for (int i = 0; i < zones.Length; ++i)
            {
                if (zones[i].Index == index)
                    return zones[i];
            }
            throw new ArgumentOutOfRangeException("index", index, "Unknown time zone index");
        }

        public static TimeZoneInfo FromStandardName(string standardName)
        {
            TimeZoneInfo[] zones = EnumZones();
            foreach (TimeZoneInfo z in zones)
            {
                if (z.StandardName.Equals(standardName))
                {
                    return z;
                }
            }
            throw new ArgumentOutOfRangeException("standardName", standardName, "Unknown timezone standard name");
        }

        public string GetShortDisplayName(DateTime time)
        {
            if (IsDaylightSavingTime(time))
                return m_ShortDaylightName;
            else
                return m_ShortStandardName;
        }

        public bool IsDaylightSavingTime(DateTime time)
        {
            DateTime gmt = this.ToUniversalTime(time);
            TimeSpan ts = time.Subtract(gmt);

            if (System.Math.Abs(ts.TotalMinutes) == System.Math.Abs(this.Bias))
                return false;
            else
                return true;
        }

        public static string FromUniversalTimeToString(int index, DateTime utc)
        {
            TimeZoneInfo tzi = FromIndex(index);
            DateTime time = tzi.FromUniversalTime(utc);
            return string.Format("{0} {1}", time.ToString("g"), tzi.GetShortDisplayName(time));
        }

        /// <summary>
        /// Enumerate the available time zones
        /// </summary>
        /// <returns>The list of known time zones</returns>
        public static TimeZoneInfo[] EnumZones(int[] zoneIndices)
        {
            if (zoneIndices == null)
                return EnumZones();

            if (m_RegistryTimeZones == null)
            {
                m_RegistryTimeZones = new Dictionary<string, REG_TZI>();
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones"))
                {
                    string[] zoneNames = key.GetSubKeyNames();

                    foreach (string zoneName in zoneNames)
                    {
                        using (RegistryKey subKey = key.OpenSubKey(zoneName))
                        {
                            REG_TZI regTzi = new REG_TZI();
                            regTzi.KeyName = zoneName;
                            regTzi.DisplayName = (string)subKey.GetValue("Display");
                            regTzi.StandardName = (string)subKey.GetValue("Std");
                            regTzi.DaylightName = (string)subKey.GetValue("Dlt");

                            m_RegistryTimeZones.Add(regTzi.KeyName, regTzi);
                        }
                    }
                }
            }

            bool activeOnly = (zoneIndices.Length > 0);
            if ((activeOnly && m_TimeZoneInfosActive == null) || (!activeOnly && m_TimeZoneInfosAll == null))
            {
                lock (m_LockZones)
                {
                    if ((activeOnly && m_TimeZoneInfosActive == null) || (!activeOnly && m_TimeZoneInfosAll == null))
                    {
                        ArrayList zones = new ArrayList();
                        string tzsXml = global::EC.Framework.TimeZone.Properties.Resources.Tzs;
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(tzsXml);
                        XmlNodeList tzNodes = xml.SelectNodes("/Tzs/Tz");
                        foreach (XmlNode tzNode in tzNodes)
                        {
                            TimeZoneInfo timeZoneInfo = new TimeZoneInfo(tzNode);
                            if (string.IsNullOrEmpty(timeZoneInfo.Id))
                                timeZoneInfo.m_Id = timeZoneInfo.StandardName;
                            if (activeOnly && System.Array.IndexOf(zoneIndices, timeZoneInfo.Index) < 0)
                                continue;

                            if (m_RegistryTimeZones.ContainsKey(timeZoneInfo.Id))
                            {
                                REG_TZI regTzi = m_RegistryTimeZones[timeZoneInfo.Id];
                                timeZoneInfo.DisplayName = regTzi.DisplayName;
                                timeZoneInfo.StandardName = regTzi.StandardName;
                                timeZoneInfo.DaylightName = regTzi.DaylightName;
                            }
                            zones.Add(timeZoneInfo);
                         //   if (m_IndexDictionary.ContainsKey(timeZoneInfo.Index))
                         ///       Log.WarnFormat("Timezone index {0} ({1}) was already added to index dictionary, being replaced with {2}",
                          ///          timeZoneInfo.Index, m_IndexDictionary[timeZoneInfo.Index].DisplayName, timeZoneInfo.DisplayName); 
                            m_IndexDictionary[timeZoneInfo.Index] = timeZoneInfo;
                        }

                        zones.Sort(new TimeZoneInfo());
                        if (activeOnly)
                        {
                            m_TimeZoneInfosActive = new TimeZoneInfo[zones.Count];
                            zones.CopyTo(m_TimeZoneInfosActive);
                        }
                        else
                        {
                            m_TimeZoneInfosAll = new TimeZoneInfo[zones.Count];
                            zones.CopyTo(m_TimeZoneInfosAll);
                        }
                    }
                }
            }

            return activeOnly ? m_TimeZoneInfosActive : m_TimeZoneInfosAll;
        }

        public static TimeZoneInfo[] EnumZones()
        {
            return EnumZones(new int[0]);
        }
        
        public override bool Equals(object obj)
        {
            if (obj is TimeZoneInfo)
            {
                TimeZoneInfo other = obj as TimeZoneInfo;
                return other.Index == Index;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Index;
        }

        public override string ToString()
        {
            //<Tz dn="(GMT+04:30) Kabul" sn="Afghanistan Standard Time" dln="Afghanistan Daylight Time" bias="-270" sbias="0" dlbias="-60" ddate="0,0,0,0,0,0,0,0" sdate="0,0,0,0,0,0,0,0" idx="1" />
            #if DEBUG
            return string.Format("dn={0}, sn={1}, dln={2}, bias={3}, sbias={4}, dlbias={5}, ddate={6}, sdate={7}, idx={8}",
                m_DisplayName, m_StandardName, m_DaylightName, m_TZI.bias, m_TZI.standardBias, m_TZI.daylightBias, m_TZI.daylightDate, m_TZI.standardDate, m_Index); 
            #else 
                return m_DisplayName;
            #endif 
        }

        /// <summary>
        /// Initialise the m_tzi member.
        /// </summary>
        /// <param name="info">The Tzi data from the registry.</param>
        private void InitTzi(byte[] info)
        {
            if (info.Length != Marshal.SizeOf(m_TZI))
                throw new ArgumentException("Information size is incorrect", "info");

            // Could have sworn there's a Marshal operation to pack bytes into
            // a structure, but I can't see it. Do it manually.

            GCHandle h = GCHandle.Alloc(info, GCHandleType.Pinned);

            try
            {
                m_TZI = (TZI)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(TZI));
            }
            finally
            {
                h.Free();
            }
        }

        /// <summary>
        /// The offset from UTC. Local = UTC + Bias.
        /// </summary>
        public int Bias
        {
            // Biases in the registry are defined as UTC = local + bias
            // We return as Local = UTC + bias
            get { return -m_TZI.bias; }
        }

        /// <summary>
        /// The offset from UTC during standard time.
        /// </summary>
        public int StandardBias
        {
            get { return -(m_TZI.bias + m_TZI.standardBias); }
        }

        /// <summary>
        /// The offset from UTC during daylight time.
        /// </summary>
        public int DaylightBias
        {
            get
            {
                if (m_TZI.daylightDate.wMonth == 0 && m_TZI.standardDate.wMonth == 0)
                {
                    return -(m_TZI.bias);
                }
                else
                {
                    return -(m_TZI.bias + m_TZI.daylightBias);
                }
            }
        }

        private TIME_ZONE_INFORMATION TziNative()
        {
            TIME_ZONE_INFORMATION tziNative = new TIME_ZONE_INFORMATION();

            tziNative.Bias = m_TZI.bias;
            tziNative.StandardDate = m_TZI.standardDate;
            tziNative.StandardBias = m_TZI.standardBias;
            tziNative.DaylightDate = m_TZI.daylightDate;
            tziNative.DaylightBias = m_TZI.daylightBias;

            return tziNative;
        }

        /// <summary>
        /// Convert a time interpreted as UTC to a time in this time zone.
        /// </summary>
        /// <param name="utc">The UTC time to convert.</param>
        /// <returns>The corresponding local time in this zone.</returns>
        public DateTime FromUniversalTime(DateTime utc)
        {
            // Convert to SYSTEMTIME
            SYSTEMTIME stUTC = DateTimeToSystemTime(utc);

            // Set up the TIME_ZONE_INFORMATION

            TIME_ZONE_INFORMATION tziNative = TziNative();

            SYSTEMTIME stLocal;

            NativeMethods.SystemTimeToTzSpecificLocalTime(ref tziNative, ref stUTC, out stLocal);

            // Convert back to DateTime
            return SystemTimeToDateTime(ref stLocal);
        }

        public static DateTime? FromUniversalTime(int? index, DateTime? utc)
        {
            if (utc.HasValue && index.HasValue)
                return FromUniversalTime((int)index, (DateTime)utc);
            else
                return utc;
        }

        /// <summary>
        /// Convert a time from UTC to the time zone with the supplied index.
        /// </summary>
        /// <param name="index">The time zone index.</param>
        /// <param name="utc">The time to convert.</param>
        /// <returns>The converted time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not found.</exception>
        public static DateTime FromUniversalTime(int index, DateTime utc)
        {
            TimeZoneInfo tzi = FromIndex(index);

            return tzi.FromUniversalTime(utc);
        }

		public static string FormatToString(int? index, DateTime? time)
        {
            if (time.HasValue)
            {
                TimeZoneInfo tzi = FromIndex((int)index);
                return string.Format("{0} {1}", ((DateTime)time).ToString(), tzi.GetShortDisplayName((DateTime)time));
            }
            else
                return string.Empty;
        }	

        /// <summary>
        /// Convert a time interpreted as a local time in this zone to the equivalent UTC.
        /// Note that there may be different possible interpretations at the daylight
        /// time boundaries.
        /// </summary>
        /// <param name="local">The local time to convert.</param>
        /// <returns>The corresponding UTC.</returns>
        /// <exception cref="NotSupportedException">Thrown if the method failed due to missing platform support.</exception>
        public DateTime ToUniversalTime(DateTime local)
        {
            SYSTEMTIME stLocal = DateTimeToSystemTime(local);

            TIME_ZONE_INFORMATION tziNative = TziNative();

            SYSTEMTIME stUTC;

            try
            {
                NativeMethods.TzSpecificLocalTimeToSystemTime(ref tziNative, ref stLocal, out stUTC);

                return SystemTimeToDateTime(ref stUTC);
            }
            catch (EntryPointNotFoundException e)
            {
                throw new NotSupportedException("This method is not supported on this operating system", e);
            }
        }

        /// <summary>
        /// Convert a time from the time zone with the supplied index to UTC.
        /// </summary>
        /// <param name="index">The time zone index.</param>
        /// <param name="utc">The time to convert.</param>
        /// <returns>The converted time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not found.</exception>
        /// <exception cref="NotSupportedException">Thrown if the method failed due to missing platform support.</exception>
        public static DateTime ToUniversalTime(int index, DateTime local)
        {
            TimeZoneInfo tzi = FromIndex(index);

            return tzi.ToUniversalTime(local);
        }

        private static SYSTEMTIME DateTimeToSystemTime(DateTime dt)
        {
            SYSTEMTIME st;
            System.Runtime.InteropServices.ComTypes.FILETIME ft = new System.Runtime.InteropServices.ComTypes.FILETIME();

            ft.dwHighDateTime = (int)(dt.Ticks >> 32);
            ft.dwLowDateTime = (int)(dt.Ticks & 0xFFFFFFFFL);

            NativeMethods.FileTimeToSystemTime(ref ft, out st);

            return st;
        }

        private static DateTime SystemTimeToDateTime(ref SYSTEMTIME st)
        {
            System.Runtime.InteropServices.ComTypes.FILETIME ft = new System.Runtime.InteropServices.ComTypes.FILETIME();

            NativeMethods.SystemTimeToFileTime(ref st, out ft);

            DateTime dt = new DateTime((((long)ft.dwHighDateTime) << 32) | (uint)ft.dwLowDateTime);

            return dt;
        }
        #endregion

        #region Struct(s)
        /// <summary>
        /// The standard Windows SYSTEMTIME structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public UInt16 wYear;
            public UInt16 wMonth;
            public UInt16 wDayOfWeek;
            public UInt16 wDay;
            public UInt16 wHour;
            public UInt16 wMinute;
            public UInt16 wSecond;
            public UInt16 wMilliseconds;

            public SYSTEMTIME(string systemTime)
            {
                string[] vals = systemTime.Split(new string[] { "," }, StringSplitOptions.None);
                this.wYear = UInt16.Parse(vals[0]);
                this.wMonth = UInt16.Parse(vals[1]);
                this.wDayOfWeek = UInt16.Parse(vals[2]);
                this.wDay = UInt16.Parse(vals[3]);
                this.wHour = UInt16.Parse(vals[4]);
                this.wMinute = UInt16.Parse(vals[5]);
                this.wSecond = UInt16.Parse(vals[6]);
                this.wMilliseconds = UInt16.Parse(vals[7]);
            }

            public override string ToString()
            {
                return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                    wYear, wMonth, wDayOfWeek, wDay, wHour, wMinute, wSecond, wMilliseconds);
            }
        }

        /// <summary>
        /// The layout of the Tzi value in the registry.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TZI
        {
            public int bias;
            public int standardBias;
            public int daylightBias;
            public SYSTEMTIME standardDate;
            public SYSTEMTIME daylightDate;
        }

        /// <summary>
        /// The standard Win32 TIME_ZONE_INFORMATION structure.
        /// Thanks to www.pinvoke.net.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct TIME_ZONE_INFORMATION
        {
            [MarshalAs(UnmanagedType.I4)]
            public Int32 Bias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string StandardName;
            public SYSTEMTIME StandardDate;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 StandardBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DaylightName;
            public SYSTEMTIME DaylightDate;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 DaylightBias;
        }

        /// <summary>
        /// A container for P/Invoke declarations.
        /// </summary>
        private struct NativeMethods
        {
            private const string KERNEL32 = "kernel32.dll";

            [DllImport(KERNEL32)]
            public static extern uint GetTimeZoneInformation(out TIME_ZONE_INFORMATION
                lpTimeZoneInformation);

            [DllImport(KERNEL32)]
            public static extern bool SystemTimeToTzSpecificLocalTime(
                [In] ref TIME_ZONE_INFORMATION lpTimeZone,
                [In] ref SYSTEMTIME lpUniversalTime,
                out SYSTEMTIME lpLocalTime);

            [DllImport(KERNEL32)]
            public static extern bool SystemTimeToFileTime(
                [In] ref SYSTEMTIME lpSystemTime,
               out System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime);


            [DllImport(KERNEL32)]
            public static extern bool FileTimeToSystemTime(
                [In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime,
                out SYSTEMTIME lpSystemTime);

            /// <summary>
            /// Convert a local time to UTC, using the supplied time zone information.
            /// Windows XP and Server 2003 and later only.
            /// </summary>
            /// <param name="lpTimeZone">The time zone to use.</param>
            /// <param name="lpLocalTime">The local time to convert.</param>
            /// <param name="lpUniversalTime">The resultant time in UTC.</param>
            /// <returns>true if successful, false otherwise.</returns>
            [DllImport(KERNEL32)]
            public static extern bool TzSpecificLocalTimeToSystemTime(
                [In] ref TIME_ZONE_INFORMATION lpTimeZone,
                [In] ref SYSTEMTIME lpLocalTime,
                out SYSTEMTIME lpUniversalTime);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct REG_TZI
        {
            public string KeyName;
            public string DisplayName;
            public string StandardName;
            public string DaylightName;
        }
        #endregion

        #region IComparer Members
        public int Compare(object x, object y)
        {
            if ((x as TimeZoneInfo).Bias < (y as TimeZoneInfo).Bias)
                return -1;
            else
                return 1;
        }
        #endregion
    }
}
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Slot.Model.Utility
{
    public static partial class Extension
    {
        // from bool to int
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        // from string to decimal
        public static decimal ToDecimal(this string s)
        {
            decimal result;
            return decimal.TryParse(s, out result) ? result : 0m;
        }

        // from string to int
        public static int ToInt(this string s)
        {
            int result;
            return int.TryParse(s, out result) ? result : 0;
        }

        // from string to long
        public static long ToLong(this string s)
        {
            long result;
            return long.TryParse(s, out result) ? result : 0;
        }

        // from string to bool
        public static bool ToBool(this string s)
        {
            bool result;
            return bool.TryParse(s, out result) ? result : false;
        }

        // from decimal to string
        public static string ToCustomString(this decimal d)
        {
            return String.Format("{0:0.00}", d);
        }

        #region DateTime convertion

        // from unix timestamp to datetime
        public static DateTime ToDateTime(this int unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }

        // from datetime to unix timestamp
        public static int ToUnixTimeStamp(this DateTime dt)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = dt - origin;
            return (int)Math.Floor(diff.TotalSeconds);
        }

        // from datetime to string date
        public static string ToStrDate(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy");
        }

        // from datetime to string date
        public static string ToStrDate(this DateTime dt, string format)
        {
            return dt.ToString(format);
        }

        // from datetime to string date, with start time of the day
        public static string ToStrDateST(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy 00:00");
        }

        // from datetime to string date, with end time of the day
        public static string ToStrDateET(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy 23:59");
        }

        // from datetime to string datetime
        public static string ToStrDateTime(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy H:mm:ss");
        }

        // from datetime to string datetime with hour and minutes only
        public static string ToStrDateHourMinute(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy HH:mm");
        }

        public static string ToStrDateHourMinuteSecond(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy HH:mm:ss");
        }

        // from datetime to string datetime sql
        public static string ToStrSQlDateTime(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        //round up to the nearest cent
        public static decimal RoundUp(this decimal d)
        {
            const decimal r = 0.005m;
            return Math.Round(d + r, 2);
        }

        // from string to datetime
        public static DateTime ToDateTime(this string dt, string format = "dd-MM-yyyy HH:mm")
        {
            try
            {
                return DateTime.ParseExact(dt, format, null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        // from datetime to offsettime
        public static DateTime ToOffSetTime(this DateTime dt, string offset, bool reverse = false)
        {
            if (reverse)
            {
                if (offset.Substring(0, 1) == "-") offset = offset.Replace("-", "+");
                if (offset.Substring(0, 1) == "+") offset = offset.Replace("+", "-");
            }

            double[] d = offset.Split(':').Select(double.Parse).ToArray();
            return dt.AddHours(d[0]).AddMinutes(d[1]);
        }

        public static DateTime GetFirstDayInWeek(int Year, int Week)
        {
            GregorianCalendar Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);

            DateTime FirstDay = new DateTime(Year, 1, 1);
            DateTime CurrentDay = Calendar.AddWeeks(FirstDay, Week);

            return Calendar.AddDays(CurrentDay, -(int)Calendar.GetDayOfWeek(CurrentDay) + 1);
        }

        public static DateTime GetLastDayInWeek(int Year, int Week)
        {
            GregorianCalendar Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);

            DateTime FirstDay = new DateTime(Year, 1, 1);
            DateTime CurrentDay = Calendar.AddWeeks(FirstDay, Week);

            return Calendar.AddDays(CurrentDay, 7 - (int)Calendar.GetDayOfWeek(CurrentDay) + 1);
        }

        public static DateTime MinDateTime(DateTime datetime1, DateTime datetime2)
        {
            return (datetime1 < datetime2) ? datetime1 : datetime2;
        }

        #endregion

        #region TimeSpan convertion

        public static string ToUtcOffsetString(this TimeSpan timeSpan)
        {
            return ((timeSpan < TimeSpan.Zero) ? "-" : "+") + timeSpan.ToString(@"hh\:mm");
        }

        #endregion

        #region Bytes and Object convertion

        // from bytes to object
        public static T ToObject<T>(this byte[] byteArray)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray, 0, byteArray.Length);
            ms.Seek(0, SeekOrigin.Begin);

            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(ms);

            return (T)obj;
        }

        // from object to bytes
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null) return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        #endregion

        #region Enumerable convertion

        public static string ToCommaDelimitedString<T>(this IEnumerable<T> list)
        {
            return string.Join(",", list);
        }

        public static string ToCommaString(this IList<Dice> list)
        {
            return string.Join(",", list.Select(d => d.Side));
        }

        #endregion

        #region Enum convertion

        public static PlatformType ToPlatformId(PlatformType pf, string res)
        {
            if (res == null) return pf;
            return res.ToUpper() == "LD"? 10 + pf : pf;
        }

        public static PlatformType ToPlatformType(PlatformType pf)
        {
            return (PlatformType)((int)pf % 10);
        }
     
        #endregion
    }
}

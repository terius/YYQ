using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure.Helpers
{
    public class TimeHelper
    {
      

        public static string GetDayOfWeek(int day)
        {
            switch (day)
            {
                case 0:
                case 7:
                    return "周日";
                case 1:
                    return "周一";
                case 2:
                    return "周二";
                case 3:
                    return "周三";
                case 4:
                    return "周四";
                case 5:
                    return "周五";
                case 6:
                    return "周六";
                default:
                    break;
            }
            return "错误星期";
        }
        public static string GetDayOfWeek(DateTime date)
        {
            int day = (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
            return GetDayOfWeek(day);
        }



        public static string DateTimeOffSetToDateTimeStr(DateTimeOffset? offtime)
        {
            if (offtime == null)
            {
                return "";
            }
            DateTime dt = offtime.Value.DateTime;
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime ConvertUnixTimeToDateTime(string unixTime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return sTime.AddSeconds(Convert.ToInt64(unixTime));
        }


        /// <summary>  
        /// 取得某月的第一天  
        /// </summary>  
        /// <param name="datetime">要取得月份第一天的时间</param>  
        /// <returns></returns>  
        public static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }


        /// <summary>  
        /// 取得某月的最后一天  
        /// </summary>  
        /// <param name="datetime">要取得月份最后一天的时间</param>  
        /// <returns></returns>  
        public static DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }


        /// <summary>  
        /// 取得上个月第一天  
        /// </summary>  
        /// <param name="datetime">要取得上个月第一天的当前时间</param>  
        /// <returns></returns>  
        public static DateTime FirstDayOfPreviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(-1);
        }


        /// <summary>  
        /// 取得上个月的最后一天  
        /// </summary>  
        /// <param name="datetime">要取得上个月最后一天的当前时间</param>  
        /// <returns></returns>  
        public static DateTime LastDayOfPrdviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddDays(-1);
        }

    }
}

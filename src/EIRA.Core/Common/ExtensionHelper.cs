using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EIRA.Common
{
    public static class ExtensionHelper
    {
        /// <summary>  
        /// 返回 EIRA yyyy-MM-dd HH:mm时间格式  
        /// </summary>  
        /// <returns></returns>  
        public static string ToEIRATime(this DateTime t)
        {
            return t.ToString("dd-MM-yyyy HH:mm");
        }

        /// <summary>
        /// 自動生成編號
        /// </summary>
        /// <param name="dt">時間</param>
        /// <returns></returns>
        public static string GetCode(this DateTime dt)
        {
            return Convert.ToString(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")), 16);
        }

        /// <summary>
        /// 四捨五入
        /// </summary>
        /// <param name="parValue">需要處理的數據</param>
        /// <param name="parDecimals">保留小數位數</param>
        /// <returns></returns>
        public static decimal Round(decimal parValue, int parDecimals)
        {
            return Math.Round(parValue, parDecimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 判斷是否是數字類型
        /// </summary>
        /// <param name="parValue">需要處理的數據</param>
        /// <returns></returns>
        public static bool IsNumberic(string parValue)
        {
            return Regex.IsMatch(parValue, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 製作分頁
        /// </summary>
        /// <param name="parCollection">需要做分頁的集合</param>
        /// <param name="SkipCount">跳過條數</param>
        /// <param name="parPerPageRecord">當頁筆數</param>
        /// <returns></returns>
        public static List<T> GetPaging<T>(List<T> parCollection, int SkipCount, int parPerPageRecord)
        {
            return parCollection.Skip(SkipCount).Take(parPerPageRecord).ToList();
        }
    }
}

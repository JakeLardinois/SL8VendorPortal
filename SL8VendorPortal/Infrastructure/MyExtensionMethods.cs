using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using Newtonsoft.Json;
using SL8VendorPortal.Models;


namespace SL8VendorPortal.Infrastructure
{
    public static class MyExtensionMethods
    {
        public static string AddSingleQuotes(this string source)
        {
            string[] strArray;
            StringBuilder objStrBldr;


            strArray = source.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length > 0)
            {
                objStrBldr = new StringBuilder();

                foreach (string strTemp in strArray)
                {
                    objStrBldr.Append("'" + strTemp.Trim() + "',");
                }

                return objStrBldr.Remove(objStrBldr.Length - 1, 1).ToString();
            }
            else
                return string.Empty;

            
        }

        public static string AddSingleQuotesAndPadLeft(this string source, int intWidth)
        {
            string[] strArray;
            StringBuilder objStrBldr;


            strArray = source.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length > 0)
            {
                objStrBldr = new StringBuilder();

                foreach (string strTemp in strArray)
                {
                    objStrBldr.Append("'" + strTemp.Trim().PadLeft(intWidth, ' ') + "',");
                }

                return objStrBldr.Remove(objStrBldr.Length - 1, 1).ToString();
            }
            else
                return string.Empty;


        }

        //Splits a comma separated string into a list
        public static IEnumerable<string> SplitNTrim(this string source)
        {
            string[] strArray;


            strArray = source.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length > 0)
            {
                return source.Split(',').Select(s => s.Trim());
            }
            else
                return new List<string>();


        }

        //my extension method that converts the JSON time format of /Date(1376625062603)/ to a DateTime object
        public static DateTime GetDateTimeFromJSON(this string source)
        {
            double dblTemp;

            return TimeZoneInfo.ConvertTimeFromUtc(
                new DateTime(1970, 1, 1)
                .AddMilliseconds(
                double.TryParse(
                source.Replace("/Date(", "").Replace(")/", ""),
                out dblTemp) ? dblTemp : 0),
                TimeZoneInfo.Local);
        }

        //I still didn't get this to work..  it needs to output /Date(1376625062603)/ from a DateTime
        public static string GetJSONFromDateTime(this DateTime source)
        {
            //var obj = new JsonResult { Data = new {source}};
            //return obj.Data.ToString();

            //return Controller.Json(new { source });

            return JsonConvert.SerializeObject(new { source });
        }

        /*This will return true if the user is in any of the roles specified in the comma separated list of roles*/
        public static bool IsInRoles(this System.Security.Principal.IPrincipal source, string roles)
        {
            var RoleList = roles.SplitNTrim();

            foreach (string objString in RoleList)
                if (source.IsInRole(objString))
                    return true;

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;


namespace SL8VendorPortal.Infrastructure
{
    public static class MyExtensionMethods
    {
        public static string AddSingleQuotes(this string source)
        {
            string[] strArray = source.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder objStrBldr;


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
    }
}
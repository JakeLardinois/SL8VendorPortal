using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;


namespace SL8VendorPortal.Models
{
    public static class QueryDefinitions
    {
        static System.Resources.ResourceManager objResourceManager = new System.Resources.ResourceManager("SL8VendorPortal.Models.QueryDefs", System.Reflection.Assembly.GetExecutingAssembly());
        static StringBuilder strSQL;


        public static string GetQuery(string strQueryName)
        {
            strSQL = new StringBuilder();
            strSQL.Append(objResourceManager.GetString(strQueryName));

            return strSQL.ToString();
        }

        public static string GetQuery(string strQueryName, string[] strParams)
        {
            strSQL = new StringBuilder();
            strSQL.Append(objResourceManager.GetString(strQueryName));
            //strSQL.Append(QueryDefs.DeleteOldestItemUnitWeightHistory);

            //for (int intCounter = 0; intCounter < strParams.Length; intCounter++)
            for (int intCounter = strParams.Length - 1; intCounter > -1; intCounter--)
            {
                string strTemp = "~p" + intCounter;
                strSQL.Replace(strTemp, strParams[intCounter]);
            }

            return strSQL.ToString();
        } 
    }

    public static class SharedVariables
    {
        public static DateTime MINDATE = new DateTime(1900, 1, 1);
        public static DateTime MAXDATE = new DateTime(2999, 1, 1);
    }
}
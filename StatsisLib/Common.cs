using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StatsisLib
{
    public class Common
    {
        public static DataTable ListToDataTable<T>(List<T> infoList)
        {
            DataTable dt = new DataTable();
            var propArray = typeof(T).GetProperties();
            foreach (var propItem in propArray)
            {
                dt.Columns.Add(propItem.Name);
            }
            foreach (var info in infoList)
            {
                DataRow dr = dt.NewRow();
                foreach (var propItem in propArray)
                {
                    dr[propItem.Name] = propItem.GetValue(info,null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static DataTable ListToDataTable<T>(List<T> infoList, List<string> cols, bool needIndex = true)
        {
            DataTable dt = new DataTable();
            string indexName = "名次";
            if (needIndex)
            {
                dt.Columns.Add(indexName);
            }
            List<PropertyInfo> pInfos = new List<PropertyInfo>();
            foreach (var colItem in cols)
            {
                dt.Columns.Add(colItem);
                var propInfo = typeof(T).GetProperty(colItem);
                if (propInfo != null)
                {
                    pInfos.Add(propInfo);
                }
            }
            int index = 1;

            foreach (var info in infoList)
            {
                DataRow dr = dt.NewRow();
                if (needIndex)
                {
                    dr[indexName] = index++;
                }
                foreach (var propItem in pInfos)
                {
                    object o = propItem.GetValue(info,null);
                    if (propItem.PropertyType.Name == "Decimal")
                    {
                        o = Decimal.Parse(o.ToString()).ToString("f2");
                    }

                    dr[propItem.Name] =o;
                }
                dt.Rows.Add(dr);
            }
           
            return dt;
        }

        public static List<T> DTToList<T>(DataTable dt)
        {
            if (dt == null)
            {
                return new List<T>();
            }
            List<T> list = new List<T>();
            Type t = typeof(T);
            var propInfos = t.GetProperties();
            List<PropertyInfo> exsitPropInfos = new List<PropertyInfo>();
            foreach (var item in propInfos)
            {
                foreach (DataColumn colItem in dt.Columns)
                {
                    if (colItem.ColumnName.ToLower() == item.Name.ToLower())
                    {
                        exsitPropInfos.Add(item);
                    }
                }
            }
            foreach (DataRow item in dt.Rows)
            {
                T info = Activator.CreateInstance<T>();
                foreach (var propItem in exsitPropInfos)
                {
                    object value = item[propItem.Name];
                    if (value == null)
                    {
                        continue;
                    }
                    var propInfo = t.GetProperty(propItem.Name);
                    switch (propInfo.PropertyType.Name)
                    {
                        case "String":
                            value = value.ToString();
                            break;
                        case "Int32":
                            int intValue = 0;
                            int.TryParse(value.ToString(), out intValue);
                            value = intValue;
                            break;
                        case "Decimal":
                            decimal dValue = 0;
                            decimal.TryParse(value.ToString(), out dValue);
                            value = dValue;
                            break;
                        default:
                            break;
                    }
                    propInfo.SetValue(info, value == null ? "" : value, null);
                }
                list.Add(info);
            }
            return list;
        }

        public static string GetConfig(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}

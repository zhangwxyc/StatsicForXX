using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
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
                    dr[propItem.Name] = propItem.GetValue(info);
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
    }
}

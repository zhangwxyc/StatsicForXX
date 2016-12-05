using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StatsisLib
{
    public static class SerializeHelper
    {
        public static void Serialize<T>(this T info,string path)
            where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(fs, info);
            }
        }
        public static T Deserialize<T>(this T info, string path)
            where T : class,new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                info = serializer.Deserialize(fs) as T;
            }
            return info;
        }
    }
}

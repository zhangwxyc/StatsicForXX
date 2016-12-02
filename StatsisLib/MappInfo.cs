using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StatsisLib
{
    [Serializable]
    public class MappInfo
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public static void Serialize(string path, List<MappInfo> infos)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<MappInfo>));
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(fs, infos);
            }
        }
        public static List<MappInfo> Deserialize(string path)
        {
            List<MappInfo> result = new List<MappInfo>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<MappInfo>));
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                result = serializer.Deserialize(fs) as List<MappInfo>;
            }
            return result;
        }
    }
}

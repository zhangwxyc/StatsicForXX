using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StatsisLib
{
    [XmlRoot("Root")]
    [Serializable]
    public class NestDirectory
    {
        public string Name { get; set; }

        public string NickName { get; set; }

        [XmlElement("Index")]
        public int OrderIndex { get; set; }
        [XmlElement("Children")]
        public List<NestDirectory> Children { get; set; }

        public NestDirectory()
        {
            Children = new List<NestDirectory>();
        }
        public void Serialize(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NestDirectory));
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(fs, this);
            }
        }
        public static NestDirectory Deserialize(string path)
        {
            NestDirectory result = null;
            XmlSerializer serializer = new XmlSerializer(typeof(NestDirectory));
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                result=serializer.Deserialize(fs) as NestDirectory;
            }
            return result;
        }
    }
}

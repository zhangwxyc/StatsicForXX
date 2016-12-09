using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder cc = new StringBuilder();
            var infos = StatsisLib.MappInfo.Deserialize("E:\\m.xml");
           infos.ForEach(x =>
               {
                   cc.AppendFormat("{0}|{1}\r\n",x.Key,x.Value);
               });
           File.WriteAllText("E:\\m.txt", cc.ToString());
        }
    }
}

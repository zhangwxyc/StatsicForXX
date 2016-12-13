using StatsisLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Text;
using DataService;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            DataService.QHXEntities DBContext = new DataService.QHXEntities();
            var NDirectory = StatsisLib.NestDirectory.Deserialize("C:\\g1.xml");
            foreach (var item in NDirectory.Children)
            {
                DBContext.StatsicInfo.Add(new StatsicInfo() { IsDel = 0, StatsicName = item.Name  });
                foreach (var subItem in item.Children)
                {
                    DBContext.StatsicRelation.Add(new StatsicRelation() {  GroupName = subItem.Name, StatsicName = item.Name  });
                }
            }
            DBContext.SaveChanges();
            // StatsisLib.PinYinHelper.GetChineseSpell("李先生");
            //NewMethod();
        }

        private static void NewMethod()
        {
            StringBuilder cc = new StringBuilder();
            var infos = StatsisLib.MappInfo.Deserialize("E:\\m.xml");
            infos.ForEach(x =>
            {
                cc.AppendFormat("{0}|{1}\r\n", x.Key, x.Value);
            });
            File.WriteAllText("E:\\m.txt", cc.ToString());
        }
    }
}

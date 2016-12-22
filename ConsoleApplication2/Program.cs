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
            DateTime dt = DateTime.Now;
            DateTime startMonth = dt.AddDays(1 - dt.Day);
            Console.WriteLine(startMonth);
           // NewMethod2();

            //NewMethod1();
            // StatsisLib.PinYinHelper.GetChineseSpell("李先生");
            //NewMethod();
        }

        private static void NewMethod2()
        {
            List<StatsisLib.UserInfo> infos = new List<StatsisLib.UserInfo>();
            string path = @"E:\Projects\tt\11.txt";
            string t = File.ReadAllText(path);
            foreach (var item in t.Split('\n'))
            {
                string[] aa = item.Replace("\r", "").Split('|');

                if (aa.Length == 2)
                {
                    string num = aa[1];
                    if (string.IsNullOrWhiteSpace(num))
                    {
                        continue;
                    }
                    infos.Add(new StatsisLib.UserInfo() { Num = num, GroupName = aa[0] });

                }

            }
            DataService.QHXEntities DBContext = new DataService.QHXEntities();
            foreach (var item in infos)
            {
                var s = DBContext.UserInfo.FirstOrDefault(x => x.Id.ToString() == item.Num);
                if (s == null)
                {
                    Console.WriteLine("缺少：{0}", item.Num);
                }
                else if (s.GroupName != item.GroupName)
                {
                    Console.WriteLine("对不上：{0},{1}=>{2}", item.Num, s.GroupName, item.GroupName);
                }
            }


            foreach (var item in DBContext.UserInfo.ToList())
            {
                var s = infos.FirstOrDefault(x => x.Num == item.Id.ToString());
                if (s == null)
                {
                    Console.WriteLine("多余：{0}", item.Id, item.GroupName);
                }
                //else if(s.GroupName!=item.GroupName)
                //{
                //    Console.WriteLine("对不上：{0}", item.Num);
                //}
            }
        }

        private static void NewMethod1()
        {
            DataService.QHXEntities DBContext = new DataService.QHXEntities();
            var NDirectory = StatsisLib.NestDirectory.Deserialize("C:\\g1.xml");
            foreach (var item in NDirectory.Children)
            {
                DBContext.StatsicInfo.Add(new StatsicInfo() { IsDel = 0, StatsicName = item.Name });
                foreach (var subItem in item.Children)
                {
                    DBContext.StatsicRelation.Add(new StatsicRelation() { GroupName = subItem.Name, StatsicName = item.Name });
                }
            }
            DBContext.SaveChanges();
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

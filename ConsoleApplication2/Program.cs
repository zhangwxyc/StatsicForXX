using StatsisLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Text;
using DataService;
using MailLib;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            var msg = "";
            var sendMail = new MailHelper("smtp.163.com", 25, "zhangwxyc@163.com", "qwerty123", new string[] { "zhangwxyc@yiche.com" }, "title is hitch send", "body is xxxx", false);
            sendMail.IsSendAttachments = true;
            sendMail.Attachments=new string[]{};
            var s= sendMail.Send(out msg);

            //var dat = NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\22.xls", 0, 1);
            //NewMethod4();
            //DateTime dt = NewMethod3();
            //DateTime startMonth = dt.AddDays(1 - dt.Day);
            //Console.WriteLine(startMonth);
            // NewMethod2();

            //NewMethod1();
            // StatsisLib.PinYinHelper.GetChineseSpell("李先生");
            //NewMethod();



        }

        private static void NewMethod4()
        {

            string data = File.ReadAllText(@"E:\Projects\tt\22.txt", Encoding.Default);
            string gName = "";
            List<StatsisLib.UserInfo> infos = new List<StatsisLib.UserInfo>();
            data.Replace("\r", "").Split('\n').ToList().ForEach(x =>
            {
                string[] paramss = x.Split('|');
                if (paramss.Length == 4)
                {
                    if (!string.IsNullOrWhiteSpace(paramss[0]))
                    {
                        gName = paramss[0];
                    }
                    if (!string.IsNullOrWhiteSpace(paramss[1]))
                    {
                        infos.Add(new StatsisLib.UserInfo()
                        {
                            GroupName = gName,
                            InTime = paramss[3],
                            Name = paramss[1],
                            Num = paramss[2],
                            Remark = "1.4"
                        });
                    }
                }
            });

            int index = 0;
            DataService.QHXEntities DBContext = new DataService.QHXEntities();
            foreach (var item in infos)
            {
                var s = DBContext.UserInfo.FirstOrDefault(x => x.Id.ToString() == item.Num);
                if (s == null)
                {
                    Console.WriteLine("缺少：{0}", item.Num);
                    index = DBContext.UserInfo.Count(y => y.GroupName == item.GroupName) + 1;
                    s = new DataService.UserInfo()
                    {
                        Id = int.Parse(item.Num),
                        GroupName = item.GroupName,
                        Name = item.Name,
                        OrderIndex = index,
                        InTime = item.InTime,
                        IsShield = 0,
                        Remark = item.Remark,
                        IsDel = 0
                    };
                    DBContext.UserInfo.Add(s);
                }
                else if (s.GroupName != item.GroupName)
                {
                    Console.WriteLine("对不上：{0},{1}=>{2}", item.Num, s.GroupName, item.GroupName);
                    s.GroupName = item.GroupName;
                }
                s.InTime = item.InTime;
            }
            DBContext.SaveChanges();
        }

        private static DateTime NewMethod3()
        {
            var dat = NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\11s.xls", 0, 1);


            DateTime dt = DateTime.Now;
            return dt;
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

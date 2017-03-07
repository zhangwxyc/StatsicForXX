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

            //AddWay();
            //DelWay();
            DelWay2();

            //var sendMail = new MailHelper();
            //sendMail.AttachmentSavePath = "C:\\";
            //sendMail.Login();

            //var sendMail = new MailReceiveHelper();
            //sendMail.Init();
            //sendMail.Receive("");
            // SendMail();

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

        private static void AddWay()
        {
            string dir = @"E:\Projects\tt\dojoin\Files";
            string[] files = Directory.GetFiles(dir, "*.xls", SearchOption.TopDirectoryOnly);
            List<BaseDataByDate> infos = new List<BaseDataByDate>();
            foreach (var item in files)
            {
                infos.Add(new BaseDataByDate()
                {
                    FileName = item,
                    Infos = StatsisLib.Common.DTToList<BaseDataInfo>(NPOIHelper.ImportExceltoDt(item, 0, 1)).Where(x => !string.IsNullOrWhiteSpace(x.工号)).ToList()
                });
            }

            #region Union
            var fInfos = StatsisLib.Common.DTToList<FilterByDateInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\join\11.xlsx"));

            List<BaseDataInfo> goodList = new List<BaseDataInfo>();
            List<BaseDataInfo> unGoodList = new List<BaseDataInfo>();
            foreach (var item in infos)
            {
                var fItems = fInfos.Where(x => x.日期 == item.RegId).ToList();
                foreach (var bItem in item.Infos)
                {
                    if (fItems.FindIndex(x => x.工号 == bItem.姓名) != -1)
                    {
                        unGoodList.Add(bItem);
                        var copyItem = bItem.DeepCloneClass();

                        copyItem.总接听量 -= copyItem.总量;
                        copyItem.满意 = 0;
                        copyItem.一般 = 0;
                        copyItem.不满意 = 0;
                        copyItem.总量 = 0;

                        goodList.Add(copyItem);
                    }
                    else
                        goodList.Add(bItem);
                }
            }


            #region @sum
            var dataInfos = goodList.GroupBy(x => x.姓名).Select(x => new BaseDataInfo()
            {
                技能组 = x.FirstOrDefault().技能组,
                工号 = x.FirstOrDefault().工号,
                姓名 = x.Key,
                录音抽检数 = x.Sum(y => y.录音抽检数),
                平均得分 = x.Average(y => y.平均得分),
                中度服务瑕疵量 = x.Sum(y => y.中度服务瑕疵量),
                重大服务失误量 = x.Sum(y => y.重大服务失误量),
                有效投诉量 = x.Sum(y => y.有效投诉量),
                XX = x.Sum(y => y.XX),
                总接听量 = x.Sum(y => y.总接听量),
                满意 = x.Sum(y => y.满意),
                不满意 = x.Sum(y => y.不满意),
                一般 = x.Sum(y => y.一般),
                总量 = x.Sum(y => y.总量),
                通过量 = x.Sum(y => y.通过量)
            }).ToList();
            #endregion

            #endregion


            //var dataInfos = unGoodList.GroupBy(x => x.工号).Select(x => new BaseDataInfo()
            //{
            //    技能组 = x.FirstOrDefault().技能组,
            //    工号 = x.Key,
            //    姓名 = x.FirstOrDefault().姓名,
            //    录音抽检数 = x.Sum(y => y.录音抽检数),
            //    平均得分 = x.Average(y => y.平均得分),
            //    中度服务瑕疵量 = x.Sum(y => y.中度服务瑕疵量),
            //    重大服务失误量 = x.Sum(y => y.重大服务失误量),
            //    有效投诉量 = x.Sum(y => y.有效投诉量),
            //    XX = x.Sum(y => y.XX),
            //    总接听量 = x.Sum(y => y.总接听量),
            //    满意 = x.Sum(y => y.满意),
            //    不满意 = x.Sum(y => y.不满意),
            //    一般 = x.Sum(y => y.一般),
            //    总量 = x.Sum(y => y.总量),
            //    通过量 = x.Sum(y => y.通过量)
            //}).ToList();

            #region Min

            var mInfos = StatsisLib.Common.DTToList<MinDataInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\dojoin\满意度修改记录.xls", DateTime.Now.AddDays(-15).Month + "月", 0));

            foreach (var item in mInfos.Where(x => !string.IsNullOrWhiteSpace(x.需修改员工工号)).ToList())
            {
                var dItem = dataInfos.FirstOrDefault(x => x.工号 == item.需修改员工工号);
                if (dItem != null)
                {
                    dItem.不满意 += item.不满意;
                    dItem.满意 += item.满意;
                    dItem.一般 += item.一般;
                }
            }

            #endregion

            var totalInfos = StatsisLib.Common.DTToList<BaseDataInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\dojoin\z.xls", 0, 1));
            totalInfos = totalInfos.Where(x => !string.IsNullOrWhiteSpace(x.工号)).ToList();
            foreach (var item in totalInfos)
            {
                var fInfo = dataInfos.FirstOrDefault(x => x.工号 == item.工号);
            }

            foreach (var item in dataInfos)
            {
                var fInfo = totalInfos.FirstOrDefault(x => x.工号 == item.工号);
                if (fInfo == null)
                {
                    item.工号 = "0";
                }
                else
                {
                    item.平均得分 = fInfo.平均得分;
                }
            }


            //   dataInfos.Insert(0, new BaseDataInfo());
            var dt1 = StatsisLib.Common.ListToDataTable(dataInfos);
            dt1.TableName = "1";
            var dt2 = StatsisLib.Common.ListToDataTable(unGoodList);
            dt2.TableName = "2";
            NPOIHelper.ExportSimple(
                new List<System.Data.DataTable>() 
                {
                   dt1,
                    dt2
                },
                @"E:\Projects\tt\dojoin\22.xlsx");
        }

        /// <summary>
        /// 采用减方法计算按天剔除数据
        /// </summary>
        private static void DelWay()
        {
            string dir = @"E:\Projects\tt\dojoin\Files";
            string[] files = Directory.GetFiles(dir, "*.xls", SearchOption.TopDirectoryOnly);
            List<BaseDataByDate> infos = new List<BaseDataByDate>();
            foreach (var item in files)
            {
                infos.Add(new BaseDataByDate()
                {
                    FileName = item,
                    Infos = StatsisLib.Common.DTToList<BaseDataInfo>(NPOIHelper.ImportExceltoDt(item, 0, 1)).Where(x => !string.IsNullOrWhiteSpace(x.工号)).ToList()
                });
            }

            #region Union
            var fInfos = StatsisLib.Common.DTToList<FilterByDateInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\dojoin\filter.xlsx"));

            List<BaseDataInfo> goodList = new List<BaseDataInfo>();
            List<BaseDataInfo> unGoodList = new List<BaseDataInfo>();
            foreach (var item in infos)
            {
                var fItems = fInfos.Where(x => x.日期 == item.RegId).ToList();
                foreach (var bItem in item.Infos)
                {
                    if (fItems.FindIndex(x => x.工号 == bItem.工号) != -1)
                    {
                        unGoodList.Add(bItem);
                    }
                }
            }


            #region @sum
            var dataInfos = unGoodList.GroupBy(x => x.工号).Select(x => new BaseDataInfo()
            {
                技能组 = x.FirstOrDefault().技能组,
                工号 = x.Key,
                姓名 = x.FirstOrDefault().姓名,
                //录音抽检数 = x.Sum(y => y.录音抽检数),
                //中度服务瑕疵量 = x.Sum(y => y.中度服务瑕疵量),
                //重大服务失误量 = x.Sum(y => y.重大服务失误量),
                有效投诉量 = x.Sum(y => y.有效投诉量),
                总接听量 = x.Sum(y => y.总接听量),
                满意 = x.Sum(y => y.满意),
                不满意 = x.Sum(y => y.不满意),
                一般 = x.Sum(y => y.一般),
                总量 = x.Sum(y => y.总量),
                //通过量 = x.Sum(y => y.通过量)
            }).ToList();
            #endregion

            #endregion

            #region d

            var totalInfos = StatsisLib.Common.DTToList<BaseDataInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\dojoin\z.xls", 0, 1));
            totalInfos = totalInfos.Where(x => !string.IsNullOrWhiteSpace(x.工号)).ToList();
            foreach (var item in totalInfos)
            {
                var fInfo = dataInfos.FirstOrDefault(x => x.工号 == item.工号);
                if (fInfo != null)
                {
                    item.录音抽检数 -= fInfo.录音抽检数;
                    item.中度服务瑕疵量 -= fInfo.中度服务瑕疵量;
                    item.重大服务失误量 -= fInfo.重大服务失误量;
                    item.总接听量 -= fInfo.总接听量;
                    item.满意 -= fInfo.满意;
                    item.不满意 -= fInfo.不满意;
                    item.一般 -= fInfo.一般;
                    item.总量 -= fInfo.总量;
                    item.通过量 -= fInfo.通过量;
                }
            }

            #endregion

            #region Min

            var mInfos = StatsisLib.Common.DTToList<MinDataInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\dojoin\满意度修改记录.xls", DateTime.Now.AddDays(-15).Month + "月", 0));

            foreach (var item in mInfos.Where(x => !string.IsNullOrWhiteSpace(x.需修改员工工号)).ToList())
            {
                var dItem = totalInfos.FirstOrDefault(x => x.工号 == item.需修改员工工号);
                if (dItem != null)
                {
                    dItem.不满意 += item.不满意;
                    dItem.满意 += item.满意;
                    dItem.一般 += item.一般;
                }
            }

            #endregion

            var cols = System.Configuration.ConfigurationManager.AppSettings["MainT"].Split(',').ToList();
            var dt1 = StatsisLib.Common.ListToDataTable(totalInfos, cols);
            dt1.TableName = "1";
            var dt2 = StatsisLib.Common.ListToDataTable(dataInfos, cols);
            dt2.TableName = "2";
            var dt3 = StatsisLib.Common.ListToDataTable(unGoodList, cols);
            dt3.TableName = "3";
            NPOIHelper.ExportSimple(
                new List<System.Data.DataTable>() 
                {
                   dt1,
                   dt2,
                   dt3
                },
                @"E:\Projects\tt\dojoin\22.xls");
        }

        /// <summary>
        /// 按人剔除
        /// </summary>
        private static void DelWay2()
        {


            //string dir = @"E:\Projects\tt\dojoin\Files";
            //string[] files = Directory.GetFiles(dir, "*.xls", SearchOption.TopDirectoryOnly);
            //List<BaseDataByDate> infos = new List<BaseDataByDate>();
            //foreach (var item in files)
            //{
            //    infos.Add(new BaseDataByDate()
            //    {
            //        FileName = item,
            //        Infos = StatsisLib.Common.DTToList<BaseDataInfo>(NPOIHelper.ImportExceltoDt(item, 0, 1)).Where(x => !string.IsNullOrWhiteSpace(x.工号)).ToList()
            //    });
            //}

            //#region Union

            var dataInfos = StatsisLib.Common.DTToList<TCInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\2月\1.xls"));

            //List<BaseDataInfo> goodList = new List<BaseDataInfo>();
            //List<BaseDataInfo> unGoodList = new List<BaseDataInfo>();
            //foreach (var item in infos)
            //{
            //    var fItems = fInfos.Where(x => x.日期 == item.RegId).ToList();
            //    foreach (var bItem in item.Infos)
            //    {
            //        if (fItems.FindIndex(x => x.工号 == bItem.工号) != -1)
            //        {
            //            unGoodList.Add(bItem);
            //        }
            //    }
            //}


            #region @sum
            //var dataInfos = unGoodList.GroupBy(x => x.工号).Select(x => new BaseDataInfo()
            //{
            //    技能组 = x.FirstOrDefault().技能组,
            //    工号 = x.Key,
            //    姓名 = x.FirstOrDefault().姓名,
            //    //录音抽检数 = x.Sum(y => y.录音抽检数),
            //    //中度服务瑕疵量 = x.Sum(y => y.中度服务瑕疵量),
            //    //重大服务失误量 = x.Sum(y => y.重大服务失误量),
            //    有效投诉量 = x.Sum(y => y.有效投诉量),
            //    总接听量 = x.Sum(y => y.总接听量),
            //    满意 = x.Sum(y => y.满意),
            //    不满意 = x.Sum(y => y.不满意),
            //    一般 = x.Sum(y => y.一般),
            //    总量 = x.Sum(y => y.总量),
            //    //通过量 = x.Sum(y => y.通过量)
            //}).ToList();
            //#endregion

            #endregion

            #region d

            var totalInfos = StatsisLib.Common.DTToList<BaseDataInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\2月\total.xls", 0, 1));
            totalInfos = totalInfos.Where(x => !string.IsNullOrWhiteSpace(x.工号)).ToList();
            foreach (var item in totalInfos)
            {
                var fInfo = dataInfos.FirstOrDefault(x => x.工号 == item.工号);
                if (fInfo != null)
                {
                    //item.录音抽检数 -= fInfo.录音抽检数;
                    // item.中度服务瑕疵量 -= fInfo.中度服务瑕疵量;
                    // item.重大服务失误量 -= fInfo.重大服务失误量;
                    item.总接听量 -= fInfo.总接听量;
                    item.满意 -= fInfo.满意;
                    item.不满意 -= fInfo.不满意;
                    item.一般 -= fInfo.一般;
                    //item.总量 -= fInfo.总量;
                    //item.通过量 -= fInfo.通过量;
                }
            }

            #endregion

            #region MYD

            string mydFile = @"E:\Projects\tt\2月\myd.xls";
            if (File.Exists(mydFile))
            {
                var mInfos = StatsisLib.Common.DTToList<MinDataInfo>(NPOIHelper.ImportExceltoDt(mydFile, DateTime.Now.AddDays(-15).Month + "月", 0));

                foreach (var item in mInfos.Where(x => !string.IsNullOrWhiteSpace(x.需修改员工工号)).ToList())
                {
                    var dItem = totalInfos.FirstOrDefault(x => x.工号 == item.需修改员工工号);
                    if (dItem != null)
                    {
                        dItem.不满意 += item.不满意;
                        dItem.满意 += item.满意;
                        dItem.一般 += item.一般;
                        dItem.总量 += item.Change;
                    }
                }
            }

            #endregion

            var cols = System.Configuration.ConfigurationManager.AppSettings["MainT"].Split(',').ToList();
            var dt1 = StatsisLib.Common.ListToDataTable(totalInfos, cols);
            dt1.TableName = "1";
            var dt2 = StatsisLib.Common.ListToDataTable(dataInfos, cols);
            dt2.TableName = "2";

            NPOIHelper.ExportSimple(
                new List<System.Data.DataTable>() 
                {
                   dt1,
                   dt2
                },
                @"E:\Projects\tt\2月\22.xls");
        }

        private static void UpdateUser()
        {
            var dataInfos = StatsisLib.Common.DTToList<DateInfo>(NPOIHelper.ImportExceltoDt(@"E:\Projects\tt\2月\date.xls"));
            DataService.QHXEntities DBContext = new DataService.QHXEntities();
            foreach (var uItem in dataInfos)
            {
                var s = DBContext.UserInfo.FirstOrDefault(x => x.Id.ToString() == uItem.工号);
                if (s != null)
                {
                    s.InTime = uItem.上岗时间;
                }
            }
            DBContext.SaveChanges();
        }

        private static void SendMail()
        {
            var msg = "";
            var sendMail = new MailHelper("smtp.163.com", 25, "zhangwxyc@163.com", "qwerty123", new string[] { "zhangwxyc@yiche.com" }, "title is hitch send", "body is xxxx", false);
            sendMail.IsSendAttachments = true;
            sendMail.Attachments = new string[] { };
            var s = sendMail.Send(out msg);
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

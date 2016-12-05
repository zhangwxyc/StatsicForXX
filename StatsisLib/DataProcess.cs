using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsisLib
{
    public class DataProcess
    {
        public static object GetT3(List<BaseDataInfo> infos, string groups)
        {
            var t3 = infos.Where(x => FilterGroup(x, groups))
                             .Select(x => new
                             {
                                 a0 = 0,
                                 a1 = x.工号,
                                 a2 = x.姓名,
                                 a3 = x.平均得分,
                                 a4 = GetPassRate(x),
                                 a5 = GetSI(x),
                                 a6 = GetEvalRate(x),
                                 a7 = GetJSI(x),
                                 a8 = GetYu(x)
                             })
                         .OrderByDescending(x => x.a3)
                         .ThenByDescending(x => x.a4)
                         .ThenByDescending(x => x.a5)
                         .ThenByDescending(x => x.a6)
                         .ThenByDescending(x => x.a7)
                         .ThenByDescending(x => x.a8)
                         .ToList()
                         ;
            return t3;
        }

        private static bool FilterGroup(BaseDataInfo x, string groups)
        {
            if (x == null || string.IsNullOrWhiteSpace(groups) || string.IsNullOrEmpty(x.工号))
            {
                return false;
            }

            return groups.Split(',').ToList().Contains(x.技能组);
        }

        private static bool FilterGroup(BaseDataInfo x)
        {
            throw new NotImplementedException();
        }





        public static int Total(List<BaseDataInfo> infos, int index)
        {
            var total = infos.OrderByDescending(x => x.平均得分).Select(x => new
            {
                g0 = index++,
                g1 = x.工号,
                g2 = x.姓名,
                g3 = x.平均得分
            }).ToList();
            return index;
        }

        private static void T1(Dictionary<string, List<BaseDataInfo>> list)
        {
            var linfos = list.Select(x => new BaseDataInfo()
            {
                技能组 = x.Key,
                录音抽检数 = x.Value.Sum(y => y.录音抽检数),
                平均得分 = x.Value.Average(y => y.平均得分),
                总接听量 = x.Value.Sum(y => y.总接听量),
                中度服务瑕疵量 = x.Value.Sum(y => y.中度服务瑕疵量),
                重大服务失误量 = x.Value.Sum(y => y.重大服务失误量)
                //gg = 0,
                // 通过率 = GetRate(x.Value.Sum(y => y.通过量), x.Value.Sum(y => y.录音抽检数)),
                // 满意度系数 = GetFactor(x.Value.Sum(y => y.通过量), x.Value.Sum(y => y.录音抽检数))
            }).ToList();
        }



        public static List<BaseDataInfo> Process(List<BaseDataInfo> list, NestDirectory nsDir)
        {
            var groupList = list.GroupBy(x => x.Tag).ToDictionary(x => x.Key);

            var sumLines = SumLine(list);

            var finallyList = new List<BaseDataInfo>();

            foreach (var item in groupList)
            {
                finallyList.AddRange(item.Value);
                var subLine = sumLines.FirstOrDefault(x => x.Tag == item.Key);
                if (subLine != null)
                {
                    finallyList.Add(subLine);
                }
            }

            foreach (var item in nsDir.Children)
            {
                var subLine = SumLine(list, item);
                finallyList.Add(subLine);
            }

            var totalLine = SumLine(list, nsDir, true);
            finallyList.Add(totalLine);

            Compute(finallyList);

            OutputMainT(finallyList);

            return finallyList;

        }

        private static void OutputMainT(List<BaseDataInfo> finallyList)
        {
            throw new NotImplementedException();
        }

        public static DataTable T1(List<BaseDataInfo> list)
        {
            var sumLines = SumLine(list);
            Compute(sumLines);
            var resultData = sumLines.OrderByDescending(x => x.平均得分).ThenByDescending(x => x.通过率).ToList();
            var dt = Common.ListToDataTable<BaseDataInfo>(resultData, Common.GetConfig("T1").Split(',').ToList(), false);
            return dt;
        }

        public static DataTable T2(List<BaseDataInfo> list)
        {
            string groups = Common.GetConfig("VIP");
            var subList = list.Where(x => FilterGroup(x, groups)).ToList();
            Compute(subList);
            var dt = Common.ListToDataTable<BaseDataInfo>(subList, Common.GetConfig("T2").Split(',').ToList(), false);
            return dt;
        }

        public static DataTable T3(List<BaseDataInfo> list)
        {
            var subList = list.Where(x => x.IsNew).ToList();
            Compute(subList);
            var dt = Common.ListToDataTable<BaseDataInfo>(subList, Common.GetConfig("T3").Split(',').ToList(), false);
            return dt;
        }
        public static DataTable T4(List<BaseDataInfo> list)
        {
            string tableHeader = "T2";

            var subList = list.Where(x => !x.IsNew).ToList();
            var dt = T0(subList, tableHeader);
            return dt;
        }

        private static DataTable T0(List<BaseDataInfo> subList, string tableHeader)
        {
            Compute(subList);
            subList = subList.OrderByDescending(x => RateToDouble(x.通过率)).ThenByDescending(x => RateToDouble(x.净满意度)).ToList();
            var dt = Common.ListToDataTable<BaseDataInfo>(subList, Common.GetConfig(tableHeader).Split(',').ToList(), true);
            return dt;
        }

        public static DataTable T5(List<BaseDataInfo> list)
        {
            string groups = Common.GetConfig("All");
            var sList = list.Where(x => FilterGroup(x,groups)).ToList();
            var dt = T0(sList,"T5");
            return dt;
        }


        public static List<BaseDataInfo> SumLine(List<BaseDataInfo> list)
        {
            var linfo = list.GroupBy(x => x.技能组).Select(x => new BaseDataInfo()
            {
                技能组 = x.Key,

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
                //gg = 0,
                // 通过率 = GetRate(x.Value.Sum(y => y.通过量), x.Value.Sum(y => y.录音抽检数)),
                // 满意度系数 = GetFactor(x.Value.Sum(y => y.通过量), x.Value.Sum(y => y.录音抽检数))
            }).ToList();

            return linfo;
        }

        public static BaseDataInfo SumLine(List<BaseDataInfo> list, NestDirectory dir, bool isNest = false)
        {
            string groups = dir.GetChildrenNames(isNest);
            var subList = list.Where(x => FilterGroup(x, groups)).ToList();
            subList.ForEach(x => x.Tag = dir.Name);

            var linfo = subList.GroupBy(x => x.Tag).Select(x => new BaseDataInfo()
            {
                技能组 = dir.Name,

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
                //gg = 0,
                // 通过率 = GetRate(x.Value.Sum(y => y.通过量), x.Value.Sum(y => y.录音抽检数)),
                // 满意度系数 = GetFactor(x.Value.Sum(y => y.通过量), x.Value.Sum(y => y.录音抽检数))
            }).FirstOrDefault();

            return linfo;
        }


        public static void Compute(List<BaseDataInfo> list)
        {
            foreach (var item in list)
            {
                item.通过率 = GetPassRate(item);
                item.满意度系数 = GetSII(item);
                //item.客户满意度=Get
                item.客户评价率 = GetEvalRate(item);
                item.客户满意度 = GetSI(item);
                item.净满意度 = GetJSI(item);
                item.通过率系数 = GetPII(item);
            }
        }

        public static double RateToDouble(string rate)
        {
            if (string.IsNullOrWhiteSpace(rate))
            {
                return 0;
            }

            rate = rate.Replace("%", "");
            double r = 0;
            double.TryParse(rate, out r);
            return r;
        }

        private static string GetPassRate(BaseDataInfo x)
        {
            return GetRate(x.通过量, x.录音抽检数);
        }
        private static string GetEvalRate(BaseDataInfo x)
        {
            return GetRate(x.总量, x.总接听量);
        }

        private static string GetSI(BaseDataInfo x)
        {
            return GetRate(x.满意, x.总量);
        }

        private static string GetJSI(BaseDataInfo x)
        {
            return GetRate((x.满意 - x.不满意), x.总量);
        }
        private static double GetPII(BaseDataInfo x)
        {
            return GetFactorPass(x.通过量, x.录音抽检数);
        }
        private static string GetSII(BaseDataInfo x)
        {
            return GetFactor((x.满意 - x.不满意), x.总量).ToString();
        }
        private static string GetYu(BaseDataInfo x)
        {
            return "";
        }

        /// <summary>
        /// 百分比小数2位
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private static string GetRate(int v1, int v2)
        {
            if (v2 == 0 || v1 == 0)
            {
                return "";
            }
            return (v1 * 100.0 / v2).ToString("f2") + "%";
        }

        /// <summary>
        /// 系数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private static double GetFactor(int v1, int v2)
        {
            if (v2 == 0 || v1 == 0)
            {
                return 0;
            }
            return GetFactor((double)v1 / v2);
        }

        public static double GetFactor(double rate)
        {
            if (rate == 1)
            {
                return 1.5;
            }
            if (rate >= 0.9)
            {
                return 1;
            }
            if (rate >= 0.85)
            {
                return 0.8;
            }
            return 0.5;
        }
        private static double GetFactorPass(int v1, int v2)
        {
            if (v2 == 0 || v1 == 0)
            {
                return 0;
            }
            return GetFactorPass((double)v1 / v2);
        }
        public static double GetFactorPass(double rate)
        {
            if (rate >= 0.97)
            {
                return 1;
            }
            if (rate >= 0.965)
            {
                return 0.9;
            }
            if (rate >= 0.955)
            {
                return 0.8;
            }
            return 0.5;
        }
    }
}

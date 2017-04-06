using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsisLib.QA
{
    public class QADataProcess
    {
        public static List<QAStatsicInfo> Compute(List<QASrcInfo> srcInfos)
        {
            return srcInfos.GroupBy(x => x.质检员编号).Select(x =>

                 new QAStatsicInfo()
                 {
                     工号 = x.Key,
                     合计 = x.Count(),
                     抽检平均成绩 = (x.Sum(y => y.总分) / x.Count()).ToString("F2"),
                     满分占比 = GetRate(x.Count(y => y.总分 == 100), x.Count()),
                     电话平台 = x.Count(y => y.GroupType == 1),
                     全媒平台 = x.Count(y => y.GroupType == 2)
                       ,
                     满分数 = x.Count(y => y.总分 == 100)
                 }

                 ).ToList();
        }
        private static string GetRate(int v1, int v2)
        {
            if (v2 == 0 || v1 == 0)
            {
                return "";
            }
            return (v1 * 100.0 / v2).ToString("f2") + "%";
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "C:\\2.xls";
            var dt = NPOIHelper.ImportExceltoDt(path, 0, 0);
            var infos = Common.DTToList<BaseDataInfo>(dt);
            int index = 1;
            index = Total(infos, index);

            infos.Select(x => new {
                a0 = 0,
                a1=x.工号,
                a2=x.姓名,
                a3=x.平均得分,
                a4=x.
            });

            var list = infos.GroupBy(x => x.技能组).ToDictionary(x => x.Key, x => x.ToList());
            T1(list);
        }

        private static int Total(List<BaseDataInfo> infos, int index)
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
            var linfos = list.Select(x => new
            {
                技能组 = x.Key,
                cc = x.Value.Sum(y => y.录音抽检数),
                bb = x.Value.Average(y => y.平均得分),
                aa = x.Value.Sum(y => y.总接听量),
                dd = x.Value.Sum(y => y.中度服务瑕疵量),
                ee = x.Value.Sum(y => y.重大服务失误量),
                gg = 0,
                ff = GetRate(x.Value.Sum(y => y.通过量), x.Value.Sum(y => y.录音抽检数)),
                mm = GetFactor(x.Value.Sum(y => y.通过量), x.Value.Sum(y => y.录音抽检数))
            }).ToList();
        }

        private static string GetRate(int v1, int v2)
        {
            if (v2 == 0 || v1 == 0)
            {
                return "";
            }
            return (v1 * 100.0 / v2).ToString("f2") + "%";
        }

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
            if (rate > 0.9)
            {
                return 1;
            }
            if (rate >= 0.85)
            {
                return 0.8;
            }
            return 0.5;
        }
    }
}

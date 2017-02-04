using StatsisLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace UnionLib
{
    public class AnaysleService
    {
        public IProviderBase DataProvider { get; set; }
        public AnaysleService()
        {
            DataProvider = new ProviderWithDB();
        }
        public List<BaseDataInfo> SrcInfos { get; set; }
        public string Create(string absoluFilePath, string outputPath = "")
        {
            var dt = NPOIHelper.ImportExceltoDt(absoluFilePath, 0, 1);
            SrcInfos = StatsisLib.Common.DTToList<BaseDataInfo>(dt);

            //SrcInfos = FilterUsers(SrcInfos);
            var DestInfos = WatshData(SrcInfos);//洗
            // DataProcess.Compute(DestInfos);//基础计算
            
            List<DataTable> ds = new List<DataTable>()
            {
                CreateMainTable(DestInfos),
                DataProcess.T1(DestInfos.OrderBy(x => GetIndex(x.技能组)).ToList()),
                DataProcess.T2(DestInfos),
                DataProcess.T2_5(DestInfos),

                DataProcess.T4(DestInfos),
                DataProcess.T3(DestInfos),
                DataProcess.T5(DestInfos)
            };

            RenameTableName(ds);
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = GetOutputPath(absoluFilePath);
            }
            NPOIHelper.ExportSimple(ds, outputPath);
            return outputPath;
        }

        public List<BaseDataInfo> GetSumLineTable(string path)
        {
            var dt = NPOIHelper.ImportExceltoDt(path, 0, 1);
            SrcInfos = StatsisLib.Common.DTToList<BaseDataInfo>(dt);
            var DestInfos = WatshData(SrcInfos);//洗
            var sumLines = DataProcess.SumLine(DestInfos);
            DataProcess.Compute(sumLines);
            var resultData = sumLines.OrderByDescending(x => x.通过率).ThenByDescending(x => x.净满意度).ToList();
            return resultData;

        }

        private string GetOutputPath(string absoluFilePath)
        {
            string dir = Path.GetDirectoryName(absoluFilePath);
            string name = string.Format("{0}_Anaysle.xls", Path.GetFileNameWithoutExtension(absoluFilePath));
            string fullPath = Path.Combine(dir, name);
            return fullPath;
        }

        /// <summary>
        /// According by UserData Watsh SrcData
        /// </summary>
        /// <param name="dataInfos"></param>
        /// <returns></returns>
        public List<BaseDataInfo> WatshData(List<BaseDataInfo> dataInfos)
        {
            List<UserInfo> userInfos = DataProvider.GetUserInfos(null);
            var infos = new List<BaseDataInfo>();
            foreach (var item in dataInfos)
            {
                if (item.总接听量==0||item.录音抽检数==0)
                {
                    continue;
                }
                string num = item.工号;
                if (!string.IsNullOrWhiteSpace(num))
                {
                    var uInfo = userInfos.FirstOrDefault(x => x.Num.Equals(num));
                    if (uInfo != null)
                    {
                        item.技能组 = uInfo.GroupName;
                        item.新人上岗时间 = uInfo.InTime;
                        item.OrderIndex = uInfo.OrderIndex;
                        item.IsShield = uInfo.IsShield;
                        infos.Add(item);
                    }
                }
            }
            return infos;
        }

        public DataTable CreateMainTable(List<BaseDataInfo> dataInfos)
        {
            List<BaseDataInfo> infos = GetGroupStatsicLine(dataInfos);
            List<BaseDataInfo> statsicInfos = GetStatsicLines(dataInfos);
            infos.AddRange(statsicInfos);
            DataProcess.Compute(infos);
            return DataProcess.T6(infos);
        }

        private List<BaseDataInfo> GetGroupStatsicLine(List<BaseDataInfo> dataInfos)
        {
            List<BaseDataInfo> infos = new List<BaseDataInfo>();
            var extDatas = dataInfos.GroupBy(x => x.技能组).ToDictionary(x => x.Key, x => x.OrderBy(y => y.OrderIndex).ToList());
            extDatas = extDatas.OrderBy(x => GetIndex(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            foreach (var item in extDatas)
            {
                infos.AddRange(item.Value);
                var subLine = DataProcess.CreateStatsicLine(item.Value, item.Key, new List<string>() { item.Key });
                infos.Add(subLine);
            }
            return infos;
        }

        private int GetIndex(string key)
        {
            return DataProvider.GetGroupIndex(key);
        }

        private List<BaseDataInfo> GetStatsicLines(List<BaseDataInfo> dataInfos)
        {
            NestDirectory dir = DataProvider.GetNestDirectory(null);
            List<BaseDataInfo> statsicInfos = new List<BaseDataInfo>();
            foreach (var item in dir.Children)
            {
                BaseDataInfo line = DataProcess.CreateStatsicLine(dataInfos, item.Name, item.Children.Select(x => x.Name).ToList());
                statsicInfos.Add(line);
            }
            return statsicInfos;
        }

        private static void RenameTableName(List<DataTable> ds)
        {
            string tableNames = Common.GetConfig("T_Names");
            if (!string.IsNullOrWhiteSpace(tableNames))
            {
                string[] names = tableNames.Split(',');
                if (names.Length >= ds.Count)
                {
                    int index = 0;
                    ds.ForEach(x => x.TableName = names[index++]);
                }
            }
        }
    }
}

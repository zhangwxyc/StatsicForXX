using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatsisLib
{
    public class AnaysleService
    {
        public AnaysleService()
        {
            
        }
        public List<BaseDataInfo> SrcInfos { get; set; }
        public string Create(string absoluFilePath)
        {
            var dt = NPOIHelper.ImportExceltoDt(absoluFilePath, 0, 0);
            SrcInfos = StatsisLib.Common.DTToList<BaseDataInfo>(dt);
            
            //SrcInfos = FilterUsers(SrcInfos);
            var DestInfos = UpdateDest(new List<MappInfo>(), SrcInfos);
            List<DataTable> ds = new List<DataTable>()
            {
                DataProcess.T1(DestInfos),
                DataProcess.T2(DestInfos),
                DataProcess.T2_5(DestInfos),

                DataProcess.T4(DestInfos),
                DataProcess.T3(DestInfos),
                DataProcess.T5(DestInfos)
            };

            RenameTableName(ds);
            string randPath = absoluFilePath;
            return absoluFilePath;

        }

        /// <summary>
        /// According by UserData Watsh SrcData
        /// </summary>
        /// <param name="userInfos"></param>
        /// <param name="dataInfos"></param>
        /// <returns></returns>
        public List<BaseDataInfo> WatshData(List<UserInfo> userInfos, List<BaseDataInfo> dataInfos)
        {
            var infos = new List<BaseDataInfo>();
            foreach (var item in dataInfos)
            {
                string num = item.工号;
                if (!string.IsNullOrWhiteSpace(num))
                {
                    var uInfo = userInfos.FirstOrDefault(x => x.Num.Equals(num));
                    if (uInfo!=null)
                    {
                        item.技能组 = uInfo.GroupName;
                        item.新人上岗时间 = uInfo.InTime;
                        infos.Add(item);
                    }
                }
            }
            return infos;
        }

        public List<BaseDataInfo> CreateMainTable(List<BaseDataInfo> dataInfos)
        {
            
            return dataInfos;
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

        public List<BaseDataInfo> UpdateDest(List<MappInfo> mapperList, List<BaseDataInfo> SrcInfos)
        {
            var destInfos = new List<BaseDataInfo>();
            destInfos.ForEach(x =>
            {

                var info = mapperList.FirstOrDefault(y => y.Key == x.技能组);
                if (info != null)
                {
                    if (info.Value == "-1")
                    {
                        // DestInfos.Remove(x);
                    }
                    else
                    {
                        x.技能组 = info.Value;
                        destInfos.Add(x);
                    }
                }
                else
                {
                    destInfos.Add(x);
                }
            });
            return destInfos;
        }

        private List<BaseDataInfo> FilterUsers(List<BaseDataInfo> srcInfos,List<string> userList)
        {
            srcInfos = srcInfos.Where(x => !string.IsNullOrEmpty(x.姓名)).ToList();
            List<BaseDataInfo> tmp = new List<BaseDataInfo>();
            foreach (var item in srcInfos)
            {
                var t = userList.FirstOrDefault(x => x.Equals(item.工号));
                if (t != null)
                {
                    tmp.Add(item);
                }
            }
            return tmp;
        }
    }
}

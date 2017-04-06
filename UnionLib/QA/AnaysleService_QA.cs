using StatsisLib;
using StatsisLib.QA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace UnionLib
{
    public partial class AnaysleService
    {
        public string CreateQA(string absoluFilePath, string outputPath = "")
        {
            var dt = NPOIHelper.ImportExceltoDt(absoluFilePath, 0, 0);
            var srcInfos = StatsisLib.Common.DTToList<QASrcInfo>(dt);
            foreach (var item in srcInfos)
            {
                if (IsQuanMei(item))
                {
                    item.GroupType = 2;
                }
                else
                    item.GroupType = 1;
            }
            var desInfos = QADataProcess.Compute(srcInfos);
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = GetOutputPath(absoluFilePath);
            }
            List<DataTable> ds = new List<DataTable>()
           {
               Common.ListToDataTable(desInfos)
           };
            ds[0].TableName = "Hi";
            NPOIHelper.ExportSimple(ds, outputPath);
            return outputPath;
        }

        public bool IsQuanMei(QASrcInfo info)
        {
            return info.技能组.Contains("全媒");
            //string[] qmNames={"",""};
            //if(qmNames.Contains(groupName))
            //{
            //    return true;
            //}
            //return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsisLib.QA
{
    public class QASrcInfo
    {
        public string 质检序号 { get; set; }
        public string 质检员编号 { get; set; }
        public string 技能组 { get; set; }
        public string 工号 { get; set; }
        public string 姓名 { get; set; }
        public decimal 总分 { get; set; }

        public int GroupType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsisLib.Models
{
    public class URInfo
    {
        public string 组别 { get; set; }
        public string 姓名 { get; set; }
        public string 工号 { get; set; }
        public string 特殊情况 { get; set; }

        public string 入组时间 { get; set; }

        public bool IsHiddenFromInGroup { get; set; }
    }
}

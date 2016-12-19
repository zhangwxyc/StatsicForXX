using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsisLib
{
    [Serializable]
    public class UserInfo
    {
        public string Num { get; set; }
        public string Name { get; set; }

        public string InTime { get; set; }
        public string GroupName { get; set; }
    }
}

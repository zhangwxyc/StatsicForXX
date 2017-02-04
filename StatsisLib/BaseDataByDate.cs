using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StatsisLib
{
    public class BaseDataByDate
    {
        public string FileName { get; set; }

        public string RegId
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FileName);
            }
        }

        public List<BaseDataInfo> Infos { get; set; }

        public BaseDataByDate()
        {
            Infos = new List<BaseDataInfo>();
        }
    }
}

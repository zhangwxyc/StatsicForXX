using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatsisLib.TM
{
    public interface IMaker
    {
        DataTable Create(List<BaseDataInfo> infos);
    }
}

using System;

namespace StatsisLib
{
    [Serializable]
    public partial class BaseDataInfo
    {


        #region base
        public string 技能组 { get; set; }

        public string 工号 { get; set; }

        public string 姓名 { get; set; }
        public string 新人上岗时间 { get; set; }
        public int 录音抽检数 { get; set; }

        public decimal 平均得分 { get; set; }

        public int 通过量 { get; set; }

        public int 中度服务瑕疵量 { get; set; }

        public int 重大服务失误量 { get; set; }

        public int 总接听量 { get; set; }

        public int 满意 { get; set; }

        public int 一般 { get; set; }

        public int 不满意 { get; set; }

        public int 总量 { get; set; }
        public int 有效投诉量 { get; set; }

        public string Tag { get; set; }

        public int XX { get; set; }

        #endregion

        #region compute
        public double 通过率系数 { get; set; }

        public string 通过率 { get; set; }

        public string 客户满意度 { get; set; }

        public string 不满意比率 { get; set; }

        public string 客户评价率 { get; set; }

        public string 净满意度 { get; set; }

        public string 满意度系数 { get; set; }
        #endregion


        public bool IsNew
        {
            get
            {
                if (string.IsNullOrEmpty(新人上岗时间))
                {
                    return false;
                }
                DateTime dt = DateTime.Now;

                DateTime startMonth = dt.Day < 15 ? dt.AddDays(1 - dt.Day) : dt.AddDays(1 - dt.Day).AddMonths(1);

                if (!DateTime.TryParse(新人上岗时间, out dt))
                {
                    return false;
                }


                return dt.AddMonths(7) > startMonth;
            }
        }

        public int OrderIndex { get; set; }

        public int IsShield { get; set; }

        public bool IsHidden
        {
            get
            {
                if (string.IsNullOrEmpty(新人上岗时间))
                {
                    return false;
                }
                DateTime dt = DateTime.Now;
                DateTime startMonth = dt.Day < 15 ? dt.AddDays(1 - dt.Day) : dt.AddDays(1 - dt.Day).AddMonths(1);

                if (!DateTime.TryParse(新人上岗时间, out dt))
                {
                    return false;
                }


                return dt.AddMonths(1) > startMonth;
            }
        }
    }
}

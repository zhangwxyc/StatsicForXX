using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsisLib
{
    public class MinDataInfo
    {
        public string 需修改员工工号 { get; set; }

        public int 满意
        {
            get
            {
                int tmp = 0;
                if (ToInt(原评价) == 1)
                {
                    tmp--;
                }
                if (ToInt(修改后评价) == 1)
                {
                    tmp++;
                }
                return tmp;
            }
        }

        public int 一般
        {
            get
            {
                int tmp = 0;
                if (ToInt(原评价) == 2)
                {
                    tmp--;
                }
                if (ToInt(修改后评价) == 2)
                {
                    tmp++;
                }
                return tmp;
            }
        }


        public int 不满意
        {
            get
            {
                int tmp = 0;
                if (ToInt(原评价) == 3)
                {
                    tmp--;
                }
                if (ToInt(修改后评价) == 3)
                {
                    tmp++;
                }
                return tmp;
            }
        }

        public int Change
        {
            get
            {
                return 满意 + 一般 + 不满意;
            }
        }


        public string 原评价 { get; set; }
        public string 修改后评价 { get; set; }


        int ToInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }
            int ret = 0;

            int.TryParse(value, out ret);
            return ret;
        }
    }
}

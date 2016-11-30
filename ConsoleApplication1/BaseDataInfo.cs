namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BaseDataInfo
    {
        [StringLength(255)]
        public string 技能组 { get; set; }

        [Key]
        [StringLength(255)]
        public string 工号 { get; set; }

        [StringLength(255)]
        public string 姓名 { get; set; }

        [StringLength(255)]
        public string 新人上岗时间 { get; set; }

        public int 录音抽检数 { get; set; }

        public decimal 平均得分 { get; set; }

        public int 通过量 { get; set; }

        public int 中度服务瑕疵量 { get; set; }

        public int 重大服务失误量 { get; set; }

        [StringLength(255)]
        public string 通过率 { get; set; }

        public int 总接听量 { get; set; }

        public int 满意 { get; set; }

        public int 一般 { get; set; }

        public int 不满意 { get; set; }

        public int 总量 { get; set; }

        [StringLength(255)]
        public string 客户满意度 { get; set; }

        [StringLength(255)]
        public string 不满意比率 { get; set; }

        [StringLength(255)]
        public string 客户评价率 { get; set; }

        [StringLength(255)]
        public string 净满意度 { get; set; }

        [StringLength(255)]
        public string 满意度系数 { get; set; }

        public string 有效投诉量 { get; set; }
    }
}

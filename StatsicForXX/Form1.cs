using StatsisLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatsicForXX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tbPath_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            if (dig.ShowDialog()==DialogResult.OK)
            {
                tbPath.Text = dig.SafeFileName;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string path = "C:\\2.xls";
            var dt = NPOIHelper.ImportExceltoDt(path, 0, 0);
            var infos = Common.DTToList<BaseDataInfo>(dt);
            int index = 1;
            var list = infos.GroupBy(x => x.技能组).ToDictionary(x => x.Key, x => x.ToList());
            new SettingGroups(list.Select(x => x.Key).ToList()).ShowDialog();
        }
    }
}

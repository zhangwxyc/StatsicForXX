using StatsisLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
        /// <summary>
        /// 首次导入数据
        /// </summary>
        public List<BaseDataInfo> SrcInfos { get; set; }

        /// <summary>
        /// 实际使用数据
        /// </summary>
        public List<BaseDataInfo> DestInfos { get; set; }


        public void UpdateDest()
        {
            // DestInfos = Clone<BaseDataInfo>(SrcInfos);
            DestInfos = SrcInfos.DeepClone();
            string path = GetMapPath();
            if (File.Exists(path))
            {
                var mapperInfos = MappInfo.Deserialize(path);

                DestInfos.ForEach(x =>
                {

                    var info = mapperInfos.FirstOrDefault(y => y.Key == x.技能组);
                    if (info != null)
                    {
                        if (info.Value == "-1")
                        {
                            DestInfos.Remove(x);
                        }
                        else
                            x.技能组 = info.Value;
                    }
                });
            }
        }

        private string GetMapPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "mapper.xml");
        }
        public static List<T> Clone<T>(object List)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, List);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as List<T>;
            }
        }
        private void tbPath_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            if (dig.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = dig.SafeFileName;
                string path = "C:\\2.xls";
                LoadSrcInfo(path);
            }
        }

        private void LoadSrcInfo(string path)
        {
            var dt = NPOIHelper.ImportExceltoDt(path, 0, 0);
            SrcInfos = Common.DTToList<BaseDataInfo>(dt);
            SrcInfos = SrcInfos.Where(x => !string.IsNullOrEmpty(x.姓名)).ToList();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            UpdateDest();
            int index = 1;
            OpenSettingDig();
        }

        private void OpenSettingDig()
        {
            var list = SrcInfos.GroupBy(x => x.技能组).ToDictionary(x => x.Key, x => x.ToList());
            new SettingGroups(list.Select(x => x.Key).ToList()).ShowDialog();
        }

        private void btn_Go_Click(object sender, EventArgs e)
        {
            string path = "C:\\2.xls";
            LoadSrcInfo(path);
            UpdateDest();
            DataProcess.T4(DestInfos);

           // string path = GetXmlPath();
            if (File.Exists(path))
            {

                //var list =DestInfos.GroupBy(x => x.技能组).ToDictionary(x => x.Key, x => x.ToList());

                //NestDirectory nsDir = StatsisLib.NestDirectory.Deserialize(path);
                //foreach (var item in nsDir.Children)
                //{
                //    string groups = item.GetChildrenNames();
                   
                //}
            }
            else
            {
                MessageBox.Show("未设置组");
            }


        }

        private string GetXmlPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "group.xml");
        }
    }
}

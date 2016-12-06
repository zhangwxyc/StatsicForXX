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
using System.Xml;

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
                var tmp = new List<BaseDataInfo>();
                DestInfos.ForEach(x =>
                {

                    var info = mapperInfos.FirstOrDefault(y => y.Key == x.技能组);
                    if (info != null)
                    {
                        if (info.Value == "-1")
                        {
                            // DestInfos.Remove(x);
                        }
                        else
                        {
                            x.技能组 = info.Value;
                            tmp.Add(x);
                        }
                    }
                    else
                    {
                        tmp.Add(x);
                    }
                });
                DestInfos = tmp;
            }
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
                tbPath.Text = dig.FileName;
            }

            if (LoadSrcInfo(tbPath.Text))
            {
                btn_Go.Enabled = btn_setting.Enabled = true;
            }
        }

        private bool LoadSrcInfo(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return false;
            }
            try
            {
                var dt = NPOIHelper.ImportExceltoDt(path, 0, 0);
                SrcInfos = Common.DTToList<BaseDataInfo>(dt);
                SrcInfos = SrcInfos.Where(x => !string.IsNullOrEmpty(x.姓名)).ToList();
                SrcInfos = FilterUsers(SrcInfos);
                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        private List<BaseDataInfo> FilterUsers(List<BaseDataInfo> srcInfos)
        {
            var infos = GetUserInfos();
            List<BaseDataInfo> tmp = new List<BaseDataInfo>();
            foreach (var item in srcInfos)
            {
                var t = infos.FirstOrDefault(x=>x.Num.Equals(item.工号));
                if (t!=null)
                {
                    item.新人上岗时间 = t.InTime;
                    tmp.Add(item);
                }
            }
            return tmp;
        }

        private List<UserInfo> GetUserInfos()
        {
            var infos = new List<UserInfo>();

            infos = infos.Deserialize<List<UserInfo>>(GetUserPath());

           // XmlDocument xml = new XmlDocument();
          //  xml.Load(GetUserPath());
           // var uInfos= xml.SelectNodes("UserInfo");
            return infos;
        }

        private void btn_Go_Click(object sender, EventArgs e)
        {
            if (!IsFinishConfig)
            {
                OpenSettingDig();
            }
            if (!IsFinishConfig)
            {
                MessageBox.Show("未设置组");
                return;
            }

            UpdateDest();
            List<DataTable> ds = new List<DataTable>()
            {
                DataProcess.T1(DestInfos),
                DataProcess.T2(DestInfos),
                DataProcess.T2_5(DestInfos),

                DataProcess.T4(DestInfos),
                DataProcess.T3(DestInfos),
                DataProcess.T5(DestInfos)
            };
            int index = 0;
            string[] names = Common.GetConfig("T_Names").Split(',');
            ds.ForEach(x => x.TableName = names[index++]);
            NPOIHelper.ExportSimple(ds, "C:\\1q.xlsx");
            MessageBox.Show("导出完成");

        }

        private string GetMapPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "mapper.xml");
        }
        private string GetXmlPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "group.xml");
        }
        private string GetUserPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "user.xml");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btn_Go.Enabled = btn_setting.Enabled = false;
            tbPath.Text = "C:\\2.xls";
            SettingConfigFlag();
        }

        private void SettingConfigFlag()
        {
            IsFinishConfig = File.Exists(GetXmlPath()) && File.Exists(GetMapPath());
        }

        public bool IsFinishConfig { get; set; }

        private void btn_setting_Click(object sender, EventArgs e)
        {
            OpenSetting();
        }

        private void OpenSetting()
        {
            string path = tbPath.Text;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                OpenSettingDig();
                SettingConfigFlag();
            }
        }
        private void OpenSettingDig()
        {
            var list = SrcInfos.GroupBy(x => x.技能组).ToDictionary(x => x.Key, x => x.ToList());
            new SettingGroups(list.Select(x => x.Key).ToList()).ShowDialog();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StatsisLib;

namespace StatsicForXX
{
    public partial class SettingGroups : Form
    {
        public SettingGroups()
        {
            InitializeComponent();
        }

        public SettingGroups(List<string> groups)
        {
            InitializeComponent();
            this.GroupsList = groups;
        }

        private void InitGroups(List<string> groups)
        {
            groups.Sort();
            groups.ForEach(x =>
            {

                var info = mapperInfos.FirstOrDefault(y => y.Key == x);
                if (info == null)
                {
                    info = new MappInfo() { Key = x, Value = x };
                }
                lv_groups.Items.Add(new ListViewItem()
                {
                    Text = info.Value,
                    Tag = info,
                    ToolTipText = info.Key
                });
            });
        }
        List<string> GroupsList;
        public StatsisLib.NestDirectory NDirectory { get; set; }
        public List<MappInfo> mapperInfos { get; set; }

        private void SettingGroups_Load(object sender, EventArgs e)
        {
            string path = GetXmlPath();
            if (File.Exists(path))
            {
                NDirectory = StatsisLib.NestDirectory.Deserialize(path);
            }
            else
            {
                NDirectory = new StatsisLib.NestDirectory() { Name = "全部", OrderIndex = 1 };
            }
            InitTree(NDirectory);

            mapperInfos = new List<MappInfo>();
            InitMapper();
        }

        private void InitMapper()
        {
            string path = GetMapPath();
            if (File.Exists(path))
            {
                mapperInfos = MappInfo.Deserialize(path);
            }
            InitGroups(GroupsList);
        }

        private static string GetXmlPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "group.xml");
        }

        private void InitTree(NestDirectory nDirectory)
        {
            var node = new TreeNode() { Text = nDirectory.Name, Tag = nDirectory };
            tv_nds.Nodes.Add(node);

            //foreach (var item in nDirectory.Children)
            //{
            //    var node = new TreeNode() { Text = item.Name, Tag = item };
            //    tv_nds.Nodes.Add(node);
            //    LoadTreeNode(item, node);
            //}
        }

        private void LoadTreeNode(NestDirectory nDirectory, TreeNode node)
        {
            node.Nodes.Clear();
            foreach (var item in nDirectory.Children.OrderBy(x => x.OrderIndex).ToList())
            {
                string name = item.Name;
                if (string.IsNullOrWhiteSpace(item.NickName))
                {
                    name = string.Format("{0}{1}", item.Name, "--"+item.NickName);
                }
                var chiildNode = new TreeNode() { Text = name, Tag = item };
                node.Nodes.Add(chiildNode);
                LoadTreeNode(item, chiildNode);
            }
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            var node = tv_nds.SelectedNode;
            if (node == null)
            {
                return;
            }



            string nodeName = "";// tbName.Text;
            NestDirectory dir = new NestDirectory();
            dir.Name = nodeName;
            AddChildNode(node, dir);
        }

        private void AddChildNode(TreeNode node, NestDirectory dir)
        {

            dir.OrderIndex = node.GetNodeCount(false) + 1;
            var parentDir = node.Tag as NestDirectory;
            if (parentDir.Children.Exists(x => x.Name == dir.Name))
            {
                return;
            }
            parentDir.Children.Add(dir);
            node.Tag = parentDir;
            RefreshNode(node);
        }

        private void RefreshNode(TreeNode node)
        {
            var parentDir = node.Tag as NestDirectory;
            LoadTreeNode(parentDir, node);
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string path = GetXmlPath();
            if (File.Exists(path))
            {
                File.Move(path, string.Format("{0}_{1}.bak", path, DateTime.Now.ToString().Replace(":", "_").Replace("-", "_")));
            }
            NDirectory.Serialize(path);



            string mapPath = GetMapPath();
            if (File.Exists(mapPath))
            {
                File.Move(mapPath, string.Format("{0}_{1}.bak", mapPath, DateTime.Now.ToString().Replace(":", "_").Replace("-", "_")));
            }
            mapperInfos.Clear();
            foreach (ListViewItem item in lv_groups.Items)
            {
                MappInfo info = item.Tag as MappInfo;
                if (info.Key != info.Value)
                {
                    mapperInfos.Add(info);
                }
            }
            MappInfo.Serialize(mapPath, mapperInfos);
        }

        private string GetMapPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "mapper.xml");
        }

        private void btn_Remove_Click(object sender, EventArgs e)
        {
            var node = tv_nds.SelectedNode;

            RemoveNode(node);
        }

        private void RemoveNode(TreeNode node)
        {
            var parentNode = node.Parent;
            if (parentNode == null)
            {
                return;
            }
            var parentDir = parentNode.Tag as NestDirectory;
            parentDir.Children.Remove(parentDir.Children.FirstOrDefault(x => x.Name == node.Text));
            int index = 1;
            foreach (var item in parentDir.Children)
            {
                item.OrderIndex = index++;
            }
            parentNode.Tag = parentDir;
            RefreshNode(parentNode);
        }

        private void btn_up_Click(object sender, EventArgs e)
        {
            UpNode();
        }

        private void UpNode()
        {
            var node = tv_nds.SelectedNode;

            var parentNode = node.Parent;
            if (parentNode == null)
            {
                return;
            }
            var parentDir = parentNode.Tag as NestDirectory;
            var currentDir = parentDir.Children.FirstOrDefault(x => x.Name == node.Text);
            if (currentDir.OrderIndex <= 1)
            {
                return;
            }
            var otherDir = parentDir.Children.FirstOrDefault(x => x.OrderIndex == currentDir.OrderIndex - 1);
            if (otherDir != null)
            {
                otherDir.OrderIndex += 1;
                currentDir.OrderIndex -= 1;
            }
            parentNode.Tag = parentDir;
            RefreshNode(parentNode);
        }

        private void btn_Down_Click(object sender, EventArgs e)
        {
            DownNode();
        }

        private void DownNode()
        {
            var node = tv_nds.SelectedNode;

            var parentNode = node.Parent;
            if (parentNode == null)
            {
                return;
            }
            var parentDir = parentNode.Tag as NestDirectory;
            var currentDir = parentDir.Children.FirstOrDefault(x => x.Name == node.Text);
            if (currentDir.OrderIndex >= parentDir.Children.Count)
            {
                return;
            }
            var otherDir = parentDir.Children.FirstOrDefault(x => x.OrderIndex == currentDir.OrderIndex + 1);
            if (otherDir != null)
            {
                otherDir.OrderIndex -= 1;
                currentDir.OrderIndex += 1;
            }
            parentNode.Tag = parentDir;
            RefreshNode(parentNode);
        }

        private void tv_nds_DragDrop(object sender, DragEventArgs e)
        {
            Point p = tv_nds.PointToClient(new Point(e.X, e.Y));
            TreeNode node = tv_nds.GetNodeAt(p);
            if (node != null)
            {
                if (lv_groups.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in lv_groups.SelectedItems)
                    {
                        AddChildNode(node, new NestDirectory
                        {
                            Name = item.Text
                        });
                    }
                }
                else if (tv_nds.SelectedNode != null)
                {
                    var oldNode = tv_nds.SelectedNode;

                    var parentNode = node.Parent;
                    if (parentNode == null || parentNode == node)
                    {
                        return;
                    }

                    AddChildNode(node, oldNode.Tag as NestDirectory);
                    RemoveNode(oldNode);
                }
            }
        }

        private void tv_nds_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void tv_nds_ItemDrag(object sender, ItemDragEventArgs e)
        {

            //if (e.Item is TreeNode && e.Button == System.Windows.Forms.MouseButtons.Left &&
            //    e.Item != null && sender is TreeView)
            //{
            //    TreeView trv = sender as TreeView;
            //    TreeNode node = e.Item as TreeNode;
            //    if (node.Parent != null && trv.Tag != null)
            //    {
            tv_nds.DoDragDrop(e.Item, DragDropEffects.Move);
            //    }
            //}
        }

        private void lv_groups_ItemDrag(object sender, ItemDragEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Left &&
            //    e.Item != null)
            //{
            lv_groups.DoDragDrop(e.Item, DragDropEffects.Move);
            //        trv.DoDragDrop(node, DragDropEffects.Move);

            //}
        }

        private void tv_m_Add_Click(object sender, EventArgs e)
        {
            if (tv_nds.SelectedNode != null)
            {
                AddChildNode(tv_nds.SelectedNode, new NestDirectory()
                {
                    Name = "未命名" + tv_nds.SelectedNode.Nodes.Count,
                });
                tv_nds.SelectedNode.Expand();
                tv_nds.SelectedNode.Nodes[tv_nds.SelectedNode.Nodes.Count - 1].BeginEdit();
            }
        }

        private void tv_m_Alter_Click(object sender, EventArgs e)
        {
            if (tv_nds.SelectedNode != null)
            {
                tv_nds.SelectedNode.BeginEdit();
            }
        }

        private void tv_m_Up_Click(object sender, EventArgs e)
        {
            UpNode();
        }

        private void tv_m_Down_Click(object sender, EventArgs e)
        {
            DownNode();
        }

        private void tv_m_Del_Click(object sender, EventArgs e)
        {
            if (tv_nds.SelectedNode != null)
            {
                RemoveNode(tv_nds.SelectedNode);
            }

        }

        private void tv_nds_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (e.Label.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
                    {
                        // Stop editing without canceling the label change.
                        e.Node.EndEdit(false);

                        var dir = e.Node.Tag as NestDirectory;
                        dir.Name = e.Label;
                        e.Node.Tag = dir;
                        if (e.Node.Parent != null)
                        {
                            var pDir = e.Node.Parent.Tag as NestDirectory;
                            var cDir = pDir.Children.FirstOrDefault(x => x.OrderIndex == dir.OrderIndex);
                            cDir.Name = e.Label;
                            e.Node.Parent.Tag = pDir;
                        }
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("名称不能包含无效字符.\n" +
                           "如: '@','.', ',', '!'",
                           "提示");
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show("名称不能为空",
                       "提示");
                    e.Node.BeginEdit();
                }
            }
        }

        private void lv_m_map_Click(object sender, EventArgs e)
        {
            if (lv_groups.SelectedItems.Count == 1)
            {
                lv_groups.SelectedItems[0].BeginEdit();
            }
        }

        private void lv_groups_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (e.Label.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
                    {
                        // Stop editing without canceling the label change.
                        //lv_groups.Items[e.Item].e

                        var dir = lv_groups.Items[e.Item].Tag as MappInfo;
                        dir.Value = e.Label;
                        lv_groups.Items[e.Item].Tag = dir;
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("名称不能包含无效字符.\n" +
                           "如: '@','.', ',', '!'",
                           "提示");
                        lv_groups.Items[e.Item].BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show("名称不能为空",
                       "提示");
                    lv_groups.Items[e.Item].BeginEdit();
                }
            }
        }

        private void lv_m_unmap_Click(object sender, EventArgs e)
        {
            var dir = lv_groups.SelectedItems[0].Tag as MappInfo;
            lv_groups.SelectedItems[0].Text = dir.Value = dir.Key;
        }

        private void tv_m_Copy_Click(object sender, EventArgs e)
        {
            var node = tv_nds.SelectedNode;
            StringBuilder str = new StringBuilder();
            foreach (TreeNode item in node.Nodes)
            {
                str.AppendFormat("{0},", item.Text);
            }
            Clipboard.SetText(str.ToString().TrimEnd(','));
        }
    }
}

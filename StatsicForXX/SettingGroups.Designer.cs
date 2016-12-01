namespace StatsicForXX
{
    partial class SettingGroups
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tv_nds = new System.Windows.Forms.TreeView();
            this.lv_groups = new System.Windows.Forms.ListView();
            this.btn_Save = new System.Windows.Forms.Button();
            this.tv_m = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tv_m_Add = new System.Windows.Forms.ToolStripMenuItem();
            this.tv_m_Alter = new System.Windows.Forms.ToolStripMenuItem();
            this.tv_m_Up = new System.Windows.Forms.ToolStripMenuItem();
            this.tv_m_Down = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tv_m_Del = new System.Windows.Forms.ToolStripMenuItem();
            this.lv_m = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lv_m_map = new System.Windows.Forms.ToolStripMenuItem();
            this.lv_m_unmap = new System.Windows.Forms.ToolStripMenuItem();
            this.tv_m.SuspendLayout();
            this.lv_m.SuspendLayout();
            this.SuspendLayout();
            // 
            // tv_nds
            // 
            this.tv_nds.AllowDrop = true;
            this.tv_nds.ContextMenuStrip = this.tv_m;
            this.tv_nds.LabelEdit = true;
            this.tv_nds.Location = new System.Drawing.Point(12, 12);
            this.tv_nds.Name = "tv_nds";
            this.tv_nds.Size = new System.Drawing.Size(264, 408);
            this.tv_nds.TabIndex = 0;
            this.tv_nds.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tv_nds_AfterLabelEdit);
            this.tv_nds.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tv_nds_ItemDrag);
            this.tv_nds.DragDrop += new System.Windows.Forms.DragEventHandler(this.tv_nds_DragDrop);
            this.tv_nds.DragEnter += new System.Windows.Forms.DragEventHandler(this.tv_nds_DragEnter);
            // 
            // lv_groups
            // 
            this.lv_groups.AllowDrop = true;
            this.lv_groups.ContextMenuStrip = this.lv_m;
            this.lv_groups.LabelEdit = true;
            this.lv_groups.Location = new System.Drawing.Point(314, 12);
            this.lv_groups.Name = "lv_groups";
            this.lv_groups.Size = new System.Drawing.Size(141, 195);
            this.lv_groups.TabIndex = 1;
            this.lv_groups.UseCompatibleStateImageBehavior = false;
            this.lv_groups.View = System.Windows.Forms.View.Tile;
            this.lv_groups.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lv_groups_AfterLabelEdit);
            this.lv_groups.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lv_groups_ItemDrag);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(338, 311);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(95, 100);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // tv_m
            // 
            this.tv_m.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tv_m_Add,
            this.tv_m_Alter,
            this.tv_m_Del,
            this.toolStripSeparator1,
            this.tv_m_Up,
            this.tv_m_Down});
            this.tv_m.Name = "tv_m";
            this.tv_m.Size = new System.Drawing.Size(101, 120);
            // 
            // tv_m_Add
            // 
            this.tv_m_Add.Name = "tv_m_Add";
            this.tv_m_Add.Size = new System.Drawing.Size(100, 22);
            this.tv_m_Add.Text = "添加";
            this.tv_m_Add.Click += new System.EventHandler(this.tv_m_Add_Click);
            // 
            // tv_m_Alter
            // 
            this.tv_m_Alter.Name = "tv_m_Alter";
            this.tv_m_Alter.Size = new System.Drawing.Size(100, 22);
            this.tv_m_Alter.Text = "编辑";
            this.tv_m_Alter.Click += new System.EventHandler(this.tv_m_Alter_Click);
            // 
            // tv_m_Up
            // 
            this.tv_m_Up.Name = "tv_m_Up";
            this.tv_m_Up.Size = new System.Drawing.Size(100, 22);
            this.tv_m_Up.Text = "向上";
            this.tv_m_Up.Click += new System.EventHandler(this.tv_m_Up_Click);
            // 
            // tv_m_Down
            // 
            this.tv_m_Down.Name = "tv_m_Down";
            this.tv_m_Down.Size = new System.Drawing.Size(100, 22);
            this.tv_m_Down.Text = "向下";
            this.tv_m_Down.Click += new System.EventHandler(this.tv_m_Down_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(97, 6);
            // 
            // tv_m_Del
            // 
            this.tv_m_Del.Name = "tv_m_Del";
            this.tv_m_Del.Size = new System.Drawing.Size(100, 22);
            this.tv_m_Del.Text = "删除";
            this.tv_m_Del.Click += new System.EventHandler(this.tv_m_Del_Click);
            // 
            // lv_m
            // 
            this.lv_m.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lv_m_map,
            this.lv_m_unmap});
            this.lv_m.Name = "lv_m";
            this.lv_m.Size = new System.Drawing.Size(153, 70);
            // 
            // lv_m_map
            // 
            this.lv_m_map.Name = "lv_m_map";
            this.lv_m_map.Size = new System.Drawing.Size(152, 22);
            this.lv_m_map.Text = "映射";
            this.lv_m_map.Click += new System.EventHandler(this.lv_m_map_Click);
            // 
            // lv_m_unmap
            // 
            this.lv_m_unmap.Name = "lv_m_unmap";
            this.lv_m_unmap.Size = new System.Drawing.Size(152, 22);
            this.lv_m_unmap.Text = "取消映射";
            this.lv_m_unmap.Click += new System.EventHandler(this.lv_m_unmap_Click);
            // 
            // SettingGroups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 457);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.lv_groups);
            this.Controls.Add(this.tv_nds);
            this.Name = "SettingGroups";
            this.Text = "SettingGroups";
            this.Load += new System.EventHandler(this.SettingGroups_Load);
            this.tv_m.ResumeLayout(false);
            this.lv_m.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tv_nds;
        private System.Windows.Forms.ListView lv_groups;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.ContextMenuStrip tv_m;
        private System.Windows.Forms.ToolStripMenuItem tv_m_Add;
        private System.Windows.Forms.ToolStripMenuItem tv_m_Alter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tv_m_Up;
        private System.Windows.Forms.ToolStripMenuItem tv_m_Down;
        private System.Windows.Forms.ToolStripMenuItem tv_m_Del;
        private System.Windows.Forms.ContextMenuStrip lv_m;
        private System.Windows.Forms.ToolStripMenuItem lv_m_map;
        private System.Windows.Forms.ToolStripMenuItem lv_m_unmap;
    }
}
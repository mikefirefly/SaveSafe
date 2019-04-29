namespace SaveSafeRemake
{
    partial class fMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMainForm));
            this.lbFolderList = new System.Windows.Forms.ListBox();
            this.mnuPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.activeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSaveBackupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performManualBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreFromSpecificBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.browseTargetFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browseSaveBackupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPopup.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbFolderList
            // 
            this.lbFolderList.BackColor = System.Drawing.Color.Linen;
            this.lbFolderList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbFolderList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbFolderList.Font = new System.Drawing.Font("Adelle Sans SemiBold", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbFolderList.ForeColor = System.Drawing.Color.Black;
            this.lbFolderList.FormattingEnabled = true;
            this.lbFolderList.ItemHeight = 30;
            this.lbFolderList.Location = new System.Drawing.Point(0, 0);
            this.lbFolderList.Name = "lbFolderList";
            this.lbFolderList.Size = new System.Drawing.Size(495, 111);
            this.lbFolderList.TabIndex = 0;
            this.lbFolderList.DoubleClick += new System.EventHandler(this.lbFolderList_DoubleClick);
            this.lbFolderList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbFolderList_MouseDown);
            // 
            // mnuPopup
            // 
            this.mnuPopup.Font = new System.Drawing.Font("Adelle Sans", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.mnuPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activeToolStripMenuItem,
            this.deleteSaveBackupsToolStripMenuItem,
            this.performManualBackupToolStripMenuItem,
            this.restoreFromSpecificBackupToolStripMenuItem,
            this.removeEntryToolStripMenuItem,
            this.toolStripSeparator1,
            this.browseTargetFolderToolStripMenuItem,
            this.informationToolStripMenuItem,
            this.browseSaveBackupsToolStripMenuItem});
            this.mnuPopup.Name = "mnuPopup";
            this.mnuPopup.Size = new System.Drawing.Size(278, 240);
            // 
            // activeToolStripMenuItem
            // 
            this.activeToolStripMenuItem.Checked = true;
            this.activeToolStripMenuItem.CheckOnClick = true;
            this.activeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.activeToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.activeToolStripMenuItem.Name = "activeToolStripMenuItem";
            this.activeToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.activeToolStripMenuItem.Text = "Monitoring active";
            this.activeToolStripMenuItem.Click += new System.EventHandler(this.activeToolStripMenuItem_Click);
            // 
            // deleteSaveBackupsToolStripMenuItem
            // 
            this.deleteSaveBackupsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteSaveBackupsToolStripMenuItem.Image")));
            this.deleteSaveBackupsToolStripMenuItem.Name = "deleteSaveBackupsToolStripMenuItem";
            this.deleteSaveBackupsToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.deleteSaveBackupsToolStripMenuItem.Text = "Delete backups for this entry";
            this.deleteSaveBackupsToolStripMenuItem.Click += new System.EventHandler(this.deleteSaveBackupsToolStripMenuItem_Click);
            // 
            // performManualBackupToolStripMenuItem
            // 
            this.performManualBackupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("performManualBackupToolStripMenuItem.Image")));
            this.performManualBackupToolStripMenuItem.Name = "performManualBackupToolStripMenuItem";
            this.performManualBackupToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.performManualBackupToolStripMenuItem.Text = "Perform manual backup";
            this.performManualBackupToolStripMenuItem.Click += new System.EventHandler(this.performManualBackupToolStripMenuItem_Click);
            // 
            // restoreFromSpecificBackupToolStripMenuItem
            // 
            this.restoreFromSpecificBackupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("restoreFromSpecificBackupToolStripMenuItem.Image")));
            this.restoreFromSpecificBackupToolStripMenuItem.Name = "restoreFromSpecificBackupToolStripMenuItem";
            this.restoreFromSpecificBackupToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.restoreFromSpecificBackupToolStripMenuItem.Text = "Restore from specific backup";
            this.restoreFromSpecificBackupToolStripMenuItem.Click += new System.EventHandler(this.restoreFromSpecificBackupToolStripMenuItem_Click);
            // 
            // removeEntryToolStripMenuItem
            // 
            this.removeEntryToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeEntryToolStripMenuItem.Image")));
            this.removeEntryToolStripMenuItem.Name = "removeEntryToolStripMenuItem";
            this.removeEntryToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.removeEntryToolStripMenuItem.Text = "Remove entry from list";
            this.removeEntryToolStripMenuItem.Click += new System.EventHandler(this.removeEntryToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(274, 6);
            // 
            // browseTargetFolderToolStripMenuItem
            // 
            this.browseTargetFolderToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("browseTargetFolderToolStripMenuItem.Image")));
            this.browseTargetFolderToolStripMenuItem.Name = "browseTargetFolderToolStripMenuItem";
            this.browseTargetFolderToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.browseTargetFolderToolStripMenuItem.Text = "Browse entry folder";
            this.browseTargetFolderToolStripMenuItem.Click += new System.EventHandler(this.browseTargetFolderToolStripMenuItem_Click);
            // 
            // informationToolStripMenuItem
            // 
            this.informationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("informationToolStripMenuItem.Image")));
            this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            this.informationToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.informationToolStripMenuItem.Text = "Entry information";
            this.informationToolStripMenuItem.Click += new System.EventHandler(this.informationToolStripMenuItem_Click);
            // 
            // browseSaveBackupsToolStripMenuItem
            // 
            this.browseSaveBackupsToolStripMenuItem.Font = new System.Drawing.Font("Adelle Sans", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.browseSaveBackupsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("browseSaveBackupsToolStripMenuItem.Image")));
            this.browseSaveBackupsToolStripMenuItem.Name = "browseSaveBackupsToolStripMenuItem";
            this.browseSaveBackupsToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.browseSaveBackupsToolStripMenuItem.Text = "Browse backup repository";
            this.browseSaveBackupsToolStripMenuItem.Click += new System.EventHandler(this.browseSaveBackupsToolStripMenuItem_Click);
            // 
            // fMainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(495, 111);
            this.Controls.Add(this.lbFolderList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "fMainForm";
            this.Text = "SaveSafe 1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fMainForm_FormClosing);
            this.Load += new System.EventHandler(this.fMainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.fMainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.fMainForm_DragEnter);
            this.mnuPopup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbFolderList;
        private System.Windows.Forms.ContextMenuStrip mnuPopup;
        private System.Windows.Forms.ToolStripMenuItem activeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSaveBackupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseSaveBackupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseTargetFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performManualBackupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreFromSpecificBackupToolStripMenuItem;
    }
}


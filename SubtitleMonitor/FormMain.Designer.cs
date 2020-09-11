﻿namespace SubtitleMonitor
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonTestFile1 = new System.Windows.Forms.Button();
            this.buttonTestFile2 = new System.Windows.Forms.Button();
            this.treeViewMain = new System.Windows.Forms.TreeView();
            this.splitContainerMaIN = new System.Windows.Forms.SplitContainer();
            this.splitContainerDetails = new System.Windows.Forms.SplitContainer();
            this.listViewDetails = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelDescripton = new System.Windows.Forms.Panel();
            this.textBoxDesc = new System.Windows.Forms.TextBox();
            this.progressBarLoading = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
            this.checkBoxCreateHTML = new System.Windows.Forms.CheckBox();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.panelAbout = new System.Windows.Forms.Panel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxSubs = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMaIN)).BeginInit();
            this.splitContainerMaIN.Panel1.SuspendLayout();
            this.splitContainerMaIN.Panel2.SuspendLayout();
            this.splitContainerMaIN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetails)).BeginInit();
            this.splitContainerDetails.Panel1.SuspendLayout();
            this.splitContainerDetails.Panel2.SuspendLayout();
            this.splitContainerDetails.SuspendLayout();
            this.panelDescripton.SuspendLayout();
            this.panelAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSubs)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonTestFile1
            // 
            this.buttonTestFile1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTestFile1.Location = new System.Drawing.Point(610, 41);
            this.buttonTestFile1.Name = "buttonTestFile1";
            this.buttonTestFile1.Size = new System.Drawing.Size(218, 23);
            this.buttonTestFile1.TabIndex = 2;
            this.buttonTestFile1.Text = "Test File 1";
            this.buttonTestFile1.UseVisualStyleBackColor = true;
            this.buttonTestFile1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonTestFile2
            // 
            this.buttonTestFile2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTestFile2.Location = new System.Drawing.Point(834, 41);
            this.buttonTestFile2.Name = "buttonTestFile2";
            this.buttonTestFile2.Size = new System.Drawing.Size(218, 23);
            this.buttonTestFile2.TabIndex = 3;
            this.buttonTestFile2.Text = "Test File 2";
            this.buttonTestFile2.UseVisualStyleBackColor = true;
            this.buttonTestFile2.Click += new System.EventHandler(this.button2_Click);
            // 
            // treeViewMain
            // 
            this.treeViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewMain.HideSelection = false;
            this.treeViewMain.Location = new System.Drawing.Point(0, 0);
            this.treeViewMain.Name = "treeViewMain";
            this.treeViewMain.Size = new System.Drawing.Size(346, 464);
            this.treeViewMain.TabIndex = 2;
            this.treeViewMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewMain_AfterSelect);
            // 
            // splitContainerMaIN
            // 
            this.splitContainerMaIN.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMaIN.Location = new System.Drawing.Point(12, 70);
            this.splitContainerMaIN.Name = "splitContainerMaIN";
            // 
            // splitContainerMaIN.Panel1
            // 
            this.splitContainerMaIN.Panel1.Controls.Add(this.treeViewMain);
            // 
            // splitContainerMaIN.Panel2
            // 
            this.splitContainerMaIN.Panel2.Controls.Add(this.splitContainerDetails);
            this.splitContainerMaIN.Size = new System.Drawing.Size(1040, 464);
            this.splitContainerMaIN.SplitterDistance = 346;
            this.splitContainerMaIN.TabIndex = 3;
            this.splitContainerMaIN.Visible = false;
            // 
            // splitContainerDetails
            // 
            this.splitContainerDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDetails.Location = new System.Drawing.Point(0, 0);
            this.splitContainerDetails.Name = "splitContainerDetails";
            this.splitContainerDetails.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerDetails.Panel1
            // 
            this.splitContainerDetails.Panel1.Controls.Add(this.listViewDetails);
            // 
            // splitContainerDetails.Panel2
            // 
            this.splitContainerDetails.Panel2.Controls.Add(this.panelDescripton);
            this.splitContainerDetails.Size = new System.Drawing.Size(690, 464);
            this.splitContainerDetails.SplitterDistance = 309;
            this.splitContainerDetails.TabIndex = 2;
            // 
            // listViewDetails
            // 
            this.listViewDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDetails.FullRowSelect = true;
            this.listViewDetails.HideSelection = false;
            this.listViewDetails.Location = new System.Drawing.Point(0, 0);
            this.listViewDetails.Name = "listViewDetails";
            this.listViewDetails.ShowItemToolTips = true;
            this.listViewDetails.Size = new System.Drawing.Size(690, 309);
            this.listViewDetails.TabIndex = 0;
            this.listViewDetails.UseCompatibleStateImageBehavior = false;
            this.listViewDetails.View = System.Windows.Forms.View.Details;
            this.listViewDetails.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewDetails_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Parameter";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Information";
            this.columnHeader3.Width = 300;
            // 
            // panelDescripton
            // 
            this.panelDescripton.Controls.Add(this.pictureBoxSubs);
            this.panelDescripton.Controls.Add(this.textBoxDesc);
            this.panelDescripton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDescripton.Location = new System.Drawing.Point(0, 0);
            this.panelDescripton.Name = "panelDescripton";
            this.panelDescripton.Size = new System.Drawing.Size(690, 151);
            this.panelDescripton.TabIndex = 1;
            // 
            // textBoxDesc
            // 
            this.textBoxDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDesc.Location = new System.Drawing.Point(0, 0);
            this.textBoxDesc.Multiline = true;
            this.textBoxDesc.Name = "textBoxDesc";
            this.textBoxDesc.Size = new System.Drawing.Size(690, 151);
            this.textBoxDesc.TabIndex = 0;
            // 
            // progressBarLoading
            // 
            this.progressBarLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarLoading.Location = new System.Drawing.Point(258, 12);
            this.progressBarLoading.Name = "progressBarLoading";
            this.progressBarLoading.Size = new System.Drawing.Size(794, 23);
            this.progressBarLoading.TabIndex = 4;
            // 
            // backgroundWorkerMain
            // 
            this.backgroundWorkerMain.WorkerReportsProgress = true;
            this.backgroundWorkerMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMain_DoWork);
            this.backgroundWorkerMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerMain_ProgressChanged);
            this.backgroundWorkerMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerMain_RunWorkerCompleted);
            // 
            // checkBoxCreateHTML
            // 
            this.checkBoxCreateHTML.AutoSize = true;
            this.checkBoxCreateHTML.Location = new System.Drawing.Point(258, 45);
            this.checkBoxCreateHTML.Name = "checkBoxCreateHTML";
            this.checkBoxCreateHTML.Size = new System.Drawing.Size(114, 17);
            this.checkBoxCreateHTML.TabIndex = 1;
            this.checkBoxCreateHTML.Text = "Create HTML Files";
            this.checkBoxCreateHTML.UseVisualStyleBackColor = true;
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(12, 12);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(240, 23);
            this.buttonOpenFile.TabIndex = 0;
            this.buttonOpenFile.Text = "Open File";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // panelAbout
            // 
            this.panelAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAbout.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAbout.Controls.Add(this.linkLabel2);
            this.panelAbout.Controls.Add(this.label3);
            this.panelAbout.Controls.Add(this.linkLabel1);
            this.panelAbout.Controls.Add(this.label2);
            this.panelAbout.Controls.Add(this.label1);
            this.panelAbout.Location = new System.Drawing.Point(12, 70);
            this.panelAbout.Name = "panelAbout";
            this.panelAbout.Size = new System.Drawing.Size(1040, 464);
            this.panelAbout.TabIndex = 7;
            // 
            // linkLabel2
            // 
            this.linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel2.Location = new System.Drawing.Point(3, 220);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(1032, 23);
            this.linkLabel2.TabIndex = 4;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "https://dvb.org/wp-content/uploads/2019/12/a009_dvb_bitmap_subtitles_nov_2017.pdf" +
    "";
            this.linkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(346, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(348, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "The DVB subtitle specification can be found on the DVB website;";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.Location = new System.Drawing.Point(349, 127);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(345, 23);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://www.nikse.dk/";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(346, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(348, 70);
            this.label2.TabIndex = 1;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(-1, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1040, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "DVB Subtitle Explorer";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBoxSubs
            // 
            this.pictureBoxSubs.BackColor = System.Drawing.SystemColors.ControlText;
            this.pictureBoxSubs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxSubs.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxSubs.Name = "pictureBoxSubs";
            this.pictureBoxSubs.Size = new System.Drawing.Size(690, 151);
            this.pictureBoxSubs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxSubs.TabIndex = 1;
            this.pictureBoxSubs.TabStop = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 546);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.checkBoxCreateHTML);
            this.Controls.Add(this.progressBarLoading);
            this.Controls.Add(this.splitContainerMaIN);
            this.Controls.Add(this.buttonTestFile2);
            this.Controls.Add(this.buttonTestFile1);
            this.Controls.Add(this.panelAbout);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DVB Subtitle Explorer";
            this.splitContainerMaIN.Panel1.ResumeLayout(false);
            this.splitContainerMaIN.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMaIN)).EndInit();
            this.splitContainerMaIN.ResumeLayout(false);
            this.splitContainerDetails.Panel1.ResumeLayout(false);
            this.splitContainerDetails.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetails)).EndInit();
            this.splitContainerDetails.ResumeLayout(false);
            this.panelDescripton.ResumeLayout(false);
            this.panelDescripton.PerformLayout();
            this.panelAbout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSubs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTestFile1;
        private System.Windows.Forms.Button buttonTestFile2;
        private System.Windows.Forms.TreeView treeViewMain;
        private System.Windows.Forms.SplitContainer splitContainerMaIN;
        private System.Windows.Forms.ListView listViewDetails;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ProgressBar progressBarLoading;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
        private System.Windows.Forms.Panel panelDescripton;
        private System.Windows.Forms.TextBox textBoxDesc;
        private System.Windows.Forms.SplitContainer splitContainerDetails;
        private System.Windows.Forms.CheckBox checkBoxCreateHTML;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.Panel panelAbout;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxSubs;
    }
}


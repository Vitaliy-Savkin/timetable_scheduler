namespace SchedulerProject.UserInterface
{
    partial class MainForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miLoadData = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miOpenTimeTable = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveTimeTable = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.miExport = new System.Windows.Forms.ToolStripMenuItem();
            this.miShedule = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditData = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditGroups = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditSubjects = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditLecturers = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditRooms = new System.Windows.Forms.ToolStripMenuItem();
            this.miEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miEditTimeTable = new System.Windows.Forms.ToolStripMenuItem();
            this.miOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.miSchedulingParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.справкаToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.timeTablesList = new System.Windows.Forms.ListView();
            this.createdTimeTablesHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timeTableDetails = new System.Windows.Forms.TabControl();
            this.tabByGroups = new System.Windows.Forms.TabPage();
            this.lblGroups = new System.Windows.Forms.Label();
            this.cbxGroups = new System.Windows.Forms.ComboBox();
            this.tabByLecturers = new System.Windows.Forms.TabPage();
            this.lblLecturer = new System.Windows.Forms.Label();
            this.cbxLecturers = new System.Windows.Forms.ComboBox();
            this.openDataFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveDataFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.timeTableDetails.SuspendLayout();
            this.tabByGroups.SuspendLayout();
            this.tabByLecturers.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miEdit,
            this.miOptions,
            this.miHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(471, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLoadData,
            this.miSaveData,
            this.toolStripSeparator3,
            this.miOpenTimeTable,
            this.miSaveTimeTable,
            this.toolStripSeparator4,
            this.miExport,
            this.miShedule,
            this.toolStripSeparator5,
            this.miExit});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(48, 20);
            this.miFile.Text = "Файл";
            // 
            // miLoadData
            // 
            this.miLoadData.Name = "miLoadData";
            this.miLoadData.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.miLoadData.Size = new System.Drawing.Size(272, 22);
            this.miLoadData.Text = "Загрузить данные";
            this.miLoadData.Click += new System.EventHandler(this.miLoadData_Click);
            // 
            // miSaveData
            // 
            this.miSaveData.Name = "miSaveData";
            this.miSaveData.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miSaveData.Size = new System.Drawing.Size(272, 22);
            this.miSaveData.Text = "Сохранить данные";
            this.miSaveData.Click += new System.EventHandler(this.miSaveData_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(269, 6);
            // 
            // miOpenTimeTable
            // 
            this.miOpenTimeTable.Name = "miOpenTimeTable";
            this.miOpenTimeTable.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.miOpenTimeTable.Size = new System.Drawing.Size(272, 22);
            this.miOpenTimeTable.Text = "Открыть расписание";
            this.miOpenTimeTable.Click += new System.EventHandler(this.miOpenTimeTable_Click);
            // 
            // miSaveTimeTable
            // 
            this.miSaveTimeTable.Name = "miSaveTimeTable";
            this.miSaveTimeTable.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.miSaveTimeTable.Size = new System.Drawing.Size(272, 22);
            this.miSaveTimeTable.Text = "Сохранить расписание";
            this.miSaveTimeTable.Click += new System.EventHandler(this.miSaveTimeTable_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(269, 6);
            // 
            // miExport
            // 
            this.miExport.Name = "miExport";
            this.miExport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.miExport.Size = new System.Drawing.Size(272, 22);
            this.miExport.Text = "Экспорт";
            // 
            // miShedule
            // 
            this.miShedule.Name = "miShedule";
            this.miShedule.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.miShedule.Size = new System.Drawing.Size(272, 22);
            this.miShedule.Text = "Составить расписание";
            this.miShedule.Click += new System.EventHandler(this.miShedule_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(269, 6);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.miExit.Size = new System.Drawing.Size(272, 22);
            this.miExit.Text = "Выход";
            // 
            // miEdit
            // 
            this.miEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miEditData,
            this.toolStripSeparator1,
            this.miEditTimeTable});
            this.miEdit.Name = "miEdit";
            this.miEdit.Size = new System.Drawing.Size(59, 20);
            this.miEdit.Text = "Правка";
            // 
            // miEditData
            // 
            this.miEditData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miEditGroups,
            this.miEditSubjects,
            this.miEditLecturers,
            this.miEditRooms,
            this.miEventsToolStripMenuItem});
            this.miEditData.Name = "miEditData";
            this.miEditData.Size = new System.Drawing.Size(176, 22);
            this.miEditData.Text = "Данные";
            // 
            // miEditGroups
            // 
            this.miEditGroups.Name = "miEditGroups";
            this.miEditGroups.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.G)));
            this.miEditGroups.Size = new System.Drawing.Size(195, 22);
            this.miEditGroups.Text = "Группы";
            this.miEditGroups.Click += new System.EventHandler(this.miEditGroups_Click);
            // 
            // miEditSubjects
            // 
            this.miEditSubjects.Name = "miEditSubjects";
            this.miEditSubjects.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.miEditSubjects.Size = new System.Drawing.Size(195, 22);
            this.miEditSubjects.Text = "Предметы";
            this.miEditSubjects.Click += new System.EventHandler(this.miEditSubjects_Click);
            // 
            // miEditLecturers
            // 
            this.miEditLecturers.Name = "miEditLecturers";
            this.miEditLecturers.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.miEditLecturers.Size = new System.Drawing.Size(195, 22);
            this.miEditLecturers.Text = "Преподаватели";
            this.miEditLecturers.Click += new System.EventHandler(this.miEditLecturers_Click);
            // 
            // miEditRooms
            // 
            this.miEditRooms.Name = "miEditRooms";
            this.miEditRooms.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.miEditRooms.Size = new System.Drawing.Size(195, 22);
            this.miEditRooms.Text = "Аудитории";
            this.miEditRooms.Click += new System.EventHandler(this.miEditRooms_Click);
            // 
            // miEventsToolStripMenuItem
            // 
            this.miEventsToolStripMenuItem.Name = "miEventsToolStripMenuItem";
            this.miEventsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.miEventsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.miEventsToolStripMenuItem.Text = "Занятия";
            this.miEventsToolStripMenuItem.Click += new System.EventHandler(this.miEventsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
            // 
            // miEditTimeTable
            // 
            this.miEditTimeTable.Name = "miEditTimeTable";
            this.miEditTimeTable.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.miEditTimeTable.Size = new System.Drawing.Size(176, 22);
            this.miEditTimeTable.Text = "Расписание";
            // 
            // miOptions
            // 
            this.miOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSettings,
            this.toolStripSeparator6,
            this.miSchedulingParameters});
            this.miOptions.Name = "miOptions";
            this.miOptions.Size = new System.Drawing.Size(56, 20);
            this.miOptions.Text = "Опции";
            // 
            // miSettings
            // 
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(278, 22);
            this.miSettings.Text = "Настройки";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(275, 6);
            // 
            // miSchedulingParameters
            // 
            this.miSchedulingParameters.Name = "miSchedulingParameters";
            this.miSchedulingParameters.Size = new System.Drawing.Size(278, 22);
            this.miSchedulingParameters.Text = "Параметры составления расписания";
            // 
            // miHelp
            // 
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.справкаToolStripMenuItem1,
            this.toolStripSeparator2,
            this.miAbout});
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(65, 20);
            this.miHelp.Text = "Справка";
            // 
            // справкаToolStripMenuItem1
            // 
            this.справкаToolStripMenuItem1.Name = "справкаToolStripMenuItem1";
            this.справкаToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F1)));
            this.справкаToolStripMenuItem1.Size = new System.Drawing.Size(171, 22);
            this.справкаToolStripMenuItem1.Text = "Справка";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(168, 6);
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.miAbout.Size = new System.Drawing.Size(171, 22);
            this.miAbout.Text = "О программе";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.timeTablesList);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.timeTableDetails);
            this.splitContainer.Size = new System.Drawing.Size(471, 414);
            this.splitContainer.SplitterDistance = 155;
            this.splitContainer.TabIndex = 1;
            // 
            // timeTablesList
            // 
            this.timeTablesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.createdTimeTablesHeader});
            this.timeTablesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeTablesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.timeTablesList.LabelEdit = true;
            this.timeTablesList.Location = new System.Drawing.Point(0, 0);
            this.timeTablesList.MultiSelect = false;
            this.timeTablesList.Name = "timeTablesList";
            this.timeTablesList.Size = new System.Drawing.Size(155, 414);
            this.timeTablesList.TabIndex = 0;
            this.timeTablesList.UseCompatibleStateImageBehavior = false;
            this.timeTablesList.View = System.Windows.Forms.View.Details;
            this.timeTablesList.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.timeTablesList_AfterLabelEdit);
            this.timeTablesList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.timeTablesList_ItemSelectionChanged);
            this.timeTablesList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.timeTablesList_KeyDown);
            // 
            // createdTimeTablesHeader
            // 
            this.createdTimeTablesHeader.Text = "Варианты расписаний";
            this.createdTimeTablesHeader.Width = 150;
            // 
            // timeTableDetails
            // 
            this.timeTableDetails.Controls.Add(this.tabByGroups);
            this.timeTableDetails.Controls.Add(this.tabByLecturers);
            this.timeTableDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeTableDetails.Location = new System.Drawing.Point(0, 0);
            this.timeTableDetails.Name = "timeTableDetails";
            this.timeTableDetails.SelectedIndex = 0;
            this.timeTableDetails.Size = new System.Drawing.Size(312, 414);
            this.timeTableDetails.TabIndex = 0;
            // 
            // tabByGroups
            // 
            this.tabByGroups.AutoScroll = true;
            this.tabByGroups.Controls.Add(this.lblGroups);
            this.tabByGroups.Controls.Add(this.cbxGroups);
            this.tabByGroups.Location = new System.Drawing.Point(4, 22);
            this.tabByGroups.Name = "tabByGroups";
            this.tabByGroups.Padding = new System.Windows.Forms.Padding(3);
            this.tabByGroups.Size = new System.Drawing.Size(304, 388);
            this.tabByGroups.TabIndex = 0;
            this.tabByGroups.Text = "По группам";
            this.tabByGroups.UseVisualStyleBackColor = true;
            // 
            // lblGroups
            // 
            this.lblGroups.AutoSize = true;
            this.lblGroups.Location = new System.Drawing.Point(6, 9);
            this.lblGroups.Name = "lblGroups";
            this.lblGroups.Size = new System.Drawing.Size(45, 13);
            this.lblGroups.TabIndex = 1;
            this.lblGroups.Text = "Группа:";
            // 
            // cbxGroups
            // 
            this.cbxGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGroups.FormattingEnabled = true;
            this.cbxGroups.Location = new System.Drawing.Point(57, 6);
            this.cbxGroups.Name = "cbxGroups";
            this.cbxGroups.Size = new System.Drawing.Size(121, 21);
            this.cbxGroups.TabIndex = 0;
            // 
            // tabByLecturers
            // 
            this.tabByLecturers.AutoScroll = true;
            this.tabByLecturers.Controls.Add(this.lblLecturer);
            this.tabByLecturers.Controls.Add(this.cbxLecturers);
            this.tabByLecturers.Location = new System.Drawing.Point(4, 22);
            this.tabByLecturers.Name = "tabByLecturers";
            this.tabByLecturers.Padding = new System.Windows.Forms.Padding(3);
            this.tabByLecturers.Size = new System.Drawing.Size(304, 388);
            this.tabByLecturers.TabIndex = 1;
            this.tabByLecturers.Text = "По преподавателям";
            this.tabByLecturers.UseVisualStyleBackColor = true;
            // 
            // lblLecturer
            // 
            this.lblLecturer.AutoSize = true;
            this.lblLecturer.Location = new System.Drawing.Point(6, 9);
            this.lblLecturer.Name = "lblLecturer";
            this.lblLecturer.Size = new System.Drawing.Size(89, 13);
            this.lblLecturer.TabIndex = 3;
            this.lblLecturer.Text = "Преподаватель:";
            // 
            // cbxLecturers
            // 
            this.cbxLecturers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLecturers.FormattingEnabled = true;
            this.cbxLecturers.Location = new System.Drawing.Point(101, 6);
            this.cbxLecturers.Name = "cbxLecturers";
            this.cbxLecturers.Size = new System.Drawing.Size(121, 21);
            this.cbxLecturers.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 438);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.Icon = global::SchedulerProject.Properties.Resources.AppIcon;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Составление расписаний";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.timeTableDetails.ResumeLayout(false);
            this.tabByGroups.ResumeLayout(false);
            this.tabByGroups.PerformLayout();
            this.tabByLecturers.ResumeLayout(false);
            this.tabByLecturers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView timeTablesList;
        private System.Windows.Forms.ColumnHeader createdTimeTablesHeader;
        private System.Windows.Forms.TabControl timeTableDetails;
        private System.Windows.Forms.TabPage tabByGroups;
        private System.Windows.Forms.TabPage tabByLecturers;
        private System.Windows.Forms.ToolStripMenuItem miFile;
        private System.Windows.Forms.ToolStripMenuItem miLoadData;
        private System.Windows.Forms.ToolStripMenuItem miSaveData;
        private System.Windows.Forms.ToolStripMenuItem miShedule;
        private System.Windows.Forms.ToolStripMenuItem miExport;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miEditTimeTable;
        private System.Windows.Forms.ToolStripMenuItem miHelp;
        private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem miOpenTimeTable;
        private System.Windows.Forms.ToolStripMenuItem miSaveTimeTable;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem miEditData;
        private System.Windows.Forms.ToolStripMenuItem miEditGroups;
        private System.Windows.Forms.ToolStripMenuItem miEditSubjects;
        private System.Windows.Forms.ToolStripMenuItem miEditRooms;
        private System.Windows.Forms.ToolStripMenuItem miEditLecturers;
        private System.Windows.Forms.ToolStripMenuItem miOptions;
        private System.Windows.Forms.ToolStripMenuItem miSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem miSchedulingParameters;
        private System.Windows.Forms.OpenFileDialog openDataFileDialog;
        private System.Windows.Forms.SaveFileDialog saveDataFileDialog;
        private System.Windows.Forms.ToolStripMenuItem miEventsToolStripMenuItem;
        private System.Windows.Forms.Label lblGroups;
        private System.Windows.Forms.ComboBox cbxGroups;
        private System.Windows.Forms.Label lblLecturer;
        private System.Windows.Forms.ComboBox cbxLecturers;
    }
}
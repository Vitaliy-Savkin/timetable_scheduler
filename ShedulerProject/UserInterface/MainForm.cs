using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SchedulerProject.Core;

namespace SchedulerProject.UserInterface
{
    public partial class MainForm : Form
    {
        TimeTableData openedData;

        EditDataForm editDataForm;

        string openedFile;
        string openedFileName;
        string openedFileDirectory;
        private bool dataChanged;
        TimeTable selectedTimeTable;

        TimeTableView byGroupsTimeTableView, byLecturersTimeTableView;

        public MainForm()
        {
            InitializeComponent();

            byGroupsTimeTableView = MakeTimeTableView();
            byLecturersTimeTableView = MakeTimeTableView();

            tabByGroups.Controls.Add(byGroupsTimeTableView);
            tabByLecturers.Controls.Add(byLecturersTimeTableView);

            OpenedData = TimeTableData.MakeEmpty();

            OpenedFile = "Untitled.xml";
            SelectedTimeTable = null;
        }

        TimeTableView MakeTimeTableView()
        {
            return new TimeTableView(new Size(140, 140)) { Location = new Point(6, 28) };
        }

        void ValidateTimeTablesList()
        {
            foreach (ListViewItem item in
                timeTablesList.Items.OfType<ListViewItem>()
                                    .Where(i => (i.Tag as TimeTable).Data.Id != OpenedData.Id))
            {
                // TODO: disable selection of item or move item no special group or do something else
                item.Remove();
            }
        }

        TimeTableData OpenedData
        {
            get { return openedData; }
            set
            {
                openedData = value;
                PopulateListBoxes();
                ValidateTimeTablesList();
            }
        }

        void PopulateListBoxes()
        {
            cbxGroups.Items.Clear();
            cbxGroups.Items.AddRange(OpenedData.Groups.OrderBy(g => g.Name).ToArray());
            cbxGroups.SelectedIndexChanged += (s, e) =>
            {
                var selectedGroup = cbxGroups.SelectedItem as Group;
                if (selectedGroup != null)
                {
                    byGroupsTimeTableView.EventsFilter = ev => ev.Groups.Contains(selectedGroup.Id);
                }
            };
            if (cbxGroups.Items.Count > 0)
                cbxGroups.SelectedIndex = 0;

            cbxLecturers.Items.Clear();
            cbxLecturers.Items.AddRange(OpenedData.Lecturers.OrderBy(l => l.Name).ToArray());
            cbxLecturers.SelectedIndexChanged += (s, e) =>
            {
                var selectedLecturer = cbxLecturers.SelectedItem as Lecturer;
                if (selectedLecturer != null)
                {
                    byLecturersTimeTableView.EventsFilter = ev => ev.LecturerId == selectedLecturer.Id;
                }
            };
            if (cbxLecturers.Items.Count > 0)
                cbxLecturers.SelectedIndex = 0;
        }

        TimeTable SelectedTimeTable
        { 
            get { return selectedTimeTable; }            
            set
            {
                selectedTimeTable = value;
                OnSelectedTimeTableChanged(value);
            }
        }

        void OnSelectedTimeTableChanged(TimeTable selectedTimeTable)
        {
            var empty = selectedTimeTable == null;
            tabByLecturers.Enabled = tabByGroups.Enabled = miSaveTimeTable.Enabled = !empty;
            byGroupsTimeTableView.TimeTable = byLecturersTimeTableView.TimeTable = selectedTimeTable;
        }

        private string OpenedFile
        {
            get { return openedFile; }
            set 
            { 
                openedFile = value;
                openedFileName = System.IO.Path.GetFileName(openedFile);
                openedFileDirectory = System.IO.Path.GetDirectoryName(openedFile);
                Text = WindowTitle; 
            }
        }

        private bool OpenedDataChanged
        {
            get { return dataChanged; }
            set { dataChanged = value; Text = WindowTitle; }
        }

        private string WindowTitle
        {
            get
            {
                return "Составление расписания - " + openedFile + (dataChanged ? " *" : "");
            }
        }

        private void miLoadData_Click(object sender, EventArgs e)
        {
            openDataFileDialog.InitialDirectory = openedFileDirectory;
            openDataFileDialog.FileName = "";
            openDataFileDialog.Filter = "Файлы данных расписания|*.ttd";
            if (openDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    OpenedData = TimeTableData.LoadFromXml(openDataFileDialog.FileName);
                    OpenedFile = openDataFileDialog.FileName;
                    OpenedDataChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при загрузке файла", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Cursor = Cursors.Arrow;
            }
        }

        private void miSaveData_Click(object sender, EventArgs e)
        {
            saveDataFileDialog.InitialDirectory = openedFileDirectory;
            saveDataFileDialog.FileName = openedFileName;
            saveDataFileDialog.Filter = "Файлы данных расписания|*.ttd";
            if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    OpenedData.SaveToXml(saveDataFileDialog.FileName);
                    OpenedDataChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при сохранении файла",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Cursor = Cursors.Arrow;
            }
        }

        private void miOpenTimeTable_Click(object sender, EventArgs e)
        {
            openDataFileDialog.InitialDirectory = "";
            openDataFileDialog.FileName = "";
            openDataFileDialog.Filter = "Файлы расписаний|*.crc";
            if (openDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                TimeTable openedTimeTable = null;
                try
                {
                    openedTimeTable = TimeTable.LoadFromXml(OpenedData, openDataFileDialog.FileName);
                }
                catch (IdMismatchException)
                {
                    MessageBox.Show("Этот файл расписания не соответствует загруженным данным", "Ошибка при загрузке файла",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при загрузке файла",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (openedTimeTable != null)
                    AddTimeTable(openedTimeTable);
                Cursor = Cursors.Arrow;
            }
        }

        private void miSaveTimeTable_Click(object sender, EventArgs e)
        {
            saveDataFileDialog.InitialDirectory = "";
            saveDataFileDialog.FileName = SelectedTimeTable.Name + ".crc";
            saveDataFileDialog.Filter = "Файлы расписаний|*.crc";
            if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    SelectedTimeTable.SaveToXml(saveDataFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при сохранении файла",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Cursor = Cursors.Arrow;
            }
        }

        private void miEditGroups_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Groups);
        }

        private void miEditSubjects_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Subjects);
        }

        private void miEditRooms_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Rooms);
        }

        private void miEditLecturers_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Lecturers);
        }

        private void miEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Events);
        }

        private void ShowEditDataForm(EditDataForm.Tab startTab)
        {
            try
            {
                editDataForm = new EditDataForm(OpenedData);
                editDataForm.SetActiveTab(startTab);
                editDataForm.ShowDialog(this);
                if (editDataForm.DataChanged)
                {
                    OpenedDataChanged = true;
                    OpenedData = editDataForm.NewData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ":\n" + ex.StackTrace, "Ошибка при работе с данными",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        void AddTimeTable(TimeTable timeTable)
        {
            var item = new ListViewItem(timeTable.Name) { Tag = timeTable };
            timeTablesList.Items.Add(item);
            item.Selected = true;
        }

        private void miShedule_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var timeTable = Scheduler.Schedule(OpenedData);
            sw.Stop();
            timeTable.Name = "Безымянное расписание";
            AddTimeTable(timeTable);
            Cursor = Cursors.Arrow;
            var text = Enumerable.Range(1, 2)
                                 .Select(i =>
                                 {
                                    var msg = timeTable.WeeklyAssignments(i)
                                                       .Where(wa => wa != null && wa.Conflicts != 0)
                                                       .Select(wa => wa.ToString(timeTable.Data) + " - " + wa.Conflicts)
                                                       .Aggregate(string.Empty, (acc, curr) => acc + curr + "\n");
                                    return msg == string.Empty ? msg : string.Format("Конфлиты на {0} неделе:\n{1}", i, msg);
                                 })
                                 .Aggregate(string.Empty, (acc, curr) => acc + curr);

            if (text == string.Empty)
                text = "Расписание составлено успешно";
            else
                text = "Расписание составлено с конфликтами:\n" + text;

            MessageBox.Show(this, text, 
                            string.Format("Расписание составлено ({0} ms)", sw.ElapsedMilliseconds),
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
        }

        private void timeTablesList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item.Selected)
            {
                SelectedTimeTable = e.Item.Tag as TimeTable;
                e.Item.BackColor = SystemColors.MenuHighlight;
                e.Item.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                SelectedTimeTable = null;
                e.Item.BackColor = Color.White;
                e.Item.ForeColor = Color.Black;
            }
        }

        private void timeTablesList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            var listView = sender as ListView;
            (listView.Items[e.Item].Tag as TimeTable).Name = e.Label;
        }

        private void timeTablesList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var listView = sender as ListView;
                foreach (ListViewItem item in listView.SelectedItems)
                    item.Remove();
                SelectedTimeTable = null;
                if (listView.Items.Count > 0)
                    listView.Items[0].Selected = true;
            }                
        }
    }
}

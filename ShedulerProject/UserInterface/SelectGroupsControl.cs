using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SchedulerProject.Core;

namespace SchedulerProject.UserInterface
{
    public partial class SelectGroupsControl : UserControl
    {
        Dictionary<int, Group[]> groupsByCourse;

        void AddCourseGroups(int course, Group[] groups, int x, out int y)
        {
            y = 0;
            var courseLabel = new Label()
            {
                Text = course + " курс",
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(x, y)
            };
            Controls.Add(courseLabel);
            y += courseLabel.Height;
            foreach (var group in groups)
            {
                var groupCheckBox = new CheckBox()
                {
                    Text = group.Name,
                    Location = new Point(x, y),
                    Tag = group,
                    Checked = selectedGroups.Contains(group),
                    Size = new Size(100, 20)
                };
                
                groupCheckBox.CheckedChanged += (s, e) =>
                {
                    var checkBox = s as CheckBox;
                    var gr = checkBox.Tag as Group;
                    if (checkBox.Checked && !selectedGroups.Contains(gr))
                        selectedGroups.Add(gr);
                    else if(!checkBox.Checked && selectedGroups.Contains(gr)) 
                        selectedGroups.Remove(gr);
                };

                Controls.Add(groupCheckBox);

                y += groupCheckBox.Height;
            }
        }

        List<Group> selectedGroups = new List<Group>();

        public IEnumerable<Group> AvailableGroups
        {
            get { return groupsByCourse.SelectMany(g => g.Value); }
            set
            {
                Controls.Clear();
                groupsByCourse = value.GroupBy(gr => gr.Course)
                                      .OrderBy(g => g.Key)
                                      .ToDictionary(g => g.Key, g => g.ToArray());

                if (groupsByCourse.Any())
                {
                    int x = 10;
                    int maxY = 0;
                    foreach (var pair in groupsByCourse)
                    {
                        int y;
                        AddCourseGroups(pair.Key, pair.Value, x, out y);
                        x += 100;
                        if (y > maxY)
                            maxY = y;
                    }
                    Size = new Size(x, maxY);
                }
                else
                {
                    Size sz = new Size(200, 60);
                    Controls.Add(new Label() 
                    { 
                        Text = "Список групп пуст",
                        Size = sz, 
                        TextAlign = ContentAlignment.MiddleCenter
                    });
                    Size = sz;
                }
            }
        }

        public IEnumerable<Group> SelectedGroups
        {
            get { return selectedGroups; }
            set 
            { 
                selectedGroups = value.ToList();
                foreach (var checkBox in Controls.OfType<CheckBox>())
                {
                    var gr = checkBox.Tag as Group;
                    checkBox.Checked = selectedGroups.Contains(gr);
                }
            }
        }
    }
}

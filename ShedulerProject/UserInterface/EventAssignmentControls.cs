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
    public class EventAssignmentControl : TimeSlotControl<EventAssignmentControl>
    {
        public EventAssignmentControl(TimeTableData problemData, 
                                      WeeklyEventAssignment firstWeekAssignment,
                                      WeeklyEventAssignment secondWeekAssignment)
        {
            data = problemData;
            SelectedFirstWeekAssignment = firstWeekAssignment;
            SelectedSecondWeekAssignment = secondWeekAssignment;
            BackColor = Color.LightBlue;
            TimeSlot = (firstWeekAssignment ?? secondWeekAssignment).TimeSlot;
        }

        TimeTableData data;
        
        public WeeklyEventAssignment SelectedFirstWeekAssignment
        {
            get;
            set;
        }

        public WeeklyEventAssignment SelectedSecondWeekAssignment
        {
            get;
            set;
        }

        void DrawAssignment(WeeklyEventAssignment assignment, Graphics g, Rectangle rect)
        {
            if (assignment != null)
            {
                using (var brush = new SolidBrush(ForeColor))
                {
                    g.DrawString(assignment.ToString(data),
                                Font, brush,
                                rect,
                                new StringFormat()
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                });
                }
            }
            else
            {
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (SelectedFirstWeekAssignment != null && SelectedSecondWeekAssignment != null && 
                SelectedFirstWeekAssignment.ToString(data) == SelectedSecondWeekAssignment.ToString(data))
                DrawAssignment(SelectedFirstWeekAssignment, e.Graphics, new Rectangle(0, 0, Width, Height));
            else
            {
                DrawAssignment(SelectedFirstWeekAssignment, e.Graphics, new Rectangle(0, 0, Width, Height / 2));
                e.Graphics.DrawLine(Pens.White, 0, Height / 2, Width, Height / 2);
                DrawAssignment(SelectedSecondWeekAssignment, e.Graphics, new Rectangle(0, Height / 2, Width, Height / 2));
            }
            base.OnPaint(e);
        }
    }

    // for editing time table TimeTableView should be extended to EditableTimeTableView f.e.
    public class TimeTableView : TimeSlotsGrid<EventAssignmentControl> 
    {
        static int DAYS_COUNT = 6, SLOTS_COUNT = 5;

        TimeTable timeTable;
        public TimeTable TimeTable
        {
            get { return timeTable; }
            set
            {
                timeTable = value;
                FillSlots();
            }
        }

        public TimeTableView(Size slotSize)
            : base(DAYS_COUNT, SLOTS_COUNT)
        {
            TimeSlotControlSize = slotSize;
        }

        Predicate<Event> eventsFilter;
        public Predicate<Event> EventsFilter
        {
            get { return eventsFilter; }
            set
            {
                eventsFilter = value;
                FillSlots();
            }
        }

        protected void FillSlots()
        {
            SuspendLayout();
            ClearAllTimeSlots();
            if (eventsFilter != null && timeTable != null)
            {
                foreach (var assignment in timeTable.Assignments
                    .Where(a => eventsFilter(a.Event))
                    .SelectMany(a => new[] { a.FirstWeekAssignment, a.SecondWeekAssignment })
                    .Where(wa => wa != null)
                    .GroupBy(wa => wa.TimeSlot)
                    .Select(wa => new
                    {
                        First = wa.FirstOrDefault(w => w.Week == 1),
                        Second = wa.FirstOrDefault(w => w.Week == 2)
                    }))
                {
                    AddControlToSlot(new EventAssignmentControl(timeTable.Data, assignment.First, assignment.Second)
                    {
                        Size = TimeSlotControlSize
                    });
                }
            }
            ResumeLayout();
        }
    }
}
